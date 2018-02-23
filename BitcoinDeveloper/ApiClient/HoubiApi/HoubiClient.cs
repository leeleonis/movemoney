using HuobiApi.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using WebSocket4Net;

namespace BitcoinService.ApiClient.HuobiApi
{
    class HuobiClientWebSocket : IDisposable
    {
        private Dictionary<int, WebSocket> WebSocketList;
        private string SocketUrl = "wss://api.huobi.pro/ws";
        public event EventHandler<ErrorMessage> OnError;
        public event EventHandler OnClose;

        public HuobiClientWebSocket(string key, string secret)
        {
            WebSocketList = new Dictionary<int, WebSocket>();
        }

        public HuobiClientWebSocket() : this("", "") { }

        public HuobiApiResult<int> SubscribeTicker(string symbol, Action<SocketData<TickerData>> onUpdate)
        {
            PingPong pong;
            SocketResult SocketResult = null;
            HuobiApiResult<int> ApiResult = new HuobiApiResult<int>();

            int streamId = WebSocketList.Any() ? WebSocketList.Keys.Max() + 1 : 1;
            WebSocket WebSocket = new WebSocket(this.SocketUrl, sslProtocols: SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls);
            ConfigSecurity(WebSocket);

            WebSocket.Opened += new EventHandler((sender, e) =>
            {
                Dictionary<string, object> send = new Dictionary<string, object>();
                {
                    send.Add("sub", string.Format("market.{0}.depth.step0", symbol));
                    send.Add("id", streamId);
                };
                WebSocket.Send(JsonConvert.SerializeObject(send));
            });

            WebSocket.DataReceived += new EventHandler<DataReceivedEventArgs>((sender, e) =>
            {
                string result = ConvertGZIPtoJSON(e.Data);

                using (pong = JsonConvert.DeserializeObject<PingPong>(result))
                {
                    if (!pong.pong.Equals(0))
                    {
                        WebSocket.Send(JsonConvert.SerializeObject(new Dictionary<string, long>() { { "pong", pong.pong } }));
                    }
                    else
                    {
                        if (SocketResult == null)
                        {
                            SocketResult = JsonConvert.DeserializeObject<SocketResult>(result);
                            if (SocketResult.Status.Equals("error"))
                            {
                                SocketResult.Error = JsonConvert.DeserializeObject<ErrorMessage>(result);
                                OnError(sender, SocketResult.Error);
                            }
                        }
                        else
                        {
                            try
                            {
                                var data = JsonConvert.DeserializeObject<JObject>(result);
                                SocketData<TickerData> SocketData = data.ToObject<SocketData<TickerData>>();
                                if (SocketData.Method != null)
                                {
                                    SocketData.Data = data.SelectToken("tick").ToObject<TickerData>();
                                    onUpdate(SocketData);
                                }
                            }
                            catch (Exception ex)
                            {
                                OnError(sender, new ErrorMessage() { Message = ex.Message });
                            }
                        }
                    }
                }
            });

            ErrorMessage error = null;
            WebSocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>((sender, e) =>
            {
                error = new ErrorMessage() { Message = e.Exception.Message };
                OnError(sender, error);
            });

            WebSocket.Closed += new EventHandler((sender, e) =>
            {
                WebSocket.Dispose();
                if (error == null) OnClose(sender, e);
            });

            WebSocket.Open();
            WebSocketList.Add(streamId, WebSocket);
            ApiResult.Data = streamId;

            return ApiResult;
        }

        public void UnsubscribeFromStream(int streamId)
        {
            if (WebSocketList.ContainsKey(streamId))
            {
                WebSocketList[streamId].Close();
            }
        }

        private void ConfigSecurity(WebSocket websocket)
        {
            var security = websocket.Security;

            if (security != null)
            {
                security.AllowUnstrustedCertificate = true;
                security.AllowNameMismatchCertificate = true;
            }
        }

        private string ConvertGZIPtoJSON(byte[] data)
        {
            using (MemoryStream zipData = new MemoryStream(data))
            using (GZipStream gZip = new GZipStream(zipData, CompressionMode.Decompress))
            using (MemoryStream output = new MemoryStream())
            {
                gZip.CopyTo(output);
                return Encoding.UTF8.GetString(output.ToArray());
            }
        }

        public void Dispose() { }
    }
    class HuobiClientREST : IDisposable
    {
        private RestClient RESTclient;
        private string RESTUrl = "https://api.huobi.pro";
        public string _apiKey { get; set; }
        public string _secretKey { get; set; }
        public string AccountId { get; set; }

