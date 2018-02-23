using Newtonsoft.Json;

namespace HuobiApi.Objects
{
    public class ErrorMessage
    {
        [JsonProperty("err-code")]
        public string Code { get; set; }
        [JsonProperty("err-msg")]
        public string Message { get; set; }
    }
}
