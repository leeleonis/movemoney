using BitcoinDeveloper.ApiClient.PoloniexApi;
using Newtonsoft.Json;
namespace PoloniexApi.Objects
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
