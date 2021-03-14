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
        [Display(Name = "Send From Branch")]
        [Required(ErrorMessage = "A branch must be selected")]
        public int BranchId { get; set; }

        public int AccountId { get; set; }

        public Branch Branch { get; set; }

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

        public string Role { get; set; }

        public Account Account { get; set; }

        public List<Group> GroupsList { get; set; }

        public List<Branch> Branches { get; set; }

        public List<DedicatedVirtualNumber> VirtualNumbers { get; set; }

        public List<GroupTextResult> ResultsList { get; set; }

        public ProcessingStates ProcessingState { get; set; }


        /* Constructors */
        public GroupText()
        {
            this.BranchId = 0;
            this.AccountId = 0;
            this.GroupId = 0;
            this.Balance = 0;
            this.Message = string.Empty;
            this.BalanceAlert = string.Empty;
            this.Role = "User";
            this.ProcessingState = ProcessingStates.Unprocessed;
            this.GroupsList = new List<Group>();
            this.VirtualNumbers = new List<DedicatedVirtualNumber>();
            this.ResultsList = new List<GroupTextResult>();
            this.Account = null;
        }

        public GroupText(int branchId, int accountId, string role)
        {
            this.BranchId = branchId;
            this.AccountId = accountId;
            this.GroupId = 0;
            this.Message = string.Empty;
            this.Role = role;
            this.ProcessingState = ProcessingStates.Unprocessed;
            this.GroupsList = new List<Group>();
            this.VirtualNumbers = new List<DedicatedVirtualNumber>();
            this.ResultsList = new List<GroupTextResult>();

            using (TextPortDA da = new TextPortDA())
            {
                this.Balance = 0;
                this.Branch = da.GetBranchByBranchId(branchId);
                this.Account = da.GetAccountById(accountId);
                this.GroupsList = da.GetGroupsForBranch(branchId);
                if (this.GroupsList != null)
                {
                    if (this.GroupsList.Count() > 0)
                    {
                        this.GroupId = this.GroupsList.FirstOrDefault().GroupId;
                    }
                }
                else
                {
                    //this.GroupsList = new List<SelectListItem>();
                    //this.GroupsList.Add(new SelectListItem()
                    //{
                    //    Value = "",
                    //    Text = "No groups defined."
                    //});
                }

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
        }
    }

    public class GroupTextResult
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public string ProcessingMessage { get; set; }
        public string Result { get; set; }

        public GroupTextResult(string memberName, string memberNumber, string result, bool isStopped)
        {
            this.Number = memberNumber;
            this.Name = memberName;
            this.Result = result;
            if (isStopped)
            {
                this.ProcessingMessage = $"OPT-OUT: The recipient {memberName} at number {memberNumber} has opted out of text notifications.";
            }
            else
            {
                this.ProcessingMessage = $"Message queued to {memberName} at {memberNumber}";
            }
        }
    }
}
