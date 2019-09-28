using Newtonsoft.Json;
using System.Collections.Generic;
namespace TextPortCore.Helpers
{
    public class CaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("error-codes")]
        public List<string> ErrorMessage { get; set; }
    }
}
