using System;

namespace TextPortCore.Models.API
{
    public class NumberRequest
    {
        public string Number { get; set; }
        public int LeasePeriod { get; set; }

        // Constructors
        public NumberRequest()
        {
            this.Number = string.Empty;
            this.LeasePeriod = 1;
        }
    }
}


