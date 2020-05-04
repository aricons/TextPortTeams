using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public partial class NumberWithAPIDetail
    {
        public int VirtualNumberId { get; set; }

        public int AccountId { get; set; }

        public int CountryId { get; set; }

        public string CountryCode { get; set; }

        public byte NumberType { get; set; }

        [Required(ErrorMessage = "A number is required")]
        [Display(Name = "Number")]
        public string VirtualNumber { get; set; }

        [Display(Name = "Lease Date")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Expiration Date")]
        public DateTime ExpirationDate { get; set; }

        [Display(Name = "Auto-Renew")]
        public bool AutoRenew { get; set; }

        public bool IsDefault { get; set; }

        public bool Cancelled { get; set; }

        public int? APIApplicationId { get; set; }

        public string ApiAppName { get; set; }

        public string NumberDisplayFormat
        {
            get
            {
                return Utilities.NumberToDisplayFormat(this.VirtualNumber, this.CountryId);
            }
        }

        public string NumberBandwidthFormat
        {
            get
            {
                return Utilities.NumberToBandwidthFormat(this.VirtualNumber);
            }
        }

        // Constructors
        public NumberWithAPIDetail()
        {
            this.AccountId = 0;
            this.ApiAppName = string.Empty;
            this.APIApplicationId = 0;
            this.Cancelled = false;
            this.CountryCode = string.Empty;
            this.CreateDate = DateTime.MinValue;
            this.ExpirationDate = DateTime.MinValue;
            this.AutoRenew = false;
            this.IsDefault = false;
            this.NumberType = 0;
            this.VirtualNumber = string.Empty;
            this.CountryId = 1;
            this.VirtualNumberId = 0;
        }

        public NumberWithAPIDetail(DedicatedVirtualNumber dvn)
        {
            this.AccountId = dvn.AccountId;

            this.Cancelled = dvn.Cancelled;
            this.CountryCode = dvn.CountryCode;
            this.CreateDate = dvn.CreateDate;
            this.ExpirationDate = dvn.ExpirationDate;
            this.AutoRenew = dvn.AutoRenew;
            this.IsDefault = dvn.IsDefault;
            this.NumberType = dvn.NumberType;
            this.VirtualNumber = dvn.VirtualNumber;
            this.CountryId = dvn.CountryId;
            this.VirtualNumberId = dvn.VirtualNumberId;
            this.APIApplicationId = dvn.APIApplicationId;
            this.ApiAppName = string.Empty;
        }
    }
}
