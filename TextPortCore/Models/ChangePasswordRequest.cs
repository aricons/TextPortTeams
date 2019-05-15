using System;
using System.ComponentModel.DataAnnotations;

using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class ChangePasswordRequest
    {
        private int accountId;
        private string oldPassword;
        private string newPassword;
        private string confirmPassword;
        private RequestStatus status;
        private string confirmationMessage;

        public int AccountId
        {
            get { return this.accountId; }
            set { this.accountId = value; }
        }

        [Required(ErrorMessage = "An old password is required")]
        [Display(Name = "Old Password")]
        [DataType(DataType.Password)]
        [StringLength(60, ErrorMessage = "Must be between 5 and 60 characters", MinimumLength = 5)]
        public string OldPassword
        {
            get { return this.oldPassword; }
            set { this.oldPassword = value; }
        }

        [Required(ErrorMessage = "A new password is required")]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [StringLength(60, ErrorMessage = "Must be between 5 and 60 characters", MinimumLength = 5)]
        public string NewPassword
        {
            get { return this.newPassword; }
            set { this.newPassword = value; }
        }

        [Required(ErrorMessage = "A password confirmation is required")]
        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [StringLength(60, ErrorMessage = "Must be between 5 and 60 characters", MinimumLength = 5)]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The passwords do not match")]
        public string ConfirmPassword
        {
            get { return this.confirmPassword; }
            set { this.confirmPassword = value; }
        }

        public RequestStatus Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

        public string ConfirmationMessage
        {
            get { return this.confirmationMessage; }
            set { this.confirmationMessage = value; }
        }

        // Constructors
        public ChangePasswordRequest()
        {
            this.AccountId = 0;
            this.OldPassword = string.Empty;
            this.NewPassword = string.Empty;
            this.ConfirmPassword = string.Empty;
            this.ConfirmationMessage = string.Empty;
            this.Status = RequestStatus.Pending;
        }
    }
}

