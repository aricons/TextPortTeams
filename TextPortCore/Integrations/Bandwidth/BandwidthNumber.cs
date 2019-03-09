using System;
using System.Configuration;
using System.Collections.Generic;

namespace TextPortCore.Integrations.Bandwidth
{
    public class BandwidthNumber
    {
        public string number { get; set; }
        public string nationalNumber { get; set; }
        public string patternMatch { get; set; }
        public string city { get; set; }
        public string lata { get; set; }
        public string rateCenter { get; set; }
        public string state { get; set; }
    }

    public class BandwidthNumbersList
    {
        public List<BandwidthNumber> Numbers { get; set; }

        public BandwidthNumbersList()
        {
            this.Numbers = new List<BandwidthNumber>();
        }
    }

    
}