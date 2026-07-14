using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OrderClientV2
{
    public class OrderRealtimeClient : IDisposable
    {
        private ClientWebSocket webSocket;
        private CancellationTokenSource cancellationTokenSource;
        private readonly object sendLock = new object();

        public event Action<RealtimeOrderSnapshot> SnapshotReceived;
        public event Action<string, string> CommandResultReceived;
        public event Action<string> ConnectionStatusChanged;

        public bool IsConnected => webSocket != null && webSocket.State == WebSocketState.Open;

        public async Task ConnectAsync(string serverUrl)
        {
            await DisconnectAsync();
            cancellationTokenSource = new CancellationTokenSource();
            webSocket = new ClientWebSocket();
            Uri uri = BuildWebSocketUri(serverUrl);
            ConnectionStatusChanged?.Invoke("正在连接调度中心...");
            await webSocket.ConnectAsync(uri, cancellationTokenSource.Token);
            ConnectionStatusChanged?.Invoke("已通过WebSocket连接调度中心");
            _ = Task.Run(ReceiveLoopAsync);
        }

        public async Task DisconnectAsync()
        {
            try
            {
                cancellationTokenSource?.Cancel();
                if (webSocket != null && webSocket.State == WebSocketState.Open)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "close", CancellationToken.None);
                }
            }
            catch
            {
            }
            finally
            {
                webSocket?.Dispose();
                webSocket = null;
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;
            }
        }

        public async Task SendCommandAsync(object command)
        {
            if (!IsConnected) throw new InvalidOperationException("WebSocket未连接");
            string json = JsonConvert.SerializeObject(command);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            lock (sendLock)
            {
                webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationTokenSource.Token).Wait();
            }
            await Task.CompletedTask;
        }

        private async Task ReceiveLoopAsync()
        {
            byte[] buffer = new byte[8192];
            try
            {
                while (webSocket != null && webSocket.State == WebSocketState.Open && !cancellationTokenSource.IsCancellationRequested)
                {
                    var builder = new StringBuilder();
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationTokenSource.Token);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            ConnectionStatusChanged?.Invoke("调度中心已断开连接");
                            return;
                        }
                        builder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    }
                    while (!result.EndOfMessage);

                    HandleMessage(builder.ToString());
                }
            }
            catch (Exception ex)
            {
                if (!cancellationTokenSource.IsCancellationRequested)
                {
                    ConnectionStatusChanged?.Invoke("WebSocket连接失败：" + ex.Message);
                }
            }
        }

        private void HandleMessage(string payload)
        {
            JObject obj = JObject.Parse(payload);
            string messageType = obj["MessageType"]?.ToString() ?? obj["messageType"]?.ToString();
            if (messageType == "snapshot")
            {
                SnapshotReceived?.Invoke(obj.ToObject<RealtimeOrderSnapshot>());
            }
            else if (messageType == "commandResult")
            {
                string action = obj["Action"]?.ToString() ?? obj["action"]?.ToString();
                string result = obj["Result"]?.ToString() ?? obj["result"]?.ToString();
                CommandResultReceived?.Invoke(action, result);
            }
            else if (messageType == "error")
            {
                string error = obj["Error"]?.ToString() ?? obj["error"]?.ToString();
                CommandResultReceived?.Invoke("error", error);
            }
        }

        private static Uri BuildWebSocketUri(string serverUrl)
        {
            string url = serverUrl.Trim();
            if (url.StartsWith("ws://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("wss://", StringComparison.OrdinalIgnoreCase))
            {
                return new Uri(url.EndsWith("/ws", StringComparison.OrdinalIgnoreCase) ? url : url.TrimEnd('/') + "/ws");
            }

            Uri httpUri = new Uri(url);
            string scheme = httpUri.Scheme == "https" ? "wss" : "ws";
            var builder = new UriBuilder(httpUri)
            {
                Scheme = scheme,
                Path = httpUri.AbsolutePath.TrimEnd('/') + "/ws"
            };
            return builder.Uri;
        }

        public void Dispose()
        {
            DisconnectAsync().Wait(1000);
        }
    }

    public class RealtimeOrderSnapshot
    {
        public string MessageType { get; set; }
        public string ServerTime { get; set; }
        public string LastestOrder { get; set; }
        public int LastestOrderNum { get; set; }
        public int Uuid { get; set; }
        public int WaitingCount { get; set; }
        public int ReportSpeed { get; set; }
        public int TotalWindowNum { get; set; }
        public string[] TypeNames { get; set; }
        public RealtimeWindowSnapshot[] Windows { get; set; }
        public string[] Logs { get; set; }
    }

    public class RealtimeWindowSnapshot
    {
        public int Window { get; set; }
        public string Status { get; set; }
        public int CurrentNum { get; set; }
        public string DisplayText { get; set; }
        public string TypeText { get; set; }
        public string TypeMask { get; set; }
        public bool IsBusy { get; set; }
        public bool IsPaused { get; set; }
    }
}
