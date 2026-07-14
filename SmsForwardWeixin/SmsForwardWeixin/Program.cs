using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmsForwardWeixin.CRUD;
using SmsForwardWeixin.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace SmsForwardWeixin
{
    public class Program
    {
        public static string MessageData = "";
        private static DBOperator _dbOperator;
        private static object _errorMessageLock ;
        public static SmsForwardWeixin.CallOrder.CallOrder callOrder = new SmsForwardWeixin.CallOrder.CallOrder();
        public static string baseUrl = "http://127.0.0.1";
        public static string resUrl = "http://127.0.0.1:8900";
        public static string imgPath = "ImageTest/";
        public static List<string> ErrMessage = new List<string>();  
        public static string expiredToken1 = "";    // 只有符合这两个Token才允许现场报道。防止复制二维码回去报道。
        public static string expiredToken2 = "";
        public static string stationToken = "xjb5HJqHwLzeJhpDE/KhIRPSVqsekg0yzYS7Di0PrXDTZfgqy85/6TDo8iCyt0Q7MWJ3JjvX0Y19cbj9ZMqiwYFvq1HnHdi/7gsoCj0nJuI=";
        public static GarupaStationRoom garupaStationRoom = null;
        public static MessageSender messageSender = new MessageSender();
        private static List<GarupaStationRoom> garupaStationRooms = new List<GarupaStationRoom>();
        private static List<GarupaStationRoom> garupaStationTempsRooms = new List<GarupaStationRoom>();
        private static readonly object _listLock = new object();

        public static string ycx20 = "";
        public static string ycx50 = "";
        public static string ycx100 = "";
        public static string ycx500 = "";
        public static string ycx1000 = "";
        public static string ycx2000 = "";
        public static string ycx5000 = "";
        public static string ycx10000 = "";
        public static string allEvent = "";
        public static string eventId = "292";
        public static HashSet<string> blockList = new HashSet<string>();
        public static DBOperator DbOperator // 我也不知道这么干安不安全，反正DBOperator那边写的一坨
        {
            get
            {
                if (_dbOperator == null)
                {
                    lock (typeof(Program))
                    {
                        if (_dbOperator == null)
                        {
                            _dbOperator = new DBOperator();
                        }
                    }
                }
                return _dbOperator;
            }
        }
        public static void AddRoomList(GarupaStationRoom garupaStationRoom) //  将房间加入监听列表，以确认房间是否已经发送上去
        {
            lock (_listLock)
            {
                garupaStationRooms.Add(garupaStationRoom);
            }
        }   // 允许有重复的房间。

        public static void DynamicCheckTempRoom(string room,bool isCheckTimeout = true)    // 检查第一个项目，是否超时。因为是按顺序加入的，因此后边的项目
        {                                                       // 不会比第一个项目要早。确认可以移除后，将List加入Temp
            // Console.WriteLine("Daily Check");
            if (room == null && !isCheckTimeout) { return; }
            if (room == "" && !isCheckTimeout) { return; }
            lock (_listLock)                                    // 加入完毕后清空List再复制回来，再清空Temp
            {
                if (garupaStationRooms.Count == 0) {  return; }  // 优化性能
                if (!isCheckTimeout)
                {
                    foreach(var a in garupaStationRooms)
                    {
                        if (a.number == room)
                        {
                            sendNormalMessageAsync($"车牌：{a.number} 上传成功",a.group_number);
                            garupaStationRooms.Remove(a);
                            return;
                        }
                    }
                    return;
                }
                else
                {
                    // Console.WriteLine("Daily Check3");
                    var obj = garupaStationRooms[0];
                    var first_time = obj.time;
                    var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    if (now - first_time > 6)    // timeout
                    {
                        var r = obj.number;
                        garupaStationRooms.Remove(obj);
                        sendNormalMessageAsync($"车牌：{obj.number} 上传超时，请检查是否为重复上传（即在其他群已经上传过一次），车站会屏蔽接下来10s的重复车牌号上传请求。可尝试重新上传，如果不是重复上传且频繁出现此提示则说明后端需要更新车站Token。", obj.group_number);
                        // SendPayload(车牌“xxxx”上传超时，如有需要请重新发起上传申请)
                    }
                }
            }
        }
        public static async void updateApiCache()
        {
            return;
            //eventId
            using var httpClient = new HttpClient();
            while (true)
            {
                try
                {
                    HttpResponseMessage responseAll = await httpClient.GetAsync($"http://bestdori.com/api/events/all.6.json");
                    string allEvent = await responseAll.Content.ReadAsStringAsync();
                    responseAll = await httpClient.GetAsync($"http://bestdori.com/api/tracker/data?server=3&event={eventId}&tier=20");
                    ycx20 = await responseAll.Content.ReadAsStringAsync();
                    responseAll = await httpClient.GetAsync($"http://bestdori.com/api/tracker/data?server=3&event={eventId}&tier=50");
                    ycx50 = await responseAll.Content.ReadAsStringAsync();
                    responseAll = await httpClient.GetAsync($"http://bestdori.com/api/tracker/data?server=3&event={eventId}&tier=100");
                    ycx100 = await responseAll.Content.ReadAsStringAsync();
                    responseAll = await httpClient.GetAsync($"http://bestdori.com/api/tracker/data?server=3&event={eventId}&tier=500");
                    ycx500 = await responseAll.Content.ReadAsStringAsync();
                    responseAll = await httpClient.GetAsync($"http://bestdori.com/api/tracker/data?server=3&event={eventId}&tier=1000");
                    ycx1000 = await responseAll.Content.ReadAsStringAsync();
                    responseAll = await httpClient.GetAsync($"http://bestdori.com/api/tracker/data?server=3&event={eventId}&tier=2000");
                    ycx2000 = await responseAll.Content.ReadAsStringAsync();
                    responseAll = await httpClient.GetAsync($"http://bestdori.com/api/tracker/data?server=3&event={eventId}&tier=5000");
                    ycx5000 = await responseAll.Content.ReadAsStringAsync();
                    responseAll = await httpClient.GetAsync($"http://bestdori.com/api/tracker/data?server=3&event={eventId}&tier=10000");
                    ycx10000 = await responseAll.Content.ReadAsStringAsync();
                    Console.WriteLine
                    ("API Cache Update.");
                }
                catch{
                    Console.WriteLine
                    ("API Cache Update Faild.");
                }
                Thread.Sleep(300000);
            }
        }
        public static async void updateApiCache(string tier)
        {
            return;
            eventId = tier;
            using var httpClient = new HttpClient();
                HttpResponseMessage responseAll = await httpClient.GetAsync($"http://bestdori.com/api/events/all.6.json");
                string allEvent = await responseAll.Content.ReadAsStringAsync();
                responseAll = await httpClient.GetAsync($"http://bestdori.com/api/tracker/data?server=3&event={eventId}&tier=20");
                ycx20 = await responseAll.Content.ReadAsStringAsync();
                responseAll = await httpClient.GetAsync($"http://bestdori.com/api/tracker/data?server=3&event={eventId}&tier=50");
                ycx50 = await responseAll.Content.ReadAsStringAsync();
                responseAll = await httpClient.GetAsync($"http://bestdori.com/api/tracker/data?server=3&event={eventId}&tier=100");
                ycx100 = await responseAll.Content.ReadAsStringAsync();
                responseAll = await httpClient.GetAsync($"http://bestdori.com/api/tracker/data?server=3&event={eventId}&tier=500");
                ycx500 = await responseAll.Content.ReadAsStringAsync();
                responseAll = await httpClient.GetAsync($"http://bestdori.com/api/tracker/data?server=3&event={eventId}&tier=1000");
                ycx1000 = await responseAll.Content.ReadAsStringAsync();
                responseAll = await httpClient.GetAsync($"http://bestdori.com/api/tracker/data?server=3&event={eventId}&tier=2000");
                ycx2000 = await responseAll.Content.ReadAsStringAsync();
                responseAll = await httpClient.GetAsync($"http://bestdori.com/api/tracker/data?server=3&event={eventId}&tier=5000");
                ycx5000 = await responseAll.Content.ReadAsStringAsync();
                responseAll = await httpClient.GetAsync($"http://bestdori.com/api/tracker/data?server=3&event={eventId}&tier=10000");
                ycx10000 = await responseAll.Content.ReadAsStringAsync();
        }
        public static async void sendNormalMessageAsync(string desc,string group)
        {
            //Console.WriteLine("desc:" + desc + " gp:" + group);
            using var httpClient = new HttpClient();
            // Console.WriteLine(desc);
            // 请求数据
            var jsonData = $"{{ \"group_id\": \"{group}\", \"message\": [ {{ \"type\": \"text\", \"data\": {{ \"text\": \"{desc}\" }} }} ] }}"; ;
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            try
            {
                // 发送 POST 请求
                HttpResponseMessage response = await httpClient.PostAsync("http://10.80.0.13:3000/send_group_msg", content);
                string responseBody = await response.Content.ReadAsStringAsync();
                // Console.WriteLine($"响应内容: {responseBody}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send Group Message Faild！: {ex.Message}");
            }
        }
        public static void AddErrMessage(string log)
        {
            lock (_errorMessageLock)
            {
                var stacktrace = new StackTrace();
                var stackName = stacktrace.GetFrame(1).GetMethod();
                var msg = $"[{DateTime.Now}] [{stackName}] {log}";
                ErrMessage.Add(msg);
                Console.WriteLine(msg);
            }
        }

        public static void ClearErrMessage()
        {
            lock (_errorMessageLock)
            {
                ErrMessage.Clear();
                var stacktrace = new StackTrace();
                var stackName = stacktrace.GetFrame(1).GetMethod();
                var msg = $"[{DateTime.Now}] [{stackName}] 当前系统错误日志已被清空";
                ErrMessage.Add(msg);
                Console.WriteLine(msg);
            }
        }
        public static void DynamicCheckTempRoomLoop()
        {
            while (true)
            {
                try
                {
                    DynamicCheckTempRoom("0", true);
                }
                catch
                {

                }
                Thread.Sleep(1000);
            }
        }
        public static void Main(string[] args)
        {

            // string a = DbOperator.AddApointmentFromWxid("20250716", "wxid_20250705110708", "Weixin");
            // var path = Utils.Utils.GenQrCode("MTc1MTQ0MTkyMjY2OQ==");
            // Thread listener = new Thread(callOrder.Listener);
            // listener.Start();
            try
            {
                JObject jObject = JObject.Parse(File.ReadAllText("conf.json"));
                stationToken = jObject["station_token"].ToString();
            }
            catch (Exception)
            {
                string SettingText = JsonConvert.SerializeObject(new SettingClass
                {
                    station_token = "xjb5HJqHwLzeJhpDE/KhIRPSVqsekg0yzYS7Di0PrXDTZfgqy85/6TDo8iCyt0Q7MWJ3JjvX0Y19cbj9ZMqiwYFvq1HnHdi/7gsoCj0nJuI=",
                });
                File.WriteAllText("conf.json", SettingText);
                stationToken = "xjb5HJqHwLzeJhpDE/KhIRPSVqsekg0yzYS7Di0PrXDTZfgqy85/6TDo8iCyt0Q7MWJ3JjvX0Y19cbj9ZMqiwYFvq1HnHdi/7gsoCj0nJuI=";

            }
            var bl = File.ReadAllText("blocklist.txt");
            var blist = bl.Split('|');
            foreach(var b in blist)
            {
                if (b != "")
                blockList.Add(b);
            }
            
            Thread GarupaStationClient = new Thread(Utils.Utils.StartGarupaStationSender);
            Thread dynamicCheckTempRoom = new Thread(DynamicCheckTempRoomLoop);
            Thread updateApiCacheT = new Thread(updateApiCache);
            
            GarupaStationClient.Start();
            dynamicCheckTempRoom.Start();
            updateApiCacheT.Start();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls("http://*:80/");
                webBuilder.UseStartup<Startup>();
            });
    }
    public class ConfigClass
    {
        public string baseUrl;
        public string resUrl;
        public string imgPath;
        public string port;
    }
    public class SettingClass
    {
        public string station_token;
    }

    public class GarupaStationRoom
    {
        public string number;
        public string roomType;
        public long time;
        public string group_number;
        public GarupaStationRoom(string number, string roomType) {
            this.number = number;
            this.roomType = roomType;
            this.time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();    //11023066
        }
    }
    public class MessageSender
    {
        // 定义事件
        public event Action<GarupaStationRoom> MessageSent;

        // 触发事件的方法
        protected virtual void OnMessageSent(GarupaStationRoom g)
        {
            MessageSent?.Invoke(g);
        }

        public void SendMessage(GarupaStationRoom g)
        {
            // 发送消息的业务逻辑
            Console.WriteLine($"Receive Room Request: {g.number}");

            // 触发事件
            OnMessageSent(g);
        }
    }
}
