using System;

namespace TextPortCore.Models
{
    public class EmailToSMSAddress
    {
        public int AddressId { get; set; }
        public int AccountId { get; set; }
        public int VirtualNumberId { get; set; }
        public string EmailAddress { get; set; }
        public bool Enabled { get; set; }

        public EmailToSMSAddress()
        {
            this.AddressId = 0;
            this.AccountId = 0;
            this.VirtualNumberId = 0;
            this.EmailAddress = string.Empty;
            this.Enabled = true;
        }
    }
}
