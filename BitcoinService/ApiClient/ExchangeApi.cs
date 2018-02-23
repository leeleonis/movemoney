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

        private bool _start = false;

        public List<ExchangeData> ExchangeList;

        public ExchangeApi()
        {
            Api_Init();

            ExchangeList = new List<ExchangeData>()
            {
                new ExchangeData("Binance"),
                new ExchangeData("Bittrex")
            };
        }
        private void Api_Init()
        {
            binance = new BinanceApi.Binance();
            bittrex = new BittrexApi.Bittrex();
        }

        public void Start_Up()
        {
            this._start = true;

            Task.Factory.StartNew(() => Check_Ticker("binance"));
            Task.Factory.StartNew(() => Check_Ticker("bittrex"));
        }

        private void Check_Ticker(string ExchangeName)
        {
            ExchangeData Data = ExchangeList.First(data => data.Name.Equals(ExchangeName));
            Data.Status = EnumData.ExchangeStatus.執行中;

            try
            {
                while (this._start)
                {
                    switch (ExchangeName.ToLower())
                    {
                        case "binance":
                            //binance.Socket_Ticker("ETHBTC");
                            break;
                        case "bittrex":
                            Bittrex_Ticker(Data);
                            break;
                    }                    

                    Thread.Sleep(1000);
                }

                Data.Status = EnumData.ExchangeStatus.停止;
            }
            catch (Exception e)
            {
                Data.Status = EnumData.ExchangeStatus.異常;
                Data.ErrorMsg = e.InnerException == null ? e.Message : e.InnerException.Message;
            }
        }

        public void End_Up()
        {
            this._start = false;
        }
        
        private void Binance_Ticker(ExchangeData Data)
        {
            var Result = binance.Ticker("ETHBTC");
            if (Result.Status)
            {
                Data.Ask = Result.Data.AskPrice;
                Data.Bid = Result.Data.BidPrice;
                Data.UpdateTime = DateTime.UtcNow;
            }
            else
            {
                throw new Exception(Result.Message);
            }
        }
        
        private void Bittrex_Ticker(ExchangeData Data)
        {
            var Result = bittrex.Ticker("BTC-ETH");
            if (Result.Status)
            {
                Data.Ask = Convert.ToDouble(Result.Data.Ask);
                Data.Bid = Convert.ToDouble(Result.Data.Bid);
                Data.UpdateTime = DateTime.UtcNow;
            }
            else
            {
                throw new Exception(Result.Message);
            }
        }
    }

    public class ApiResult<T>
    {
        private bool statusField = true;

        private T dataField;

        private string messageField;

        public bool Status { get { return this.statusField; } private set { } }

        public T Data { get { return this.dataField; } set { this.dataField = value; } }

        public string Message { get { return messageField; } set { statusField = false; messageField = value; } }
    }
}
