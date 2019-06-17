using System;

namespace TextPortCore.Models
{
    public partial class SupportRequest
    {
        public int SupportId { get; set; }

        public DateTime TimeStamp { get; set; }

        public byte RequestType { get; set; }

        public string Category { get; set; }

        public string RequestorName { get; set; }

        public string RequestorEmail { get; set; }

        public string Ipaddress { get; set; }

        public string SendingNumber { get; set; }

        public string ReceivingNumber { get; set; }

        public string Message { get; set; }

        public SupportRequest()
        {
        }

        public SupportRequest(SupportRequestModel model)
        {
            this.SupportId = model.SupportId;
            this.RequestType = (byte)model.RequestType;
            this.TimeStamp = model.TimeStamp;
            this.Category = model.Category;
            this.RequestorName = model.RequestorName;
            this.RequestorEmail = model.RequestorEmail;
            this.Ipaddress = model.Ipaddress;
            this.SendingNumber = model.SendingNumber;
            this.ReceivingNumber = model.ReceivingNumber;
            this.Message = model.Message;
        }
    }
}