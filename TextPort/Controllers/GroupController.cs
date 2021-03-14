using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Security.Claims;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPort.Controllers
{
    public class GroupController : Controller
    {
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            int branchId = Utilities.GetBranchIdFromClaim(ClaimsPrincipal.Current);
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
            string role = Utilities.GetRoleFromClaim(ClaimsPrincipal.Current);

            if (accountId > 0 && branchId > 0)
            {
                GroupText groupText = new GroupText(branchId, accountId, role);
                return View(groupText);
            }

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(GroupText groupText)
        {
            List<GroupTextResult> results = new List<GroupTextResult>();
            int branchId = 0;

            if (groupText.GroupId > 0 && groupText.BranchId > 0)
            {
                branchId = groupText.BranchId;
                groupText.ProcessingState = ProcessingStates.ProcessedSuccessfully;

                using (TextPortDA da = new TextPortDA())
                {
                    List<GroupMember> members = da.GetMembersForGroup(groupText.GroupId);

                    foreach (GroupMember member in members)
                    {
                        if (!string.IsNullOrEmpty(member.MobileNumber))
                        {
                            Message message = new Message(groupText.AccountId, (byte)MessageTypes.Group, groupText.VirtualNumberId, groupText.Message)
                            {
                                BranchId = branchId,
                                MobileNumber = member.MobileNumber
                            };

                            string result;
                            bool isStopped = false;
                            if (da.IsNumberStopped(message.MobileNumber))
                            {
                                message.MessageText = $"OPT-OUT: The recipient at number {message.MobileNumber} has opted out of text notifications. The message will not be sent.";
                                result = "Failed";
                                isStopped = true;
                            }
                            else
                            {
                                decimal newBalance = 0;
                                da.InsertMessage(message, ref newBalance);

                                result = (message.Send()) ? "Success" : "Failed";
                            }
                            results.Add(new GroupTextResult(member.MemberName, member.MobileNumber, result, isStopped));
                        }
                    }

                    groupText.ResultsList = results;

                    // Repopulate the drop-downs.
                    groupText.GroupsList = da.GetGroupsForBranch(branchId);
                    groupText.VirtualNumbers = da.GetNumbersForBranch(branchId, false);
                    groupText.Branch = da.GetBranchByBranchId(branchId);

                    if (groupText.Role == "Administrative User" || groupText.Role == "Branch Manager")
                    {
                        groupText.Branches = groupText.Branches = da.GetAllBranches();
                    }
                    else
                    {
                        groupText.Branches = new List<Branch>() { da.GetBranchByBranchId(branchId) };
                    };
                }
            }

            return View(groupText);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetGroupsForBranch(int bid)
        {
            try
            {
                using (TextPortDA da = new TextPortDA())
                {
                    List<Group> groupsList = da.GetGroupsForBranch(bid);
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
        public ActionResult Getmemberlist(int id)
        {
            List<GroupMember> groupMembers = new List<GroupMember>();

            using (TextPortDA da = new TextPortDA())
            {
                groupMembers = da.GetMembersForGroup(id);
            }

            return PartialView("_MemberList", groupMembers);
        }
    }
}