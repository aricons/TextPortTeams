using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Security.Claims;

using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPortTeams.Controllers
{
    public class GroupController : Controller
    {
        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult Index()
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            GroupText groupText = new GroupText(accountId);

            return View(groupText);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult Index(GroupText groupText)
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            List<GroupTextResult> results = new List<GroupTextResult>();

            if (groupText.GroupId > 0)
            {
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
                                MobileNumber = member.MobileNumber
                            };

                            string result = string.Empty;
                            if (da.NumberIsBlocked(message.MobileNumber, MessageDirection.Outbound))
                            {
                                message.MessageText = $"BLOCKED: The recipient at number {message.MobileNumber} has reported abuse from this account abuse and requested their number be blocked. TextPort does not condone the exchange of abusive, harrassing or defamatory messages.";
                                result = "Failed";
                            }
                            else
                            {
                                decimal newBalance = 0;
                                da.InsertMessage(message, ref newBalance);

                                result = (message.Send()) ? "Success" : "Failed";
                            }
                            results.Add(new GroupTextResult(member.MemberName, member.MobileNumber, result));
                        }
                    }

                    groupText.ResultsList = results;

                    // Repopulate the drop-downs.
                    groupText.GroupsList = da.GetGroupsList(groupText.AccountId);

                    List<DedicatedVirtualNumber> dvns = da.GetNumbersForAccount(accountId, false);
                    foreach (DedicatedVirtualNumber dvn in dvns)
                    {
                        groupText.VirtualNumbers.Add(new SelectListItem()
                        {
                            Value = dvn.VirtualNumberId.ToString(),
                            Text = dvn.NumberDisplayFormat
                        });
                    };
                }
            }

            return View(groupText);
        }

        [Authorize(Roles = "User")]
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