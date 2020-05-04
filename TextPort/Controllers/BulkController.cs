using System;
using System.IO;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;

using TextPort.Helpers;
using TextPortCore.Models;
using TextPortCore.Data;
using TextPortCore.Helpers;

//using PagedList;

namespace TextPort.Controllers
{
    public class BulkController : Controller
    {
        private int inboxPageSize = 10;

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult Index()
        {
            string accountIdStr = ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            int accountId = Convert.ToInt32(accountIdStr);

            if (accountId > 0)
            {
                BulkMessages bulk = new BulkMessages(accountId, 10);
                return View(bulk);
            }

            return View();
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult Index(BulkMessages messageData)
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
            decimal remainingBalance = 0;
            MessageTypes messageType = (messageData.SubmitType == "UPLOAD") ? MessageTypes.BulkUpload : MessageTypes.Bulk;

            try
            {
                if (accountId > 0)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        remainingBalance = da.GetAccountBalance(accountId);

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
                                                Message bulkMessage = new Message(message, messageType, accountId, messageData.VirtualNumberId, string.Empty);

                                                if (!da.NumberIsBlocked(bulkMessage.MobileNumber, MessageDirection.Outbound))
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
                                                    message.ProcessingResult = $"BLOCKED: The recipient at number {bulkMessage.MobileNumber} has reported abuse from this account abuse and requested their number be blocked. TextPort does not condone the exchange of abusive, harrassing or defamatory messages.";
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
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("BulkController.Index_POST", ex);
            }

            return View(messageData);
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult Inbox()
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);
            InboxContainer inboxContainer = new InboxContainer();
            return View(inboxContainer);
        }

        [Authorize(Roles = "User")]
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

        [Authorize(Roles = "User")]
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

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult Upload()
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            if (accountId > 0)
            {
                string fileName = Request.Headers["X-File-Name"];

                Stream fileContent = Request.InputStream;

                var fileHandler = new FileHandling();
                DataTable dt = null;
                string connString = string.Empty;
                string fullPathName = string.Empty;

                if (fileHandler.SaveUploadFile(fileContent, accountId, fileName, ref fullPathName))
                {
                    string extension = System.IO.Path.GetExtension(fullPathName).ToLower();
                    if (extension == ".csv")
                    {
                        dt = UploadUtilities.ConvertCSVtoDataTable(fullPathName);
                    }
                    else if (extension.Trim() == ".xls")
                    {
                        //connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fullPathName + ";Extended Properties=\"Excel 8.0;HDR=No;IMEX=2\"";
                        connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullPathName + ";Extended Properties=\"Excel 8.0;HDR=No;IMEX=2\"";
                        dt = UploadUtilities.ConvertXSLXtoDataTable(fullPathName, connString);
                    }
                    else if (extension.Trim() == ".xlsx")
                    {
                        connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullPathName + ";Extended Properties=\"Excel 12.0;HDR=No;IMEX=2\"";
                        dt = UploadUtilities.ConvertXSLXtoDataTable(fullPathName, connString);
                    }

                    if (dt != null)
                    {
                        BulkMessages bm = new BulkMessages(accountId, 0);
                        bm.SubmitType = "UPLOAD";
                        bm.ProcessingState = "PENDING";
                        
                        using (TextPortDA da = new TextPortDA())
                        {
                            Account account = da.GetAccountById(accountId);
                            decimal balance = account.Balance;
                           
                            if (dt.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dt.Rows)
                                {
                                    string number = dr[0].ToString();
                                    string message = dr[1].ToString();

                                    BulkMessageItem messageItem = new BulkMessageItem()
                                    {
                                        Number = Utilities.StripLeading1(Utilities.StripNumber(number)),
                                        MessageText = message,
                                        SegmentCost = account.SMSSegmentCost
                                    };

                                    if (!messageItem.Validate(ref balance))
                                    {
                                        bm.ProcessingState = "UPLOAD FILE ERRORS";
                                    }

                                    bm.Messages.Add(messageItem);
                                }

                                Response.StatusCode = 200;
                                return PartialView("_MessageList", bm);
                            }
                        }
                    }
                }
            }
            Response.StatusCode = 404;
            return Json("Upload failed. The file format was invalid, or the file could not be read.");
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public PartialViewResult GridOps(BulkMessages messageData)
        {
            string accountIdStr = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            int accountId = Convert.ToInt32(accountIdStr);

            // The line below prevents the form data from being posted back and cached/replicated.
            ModelState.Clear();

            if (accountId > 0)
            {
                switch (messageData.SubmitOperation)
                {
                    case "ChangeGridRows":
                        List<BulkMessageItem> tempItems = messageData.Messages;
                        messageData.Messages = new List<BulkMessageItem>();

                        for (int x = 0; x < messageData.MessageLimit; x++)
                        {
                            if (x < tempItems.Count)
                            {
                                messageData.Messages.Add(tempItems[x]);
                            }
                            else
                            {
                                messageData.Messages.Add(new BulkMessageItem());
                            }
                        }
                        break;

                    case "SameMessage":
                        string firstMessage = messageData.Messages.FirstOrDefault().MessageText;
                        int i = 0;

                        foreach (BulkMessageItem item in messageData.Messages)
                        {
                            if (messageData.SameMessageToAllNumbers)
                            {
                                item.MessageText = firstMessage;
                            }
                            else
                            {
                                item.MessageText = (i > 0) ? string.Empty : item.MessageText;
                            }
                            i++;
                        }
                        break;
                }
            }
            return PartialView("_MessageList", messageData);
        }

        [AllowAnonymous]
        [HttpGet]
        [ActionName("upload-guidelines")]
        public ActionResult UploadGuidelines()
        {
            return View();
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