using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SmsForwardWeixin.BandoriStation
{
    public class GarupaStation
    {
        private readonly Uri _serverUri = new("wss://api.bandoristation.com/");
        private ClientWebSocket _webSocket = new();
        private readonly CancellationTokenSource _cts = new();
        private string _token;
        private readonly string _clientName = "BandoriStation";
        private readonly TimeSpan _heartbeatInterval = TimeSpan.FromSeconds(15);
        private readonly TimeSpan _reconnectDelay = TimeSpan.FromSeconds(5);

        private bool _manualClose = false;
        private MessageSender _messageSender;
        public GarupaStation(string token, MessageSender messageSender)
        {
            _token = token;
            _messageSender = messageSender;
            // 订阅事件
            _messageSender.MessageSent += SendRoomNumberAsync;
        }
        public async Task StartAsync()
        {
            _manualClose = false;
            await ConnectAsync();
            _ = Task.Run(HeartbeatLoopAsync);
        }

        private async Task ConnectAsync()
        {
            try
            {
                _webSocket = new ClientWebSocket();
                Console.WriteLine($"Connecting to {_serverUri} ...");
                await _webSocket.ConnectAsync(_serverUri, _cts.Token);
                Console.WriteLine("Connected!");

                // 启动接收循环
                _ = Task.Run(ReceiveLoopAsync);

                // 稍等服务端下发初始化信息
                await Task.Delay(1000);

                // 注册客户端
                await SendClientRegistrationAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Connect Error] {ex.Message}");
                await AttemptReconnectAsync();
            }
        }

        private async Task AttemptReconnectAsync()
        {
            if (_manualClose) return;
            Console.WriteLine($"Reconnecting in {_reconnectDelay.TotalSeconds} seconds...");
            await Task.Delay(_reconnectDelay);
            await ConnectAsync();
        }

        private async Task SendClientRegistrationAsync()
        {
            var setupPayload = new object[]
            {
            new { action = "setClient", data = new { client = _clientName, send_room_number = true } },
            new { action = "getRoomNumberList", data = (object?)null },
            new { action = "setAccessPermission", data = new { token = _token } },
            new { action = "checkUnreadChat", data = (object?)null }
            };

            string json = System.Text.Json.JsonSerializer.Serialize(setupPayload);
            await SendAsync(json);
            Console.WriteLine("Sent client setup payload.");
        }

        private async Task HeartbeatLoopAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    if (_webSocket?.State == WebSocketState.Open)
                    {
                        var heartbeat = new
                        {
                            action = "heartbeat",
                            data = new { client = _clientName }
                        };

                        string json = System.Text.Json.JsonSerializer.Serialize(heartbeat);
                        await SendAsync(json);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Heartbeat Error] {ex.Message}");
                }

                await Task.Delay(_heartbeatInterval);
            }
        }

        private async Task ReceiveLoopAsync()
        {
            var buffer = new byte[4096];
            try
            {
                while (_webSocket.State == WebSocketState.Open)
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("Server closed connection.");
                        break;
                    }

                    string json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    try
                    {
                        JObject j = JObject.Parse(json);
                        Program.DynamicCheckTempRoom((string)j["response"][0]["number"],false);
                    }
                    catch
                    {

                    }
                }
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"[WebSocket Exception] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Receive Error] {ex.Message}");
            }

            if (!_manualClose)
            {
                await AttemptReconnectAsync();
            }
        }

        public void  SendRoomNumberAsync(GarupaStationRoom g)
        {
            string roomNumber = g.number.ToString();
            if (roomNumber == "token") { 
                _token = g.roomType.ToString();
                Program.stationToken = _token;
                string SettingText = JsonConvert.SerializeObject(new SettingClass
                {
                    station_token = _token,
                });
                File.WriteAllText("conf.json", SettingText);
                Console.WriteLine("Token Updated:" + _token);
                SendClientRegistrationAsync();
                return;
            }
            string description = g.roomType;
            string type = "other";
            var payload = new
            {
                action = "sendRoomNumber",
                data = new
                {
                    room_number = roomNumber,
                    description,
                    type
                }
            };

            string json = System.Text.Json.JsonSerializer.Serialize(payload);
            SendAsync(json);
            Console.WriteLine($"Uploaded room number {roomNumber}.");
        }

        private async Task SendAsync(string message)
        {
            if (_webSocket == null || _webSocket.State != WebSocketState.Open)
            {
                Console.WriteLine("[Send Warning] WebSocket not connected, message skipped.");
                return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes),
                                       WebSocketMessageType.Text,
                                       true,
                                       _cts.Token);
        }

        public async Task StopAsync()
        {
            _manualClose = true;
            _cts.Cancel();

            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
            }

            _webSocket.Dispose();
            Console.WriteLine("Connection closed manually.");
        }
    }
}
