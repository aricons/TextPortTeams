using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using TextPortCore.Helpers.CustomValidation;

namespace TextPortCore.Models
{
    [Table("Accounts")]
    public partial class Account
    {
        public int AccountId { get; set; }

        [Required(ErrorMessage = "A user name is required")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "A password is required")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "An email address is required")]
        [StringLength(60, ErrorMessage = "Must be between 5 and 60 characters", MinimumLength = 5)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Time Zone")]
        public short TimeZoneId { get; set; }

        [Display(Name = "Date Created")]
        public DateTime CreateDate { get; set; }

        public bool RegisteredAsTrial { get; set; }

        [Display(Name = "Last Login")]
        public DateTime? LastLogin { get; set; }

        [Display(Name = "Login Count")]
        public int LoginCount { get; set; }

        [Display(Name = "Balance")]
        public decimal Balance { get; set; }

        public decimal SMSSegmentCost { get; set; }

        public decimal MMSSegmentCost { get; set; }

        [Display(Name = "Forward Inbound Messages to Email")]
        public bool EnableEmailNotifications { get; set; }

        [Display(Name = "Forward Inbound Messages to Mobile Number")]
        public bool EnableMobileForwarding { get; set; }

        [Display(Name = "Notifications Email Address")]
        [StringLength(60, ErrorMessage = "Must be between 5 and 60 characters", MinimumLength = 5)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RequiredIf("EnableEmailNotifications", true, ErrorMessage = "A forwarding email address must be entered if Enable Email Notifications is checked.")]
        public string NotificationsEmailAddress { get; set; }

        [Display(Name = "Forward Messages To Mobile #")]
        [RequiredIf("EnableMobileForwarding", true, ErrorMessage = "A forwarding mobile number must be entered if Forward Messages To Mobile # is checked.")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string ForwardVnmessagesTo { get; set; }

        [Display(Name = "Messages Sent")]
        public int MessageOutCount { get; set; }

        [Display(Name = "Messages Received")]
        public int MessageInCount { get; set; }

        public string AccountValidationKey { get; set; }

        public string PasswordResetToken { get; set; }

        public bool AccountValidated { get; set; }

        public string RegistrationVirtualNumber { get; set; }

        public bool Deleted { get; set; }

        public bool Enabled { get; set; }

        public byte ComplimentaryNumber { get; set; }
    }
}
