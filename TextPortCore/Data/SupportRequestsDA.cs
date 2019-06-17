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
        #endregion

        #region "Insert Methods"

        public int AddSupportRequest(SupportRequestModel model)
        {
            try
            {
                SupportRequest supportRequest = new SupportRequest(model);
                if (supportRequest != null)
                {
                    _context.SupportRequests.Add(supportRequest);
                    _context.SaveChanges();

                    return supportRequest.SupportId;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("SupportRequestDA.AddSupportRequest", ex);
            }
            return 0;
        }

        #endregion

    }
}
