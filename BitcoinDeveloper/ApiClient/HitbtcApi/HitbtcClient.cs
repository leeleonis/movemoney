using HitbtcApi.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using WebSocket4Net;

namespace BitcoinService.ApiClient.HitbtcApi
{
    class HitbtcClient : IDisposable
    {
        private Dictionary<int, WebSocket> WebSocketList;
        private string SocketUri = "wss://api.hitbtc.com/api/2/ws";
        private const string ApiUrl = "http://api.hitbtc.com";
        public event EventHandler<ErrorMessage> OnError;
        public event EventHandler OnClose;

        private string _apiKey { get; set; }
        private string _secretKey { get; set; }
        public HitbtcClient(string key, string secret)
        {
            _apiKey = key;
            _secretKey = secret;
            WebSocketList = new Dictionary<int, WebSocket>() { };
        }

        public HitbtcClient() : this("", "") { }

        public HitbtcApiResult<int> SubscribeTicker(string symbol, Action<SocketData<TickerData>> onUpdate)
        {
            SocketResult SocketResult = null;
            HitbtcApiResult<int> ApiResult = new HitbtcApiResult<int>();

            int streamId = WebSocketList.Any() ? WebSocketList.Keys.Max() + 1 : 1;
            WebSocket WebSocket = new WebSocket(this.SocketUri);
            ConfigSecurity(WebSocket);

            WebSocket.Opened += new EventHandler((sender, e) =>
            {
                Dictionary<string, object> send = new Dictionary<string, object>();
                {
                    send.Add("method", "subscribeTicker");
                    send.Add("params", new Dictionary<string, string>() { { "symbol", symbol } });
                    send.Add("id", streamId);
                };
                WebSocket.Send(JsonConvert.SerializeObject(send));
            });

            WebSocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>((sender, e) =>
            {
                if (SocketResult == null)
                {
                    SocketResult = JsonConvert.DeserializeObject<SocketResult>(e.Message);

                    if (!SocketResult.Status) OnError(sender, SocketResult.Error);
                }
                else
                {
                    SocketData<TickerData> SocketData = JsonConvert.DeserializeObject<JObject>(e.Message).ToObject<SocketData<TickerData>>();
                    if (SocketData.Data != null) onUpdate(SocketData);
                }
            });

            ErrorMessage error = null;
            WebSocket.Error += new EventHandler<ErrorEventArgs>((sender, e) =>
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

        public void Dispose() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="side">交易別</param>
        /// <param name="symbol">幣別</param>
        /// <param name="MinQuantity">數量</param>
        /// <param name="price">單價</param>
        /// <returns></returns>
        internal ExchangeApiData NewOrder(string side, string symbol, decimal quantity, decimal price)
        {
            var ExchangeApiData = new ExchangeApiData();
            RestClient client1 = new RestClient(ApiUrl);
            var request1 = new RestRequest("/api/2/order", Method.POST);
            client1.Authenticator = new HttpBasicAuthenticator(_apiKey, _secretKey);
            request1.RequestFormat = DataFormat.Json;
            request1.AddBody(new
            {
                symbol = symbol,
                side = side,
                quantity = quantity,
                price = price,
            });

            var Content = client1.Execute(request1).Content;
            JObject restoredObject = JObject.Parse(Content);
            var errordata = (JObject)restoredObject["error"];
            if (errordata != null)
            {
                ExchangeApiData.Msg = (string)errordata.Property("message").Value;

                ExchangeApiData.Stace = false;
            }
            else
            {
                ExchangeApiData.Stace = true;
            }
            return ExchangeApiData;
        }
    }
}
