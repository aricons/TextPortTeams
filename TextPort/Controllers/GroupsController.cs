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
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            GroupsContainer gc = new GroupsContainer(accountId);

            return View(gc);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Add()
        {
            try
            {
                Group newGroup = new Group()
                {
                    AccountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current)
                };

                return PartialView("_AddGroup", newGroup);
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
        public ActionResult Add(Group newGroup)
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    da.AddGroup(newGroup);
                }

                return View("Index", new GroupsContainer(newGroup));
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
    }
}