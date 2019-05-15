using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;

using TextPortCore.Helpers;

namespace TextPortCore.Data
{
    public partial class TextPortDA
    {
        #region "Select Methods"

        public IEnumerable<SelectListItem> GetTimeZones()
        {
            try
            {
                List<SelectListItem> timeZones = _context.TimeZones
                .OrderBy(tz => tz.TimeZoneId)
                    .Select(tz =>
                    new SelectListItem
                    {
                        Value = tz.TimeZoneId.ToString(),
                        Text = tz.Name
                    }).ToList();

                SelectListItem firstItem = new SelectListItem()
                {
                    Value = null,
                    Text = "--- select time zone ---"
                };

                timeZones.Insert(0, firstItem);

                return new SelectList(timeZones, "Value", "Text");
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("Lists.GetTimeZones", ex);
            }

            return null;
        }

        public IEnumerable<SelectListItem> GetLeasePeriods(bool complimentaryOnly)
        {
            try
            {
                Dictionary<int, string> leasePeriodsDict = null;

                if (complimentaryOnly)
                {
                    leasePeriodsDict = new Dictionary<int, string>()
                    {
                        {1, "1 Month"}
                    };
                }
                else
                {
                    leasePeriodsDict = new Dictionary<int, string>()
                    {
                        {1, "1 Month"},
                        {2, "2 Months"},
                        {3, "3 Months"},
                        {6, "6 Months"},
                        {12, "1 Year"},
                        {24, "2 Years" }
                    };
                }

                List<SelectListItem> periods = leasePeriodsDict
                    .OrderBy(p => p.Key)
                        .Select(lp =>
                        new SelectListItem
                        {
                            Value = lp.Key.ToString(),
                            Text = lp.Value
                        }).ToList();

                if (!complimentaryOnly)
                {
                    SelectListItem firstItem = new SelectListItem()
                    {
                        Value = "0",
                        Text = "--- select lease period ---"
                    };

                    periods.Insert(0, firstItem);
                }

                return new SelectList(periods, "Value", "Text");
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("Lists.GetLeasePeriods", ex);
            }

            return null;
        }

        public IEnumerable<SelectListItem> GetCreditAmounts()
        {
            try
            {
                Dictionary<decimal, string> creditAmountsDict = new Dictionary<decimal, string>()
                {
                    {(decimal)2.00, "$2.00"},
                    {(decimal)5.00, "$5.00"},
                    {(decimal)10.00, "$10.00"},
                    {(decimal)20.00, "$20.00"},
                    {(decimal)30.00, "$20.00"},
                    {(decimal)50.00, "$50.00" },
                    {(decimal)100.00, "$100.00" }
                };

                List<SelectListItem> periods = creditAmountsDict
                    .OrderBy(p => p.Key)
                        .Select(lp =>
                        new SelectListItem
                        {
                            Value = lp.Key.ToString(),
                            Text = lp.Value
                        }).ToList();

                SelectListItem firstItem = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- select credit amount ---"
                };

                periods.Insert(0, firstItem);

                return new SelectList(periods, "Value", "Text");
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling(_context);
                eh.LogException("Lists.GetCreditAmountsList", ex);
            }

            return null;
        }

        #endregion

    }
}