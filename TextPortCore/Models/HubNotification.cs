using System;

using TextPortCore.Integrations.Common;

namespace TextPortCore.Models
{
    public class HubNotification
    {
        public int MessageId { get; set; }
        public string HubClientId { get; set; }
        public string NotificationMessage { get; set; }

        public HubNotification()
        {
            this.MessageId = 0;
            this.HubClientId = string.Empty;
            this.NotificationMessage = string.Empty;
        }

        public HubNotification(string hubClientId, string notificationMessage)
        {
            this.MessageId = 0;
            this.HubClientId = string.Empty;
            this.NotificationMessage = string.Empty;
        }
    }
}
