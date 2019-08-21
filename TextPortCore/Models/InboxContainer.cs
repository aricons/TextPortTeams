using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextPortCore.Models
{
    public class InboxContainer
    {
        public int AccountId { get; set; }
        public int MessageCount { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public List<InboxMessage> Messages { get; set; }

        // Constructors
        public InboxContainer()
        {
            this.AccountId = 0;
            this.MessageCount = 0;
            this.PageCount = 0;
            this.PageSize = 0;
            this.CurrentPage = 1;
            this.Messages = new List<InboxMessage>();
        }

        //public InboxContainer(int accountId, int messageCount, int pageSize, int currentPage, List<InboxMessage> messages)
        //{
        //    this.AccountId = accountId;
        //    this.PageSize = pageSize;
        //    this.CurrentPage = currentPage;
        //    this.MessageCount = messageCount;
        //    this.Messages = messages;
        //    if (this.MessageCount > 0 && this.PageSize > 0)
        //    {
        //        this.PageCount = this.MessageCount / this.PageSize;
        //    }
        //}
    }

    public class InboxMessage
    {
        public string VirtualNumber { get; set; }
        public string MobileNumber { get; set; }
        public DateTime TimeStamp { get; set; }
        public string MessageText { get; set; }

        // Constructors
        public InboxMessage()
        {
            this.VirtualNumber = string.Empty;
            this.MobileNumber = string.Empty;
            this.TimeStamp = DateTime.MinValue;
            this.MessageText = string.Empty;
        }

        public InboxMessage(string virtualNumber, string mobileNumber, DateTime timeStamp, string message)
        {
            this.VirtualNumber = virtualNumber;
            this.MobileNumber = mobileNumber;
            this.TimeStamp = timeStamp;
            this.MessageText = message;
        }
    }
}
