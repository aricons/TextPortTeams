using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using TextPortCore.Data;

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
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        [Display(Name = "Time Zone")]
        public short TimeZoneId { get; set; }
        [Display(Name = "Date Created")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "Last Login")]
        public DateTime? LastLogin { get; set; }
        [Display(Name = "Login Count")]
        public int LoginCount { get; set; }
        [Display(Name = "Credits")]
        public int Credits { get; set; }
        [Display(Name = "Notifications Email Address")]
        public string NotificationsEmailAddress { get; set; }
        [Display(Name = "Forward Messages To")]
        public string ForwardVnmessagesTo { get; set; }
        [Display(Name = "Messages Sent")]
        public int MessageOutCount { get; set; }
        [Display(Name = "Messages Received")]
        public int MessageInCount { get; set; }
        public string AccountValidationKey { get; set; }
        public string PasswordResetToken { get; set; }
        public bool AccountValidated { get; set; }
        public bool Deleted { get; set; }
        public bool Enabled { get; set; }
        public bool ComplimentaryNumber { get; set; }
    }
}
