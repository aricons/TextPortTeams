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
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("ContactsDA.GetContactsForAccount", ex);
            }

            return null;
        }

        #endregion

    }
}