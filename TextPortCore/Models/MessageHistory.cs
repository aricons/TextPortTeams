using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using TextPortCore.Data;

namespace TextPortCore.Models
{
    public class MessageHistory
    {
        //private readonly TextPortContext _context;

        //public MessageHistory(TextPortContext context)
        //{
        //    this._context = context;
        //}

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
        public MessageHistory(int virtualNumberId)
        {
            using (TextPortDA da = new TextPortDA())
            {
                DedicatedVirtualNumber vn = da.GetVirtualNumberById(virtualNumberId);
                if (vn != null)
                {
                    this.Number = vn.NumberDisplayFormat;
                    //this.Messages = _context.Messages.Include(m => m.MMSFiles).Where(x => x.VirtualNumberId == vn.VirtualNumberId).OrderByDescending(x => x.MessageId).ToList();
                    this.Messages = da.GetMessagsForVirtualNumber(virtualNumberId);
                }
                else
                {
                    this.Messages = new List<Message>();
                }
            }
        }
    }

}
