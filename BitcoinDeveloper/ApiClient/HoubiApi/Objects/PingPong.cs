using Newtonsoft.Json;
using System;

namespace HuobiApi.Objects
{
    public class PingPong : IDisposable
    {
        [JsonProperty("ping")]
        public long pong { get; set; }

        public void Dispose() { }
    }
}
