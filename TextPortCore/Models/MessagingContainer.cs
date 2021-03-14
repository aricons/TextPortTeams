using System;
using System.Linq;
using System.Collections.Generic;

using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class MessagingContainer
    {
        private Branch branch;
        private Account account;
        private List<Message> messages;
        private List<Branch> branches;
        private List<DedicatedVirtualNumber> numbers;
        private List<Contact> contacts;
        private List<Recent> recents;
        private string role;
        private int activeVirtualNumberId;
        private string activeDestinationNumber;

        public Branch Branch
        {
            get { return this.branch; }
            set { this.branch = value; }
        }

        public Account Account
        {
            get { return this.account; }
            set { this.account = value; }
        }

        public List<Message> Messages
        {
            get { return this.messages; }
            set { this.messages = value; }
        }

        public List<Branch> Branches
        {
            get { return this.branches; }
            set { this.branches = value; }
        }

        public List<DedicatedVirtualNumber> Numbers
        {
            get { return this.numbers; }
            set { this.numbers = value; }
        }

        public List<Contact> Contacts
        {
            get { return this.contacts; }
            set { this.contacts = value; }
        }

        public List<Recent> Recents
        {
            get { return this.recents; }
            set { this.recents = value; }
        }

        public string Role
        {
            get { return this.role; }
            set { this.role = value; }
        }

        public int ActiveVirtualNumberId
        {
            get { return this.activeVirtualNumberId; }
            set { this.activeVirtualNumberId = value; }
        }

        public string ActiveDestinationNumber
        {
            get { return this.activeDestinationNumber; }
            set { this.activeDestinationNumber = value; }
        }

        public int VirtualNumberCount
        {
            get
            {
                if (this.Numbers != null)
                {
                    if (this.Numbers.Any())
                    {
                        return (this.Numbers.Count);
                    }
                }
                return 0;
            }
        }


        // Constructors
        public MessagingContainer(int branchId, int accountId, string role)
        {
            this.ActiveVirtualNumberId = 0;
            this.Role = role;

            using (TextPortDA da = new TextPortDA())
            {
                this.Branch = da.GetBranchByBranchId(branchId);
                this.Account = da.GetAccountById(accountId);
                this.Numbers = da.GetNumbersForBranch(branchId, false);

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

                if (this.Numbers.Any())
                {
                    this.ActiveVirtualNumberId = this.Numbers.FirstOrDefault().VirtualNumberId;
                }
                this.Contacts = da.GetContactsForBranch(branchId);
                this.Recents = da.GetRecentMessagesForBranchAndVirtualNumber(branchId, this.ActiveVirtualNumberId);
                if (this.Recents != null && this.Recents.Count > 0)
                {
                    recents.FirstOrDefault().IsActiveMessage = true;
                    this.Messages = da.GetMessagesForBranchAndRecipient(branchId, this.ActiveVirtualNumberId, this.Recents.FirstOrDefault().Number);
                    this.ActiveDestinationNumber = Utilities.NumberToE164(recents.FirstOrDefault().Number, "1");
                }
                else
                {
                    this.Messages = new List<Message>();
                }
            }
        }
    }

}
