using System;
using System.Configuration;
using System.Web.Script.Serialization;

using TextPortCore.Models;

namespace TextPortCore.Integrations.Bandwidth
{
    public class BandwidthOutboundMessage
    {
        public string from { get; set; }
        public string to { get; set; }
        public string text { get; set; }
        public string callbackUrl { get; set; }

        // Constructors
        public BandwidthOutboundMessage()
        {
            this.from = String.Empty;
            this.to = String.Empty;
            this.text = String.Empty;
            this.callbackUrl = ConfigurationManager.AppSettings["BandwidthComCallbackUrl"];
        }

        //public BandwidthOutboundMessage(string fromNumber, string toNumber, string message)
        public BandwidthOutboundMessage(Message msg)
        {
            this.from = msg.VirtualNumber;
            this.to = msg.MobileNumber;
            this.text = msg.MessageText;
            this.callbackUrl = ConfigurationManager.AppSettings["BandwidthComCallbackUrl"];
        }

        // Public methods
        //public BandwidthResponseData Send(string url)
        //{
        //    BandwidthResponseData responseData = new BandwidthResponseData((decimal)0.0075);
        //    try
        //    {
        //        string jsonMessage = new JavaScriptSerializer().Serialize(this);
        //        responseData.MessageID = REST.PostData(url, jsonMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        string foo = ex.Message;
        //    }
        //    return responseData;
        //}
    }
}