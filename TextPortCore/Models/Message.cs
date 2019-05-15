using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using TextPortCore.Helpers;
using TextPortCore.Models.Bandwidth;

namespace TextPortCore.Models
{
    public partial class Message
    {
        public int MessageId { get; set; }

        public DateTime TimeStamp { get; set; }

        public int AccountId { get; set; }

        public int VirtualNumberId { get; set; }

        [Display(Name = "In/Out")]
        public byte Direction { get; set; }

        public string Ipaddress { get; set; }

        public bool IsMMS { get; set; }

        [Display(Name = "To")]
        public string MobileNumber { get; set; }

        [Display(Name = "From")]
        public string FromNumber { get; set; }

        public string FromEmail { get; set; }

        public int? CarrierId { get; set; }

        public string GatewayEmail { get; set; }

        public string SmtphostName { get; set; }

        public string Subject { get; set; }

        [Display(Name = "Message")]
        public string MessageText { get; set; }

        public byte? Result { get; set; }

        public string VirtualNumber { get; set; }

        public string UniqueMessageId { get; set; }

        public bool? MobileDevice { get; set; }

        public string GatewayMessageId { get; set; }

        public string MmsfileNames { get; set; }

        public int? CreditCost { get; set; }

        public DateTime? Delivered { get; set; }

        public decimal? Price { get; set; }

        public string ProcessingMessage { get; set; }

        public DateTime? DeleteFlag { get; set; }

        public string SourceType { get; set; }

        public string RoutingType { get; set; }

        public byte? QueueStatus { get; set; }

        public List<MMSFile> MMSFiles { get; set; } = new List<MMSFile>();


        // Constructors
        public Message()
        {
            this.Direction = (int)MessageDirection.Outbound;
            this.VirtualNumber = string.Empty;
            this.MobileNumber = string.Empty;
            this.GatewayMessageId = string.Empty;
            this.TimeStamp = DateTime.UtcNow;
            this.MessageText = string.Empty;
            this.IsMMS = false;
            this.MMSFiles = new List<MMSFile>();
        }

        public Message(BandwidthInboundMessage bwMessage)
        {
            this.Direction = (int)MessageDirection.Inbound;
            this.VirtualNumber = bwMessage.to.Replace("+", "");
            this.VirtualNumberId = 0;
            this.MobileNumber = bwMessage.from.Replace("+", "");
            this.GatewayMessageId = bwMessage.messageId;
            this.TimeStamp = DateTime.UtcNow;
            this.Ipaddress = "0.0.0.0";
            this.CarrierId = (int)Carriers.BandWidth;
            this.CreditCost = 0;
            this.MessageText = bwMessage.text;
            this.IsMMS = false;
            this.MMSFiles = new List<MMSFile>();
        }

        public Message(BulkMessageItem bulkMessage, int sourceNumberId, string sourceNumber)
        {
            this.Direction = (int)MessageDirection.Outbound;
            this.VirtualNumberId = sourceNumberId;
            this.VirtualNumber = Utilities.NumberToGlobalFormat(sourceNumber);
            this.MobileNumber = Utilities.NumberToGlobalFormat( bulkMessage.Number);
            this.GatewayMessageId = string.Empty;
            this.TimeStamp = DateTime.UtcNow;
            this.MessageText = bulkMessage.MessageText;
            this.IsMMS = false;
            this.MMSFiles = new List<MMSFile>();
        }

        public bool WriteQueueSemaphore()
        {
            MessageRouting.WriteSemaphoreFile(this);
            return true;
        }
    }
}
