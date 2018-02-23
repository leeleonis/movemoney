using Newtonsoft.Json;

namespace HitbtcApi.Objects
{
    public class ErrorMessage
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
