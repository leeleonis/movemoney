using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BitcoinDeveloper.ApiClient.PoloniexApi;

namespace BitcoinService.ApiClient.PoloniexApi
{
    class Poloniex
    {
        private string ApiKey;
        private string ApiSecret;
        private bool InProgress;
        private PoloniexClient ApiClient;
        public Poloniex(string key, string secret)
        {
            ApiKey = key;
            ApiSecret = secret;
        }
        public Poloniex() : this("", "") { }
        public async void TickerThread(ExchangeData Data)
        {
            InProgress = true;
           using (ApiClient = new PoloniexClient(ApiKey, ApiSecret))
           {
               ApiClient.OnError += (sender, error) =>
               {
                   Data.Status = EnumData.ExchangeStatus.異常;
                   Data.ErrorMsg = error.Message;
               };

               ApiClient.OnClose += (sender, e) => { Data.Status = EnumData.ExchangeStatus.停止; };
               //USDT_BTC
               var SocketResult = ApiClient.SubscribeTicker(Data.ExchangeType, (data) =>
               {
                   Data.Ask = data.Data.Ask;
                   Data.Bid = data.Data.Bid;
                   Data.UpdateTime = DateTime.UtcNow;
               });

               if (!SocketResult.Status) throw new Exception(SocketResult.Message);

               while (InProgress) Thread.Sleep(500);

               ApiClient.UnsubscribeFromStream(SocketResult.Data);
           }
        }

        public void ThreadStop()
        {
            InProgress = false;
        }
        /// <summary>
        /// 交易所買進
        /// </summary>
        /// <param name="lowestAsk">(買價)最低資訊</param>
        /// <param name="MinQuantity">數量</param>
        /// <returns></returns>
        internal ExchangeApiData PlaceOrderAsk(ExchangeData lowestAsk, decimal MinQuantity)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///  交易所賣出
        /// </summary>
        /// <param name="highestBid">(賣價)最高資訊</param>
        /// <param name="MinQuantity">數量</param>
        /// <returns></returns>
        internal ExchangeApiData PlaceOrderBid(ExchangeData highestBid, decimal MinQuantity)
        {
            throw new NotImplementedException();
        }
    }
}
