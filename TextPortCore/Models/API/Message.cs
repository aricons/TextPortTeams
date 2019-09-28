using System;
using System.ComponentModel.DataAnnotations;

using core = TextPortCore.Models;
using bw = TextPortCore.Integrations.Bandwidth;

namespace TextPortCore.Models.API
{
    public class Message
    {
        /// <summary>Your TextPort virtual number</summary>
        [Required]
        public string From { get; set; }

        /// <summary>The destination mobile number</summary>
        [Required]
        public string To { get; set; }

        /// <summary>The text message content</summary>
        [Required]
        public string MessageText { get; set; }

        // Constructors
        public Message()
        {
            this.From = string.Empty;
            this.To = string.Empty;
            this.MessageText = string.Empty;
        }

        public Message(core.Message messageIn)
        {
            this.From = messageIn.MobileNumber;
            this.To = messageIn.VirtualNumber;
            this.MessageText = messageIn.MessageText;
        }

        public Message(bw.BandwidthInboundMessage bwMessage)
        {
            this.From = bwMessage.message.from;
            this.To = bwMessage.to;
            this.MessageText = bwMessage.message.text;
        }
    }
}
