using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OrderServer;

public static class OrderWebSocketHub
{
    private static readonly ConcurrentDictionary<Guid, WebSocket> Clients = new ConcurrentDictionary<Guid, WebSocket>();
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task HandleAsync(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("WebSocket endpoint only.");
            return;
        }

        using WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
        Guid clientId = Guid.NewGuid();
        Clients[clientId] = socket;
        Program.Log($"WebSocket客户端已连接：{clientId}");
        await SendSnapshotAsync(socket);

        var buffer = new byte[8192];
        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var message = new StringBuilder();
                WebSocketReceiveResult result;
                do
                {
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", CancellationToken.None);
                        return;
                    }

                    message.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                }
                while (!result.EndOfMessage);

                await HandleCommandAsync(socket, message.ToString());
            }
        }
        catch (Exception e)
        {
            Program.Log("WebSocket连接异常：" + e.Message);
        }
        finally
        {
            Clients.TryRemove(clientId, out _);
            Program.Log($"WebSocket客户端已断开：{clientId}");
        }
    }

    public static void BroadcastSnapshot()
    {
        _ = BroadcastSnapshotAsync();
    }

    public static async Task BroadcastSnapshotAsync()
    {
        string json = JsonSerializer.Serialize(DataClass.CreateRealtimeSnapshot(), JsonOptions);
        byte[] data = Encoding.UTF8.GetBytes(json);

        foreach (var pair in Clients)
        {
            var socket = pair.Value;
            if (socket.State != WebSocketState.Open)
            {
                Clients.TryRemove(pair.Key, out _);
                continue;
            }

            try
            {
                await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch
            {
                Clients.TryRemove(pair.Key, out _);
            }
        }
    }

    private static async Task SendSnapshotAsync(WebSocket socket)
    {
        string json = JsonSerializer.Serialize(DataClass.CreateRealtimeSnapshot(), JsonOptions);
        byte[] data = Encoding.UTF8.GetBytes(json);
        await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private static async Task SendResponseAsync(WebSocket socket, object response)
    {
        string json = JsonSerializer.Serialize(response, JsonOptions);
        byte[] data = Encoding.UTF8.GetBytes(json);
        await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private static async Task HandleCommandAsync(WebSocket socket, string payload)
    {
        try
        {
            var command = JsonSerializer.Deserialize<OrderCommand>(payload, JsonOptions);
            if (command == null || string.IsNullOrWhiteSpace(command.Action))
            {
                await SendResponseAsync(socket, new { MessageType = "error", Error = "无效命令" });
                return;
            }

            string result = ExecuteCommand(command);
            await SendResponseAsync(socket, new { MessageType = "commandResult", Action = command.Action, Result = result });
            BroadcastSnapshot();
        }
        catch (Exception e)
        {
            await SendResponseAsync(socket, new { MessageType = "error", Error = e.Message });
        }
    }

    private static string ExecuteCommand(OrderCommand command)
    {
        switch (command.Action)
        {
            case "GenNewUuid":
                return DataClass.GenNewUuid(command.TypeMask ?? "11").ToString();
            case "CallStart":
                DataClass.CallStart(command.Window - 1);
                return "200";
            case "CallStop":
                DataClass.CallStop(command.Window - 1);
                return "200";
            case "CallFinished":
                DataClass.CallFinished(command.Window - 1);
                return "200";
            case "SetTypes":
                if (string.IsNullOrEmpty(command.TypeMask)) return "您至少需要为窗口设置一个业务。";
                DataClass.ChangeType(command.Window, command.TypeMask);
                return "Kasumi：Yeah!!!!!";
            case "SetWindowCount":
                DataClass.ResizeWindows(command.WindowCount);
                return "窗口数量已设置为 " + DataClass.totalWindowNum;
            case "GenHighLevel":
                return DataClass.GenHighLevel(command.OriginalOperator, command.TargetWindow, command.Message ?? "插队").ToString();
            case "GenHighLevelPass":
                return DataClass.GenHighLevelPass(command.Number).ToString();
            case "FixID":
                return DataClass.TryFixID(command.Number);
            case "SaveState":
                OrderStateStore.SaveNow();
                return "状态已保存";
            case "ReloadState":
                return DataClass.LoadStateOrDefault(DataClass.totalWindowNum) ? "已加载上一次归档状态" : "未找到可加载的历史状态";
            default:
                return "未知命令：" + command.Action;
        }
    }
}

public class OrderCommand
{
    public string Action { get; set; } = "";
    public int Window { get; set; }
    public int WindowCount { get; set; }
    public string TypeMask { get; set; } = "";
    public int OriginalOperator { get; set; }
    public int TargetWindow { get; set; }
    public string Message { get; set; } = "";
    public int Number { get; set; }
}
