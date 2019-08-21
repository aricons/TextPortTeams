using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TextPortCore.Models
{
    public partial class BlockedNumber
    {
        public int BlockID { get; set; }
        public string MobileNumber { get; set; }
        public DateTime DateRequested { get; set; }

        // Constructor
        public BlockedNumber()
        {
            this.BlockID = 0;
            this.MobileNumber = string.Empty;
            this.DateRequested = DateTime.MinValue;
        }
    }
}

