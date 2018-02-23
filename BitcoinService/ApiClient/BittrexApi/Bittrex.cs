using Bittrex.Net;
using Bittrex.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinService.ApiClient.BittrexApi
{
    class Bittrex
    {
        private string ApiKey = "";
        private string ApiSecret = "";

        private BittrexClient ApiClient;
        
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
    }
}
