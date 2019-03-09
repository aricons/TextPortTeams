using System;
using System.Collections.Generic;
using System.Text;

namespace TextPortCore.Models
{
    public class MessageList
    {
        private List<Message> messages;

        public List<Message> Messages
        {
            get { return this.messages; }
            set { this.messages = value; }
        }

        public MessageList()
        {
            this.Messages = new List<Message>();
        }

        public MessageList(Message msg)
        {
            this.Messages = new List<Message>() { msg };
        }
    }
}
