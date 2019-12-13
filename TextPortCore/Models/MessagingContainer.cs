using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using TextPortCore.Data;
using TextPortCore.Helpers;

namespace TextPortCore.Models
{
    public class MessagingContainer
    {
        private Account account;
        private List<Message> messages;
        private List<DedicatedVirtualNumber> numbers;
        private List<Contact> contacts;
        private List<Recent> recents;
        private int activeVirtualNumberId;
        private string activeDestinationNumber;

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
        public MessagingContainer(int accountId)
        {
            this.ActiveVirtualNumberId = 0;

            using (TextPortDA da = new TextPortDA())
            {
                this.Account = da.GetAccountById(accountId);
                this.Numbers = da.GetNumbersForAccount(accountId, false);

                if (this.Numbers.Any())
                {
                    this.ActiveVirtualNumberId = this.Numbers.FirstOrDefault().VirtualNumberId;
                }
                this.Contacts = da.GetContactsForAccount(accountId);
                this.Recents = da.GetRecentMessagesForAccountAndVirtualNumber(accountId, this.ActiveVirtualNumberId);
                if (this.Recents != null && this.Recents.Count > 0)
                {
                    recents.FirstOrDefault().IsActiveMessage = true;
                    this.Messages = da.GetMessagesForAccountAndRecipient(accountId, this.ActiveVirtualNumberId, this.Recents.FirstOrDefault().Number);
                    this.ActiveDestinationNumber = Utilities.NumberToE164(recents.FirstOrDefault().Number);
                }
                else
                {
                    this.Messages = new List<Message>();
                }
            }
        }
    }

}
