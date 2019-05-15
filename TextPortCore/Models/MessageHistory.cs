using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using TextPortCore.Data;

namespace TextPortCore.Models
{
    public class MessageHistory
    {
        private readonly TextPortContext _context;

        public MessageHistory(TextPortContext context)
        {
            this._context = context;
        }

        private string number;
        private List<Message> messages;


        public List<Message> Messages
        {
            get { return this.messages; }
            set { this.messages = value; }
        }


        public string Number
        {
            get { return this.number; }
            set { this.number = value; }
        }

        // Constructors
        public MessageHistory(TextPortContext context, int virtualNumberId)
        {
            this._context = context;

            DedicatedVirtualNumber vn = _context.DedicatedVirtualNumbers.FirstOrDefault(x => x.VirtualNumberId == virtualNumberId);
            if (vn != null)
            {
                this.Number = vn.NumberLocalFormat;
                this.Messages = _context.Messages.Include(m => m.MMSFiles).Where(x => x.VirtualNumberId == vn.VirtualNumberId).OrderByDescending(x => x.MessageId).ToList();
            }
            else
            {
                this.Messages = new List<Message>();
            }
        }
    }

}
