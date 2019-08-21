using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextPortCore.Models.API
{
    public class MessagesResult
    {
        string From { get; set; }
        string To { get; set; }
        string MessageText { get; set; }
        string Status { get; set; }
        string ErrorMessage { get; set; }
        int MessageId { get; set; }

        // Constructors

        public MessagesResult()
        {
            this.From = string.Empty;
            this.To = string.Empty;
            this.MessageText = string.Empty;
            this.Status = string.Empty;
            this.ErrorMessage = string.Empty;
            this.MessageId = 0;
        }

        public MessagesResult(Message message, string status, int messageId, string errorMessage)
        {
            this.From = message.From;
            this.To = message.To;
            this.MessageText = message.MessageText;
            this.Status = string.Empty;
            this.ErrorMessage = errorMessage;
            this.MessageId = messageId;
        }

    }
}
