﻿using System;
using System.Linq;
using System.Collections.Generic;
//using System.Web.Mvc;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using System.Web.Http.Results;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc.ViewEngines;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;

using TextPort.Hubs;
using TextPort.Helpers;
using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Models.Bandwidth;
using TextPortCore.Integrations.Bandwidth;

namespace TextPort.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class BandwidthController : ApiController
    {
        private readonly TextPortContext _context;

        public BandwidthController(TextPortContext context)
        {
            this._context = context;
        }

        //private readonly ICompositeViewEngine _viewEngine;
        //private readonly IHubContext<InboundHub> _hubContext;

        //public BandwidthController(TextPortContext context, ICompositeViewEngine viewEngine, IHubContext<InboundHub> hubContext)
        //public BandwidthController(TextPortContext context, IHubContext<InboundHub> hubContext)
        //public BandwidthController(TextPortContext context)
        //{
        //    //_context = context;
        //    //_viewEngine = viewEngine;
        //    //_hubContext = hubContext;
        //}

        //private readonly static Lazy<BandwidthController> _instance = new Lazy<BandwidthController>(() => new BandwidthController(GlobalHost.ConnectionManager.GetHubContext<InboundHub>()));

        //private BandwidthController()
        //{
        //}

        //private IHubContext _hubContext;

        //private BandwidthController()
        //{
        //    _hubContext = GlobalHost.ConnectionManager.GetHubContext<InboundHub>();
        //}

        [HttpGet]
        [ActionName("ping")]
        public string Ping()
        {
            return String.Format("Bandwidth API alive at {0}", DateTime.Now);
        }

        [HttpGet]
        //[Route("[action]/{value}")]
        [ActionName("pingval")]
        public string PingVal(string value)
        {
            return String.Format("PingVal received {0}", value);
        }

        [HttpPost]
        //[Route("[action]")]
        public IHttpActionResult MessageIn([FromBody] List<BandwidthInboundMessage> messages)
        {
            if (messages != null)
            {
                BandwidthInboundMessage bwMessage = messages.FirstOrDefault();

                switch (bwMessage.type)
                {
                    case "message-received":
                        using (Bandwidth bw = new Bandwidth(_context))
                        {
                            Message newMessage = bw.ProcessInboundMessage(bwMessage);
                            if (newMessage != null)
                            {
                                Account account = _context.Accounts.FirstOrDefault(x => x.AccountId == newMessage.AccountId);
                                if (account != null)
                                {
                                    if (!String.IsNullOrEmpty(account.UserName))
                                    {
                                        //MessageList msgList = new MessageList(newMessage);
                                        string messageHtml = Rendering.RenderMessageIn(newMessage);
                                        using (HubFunctions hubFunctions = new HubFunctions())
                                        {
                                            hubFunctions.SendInboundMessageNotification(account.UserName, newMessage.MobileNumber, newMessage.VirtualNumber, newMessage.MessageText, messageHtml);
                                        }
                                    }
                                }
                                return Ok();
                            }
                        }
                        break;

                    case "message-delivered":
                        using (Bandwidth bw = new Bandwidth(_context))
                        {
                            bw.ProcessDeliveryReceipt(bwMessage);
                        }

                        DeliveryReceipt receipt = new DeliveryReceipt(bwMessage);
                        if (!string.IsNullOrEmpty(receipt.GatewayMessageId))
                        {
                            Message originatingMessage = _context.Messages.FirstOrDefault(x => x.GatewayMessageId == receipt.GatewayMessageId);
                            if (originatingMessage != null)
                            {
                                int messageId = originatingMessage.MessageId;
                                Account account = _context.Accounts.FirstOrDefault(x => x.AccountId == originatingMessage.AccountId);
                                if (account != null)
                                {
                                    string messageHtml = @"<span class=""rcpt"">Delivered</span>";
                                    using (HubFunctions hubFunctions = new HubFunctions())
                                    {
                                        hubFunctions.SendDeliveryReceipt(account.UserName, messageId.ToString(), messageHtml);
                                    }
                                }
                            }
                        }
                        return Ok();
                }
            }
            return NotFound();
        }
    }
}

// Sample inbound message JSON
//{
//"eventType"     : "sms",
//"direction"     : "in",
//"messageId"     : "12345678",
//"messageUri"    : "https://api.catapult.inetwork.com/v1/users/{userId}/messages/{messageId}",
//"from"          : "19492339386",
//"to"            : "14154847947",
//"text"          : "This is a test message inbound 3",
//"applicationId" : "a-gp37trhojrbs2fgemvw5wzq",
//"time"          : "2019-02-19T18:06:06.076Z",
//"state"         : "received"
//}


// Sample delivery receipt JSON
// {  
//"type":"message-delivered",
//"time":"2019-05-22T05:23:42.714Z",
//"description":"ok",
//"to":"+19492339386",
//"message":{  
//   "id":"15585026221193k3rd3nspl7p7bwv",
//   "time":"2019-05-22T05:23:42.119Z",
//   "to":[
//      "+19492339386"
//   ],
//   "from":"+19493174450",
//   "text":"Test Out",
//   "applicationId":"5abf6fa7-5e0f-4f1c-828b-c01f0c9674c1",
//   "media":[

//   ],
//   "owner":"+19493174450",
//   "direction":"out",
//   "segmentCount":1
//}
//}
