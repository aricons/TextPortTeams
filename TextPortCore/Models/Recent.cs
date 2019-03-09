using System;
using System.Collections.Generic;
using System.Text;

namespace TextPortCore.Models
{
    public class Recent
    {
        public int MessageId { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Number { get; set; }

        public string Message { get; set; }

        public bool IsActiveMessage { get; set; }
    }
}
