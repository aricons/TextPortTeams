using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;

using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPortCore.Data
{
    public partial class TextPortDA
    {

        #region "Select Methods"

        public EmailToSMSAddress GetEmailToSMSAddressByEmailAddress(string emailAddress)
        {
            try
            {
                return _context.EmailToSMSAddresses.Where(x => x.EmailAddress == emailAddress).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("EmailToSmsDA.GetEmailToSMSAddressByEmailAddress", ex);
            }
            return null;
        }

        #endregion

        #region "Insert Methods"

        public bool AddEmailToSmsAddress(EmailToSMSAddress address)
        {
            try
            {
                _context.EmailToSMSAddresses.Add(address);
                _context.SaveChanges();

                int foo = address.AddressId;

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("EmailToSmsDA.AddEmailToSmsAddress", ex);
            }
            return false;
        }

        #endregion

        #region "Delete Methods"

        public bool DeleteEmailToSMSAddress(int addressId)
        {
            try
            {
                // Need to check if the email to SMS address is part of the current context. If it is, detach it first before deleting.
                EmailToSMSAddress localEmailToSMS = _context.Set<EmailToSMSAddress>().Local.FirstOrDefault(x => x.AddressId == addressId);
                if (localEmailToSMS != null)
                {
                    _context.Entry(localEmailToSMS).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                }
                else
                {
                    localEmailToSMS = _context.EmailToSMSAddresses.FirstOrDefault(x => x.AddressId == addressId);
                }
                _context.EmailToSMSAddresses.Remove(localEmailToSMS);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("EmailToSmsDA.DeleteEmailToSMSAddress", ex);
            }
            return false;
        }

        #endregion

    }
}
