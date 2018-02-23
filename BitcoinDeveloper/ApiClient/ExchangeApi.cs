using BitcoinDeveloper.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BitcoinService.ApiClient
{
    class ExchangeApi
    {
        private BinanceApi.Binance binance;
        private BittrexApi.Bittrex bittrex;
        private HitbtcApi.Hitbtc hitbtc;
        private HuobiApi.Huobi huobi;
        private PoloniexApi.Poloniex Poloniex;
        private OKExApi.OKExAPI OKEx;
        public string ExchangeType { get; set; }
        private bool _start = false;

        public bool InProgress { get { return _start; } }

        public Dictionary<Exchange, ExchangeData> ExchangeList;

        public ExchangeApi()
        {
            ExchangeList = new Dictionary<Exchange, ExchangeData>();
        }
        /// <summary>
        /// 建立連線
        /// </summary>
        /// <param name="Account"></param>
        public string ApiInit(Account Account, string sExchangeType)
        {
            Exchange Exchange = Account.Exchange;
            var ExchangeTypeVal = "";
            var MC = "";
            var DC = "";
            var ExchangeTypelist = Account.Exchange.ExchangeTypes.Where(x => x.Text == sExchangeType);
            if (ExchangeTypelist.Any())
            {
                ExchangeTypeVal = ExchangeTypelist.FirstOrDefault().ExchangeType1;
                MC = ExchangeTypelist.FirstOrDefault().MainCurrency;
                DC =  ExchangeTypelist.FirstOrDefault().DeputyCurrency;
            }
            else
            {
                return Exchange.Name + "：交易所沒有設定對應幣別";
            }

            switch (Exchange.Name.ToLower())
            {
                case "binance":
                    ExchangeList.Add(Exchange, new ExchangeData(Exchange.Name, Account.Exchange.ProcessingFee, ExchangeTypeVal, MC, DC, Account.APIKey, Account.Secret));
                    binance = new BinanceApi.Binance(Account.APIKey, Account.Secret);
                    break;
                case "bittrex":
                    ExchangeList.Add(Exchange, new ExchangeData(Exchange.Name, Account.Exchange.ProcessingFee, ExchangeTypeVal, MC, DC, Account.APIKey, Account.Secret));
                    bittrex = new BittrexApi.Bittrex(Account.APIKey, Account.Secret);
                    break;
                case "hitbtc":
                    ExchangeList.Add(Exchange, new ExchangeData(Exchange.Name, Account.Exchange.ProcessingFee, ExchangeTypeVal, MC, DC, Account.APIKey, Account.Secret));
                    hitbtc = new HitbtcApi.Hitbtc(Account.APIKey, Account.Secret);
                    break;
                case "huobi":
                    ExchangeList.Add(Exchange, new ExchangeData(Exchange.Name, Account.Exchange.ProcessingFee, ExchangeTypeVal, MC, DC, Account.APIKey, Account.Secret));
                    huobi = new HuobiApi.Huobi(Account.APIKey, Account.Secret);
                    break;
                case "poloniex":
                    ExchangeList.Add(Exchange, new ExchangeData(Exchange.Name, Account.Exchange.ProcessingFee, ExchangeTypeVal, MC, DC, Account.APIKey, Account.Secret));
                    Poloniex = new PoloniexApi.Poloniex(Account.APIKey, Account.Secret);
                    break;
                case "okex":
                    ExchangeList.Add(Exchange, new ExchangeData(Exchange.Name, Account.Exchange.ProcessingFee, ExchangeTypeVal, MC, DC, Account.APIKey, Account.Secret));
                    OKEx = new OKExApi.OKExAPI(Account.APIKey, Account.Secret);
                    break;
            }
            return "";
        }

        public void StartUp()
        {
            this._start = true;

            foreach (ExchangeData Data in ExchangeList.Select(e => e.Value).ToList())
            {
                try
                {
                    switch (Data.Name.ToLower())
                    {
                        case "binance":
                            Task.Factory.StartNew(() => binance.TickerThread(Data));
                            Data.Status = EnumData.ExchangeStatus.執行中;
                            Console.WriteLine(string.Format("{0} - 工作開始執行!", Data.Name));
                            break;
                        case "bittrex":
                            Task.Factory.StartNew(() => bittrex.TickerThread(Data));
                            Data.Status = EnumData.ExchangeStatus.執行中;
                            Console.WriteLine(string.Format("{0} - 工作開始執行!", Data.Name));
                            break;
                        case "hitbtc":
                            Task.Factory.StartNew(() => hitbtc.TickerThread(Data));
                            Data.Status = EnumData.ExchangeStatus.執行中;
                            Console.WriteLine(string.Format("{0} - 工作開始執行!", Data.Name));
                            break;
                        case "huobi":
                            Task.Factory.StartNew(() => huobi.TickerThread(Data));
                            Data.Status = EnumData.ExchangeStatus.執行中;
                            Console.WriteLine(string.Format("{0} - 工作開始執行!", Data.Name));
                            break;
                        case "poloniex":
                            Task.Factory.StartNew(() => Poloniex.TickerThread(Data));
                            Data.Status = EnumData.ExchangeStatus.執行中;
                            Console.WriteLine(string.Format("{0} - 工作開始執行!", Data.Name));
                            break;
                        case "okex":
                            Task.Factory.StartNew(() => OKEx.TickerThread(Data));
                            Data.Status = EnumData.ExchangeStatus.執行中;
                            Console.WriteLine(string.Format("{0} - 工作開始執行!", Data.Name));
                            break;
                    }
                }
                catch (Exception e)
                {
                    Data.Status = EnumData.ExchangeStatus.異常;
                    Data.ErrorMsg = e.InnerException == null ? e.Message : e.InnerException.Message;
                }
            }
        }

        public void EndUp()
        {
            this._start = false;
            while (ExchangeList.Any(e => e.Value.Status.Equals(EnumData.ExchangeStatus.執行中)))
            {
                foreach (ExchangeData Data in ExchangeList.Where(e => e.Value.Status.Equals(EnumData.ExchangeStatus.執行中)).Select(e => e.Value).ToList())
                {
                    switch (Data.Name.ToLower())
                    {
                        case "binance":
                            binance.ThreadStop();
                            break;
                        case "bittrex":
                            bittrex.ThreadStop();
                            break;
                        case "hitbtc":
                            hitbtc.ThreadStop();
                            break;
                        case "huobi":
                            huobi.ThreadStop();
                            break;
                        case "poloniex":
                            Poloniex.ThreadStop();
                            break;
                        case "okex":
                            OKEx.ThreadStop();
                            break;
                    }
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 交易所交易
        /// </summary>
        /// <param name="highestBid">(賣價)最高</param>
        /// <param name="lowestAsk">(買價)最低</param>
        /// <param name="MinQuantity">交易數量</param>
        /// <returns></returns>
        internal StockExchangeApiData StockExchange(ExchangeData highestBid, ExchangeData lowestAsk, decimal MinQuantity)
        {
            var Stace = false;
            var AskStace = new ExchangeApiData();
            var BidStace = new ExchangeApiData();
            switch (lowestAsk.Name.ToLower())
            {
                case "binance":
                    AskStace = binance.PlaceOrderAsk(lowestAsk, MinQuantity);
                    break;
                case "bittrex":
                    AskStace = bittrex.PlaceOrderAsk(lowestAsk, MinQuantity);
                    break;
                case "hitbtc":
                    AskStace = hitbtc.PlaceOrderAsk(lowestAsk, MinQuantity);
                    break;
                case "huobi":
                    AskStace = huobi.PlaceOrderAsk(lowestAsk, MinQuantity);
                    break;
                case "poloniex":
                    AskStace = Poloniex.PlaceOrderAsk(lowestAsk, MinQuantity);
                    break;
                case "okex":
                    AskStace = OKEx.PlaceOrderAsk(lowestAsk, MinQuantity);
                    break;
            }
            switch (highestBid.Name.ToLower())
            {
                case "binance":
                    BidStace = binance.PlaceOrderBid(highestBid, MinQuantity);
                    break;
                case "bittrex":
                    BidStace = bittrex.PlaceOrderBid(highestBid, MinQuantity);
                    break;
                case "hitbtc":
                    BidStace = hitbtc.PlaceOrderBid(highestBid, MinQuantity);
                    break;
                case "huobi":
                    BidStace = huobi.PlaceOrderBid(highestBid, MinQuantity);
                    break;
                case "poloniex":
                    BidStace = Poloniex.PlaceOrderBid(highestBid, MinQuantity);
                    break;
                case "okex":
                    BidStace = OKEx.PlaceOrderBid(highestBid, MinQuantity);
                    break;
            }
            if (AskStace.Stace && BidStace.Stace)
            {
                Stace = true;
            }
            AskStace.Name = lowestAsk.Name;
            BidStace.Name = highestBid.Name;
            return new StockExchangeApiData { AskStace = AskStace, BidStace = BidStace, Stace = Stace };
        }
    }
    public class ApiResult<T>
    {
        private bool statusField = true;

        private T dataField;

        private string messageField;

        public bool Status { get { return statusField; } private set { } }

        public T Data { get { return dataField; } set { dataField = value; } }

        public string Message { get { return messageField; } set { statusField = false; messageField = value; } }
    }
}
