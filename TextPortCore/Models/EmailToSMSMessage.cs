using System;
using System.Collections.Generic;

namespace TextPortCore.Models
{
    public class EmailToSMSMessage
    {
        public string InputFile { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public int AccountId { get; set; }
        public int VirtualNumberId { get; set; }
        public bool IsValid { get; set; }
        public string MessageText { get; set; }
        public List<string> DestinationNumbers { get; set; }
        public string ProcessingLog { get; set; }
        public int? AddressId { get; set; }

        // Constructors
        public EmailToSMSMessage()
        {
            InputFile = string.Empty;
            To = string.Empty;
            Cc = string.Empty;
            From = string.Empty;
            AccountId = 0;
            VirtualNumberId = 0;
            IsValid = false;
            MessageText = string.Empty;
            DestinationNumbers = new List<string>();
            ProcessingLog = string.Empty;
        }

        public EmailToSMSMessage(string messageFileName)
        {
            InputFile = messageFileName;
            To = string.Empty;
            Cc = string.Empty;
            From = string.Empty;
            AccountId = 0;
            VirtualNumberId = 0;
            IsValid = false;
            MessageText = string.Empty;
            DestinationNumbers = new List<string>();
            ProcessingLog = $"Initiating Email to SMS Gateway processing for file { messageFileName}\r\nLoading file {messageFileName}\r\n";
        }
    }
}