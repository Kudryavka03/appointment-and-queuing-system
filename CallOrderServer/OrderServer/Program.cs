using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Reflection.Metadata;

namespace OrderServer;

public class Program
{
    // 信宜市生源地助学贷款叫号系统 服务后端
	public static string uuid = "-1";
	public static string currentId = "0";
    public static Hashtable history = new Hashtable();
    public static Hashtable uuid2id = new Hashtable();
    public static Hashtable id2uuid = new Hashtable();
    private static object _logMessageLock = new object ();
    public static List<string> logs = new List<string>();
    public static string  StartTimestamp;


    public static void Main(string[] args)
	{
        StartTimestamp = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
        Log($"系统启动：{StartTimestamp}");
        DataClass.initData(5);
		Thread t = new Thread(DataClass.Listener);
		t.Start();
		CreateHostBuilder(args).Build().Run();
	}

    public static void Log(string log,[CallerMemberName] string callerName = "",
    [CallerFilePath] string sourceFilePath = "",
    [CallerLineNumber] int lineNumber = 0)  // 日志记录系统
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
            // 清除默认的日志提供程序
            logging.ClearProviders();

            // 添加控制台日志（只记录Warning及以上级别）
            logging.AddSimpleConsole(options =>
            {
                options.SingleLine = true;  // 单行输出更简洁
                options.TimestampFormat = "HH:mm:ss ";  // 时间格式
            });

            // 设置全局日志级别
            logging.SetMinimumLevel(LogLevel.Warning);

            // 针对特定命名空间设置更严格的日志级别
            logging.AddFilter("Microsoft.AspNetCore.Routing", LogLevel.Error);
            logging.AddFilter("Microsoft.AspNetCore.Mvc.Infrastructure", LogLevel.Error);
            logging.AddFilter("Microsoft.AspNetCore.Mvc", LogLevel.Error);
        }).ConfigureWebHostDefaults(delegate(IWebHostBuilder webBuilder)
		{
            string urls = "http://0.0.0.0:888/";
            webBuilder.UseUrls(urls);
            Program.Log($"UseUrls: {urls}");
			webBuilder.UseStartup<Startup>();
            Program.Log("叫号后端启动成功。");
        });
		
	}
}
