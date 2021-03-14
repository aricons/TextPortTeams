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
    public class ContactsContainer
    {
        [Display(Name = "Branch")]
        [Required(ErrorMessage = "A branch must be selected")]
        public int BranchId { get; set; }

        public string Role { get; set; }

        public Branch Branch { get; set; }

        public Account Account { get; set; }

        public List<Branch> Branches { get; set; }

        public List<Contact> Contacts { get; set; }


        // Constructors
        public ContactsContainer()
        {
            this.BranchId = 0;
            this.Contacts = new List<Contact>();
            this.Branch = null;
            this.Account = null;
        }

        public ContactsContainer(int branchId, int accountId, string role)
        {
            this.BranchId = branchId;
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

                this.Contacts = da.GetContactsForBranch(branchId);
            }
        }


    }

    //public class DeleteGroupMemberRequest
    //{
    //    public int GroupId { get; set; }
    //    public int MemberId { get; set; }
    //}
}
