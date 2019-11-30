using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TextPortCore.Integrations.Bandwidth;

namespace TextPortCore.Models
{
    public class DeliveryReceipt
    {
        public int MessageId { get; set; }
        public string GatewayMessageId { get; set; }
        public int SegmentCount { get; set; }
        public string Text { get; set; }

        public DeliveryReceipt()
        {
            this.MessageId = 0;
            this.GatewayMessageId = string.Empty;
            this.SegmentCount = 1;
            this.Text = string.Empty;
        }

        public DeliveryReceipt(BandwidthInboundMessage bwMessage)
        {
            this.MessageId = 0;
            this.Text = "Delivered";

            if (bwMessage != null)
            {
                if (bwMessage.message != null)
                {
                    this.GatewayMessageId = bwMessage.message.id;
                    this.SegmentCount = (bwMessage.message.segmentCount > 0) ? bwMessage.message.segmentCount : 1;
                }
            }
        }
    }
}
