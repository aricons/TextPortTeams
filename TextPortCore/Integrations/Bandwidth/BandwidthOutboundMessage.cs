using System;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Collections.Generic;

using Newtonsoft.Json;

using TextPortCore.Models;

namespace TextPortCore.Integrations.Bandwidth
{
    public class BandwidthOutboundMessage
    {
        public string from { get; set; }
        public string to { get; set; }
        public string text { get; set; }
        public string applicationId { get; set; }
        public string tag { get; set; }
        public List<string> media { get; set; }
        //public string callbackUrl { get; set; }

        // Constructors
        public BandwidthOutboundMessage()
        {
            this.from = String.Empty;
            this.to = String.Empty;
            this.text = String.Empty;
            this.media = null;
            //this.callbackUrl = ConfigurationManager.AppSettings["BandwidthComCallbackUrl"];
        }

        //public BandwidthOutboundMessage(string fromNumber, string toNumber, string message)
        public BandwidthOutboundMessage(Message msg, string appId)
        {
            this.from = msg.NumberBandwidthFormat;
            this.to = msg.MobileNumber;
            this.text = msg.MessageText;
            this.applicationId = appId;
            this.tag = $"Account {msg.AccountId}";
            //this.callbackUrl = ConfigurationManager.AppSettings["BandwidthComCallbackUrl"];
            if (msg.MMSFiles != null && msg.MMSFiles.Count > 0)
            {
                //List<string> mmsFileNames = JsonConvert.DeserializeObject<List<string>>(msg.MmsfileNames);
                this.media = new List<string>();
                foreach (MMSFile mmsFile in msg.MMSFiles)
                {
                    this.media.Add($"{ConfigurationManager.AppSettings["MMSPublicBaseUrl"]}{msg.AccountId}/{mmsFile.FileName}");
                }
            }
        }
    }

    //{"id":"1558161653161imgtl7etubespbfe","owner":"+15053172293","applicationId":"5abf6fa7-5e0f-4f1c-828b-c01f0c9674c1","time":"2019-05-18T06:40:53.161Z","segmentCount":1,"direction":"out","to":["+19492339386"],"from":"+15053172293","text":"Bandwidth V2 Test Message","tag":"Account 1"}


    public class BandwidthMessageResponse
    {
        public string id { get; set; }
        public string owner { get; set; }
        public string applicationId { get; set; }
        public DateTime time { get; set; }
        public int segmentCount { get; set; }
        public string direction { get; set; }
        public List<string> to { get; set; }
        public string from { get; set; }
        public string text { get; set; }
        public string tag { get; set; }
    }
}