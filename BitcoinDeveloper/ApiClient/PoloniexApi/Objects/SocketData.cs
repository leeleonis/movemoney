using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinService.ApiClient.PoloniexApi
{
    public class SocketData<T>
    {
        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("params")]
        public T Data { get; set; }
    }
}
