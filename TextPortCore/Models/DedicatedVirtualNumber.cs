using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public partial class DedicatedVirtualNumber
    {
        public int VirtualNumberId { get; set; }

        public int AccountId { get; set; }

        public int VirtualNumberCountryId { get; set; }

        public string CountryCode { get; set; }

        public byte NumberType { get; set; }

        [Required(ErrorMessage = "A number is required")]
        [Display(Name = "Number")]
        public string VirtualNumber { get; set; }

        public string Provider { get; set; }

        [Display(Name = "Cost")]
        public decimal Fee { get; set; }

        [Display(Name = "Lease Date")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Expiration Date")]
        public DateTime ExpirationDate { get; set; }

        public int? APIApplicationId { get; set; }

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
                return Utilities.NumberToDisplayFormat(this.VirtualNumber, this.VirtualNumberCountryId);
            }
        }

        public string NumberBandwidthFormat
        {
            get
            {
                return Utilities.NumberToBandwidthFormat(this.VirtualNumber);
            }
        }
    }
}
