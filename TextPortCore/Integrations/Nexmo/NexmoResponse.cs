using RestSharp.Deserializers;

namespace TextPortCore.Integrations.Nexmo
{
    public class NexmoResponse
    {
        [DeserializeAs(Name = "error-code")]
        public string error_code { get; set; }

        [DeserializeAs(Name = "error-code-label")]
        public string error_code_label { get; set; }

        public NexmoResponse()
        {
            this.error_code = string.Empty;
            this.error_code_label = string.Empty;
        }
    }
}
