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
                    Text = "--- select a category ---"
                };

                timeZones.Insert(0, firstItem);

                return new SelectList(timeZones, "Value", "Text");
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("Lists.GetTimeZones", ex);
            }

            return null;
        }

        public IEnumerable<SelectListItem> GetLeasePeriods()
        {
            try
            {
                //Dictionary<int, string> leasePeriodsDict = null;

                //if (complimentaryOnly)
                //{
                //    leasePeriodsDict = new Dictionary<int, string>()
                //    {
                //        {1, "1 Month"}
                //    };
                //}
                //else
                //{
                //    leasePeriodsDict = new Dictionary<int, string>()
                //    {
                //        {1, "1 Month"},
                //        {2, "2 Months"},
                //        {3, "3 Months"},
                //        {6, "6 Months"},
                //        {12, "1 Year"},
                //        {24, "2 Years"}
                //    };
                //}

                //List<SelectListItem> periods = leasePeriodsDict
                //    .OrderBy(p => p.Key)
                //        .Select(lp =>
                //        new SelectListItem
                //        {
                //            Value = lp.Key.ToString(),
                //            Text = lp.Value
                //        }).ToList();

                //if (!complimentaryOnly)
                //{
                //    SelectListItem firstItem = new SelectListItem()
                //    {
                //        Value = "0",
                //        Text = "--- select lease period ---"
                //    };

                //    periods.Insert(0, firstItem);
                //}

                List<SelectListItem> prices = _context.NumberPricing.Where(n => n.Enabled && n.CountryId == 22)
                .OrderBy(n => n.SortOrder)
                    .Select(n =>
                    new SelectListItem
                    {
                        Value = $"{n.LeasePeriodType}|{n.LeasePeriod}|{n.Cost:N2}",
                        Text = $"{n.Description} - {n.Cost:C2}"
                    }).ToList();

                SelectListItem firstItem = new SelectListItem()
                {
                    Value = "",
                    Text = "--- select a lease period ---"
                };

                prices.Insert(0, firstItem);

                return new SelectList(prices, "Value", "Text");
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("Lists.GetLeasePeriods", ex);
            }

            return null;
        }

        public IEnumerable<SelectListItem> GetCreditAmounts(string purchaseType)
        {
            try
            {
                Dictionary<decimal, string> creditAmountsDict = new Dictionary<decimal, string>()
                {
                    {(decimal)1.00, "$1.00"},
                    {(decimal)2.00, "$2.00"},
                    {(decimal)5.00, "$5.00"},
                    {(decimal)10.00, "$10.00"},
                    {(decimal)20.00, "$20.00"},
                    {(decimal)30.00, "$30.00"},
                    {(decimal)40.00, "$40.00"},
                    {(decimal)50.00, "$50.00" },
                    {(decimal)75.00, "$75.00" },
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

                string firstItemText = "--- select credit amount ---";
                string firstitemValue = "-1";

                if (purchaseType != "Credit")
                {
                    firstitemValue = "0";
                    firstItemText = "No additional credit";
                }

                SelectListItem firstItem = new SelectListItem()
                {
                    Value = firstitemValue,
                    Text = firstItemText
                };

                periods.Insert(0, firstItem);

                return new SelectList(periods, "Value", "Text");
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("Lists.GetCreditAmountsList", ex);
            }

            return null;
        }

        public IEnumerable<SelectListItem> GetPooledNumbersList()
        {
            try
            {
                List<SelectListItem> pooledNumbers = _context.PooledNumbers
                .Where(pn => pn.IsFreeNumber == false)
                .OrderBy(pn => pn.VirtualNumber)
                    .Select(pn =>
                    new SelectListItem
                    {
                        Value = pn.VirtualNumber,
                        Text = $"{Utilities.NumberToDisplayFormat(pn.VirtualNumber, 22)} - {pn.Description}"
                    }).ToList();

                SelectListItem firstItem = new SelectListItem()
                {
                    Value = null,
                    Text = "--- select a number ---"
                };

                pooledNumbers.Insert(0, firstItem);

                return new SelectList(pooledNumbers, "Value", "Text");
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("Lists.GetPooledNumbers", ex);
            }

            return null;
        }

        public IEnumerable<SelectListItem> GetFreeNumbersList()
        {
            try
            {
                List<SelectListItem> freeNumbers = _context.PooledNumbers
                .Where(pn => pn.IsFreeNumber == true)
                .OrderBy(pn => pn.VirtualNumber)
                    .Select(pn =>
                    new SelectListItem
                    {
                        Value = pn.VirtualNumberId.ToString(),
                        Text = $"{Utilities.NumberToDisplayFormat(pn.VirtualNumber, 22)} - {pn.Description}"
                    }).ToList();

                SelectListItem firstItem = new SelectListItem()
                {
                    Value = null,
                    Text = "--- select a number ---"
                };

                freeNumbers.Insert(0, firstItem);

                return new SelectList(freeNumbers, "Value", "Text");
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("Lists.GetFreeNumbersList", ex);
            }

            return null;
        }

        public IEnumerable<SelectListItem> GetVirtualNumbersForAccount(int accountId, bool includeExpiredNumbers)
        {
            try
            {
                List<SelectListItem> virtualNumbers = _context.DedicatedVirtualNumbers.Where(x => x.AccountId == accountId && x.Cancelled == includeExpiredNumbers)
                .OrderByDescending(x => x.VirtualNumberId)
                    .Select(vn =>
                    new SelectListItem
                    {
                        Value = vn.VirtualNumberId.ToString(),
                        Text = $"{Utilities.NumberToDisplayFormat(vn.VirtualNumber, 22)}"
                    }).ToList();

                SelectListItem firstItem = new SelectListItem()
                {
                    Value = null,
                    Text = "--- select a number ---"
                };

                virtualNumbers.Insert(0, firstItem);

                return new SelectList(virtualNumbers, "Value", "Text");
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("Lists.GetVirtualNumbersForAccount", ex);
            }

            return null;
        }

        public IEnumerable<SelectListItem> GetTollFreeAreaCodes()
        {
            try
            {
                Dictionary<string, string> areaCodesDict = new Dictionary<string, string>()
                {
                    {"800", "800"},
                    //{"833", "833"},
                    {"844", "844"},
                    {"855", "855"},
                    {"866", "866"},
                    {"877", "877"},
                    {"888", "888"},
                };

                List<SelectListItem> areaCodes = areaCodesDict
                    .OrderBy(p => p.Key)
                        .Select(lp =>
                        new SelectListItem
                        {
                            Value = lp.Key,
                            Text = lp.Value
                        }).ToList();

                string firstItemText = "--- select ---";


                SelectListItem firstItem = new SelectListItem()
                {
                    Value = "",
                    Text = firstItemText
                };

                areaCodes.Insert(0, firstItem);

                return new SelectList(areaCodes, "Value", "Text");
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("Lists.GetTollFreeAreaCodes", ex);
            }

            return null;
        }

        public IEnumerable<SelectListItem> GetAPIApplicationsList(int accountId, int virtualumberId)
        {
            try
            {
                List<SelectListItem> applicationsDDlist = new List<SelectListItem>();
                List<APIApplication> appsList = _context.APIApplications.Where(x => x.AccountId == accountId).ToList();
                foreach (APIApplication application in appsList)
                {
                    SelectListItem listItem = new SelectListItem();

                    listItem.Value = application.APIApplicationId.ToString();
                    listItem.Text = application.ApplicationName;

                    applicationsDDlist.Add(listItem);
                }

                SelectListItem firstItem = new SelectListItem()
                {
                    Value = "0",
                    Text = (virtualumberId > 0) ? "Unassign Application" : "--- add new application ---"
                };

                applicationsDDlist.Insert(0, firstItem);

                return new SelectList(applicationsDDlist, "Value", "Text");
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("Lists.GetAPIApplicationsList", ex);
            }

            return null;
        }

        public IEnumerable<SelectListItem> GetSupportCategoriesList(SupportRequestType requestType)
        {
            try
            {
                Dictionary<string, string> supportTypesDict;
                if (requestType == SupportRequestType.Contact)
                {
                    supportTypesDict = new Dictionary<string, string>()
                    {
                        {"General", "General question" },
                        {"Feedback", "I want to provide feedback" },
                        {"Abuse", "I want to report abuse" }
                    };
                }
                else
                {
                    supportTypesDict = new Dictionary<string, string>()
                    {
                        {"General", "I have a general question" },
                        {"Feedback", "I want to provide feedback" },
                        {"NotDelivered", "My messages were not delivered" },
                        {"Error", "I received an error" },
                        {"Abuse", "I want to report abuse" }
                    };
                }

                List<SelectListItem> items = supportTypesDict
                        .Select(lp =>
                        new SelectListItem
                        {
                            Value = lp.Key,
                            Text = lp.Value
                        }).ToList();

                string firstItemText = "--- select a category ---";
                string firstitemValue = "-1";

                SelectListItem firstItem = new SelectListItem()
                {
                    Value = firstitemValue,
                    Text = firstItemText
                };

                items.Insert(0, firstItem);

                return new SelectList(items, "Value", "Text");
            }
            catch (Exception ex)
            {
                ErrorHandling eh = new ErrorHandling();
                eh.LogException("Lists.GetSupportCategoriesList", ex);
            }

            return null;
        }

        #endregion

    }
}