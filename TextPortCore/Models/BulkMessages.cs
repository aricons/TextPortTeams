using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class BulkMessages
    {
        private List<int> gridSizeOptions = new List<int>() { 5, 10, 15, 20, 50, 75, 100, 150, 200, 250 };

        public int AccountId { get; set; }

        public decimal Balance { get; set; }

        [Required(ErrorMessage = "A virtual number must be selected")]
        [Display(Name = "Send From Number")]
        public int VirtualNumberId { get; set; }

        [Display(Name = "Message Count (approx)")]
        public int MessageLimit { get; set; }

        [Display(Name = "Same Message to All Numbers")]
        public bool SameMessageToAllNumbers { get; set; }

        public List<SelectListItem> VirtualNumbers { get; set; }

        public List<SelectListItem> MessageCountOptions { get; set; }

        public List<BulkMessageItem> Messages { get; set; }

        public string SubmitType { get; set; }

        public string SubmitOperation { get; set; }

        public string ProcessingState { get; set; }

        public string BalanceAlert { get; set; }

        /* Constructors */
        public BulkMessages()
        {
            this.AccountId = 0;
            this.Balance = 0;
            this.MessageLimit = 0;
            this.SubmitType = "MANUAL";
            this.Messages = new List<BulkMessageItem>();
            this.MessageCountOptions = new List<SelectListItem>();
            this.VirtualNumbers = new List<SelectListItem>();
            this.ProcessingState = string.Empty;
            this.BalanceAlert = string.Empty;
        }

        public BulkMessages(int accId, int gridRows)
        {
            this.AccountId = accId;
            this.MessageLimit = gridRows;
            this.SubmitType = "MANUAL";
            this.Messages = new List<BulkMessageItem>();
            this.MessageCountOptions = new List<SelectListItem>();
            this.VirtualNumbers = new List<SelectListItem>();

            using (TextPortDA da = new TextPortDA())
            {
                this.Balance = da.GetAccountBalance(accId);

                List<DedicatedVirtualNumber> dvns = da.GetNumbersForAccount(accId, false);
                foreach (DedicatedVirtualNumber dvn in dvns)
                {
                    this.VirtualNumbers.Add(new SelectListItem()
                    {
                        Value = dvn.VirtualNumberId.ToString(),
                        Text = dvn.NumberDisplayFormat
                    });
                };
            }

            foreach (int opt in this.gridSizeOptions)
            {
                this.MessageCountOptions.Add(new SelectListItem()
                {
                    Value = opt.ToString(),
                    Text = opt.ToString()
                });
            };

            for (int x = 0; x < this.MessageLimit; x++)
            {
                this.Messages.Add(new BulkMessageItem());
            }

            this.ProcessingState = "PENDING";

            if (this.Balance <= (Constants.BaseSMSMessageCost * 2))
            {
                this.BalanceAlert = $"Your balance of {this.Balance:C3} is insufficient for sending messages.";
            }
        }
    }

    public class BulkMessageItem
    {
        public string Number { get; set; }
        public string MessageText { get; set; }
        public string ProcessingStatus { get; set; }
        public string ProcessingResult { get; set; }

        public bool Validate()
        {
            this.ProcessingStatus = "OK";
            string message = string.Empty;

            if (String.IsNullOrEmpty(this.Number))
            {
                message = "The number is missing";
            }
            else
            {
                if (!Utilities.IsValidNumber(this.Number))
                {
                    message = "The number is invalid";
                }
            }

            if (String.IsNullOrEmpty(this.MessageText))
            {
                message = "The message is empty.";
            }

            this.ProcessingResult = message;
            this.ProcessingStatus = (string.IsNullOrEmpty(message)) ? "OK" : "FAIL";

            return (this.ProcessingStatus == "OK") ? true : false;
        }
    }
}
