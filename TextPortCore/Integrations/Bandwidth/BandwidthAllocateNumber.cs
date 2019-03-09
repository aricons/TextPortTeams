using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextPortCore.Integrations.Bandwidth
{
    class BandwidthAllocateNumber
    {
        public string number { get; set; }
        public string name { get; set; }
        public string applicationId { get; set; }
        public string fallbackNumber { get; set; }

        public BandwidthAllocateNumber(string bwNumber, int accountId, string bwApplicationId)
        {
            this.number = bwNumber;
            this.name = $"V2_{applicationId}";
            this.applicationId = bwApplicationId;
            this.fallbackNumber = string.Empty;
        }
    }
}
