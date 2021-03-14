using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.ComponentModel.DataAnnotations;

using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class BranchesContainer
    {
        public int AccountId { get; set; }

        [Display(Name = "Select a Branch")]
        public int CurrentBranchId { get; set; }

        [Display(Name = "Branch Name")]
        public Branch CurrentBranch { get; set; }

        public IEnumerable<SelectListItem> BranchesList { get; set; }

        public string StatusMessage { get; set; }

        public RequestStatus Status { get; set; }

        public int VirtualNumberId { get; set; }

        // Constructors
        public BranchesContainer()
        {
            this.AccountId = 0;
            this.CurrentBranchId = 0;
            this.CurrentBranch = new Branch();
            this.BranchesList = new List<SelectListItem>();
            this.StatusMessage = string.Empty;
            this.Status = RequestStatus.Pending;
            this.VirtualNumberId = 0;
        }

        public BranchesContainer(int accountId, int currentBranchId, int virtualNumberId)
        {
            this.AccountId = accountId;
            this.CurrentBranchId = 0;
            this.CurrentBranch = new Branch();
            this.StatusMessage = string.Empty;
            this.Status = RequestStatus.Pending;
            this.VirtualNumberId = virtualNumberId;

            using (TextPortDA da = new TextPortDA())
            {
                this.BranchesList = da.GetBranchesForDropDown(true);

                if (currentBranchId > 0)
                {
                    this.CurrentBranchId = currentBranchId;
                    this.CurrentBranch = da.GetBranchByBranchId(this.CurrentBranchId);
                    this.StatusMessage = "Branch Applied Successfully";
                    this.Status = RequestStatus.Success;
                }
                else
                {
                    if (currentBranchId == -1)
                    {
                        this.StatusMessage = "Branch Removed Successfully";
                        this.Status = RequestStatus.Success;
                        this.CurrentBranchId = 0;
                    };

                    if (this.BranchesList.Count() > 1)
                    {
                        this.CurrentBranchId = Convert.ToInt32(this.BranchesList.Skip(1).FirstOrDefault().Value);
                        this.CurrentBranch = da.GetBranchByBranchId(this.CurrentBranchId);
                    }
                }

                //if (BranchesList.Count() == 1)
                //{
                //    CurrentApplication.APIToken = $"{this.AccountId}-{RandomString.GenerateRandomToken(10)}";
                //    CurrentApplication.APISecret = RandomString.GenerateRandomToken(20);
                //}
            }
        }
    }
}
