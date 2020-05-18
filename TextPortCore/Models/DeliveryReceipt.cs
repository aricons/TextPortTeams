using System;

using TextPortCore.Integrations.Common;

namespace TextPortCore.Models
{
    public class DeliveryReceipt
    {
        public int MessageId { get; set; }
        public string GatewayMessageId { get; set; }
        public int SegmentCount { get; set; }
        public string Text { get; set; }
        public string UserNotificationMessage { get; set; }
        public HubNotification HubNotification { get; set; }

        public DeliveryReceipt()
        {
            this.MessageId = 0;
            this.GatewayMessageId = string.Empty;
            this.SegmentCount = 1;
            this.Text = string.Empty;
            this.UserNotificationMessage = string.Empty;
            this.HubNotification = new HubNotification();
        }

        public DeliveryReceipt(IntegrationMessageIn integrationReceipt, HubNotification hubNotification)
        {
            if (integrationReceipt != null)
            {
                switch (integrationReceipt.EventType)
                {
                    case Helpers.EventTypes.MessageDelivered:
                        this.Text = "Delivered";
                        break;
                    case Helpers.EventTypes.MessageFailed:
                        this.Text = "Failed";
                        break;
                }

                this.GatewayMessageId = integrationReceipt.CarrierMessageId;
                this.SegmentCount = (integrationReceipt.SegmentCount > 0) ? integrationReceipt.SegmentCount : 1;
                this.UserNotificationMessage = integrationReceipt.UserNotificationMessage;
                this.HubNotification = hubNotification;
            }
        }
    }
}
