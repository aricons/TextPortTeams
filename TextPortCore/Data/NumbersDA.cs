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

        //public List<SelectListItem> GetCountriesList(string purchaseType)
        //{
        //    List<SelectListItem> countriesList = new List<SelectListItem>();

        //    try
        //    {
        //        List<Country> vncs = new List<Country>();
        //        vncs = _context.Countries.Where(x => x.Enabled == true).OrderBy(x => x.SortOrder).ToList();

        //        foreach (Country country in vncs)
        //        {
        //            SelectListItem listItem = new SelectListItem();
        //            listItem.Text = $"{country.CountryName}";
        //            listItem.Value = country.CountryId.ToString();

        //            countriesList.Add(listItem);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandling eh = new ErrorHandling();
        //        eh.LogException("NumbersDA.GetCountriesList", ex);
        //    }
        //    return countriesList;
        //}

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
                return _context.PooledNumbers.Where(x => x.Enabled == true && x.IsFreeNumber == false).OrderBy(x => x.VirtualNumber).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.GetPooledNumbers", ex);
            }
            return new List<PooledNumber>();
        }

        public List<PooledNumber> GetFreeNumbers()
        {
            try
            {
                return _context.PooledNumbers.Where(x => x.Enabled == true && x.IsFreeNumber == true).OrderBy(x => x.VirtualNumber).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.GetFreeNumbers", ex);
            }
            return new List<PooledNumber>();
        }

        public List<NumberExpirationData> GetNumberExpirationNotifications(int days, NumberTypes numberType, string notificationType, string emailLinkAction)
        {
            IEnumerable<NumberExpirationData> numbersExpiring = new List<NumberExpirationData>();

            switch (days)
            {
                case 7:
                    numbersExpiring =
                        from
                            dvn in _context.DedicatedVirtualNumbers
                        join
                            acc in _context.Accounts on dvn.AccountId equals acc.AccountId
                        where
                            dvn.Cancelled == false
                            && (dvn.ExpirationDate - DateTime.UtcNow).TotalMinutes <= (Constants.MinutesInDay * days)
                            && dvn.NumberType == (int)numberType
                            && dvn.SevenDayReminderSent == null
                            && dvn.AutoRenew == false
                        select
                            new NumberExpirationData()
                            {
                                AccountID = dvn.AccountId,
                                CountryCode = dvn.CountryCode,
                                DaysUntilExpiration = DateAndTime.GetDaysBetweenTwoDates(DateTime.UtcNow, dvn.ExpirationDate),
                                HoursUntilExpiration = DateAndTime.GetRemainingHoursBetweenTwoDates(DateTime.UtcNow, dvn.ExpirationDate),
                                Email = acc.Email,
                                Balance = acc.Balance,
                                ExpirationDate = dvn.ExpirationDate,
                                NumberType = dvn.NumberType,
                                Provider = dvn.Provider,
                                UserName = acc.UserName,
                                VirtualNumber = dvn.VirtualNumber,
                                VirtualNumberID = dvn.VirtualNumberId,
                                AutoRenew = dvn.AutoRenew,
                                LeasePeriod = dvn.LeasePeriod,
                                LeasePeriodType = dvn.LeasePeriodType,
                                Fee = dvn.Fee,
                                NotificationType = notificationType,
                                EmailAction = emailLinkAction
                            };
                    break;

                case 2:
                    numbersExpiring =
                        from
                            dvn in _context.DedicatedVirtualNumbers
                        join
                            acc in _context.Accounts on dvn.AccountId equals acc.AccountId
                        where
                           dvn.Cancelled == false
                           && (dvn.ExpirationDate - DateTime.UtcNow).TotalMinutes <= (Constants.MinutesInDay * days)
                           && dvn.NumberType == (int)numberType
                           && dvn.TwoDayReminderSent == null
                           && dvn.AutoRenew == false
                        select new NumberExpirationData()
                        {
                            AccountID = dvn.AccountId,
                            CountryCode = dvn.CountryCode,
                            DaysUntilExpiration = DateAndTime.GetDaysBetweenTwoDates(DateTime.UtcNow, dvn.ExpirationDate),
                            HoursUntilExpiration = DateAndTime.GetRemainingHoursBetweenTwoDates(DateTime.UtcNow, dvn.ExpirationDate),
                            Email = acc.Email,
                            Balance = acc.Balance,
                            ExpirationDate = dvn.ExpirationDate,
                            NumberType = dvn.NumberType,
                            Provider = dvn.Provider,
                            UserName = acc.UserName,
                            VirtualNumber = dvn.VirtualNumber,
                            VirtualNumberID = dvn.VirtualNumberId,
                            AutoRenew = dvn.AutoRenew,
                            LeasePeriod = dvn.LeasePeriod,
                            LeasePeriodType = dvn.LeasePeriodType,
                            Fee = dvn.Fee,
                            NotificationType = notificationType,
                            EmailAction = emailLinkAction
                        };
                    break;
            }

            return numbersExpiring.ToList();
        }

        public List<NumberExpirationData> GetAutoRenewBalanceWarningNotifications(int days, NumberTypes numberType, string notificationType, string emailLinkAction)
        {
            IEnumerable<NumberExpirationData> numbersExpiring = new List<NumberExpirationData>();

            switch (days)
            {
                case 7:
                    numbersExpiring =
                        from
                            dvn in _context.DedicatedVirtualNumbers
                        join
                            acc in _context.Accounts on dvn.AccountId equals acc.AccountId
                        where
                            dvn.Cancelled == false
                            && (dvn.ExpirationDate - DateTime.UtcNow).TotalMinutes <= (Constants.MinutesInDay * days)
                            && dvn.NumberType == (int)numberType
                            && dvn.SevenDayReminderSent == null
                            && dvn.AutoRenew == true
                            && acc.Balance < dvn.Fee
                        select
                            new NumberExpirationData()
                            {
                                AccountID = dvn.AccountId,
                                CountryCode = dvn.CountryCode,
                                DaysUntilExpiration = DateAndTime.GetDaysBetweenTwoDates(DateTime.UtcNow, dvn.ExpirationDate),
                                HoursUntilExpiration = DateAndTime.GetRemainingHoursBetweenTwoDates(DateTime.UtcNow, dvn.ExpirationDate),
                                Email = acc.Email,
                                Balance = acc.Balance,
                                ExpirationDate = dvn.ExpirationDate,
                                NumberType = dvn.NumberType,
                                Provider = dvn.Provider,
                                UserName = acc.UserName,
                                VirtualNumber = dvn.VirtualNumber,
                                VirtualNumberID = dvn.VirtualNumberId,
                                AutoRenew = dvn.AutoRenew,
                                LeasePeriod = dvn.LeasePeriod,
                                LeasePeriodType = dvn.LeasePeriodType,
                                Fee = dvn.Fee,
                                NotificationType = notificationType,
                                EmailAction = emailLinkAction
                            };
                    break;

                case 2:
                    numbersExpiring =
                        from
                            dvn in _context.DedicatedVirtualNumbers
                        join
                            acc in _context.Accounts on dvn.AccountId equals acc.AccountId
                        where
                           dvn.Cancelled == false
                           && (dvn.ExpirationDate - DateTime.UtcNow).TotalMinutes <= (Constants.MinutesInDay * days)
                           && dvn.NumberType == (int)numberType
                           && dvn.TwoDayReminderSent == null
                           && dvn.AutoRenew == true
                           && acc.Balance < dvn.Fee
                        select new NumberExpirationData()
                        {
                            AccountID = dvn.AccountId,
                            CountryCode = dvn.CountryCode,
                            DaysUntilExpiration = DateAndTime.GetDaysBetweenTwoDates(DateTime.UtcNow, dvn.ExpirationDate),
                            HoursUntilExpiration = DateAndTime.GetRemainingHoursBetweenTwoDates(DateTime.UtcNow, dvn.ExpirationDate),
                            Email = acc.Email,
                            Balance = acc.Balance,
                            ExpirationDate = dvn.ExpirationDate,
                            NumberType = dvn.NumberType,
                            Provider = dvn.Provider,
                            UserName = acc.UserName,
                            VirtualNumber = dvn.VirtualNumber,
                            VirtualNumberID = dvn.VirtualNumberId,
                            AutoRenew = dvn.AutoRenew,
                            LeasePeriod = dvn.LeasePeriod,
                            LeasePeriodType = dvn.LeasePeriodType,
                            Fee = dvn.Fee,
                            NotificationType = notificationType,
                            EmailAction = emailLinkAction
                        };
                    break;
            }

            return numbersExpiring.ToList();
        }

        public List<NumberExpirationData> GetExpiredNumbers()
        {
            IEnumerable<NumberExpirationData> numbersExpiring = new List<NumberExpirationData>();

            numbersExpiring =
                from
                    dvn in _context.DedicatedVirtualNumbers
                join
                    acc in _context.Accounts on dvn.AccountId equals acc.AccountId
                where
                    dvn.ExpirationDate <= DateTime.UtcNow
                    && dvn.SevenDayReminderSent != null
                    && dvn.TwoDayReminderSent != null
                    && dvn.Cancelled == false
                    && dvn.AutoRenew == false
                select
                    new NumberExpirationData()
                    {
                        AccountID = dvn.AccountId,
                        CountryCode = dvn.CountryCode,
                        DaysUntilExpiration = DateAndTime.GetDaysBetweenTwoDates(DateTime.UtcNow, dvn.ExpirationDate),
                        HoursUntilExpiration = DateAndTime.GetRemainingHoursBetweenTwoDates(DateTime.UtcNow, dvn.ExpirationDate),
                        Email = acc.Email,
                        Balance = acc.Balance,
                        ExpirationDate = dvn.ExpirationDate,
                        NumberType = dvn.NumberType,
                        Provider = dvn.Provider,
                        UserName = acc.UserName,
                        VirtualNumber = dvn.VirtualNumber,
                        VirtualNumberID = dvn.VirtualNumberId,
                        AutoRenew = dvn.AutoRenew,
                        LeasePeriod = dvn.LeasePeriod,
                        LeasePeriodType = dvn.LeasePeriodType,
                        Fee = dvn.Fee,
                        NotificationType = string.Empty,
                        EmailAction = "void"
                    };

            return numbersExpiring.ToList();
        }

        public List<NumberExpirationData> GetAutoRenewNumbers()
        {
            IEnumerable<NumberExpirationData> numbersExpiring = new List<NumberExpirationData>();

            numbersExpiring =
                from
                    dvn in _context.DedicatedVirtualNumbers
                join
                    acc in _context.Accounts on dvn.AccountId equals acc.AccountId
                where
                    dvn.ExpirationDate <= DateTime.UtcNow
                    && dvn.Cancelled == false
                    && dvn.AutoRenew == true
                    && acc.Balance >= dvn.Fee
                select
                    new NumberExpirationData()
                    {
                        AccountID = dvn.AccountId,
                        CountryCode = dvn.CountryCode,
                        DaysUntilExpiration = DateAndTime.GetDaysBetweenTwoDates(DateTime.UtcNow, dvn.ExpirationDate),
                        HoursUntilExpiration = DateAndTime.GetRemainingHoursBetweenTwoDates(DateTime.UtcNow, dvn.ExpirationDate),
                        Email = acc.Email,
                        Balance = acc.Balance,
                        ExpirationDate = dvn.ExpirationDate,
                        NumberType = dvn.NumberType,
                        Provider = dvn.Provider,
                        UserName = acc.UserName,
                        VirtualNumber = dvn.VirtualNumber,
                        VirtualNumberID = dvn.VirtualNumberId,
                        AutoRenew = dvn.AutoRenew,
                        LeasePeriod = dvn.LeasePeriod,
                        LeasePeriodType = dvn.LeasePeriodType,
                        Fee = dvn.Fee,
                        NotificationType = string.Empty,
                        EmailAction = string.Empty
                    };

            return numbersExpiring.ToList();
        }

        public List<NumberExpirationData> GetAutoRenewInsufficientBalanceExpirations()
        {
            IEnumerable<NumberExpirationData> numbersExpiring = new List<NumberExpirationData>();

            numbersExpiring =
                from
                    dvn in _context.DedicatedVirtualNumbers
                join
                    acc in _context.Accounts on dvn.AccountId equals acc.AccountId
                where
                    dvn.ExpirationDate <= DateTime.UtcNow
                    && dvn.NumberType == (int)NumberTypes.Regular
                    && dvn.Cancelled == false
                    && dvn.AutoRenew == true
                    && acc.Balance < dvn.Fee
                select
                    new NumberExpirationData()
                    {
                        AccountID = dvn.AccountId,
                        CountryCode = dvn.CountryCode,
                        DaysUntilExpiration = DateAndTime.GetDaysBetweenTwoDates(DateTime.UtcNow, dvn.ExpirationDate),
                        HoursUntilExpiration = DateAndTime.GetRemainingHoursBetweenTwoDates(DateTime.UtcNow, dvn.ExpirationDate),
                        Email = acc.Email,
                        Balance = acc.Balance,
                        ExpirationDate = dvn.ExpirationDate,
                        Provider = dvn.Provider,
                        UserName = acc.UserName,
                        VirtualNumber = dvn.VirtualNumber,
                        VirtualNumberID = dvn.VirtualNumberId,
                        AutoRenew = dvn.AutoRenew,
                        LeasePeriod = dvn.LeasePeriod,
                        LeasePeriodType = dvn.LeasePeriodType,
                        Fee = dvn.Fee,
                        NotificationType = string.Empty,
                        EmailAction = string.Empty
                    };

            return numbersExpiring.ToList();
        }

        #endregion

        #region "Insert Methods"

        public bool AddNumberToAccount(RegistrationData rd)
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
                    AccountId = rd.AccountId,
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

        public bool AddTrialNumberToAccount(ActivateAccountRequest actReq)
        {
            try
            {
                DateTime expirationDate = DateTime.UtcNow.AddDays(15);
                expirationDate = expirationDate.AddHours(-6);

                DedicatedVirtualNumber number = new DedicatedVirtualNumber()
                {
                    AccountId = actReq.AccountId,
                    CancellationFailureCount = 0,
                    Cancelled = false,
                    CountryCode = "2",
                    CarrierId = 1,
                    NumberType = (byte)NumberTypes.Pooled,
                    CreateDate = DateTime.Now,
                    ExpirationDate = expirationDate,
                    IsDefault = true,
                    Fee = 0,
                    Provider = "Bandwidth",
                    ReminderFailureCount = 0,
                    RenewalCount = 0,
                    SevenDayReminderSent = null,
                    TwoDayReminderSent = null,
                    VirtualNumber = actReq.VirtualNumber,
                    CountryId = (int)Countries.UnitedStates,
                    VirtualNumberId = 0
                };

                _context.DedicatedVirtualNumbers.Add(number);
                _context.SaveChanges();

                actReq.VirtualNumberId = number.VirtualNumberId;

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("NumbersDA.AddTrialNumberToAccount", ex);
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
