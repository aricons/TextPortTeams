using RestSharp.Deserializers;

namespace TextPortCore.Integrations.Nexmo
{
    public class NexmoMessageAckDetails
    {
        public string to { get; set; }

        [DeserializeAs(Name = "message-id")]
        public string message_id { get; set; }

        public string status { get; set; }

        [DeserializeAs(Name = "remaining-balance")]
        public string remaining_balance { get; set; }

        [DeserializeAs(Name = "message-price")]
        public string message_price { get; set; }

        public string network { get; set; }

        [DeserializeAs(Name = "client-ref")]
        public string client_ref { get; set; }

        [DeserializeAs(Name = "account-ref")]
        public string account_ref { get; set; }

        public NexmoMessageAckDetails()
        {
            this.to = string.Empty;
            this.message_id = string.Empty;
            this.status = string.Empty;
            this.remaining_balance = string.Empty;
            this.message_price = string.Empty;
            this.network = string.Empty;
            this.client_ref = string.Empty;
            this.account_ref = string.Empty;
        }
    }
}
