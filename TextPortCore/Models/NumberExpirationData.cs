using System;
using System.Collections.Generic;

using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class NumberExpirationData
    {
        public int VirtualNumberID { get; set; }
        public string VirtualNumber { get; set; }
        public int AccountID { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int DaysUntilExpiration { get; set; }
        public int HoursUntilExpiration { get; set; }
        public string UserName { get; set; }
        public int NumberType { get; set; }
        public string CountryCode { get; set; }
        public int CarrierId { get; set; }
        public bool AutoRenew { get; set; }
        public string NotificationType { get; set; }
        public string EmailAction { get; set; }
        public decimal Fee { get; set; }
        public string LeasePeriodType { get; set; }
        public short LeasePeriod { get; set; }
        public string VirtualNumberDisplay
        {
            get
            {
                return Utilities.NumberToDisplayFormat(this.VirtualNumber, 22);
            }
        }
        public string ActionUrl
        {
            get
            {
                return this.generateActionUrl();
            }
        }
        public string ExpirationDaysAndHours
        {
            get
            {
                string s = string.Empty;
                if (this.DaysUntilExpiration > 0)
                {
                    s += $"{this.DaysUntilExpiration}";
                    s += (this.DaysUntilExpiration == 1) ? " day and " : " days and ";
                }

                s += $"{this.HoursUntilExpiration}";
                s += (this.HoursUntilExpiration == 1) ? " hour" : " hours";

                return s;
            }
        }


        public NumberExpirationData()
        {
            DaysUntilExpiration = 0;
            HoursUntilExpiration = 0;
            AccountID = 0;
            Email = string.Empty;
            Balance = 0;
            ExpirationDate = DateTime.MinValue;
            UserName = string.Empty;
            VirtualNumber = string.Empty;
            NumberType = 1;
            VirtualNumberID = 0;
            CountryCode = string.Empty;
            CarrierId = 0;
            AutoRenew = false;
            EmailAction = string.Empty;
            Fee = 0;
            LeasePeriodType = string.Empty;
            LeasePeriod = 0;
        }

        private string generateActionUrl()
        {
            //string linkBase = $"https://textport.com/{this.EmailAction}";
            //string linkKey = Uri.EscapeDataString(AESEncryptDecrypt.Encrypt($"{this.AccountID}|{this.VirtualNumberID}", Constants.RC4Key));

            //return $"{linkBase}?id={linkKey}";

            //string linkBase = $"https://textport.com/{this.EmailAction}";
            //string leader = RandomString.GenerateRandomTokenNoJ(8);
            //string firstFiller = RandomString.GenerateRandomTokenNoJ(5);
            //string trailer = RandomString.GenerateRandomTokenNoJ(7);

            //string linkKey = $"{leader}J{this.AccountID + Constants.AccountIdScramblerOffset}J{firstFiller}J{this.VirtualNumberID}J{trailer}";

            string linkBase = $"https://textport.com/{this.EmailAction}";
            string linkKey = RandomString.GenerateJDelimitedUrlParameter(new List<int>() { this.AccountID, this.VirtualNumberID });

            return $"{linkBase}/{linkKey}";
        }
    }
}
