using System;
using System.Linq;
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

        private string purchaseType;
        private string userName;
        private string emailAddress;
        private string password;
        private string confirmPassword;
        private bool chooseNumberNow;
        private int numberCountryId;
        private string areaCode;
        private string virtualNumber;
        private int virtualNumberId;
        private string numberProvider;
        private int leasePeriod;
        private decimal creditCurrentBalance;
        private decimal creditPurchaseAmount;
        private decimal numberCost;
        private decimal totalCost;
        private int accountId;
        private bool success;
        private string completionTitle;
        private string completionMessage;
        private string orderingMessage;

        public string PurchaseType
        {
            get { return this.purchaseType; }
            set { this.purchaseType = value; }
        }

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

        public string NumberDisplayFormat
        {
            get { return Utilities.NumberToDisplayFormat(this.virtualNumber, 22); }
        }

        public int VirtualNumberId
        {
            get { return this.virtualNumberId; }
            set { this.virtualNumberId = value; }
        }

        [Required(ErrorMessage = "A country must be selected")]
        [Display(Name = "Country")]
        public int NumberCountryId
        {
            get { return this.numberCountryId; }
            set { this.numberCountryId = value; }
        }

        [Required(ErrorMessage = "A lease period is required")]
        [Range(typeof(int), "1", "200", ErrorMessage = "A lease period is required")]
        [Display(Name = "Keep Number for")]
        public int LeasePeriod
        {
            get { return this.leasePeriod; }
            set { this.leasePeriod = value; }
        }

        public string LeasePeriodWord
        {
            get { return (this.LeasePeriod == 1) ? "month" : "months"; }
        }

        public string NumberProvider
        {
            get { return this.numberProvider; }
            set { this.numberProvider = value; }
        }

        [Display(Name = "Account balance")]
        public decimal CreditCurrentBalance
        {
            get { return this.creditCurrentBalance; }
            set { this.creditCurrentBalance = value; }
        }

        [Required(ErrorMessage = "An amount is required")]
        [Range(typeof(decimal), "1", "1000", ErrorMessage = "An amount is required")]
        [Display(Name = "Amount to Add")]
        public decimal CreditPurchaseAmount
        {
            get { return this.creditPurchaseAmount; }
            set { this.creditPurchaseAmount = value; }
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

        public string ProductDescription
        {
            get
            {
                string cost = (this.TotalCost > 0) ? $" {this.TotalCost:C2}" : string.Empty;
                switch (this.PurchaseType)
                {
                    case "VirtualNumberRenew":
                        return $"TextPort virtual number {this.NumberDisplayFormat}, {this.LeasePeriod} month lease renewal.{cost}";

                    case "Credits":
                        return $"Credit value{cost}.";

                    default: // VirtualNumberSignUp and VirtualNumber
                        return $"TextPort virtual number {this.NumberDisplayFormat}, {this.LeasePeriod} month lease.{cost}";
                }
            }
        }

        public string PayPalCustom
        {
            get { return string.Format("VMN|{0}|{1}|{2}|{3}|{4}", this.accountId, this.VirtualNumber, this.numberCountryId, this.leasePeriod, this.creditPurchaseAmount); }
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

        public string PurchaseTitle
        {
            get
            {
                switch (this.PurchaseType)
                {
                    case "VirtualNumberRenew":
                        return $"Renew Number {this.NumberDisplayFormat}";

                    case "Credits":
                        string cost = (this.TotalCost > 0) ? $" {this.TotalCost:C2}" : string.Empty;
                        return $"Add{cost} TextPort Credit.";

                    default: // VirtualNumberSignUp and VirtualNumber
                        return $"Register Number {this.NumberDisplayFormat}";
                }
            }
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

        public string OrderingMessage
        {
            get { return this.orderingMessage; }
            set { this.orderingMessage = value; }
        }

        public IEnumerable<SelectListItem> CountriesList { get; set; }

        public IEnumerable<SelectListItem> NumbersList { get; set; }

        public IEnumerable<SelectListItem> LeasePeriodsList { get; set; }

        public IEnumerable<SelectListItem> CreditAmountsList { get; set; }


        // Constructors
        public RegistrationData()
        {
            this.PurchaseType = string.Empty;
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.EmailAddress = string.Empty;
            this.ConfirmPassword = string.Empty;
            this.ChooseNumberNow = false;
            this.AreaCode = string.Empty;
            this.VirtualNumber = string.Empty;
            this.VirtualNumberId = 0;
            this.NumberProvider = "Bandwidth";
            this.NumberCost = 6;
            this.LeasePeriod = 1;
            this.CreditCurrentBalance = 0;
            this.CreditPurchaseAmount = 0;
            this.AccountId = 0;
            this.Success = false;
            this.CompletionTitle = string.Empty;
            this.CompletionMessage = string.Empty;
            this.OrderingMessage = string.Empty;

            // Initialize the numbers drop-down.
            List<SelectListItem> numbers = new List<SelectListItem>();
        }

        public RegistrationData(TextPortContext context, string purcType, int accId)
        {
            this._context = context;

            this.PurchaseType = purcType;
            this.AccountId = accId;
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.EmailAddress = string.Empty;
            this.ConfirmPassword = string.Empty;
            this.ChooseNumberNow = false;
            this.AreaCode = string.Empty;
            this.VirtualNumber = string.Empty;
            this.VirtualNumberId = 0;
            this.NumberProvider = "Bandwidth";
            this.NumberCost = 6;
            this.LeasePeriod = 1;
            this.CreditCurrentBalance = 0;
            this.CreditPurchaseAmount = 0;
            this.Success = false;
            this.CompletionTitle = string.Empty;
            this.CompletionMessage = string.Empty;
            this.OrderingMessage = string.Empty;

            if (purcType == "ComplimentaryNumber")
            {
                this.NumberCost = 0;
                this.LeasePeriod = 1;
            }

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
                this.LeasePeriodsList = da.GetLeasePeriods((purcType == "ComplimentaryNumber"));
                this.CreditAmountsList = da.GetCreditAmounts();
            }

            if (this.PurchaseType == "Credits")
            {
                Account acc = _context.Accounts.FirstOrDefault(x => x.AccountId == this.AccountId);
                if (acc != null)
                {
                    this.CreditCurrentBalance = acc.Credits;
                }
            }
        }
    }
}
