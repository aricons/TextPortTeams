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
        public int PageCount { get; set; }
        public int RecordsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int RecordCount { get; set; }
        public int LowRecord { get; set; }
        public int HighRecord { get; set; }
        public string SortOrder { get; set; }
        public string RecordLabel
        {
            get
            {
                return $"Messages {this.LowRecord} to {this.HighRecord} of {this.RecordCount}";
            }
        }
        public List<InboxMessage> Messages { get; set; }

        // Constructors
        public InboxContainer()
        {
            this.AccountId = 0;
            this.RecordCount = 0;
            this.PageCount = 0;
            this.RecordsPerPage = 0;
            this.CurrentPage = 1;
            this.LowRecord = 0;
            this.HighRecord = 0;
            this.SortOrder = "desc";
            this.Messages = new List<InboxMessage>();
        }
    }


    public class InboxMessage
    {
        public int MessageId { get; set; }
        public byte Direction { get; set; }
        public string VirtualNumber { get; set; }
        public string MobileNumber { get; set; }
        public DateTime TimeStamp { get; set; }
        public string MessageText { get; set; }

        // Constructors
        public InboxMessage()
        {
            this.MessageId = 0;
            this.Direction = 0;
            this.VirtualNumber = string.Empty;
            this.MobileNumber = string.Empty;
            this.TimeStamp = DateTime.MinValue;
            this.MessageText = string.Empty;
        }

        public InboxMessage(int messageid, byte direction, string virtualNumber, string mobileNumber, DateTime timeStamp, string message)
        {
            this.MessageId = MessageId;
            this.Direction = Direction;
            this.VirtualNumber = virtualNumber;
            this.MobileNumber = mobileNumber;
            this.TimeStamp = timeStamp;
            this.MessageText = message;
        }
    }
}
