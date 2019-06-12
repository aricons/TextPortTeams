using System;
using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Models
{
    public class LoginCredentials
    {
        private string userNameOrEmail;
        private string loginPassword;

        [Required(ErrorMessage = "A user name or email is required")]
        [Display(Name = "User Name or Email")]
        [StringLength(60, ErrorMessage = "Must be between 5 and 60 characters", MinimumLength = 5)]
        public String UserNameOrEmail
        {
            get { return this.userNameOrEmail; }
            set { this.userNameOrEmail = value; }
        }

        [Required(ErrorMessage = "A password is required")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(60, ErrorMessage = "Must be between 5 and 60 characters", MinimumLength = 5)]
        public string LoginPassword
        {
            get { return this.loginPassword; }
            set { this.loginPassword = value; }
        }

        // Constructors
        public LoginCredentials()
        {
            this.UserNameOrEmail = string.Empty;
            this.LoginPassword = string.Empty;
        }
    }
}
