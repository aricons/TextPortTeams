using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using TextPortCore.Helpers;
using TextPortCore.Integrations.Bandwidth;
using TextPortCore.Integrations.Nexmo;

namespace TextPortCore.Integrations.Common
{
    public class IntegrationMessageIn
    {
        public Carriers CarrierId { get; set; }
        public string CarrierName { get; set; }
        public string CarrierMessageId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Message { get; set; }
        public int SegmentCount { get; set; }
        public EventTypes EventType { get; set; }
        public string ErrorCode { get; set; }
        public string Description { get; set; }
        public string DataFromVendor { get; set; }
        public string UserNotificationMessage { get; set; }
        public List<string> Media { get; set; }

        public IntegrationMessageIn()
        {
            this.CarrierId = Carriers.BandWidth;
            this.CarrierName = string.Empty;
            this.CarrierMessageId = string.Empty;
            this.TimeStamp = DateTime.MinValue;
            this.To = string.Empty;
            this.From = string.Empty;
            this.Message = string.Empty;
            this.SegmentCount = 0;
            this.EventType = EventTypes.Unknown;
            this.ErrorCode = string.Empty;
            this.Description = string.Empty;
            this.DataFromVendor = string.Empty;
            this.UserNotificationMessage = string.Empty;
            this.Media = new List<string>();

        }

        public IntegrationMessageIn(BandwidthInboundMessage bwMessage)
        {
            this.CarrierId = Carriers.BandWidth;
            this.CarrierName = "Bandwidth";
            if (bwMessage.message != null)
            {
                this.CarrierMessageId = bwMessage.message.id;
                this.TimeStamp = bwMessage.message.time;
                this.To = bwMessage.to.Replace("+", "");
                this.From = bwMessage.message.from.Replace("+", "");
                this.Message = bwMessage.message.text;
                this.SegmentCount = bwMessage.message.segmentCount;
                this.ErrorCode = bwMessage.errorCode.ToString();
                this.Description = bwMessage.description;
                this.DataFromVendor = JsonConvert.SerializeObject(bwMessage);
                this.UserNotificationMessage = string.Empty;

                switch (bwMessage.type)
                {
                    case "message-received":
                        this.EventType = EventTypes.MessageReceived;
                        break;
                    case "message-delivered":
                        this.EventType = EventTypes.MessageDelivered;
                        break;
                    case "message-failed":
                        this.EventType = EventTypes.MessageFailed;
                        break;
                }

                if (bwMessage.message?.media?.Count > 0)
                {
                    this.Media = bwMessage.message.media;
                }
            }
        }

        public IntegrationMessageIn(NexmoInboundMessage nexmoMessage)
        {
            this.CarrierId = Carriers.Nexmo;
            this.CarrierName = "Nexmo";
            this.CarrierMessageId = nexmoMessage.MessageId;
            this.TimeStamp = DateTime.UtcNow;
            this.To = nexmoMessage.To;
            this.From = nexmoMessage.Msisdn;
            this.Message = nexmoMessage.Text;
            this.SegmentCount = 0;
            this.ErrorCode = string.Empty;
            this.Description = "message-received";
            this.DataFromVendor = JsonConvert.SerializeObject(nexmoMessage);
            this.EventType = EventTypes.MessageReceived;
            this.UserNotificationMessage = string.Empty;
        }

        public IntegrationMessageIn(NexmoDeliveryReceipt nexmoReceipt)
        {
            this.CarrierId = Carriers.Nexmo;
            this.CarrierName = "Nexmo";
            this.CarrierMessageId = nexmoReceipt.MessageId;
            this.TimeStamp = DateTime.UtcNow;
            this.To = nexmoReceipt.To;
            this.From = nexmoReceipt.Msisdn;
            this.Message = string.Empty;
            this.SegmentCount = 0;
            this.ErrorCode = string.Empty;
            this.Description = "message-delivered";
            this.DataFromVendor = JsonConvert.SerializeObject(nexmoReceipt);
            this.EventType = EventTypes.MessageDelivered;
            this.UserNotificationMessage = string.Empty;
        }
    }
}
