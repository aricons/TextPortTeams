using System;
using System.Linq;

using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPortCore.Data
{
    public partial class TextPortDA
    {
        #region "Select Methods"

        public bool IsNumberStopped(string mobileNumber)
        {
            try
            {
                StopRequest sr = _context.StopRequests.FirstOrDefault(x => x.MobileNumber == mobileNumber);
                return (sr != null);
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("StopDA.IsNumberStopped", ex);
            }
            return false;
        }

        #endregion

        #region "Insert Methods"

        public bool AddNumberStop(StopRequest stopRequest)
        {
            try
            {
                StopRequest existingBlock = _context.StopRequests.FirstOrDefault(x => x.MobileNumber == stopRequest.MobileNumberE164);
                if (existingBlock != null)
                {
                    return false;
                }

                _context.StopRequests.Add(stopRequest);
                _context.SaveChanges();

                return (stopRequest.StopId > 0);
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("StopDA.AddNumberStop", ex);
            }

            return false;
        }

        #endregion

        #region "Delete Methods"
        #endregion
    }
}