        public HuobiClientREST(string key, string secret)
        {
            _apiKey = key;
            _secretKey = secret;
            GetAccountId();
        }
        /// <summary>
        /// 交易API
        /// </summary>
        /// <param name="type">buy-market：市價買，sell-market：市價賣，buy-limit：限價買，sell-limit：限價賣</param>
        /// <param name="symbol">btcusdt，bccbtc</param>
        /// <param name="amount">限價單表示下單數量，市價買單時表示買多少錢，市價賣單時表示賣多少幣</param>
        /// <param name="price">下單價格，市價單不傳該參數</param>
        public ExchangeApiData ordersPlace(string type, string symbol, decimal amount, decimal price)
        {
            var ExchangeApiData = new ExchangeApiData(); ; 
            var ParameterS = "{\"account-id\"" + ":\"" + AccountId + "\",";
            ParameterS += "\"amount\"" + ":\"" + amount.ToString("0.####") + "\",";
            ParameterS += "\"price\"" + ":\"" + price.ToString("0.#######") + "\",";
            ParameterS += "\"symbol\"" + ":\"" + symbol.ToLower() + "\",";
            ParameterS += "\"type\"" + ":\"" + type + "\"}";

            //ParameterS.Add(new Parameter { Name = "account-id", Value = AccountId });
            //ParameterS.Add(new Parameter { Name = "amount", Value = amount });
            //ParameterS.Add(new Parameter { Name = "price", Value = price });
            //ParameterS.Add(new Parameter { Name = "symbol", Value = symbol.ToLower() });
            //ParameterS.Add(new Parameter { Name = "type", Value = type });
            var GetReturn = RestClientPost("/v1/order/orders/place", ParameterS);
            JObject restoredObject = JObject.Parse(GetReturn);
            var status = (string)restoredObject.Property("status").Value;
            if (status == "ok")
            {
                ExchangeApiData.Stace = true;           
            }
            else
            {
                var errcode = (string)restoredObject.Property("err-code").Value;
                var errmsg = (string)restoredObject.Property("err-msg").Value;
                ExchangeApiData.Stace = false;
                ExchangeApiData.Msg = errmsg;
            }
            return ExchangeApiData;
        }
        public void GetAccountId()
        {
            var GetReturn = RestClientGet("/v1/account/accounts");
            JObject restoredObject = JObject.Parse(GetReturn.Content);
            var status =(string)restoredObject.Property("status").Value;
            if (status=="ok")
            {
                var datalist = restoredObject["data"].Children().ToList();
                foreach (var dataitem in datalist)
                {
                    JObject DataObj = JObject.Parse(dataitem.ToString());
                    AccountId = (string)DataObj.Property("id").Value;
                }
            }
            else
            {
                var errcode = (string)restoredObject.Property("err-code").Value;
                var errmsg = (string)restoredObject.Property("err-msg").Value;
            }
        }
          public string RestClientPost(string Url, string ParameterS)
        {
            RESTclient = new RestClient(RESTUrl);
            var request = new RestRequest(Url);
            var ParameterList = new List<Parameter>();
            

            ParameterList.Add(new Parameter { Name = "AccessKeyId", Value = _apiKey });
            ParameterList.Add(new Parameter { Name = "SignatureMethod", Value = "HmacSHA256" });
            ParameterList.Add(new Parameter { Name = "SignatureVersion", Value = "2" });
            ParameterList.Add(new Parameter { Name = "Timestamp", Value = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss") });
            foreach (var item in ParameterList.OrderBy(x => x.Name, new OrdinalComparer()))
            {
                request.AddParameter(item.Name, item.Value);
            }
            var RESTclientUrl = RESTclient.BuildUri(request);
            var st = RESTclientUrl.ToString();
            var nu = RestSharp.Extensions.MonoHttp.HttpUtility.UrlEncode(st);
            var enu = RestSharp.Extensions.MonoHttp.HttpUtility.UrlEncode(st, Encoding.UTF8);
            var u = new UriBuilder(st);
            string sign = CalculateSignature256("POST\n" + RESTclientUrl.Authority + "\n" + RESTclientUrl.AbsolutePath + "\n" + RESTclientUrl.Query.Replace("?", ""), _secretKey);
            request.AddParameter("Signature", sign);

            var result = "";
            var postsurl = RESTclient.BuildUri(request).ToString();
            byte[] bs = Encoding.UTF8.GetBytes(ParameterS);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(postsurl);
            req.Method = "POST";
            req.ContentType = "application/json";
            req.ContentLength = bs.Length;

            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
            }
            using (WebResponse responseW = req.GetResponse())
            {
                StreamReader sr = new StreamReader(responseW.GetResponseStream());
                 result = sr.ReadToEnd();
                sr.Close();
            }


            return result;
        }
        public IRestResponse RestClientGet(string Url, List<Parameter> ParameterS = null)
        {
            RESTclient = new RestClient(RESTUrl);
            var request = new RestRequest(Url, Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36");
            var ParameterList = new List<Parameter>();
            if (ParameterS != null)
            {
                ParameterList.AddRange(ParameterS);
            }

            ParameterList.Add(new Parameter { Name = "AccessKeyId", Value = _apiKey });
            ParameterList.Add(new Parameter { Name = "SignatureMethod", Value = "HmacSHA256" });
            ParameterList.Add(new Parameter { Name = "SignatureVersion", Value = "2" });
            ParameterList.Add(new Parameter { Name = "Timestamp", Value = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss") });
            foreach (var item in ParameterList.OrderBy(x => x.Name, new OrdinalComparer()))
            {
                request.AddParameter(item.Name, item.Value);
            }
            var RESTclientUrl = RESTclient.BuildUri(request);
            var st = RESTclientUrl.ToString();
            var nu = RestSharp.Extensions.MonoHttp.HttpUtility.UrlEncode(st);
            var enu = RestSharp.Extensions.MonoHttp.HttpUtility.UrlEncode(st, Encoding.UTF8);
            var u = new UriBuilder(st);
            string sign = CalculateSignature256(request.Method + "\n" + RESTclientUrl.Authority + "\n" + RESTclientUrl.AbsolutePath + "\n" + RESTclientUrl.Query.Replace("?", ""), _secretKey);
            request.AddParameter("Signature", sign);
            return RESTclient.Execute(request);
            //var content = response.Content;
        }
        private static string CalculateSignature256(string text, string secretKey)
        {
            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                return Convert.ToBase64String(hashmessage);
            }
        }
        public void Dispose() { }
    }
    /// <summary>
    /// ASCII值排序
    /// </summary>
    public class OrdinalComparer : System.Collections.Generic.IComparer<String>
    {
        public int Compare(String x, String y)
        {
            return string.CompareOrdinal(x, y);
        }
    }
}
