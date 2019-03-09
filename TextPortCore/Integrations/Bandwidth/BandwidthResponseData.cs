using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextPortCore.Integrations.Bandwidth
{
    public class BandwidthResponseData
    {
        public string MessageID { get; set; }
        public decimal Price { get; set; }

        public BandwidthResponseData()
        {
            this.MessageID = String.Empty;
            this.Price = 0;
        }

        public BandwidthResponseData(decimal defaultPrice)
        {
            this.MessageID = String.Empty;
            this.Price = defaultPrice;
        }
    }
}
