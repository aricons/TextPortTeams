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
        public int AccountId { get; set; }

        //[Display(Name = "Select a Group")]
        //public int CurrentGroupId { get; set; }

        //[Display(Name = "Group Name")]
        //public Group CurrentGroup { get; set; }

        public List<Contact> Contacts { get; set; }


        // Constructors
        public ContactsContainer()
        {
            this.AccountId = 0;
            this.Contacts = new List<Contact>();
            //this.CurrentContactId = 0;
            //this.CurrentContact = new Contact();
        }

        public ContactsContainer(int accountId)
        {
            this.AccountId = accountId;
            //this.CurrentGroupId = 0;
            //this.CurrentGroup = new Group();

            using (TextPortDA da = new TextPortDA())
            {
                this.Contacts = da.GetContactsForAccount(accountId);

                //if (this.GroupsList.Count() > 0)
                //{
                //    this.CurrentGroupId = Convert.ToInt32(this.GroupsList.FirstOrDefault().Value);
                //}

                //if (this.CurrentGroupId > 0)
                //{
                //    this.CurrentGroup.Members = da.GetMembersForGroup(this.CurrentGroupId);
                //}
            }
        }

        //public ContactsContainer(Group newGroup)
        //{
        //    using (TextPortDA da = new TextPortDA())
        //    {
        //        this.AccountId = newGroup.AccountId;
        //        this.CurrentGroupId = newGroup.GroupId;
        //        this.GroupsList = da.GetGroupsList(newGroup.AccountId);
        //        this.CurrentGroup = new Group();
        //        if (this.CurrentGroupId > 0)
        //        {
        //            this.CurrentGroup.Members = da.GetMembersForGroup(this.CurrentGroupId);
        //        }
        //    }
        //}
        //}
    }

    //public class DeleteGroupMemberRequest
    //{
    //    public int GroupId { get; set; }
    //    public int MemberId { get; set; }
    //}
}
