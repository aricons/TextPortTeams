using System;
using System.IO;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using TextPort.Helpers;
using TextPortCore.Models;
using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPort.Controllers
{
    public class BulkController : Controller
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
                BulkMessages bulk = new BulkMessages(accountId, branchId, role, 0);
                return View(bulk);
            }

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(BulkMessages messageData)
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
            int branchId = messageData.BranchId;
            decimal remainingBalance = 0;
            MessageTypes messageType = (messageData.SubmitType == "UPLOAD") ? MessageTypes.BulkUpload : MessageTypes.Bulk;

            try
            {
                if (accountId > 0)
                {
                    messageData.Messages = parseNumbersList(messageData.NumbersList, messageData.MessageText);

                    if (messageData.Messages.Count > 0)
                    {
                        using (TextPortDA da = new TextPortDA())
                        {
                            messageData.Account = da.GetAccountById(accountId);
                            messageData.Branch = da.GetBranchByBranchId(branchId);
                            remainingBalance = messageData.Account.Balance;

                            messageData.ProcessingState = "PROCESSED";
                            if (messageData.Messages.Count > 0 && messageData.VirtualNumberId > 0)
                            {
                                foreach (BulkMessageItem message in messageData.Messages)
                                {
                                    if (!string.IsNullOrEmpty(message.Number))
                                    {
                                        if (Utilities.IsValidNumber(message.Number))
                                        {
                                            if (!string.IsNullOrEmpty(message.MessageText))
                                            {
                                                if (remainingBalance > 0M)
                                                {
                                                    Message bulkMessage = new Message(message, messageType, accountId, branchId, messageData.VirtualNumberId, string.Empty);

                                                    if (!da.IsNumberStopped(bulkMessage.MobileNumber))
                                                    {
                                                        if (da.InsertMessage(bulkMessage, ref remainingBalance) > 0)
                                                        {
                                                            Cookies.Write("balance", remainingBalance.ToString(), 0);

                                                            if (bulkMessage.Send())
                                                            {
                                                                message.ProcessingStatus = "OK";
                                                                message.ProcessingResult = $"Messaage to {message.Number} queued successfully.";
                                                            }
                                                            else
                                                            {
                                                                message.ProcessingStatus = "FAIL";
                                                                message.ProcessingResult = $"Error queuing message to {message.Number}.";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            message.ProcessingStatus = "FAIL";
                                                            message.ProcessingResult = $"Error saving message for {message.Number}.";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        message.ProcessingStatus = "FAIL";
                                                        message.ProcessingResult = $"OPT-OUT: The recipient at number {bulkMessage.MobileNumber} has opted out of text message notifications.";
                                                        message.MessageText = message.ProcessingResult;
                                                    }
                                                }
                                                else
                                                {
                                                    message.ProcessingStatus = "FAIL";
                                                    message.ProcessingResult = $"The account balance is exhausted.";
                                                }
                                            }
                                            else
                                            {
                                                message.ProcessingStatus = "FAIL";
                                                message.ProcessingResult = $"The message was empty.";
                                            }
                                        }
                                        else
                                        {
                                            message.ProcessingStatus = "FAIL";
                                            message.ProcessingResult = $"The number {message.Number} is invalid.";
                                        }
                                    }
                                }
                                return View(messageData);
                            }
                        }
                    }
                    else
                    {
                        messageData.ProcessingState = "FAIL";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("BulkController.Index_POST", ex);
            }

            return View(messageData);
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult VerifyNumbers(string numberslist)
        {
            int maxBatchSize = 300;

            if (!string.IsNullOrEmpty(numberslist))
            {
                List<string> badNumbers = new List<string>();

                List<string> nums = numberslist.Split(new string[] { ",", " ", ";", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (nums.Count > maxBatchSize)
                {
                    return Json($"Batch is too large. Split each submission into groups of {maxBatchSize} numbers or less.", JsonRequestBehavior.AllowGet);
                }

                foreach (string number in nums)
                {
                    if (!Utilities.IsValidNumber(number))
                    {
                        badNumbers.Add(number);
                    }
                }

                if (badNumbers.Count > 0)
                {
                    return Json($"These numbers are invalid: {string.Join(", ", badNumbers)}.", JsonRequestBehavior.AllowGet);
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        private List<BulkMessageItem> parseNumbersList(string numbers, string messageText)
        {
            List<BulkMessageItem> numbersList = new List<BulkMessageItem>();
            try
            {
                List<string> nums = numbers.Split(new string[] { ",", " ", ";", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (nums != null)
                {
                    if (nums.Count > 0)
                    {
                        foreach (string number in nums)
                        {
                            if (!string.IsNullOrEmpty(number))
                            {
                                numbersList.Add(new BulkMessageItem(number, messageText));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return numbersList;
        }

        [Authorize]
        [HttpGet]
        public ActionResult Inbox()
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
            InboxContainer inboxContainer = new InboxContainer();
            return View(inboxContainer);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetInboxPage(PagingParameters parameters)
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
            InboxContainer inboxContainer = new InboxContainer();

            if (accountId > 0)
            {
                using (TextPortDA da = new TextPortDA())
                {
                    inboxContainer = da.GetInboundMessagesForAccount(accountId, parameters);
                }
            }

            return Json(new
            {
                page = inboxContainer.CurrentPage,
                recordsPerPage = inboxContainer.RecordsPerPage,
                pageCount = inboxContainer.PageCount,
                recordLabel = inboxContainer.RecordLabel,
                sortOrder = inboxContainer.SortOrder,
                html = renderRazorViewToString("_InboundMessages", inboxContainer)
            });
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteInboxMessages(MessageIdList idsList)
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
            int messagesDeleted = 0;

            using (TextPortDA da = new TextPortDA())
            {
                messagesDeleted = da.FlagListOfMessagesAsDeleted(accountId, idsList);
            }

            return Json(new
            {
                messageCount = messagesDeleted,
                message = $"{messagesDeleted} messages successfully deleted."
            });
        }

        private string renderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}