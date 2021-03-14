using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using TextPortCore.Data;

namespace TextPortCore.Models
{
    public class GroupsContainer
    {
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        public string Role { get; set; }

        public Branch Branch { get; set; }

        public Account Account { get; set; }

        [Display(Name = "Groups")]
        public int CurrentGroupId { get; set; }

        [Display(Name = "Group Name")]
        public Group CurrentGroup { get; set; }

        public List<Branch> Branches { get; set; }

        public List<Group> GroupsList { get; set; }


        // Constructors
        public GroupsContainer()
        {
            this.BranchId = 0;
            this.Branch = null;
            this.Account = null;
            this.Role = "User";
            this.CurrentGroupId = 0;
            this.CurrentGroup = new Group();
        }

        public GroupsContainer(int branchId, int accountId, string role)
        {
            this.BranchId = branchId;
            this.Role = role;
            this.CurrentGroupId = 0;
            this.CurrentGroup = new Group();

            using (TextPortDA da = new TextPortDA())
            {
                this.Account = da.GetAccountById(accountId);
                this.Branch = da.GetBranchByBranchId(branchId);
                this.GroupsList = da.GetGroupsForBranch(branchId);

                if (this.Role == "Administrative User")
                {
                    this.Branches = this.Branches = da.GetAllBranches();
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

                if (this.GroupsList.Count() > 0)
                {
                    this.CurrentGroupId = Convert.ToInt32(this.GroupsList.FirstOrDefault().GroupId);
                }

                if (this.CurrentGroupId > 0)
                {
                    this.CurrentGroup.Members = da.GetMembersForGroup(this.CurrentGroupId);
                }
            }
        }

        public GroupsContainer(Group newGroup, int accountId, string role)
        {
            using (TextPortDA da = new TextPortDA())
            {
                this.Role = role;
                this.Account = da.GetAccountById(accountId);
                this.BranchId = newGroup.BranchId;
                this.Branch = da.GetBranchByBranchId(this.BranchId);
                this.CurrentGroupId = newGroup.GroupId;
                this.GroupsList = da.GetGroupsForBranch(newGroup.BranchId);
                this.CurrentGroup = new Group();
                if (this.CurrentGroupId > 0)
                {
                    this.CurrentGroup.Members = da.GetMembersForGroup(this.CurrentGroupId);
                }

                if (this.Role == "Administrative User")
                {
                    this.Branches = this.Branches = da.GetAllBranches();
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

    public class DeleteGroupMemberRequest
    {
        public int GroupId { get; set; }
        public int MemberId { get; set; }
    }
}
