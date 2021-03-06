﻿using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPortCore.Data
{
    public partial class TextPortDA
    {
        #region "Select Methods"

        public Carrier GetCarrierForCountry(int countryId)
        {
            try
            {
                return _context.Countries.Include(x => x.Carrier).FirstOrDefault(x => x.CountryId == countryId).Carrier;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("CarrierDA.GetCarrierForCountry", ex);
            }
            return null;
        }

        public CarrierResponseCode GetCarrierResponseCode(int carrierId, string responseCode)
        {
            try
            {
                CarrierResponseCode rc = _context.CarrierResponseCodes.FirstOrDefault(x => x.CarrierId == carrierId && x.ResponseCode == responseCode);
                if (rc != null)
                {
                    return rc;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("CarrierDA.GetCarrierResponseCode", ex);
            }

            // Return a generic response and assume the message is still billable.
            return new CarrierResponseCode()
            {
                CarrierId = carrierId,
                ResponseCode = responseCode,
                Description = $"Unlisted code {responseCode}",
                Billable = true
            };
        }

        #endregion

        #region "Insert Methods"
        #endregion

        #region "Delete Methods"
        #endregion

    }
}
