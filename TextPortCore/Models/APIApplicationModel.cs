using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Models
{
    public partial class APIApplicationModel
    {
        public int APIApplicationId { get; set; }

        public int AccountId { get; set; }

        [Required(ErrorMessage = "An application name is required")]
        [Display(Name = "Application Name")]
        [StringLength(30, ErrorMessage = "Must be between 1 and 30 characters", MinimumLength = 1)]
        public string ApplicationName { get; set; }
        public string APIToken { get; set; }
        public string APISecret { get; set; }

        // Constructors
        public APIApplicationModel()
        {
            this.AccountId = 0;
            this.ApplicationName = string.Empty;
            this.APIToken = string.Empty;
            this.APISecret = string.Empty;
        }
    }
}
