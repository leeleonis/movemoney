using Newtonsoft.Json;

namespace HuobiApi.Objects
{
    class SocketData<T>
    {
        [JsonProperty("ch")]
        public string Method { get; set; }
        
        public T Data { get; set; }
    }
}
