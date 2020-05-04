using System.Collections.Generic;

namespace TextPortCore.Integrations.Nexmo
{
    public class NexmoNumber
    {
        public string country { get; set; }
        public string msisdn { get; set; }
        public string type { get; set; }
        public string cost { get; set; }
        public List<string> features { get; set; }

        public NexmoNumber()
        {
            this.country = string.Empty;
            this.msisdn = string.Empty;
            this.type = string.Empty;
            this.cost = string.Empty;
            this.features = new List<string>();
        }
    }
}