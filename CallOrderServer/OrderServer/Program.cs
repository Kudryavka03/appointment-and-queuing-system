using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OrderServer;

public class Program
{
    public static string uuid = "-1";
    public static string currentId = "0";
    public static Hashtable history = new Hashtable();
    public static Hashtable uuid2id = new Hashtable();
    public static Hashtable id2uuid = new Hashtable();
    private static readonly object _logMessageLock = new object();
    public static List<string> logs = new List<string>();
    public static string StartTimestamp;

    public static void Main(string[] args)
    {
        StartTimestamp = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
        Log($"系统启动：{StartTimestamp}");
        DataClass.initData(5);

        Thread t = new Thread(DataClass.Listener);
        t.Start();
        Thread rp = new Thread(DataClass.ReportEx);
        rp.Start();
        Thread ats = new Thread(DataClass.AutoSaveLog);
        ats.Start();
        Thread ass = new Thread(OrderStateStore.AutoSaveState);
        ass.Start();

        CreateHostBuilder(args).Build().Run();
    }

    public static void Log(string log, [CallerMemberName] string callerName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        lock (_logMessageLock)
        {
            var msg = $"[{DateTime.Now}] [{callerName}] {log}";
            logs.Add(msg);
            Console.WriteLine(msg);
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureLogging((hostContext, logging) =>
            {
                logging.ClearProviders();
                logging.AddSimpleConsole(options =>
                {
                    options.SingleLine = true;
                    options.TimestampFormat = "HH:mm:ss ";
                });
                logging.SetMinimumLevel(LogLevel.Warning);
                logging.AddFilter("Microsoft.AspNetCore.Routing", LogLevel.Error);
                logging.AddFilter("Microsoft.AspNetCore.Mvc.Infrastructure", LogLevel.Error);
                logging.AddFilter("Microsoft.AspNetCore.Mvc", LogLevel.Error);
            })
            .ConfigureWebHostDefaults(delegate(IWebHostBuilder webBuilder)
            {
                string urls = "http://0.0.0.0:888/";
                webBuilder.UseUrls(urls);
                Program.Log($"UseUrls: {urls}");
                webBuilder.UseStartup<Startup>();
                Program.Log("叫号后台启动成功。状态文件：" + OrderStateStore.StateFilePath);
            });
    }
}
