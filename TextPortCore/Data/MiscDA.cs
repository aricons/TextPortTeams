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

        public int CheckFreeTextCountForIP(string ipAddress, int limit)
        {
            try
            {
                FreeTextIPAddress addr = _context.FreeTextIPAddresses.Where(x => x.IPAddress == ipAddress).FirstOrDefault();
                if (addr != null)
                {
                    if (addr.RequestCount <= limit)
                    {
                        addr.RequestCount++;
                        this.SaveChanges();
                    }
                    return addr.RequestCount;
                }
                else
                {
                    _context.FreeTextIPAddresses.Add(new FreeTextIPAddress(ipAddress));
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MiscDA.CheckFreeTextCountForIP", ex);
            }
            return 0;
        }

        public int CheckFreeSendCountForNumber(int accountId, string mobileNumber)
        {
            try
            {
                return _context.Messages.Where(x => x.AccountId == accountId && x.MessageType == (int)MessageTypes.FreeTextSend && x.MobileNumber == mobileNumber && x.Direction == 0 && x.TimeStamp >= DateTime.UtcNow.AddHours(-24)).Count();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("MiscDA.CheckFreeSendCountForNumber", ex);
            }
            return 0;
        }

        #endregion

        #region "Insert Methods"
        #endregion

        #region "Delete Methods"
        #endregion

    }
}