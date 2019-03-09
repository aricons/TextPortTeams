﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;

using TextPortCore.Models;
using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPort.Controllers
{

    public class MessagesController : Controller
    {
        private readonly TextPortContext _context;

        public MessagesController(TextPortContext context)
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
                MessagingContainer mc = new MessagingContainer(_context, accountId);
                return View(mc);
            }
            else
            {
                // TODO: Redirect to error page if account ID <= 0;
            }

            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetRecentToNumbersForDedicatedVirtualNumber(int aid, int vnid)
        {
            try
            {
                using (TextPortDA da = new TextPortDA(_context))
                {
                    List<Recent> recentsList = new List<Recent>();
                    recentsList = da.GetRecentToNumbersForDedicatedVirtualNumber(aid, vnid);
                    recentsList.FirstOrDefault().IsActiveMessage = true;

                    return PartialView("_RecentsList", recentsList);
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
        public ActionResult GetMessagesForNumber(int aid, int vnid, string num)
        {
            try
            {
                using (TextPortDA da = new TextPortDA(_context))
                {
                    MessageList messageList = new MessageList();
                    messageList.Messages = da.GetMessagesForAccountAndRecipient(aid, vnid, num);

                    return PartialView("_MessageList", messageList);
                }
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
                return null;
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult SendMessage([System.Web.Http.FromBody] Message message)
        {
            try
            {
                string accountIdStr = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("AccountId").Value;
                message.AccountId = Convert.ToInt32(accountIdStr);
                message.TimeStamp = DateTime.UtcNow;
                message.Direction = (int)MessageDirection.Outbound;
                message.CarrierId = (int)Carriers.BandWidth;
                message.QueueStatus = (int)QueueStatus.Queued;
                message.Ipaddress = "0.0.0.0";
                message.RoutingType = "Bandwidth";
                message.SourceType = "Normal";

                if (message.AccountId > 0)
                {
                    using (TextPortDA da = new TextPortDA(_context))
                    {
                        if (da.InsertMessage(message) > 0)
                        {
                            MessageList messageList = new MessageList()
                            {
                                Messages = { message }
                            };

                            message.WriteQueueSemaphore();
                            _context.SaveChanges();

                            return PartialView("_MessageList", messageList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string bar = ex.Message;
            }
            return null;
        }

        [Authorize]
        [HttpGet]
        public ActionResult SignalRTest()
        {
            return View();
        }
    }
}