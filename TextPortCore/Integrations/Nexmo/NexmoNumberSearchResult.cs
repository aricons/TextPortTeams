using System.Collections.Generic;

namespace TextPortCore.Integrations.Nexmo
{
    public class NexmoNumberSearchResult
    {
        public int count { get; set; }
        public List<NexmoNumber> numbers { get; set; }
    }
}