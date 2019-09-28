using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class ForgotPasswordRequest
    {
        private string emailAddress;
        private string userName;
        private string confirmationMessage;
        private string browserType;
        private string ipAddress;
        private string resetUrl;
        private int accountId;
        private string password;
        private string confirmPassword;
        private RequestStatus status;

        [Required(ErrorMessage = "An email address is required")]
        [Display(Name = "Email Address")]
        [StringLength(60, ErrorMessage = "Must be between 5 and 60 characters", MinimumLength = 5)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress
        {
            get { return this.emailAddress; }
            set { this.emailAddress = value; }
        }

        public string UserName
        {
            get { return this.userName; }
            set { this.userName = value; }
        }

        public string ConfirmationMessage
        {
            get { return this.confirmationMessage; }
            set { this.confirmationMessage = value; }
        }

        public string BrowserType
        {
            get { return this.browserType; }
            set { this.browserType = value; }
        }

        public string IPAddress
        {
            get { return this.ipAddress; }
            set { this.ipAddress = value; }
        }

        public string ResetUrl
        {
            get { return this.resetUrl; }
            set { this.resetUrl = value; }
        }

        public int AccountId
        {
            get { return this.accountId; }
            set { this.accountId = value; }
        }

        [Required(ErrorMessage = "A password is required")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(60, ErrorMessage = "Must be between 5 and 60 characters", MinimumLength = 5)]
        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        [Required(ErrorMessage = "A password confirmation is required")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [StringLength(60, ErrorMessage = "Must be between 5 and 60 characters", MinimumLength = 5)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The passwords do not match")]
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


        // Constructors
        public ForgotPasswordRequest()
        {
            this.EmailAddress = string.Empty;
            this.UserName = string.Empty;
            this.ConfirmationMessage = string.Empty;
            this.BrowserType = string.Empty;
            this.IPAddress = string.Empty;
            this.ResetUrl = string.Empty;
            this.AccountId = 0;
            this.Password = string.Empty;
            this.ConfirmPassword = string.Empty;
            this.Status = RequestStatus.Pending;
        }

    }
}
