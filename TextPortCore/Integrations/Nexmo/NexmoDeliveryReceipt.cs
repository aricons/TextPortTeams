using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TextPortCore.Integrations.Nexmo
{
    public partial class NexmoDeliveryReceipt
    {
        [JsonProperty("msisdn")]
        public string Msisdn { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("network-code")]
        public string NetworkCode { get; set; }

        [JsonProperty("messageId")]
        public string MessageId { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("scts")]
        public string Scts { get; set; }

        [JsonProperty("err-code")]
        public string ErrCode { get; set; }

        [JsonProperty("api-key")]
        public string ApiKey { get; set; }

        [JsonProperty("client-ref")]
        public string ClientRef { get; set; }

        [JsonProperty("message-timestamp")]
        public DateTimeOffset MessageTimestamp { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("nonce")]
        public Guid nonce { get; set; }

        [JsonProperty("sig")]
        public string sig { get; set; }
    }
}