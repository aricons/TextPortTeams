using System;
using System.Collections.Generic;

using TextPortCore.Data;

namespace TextPortCore.Models
{
    public class NumbersContainer
    {
        public int AccountId { get; set; }
        public bool ShowExpiredNumbers { get; set; }
        public List<DedicatedVirtualNumber> Numbers { get; set; }


        // Constructors
        public NumbersContainer()
        {
            this.AccountId = 0;
            this.Numbers = new List<DedicatedVirtualNumber>();
        }

        public NumbersContainer(int accId, bool showExpiredNumbers)
        {
            this.AccountId = accId;
            this.ShowExpiredNumbers = showExpiredNumbers;

            using (TextPortDA da = new TextPortDA())
            {
                this.Numbers = da.GetNumbersForAccount(accId, this.ShowExpiredNumbers);
            }
        }
    }
}
