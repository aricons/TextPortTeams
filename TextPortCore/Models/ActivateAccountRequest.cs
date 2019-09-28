using System;
using System.ComponentModel.DataAnnotations;

using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class ActivateAccountRequest
    {
        private string activationKey;
        private int accountId;
        private int virtualNumberId;
        private string userName;
        private string emailAddress;
        private string completionTitle;
        private string completionMessage;
        private string virtualNumber;
        private decimal creditAmount;
        private bool success;

        [Required(ErrorMessage = "An activation key is required")]
        [Display(Name = "Activation Key")]
        [EmailAddress(ErrorMessage = "An activation key is required")]
        public string ActivationKey
        {
            get { return this.activationKey; }
            set { this.activationKey = value; }
        }

        public int AccountId
        {
            get { return this.accountId; }
            set { this.accountId = value; }
        }

        public int VirtualNumberId
        {
            get { return this.virtualNumberId; }
            set { this.virtualNumberId = value; }
        }

        public string UserName
        {
            get { return this.userName; }
            set { this.userName = value; }
        }

        public string EmailAddress
        {
            get { return this.emailAddress; }
            set { this.emailAddress = value; }
        }

        public string CompletionTitle
        {
            get { return this.completionTitle; }
            set { this.completionTitle = value; }
        }

        public string CompletionMessage
        {
            get { return this.completionMessage; }
            set { this.completionMessage = value; }
        }

        public string VirtualNumber
        {
            get { return this.virtualNumber; }
            set { this.virtualNumber = value; }
        }

        public decimal CreditAmount
        {
            get { return this.creditAmount; }
            set { this.creditAmount = value; }
        }

        public bool Success
        {
            get { return this.success; }
            set { this.success = value; }
        }


        // Constructors
        public ActivateAccountRequest()
        {
            this.ActivationKey = string.Empty;
            this.AccountId = 0;
            this.VirtualNumberId = 0;
            this.UserName = string.Empty;
            this.EmailAddress = string.Empty;
            this.CompletionTitle = string.Empty;
            this.CompletionMessage = string.Empty;
            this.VirtualNumber = string.Empty;
            this.CreditAmount = 0;
            this.Success = false;
        }

        public ActivateAccountRequest(string key)
        {
            this.ActivationKey = key;
            this.AccountId = 0;
            this.VirtualNumberId = 0;
            this.UserName = string.Empty;
            this.EmailAddress = string.Empty;
            this.CompletionTitle = string.Empty;
            this.CompletionMessage = string.Empty;
            this.VirtualNumber = string.Empty;
            this.CreditAmount = 0;
            this.Success = false;
        }

    }
}
