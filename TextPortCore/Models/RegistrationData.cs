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
        private string purchaseType;
        private string userName;
        private string emailAddress;
        private string password;
        private string confirmPassword;
        private bool chooseNumberNow;
        private int countryId;
        private string areaCode;
        private string tollFreePrefix;
        private string virtualNumber;
        private int virtualNumberId;
        private int carrierId;
        private string productDescription;
        private string leasePeriodCode;
        private string leasePeriodType;
        private short leasePeriod;
        private decimal creditCurrentBalance;
        private decimal creditPurchaseAmount;
        private decimal totalCost;
        private int accountId;
        private string status;
        private bool success;
        private string completionTitle;
        private string completionMessage;
        private string orderingMessage;
        private decimal numberCost;
        private bool showAnnouncementBanner;
        private string accountValidationKey;
        private string ipAddress;
        private string browserType;
        private bool freeTrial;
        private bool accountEnabled;
        private NumberTypes numberType;

        public string PurchaseType
        {
            get { return this.purchaseType; }
            set { this.purchaseType = value; }
        }

        [Required(ErrorMessage = "A user name is required")]
        [Display(Name = "User Name")]
        [StringLength(16, ErrorMessage = "Must be between 5 and 16 characters", MinimumLength = 5)]
        [RegularExpression(@"^\S*$", ErrorMessage = "The username cannot contain spaces")]
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

        [Required(ErrorMessage = "A toll free prefix is required")]
        [Display(Name = "Toll Free Prefix")]
        public string TollFreePrefix
        {
            get { return this.tollFreePrefix; }
            set { this.tollFreePrefix = value; }
        }

        [Required(ErrorMessage = "A number must be selected")]
        [Display(Name = "Selected Number")]
        public string VirtualNumber
        {
            get { return this.virtualNumber; }
            set { this.virtualNumber = value; }
        }

        public NumberTypes NumberType
        {
            get { return this.numberType; }
            set { this.numberType = value; }
        }

        public string NumberDisplayFormat
        {
            get { return Utilities.NumberToDisplayFormat(this.virtualNumber, this.CountryId); }
        }

        public int VirtualNumberId
        {
            get { return this.virtualNumberId; }
            set { this.virtualNumberId = value; }
        }

        [Required(ErrorMessage = "A country must be selected")]
        [Display(Name = "Country")]
        public int CountryId
        {
            get { return this.countryId; }
            set { this.countryId = value; }
        }

        [Required(ErrorMessage = "A lease period is required")]
        [Display(Name = "Keep Number for")]
        public string LeasePeriodCode
        {
            get { return this.leasePeriodCode; }
            set { this.leasePeriodCode = value; }
        }

        public string LeasePeriodType
        {
            get { return this.leasePeriodType; }
            set { this.leasePeriodType = value; }
        }

        public short LeasePeriod
        {
            get { return this.leasePeriod; }
            set { this.leasePeriod = value; }
        }

        public string LeasePeriodWord
        {
            get
            {
                switch (this.LeasePeriodType)
                {
                    case "D":
                        return (this.LeasePeriod == 1) ? "day" : "days";
                    case "W":
                        return (this.LeasePeriod == 1) ? "week" : "weeks";
                    case "Y":
                        return (this.LeasePeriod == 1) ? "year" : "years";
                    default:
                        return (this.LeasePeriod == 1) ? "month" : "months";
                }
            }
        }

        public int CarrierId
        {
            get { return this.carrierId; }
            set { this.carrierId = value; }
        }

        [Display(Name = "Account balance")]
        public decimal CreditCurrentBalance
        {
            get { return this.creditCurrentBalance; }
            set { this.creditCurrentBalance = value; }
        }

        [Required(ErrorMessage = "An amount is required")]
        [Range(typeof(decimal), "0", "1000", ErrorMessage = "An amount is required")]
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
            get { return this.productDescription; }
            set { this.productDescription = value; }
        }

        public string PayPalCustom
        {
            get
            {
                switch (this.PurchaseType)
                {
                    case "Credit":
                        return string.Format("CREDIT|{0}|{1:N2}", this.accountId, this.creditPurchaseAmount);

                    default:
                        return string.Format("VMN|{0}|{1}|{2}|{3}|{4}", this.accountId, this.VirtualNumber, this.CountryId, this.leasePeriod, this.creditPurchaseAmount);
                }
            }
        }

        public int AccountId
        {
            get { return this.accountId; }
            set { this.accountId = value; }
        }

        public string Status
        {
            get { return this.status; }
            set { this.status = value; }
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

                    case "FreeTrial":
                        return $"Free Trial";

                    case "Credit":
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

        public bool ShowAnnouncementBanner
        {
            get { return this.showAnnouncementBanner; }
            set { this.showAnnouncementBanner = value; }
        }

        public string AccountValidationKey
        {
            get { return this.accountValidationKey; }
            set { this.accountValidationKey = value; }
        }

        public string IPAddress
        {
            get { return this.ipAddress; }
            set { this.ipAddress = value; }
        }

        public string BrowserType
        {
            get { return this.browserType; }
            set { this.browserType = value; }
        }

        public bool FreeTrial
        {
            get { return this.freeTrial; }
            set { this.freeTrial = value; }
        }

        public bool AccountEnabled
        {
            get { return this.accountEnabled; }
            set { this.accountEnabled = value; }
        }

        public List<Country> CountriesList { get; set; }

        public IEnumerable<SelectListItem> NumbersList { get; set; }

        public IEnumerable<SelectListItem> LeasePeriodsList { get; set; }

        public IEnumerable<SelectListItem> CreditAmountsList { get; set; }

        public IEnumerable<SelectListItem> TollFreeAreaCodesList { get; set; }

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
            this.TollFreePrefix = string.Empty;
            this.VirtualNumber = string.Empty;
            this.VirtualNumberId = 0;
            this.CarrierId = (int)Carriers.BandWidth;
            this.LeasePeriodType = "M";
            this.LeasePeriod = 0;
            this.CreditCurrentBalance = 0;
            this.CreditPurchaseAmount = 0;
            this.AccountId = 0;
            this.Status = "Pending";
            this.Success = false;
            this.ProductDescription = string.Empty;
            this.CompletionTitle = string.Empty;
            this.CompletionMessage = string.Empty;
            this.OrderingMessage = string.Empty;
            this.ShowAnnouncementBanner = false;
            this.FreeTrial = false;
            this.AccountEnabled = false;
            this.AccountValidationKey = string.Empty;
            this.IPAddress = string.Empty;
            this.BrowserType = string.Empty;
            this.NumberType = NumberTypes.Regular;
            this.CountryId = (int)Countries.UnitedStates;

            // Initialize the numbers drop-down.
            List<SelectListItem> numbers = new List<SelectListItem>();
        }

        public RegistrationData(string purcType, int accId)
        {
            this.PurchaseType = purcType;
            this.AccountId = accId;
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.EmailAddress = string.Empty;
            this.ConfirmPassword = string.Empty;
            this.ChooseNumberNow = false;
            this.AreaCode = string.Empty;
            this.TollFreePrefix = string.Empty;
            this.VirtualNumber = string.Empty;
            this.CarrierId = (int)Carriers.BandWidth;
            this.CreditCurrentBalance = 0;
            this.CreditPurchaseAmount = 0;
            this.Status = "Pending";
            this.Success = false;
            this.ProductDescription = string.Empty;
            this.CompletionTitle = string.Empty;
            this.CompletionMessage = string.Empty;
            this.OrderingMessage = string.Empty;
            this.ShowAnnouncementBanner = false;
            this.FreeTrial = false;
            this.AccountEnabled = false;
            this.AccountValidationKey = string.Empty;
            this.IPAddress = string.Empty;
            this.BrowserType = string.Empty;
            this.NumberType = NumberTypes.Regular;
            this.CountryId = (int)Countries.UnitedStates;

            if (purcType == "ComplimentaryNumber")
            {
                this.NumberCost = 0;
                this.LeasePeriod = 1;
            }
            else if (purcType == "Credit")
            {
                this.LeasePeriod = 0;
                this.NumberCost = 0;
            }
            else if (purcType == "FreeTrial")
            {
                this.NumberCost = 0;
                this.LeasePeriodType = "W";
                this.LeasePeriod = 2;
                this.NumberType = NumberTypes.Pooled;
                this.FreeTrial = true;
                this.CreditPurchaseAmount = Constants.InitialFreeTrialBalanceAllocation;
                this.AccountValidationKey = RandomString.GenerateRandomToken(30);
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
            using (TextPortDA da = new TextPortDA())
            {
                this.CountriesList = da.GetCountriesList();
                this.LeasePeriodsList = da.GetLeasePeriods((int)Countries.UnitedStates);
                this.CreditAmountsList = da.GetCreditAmounts(purcType);
                this.TollFreeAreaCodesList = da.GetTollFreeAreaCodes();

                if (this.PurchaseType == "Credit" || this.PurchaseType == "VirtualNumber" || this.PurchaseType == "VirtualNumberRenew")
                {
                    Account acc = da.GetAccountById(AccountId);
                    if (acc != null)
                    {
                        this.CreditCurrentBalance = acc.Balance;
                    }
                }
                else if (this.PurchaseType == "FreeTrial")
                {
                    this.LeasePeriodType = "M";
                    this.LeasePeriod = 15;
                    this.NumbersList = da.GetPooledNumbersList(this.CountryId);
                }
            }
        }

    }
}
