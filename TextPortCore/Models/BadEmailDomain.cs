using System;

namespace TextPortCore.Models
{
    public class BadEmailDomain
    {
        public int DomainId { get; set; }
        public string DomainName { get; set; }
        public bool Blocked { get; set; }

        public BadEmailDomain()
        {
            this.DomainId = 0;
            this.DomainName = string.Empty;
            this.Blocked = false;
        }
    }
}
