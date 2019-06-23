using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class GroupText
    {
        public int AccountId { get; set; }

        [Display(Name = "Send From Number")]
        [Required(ErrorMessage = "A number must be selected")]
        public int VirtualNumberId { get; set; }

        [Display(Name = "Select a Group")]
        [Required(ErrorMessage = "A group must be selected")]
        public int GroupId { get; set; }

        [Display(Name = "Message")]
        [Required(ErrorMessage = "The message cannot be empty")]
        public string Message { get; set; }

        public decimal Balance { get; set; }

        public string BalanceAlert { get; set; }

        public List<SelectListItem> GroupsList { get; set; }

        public List<SelectListItem> VirtualNumbers { get; set; }

        public List<GroupTextResult> ResultsList { get; set; }

        public ProcessingStates ProcessingState { get; set; }


        /* Constructors */
        public GroupText()
        {
            this.AccountId = 0;
            this.GroupId = 0;
            this.Balance = 0;
            this.Message = string.Empty;
            this.BalanceAlert = string.Empty;
            this.ProcessingState = ProcessingStates.Unprocessed;
            this.GroupsList = new List<SelectListItem>();
            this.VirtualNumbers = new List<SelectListItem>();
            this.ResultsList = new List<GroupTextResult>();
        }

        public GroupText(int accountId)
        {
            this.AccountId = accountId;
            this.GroupId = 0;
            this.Message = string.Empty;
            this.ProcessingState = ProcessingStates.Unprocessed;
            this.GroupsList = new List<SelectListItem>();
            this.VirtualNumbers = new List<SelectListItem>();
            this.ResultsList = new List<GroupTextResult>();

            using (TextPortDA da = new TextPortDA())
            {
                this.Balance = da.GetAccountBalance(accountId);

                this.GroupsList = da.GetGroupsList(accountId);
                if (this.GroupsList != null)
                {
                    if (this.GroupsList.Count() > 0)
                    {
                        this.GroupId = Convert.ToInt32(this.GroupsList.FirstOrDefault().Value);
                    }
                }
                else
                {
                    this.GroupsList = new List<SelectListItem>();
                    this.GroupsList.Add(new SelectListItem()
                    {
                        Value = "",
                        Text = "No groups defined."
                    });
                }

                List<DedicatedVirtualNumber> dvns = da.GetNumbersForAccount(accountId, false);
                foreach (DedicatedVirtualNumber dvn in dvns)
                {
                    this.VirtualNumbers.Add(new SelectListItem()
                    {
                        Value = dvn.VirtualNumberId.ToString(),
                        Text = dvn.NumberDisplayFormat
                    });
                };

                if (this.Balance <= (Constants.BaseSMSMessageCost * 2))
                {
                    this.BalanceAlert = $"Your balance of {this.Balance:C3} is insufficient for sending messages.";
                }
            }
        }
    }

    public class GroupTextResult
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public string ProcessingMessage { get; set; }
        public string Result { get; set; }

        public GroupTextResult(string memberName, string memberNumber, string result)
        {
            this.Number = memberNumber;
            this.Name = memberName;
            this.Result = result;
            this.ProcessingMessage = $"Message queued to {memberName} at {memberNumber}";
        }
    }
}
