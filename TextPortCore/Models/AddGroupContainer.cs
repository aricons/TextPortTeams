using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using TextPortCore.Data;

namespace TextPortCore.Models
{
    public partial class AddGroupContainer
    {
        public int GroupId { get; set; }

        public int BranchId { get; set; }

        public string Role { get; set; }

        public Branch Branch { get; set; }

        public Account Account { get; set; }

        public List<Branch> Branches { get; set; }

        [Required(ErrorMessage = "A group name is required")]
        [Display(Name = "Group Name")]
        [StringLength(30, ErrorMessage = "Must be between 1 and 30 characters", MinimumLength = 1)]
        public string GroupName { get; set; }

        public List<GroupMember> Members { get; set; }

        // Constructors
        public AddGroupContainer()
        {
            this.GroupId = 0;
            this.BranchId = 0;
            this.Branches = new List<Branch>();
            this.GroupName = string.Empty;
            this.Members = new List<GroupMember>();
            this.Account = null;
            this.Branch = null;
        }

        public AddGroupContainer(int branchId, int accountId, string role)
        {
            this.Role = role;
            using (TextPortDA da = new TextPortDA())
            {
                this.Branch = da.GetBranchByBranchId(branchId);
                this.Account = da.GetAccountById(accountId);

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
}
