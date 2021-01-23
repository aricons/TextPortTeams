using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextPortCore.Integrations.Coinbase
{
    public class ChargeEvent
    {
        public int id { get; set; }
        public DateTime scheduled_for { get; set; }
        public Event @event { get; set; }

        public ChargeEvent() { }
    }

    public class Event
    {
        public string id { get; set; }
        public string resource { get; set; }
        public string type { get; set; }
        public string api_version { get; set; }
        public DateTime created_at { get; set; }
        public Data data { get; set; }

        public Event() { }
    }
}
