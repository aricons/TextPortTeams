using System;
using System.Collections.Generic;
using System.Text;

namespace TextPortCore.Models.Bandwidth
{
    public class BandwidthInboundMessageV1
    {
        public string eventType { get; set; }
        public string direction { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string messageId { get; set; }
        public string messageUri { get; set; }
        public string text { get; set; }
        public string applicationId { get; set; }
        public string state { get; set; }
        public DateTime time { get; set; }

        // Default constructor
        public BandwidthInboundMessageV1()
        {
            this.eventType = String.Empty;
            this.direction = String.Empty;
            this.from = String.Empty;
            this.to = String.Empty;
            this.messageId = String.Empty;
            this.messageUri = String.Empty;
            this.text = String.Empty;
            this.applicationId = String.Empty;
            this.time = DateTime.MinValue;
        }

    }
}
