using System;

using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class PurchaseCustomField
    {
        public string TransactionType { get; set; }
        public int AccountId { get; set; }
        public int CountryId { get; set; }
        public string VirtualNumber { get; set; }
        public int LeasePeriod { get; set; }
        public string LeasePeriodWord { get; set; }
        public decimal CreditPurchaseAmount { get; set; }

        public PurchaseCustomField(string[] customFields)
        {
            if (customFields != null)
            {
                if (customFields.Length > 0)
                {
                    TransactionType = customFields[0];
                    AccountId = int.Parse(customFields[1]);

                    if (TransactionType == "VMN")
                    {
                        CountryId = int.Parse(customFields[3]);
                        VirtualNumber = Utilities.NumberToDisplayFormat(customFields[2], CountryId);
                        LeasePeriod = int.Parse(customFields[4]);
                        CreditPurchaseAmount = decimal.Parse(customFields[5]);
                        LeasePeriodWord = (customFields.Length > 6) ? customFields[6] : string.Empty;
                    }
                    else // credit only
                    {
                        CreditPurchaseAmount = decimal.Parse(customFields[2]);
                    }
                }
            }
        }
        //    // return string.Format("CREDIT|{0}|{1:N2}", this.AccountId, this.CreditPurchaseAmount);
        //    return string.Format("VMN|{0}|{1}|{2}|{3}|{4}", this.AccountId, this.VirtualNumber, this.CountryId, this.LeasePeriod, this.CreditPurchaseAmount);
    }
}
