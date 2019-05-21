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

        public string GetAreaCodeName(string areaCode)
        {
            try
            {
                return _context.AreaCodes.Where(x => x.AreaCodeNum.Equals(areaCode)).FirstOrDefault().GeographicArea;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("NumbersDA.GetAreaCodeName", ex);
            }
            return string.Empty;
        }

        public List<SelectListItem> GetNumberCountriesList()
        {
            List<SelectListItem> countriesList = new List<SelectListItem>();

            try
            {
                foreach (VirtualNumberCountry country in _context.VirtualNumberCountries.Where(x => x.Enabled == true).OrderBy(x => x.CountryName))
                {
                    countriesList.Add(new SelectListItem()
                    {
                        Text = string.Format("{0} - {1:C}/month", country.CountryName, country.MonthlyRate),
                        Value = country.VirtualNumberCountryId.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("NumbersDA.GetNumberCountriesList", ex);
            }
            return countriesList;
        }

        public List<DedicatedVirtualNumber> GetNumbersForAccount(int accountId, bool includeExpiredNumbers)
        {
            try
            {
                if (includeExpiredNumbers)
                {
                    return _context.DedicatedVirtualNumbers.Where(x => x.AccountId == accountId).OrderByDescending(x => x.CreateDate).ToList();
                }
                else
                {
                    return _context.DedicatedVirtualNumbers.Where(x => x.AccountId == accountId && x.Cancelled == false).OrderByDescending(x => x.CreateDate).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("NumbersDA.GetNumbersForAccount", ex);
            }
            return new List<DedicatedVirtualNumber>();
        }

        #endregion

        #region "Insert Methods"

        public bool AddNumberToAccount(RegistrationData rd)
        {
            try
            {
                DedicatedVirtualNumber number = new DedicatedVirtualNumber()
                {
                    AccountId = rd.AccountId,
                    CancellationFailureCount = 0,
                    Cancelled = false,
                    CountryCode = rd.NumberCountryId.ToString(),
                    CreateDate = DateTime.Now,
                    ExpirationDate = DateTime.Now.AddMonths(rd.LeasePeriod),
                    IsDefault = true,
                    Fee = rd.TotalCost,
                    Provider = rd.NumberProvider,
                    ReminderFailureCount = 0,
                    RenewalCount = 0,
                    SevenDayReminderSent = null,
                    TwoDayReminderSent = null,
                    VirtualNumber = rd.VirtualNumber,
                    VirtualNumberCountryId = rd.NumberCountryId,
                    VirtualNumberId = 0
                };

                _context.DedicatedVirtualNumbers.Add(number);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("NumbersDA.AddNumberToAccount", ex);
            }
            return false;
        }

        #endregion
    }
}
