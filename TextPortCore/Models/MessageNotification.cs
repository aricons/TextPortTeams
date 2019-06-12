using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public NotificationHtmls Htmls { get; set; }

        public MessageNotification()
        {
            this.AccountId = 0;
            this.UserName = string.Empty;
            this.MobileNumber = string.Empty;
            this.VirtualNumberId = 0;
            this.VirtualNumber = string.Empty;
            this.MessageText = string.Empty;
            this.Htmls = new NotificationHtmls();
        }

        public MessageNotification(Message msg)
        {
            this.AccountId = msg.Account.AccountId;
            this.UserName = msg.Account.UserName;
            this.MobileNumber = msg.MobileNumber;
            this.VirtualNumberId = msg.VirtualNumberId;
            this.VirtualNumber = msg.VirtualNumber;
            this.MessageText = msg.MessageText;
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