using System;

namespace TextPortCore.Models.API
{
    public class MessageEvent
    {
        public string EventType { get; set; }
        public DateTime Time { get; set; }
        public int MessageId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Cost { get; set; }
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
    }
}