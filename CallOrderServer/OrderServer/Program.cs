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
    // ��������Դ����ѧ����к�ϵͳ ������
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
        Log($"ϵͳ������{StartTimestamp}");
        DataClass.initData(5);
		Thread t = new Thread(DataClass.Listener);
		t.Start();
		CreateHostBuilder(args).Build().Run();
	}

    public static void Log(string log,[CallerMemberName] string callerName = "",
    [CallerFilePath] string sourceFilePath = "",
    [CallerLineNumber] int lineNumber = 0)  // ��־��¼ϵͳ
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
            // ���Ĭ�ϵ���־�ṩ����
            logging.ClearProviders();

            // ��ӿ���̨��־��ֻ��¼Warning�����ϼ���
            logging.AddSimpleConsole(options =>
            {
                options.SingleLine = true;  // ������������
                options.TimestampFormat = "HH:mm:ss ";  // ʱ���ʽ
            });

            // ����ȫ����־����
            logging.SetMinimumLevel(LogLevel.Warning);

            // ����ض������ռ����ø��ϸ����־����
            logging.AddFilter("Microsoft.AspNetCore.Routing", LogLevel.Error);
            logging.AddFilter("Microsoft.AspNetCore.Mvc.Infrastructure", LogLevel.Error);
            logging.AddFilter("Microsoft.AspNetCore.Mvc", LogLevel.Error);
        }).ConfigureWebHostDefaults(delegate(IWebHostBuilder webBuilder)
		{
            string urls = "http://0.0.0.0:888/";
            webBuilder.UseUrls(urls);
            Program.Log($"UseUrls: {urls}");
			webBuilder.UseStartup<Startup>();
            Program.Log("�кź�������ɹ���");
        });
		
	}
}
