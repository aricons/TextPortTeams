using System;

using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public partial class PooledNumber
    {
        public int PooledNumberId { get; set; }

        public int CarrierId { get; set; }

        public bool Enabled { get; set; }

        public string VirtualNumber { get; set; }

        public string Description { get; set; }

        public PooledNumber()
        {
            this.PooledNumberId = 0;
            this.CarrierId = (int)Carriers.BandWidth;
            this.Enabled = true;
            this.VirtualNumber = string.Empty;
            this.Description = string.Empty;
        }
    }
}
