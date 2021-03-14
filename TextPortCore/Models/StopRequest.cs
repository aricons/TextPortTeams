using System;

using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public partial class StopRequest
    {
        public int StopId { get; set; }

        public string MobileNumber { get; set; }

        public DateTime DateRequested { get; set; }

        public string MobileNumberE164
        {
            get { return this.MobileNumber.ToE164(); }
        }


        public StopRequest()
        {
            this.StopId = 0;
            this.MobileNumber = string.Empty;
        }

        public StopRequest(string mobileNumber)
        {
            this.StopId = 0;
            this.DateRequested = DateTime.Now;
            this.MobileNumber = mobileNumber;
        }
    }
}
