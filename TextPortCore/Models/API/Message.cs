using System;
using System.ComponentModel.DataAnnotations;

using core = TextPortCore.Models;
using intCommon = TextPortCore.Integrations.Common;

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

        public Message(intCommon.IntegrationMessageIn integrationMessageIn)
        {
            this.From = integrationMessageIn.From;
            this.To = integrationMessageIn.To;
            this.MessageText = integrationMessageIn.Message;
        }
    }
}
