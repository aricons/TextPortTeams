using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using TextPortCore.Models;
using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPort.Controllers
{
    public class BulkController : Controller
    {
        private TextPortContext _context;

        public BulkController(TextPortContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public ActionResult Main()
        {
            string accountIdStr = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            int accountId = Convert.ToInt32(accountIdStr);

            if (accountId > 0)
            {
                BulkMessages bulk = new BulkMessages(_context, accountId, 10);
                return View(bulk);
            }

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Main(BulkMessages messageData)
        {
            string accountIdStr = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("AccountId").Value;
            int accountId = Convert.ToInt32(accountIdStr);

            if (accountId > 0)
            {
                messageData.ProcessingState = "PROCESSED";
                if (messageData.Messages.Count > 0 && messageData.VirtualNumberId > 0)
                {
                    //BulkMessages results = new BulkMessages(_context, accountId, 0);
                    foreach (BulkMessageItem message in messageData.Messages)
                    {
                        if (!string.IsNullOrEmpty(message.Number))
                        {
                            if (!string.IsNullOrEmpty(message.MessageText))
                            {
                                Message bulkMessage = new Message(message, messageData.VirtualNumberId, string.Empty);

                                //if (bulkMessage.WriteQueueSemaphore())
                                if (true)
                                {
                                    message.ProcessingStatus = "OK";
                                    message.ProcessingResult = $"Messaage to {message.Number} queued successfully.";
                                }
                                else
                                {
                                    message.ProcessingStatus = "FAIL";
                                    message.ProcessingResult = $"Error queuing message to {message.Number}";
                                }
                            }
                        }
                    }
                    return View(messageData);
                }
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