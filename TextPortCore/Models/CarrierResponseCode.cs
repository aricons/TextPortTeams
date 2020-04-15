using System;

namespace TextPortCore.Models
{
    public partial class CarrierResponseCode
    {
        public int ResponseCodeId { get; set; }
        public int CarrierId { get; set; }
        public string ResponseCode { get; set; }
        public string Description { get; set; }
        public bool Billable { get; set; }

        // Constructor
        public CarrierResponseCode()
        {
            this.ResponseCodeId = 0;
            this.CarrierId = 0;
            this.ResponseCode = string.Empty;
            this.Description = string.Empty;
            this.Billable = true;
        }
    }
}