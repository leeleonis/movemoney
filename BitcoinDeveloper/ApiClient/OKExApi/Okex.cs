using BitcoinDeveloper.ApiClient.OKExApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BitcoinService.ApiClient.OKExApi
{
    class BuissnesServiceImpl : WebSocketService
    {
        public void onReceive(string msg)
        {
          //  Console.WriteLine(msg);
        }
    }
    class OKExAPI
    {
        private string APIKey;
        private string Secret;
        private string urlApi = "https://www.okex.com";
        private bool InProgress;
        public OKExAPI(string APIKey, string Secret)
        {
            this.APIKey = APIKey;
            this.Secret = Secret;
        }
        public OKExAPI() : this("", "") { }
        public void TickerThread(ExchangeData Data)
        {
            InProgress = true;
            //string url = "wss://real.okcoin.com:10440/websocket/okcoinapi";
            string url = "wss://real.okex.com:10441/websocket";
           
            WebSocketService wss = new BuissnesServiceImpl();
            OKexClient wb = new OKexClient(url, wss);
            wb.start();

            //websocket.send("{'event':'addChannel','channel':'ok_sub_futureusd_X_ticker_Y'}");
            //wb.send("{'event':'addChannel','channel':'ok_sub_spotcny_btc_ticker'}");
            //wb.send("{'event':'addChannel','channel':'ok_sub_spot_eth_btc_ticker'}");
            while (InProgress)
            {
                if (wb.isReconnect())
                {
                    wb.send("{'event':'addChannel','channel':'ok_sub_spot_" + Data.ExchangeType + "_ticker'}");
                }
                Thread.Sleep(1000);
            }
            Data.Status = EnumData.ExchangeStatus.停止;
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
            StockRestApi postRequest1 = new StockRestApi(urlApi, lowestAsk.APIKey, lowestAsk.Secret);
            var orderResult = ReturnData(postRequest1.trade(lowestAsk.ExchangeType, "buy", lowestAsk.Ask.ToString(), MinQuantity.ToString()));
            return new ExchangeApiData { Stace = orderResult.result, Msg = orderResult.Msg };       
        }

        /// <summary>
        ///  交易所賣出
        /// </summary>
        /// <param name="highestBid">(賣價)最高資訊</param>
        /// <param name="MinQuantity">數量</param>
        /// <returns></returns>
        internal ExchangeApiData PlaceOrderBid(ExchangeData highestBid, decimal MinQuantity)
        {
            StockRestApi postRequest1 = new StockRestApi(urlApi, highestBid.APIKey, highestBid.Secret);
            var orderResult = ReturnData(postRequest1.trade(highestBid.ExchangeType, "sell", highestBid.Bid.ToString(), MinQuantity.ToString()));
            return new ExchangeApiData { Stace = orderResult.result, Msg = orderResult.Msg };   
        }
        private ReturnData ReturnData(string respons)
        {
            var ReturnDataVal = JsonConvert.DeserializeObject<ReturnData>(respons);
            if (!string.IsNullOrWhiteSpace(ReturnDataVal.error_code))
            {
                ReturnDataVal.Msg = ErrCode.Error_codeVal(ReturnDataVal.error_code);
            }
            return ReturnDataVal;
        }      
    }
    class ReturnData
    {
        public bool result { get; set; }
        public string error_code { get; set; }
        public string Msg { get; set; }
    }
 
}
