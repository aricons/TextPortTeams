using System;
using System.ComponentModel.DataAnnotations;

using TextPortCore.Helpers;
using core = TextPortCore.Models;
using intCommon = TextPortCore.Integrations.Common;

namespace TextPortCore.Models.API
{
    public class MessageEvent
    {
        [Required]
        public string EventType { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public int MessageId { get; set; }

        [Required]
        public string From { get; set; }

        [Required]
        public string To { get; set; }

        [Required]
        public decimal Cost { get; set; }

        [Required]
        public Message Message { get; set; }

        public string Notifications { get; set; }

        public MessageEvent()
        {
            this.EventType = string.Empty;
            this.Time = DateTime.Now;
            this.MessageId = 0;
            this.From = string.Empty;
            this.To = string.Empty;
            this.Cost = 0;
            this.Message = new Message();
            this.Notifications = string.Empty;
        }

        public MessageEvent(core.Message message, EventTypes eventType)
        {
            this.EventType = getAPIEventCodeFromEventType(eventType);
            this.Time = DateTime.Now;
            this.MessageId = message.MessageId;
            this.From = message.MobileNumber;
            this.To = message?.DedicatedVirtualNumber?.VirtualNumber;
            this.Cost = (message.CustomerCost != null) ? (decimal)message.CustomerCost : 0.015M;
            this.Message = new Message(message);
            switch (eventType)
            {
                case EventTypes.MessageFailed:
                    this.Notifications = $"Delivery to {message.MobileNumber} falied.";
                    break;
                default:
                    this.Notifications = $"Inbound message received from {message.MobileNumber}.";
                    break;
            }
        }

        public MessageEvent(intCommon.IntegrationMessageIn integrationMessageIn, core.Message originalMessage)
        {
            this.EventType = getAPIEventCodeFromEventType(integrationMessageIn.EventType);
            this.Time = DateTime.Now;
            this.MessageId = originalMessage.MessageId;
            this.From = integrationMessageIn.From;
            this.To = integrationMessageIn.To;
            this.Cost = originalMessage.CustomerCost ?? 0;
            if (this.EventType == "message-delivered")
            {
                this.Message = new Message(originalMessage);
            }
            else
            {
                this.Message = new Message(integrationMessageIn);
            }
            this.Notifications = $"{integrationMessageIn.EventType} received from {integrationMessageIn.To}.";
        }

        private string getAPIEventCodeFromEventType(EventTypes eventType)
        {
            switch (eventType)
            {
                case EventTypes.MessageReceived:
                    return "message-received";

                case Helpers.EventTypes.MessageDelivered:
                    return "message-delivered";

                case Helpers.EventTypes.MessageFailed:
                    return "message-failed";
            }

            return "unknown-event";
        }
    }
}