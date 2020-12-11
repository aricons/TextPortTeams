using System;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

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
                return _context.Contacts.Where(x => x.AccountId == accountId).OrderBy(x => x.Name).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("ContactsDA.GetContactsForAccount", ex);
            }

            return null;
        }

        public Contact GetContactByContactId(int accountId, int contactId)
        {
            try
            {
                return _context.Contacts.FirstOrDefault(x => x.AccountId == accountId && x.ContactId == contactId);
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("ContactsDA.GetContactByContactId", ex);
            }

            return null;
        }

        #endregion


        #region "Insert Methods"

        public bool AddContact(Contact contact)
        {
            try
            {
                contact.MobileNumber = Utilities.NumberToE164(contact.MobileNumber, "1");
                contact.DateAdded = DateTime.UtcNow;

                _context.Contacts.Add(contact);
                _context.SaveChanges();

                // Apply the contact to any existing messages.
                ApplyContactToAccountAndNumber(contact);

                return (contact.ContactId > 0);
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("ContactsDA.AddContact", ex);
            }
            return false;
        }

        #endregion

        #region "Update Methods"

        public bool UpdateContact(Contact contact)
        {
            try
            {
                Contact ctc = _context.Contacts.FirstOrDefault(x => x.ContactId == contact.ContactId);

                if (ctc != null)
                {
                    Contact oldContact = new Contact()
                    {
                        ContactId = ctc.ContactId,
                        AccountId = ctc.AccountId,
                        MobileNumber = ctc.MobileNumber,
                        Name = ctc.Name,
                        DateAdded = ctc.DateAdded
                    };

                    _context.Contacts.Attach(ctc);
                    ctc.MobileNumber = contact.MobileNumber;
                    ctc.Name = contact.Name;

                    // If the numbers don't match, remove the old contact-to-number association and add a new one.
                    if (!contact.MobileNumber.Equals(oldContact.MobileNumber))
                    {
                        RemoveContactForAccountAndNumber(oldContact);
                        ApplyContactToAccountAndNumber(ctc);
                    }

                    _context.Entry(ctc).State = EntityState.Modified;
                    _context.SaveChanges();

                    return (contact.ContactId > 0);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("ContactsDA.EditContact", ex);
            }
            return false;
        }

        public bool AddOrReplaceContact(Contact contact)
        {
            try
            {
                Contact ctc = _context.Contacts.FirstOrDefault(x => x.AccountId == contact.AccountId && x.MobileNumber == contact.MobileNumber);

                if (ctc == null)
                {
                    AddContact(contact);
                }
                else
                {
                    Contact oldContact = new Contact()
                    {
                        ContactId = ctc.ContactId,
                        AccountId = ctc.AccountId,
                        MobileNumber = ctc.MobileNumber,
                        Name = ctc.Name,
                        DateAdded = ctc.DateAdded
                    };

                    _context.Contacts.Attach(ctc);
                    ctc.MobileNumber = contact.MobileNumber;
                    ctc.Name = contact.Name;

                    // If the numbers don't match, remove the old contact-to-number association and add a new one.
                    if (!contact.MobileNumber.Equals(oldContact.MobileNumber))
                    {
                        RemoveContactForAccountAndNumber(oldContact);
                        ApplyContactToAccountAndNumber(ctc);
                    }

                    _context.Entry(ctc).State = EntityState.Modified;
                    _context.SaveChanges();

                    return (contact.ContactId > 0);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("ContactsDA.EditContact", ex);
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

                RemoveContactForAccountAndNumber(localContact);

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

        #region "Stored Procedures"

        public void ApplyContactToAccountAndNumber(Contact contact)
        {
            SqlParameter accountIdParam = new SqlParameter("@AccountId", contact.AccountId);
            SqlParameter mobileNumberParam = new SqlParameter("@MobileNumber", Utilities.NumberToE164(contact.MobileNumber, "1"));
            SqlParameter contactIdParam = new SqlParameter("@ContactId", contact.ContactId);

            int res = _context.Database.ExecuteSqlCommand("Contacts_ApplyContactForAccountAndNumber @AccountId, @MobileNumber, @ContactId", accountIdParam, mobileNumberParam, contactIdParam);
        }

        public void RemoveContactForAccountAndNumber(Contact contact)
        {
            SqlParameter accountIdParam = new SqlParameter("@AccountId", contact.AccountId);
            SqlParameter mobileNumberParam = new SqlParameter("@MobileNumber", Utilities.NumberToE164(contact.MobileNumber, "1"));

            int res = _context.Database.ExecuteSqlCommand("Contacts_RemoveContactForAccountAndNumber @AccountId, @MobileNumber", accountIdParam, mobileNumberParam);
        }

        #endregion

    }
}