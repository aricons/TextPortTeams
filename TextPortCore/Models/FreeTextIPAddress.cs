using System;

namespace TextPortCore.Models
{
    public class FreeTextIPAddress
    {
        public int IpId { get; set; }
        public string IPAddress { get; set; }
        public short RequestCount { get; set; }
        public DateTime LastRequest { get; set; }


        // Constructors
        public FreeTextIPAddress()
        {
            this.IpId = 0;
            this.RequestCount = 0;
            this.IPAddress = string.Empty;
            this.LastRequest = DateTime.UtcNow;
        }

        public FreeTextIPAddress(string ipAddress)
        {
            this.IpId = 0;
            this.RequestCount = 1;
            this.IPAddress = ipAddress;
            this.LastRequest = DateTime.UtcNow;
        }
    }
}
