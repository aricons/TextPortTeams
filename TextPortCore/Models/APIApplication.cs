using System;
using System.ComponentModel.DataAnnotations;

using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public partial class APIApplication
    {
        public int APIApplicationId { get; set; }
        public int AccountId { get; set; }
        [Required(ErrorMessage = "An application name is required")]
        [Display(Name = "Application Name")]
        [StringLength(30, ErrorMessage = "Must be between 1 and 30 characters", MinimumLength = 1)]
        public string ApplicationName { get; set; }
        [Display(Name = "API Token")]
        public string APIToken { get; set; }
        [Display(Name = "API Secret")]
        public string APISecret { get; set; }
        [Display(Name = "Callbak URL")]
        public string CallbackURL { get; set; }
        public bool CallBackCredentialsRequired { get; set; }
        [Display(Name = "Callback Username")]
        public string CallbackUserName { get; set; }
        [Display(Name = "Callback Password")]
        public string CallbackPassword { get; set; }

        // Constructors
        public APIApplication()
        {
            this.APIApplicationId = 0;
            this.AccountId = 0;
            this.ApplicationName = string.Empty;
            this.APIToken = string.Empty;
            this.APISecret = string.Empty;
            this.CallbackURL = string.Empty;
            this.CallbackUserName = string.Empty;
            this.CallbackPassword = string.Empty;
            this.CallBackCredentialsRequired = false;
        }

        public APIApplication(int accountId)
        {
            this.APIApplicationId = 0;
            this.AccountId = 0;
            this.ApplicationName = string.Empty;
            this.APIToken = $"{accountId}-{RandomString.GenerateRandomToken(10)}";
            this.APISecret = RandomString.GenerateRandomToken(20);
            this.CallbackURL = string.Empty;
            this.CallbackUserName = string.Empty;
            this.CallbackPassword = string.Empty;
            this.CallBackCredentialsRequired = false;
        }
    }
}
