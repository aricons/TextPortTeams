using System;

namespace TextPortCore.Models
{
    public class DeleteMessageInfo
    {
        public string DeleteType { get; set; }
        public int AccountId { get; set; }
        public int VirtualNumberId { get; set; }
        public string VirtualNumber { get; set; }
        public string MobileNumber { get; set; }
        public int MessageId { get; set; }
        public string Title { get; set; }
        public string Prompt { get; set; }
        public string SubPrompt { get; set; }
        public string Action { get; set; }

        public DeleteMessageInfo()
        {
            this.DeleteType = string.Empty;
            this.AccountId = 0;
            this.VirtualNumberId = 0;
            this.MobileNumber = string.Empty;
            this.VirtualNumber = string.Empty;
            this.MessageId = 0;
            this.Title = string.Empty;
            this.Prompt = string.Empty;
            this.SubPrompt = string.Empty;
            this.Action = string.Empty;
        }
    }
}
