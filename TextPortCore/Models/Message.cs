using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using TextPortCore.Helpers;
using TextPortCore.Integrations.Common;

namespace TextPortCore.Models
{
    public partial class Message
    {
        public int MessageId { get; set; }

        public int AccountId { get; set; }

        public DateTime TimeStamp { get; set; }

        public int VirtualNumberId { get; set; }

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

        public DedicatedVirtualNumber DedicatedVirtualNumber { get; set; }

        public List<MMSFile> MMSFiles { get; set; } //= new List<MMSFile>();

        public string NumberBandwidthFormat
        {
            get
            {
                return Utilities.NumberToBandwidthFormat(this.VirtualNumber);
            }
        }

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
            this.IsMMS = false;
            this.Account = null;
            this.DedicatedVirtualNumber = null;// new DedicatedVirtualNumber();
            this.MMSFiles = null; // new List<MMSFile>();
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
            this.IsMMS = false;
            this.Account = null;
            this.MMSFiles = null; // new List<MMSFile>();
        }

        public Message(BulkMessageItem bulkMessage, MessageTypes msgType, int accountId, int sourceNumberId, string sourceNumber)
        {
            // Bulk outbound message
            this.AccountId = accountId;
            this.MessageType = (byte)msgType;
            this.Direction = (byte)MessageDirection.Outbound;
            this.QueueStatus = (byte)QueueStatuses.NotProcessed;
            this.CustomerCost = Constants.BaseSMSSegmentCost;
            this.Ipaddress = Utilities.GetUserHostAddress();
            this.VirtualNumberId = sourceNumberId;
            this.MobileNumber = Utilities.NumberToE164(bulkMessage.Number);
            this.GatewayMessageId = string.Empty;
            this.TimeStamp = DateTime.UtcNow;
            this.MessageText = bulkMessage.MessageText;
            this.Segments = Utilities.GetSegmentCount(bulkMessage.MessageText);
            this.IsMMS = false;
            this.Account = null;
            this.MMSFiles = null; // new List<MMSFile>();
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
            this.MessageText = emailToSMSMessage.MessageText;
            this.Segments = Utilities.GetSegmentCount(emailToSMSMessage.MessageText);
            this.IsMMS = false;
            this.Account = null;
            this.EmailToSMSAddressId = emailToSMSMessage.AddressId;
            this.MMSFiles = null; // new List<MMSFile>();
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
            this.MessageText = apiMessage.MessageText;
            this.Segments = Utilities.GetSegmentCount(apiMessage.MessageText);
            this.IsMMS = false;
            this.Account = null;
            this.MMSFiles = null; // new List<MMSFile>();
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
            this.Account = dvn.Account;
            this.AccountId = dvn.AccountId;
            this.Ipaddress = Utilities.GetUserHostAddress();
            this.VirtualNumber = integrationMessageIn.To;
            this.VirtualNumberId = dvn.VirtualNumberId;
            this.SessionId = sessionId;
            this.TimeStamp = DateTime.UtcNow;
            this.Segments = 1;
            this.MobileNumber = integrationMessageIn.From;
            this.GatewayMessageId = integrationMessageIn.CarrierMessageId;
            this.MessageText = integrationMessageIn.Message;
            this.Segments = (integrationMessageIn.SegmentCount == 0) ? 1 : integrationMessageIn.SegmentCount;
            this.IsMMS = false;
            this.MMSFiles = new List<MMSFile>();

            if (integrationMessageIn.Media != null && integrationMessageIn.Media.Count > 0)
            {
                this.IsMMS = true;
                foreach (string mediaItem in integrationMessageIn.Media)
                {
                    if (!mediaItem.EndsWith(".smil", StringComparison.CurrentCultureIgnoreCase) && !mediaItem.EndsWith("smil.xml", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string localFileName = WebFunctions.GetImageFromURL(mediaItem, this.AccountId);

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

        public bool Send()
        {
            return MessageRouting.WriteSemaphoreFile(this);
        }

        public void ConvertTimeStampToLocalTimeZone()
        {
            if (this.Account != null)
            {
                this.TimeStamp = TimeFunctions.GetUsersLocalTime(this.TimeStamp, this.Account.TimeZoneId);
            }
        }

    }
}
