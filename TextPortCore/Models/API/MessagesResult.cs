using System;
using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Models.API
{
    public class MessageResult
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

        /// <summary>The status of the request</summary>
        [Required]
        public string Status { get; set; }

        /// <summary>An error message, if any errors occurred</summary>
        public string ErrorMessage { get; set; }

        /// <summary>The unique TextPort message ID. 0 if an error is encountered.</summary>
        [Required]
        public int MessageId { get; set; }

        /// <summary>Balance in $USD remaining on your account</summary>
        [Required]
        public decimal Balance { get; set; }

        // Constructors
        public MessageResult()
        {
            this.From = string.Empty;
            this.To = string.Empty;
            this.MessageText = string.Empty;
            this.Status = string.Empty;
            this.ErrorMessage = string.Empty;
            this.MessageId = 0;
            this.Balance = 0;

        }

        public MessageResult(Message message, string status, int messageId, string errorMessage, decimal newBalance)
        {
            this.From = message.From;
            this.To = message.To;
            this.MessageText = message.MessageText;
            this.Status = status;
            this.ErrorMessage = errorMessage;
            this.MessageId = messageId;
            this.Balance = newBalance;
        }

    }
}
