using Newtonsoft.Json;

namespace HuobiApi.Objects
{
    public class SocketResult
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        public ErrorMessage Error { get; set; }

        [JsonProperty("id")]
        public string ID { get; set; }
    }
}
