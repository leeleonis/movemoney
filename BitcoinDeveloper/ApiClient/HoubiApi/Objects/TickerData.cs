using Newtonsoft.Json;
using System.Collections.Generic;

namespace HuobiApi.Objects
{
    class TickerData
    {
        [JsonProperty("bids")]
        public List<decimal[]> Bids { get; set; }

        [JsonProperty("asks")]
        public List<decimal[]> Asks { get; set; }
    }
}
