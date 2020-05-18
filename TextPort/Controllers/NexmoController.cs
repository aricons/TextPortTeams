using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using TextPort.Hubs;
using TextPortCore.Data;
using TextPortCore.Models;
using TextPortCore.Helpers;
using TextPortCore.Integrations.Nexmo;
using TextPortCore.Integrations.Common;

namespace TextPort.Controllers
{
    public class NexmoController : ApiController
    {
        [HttpGet]
        [ActionName("ping")]
        public string Ping()
        {
            return String.Format("Nexmo API alive at {0}", DateTime.Now);
        }

        [HttpGet]
        [ActionName("pingval")]
        public string PingVal(string value)
        {
            return String.Format("PingVal received {0}", value);
        }

        [HttpPost]
        public IHttpActionResult MessageIn([FromBody] NexmoInboundMessage message)
        {
            Message newMessage = CarrierEventProcessing.ProcessInboundMessage(message);

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
                                    decimal balance = account.Balance - (Constants.BaseSMSSegmentCost * Utilities.GetSegmentCount(newMessage.MessageText));
                                    hubFunctions.SendBalanceUpdate(account.UserName, balance.ToString());
                                }
                            }
                        }
                    }
                    return Ok();
                }
            }
            return NotFound();
        }

        [HttpPost]
        public IHttpActionResult Delivery([FromBody] NexmoDeliveryReceipt nexmoReceipt)
        {
            DeliveryReceipt receipt = CarrierEventProcessing.ProcessDeliveryReceipt(nexmoReceipt);

            if (!string.IsNullOrEmpty(receipt.GatewayMessageId))
            {
                using (TextPortDA da = new TextPortDA())
                {
                    Message originalMessage = da.GetMessageByGatewayMessageId(receipt.GatewayMessageId);
                    if (originalMessage != null)
                    {
                        originalMessage.QueueStatus = (byte)QueueStatuses.DeliveryConfirmed;
                        originalMessage.Segments = receipt.SegmentCount;

                        applyChargesAndUpdateBalance(originalMessage);
                        da.SaveChanges();

                        string messageHtml = @"<div class='rcpt'><i class='fa fa-check'></i>Delivered</div>";
                        using (HubFunctions hubFunctions = new HubFunctions())
                        {
                            if (originalMessage.MessageType == (byte)MessageTypes.FreeTextSend && !String.IsNullOrEmpty(originalMessage.SessionId))
                            {
                                hubFunctions.SendDeliveryReceipt(getNotificationUserName(originalMessage), originalMessage.MessageId.ToString(), messageHtml);
                            }
                            else
                            {
                                hubFunctions.SendDeliveryReceipt(originalMessage.Account.UserName, originalMessage.MessageId.ToString(), messageHtml);
                            }
                        }
                    }
                }
            }
            return Ok();
        }


        // These need to be migrated to common processing.
        private bool applyChargesAndUpdateBalance(Message originalMessage)
        {
            // Deduct the message cost (base rate * segment count) from the account balance
            decimal messageRate;
            decimal messageCost;

            if (originalMessage.IsMMS)
            {
                messageRate = (originalMessage.Account.MMSSegmentCost > 0) ? originalMessage.Account.MMSSegmentCost : Constants.BaseMMSSegmentCost;
            }
            else
            {
                messageRate = (originalMessage.Account.SMSSegmentCost > 0) ? originalMessage.Account.SMSSegmentCost : Constants.BaseSMSSegmentCost;
            }

            messageCost = (messageRate * (int)originalMessage.Segments);
            originalMessage.CustomerCost = messageCost;
            originalMessage.Account.Balance -= messageCost;

            return true;
        }

        private string getNotificationUserName(Message msg)
        {
            if (msg.MessageType == (byte)MessageTypes.FreeTextSend && !string.IsNullOrEmpty(msg.SessionId))
            {
                return msg.SessionId;
            }
            else
            {
                return msg.Account.UserName;
            }
        }
    }
}
