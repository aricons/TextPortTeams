using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class RegistrationData
    {
        private readonly TextPortContext _context;

        private string userName;
        private string emailAddress;
        private string password;
        private string confirmPassword;
        private bool chooseNumberNow;
        private int numberCountryId;
        private string areaCode;
        private string virtualNumber;
        private string numberProvider;
        private int leasePeriod;
        private int creditCount;
        private decimal numberCost;
        private decimal totalCost;
        private int accountId;
        private bool success;

        [Required(ErrorMessage = "A user name is required")]
        [Display(Name = "User Name")]
        [StringLength(16, ErrorMessage = "Must be between 5 and 16 characters", MinimumLength = 5)]
        [Remote(action: "VerifyUsername", controller: "Account")]
        public String UserName
        {
            get { return this.userName; }
            set { this.userName = value; }
        }

        [Required(ErrorMessage = "An email address is required")]
        [Display(Name = "Email Address")]
        [StringLength(60, ErrorMessage = "Must be between 5 and 60 characters", MinimumLength = 5)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Remote(action: "VerifyEmail", controller: "Account")]
        public String EmailAddress
        {
            get { return this.emailAddress; }
            set { this.emailAddress = value; }
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

        [Required(ErrorMessage = "A password is required")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [StringLength(60, ErrorMessage = "Must be between 5 and 60 characters", MinimumLength = 5)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The passwords do not match")]
        public string ConfirmPassword
        {
            get { return this.confirmPassword; }
            set { this.confirmPassword = value; }
        }

        public bool ChooseNumberNow
        {
            get { return this.chooseNumberNow; }
            set { this.chooseNumberNow = value; }
        }

        [Required(ErrorMessage = "An area code is required")]
        [Display(Name = "Area Code")]
        [StringLength(3, ErrorMessage = "Must be 3 characters long", MinimumLength = 3)]
        public string AreaCode
        {
            get { return this.areaCode; }
            set { this.areaCode = value; }
        }

        [Required(ErrorMessage = "A number must be selected")]
        [Display(Name = "Selected Number")]
        public string VirtualNumber
        {
            get { return this.virtualNumber; }
            set { this.virtualNumber = value; }
        }

        public string VirtualNumberGlobalFormat
        {
            get { return Utilities.NumberToGlobalFormat(this.virtualNumber); }
        }

        [Required(ErrorMessage = "A country must be selected")]
        [Display(Name = "Country")]
        public int NumberCountryId
        {
            get { return this.numberCountryId; }
            set { this.numberCountryId = value; }
        }

        [Required(ErrorMessage = "A lease period is required")]
        [Display(Name = "Keep Number for")]
        public int LeasePeriod
        {
            get { return this.leasePeriod; }
            set { this.leasePeriod = value; }
        }

        public string NumberProvider
        {
            get { return this.numberProvider; }
            set { this.numberProvider = value; }
        }

        public int CreditCount
        {
            get { return this.creditCount; }
            set { this.creditCount = value; }
        }

        public decimal NumberCost
        {
            get { return this.numberCost; }
            set { this.numberCost = value; }
        }

        [Display(Name = "Total Cost")]
        public decimal TotalCost
        {
            get { return this.totalCost; }
            set { this.totalCost = value; }
        }

        public string FullNumber
        {
            get
            {
                string numberStripped = this.virtualNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                return string.Format("1{0}", numberStripped);
            }
        }

        public string ProductDescription
        {
            get { return string.Format("TextPort virtual number {0}, {1} month lease", this.virtualNumber, "1"); }
        }

        public string PayPalCustom
        {
            get { return string.Format("VMN|{0}|{1}|{2}|{3}|{4}", this.accountId, this.FullNumber, this.numberCountryId, this.leasePeriod, this.creditCount); }
        }

        public int AccountId
        {
            get { return this.accountId; }
            set { this.accountId = value; }
        }

        public bool Success
        {
            get { return this.success; }
            set { this.success = value; }
        }

        public IEnumerable<SelectListItem> CountriesList { get; set; }

        public IEnumerable<SelectListItem> NumbersList { get; set; }

        public IEnumerable<SelectListItem> LeasePeriodsList { get; set; }


        // Constructors
        public RegistrationData()
        {
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.EmailAddress = string.Empty;
            this.ConfirmPassword = string.Empty;
            this.ChooseNumberNow = false;
            this.AreaCode = string.Empty;
            this.VirtualNumber = string.Empty;
            this.NumberProvider = "Bandwidth";
            this.NumberCost = 6;
            this.LeasePeriod = 1;
            this.CreditCount = 100;
            this.AccountId = 0;
            this.Success = false;

            // Initialize the numbers drop-down.
            List<SelectListItem> numbers = new List<SelectListItem>();
        }

        public RegistrationData(TextPortContext context)
        {
            this._context = context;

            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.EmailAddress = string.Empty;
            this.ConfirmPassword = string.Empty;
            this.ChooseNumberNow = false;
            this.AreaCode = string.Empty;
            this.VirtualNumber = string.Empty;
            this.NumberProvider = "Bandwidth";
            this.NumberCost = 6;
            this.LeasePeriod = 1;
            this.CreditCount = 100;
            this.AccountId = 0;
            this.Success = false;

            // Initialize the numbers drop-down.
            List<SelectListItem> numbers = new List<SelectListItem>();
            SelectListItem firstItem = new SelectListItem()
            {
                Value = null,
                Text = "Select number..."
            };
            numbers.Insert(0, firstItem);
            this.NumbersList = numbers;

            // Initialize number countries drop-down
            using (TextPortDA da = new TextPortDA(_context))
            {
                this.CountriesList = da.GetNumberCountriesList();
                this.LeasePeriodsList = da.GetLeasePeriods();
            }
        }
    }
}
