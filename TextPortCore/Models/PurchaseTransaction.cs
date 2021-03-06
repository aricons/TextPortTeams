﻿using System;
using System.ComponentModel.DataAnnotations;

using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public partial class PurchaseTransaction
    {
        public int PurchaseId { get; set; }

        public string PaymentService { get; set; }

        public string TransactionId { get; set; }

        public TransactionStatus Status { get; set; }

        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }

        [Display(Name = "Transaction Type")]
        public string TransactionType { get; set; }

        [Display(Name = "Item Code")]
        public string ItemPurchased { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public int AccountId { get; set; }

        public string ReceiverId { get; set; }

        [Display(Name = "Amount")]
        public decimal GrossAmount { get; set; }

        [Display(Name = "Fee")]
        public decimal Fee { get; set; }

        public string ReservedNumber { get; set; }

        public string NumberReservationId { get; set; }
    }
}
