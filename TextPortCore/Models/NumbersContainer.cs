using System;
using System.Collections.Generic;

using TextPortCore.Data;

namespace TextPortCore.Models
{
    public class NumbersContainer
    {
        public int BranchId { get; set; }
        public bool ShowExpiredNumbers { get; set; }
        public int BranchCount { get; set; }
        public List<NumberWithBranchDetail> Numbers { get; set; }


        // Constructors
        //public NumbersContainer()
        //{
        //    this.BranchId = 0;
        //    this.Numbers = new List<NumberWithBranchDetail>();
        //}

        //public NumbersContainer(int branchId, bool showExpiredNumbers)
        public NumbersContainer()
        {
            this.BranchId = 0;
            this.ShowExpiredNumbers = false;
            this.Numbers = new List<NumberWithBranchDetail>();

            using (TextPortDA da = new TextPortDA())
            {
                //List<DedicatedVirtualNumber> vns = da.GetNumbersForBranch(branchId, this.ShowExpiredNumbers);
                List<DedicatedVirtualNumber> vns = da.GetActiveNumbers();
                foreach (DedicatedVirtualNumber vn in vns)
                {
                    NumberWithBranchDetail number = new NumberWithBranchDetail(vn);
                    this.Numbers.Add(number);
                }
            }
        }
    }
}
