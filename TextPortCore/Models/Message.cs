﻿using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using TextPortCore.Helpers;
using TextPortCore.Integrations.Bandwidth;

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

        public byte MessageType { get; set; }

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

        public decimal? CustomerCost { get; set; }

        public DateTime? Delivered { get; set; }

        public decimal? Price { get; set; }

        public string ProcessingMessage { get; set; }

        public DateTime? DeleteFlag { get; set; }

        public string RoutingType { get; set; }

        public byte? QueueStatus { get; set; }

        public Account Account { get; set; }

        public List<MMSFile> MMSFiles { get; set; } = new List<MMSFile>();

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
            this.Direction = (int)MessageDirection.Outbound;
            this.CarrierId = (int)Carriers.BandWidth;
            this.QueueStatus = (byte)QueueStatuses.Queued;
            this.Ipaddress = "0.0.0.0";
            this.CustomerCost = 0;
            this.MobileNumber = string.Empty;
            this.GatewayMessageId = string.Empty;
            this.TimeStamp = DateTime.UtcNow;
            this.MessageText = string.Empty;
            this.IsMMS = false;
            this.Account = null;
            this.MMSFiles = new List<MMSFile>();
        }

        public Message(int accId, byte msgType, int virtualNumId, string msgText)
        {
            // Outbound message for notifications
            this.MessageType = msgType;
            this.AccountId = accId;
            this.CarrierId = (int)Carriers.BandWidth;
            this.QueueStatus = (byte)QueueStatuses.Queued;
            this.CarrierId = (int)Carriers.BandWidth;
            this.Ipaddress = Utilities.GetUserHostAddress();
            this.VirtualNumberId = virtualNumId;
            this.Direction = (int)MessageDirection.Outbound;
            this.CustomerCost = Constants.BaseSMSMessageCost;
            this.MobileNumber = string.Empty;
            this.GatewayMessageId = string.Empty;
            this.TimeStamp = DateTime.UtcNow;
            this.MessageText = msgText;
            this.IsMMS = false;
            this.Account = null;
            this.MMSFiles = new List<MMSFile>();
        }

        public Message(BulkMessageItem bulkMessage, int accountId, int sourceNumberId, string sourceNumber)
        {
            // Bulk outbound message
            this.AccountId = accountId;
            this.MessageType = (byte)MessageTypes.Bulk;
            this.Direction = (int)MessageDirection.Outbound;
            this.QueueStatus = (byte)QueueStatuses.Queued;
            this.CarrierId = (int)Carriers.BandWidth;
            this.CustomerCost = Constants.BaseSMSMessageCost;
            this.Ipaddress = Utilities.GetUserHostAddress();
            this.VirtualNumberId = sourceNumberId;
            this.MobileNumber = Utilities.NumberToE164(bulkMessage.Number);
            this.GatewayMessageId = string.Empty;
            this.TimeStamp = DateTime.UtcNow;
            this.MessageText = bulkMessage.MessageText;
            this.IsMMS = false;
            this.Account = null;
            this.MMSFiles = new List<MMSFile>();
        }

        // Inbound messages
        public Message(BandwidthInboundMessage bwMessage, int accountId, int virtualNumberId)
        {
            // Inbound from Bandwidth        
            this.MessageType = (byte)MessageTypes.Normal;
            this.CarrierId = (int)Carriers.BandWidth;
            this.QueueStatus = (byte)QueueStatuses.Received;
            this.Direction = (int)MessageDirection.Inbound;
            this.CustomerCost = 0;
            this.AccountId = accountId;
            this.Ipaddress = Utilities.GetUserHostAddress();
            this.VirtualNumber = string.Empty; // bwMessage.to.Replace("+", "");
            this.VirtualNumberId = virtualNumberId;
            this.TimeStamp = DateTime.UtcNow;
            this.Account = null;

            if (bwMessage.message != null)
            {
                this.MobileNumber = bwMessage.message.from.Replace("+", "");
                this.GatewayMessageId = bwMessage.message.id;
                this.MessageText = bwMessage.message.text;
                this.IsMMS = false;
                this.MMSFiles = new List<MMSFile>();

                if (bwMessage.message.media != null && bwMessage.message.media.Count > 0)
                {
                    this.IsMMS = true;
                    foreach (string mediaItem in bwMessage.message.media)
                    {
                        if (!mediaItem.EndsWith(".smil", StringComparison.CurrentCultureIgnoreCase))
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
        }

        public bool Send()
        {
            return MessageRouting.WriteSemaphoreFile(this);
        }

    }
}
