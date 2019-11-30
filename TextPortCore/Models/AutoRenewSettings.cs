using System;

namespace TextPortCore.Models
{
    public class AutoRenewSettings
    {
        public int VirtualNumberId { get; set; }
        public bool AutoRenew { get; set; }
        public string ConfirmationTitle { get; set; }
        public string ConfirmationDetail { get; set; }

        public AutoRenewSettings()
        {
            this.VirtualNumberId = 0;
            this.AutoRenew = false;
            this.ConfirmationTitle = string.Empty;
            this.ConfirmationDetail = string.Empty;
        }
    }
}
