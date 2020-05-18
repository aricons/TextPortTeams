using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestSharp.Deserializers;

namespace TextPortCore.Integrations.Nexmo
{
    public class NexmoMessageAck
    {
        [DeserializeAs(Name = "message-count")]
        public int message_count { get; set; }
        public List<NexmoMessageAckDetails> messages { get; set; }

        public NexmoMessageAck()
        {
            this.message_count = 0;
            this.messages = new List<NexmoMessageAckDetails>();
        }
    }
}
