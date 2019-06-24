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

        public string GetAreaCodeName(string areaCode, bool tollFree)
        {
            try
            {
                return _context.AreaCodes.Where(x => x.AreaCodeNum.Equals(areaCode) && x.TollFree == tollFree).FirstOrDefault().GeographicArea;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.GetAreaCodeName", ex);
            }
            return string.Empty;
        }

        public List<SelectListItem> GetNumberCountriesList(string purchaseType)
        {
            List<SelectListItem> countriesList = new List<SelectListItem>();

            try
            {
                List<VirtualNumberCountry> vncs = new List<VirtualNumberCountry>();
                if (purchaseType == "ComplimentaryNumber")
                {
                    vncs = _context.VirtualNumberCountries.Where(x => x.Enabled == true && x.VirtualNumberCountryId != 23).OrderBy(x => x.CountryName).ToList();
                }
                else
                {
                    vncs = _context.VirtualNumberCountries.Where(x => x.Enabled == true).OrderBy(x => x.CountryName).ToList();
                }

                foreach (VirtualNumberCountry country in vncs)
                {
                    SelectListItem listItem = new SelectListItem();

                    listItem.Text = (purchaseType != "ComplimentaryNumber") ? $"{country.CountryName} - {country.MonthlyRate:C}/month" : $"{country.CountryName}";
                    listItem.Value = country.VirtualNumberCountryId.ToString();

                    countriesList.Add(listItem);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.GetNumberCountriesList", ex);
            }
            return countriesList;
        }

        public DedicatedVirtualNumber GetVirtualNumberById(int virtualNumberId)
        {
            try
            {
                return _context.DedicatedVirtualNumbers.FirstOrDefault(x => x.VirtualNumberId == virtualNumberId);
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.GetVirtualNumberById", ex);
            }
            return null;
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
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.GetNumbersForAccount", ex);
            }
            return new List<DedicatedVirtualNumber>();
        }

        public List<PooledNumber> GetPooledNumbers()
        {
            try
            {
                return _context.PooledNumbers.Where(x => x.Enabled == true).OrderBy(x => x.VirtualNumber).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.GetPooledNumbers", ex);
            }
            return new List<PooledNumber>();
        }

        #endregion

        #region "Insert Methods"

        public bool AddNumberToAccount(RegistrationData rd)
        {
            try
            {
                DateTime expirationDate = DateTime.UtcNow.AddMonths(1);
                if (rd.FreeTrial)
                {
                    expirationDate = DateTime.UtcNow.AddDays(15);
                }
                else
                {
                    expirationDate = (rd.LeasePeriod > 0) ? DateTime.UtcNow.AddMonths(rd.LeasePeriod) : DateTime.UtcNow.AddMonths(1);
                }
                expirationDate = expirationDate.AddHours(-12);

                DedicatedVirtualNumber number = new DedicatedVirtualNumber()
                {
                    AccountId = rd.AccountId,
                    CancellationFailureCount = 0,
                    Cancelled = false,
                    CountryCode = rd.NumberCountryId.ToString(),
                    NumberType = (byte)rd.NumberType,
                    CreateDate = DateTime.Now,
                    ExpirationDate = expirationDate,
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

                rd.VirtualNumberId = number.VirtualNumberId;

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.AddNumberToAccount", ex);
            }
            return false;
        }

        #endregion

        #region "Update methods"

        public bool ApplyVirtualNumberIdToAllMessages(int accountId, int virtualNumberId)
        {
            try
            {
                var messages = _context.Messages.Where(x => x.AccountId == accountId && x.VirtualNumberId == 0).ToList();
                messages.ForEach(x => x.VirtualNumberId = virtualNumberId);
                SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.ApplyVirtualNumberIdToAllMessages", ex);
            }
            return false;
        }

        #endregion

        #region "Delete methods"
        public bool DeleteVirtualNumber(int virtualNumberId)
        {
            try
            {
                // Need to check if the number is part of the current context. If it is, detach it first before deleting.
                DedicatedVirtualNumber localVn = _context.Set<DedicatedVirtualNumber>().Local.FirstOrDefault(x => x.VirtualNumberId.Equals(virtualNumberId));
                if (localVn != null)
                {
                    _context.Entry(localVn).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                }
                _context.DedicatedVirtualNumbers.Remove(localVn);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.DeleteVirtualNumber", ex);
            }
            return false;
        }
        #endregion
    }
}
