using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmsForwardWeixin.CRUD;
using SmsForwardWeixin.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        public static void Main(string[] args)
        {

            string a = DbOperator.AddApointmentFromWxid("20250716", "wxid_20250705110708", "Weixin");
            // var path = Utils.Utils.GenQrCode("MTc1MTQ0MTkyMjY2OQ==");
            Thread listener = new Thread(callOrder.Listener);
            listener.Start();
            Thread AutoGenOtp = new Thread(Utils.Utils.GenOtp);
            AutoGenOtp.Start();
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
}
