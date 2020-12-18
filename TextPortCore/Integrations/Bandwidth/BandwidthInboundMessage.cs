using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextPortCore.Integrations.Bandwidth
{
    public class BandwidthInboundMessageList
    {
        public List<BandwidthInboundMessage> messages { get; set; }

        public BandwidthInboundMessageList()
        {
            this.messages = new List<BandwidthInboundMessage>();
        }
    }

    public class BandwidthInboundMessage
    {
        public string type { get; set; }
        public DateTime time { get; set; }
        public string description { get; set; }
        public string to { get; set; }
        public string from { get; set; }
        public int errorCode { get; set; }
        public MessageIn message { get; set; }

        public BandwidthInboundMessage()
        {
            this.type = string.Empty;
            this.time = DateTime.MinValue;
            this.description = string.Empty;
            this.to = string.Empty;
            this.from = string.Empty;
            this.message = new MessageIn();
            this.errorCode = 0;
        }
    }

    public class MessageIn
    {
        public string id { get; set; }
        public DateTime time { get; set; }
        public List<string> to { get; set; }
        public string from { get; set; }
        public string text { get; set; }
        public string applicationId { get; set; }
        public List<string> media { get; set; }
        public string owner { get; set; }
        public string direction { get; set; }
        public int segmentCount { get; set; }

        public MessageIn()
        {
            this.id = string.Empty;
            this.time = DateTime.MinValue;
            this.to = new List<string>();
            this.from = string.Empty;
            this.text = string.Empty;
            this.applicationId = string.Empty;
            this.media = new List<string>();
            this.owner = string.Empty;
            this.direction = string.Empty;
            this.segmentCount = 0;
        }
    }
}

//Sample JSON:

//[
//  {
//    "type"        : "message-received",
//    "time"        : "2019-05-20T18:20:16Z",
//    "description" : "Incoming message received",
//    "to"          : "+19493174450",
//    "message"     : {
//      "id"            : "14763090468292kw2fuqty55yp2b2",
//      "time"          : "2016-09-14T18:20:16Z",
//      "to"            : ["+19493174450"],
//      "from"          : "+19492339386",
//      "text"          : "Test inbound message",
//      "applicationId" : "5abf6fa7-5e0f-4f1c-828b-c01f0c9674c1",
//      "media"         : [
//        "https://messaging.bandwidth.com/api/v2/users/3000006/media/14762070468292kw2fuqty55yp2b2/0/bw.png"
//        ],
//      "owner"         : "+19493174450",
//      "direction"     : "in",
//      "segmentCount"  : 1
//    }
//  }
//]