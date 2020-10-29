using System.Configuration;
using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class MessageNotification
    {

        public int AccountId { get; set; }
        public string UserName { get; set; }
        public string MobileNumber { get; set; }
        public int VirtualNumberId { get; set; }
        public string VirtualNumber { get; set; }
        public string MessageText { get; set; }
        public int SegmentCount { get; set; }
        public NotificationHtmls Htmls { get; set; }

        public MessageNotification()
        {
            this.AccountId = 0;
            this.UserName = string.Empty;
            this.MobileNumber = string.Empty;
            this.VirtualNumberId = 0;
            this.VirtualNumber = string.Empty;
            this.MessageText = string.Empty;
            this.SegmentCount = 1;
            this.Htmls = new NotificationHtmls();
        }

        public MessageNotification(Message msg)
        {
            this.AccountId = msg.Account.AccountId;
            // If the inbound message is directed to the free texting account, generate a unique 
            // username so any notifications get pushed to the correct SignalR client.
            if (msg.AccountId == Conversion.StringToIntOrZero(ConfigurationManager.AppSettings["FreeTextAccountId"]))
            {
                this.UserName = msg.SessionId;
            }
            else
            {
                this.UserName = msg.Account.UserName;
            }
            this.MobileNumber = msg.MobileNumber;
            this.VirtualNumberId = msg.VirtualNumberId;
            this.VirtualNumber = msg.DedicatedVirtualNumber.VirtualNumber;
            this.MessageText = msg.MessageText;
            this.SegmentCount = Utilities.GetSegmentCount(this.MessageText);
            this.Htmls = Rendering.RenderMessageIn(msg);
        }
    }

    public class NotificationHtmls
    {
        public string RecentHtml { get; set; }
        public string MessageHtml { get; set; }

        public NotificationHtmls()
        {
            this.RecentHtml = string.Empty;
            this.MessageHtml = string.Empty;
        }

        public NotificationHtmls(string rHtml, string mHtml)
        {
            this.RecentHtml = rHtml;
            this.MessageHtml = mHtml;
        }
    }
}