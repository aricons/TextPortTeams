using System;

namespace TextPortCore.Models
{
    public partial class NumberPrice
    {
        public int PriceId { get; set; }

        public bool Enabled { get; set; }

        public int CountryId { get; set; }

        public byte SortOrder { get; set; }

        public string LeasePeriodType { get; set; }

        public short LeasePeriod { get; set; }

        public string Description { get; set; }

        public decimal Cost { get; set; }
    }
}
