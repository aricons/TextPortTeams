using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using TextPortCore.Helpers;
using TextPortCore.Integrations.Common;

namespace TextPortCore.Models
{
    public partial class Message
    {
        public int MessageId { get; set; }

        public int BranchId { get; set; }

        public int AccountId { get; set; }

        public DateTime TimeStamp { get; set; }

        public int VirtualNumberId { get; set; }

        public int? ContactId { get; set; }

        [Display(Name = "In/Out")]
        public byte Direction { get; set; }

        public byte MessageType { get; set; }

        public string Ipaddress { get; set; }

        public bool IsMMS { get; set; }

        [Display(Name = "To")]
        public string MobileNumber { get; set; }

        [Display(Name = "Message")]
        public string MessageText { get; set; }

        public string VirtualNumber { get; set; }

        public string SessionId { get; set; }

        public string GatewayMessageId { get; set; }

        public decimal? CustomerCost { get; set; }

        public decimal? Price { get; set; }

        public string ProcessingMessage { get; set; }

        public string FailureReason { get; set; }

        public DateTime? DeleteFlag { get; set; }

        public byte? QueueStatus { get; set; }

        public int? Segments { get; set; }

        public int? EmailToSMSAddressId { get; set; }

        public Account Account { get; set; }

        public Branch Branch { get; set; }

        public DedicatedVirtualNumber DedicatedVirtualNumber { get; set; }

        public Contact Contact { get; set; }

        public List<MMSFile> MMSFiles { get; set; }

        // Constructors

        // Outbound Messages
        public Message()
        {
            // Default outbound message
            this.MessageType = (byte)MessageTypes.Normal;
            this.Direction = (byte)MessageDirection.Outbound;
            this.QueueStatus = (byte)QueueStatuses.NotProcessed;
            this.Ipaddress = "0.0.0.0";
            this.CustomerCost = 0;
            this.MobileNumber = string.Empty;
            this.GatewayMessageId = string.Empty;
            this.TimeStamp = DateTime.UtcNow;
            this.MessageText = string.Empty;
            this.Segments = 1;
            this.Account = null;
            this.DedicatedVirtualNumber = null;// new DedicatedVirtualNumber();
            this.Contact = null;
            this.MMSFiles = null; // new List<MMSFile>();
            this.IsMMS = false;
        }

        public Message(int accId, byte msgType, int virtualNumId, string msgText)
        {
            // Outbound message for notifications
            this.MessageType = msgType;
            this.AccountId = accId;
            this.QueueStatus = (byte)QueueStatuses.NotProcessed;
            this.Ipaddress = Utilities.GetUserHostAddress();
            this.VirtualNumberId = virtualNumId;
            this.Direction = (byte)MessageDirection.Outbound;
            this.CustomerCost = Constants.BaseSMSSegmentCost;
            this.MobileNumber = string.Empty;
            this.GatewayMessageId = string.Empty;
            this.TimeStamp = DateTime.UtcNow;
            this.MessageText = msgText;
            this.Segments = Utilities.GetSegmentCount(msgText);
            this.Account = null;
            this.Contact = null;
            this.MMSFiles = null; // new List<MMSFile>();
            this.IsMMS = false;
        }

        public Message(BulkMessageItem bulkMessage, MessageTypes msgType, int accountId, int branchId, int sourceNumberId, string sourceNumber)
        {
            // Bulk outbound message
            this.AccountId = accountId;
            this.BranchId = branchId;
            this.MessageType = (byte)msgType;
            this.Direction = (byte)MessageDirection.Outbound;
            this.QueueStatus = (byte)QueueStatuses.NotProcessed;
            this.CustomerCost = Constants.BaseSMSSegmentCost;
            this.Ipaddress = Utilities.GetUserHostAddress();
            this.VirtualNumberId = sourceNumberId;
            this.MobileNumber = Utilities.NumberToE164(bulkMessage.Number, "1");
            this.GatewayMessageId = string.Empty;
            this.TimeStamp = DateTime.UtcNow;
            this.MessageText = applyOptOutTrailer(bulkMessage.MessageText, MessageTypes.Bulk);
            this.Segments = Utilities.GetSegmentCount(bulkMessage.MessageText);
            this.Account = null;
            this.Contact = null;
            this.MMSFiles = null; // new List<MMSFile>();
            this.IsMMS = false;
        }

