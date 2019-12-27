using System;
using System.Linq;
using System.Collections.Generic;

using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPortCore.Data
{
    public partial class TextPortDA
    {
        #region "Select Methods"

        public List<Contact> GetContactsForAccount(int accountId)
        {
            try
            {
                return _context.Contacts.Where(x => x.AccountId == accountId).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("ContactsDA.GetContactsForAccount", ex);
            }

            return null;
        }

        #endregion


        #region "Insert Methods"

        public bool AddContact(Contact contact)
        {
            try
            {
                _context.Contacts.Add(contact);
                _context.SaveChanges();

                int foo = contact.ContactId;

                return (foo > 0);
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("ContactsDA.AddContact", ex);
            }
            return false;
        }

        #endregion

        #region "Delete Methods"

        public bool DeleteContact(int contactId)
        {
            try
            {
                // Need to check if the contact is part of the current context. If it is, detach it first before deleting.
                Contact localContact = _context.Set<Contact>().Local.FirstOrDefault(x => x.ContactId == contactId);
                if (localContact != null)
                {
                    _context.Entry(localContact).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                }
                else
                {
                    localContact = _context.Contacts.FirstOrDefault(x => x.ContactId == contactId);
                }
                _context.Contacts.Remove(localContact);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("ContactsDA.DeleteContact", ex);
            }
            return false;
        }

        #endregion

    }
}