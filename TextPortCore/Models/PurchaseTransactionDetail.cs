using System;

namespace TextPortCore.Models
{
    public class PurchaseTransactionDetail
    {
        public string TransactionType { get; set; }
        public int AccountId { get; set; }
        public string Description { get; set; }
        public string VirtualNumber { get; set; }
        public int CountryId { get; set; }
        public string LeasePeriod { get; set; }
        public decimal Cost { get; set; }
        public decimal Fee { get; set; }
        public decimal CreditPurchaseAmount { get; set; }

        public PurchaseTransactionDetail() { }
    }
}
