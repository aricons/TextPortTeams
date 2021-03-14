using System;
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

        public string GetAreaCodeName(string areaCode, bool tollFree)
        {
            try
            {
                AreaCode ac = _context.AreaCodes.FirstOrDefault(x => x.AreaCodeNum.Equals(areaCode) && x.TollFree == tollFree);
                if (ac != null)
                {
                    return ac.GeographicArea;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.GetAreaCodeName", ex);
            }
            return "Invalid area code";
        }

        public List<Country> GetCountriesList()
        {
            List<Country> countriesList = new List<Country>();

            try
            {
                return _context.Countries.Where(x => x.Enabled == true).OrderBy(x => x.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.GetCountriesList", ex);
            }
            return countriesList;
        }

        public DedicatedVirtualNumber GetVirtualNumberById(int virtualNumberId)
        {
            try
            {
                return _context.DedicatedVirtualNumbers.Include(x => x.Country).FirstOrDefault(x => x.VirtualNumberId == virtualNumberId);
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.GetVirtualNumberById", ex);
            }
            return null;
        }

        public List<DedicatedVirtualNumber> GetNumbersForBranch(int branchId, bool includeExpiredNumbers)
        {
            try
            {
                if (includeExpiredNumbers)
                {
                    return _context.DedicatedVirtualNumbers.Include(x => x.Country).Where(x => x.BranchId == branchId).OrderBy(x => x.CountryId).ThenByDescending(x => x.CreateDate).ToList();
                }
                else
                {
                    return _context.DedicatedVirtualNumbers.Include(x => x.Country).Where(x => x.BranchId == branchId && x.Cancelled == false).OrderBy(x => x.CountryId).ThenByDescending(x => x.CreateDate).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.GetNumbersForBranch", ex);
            }
            return new List<DedicatedVirtualNumber>();
        }

        public List<DedicatedVirtualNumber> GetActiveNumbers()
        {
            try
            {
                return _context.DedicatedVirtualNumbers.Include(x => x.Branch).Where(x => x.Cancelled == false).OrderBy(x => x.Branch.BranchName).ThenByDescending(x => x.CreateDate).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.GetNumbersForBranch", ex);
            }
            return new List<DedicatedVirtualNumber>();
        }

        #endregion

        #region "Insert Methods"

        public bool AddNumberToBranch(RegistrationData rd)
        {
            try
            {
                DateTime expirationDate = DateTime.UtcNow.AddDays(7);
                DateTime? twoDayRenewalReminderDate = null;
                DateTime? sevenDayRenewalReminderDate = null;
                int leasePeriod = (rd.LeasePeriod == 0) ? 1 : rd.LeasePeriod;

                if (rd.FreeTrial)
                {
                    expirationDate = DateTime.UtcNow.AddDays(15);
                }
                else
                {
                    switch (rd.LeasePeriodType)
                    {
                        case "D":
                            expirationDate = DateTime.UtcNow.AddDays(leasePeriod);
                            twoDayRenewalReminderDate = DateTime.UtcNow;
                            sevenDayRenewalReminderDate = DateTime.UtcNow;
                            break;
                        case "W":
                            expirationDate = DateTime.UtcNow.AddDays(leasePeriod * 7);
                            if (leasePeriod <= 1)
                            {
                                sevenDayRenewalReminderDate = DateTime.UtcNow;
                            }
                            break;
                        case "M":
                            expirationDate = DateTime.UtcNow.AddMonths(leasePeriod).AddHours(-4);
                            break;
                        case "Y":
                            expirationDate = DateTime.UtcNow.AddYears(leasePeriod).AddHours(-6);
                            break;
                    }
                }

                DedicatedVirtualNumber number = new DedicatedVirtualNumber()
                {
                    BranchId = rd.BranchId,
                    CancellationFailureCount = 0,
                    Cancelled = false,
                    CarrierId = rd.CarrierId,
                    CountryCode = rd.CountryId.ToString(),
                    NumberType = (byte)rd.NumberType,
                    CreateDate = DateTime.Now,
                    ExpirationDate = expirationDate,
                    IsDefault = false,
                    AutoRenew = false,
                    Fee = rd.NumberCost,
                    LeasePeriod = rd.LeasePeriod,
                    LeasePeriodType = rd.LeasePeriodType,
                    ReminderFailureCount = 0,
                    RenewalCount = 0,
                    SevenDayReminderSent = sevenDayRenewalReminderDate,
                    TwoDayReminderSent = twoDayRenewalReminderDate,
                    VirtualNumber = rd.VirtualNumber,
                    CountryId = rd.CountryId,
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

        public bool SetVirtualNumberCancelledFlag(int virtualNumberId, bool cancelValue)
        {
            try
            {
                DedicatedVirtualNumber virtualNumber = _context.DedicatedVirtualNumbers.FirstOrDefault(x => x.VirtualNumberId == virtualNumberId);
                if (virtualNumber != null)
                {
                    virtualNumber.Cancelled = cancelValue;

                    this.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.SetVirtualNumberCancelledFlag", ex);
            }
            return false;
        }

        public bool SetVirtualNumberXDayReminderSentFlag(int virtualNumberId, int days)
        {
            try
            {
                if (days == 2 || days == 7)
                {
                    DedicatedVirtualNumber virtualNumber = _context.DedicatedVirtualNumbers.FirstOrDefault(x => x.VirtualNumberId == virtualNumberId);
                    if (virtualNumber != null)
                    {
                        if (days == 2)
                        {
                            virtualNumber.TwoDayReminderSent = DateTime.UtcNow;
                        }
                        else
                        {
                            virtualNumber.SevenDayReminderSent = DateTime.UtcNow;
                        }
                        this.SaveChanges();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.SetVirtualNumberCancelledFlag", ex);
            }
            return false;
        }

        public bool AutoRenewNumber(int virtualNumberId)
        {
            try
            {
                DedicatedVirtualNumber virtualNumber = _context.DedicatedVirtualNumbers.FirstOrDefault(x => x.VirtualNumberId == virtualNumberId);
                if (virtualNumber != null)
                {
                    virtualNumber.Cancelled = false;
                    virtualNumber.ExpirationDate = virtualNumber.ExpirationDate.AddMonths(1);
                    virtualNumber.SevenDayReminderSent = null;
                    virtualNumber.TwoDayReminderSent = null;
                    virtualNumber.RenewalCount++;

                    this.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.AutoRenewNumber", ex);
            }
            return false;
        }

        public bool IncrementVirtualNumberCancellationFailureCount(int virtualNumberId)
        {
            try
            {
                DedicatedVirtualNumber virtualNumber = _context.DedicatedVirtualNumbers.FirstOrDefault(x => x.VirtualNumberId == virtualNumberId);
                if (virtualNumber != null)
                {
                    virtualNumber.CancellationFailureCount++;

                    this.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.SetVirtualNumberCancellationFailureCount", ex);
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
