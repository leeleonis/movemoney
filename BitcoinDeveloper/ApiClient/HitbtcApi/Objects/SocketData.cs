using Newtonsoft.Json;

namespace HitbtcApi.Objects
{
    public class SocketData<T>
    {
        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("params")]
        public T Data { get; set; }
    }
}
