using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinService.ApiClient
{
    class ExchangeData
    {
        public EnumData.ExchangeStatus Status { get; set; }
        public string Name { get; private set; }
        public double Ask { get; set; }
        public double Bid { get; set; }
        public string ErrorMsg { get; set; }
        public DateTime UpdateTime { get; set; }

        public ExchangeData(string Name)
        {
            this.Status = EnumData.ExchangeStatus.停止;
            this.Name = Name;
            this.Ask = 0;
            this.Bid = 0;
            this.UpdateTime = DateTime.UtcNow;
        }
    }
}
