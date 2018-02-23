using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinService.ApiClient.PoloniexApi
{
    class TickerData
    {
        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
        public decimal Last { get; set; }
        public decimal Open { get; set; }
        public decimal Low { get; set; }
        public decimal High { get; set; }
        public decimal Volume { get; set; }
        public decimal VolumeQuote { get; set; }
        public DateTime timestamp { get; set; }
        public string Symbol { get; set; }
    }
}
