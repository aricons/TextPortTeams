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

        public Branch Branch { get; set; }

        public decimal Balance { get; set; }

        [Required(ErrorMessage = "A branch must be selected")]
        [Display(Name = "Send From Branch")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "A virtual number must be selected")]
        [Display(Name = "Send From Number")]
        public int VirtualNumberId { get; set; }

        [Display(Name = "Message Count (approx)")]
        public int MessageLimit { get; set; }

        [Display(Name = "Same Message to All Numbers")]
        public bool SameMessageToAllNumbers { get; set; }

        public List<Branch> Branches { get; set; }

        public List<DedicatedVirtualNumber> VirtualNumbers { get; set; }

        public List<SelectListItem> MessageCountOptions { get; set; }

        public List<BulkMessageItem> Messages { get; set; }

        [Required(ErrorMessage = "At least one number is required")]
        [Display(Name = "Send To")]
        [Remote(action: "VerifyNumbers", controller: "Bulk")]
        public string NumbersList { get; set; }

        [Required(ErrorMessage = "A message is required")]
        [Display(Name = "Message")]
        public string MessageText { get; set; }

        public string SubmitType { get; set; }

        public string SubmitOperation { get; set; }

        public string ProcessingState { get; set; }

        public string BalanceAlert { get; set; }

        public string Role { get; set; }

        public Account Account { get; set; }

        /* Constructors */
        public BulkMessages()
        {
            this.AccountId = 0;
            this.BranchId = 0;
            this.Balance = 0;
            this.MessageLimit = 0;
            this.SubmitType = "MANUAL";
            this.NumbersList = string.Empty;
            this.MessageText = string.Empty;
            this.Messages = new List<BulkMessageItem>();
            this.MessageCountOptions = new List<SelectListItem>();
            this.VirtualNumbers = new List<DedicatedVirtualNumber>();
            this.ProcessingState = string.Empty;
            this.BalanceAlert = string.Empty;
            this.Role = "User";
            this.Account = null;
        }

        public BulkMessages(int accId, int branchId, string role, int gridRows)
        {
            this.AccountId = accId;
            this.MessageLimit = gridRows;
            this.SubmitType = "MANUAL";
            this.Messages = new List<BulkMessageItem>();
            this.MessageText = string.Empty;
            this.Role = role;
            this.MessageCountOptions = new List<SelectListItem>();
            this.VirtualNumbers = new List<DedicatedVirtualNumber>();

            using (TextPortDA da = new TextPortDA())
            {
                this.Branch = da.GetBranchByBranchId(branchId);
                this.BranchId = branchId;
                this.Account = da.GetAccountById(accId);
                this.Balance = this.Account.Balance;
                this.VirtualNumbers = da.GetNumbersForBranch(branchId, false);

                if (this.Role == "Administrative User")
                {
                    this.Branches = da.GetAllBranches();
                }
                else if (this.Role == "General Manager")
                {
                    List<int> branchIds = this.Account.BranchIds.Split(',').Select(Int32.Parse).ToList();
                    this.Branches = this.Branches = da.GetBranchesForIds(branchIds);
                }
                else
                {
                    this.Branches = new List<Branch>() { this.Branch };
                }

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

            if (this.Balance <= (Constants.BaseSMSSegmentCost * 2))
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
        public decimal SegmentCost { get; set; }
        public int SegmentCount
        {
            get
            {
                return Utilities.GetSegmentCount(this.MessageText);
            }
        }

        public BulkMessageItem()
        {
            this.ProcessingStatus = "FAIL";
            this.ProcessingResult = string.Empty;
        }

        public BulkMessageItem(string number, string messageText)
        {
            this.Number = Utilities.NumberToE164(number, "1");
            this.MessageText = messageText;
            this.ProcessingStatus = "FAIL";
            this.ProcessingResult = string.Empty;
        }

        public bool Validate(ref decimal currentBalance)
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

            currentBalance -= (this.SegmentCost * this.SegmentCount);
            if (currentBalance <= 0)
            {
                message = "Insufficient balance to send message.";
            }

            this.ProcessingResult = message;
            this.ProcessingStatus = (string.IsNullOrEmpty(message)) ? "OK" : "FAIL";

            return (this.ProcessingStatus == "OK") ? true : false;
        }
    }
}
