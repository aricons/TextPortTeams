using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Security.Claims;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPort.Controllers
{
    public class GroupsController : Controller
    {
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            int branchId = Utilities.GetBranchIdFromClaim(ClaimsPrincipal.Current);
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
            string role = Utilities.GetRoleFromClaim(ClaimsPrincipal.Current);

            GroupsContainer gc = new GroupsContainer(branchId, accountId, role);

            return View(gc);
        }

        [HttpGet]
        public ActionResult GetGroupsForBranch(int bid)
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    List<GroupDDItem> groupsList = new List<GroupDDItem>();

                    foreach (Group g in da.GetGroupsForBranch(bid))
                    {
                        groupsList.Add(new GroupDDItem()
                        {
                            GroupId = g.GroupId.ToString(),
                            GroupName = g.GroupName
                        });
                    }

                    return Json(groupsList, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Add(int id)
        {
            int branchId = Utilities.GetBranchIdFromClaim(ClaimsPrincipal.Current);
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
            string role = Utilities.GetRoleFromClaim(ClaimsPrincipal.Current);

            if (id > 0)
            {
                branchId = id;
            }

            try
            {
                AddGroupContainer newGroupContainer = new AddGroupContainer(branchId, accountId, role);

                return PartialView("_AddGroup", newGroupContainer);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddGroupContainer newGroup)
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    da.AddGroup(newGroup);
                }

                int branchId = Utilities.GetBranchIdFromClaim(ClaimsPrincipal.Current);
                int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
                string role = Utilities.GetRoleFromClaim(ClaimsPrincipal.Current);
                return View("Index", new GroupsContainer(newGroup.BranchId, accountId, role));
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddMember(int id)
        {
            try
            {
                GroupMember newMember = new GroupMember()
                {
                    GroupId = id,
                    MemberName = string.Empty,
                    MobileNumber = string.Empty
                };

                return PartialView("_AddMember", newMember);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddMember(GroupMember newMember)
        {
            try
            {
                int groupId = newMember.GroupId;
                List<GroupMember> groupMembers = new List<GroupMember>();

                using (TextPortDA da = new TextPortDA())
                {
                    da.AddGroupMember(newMember);

                    groupMembers = da.GetMembersForGroup(newMember.GroupId);
                }

                return PartialView("_MembersList", groupMembers);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteMember(DeleteGroupMemberRequest request)
        {
            try
            {
                List<GroupMember> groupMembers = new List<GroupMember>();
                using (TextPortDA da = new TextPortDA())
                {
                    da.DeleteGroupMember(request.MemberId);
                    groupMembers = da.GetMembersForGroup(request.GroupId);
                }

                return PartialView("_MembersList", groupMembers);
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [HttpGet]
        public ActionResult GetMembers(int id)
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    return PartialView("_MembersList", da.GetMembersForGroup(id));
                }
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        public class GroupDDItem
        {
            public string GroupId { get; set; }
            public string GroupName { get; set; }
        }
    }
}