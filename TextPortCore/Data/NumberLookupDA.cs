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

        public NumberLookupResult LookupNumber(string phoneNumber)
        {
            short npa = 0;
            short nxx = 0;
            string thousands = string.Empty;
            string displayNumber = Utilities.NumberToDisplayFormat($"1{phoneNumber}", 22);

            try
            {
                phoneNumber = Utilities.StripNumber(phoneNumber);
                npa = (short)Conversion.StringToIntOrZero(phoneNumber.Substring(0, 3));
                nxx = (short)Conversion.StringToIntOrZero(phoneNumber.Substring(3, 3));
                thousands = phoneNumber.Substring(6, 1);

                List<NumberLookupResult> results = (
                from nxt in _context.NpaNxxThous
                from ncc in _context.NpaNxxCities.Where(nc => nc.NPA == nxt.NPA && nc.NXX == nxt.NXX).DefaultIfEmpty() // left outer join
                from zll in _context.ZipLatLongs.Where(zl => zl.Zip == ncc.Zip).DefaultIfEmpty()
                where nxt.NPA == npa && nxt.NXX == nxx
                select new NumberLookupResult()
                {
                    Number = displayNumber,
                    CLLI = nxt.CLLI,
                    City = ncc.City,
                    CityId = ncc.CityId,
                    Company = nxt.Company,
                    County = ncc.County,
                    LATA = nxt.LATA,
                    NpaNxxId = nxt.Id,
                    NPA = nxt.NPA,
                    NXX = nxt.NXX,
                    Latitude = zll.Latitude,
                    Longitude = zll.Longitude,
                    PrefixType = nxt.PrefixType,
                    RateCenter = nxt.RateCenter,
                    State = nxt.State,
                    Thousands = nxt.Thousands,
                    Zip = ncc.Zip,
                    ZipId = zll.ZipId,
                    Message = string.Empty,
                    MatchFound = true,
                    HasResult = true
                }).ToList();

                // See if a match can be made on the thousands value. If not, return the first result.
                if (results != null && results.Count > 0)
                {
                    foreach (NumberLookupResult res in results)
                    {
                        if (res.Thousands == thousands)
                        {
                            return res;
                        }
                    }

                    return results.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumberLookupDA.LookupNumber", ex);
            }

            return new NumberLookupResult()
            {
                HasResult = true,
                MatchFound = false,
                Number = displayNumber,
                Message = $"A match was not found for {displayNumber}"
            };
        }

        #endregion
    }
}