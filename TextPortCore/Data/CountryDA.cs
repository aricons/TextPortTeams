using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPortCore.Data
{
    public partial class TextPortDA
    {
        #region "Select Methods"

        public Country GetCountryByCountryId(int countryId)
        {
            try
            {
                return _context.Countries.Include(x => x.Carrier).Where(x => x.CountryId == countryId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("CarrierDA.GetCountryByCountryId", ex);
            }
            return null;
        }

        #endregion

        #region "Insert Methods"
        #endregion

        #region "Delete Methods"
        #endregion

    }
}
