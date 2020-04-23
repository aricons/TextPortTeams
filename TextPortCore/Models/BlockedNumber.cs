using System;

using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public partial class BlockedNumber
    {
        public int BlockID { get; set; }
        public string MobileNumber { get; set; }
        public byte Direction { get; set; }
        public DateTime DateRequested { get; set; }
        public int BlockCount { get; set; }

        // Constructors
        public BlockedNumber()
        {
            this.BlockID = 0;
            this.Direction = (byte)MessageDirection.Outbound;
            this.MobileNumber = string.Empty;
            this.DateRequested = DateTime.MinValue;
            this.BlockCount = 0;
        }

        public BlockedNumber(BlockRequest blockRequest)
        {
            this.BlockID = 0;
            this.BlockCount = 0;
            this.DateRequested = DateTime.UtcNow;
            this.MobileNumber = blockRequest.MobileNumberE164;
            this.Direction = (byte)blockRequest.Direction;
        }
    }
}

