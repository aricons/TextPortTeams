using System;
using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Models
{
    public class EmailToSMSAddress
    {
        public int AddressId { get; set; }

        public int AccountId { get; set; }

        [Required(ErrorMessage = "A virtual number is required")]
        [Display(Name = "Virtual Number")]
        public int VirtualNumberId { get; set; }

        [Required(ErrorMessage = "An email address is required")]
        [StringLength(60, ErrorMessage = "Must be between 5 and 60 characters", MinimumLength = 5)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email Address")]
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
