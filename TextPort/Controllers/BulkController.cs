using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            string accountIdStr = ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            int accountId = Convert.ToInt32(accountIdStr);

            if (accountId > 0)
            {
                BulkMessages bulk = new BulkMessages(accountId, 10);
                return View(bulk);
            }

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(BulkMessages messageData)
        {
            string accountIdStr = ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            int accountId = Convert.ToInt32(accountIdStr);
            decimal balance = 0;

            try
            {
                if (accountId > 0)
                {
                    using (TextPortDA da = new TextPortDA())
                    {
                        balance = da.GetAccountBalance(accountId);

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
                                            if (balance > 0M)
                                            {
                                                Message bulkMessage = new Message(message, accountId, messageData.VirtualNumberId, string.Empty);

                                                if (da.InsertMessage(bulkMessage, ref balance) > 0)
                                                {
                                                    Cookies.Write("balance", balance.ToString(), 0);

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

        [Authorize]
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
    }
}