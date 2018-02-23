using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinService.ApiClient
{
    class ExchangeData
    {
        /// <summary>
        /// 執行狀態
        /// </summary>
        public EnumData.ExchangeStatus Status { get; set; }
        /// <summary>
        /// 交易所名稱
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// APIKey
        /// </summary>
         public string APIKey{ get; private set; }
        /// <summary>
         /// Secret
        /// </summary>
         public string Secret { get; private set; }
        /// <summary>
        /// 交易幣別
        /// </summary>
        public string ExchangeType { get; private set; }
        /// <summary>
        /// 基準幣(主)
        /// </summary>
        public string MC { get; private set; }
        /// <summary>
        /// 兌換幣(副)
        /// </summary>
        public string DC { get; private set; }
        /// <summary>
        /// 買價(找最便宜的)
        /// </summary>
        public decimal Ask { get; set; }
        /// <summary>
        /// 賣價(找最高)
        /// </summary>
        public decimal Bid { get; set; }
        /// <summary>
        /// 手續費
        /// </summary>
        public decimal Fee { get; set; }
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime UpdateTime { get; set; }

        public ExchangeData(string Name, decimal Fee, string ExchangeType, string MC, string DC, string APIKey, string Secret)
        {
            this.Status = EnumData.ExchangeStatus.停止;
            this.Name = Name;
            this.MC = MC;
            this.DC = DC;
            this.Ask = 0;
            this.Bid = 0;
            this.Fee = Fee;
            this.ExchangeType = ExchangeType;
            this.UpdateTime = DateTime.UtcNow;
            this.APIKey = APIKey;
            this.Secret = Secret;
        }
    }
    public class ExchangeApiData
    {
        public string Name { get; set; }
        public bool Stace { get; set; }
        public string Msg { get; set; }
    }
    public class StockExchangeApiData
     {
        public bool Stace { get; set; }
        public ExchangeApiData AskStace { get; set; }
        public ExchangeApiData BidStace { get; set; }
     }
}
