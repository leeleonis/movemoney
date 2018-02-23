using Binance.Net;
using Binance.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinService.ApiClient.BinanceApi
{
    class Binance
    {
        private string ApiKey = "";
        private string ApiSecret = "";

        BinanceClient ApiClient;

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
    }
}
