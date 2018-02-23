using Bittrex.Net;
using Bittrex.Net.Objects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace BitcoinService.ApiClient.BittrexApi
{
    class Bittrex
    {
        private string ApiKey = "";
        private string ApiSecret = "";
        private bool InProgress;

        private BittrexClient ApiClient;
        private BittrexSocketClient ApiSocketClient;

        public Bittrex(string key, string secret)
        {
            ApiKey = key;
            ApiSecret = secret;
        }

        public Bittrex() : this("", "") { }

        public ApiResult<BittrexPrice> Ticker(string market)
        {
            using(ApiClient = new BittrexClient())
            {
                BittrexApiResult<BittrexPrice> ticker = ApiClient.GetTicker(market);

                return Return(ticker);
            }
        }

        private ApiResult<T> Return<T>(BittrexApiResult<T> apiResult) where T : class
        {
            ApiResult<T> Result = new ApiResult<T>();

            if (apiResult.Success)
            {
                Result.Data = apiResult.Result;
            }
            else
            {
                Result.Message = apiResult.Error.ErrorMessage;
            }

            return Result;
        }

        public void TickerThread(ExchangeData Data)
        {
            InProgress = true;
            //BTC-ETH
            //USDT-BTC
            //BTC-QTUM
            using (var BittrexClient = new BittrexClient())
            {
                while (InProgress)
                {
                    //var markets = BittrexClient.GetMarkets();
                    //var currencies = BittrexClient.GetCurrencies().Result.Where(x => x.Currency == "BTC" || x.Currency == "USDT").ToList();
                    //var marketSummary = BittrexClient.GetMarketSummary("BTC-ETH");
                    //var marketSummaries = BittrexClient.GetMarketSummaries();
                    //var orderbook = BittrexClient.GetOrderBook("BTC-ETH");
                    //var marketHistory = BittrexClient.GetMarketHistory("BTC-ETH");
                    //var list = new List<BittrexCurrency>(currencies);
                    var Ticker = BittrexClient.GetTicker(Data.ExchangeType);
                    if (Ticker.Success)
                    {
                        Data.Ask = Convert.ToDecimal(Ticker.Result.Ask);
                        Data.Bid = Convert.ToDecimal(Ticker.Result.Bid);
                        Data.UpdateTime = DateTime.UtcNow;
                    }
                    else
                    {
                        Console.WriteLine(Data.Name + "異常");
                        Console.WriteLine(Ticker.Error);
                    }
                    Thread.Sleep(500);
                }
                //ApiSocketClient.UnsubscribeFromStream(successDepth.Result);
                Data.Status = EnumData.ExchangeStatus.停止;
            }
            //Socket不明原因不能連，疑似官方已關閉
            //using (ApiSocketClient = new BittrexSocketClient())
            //{


            //    var successDepth = ApiSocketClient.SubscribeToMarketDeltaStream("USDT-BTC", (data) =>
            //    {
            //        Console.WriteLine("USDT-BTC: {0}", data.Last);
            //        Data.Ask = Convert.ToDouble(data.Ask);
            //        Data.Bid = Convert.ToDouble(data.Bid);
            //        Data.UpdateTime = DateTime.UtcNow;
            //    });

            //    if (!successDepth.Success)
            //    {
            //        Console.WriteLine(Data.Name + "異常");
            //        Console.WriteLine(successDepth.Error.ErrorMessage);
            //        //throw new Exception(successDepth.Error.ErrorMessage);
            //    }
            //    while (InProgress) Thread.Sleep(500);


            //    ApiSocketClient.UnsubscribeFromStream(successDepth.Result);
            //    Data.Status = EnumData.ExchangeStatus.停止;
            //}
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
            using (var client = new BittrexClient(lowestAsk.APIKey, lowestAsk.Secret))
            {
                var placedOrder = client.PlaceOrder(OrderType.Buy, lowestAsk.ExchangeType, MinQuantity, lowestAsk.Ask);
                return new ExchangeApiData { Stace = placedOrder.Success, Msg = placedOrder.Error.ErrorMessage };
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
            using (var client = new BittrexClient(highestBid.APIKey, highestBid.Secret))
            {
                var placedOrder = client.PlaceOrder(OrderType.Sell, highestBid.ExchangeType, MinQuantity, highestBid.Ask);
                return new ExchangeApiData { Stace = placedOrder.Success, Msg = placedOrder.Error.ErrorMessage };
            }
        }
    }
}
