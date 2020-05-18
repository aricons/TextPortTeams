using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TextPortCore.Integrations.Nexmo
{
    public partial class NexmoInboundMessage
    {
        [JsonProperty("api-key")]
        public string ApiKey { get; set; }

        [JsonProperty("msisdn")]
        public string Msisdn { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("messageId")]
        public string MessageId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("keyword")]
        public string Keyword { get; set; }

        [JsonProperty("message-timestamp")]
        public DateTime MessageTimestamp { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("nonce")]
        public Guid Nonce { get; set; }

        [JsonProperty("concat")]
        public bool Concat { get; set; }

        [JsonProperty("concat-ref")]
        public long ConcatRef { get; set; }

        [JsonProperty("concat-total")]
        public long ConcatTotal { get; set; }

        [JsonProperty("concat-part")]
        public long ConcatPart { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("udh")]
        public string Udh { get; set; }
    }
}