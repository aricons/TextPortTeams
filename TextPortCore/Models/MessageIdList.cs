using System;
using System.Collections.Generic;

namespace TextPortCore.Models
{
    public class MessageIdList
    {
        public List<MessageIdItem> Ids { get; set; }

        public MessageIdList()
        {
            this.Ids = new List<MessageIdItem>();
        }
    }

    public class MessageIdItem
    {
        public int Id { get; set; }

        public MessageIdItem()
        {
            this.Id = 0;
        }
    }
}
