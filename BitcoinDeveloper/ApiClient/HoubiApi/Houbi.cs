using System;
using System.Linq;
using System.Threading;

namespace BitcoinService.ApiClient.HuobiApi
{
    class Huobi
    {
        private string ApiKey { get; set; }
        private string ApiSecret { get; set; }
        private string AccountId { get; set; }
        private bool InProgress;

        private HuobiClientWebSocket ApiClientWebSocket;
        private HuobiClientREST ApiClientREST;
        public Huobi(string key, string secret)
        {
            ApiKey = key;
            ApiSecret = secret;
        }

        public Huobi() : this("", "") { }

        public void TickerThread(ExchangeData Data)
        {
            InProgress = true;
            ApiClientREST = new HuobiClientREST(ApiKey, ApiSecret);
            AccountId = ApiClientREST.AccountId;
            using (ApiClientWebSocket = new HuobiClientWebSocket(ApiKey, ApiSecret))
            {
                ApiClientWebSocket.OnError += (sender, error) =>
                {
                    Data.Status = EnumData.ExchangeStatus.異常;
                    Data.ErrorMsg = error.Message;
                };

                ApiClientWebSocket.OnClose += (sender, e) => { Data.Status = EnumData.ExchangeStatus.停止; };
                //ethbtc
                //BTCUSDT
                var SocketResult = ApiClientWebSocket.SubscribeTicker(Data.ExchangeType, (data) =>
                {
                    if (data.Data.Asks.Any()) Data.Ask = data.Data.Asks.First().ElementAt(0);
                    if (data.Data.Bids.Any()) Data.Bid = data.Data.Bids.First().ElementAt(0);
                    Data.UpdateTime = DateTime.UtcNow;
                });

                if (!SocketResult.Status) throw new Exception(SocketResult.Message);

                while (InProgress) Thread.Sleep(500);

                ApiClientWebSocket.UnsubscribeFromStream(SocketResult.Data);
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
            if (string.IsNullOrWhiteSpace(AccountId))
            {
                 ApiClientREST = new HuobiClientREST(ApiKey, ApiSecret);
                 AccountId = ApiClientREST.AccountId;
            }
            return ApiClientREST.ordersPlace("buy-limit", lowestAsk.ExchangeType, MinQuantity, lowestAsk.Ask);
        }
        /// <summary>
        ///  交易所賣出
        /// </summary>
        /// <param name="highestBid">(賣價)最高資訊</param>
        /// <param name="MinQuantity">數量</param>
        /// <returns></returns>
        internal ExchangeApiData PlaceOrderBid(ExchangeData highestBid, decimal MinQuantity)
        {
            if (string.IsNullOrWhiteSpace(AccountId))
            {
                ApiClientREST = new HuobiClientREST(ApiKey, ApiSecret);
                AccountId = ApiClientREST.AccountId;
            }
            return ApiClientREST.ordersPlace("buy-limit", highestBid.ExchangeType, MinQuantity, highestBid.Bid);
        }
    }
}
