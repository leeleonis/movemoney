using Binance.Net;
using Binance.Net.Objects;
using System;
using System.Linq;
using System.Threading;

namespace BitcoinService.ApiClient.BinanceApi
{
    class Binance
    {
        private string ApiKey = "";
        private string ApiSecret = "";
        private bool InProgress;

        private BinanceClient ApiClient;
        private BinanceSocketClient ApiSocketClient;

        public Binance(string key, string secret)
        {
            ApiKey = key;
            ApiSecret = secret;
        }

        public Binance() : this("", "") { }
        /// <summary>
        /// 24小時內的報價
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public ApiResult<Binance24HPrice> Ticker(string symbol)
        {
            using (ApiClient = new BinanceClient())
            {
                BinanceApiResult<Binance24HPrice> ticker = ApiClient.Get24HPrices(symbol);

                return Return(ticker);
            }
        }

        private ApiResult<T> Return<T>(BinanceApiResult<T> apiResult) where T : class
        {
            ApiResult<T> Result = new ApiResult<T>();

            if (apiResult.Success)
            {
                Result.Data = apiResult.Data;
            }
            else
            {
                Result.Message = apiResult.Error.Message;
            }

            return Result;
        }

        public void TickerThread(ExchangeData Data)
        {
            InProgress = true;

            using (ApiSocketClient = new BinanceSocketClient())
            {

                //ApiSocketClient.SubscribeToAccountUpdateStream("ikKz2GYMzCSNFjPzPeJHGYQI6sXwVNIzX7KNg31FDZkRE9OVrOE3al8PQwS8", (data) =>
                //{
                //    var d = data;
                
                //});
                //ETHBTC 
                //BTCUSDT
                //QTUMBTC

                var successDepth = ApiSocketClient.SubscribeToDepthStream(Data.ExchangeType, (data) =>
                {
                    if (data.Asks.Any()) Data.Ask = (decimal)data.Asks.OrderBy(x=>x.Price).First().Price;
                    if (data.Bids.Any()) Data.Bid = (decimal)data.Bids.OrderByDescending(x => x.Price).First().Price;
                    Data.UpdateTime = DateTime.UtcNow;
                });

                if (!successDepth.Success) throw new Exception(successDepth.Error.Message);

                while (InProgress) Thread.Sleep(500);

                ApiSocketClient.UnsubscribeFromStream(successDepth.Data);
                Data.Status = EnumData.ExchangeStatus.停止;
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
            using (var client = new BinanceClient(lowestAsk.APIKey, lowestAsk.Secret))
            {
                //client.AutoTimestamp = true;
                var orderResult = client.PlaceOrder(lowestAsk.ExchangeType, OrderSide.Buy, OrderType.Limit, TimeInForce.ImmediateOrCancel,MinQuantity, lowestAsk.Ask);
                return new ExchangeApiData { Stace = orderResult.Success, Msg = orderResult.Error.Message };        
            }
        }
        /// <summary>
        ///  交易所賣出
        /// </summary>
        /// <param name="highestBid">(賣價)最高資訊</param>
        /// <param name="MinQuantity">數量</param>
        /// <returns></returns>
        internal ExchangeApiData PlaceOrderBid(ExchangeData highestBid, decimal MinQuantity)
        {
            using (var client = new BinanceClient(highestBid.APIKey, highestBid.Secret))
            {
                var orderResult = client.PlaceOrder(highestBid.ExchangeType, OrderSide.Sell, OrderType.Limit, TimeInForce.ImmediateOrCancel, MinQuantity, highestBid.Ask);
                return new ExchangeApiData { Stace = orderResult.Success, Msg = orderResult.Error.Message };        
            }
        }
    }
}
