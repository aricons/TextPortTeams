using System;

using TextPortCore.Models;

namespace TextPortCore.Integrations.Coinbase
{
    public class charge
    {
        public string name { get; set; }
        public string description { get; set; }
        public LocalPrice local_price { get; set; }
        public string pricing_type { get; set; }
        public Metadata metadata { get; set; }
        public string redirect_url { get; set; }
        public string cancel_url { get; set; }


        // Constructors
        public charge(RegistrationData regData)
        {
            this.name = "TextPort";
            this.description = regData.ProductDescription;
            this.pricing_type = "fixed_price";
            this.local_price = new LocalPrice()
            {
                amount = $"{regData.TotalCost:N2}",
                currency = "USD"
            };
            this.metadata = new Metadata()
            {
                account_id = regData.BranchId.ToString(),
                virtual_number = regData.VirtualNumber,
                number_cost = regData.NumberCost,
                country_id = regData.CountryId,
                lease_period = regData.LeasePeriod,
                lease_period_type = regData.LeasePeriodType,
                lease_period_word = regData.LeasePeriodWord,
                credit_amount = regData.CreditPurchaseAmount,
                purchase_code = regData.PayPalCustom
            };
        }
    }

    public class LocalPrice
    {
        public string amount { get; set; }
        public string currency { get; set; }
    }

    public class Metadata
    {
        public string account_id { get; set; }
        public string virtual_number { get; set; }
        public decimal number_cost { get; set; }
        public int country_id { get; set; }
        public int lease_period { get; set; }
        public string lease_period_type { get; set; }
        public decimal credit_amount { get; set; }
        public string lease_period_word { get; set; }
        public string purchase_code { get; set; }
    }
}



