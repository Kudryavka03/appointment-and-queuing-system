using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OrderScreen
{
    public class OrderRealtimeClient : IDisposable
    {
        private ClientWebSocket webSocket;
        private CancellationTokenSource cancellationTokenSource;

        public event Action<RealtimeOrderSnapshot> SnapshotReceived;
        public event Action<string> ConnectionStatusChanged;

        public bool IsConnected => webSocket != null && webSocket.State == WebSocketState.Open;

        public async Task ConnectAsync(string serverUrl)
        {
            await DisconnectAsync();
            cancellationTokenSource = new CancellationTokenSource();
            webSocket = new ClientWebSocket();
            Uri uri = BuildWebSocketUri(serverUrl);
            ConnectionStatusChanged?.Invoke("正在连接");
            await webSocket.ConnectAsync(uri, cancellationTokenSource.Token);
            ConnectionStatusChanged?.Invoke("WebSocket已连接");
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
                            ConnectionStatusChanged?.Invoke("连接已断开");
                            return;
                        }
                        builder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    }
                    while (!result.EndOfMessage);

                    JObject obj = JObject.Parse(builder.ToString());
                    string messageType = obj["MessageType"]?.ToString() ?? obj["messageType"]?.ToString();
                    if (messageType == "snapshot")
                    {
                        SnapshotReceived?.Invoke(obj.ToObject<RealtimeOrderSnapshot>());
                    }
                }
            }
            catch (Exception ex)
            {
                if (!cancellationTokenSource.IsCancellationRequested)
                {
                    ConnectionStatusChanged?.Invoke("连接失败：" + ex.Message);
                }
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
        public RealtimeWindowSnapshot[] Windows { get; set; }
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
