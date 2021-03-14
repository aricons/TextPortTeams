using System;
using System.ComponentModel.DataAnnotations;

using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public partial class DedicatedVirtualNumber
    {
        public int VirtualNumberId { get; set; }

        public int BranchId { get; set; }

        public int CountryId { get; set; }

        public int CarrierId { get; set; }

        public string CountryCode { get; set; }

        public byte NumberType { get; set; }

        [Required(ErrorMessage = "A number is required")]
        [Display(Name = "Number")]
        public string VirtualNumber { get; set; }

        public string LeasePeriodType { get; set; }

        public short LeasePeriod { get; set; }

        [Display(Name = "Cost")]
        public decimal Fee { get; set; }

        [Display(Name = "Lease Date")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Expiration Date")]
        public DateTime ExpirationDate { get; set; }

        [Display(Name = "Auto-Renew")]
        public bool AutoRenew { get; set; }

        public bool IsDefault { get; set; }

        public bool Cancelled { get; set; }

        [Display(Name = "Times Renewed")]
        public int RenewalCount { get; set; }

        [Display(Name = "7-Day Expiration Reminder Sent")]
        public DateTime? SevenDayReminderSent { get; set; }

        [Display(Name = "2-Day Expiration Reminder Sent")]
        public DateTime? TwoDayReminderSent { get; set; }

        public int ReminderFailureCount { get; set; }

        public int CancellationFailureCount { get; set; }

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

        public string ImageUrl
        {
            get
            {
                return $"/content/images/flags/20px/{this.Country?.CountryAlphaCode}.png";
            }
        }

        public Branch Branch { get; set; }

        public Carrier Carrier { get; set; }

        public Country Country { get; set; }

        // Constructors
        public DedicatedVirtualNumber()
        {
            this.CarrierId = (int)Carriers.BandWidth;
            this.VirtualNumber = string.Empty;
            this.BranchId = 0;
        }

        public DedicatedVirtualNumber(NumberExpirationData expData)
        {
            this.VirtualNumberId = expData.VirtualNumberID;
            this.VirtualNumber = expData.VirtualNumber;
            this.BranchId = expData.BranchId;
            this.CountryCode = expData.CountryCode;
            this.ExpirationDate = expData.ExpirationDate;
        }
    }
}
