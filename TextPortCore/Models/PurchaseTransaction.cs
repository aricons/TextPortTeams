using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TextPortCore.Models
{
    public partial class PurchaseTransaction
    {
        public int PurchaseId { get; set; }

        public string PaymentService { get; set; }

        public string TransactionId { get; set; }

        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }

        [Display(Name = "Transaction Type")]
        public string TransactionType { get; set; }

        [Display(Name = "Description")]
        public string ItemPurchased { get; set; }

        public int AccountId { get; set; }

        public string ReceiverId { get; set; }

        [Display(Name = "Amount")]
        public decimal GrossAmount { get; set; }

        [Display(Name = "Fee")]
        public decimal Fee { get; set; }
    }
}