        public Message(EmailToSMSMessage emailToSMSMessage, string destinationNumber)
        {
            // EmailToSMS gateway outbound message
            this.AccountId = emailToSMSMessage.AccountId;
            this.MessageType = (byte)MessageTypes.EmailToSMS;
            this.Direction = (byte)MessageDirection.Outbound;
            this.QueueStatus = (byte)QueueStatuses.NotProcessed;
            this.CustomerCost = Constants.BaseSMSSegmentCost;
            this.Ipaddress = Utilities.GetUserHostAddress();
            this.VirtualNumberId = emailToSMSMessage.VirtualNumberId;
            this.MobileNumber = destinationNumber;
            this.GatewayMessageId = string.Empty;
            this.TimeStamp = DateTime.UtcNow;
            this.MessageText = applyOptOutTrailer(emailToSMSMessage.MessageText, MessageTypes.EmailToSMS);
            this.Segments = Utilities.GetSegmentCount(emailToSMSMessage.MessageText);
            this.Account = null;
            this.Contact = null;
            this.EmailToSMSAddressId = emailToSMSMessage.AddressId;
            this.MMSFiles = null; // new List<MMSFile>();
            this.IsMMS = false;
        }


        public Message(API.Message apiMessage, int accountId, int virtualNumberId)
        {
            // API-originated outbound message
            this.AccountId = accountId;
            this.MessageType = (byte)MessageTypes.API;
            this.Direction = (byte)MessageDirection.Outbound;
            this.QueueStatus = (byte)QueueStatuses.NotProcessed;
            this.CustomerCost = Constants.BaseSMSSegmentCost;
            this.Ipaddress = Utilities.GetUserHostAddress();
            this.VirtualNumberId = virtualNumberId;
            this.MobileNumber = apiMessage.To;
            this.GatewayMessageId = string.Empty;
            this.TimeStamp = DateTime.UtcNow;
            this.MessageText = applyOptOutTrailer(apiMessage.MessageText, MessageTypes.API);
            this.Segments = Utilities.GetSegmentCount(apiMessage.MessageText);
            this.Account = null;
            this.Contact = null;
            this.MMSFiles = null; // new List<MMSFile>();
            this.IsMMS = false;
        }

        // Inbound messages
        public Message(IntegrationMessageIn integrationMessageIn, DedicatedVirtualNumber dvn, string sessionId)
        {
            // Inbound from carrier        
            this.MessageType = (byte)MessageTypes.Normal;
            this.QueueStatus = (byte)QueueStatuses.Received;
            this.Direction = (byte)MessageDirection.Inbound;
            this.CustomerCost = 0;
            this.DedicatedVirtualNumber = dvn;
            this.Branch = dvn.Branch;
            this.BranchId = dvn.BranchId;
            this.AccountId = 0;
            this.Ipaddress = Utilities.GetUserHostAddress();
            this.VirtualNumberId = dvn.VirtualNumberId;
            this.SessionId = sessionId;
            this.TimeStamp = DateTime.UtcNow;
            this.Segments = 1;
            this.MobileNumber = integrationMessageIn.From;
            this.GatewayMessageId = integrationMessageIn.CarrierMessageId;
            this.MessageText = integrationMessageIn.Message;
            this.Segments = (integrationMessageIn.SegmentCount == 0) ? 1 : integrationMessageIn.SegmentCount;
            this.MMSFiles = new List<MMSFile>();

            if (integrationMessageIn.Media != null && integrationMessageIn.Media.Count > 0)
            {
                this.IsMMS = true;
                foreach (string mediaItem in integrationMessageIn.Media)
                {
                    if (!mediaItem.EndsWith(".smil", StringComparison.CurrentCultureIgnoreCase) && !mediaItem.EndsWith("smil.xml", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string localFileName = WebFunctions.GetImageFromURL(mediaItem, this.BranchId);

                        if (!string.IsNullOrEmpty(localFileName))
                        {
                            this.MMSFiles.Add(new MMSFile()
                            {
                                FileName = localFileName
                            });
                        }
                    }
                }
            }
        }

        private string applyOptOutTrailer(string messageText, MessageTypes messageType)
        {
            switch (messageType)
            {
                case MessageTypes.API:
                case MessageTypes.Bulk:
                case MessageTypes.BulkUpload:
                case MessageTypes.Group:
                    return $"{messageText} Reply STOP to opt out";

                default:
                    return messageText;
            }
        }

        public bool Send()
        {
            return MessageRouting.WriteSemaphoreFile(this);
        }

        public void ConvertTimeStampToLocalTimeZone()
        {
            if (this.Branch != null)
            {
                this.TimeStamp = TimeFunctions.GetUsersLocalTime(this.TimeStamp, this.Branch.TimeZoneId);
            }
        }

    }
}
