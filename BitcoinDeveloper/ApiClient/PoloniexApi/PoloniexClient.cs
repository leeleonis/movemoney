using BitcoinService.ApiClient.PoloniexApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PoloniexApi.Objects;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocket4Net;

namespace BitcoinDeveloper.ApiClient.PoloniexApi
{
    public class PoloniexApiResult<T>
    {
        public bool Status { get; set; }

        public T Data { get; set; }

        public string Message { get; set; }

        public PoloniexApiResult()
        {
            Status = true;
        }
    }
    public class ErrorMessage
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
    class PoloniexClient : IDisposable
    {
        private Dictionary<int, WebSocket> WebSocketList;
        private string SocketUri = "wss://api2.poloniex.com";
          public event EventHandler<ErrorMessage> OnError;
        public event EventHandler OnClose;

        private string _apiKey { get; set; }
        private string _secretKey { get; set; }
        public PoloniexClient(string key, string secret)
        {
            _apiKey = key;
            _secretKey = secret;
            WebSocketList = new Dictionary<int, WebSocket>() { };
        }

        public PoloniexClient() : this("", "") { }
        public PoloniexApiResult<int> SubscribeTicker(string symbol, Action<SocketData<TickerData>> onUpdate)
        {
            SocketResult SocketResult = null;
            PoloniexApiResult<int> ApiResult = new PoloniexApiResult<int>();

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
    }
}
