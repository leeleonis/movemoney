using Newtonsoft.Json;

namespace HitbtcApi.Objects
{
    public class SocketResult
    {
        [JsonProperty("result")]
        public bool Status { get; set; }

        [JsonProperty("error")]
        public ErrorMessage Error { get; set; }

        [JsonProperty("id")]
        public int ID { get; set; }
    }
}
