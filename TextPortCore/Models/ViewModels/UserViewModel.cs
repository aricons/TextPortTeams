using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using TextPortCore.Data;
using TextPortCore.Models;

namespace TextPortCore.ViewModels
{
    public class UserViewModel
    {
        [Display(Name = "UserId")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "A user name is required")]
        [Display(Name = "User Name")]
        [StringLength(16, ErrorMessage = "Must be between 5 and 16 characters", MinimumLength = 5)]
        [RegularExpression(@"^\S*$", ErrorMessage = "The username cannot contain spaces")]
        [Remote(action: "VerifyUsername", controller: "Account")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "A password is required")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(60, ErrorMessage = "Must be between 5 and 60 characters", MinimumLength = 5)]
        public string Password { get; set; }

        [Required(ErrorMessage = "A password confirmation is required")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [StringLength(60, ErrorMessage = "Must be between 5 and 60 characters", MinimumLength = 5)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "A name is required")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "An email address is required")]
        [Display(Name = "Email Address")]
        [StringLength(60, ErrorMessage = "Must be between 5 and 60 characters", MinimumLength = 5)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Remote(action: "VerifyEmail", controller: "Account")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "A branch is required")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "A role is required")]
        [Display(Name = "Role")]
        public int RoleId { get; set; }

        public string[] SelectedBranches { get; set; }

        public List<SelectListItem> BranchList { get; set; }

        public List<SelectListItem> RoleList { get; set; }

        public UserViewModel()
        {
            using (TextPortDA da = new TextPortDA())
            {
                this.BranchList = da.GetBranchesForDropDown(false);
                this.RoleList = da.GetRolesForDropDown();
            }
        }

        public UserViewModel(int userId)
        {
            using (TextPortDA da = new TextPortDA())
            {
                this.BranchList = da.GetBranchesForDropDown(false);
                this.RoleList = da.GetRolesForDropDown();

                Account account = da.GetAccountById(userId);
                if (account != null)
                {
                    this.AccountId = account.AccountId;
                    this.Name = account.Name;
                    this.UserName = account.UserName;
                    this.Password = account.Password;
                    this.Phone = account.Phone;
                    this.Email = account.Email;
                    this.BranchId = account.BranchId;
                    this.RoleId = account.RoleId;
                    this.Email = account.Email;

                    if (!string.IsNullOrEmpty(account.BranchIds))
                    {
                        this.SelectedBranches = account.BranchIds.Split(',');
                       
                        foreach (SelectListItem branch in this.BranchList)
                        {
                            branch.Selected = this.SelectedBranches.Contains(branch.Value) ? true : false;
                        }
                    }
                };
            }
        }
    }
}
