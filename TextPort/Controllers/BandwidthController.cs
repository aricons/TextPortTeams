using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using System.Web.Http.Results;

using TextPort.Hubs;
using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;
using TextPortCore.Integrations.Bandwidth;

namespace TextPort.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class BandwidthController : ApiController
    {
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
                        using (Bandwidth bw = new Bandwidth())
                        {
                            Message newMessage = bw.ProcessInboundMessage(bwMessage);
                            if (newMessage != null)
                            {
                                using (TextPortDA da = new TextPortDA())
                                {
                                    Account account = da.GetAccountById(newMessage.AccountId);
                                    if (account != null)
                                    {
                                        newMessage.Account = account;
                                        if (!String.IsNullOrEmpty(account.UserName))
                                        {
                                            MessageNotification notification = new MessageNotification(newMessage);

                                            using (HubFunctions hubFunctions = new HubFunctions())
                                            {
                                                hubFunctions.SendInboundMessageNotification(notification);
                                                if (account.EnableMobileForwarding && !string.IsNullOrEmpty(account.ForwardVnmessagesTo))
                                                {
                                                    decimal balance = account.Balance - Constants.BaseSMSMessageCost;
                                                    hubFunctions.SendBalanceUpdate(account.UserName, balance.ToString());
                                                }
                                            }
                                        }
                                    }
                                    return Ok();
                                }
                            }
                        }
                        break;

                    case "message-delivered":
                        using (Bandwidth bw = new Bandwidth())
                        {
                            bw.ProcessDeliveryReceipt(bwMessage);
                        }

                        DeliveryReceipt receipt = new DeliveryReceipt(bwMessage);
                        if (!string.IsNullOrEmpty(receipt.GatewayMessageId))
                        {
                            using (TextPortDA da = new TextPortDA())
                            {
                                Message originatingMessage = da.GetMessageByGatewayMessageId(receipt.GatewayMessageId);
                                if (originatingMessage != null)
                                {
                                    int messageId = originatingMessage.MessageId;
                                    Account account = da.GetAccountById(originatingMessage.AccountId);
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
                        }
                        return Ok();

                    case "message-failed":
                        using (Bandwidth bw = new Bandwidth())
                        {
                            Message newMessage = bw.ProcessInboundMessage(bwMessage);
                            if (newMessage != null)
                            {
                                using (TextPortDA da = new TextPortDA())
                                {
                                    Account account = da.GetAccountById(newMessage.AccountId);
                                    if (account != null)
                                    {
                                        newMessage.Account = account;
                                        if (!String.IsNullOrEmpty(account.UserName))
                                        {
                                            MessageNotification notification = new MessageNotification(newMessage);

                                            using (HubFunctions hubFunctions = new HubFunctions())
                                            {
                                                hubFunctions.SendInboundMessageNotification(notification);
                                                if (account.EnableMobileForwarding && !string.IsNullOrEmpty(account.ForwardVnmessagesTo))
                                                {
                                                    decimal balance = account.Balance - Constants.BaseSMSMessageCost;
                                                    hubFunctions.SendBalanceUpdate(account.UserName, balance.ToString());
                                                }
                                            }
                                        }
                                    }
                                    return Ok();
                                }
                            }
                        }
                        break;

                    default: // Log anything else
                        using (Bandwidth bw = new Bandwidth())
                        {
                            bw.ProcessDeliveryReceipt(bwMessage);
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
