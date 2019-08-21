using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using core = TextPortCore.Models;

namespace TextPortCore.Models.API
{
    public class Message
    {
        public string From { get; set; }
        public string To { get; set; }
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
    }
}
