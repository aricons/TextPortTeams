using System;
using System.Collections.Generic;

namespace TextPortCore.Models
{
    public class FreeTextResult : MessageList
    {
        public string Status { get; set; }

        public string SubmissionMessage { get; set; }

        public FreeTextResult()
        {
            this.Status = string.Empty;
            this.SubmissionMessage = string.Empty;
            this.Messages = new List<Message>();
        }
    }
}