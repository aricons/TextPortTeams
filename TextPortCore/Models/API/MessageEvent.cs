using System;
using System.ComponentModel.DataAnnotations;

using core = TextPortCore.Models;
using bw = TextPortCore.Integrations.Bandwidth;

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

        public MessageEvent(core.Message message, string eventType)
        {
            this.EventType = eventType;
            this.Time = DateTime.Now;
            this.MessageId = message.MessageId;
            this.From = message.MobileNumber;
            this.To = message.VirtualNumber;
            this.Cost = (message.CustomerCost != null) ? (decimal)message.CustomerCost : 0.015M;
            this.Message = new Message(message);
            this.Notifications = $"Inbound message received from {message.MobileNumber}.";
        }

        public MessageEvent(bw.BandwidthInboundMessage bwMessage)
        {
            this.EventType = bwMessage.type;
            this.Time = DateTime.Now;
            this.MessageId = 0;
            this.From = bwMessage.message.from;
            this.To = bwMessage.to;
            this.Cost = 0;
            this.Message = new Message(bwMessage);
            this.Notifications = $"{bwMessage.type} received from {bwMessage.to}.";
        }
    }
}