using System;
using System.Collections.Generic;

using TextPortCore.Data;

namespace TextPortCore.Models
{
    public class NumbersContainer
    {
        public int AccountId { get; set; }
        public bool ShowExpiredNumbers { get; set; }
        public int ApiAppCount { get; set; }
        public List<NumberWithAPIDetail> Numbers { get; set; }


        // Constructors
        public NumbersContainer()
        {
            this.AccountId = 0;
            this.Numbers = new List<NumberWithAPIDetail>();
            this.ApiAppCount = 0;
        }

        public NumbersContainer(int accId, bool showExpiredNumbers)
        {
            this.AccountId = accId;
            this.ShowExpiredNumbers = showExpiredNumbers;
            this.ApiAppCount = 0;
            this.Numbers = new List<NumberWithAPIDetail>();

            using (TextPortDA da = new TextPortDA())
            {
                List<APIApplication> apiApps = da.GetAPIApplicationsForAccount(accId);
                if (apiApps != null && apiApps.Count > 0)
                {
                    this.ApiAppCount = apiApps.Count;
                }

                List<DedicatedVirtualNumber> vns = da.GetNumbersForAccount(accId, this.ShowExpiredNumbers);
                foreach (DedicatedVirtualNumber vn in vns)
                {
                    NumberWithAPIDetail number = new NumberWithAPIDetail(vn);
                    if (vn.APIApplicationId != null && vn.APIApplicationId > 0)
                    {
                        number.ApiAppName = da.GetAPIApplicationById((int)vn.APIApplicationId).ApplicationName;
                    }
                    this.Numbers.Add(number);
                }
            }
        }
    }
}
