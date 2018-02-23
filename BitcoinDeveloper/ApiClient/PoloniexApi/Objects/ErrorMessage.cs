using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinService.ApiClient.PoloniexApi
{
    public class ErrorMessage
    {
        [JsonProperty("err-code")]
        public string Code { get; set; }
        [JsonProperty("err-msg")]
        public string Message { get; set; }
    }
}
