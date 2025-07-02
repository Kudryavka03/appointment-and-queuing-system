using System;
using System.Collections;
using System.IO;
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

    public static void Main(string[] args)
	{
		DataClass.initData(5);
		Thread t = new Thread(DataClass.Listener);
		t.Start();
		CreateHostBuilder(args).Build().Run();
	}

	public static async void OnNumChanged(object sender, FileSystemEventArgs e)
	{
		if (!(e.Name == "QueueResult-CurrentNum"))
		{
			return;
		}
		bool result = false;
		while (!result)
		{
			try
			{
				uuid = File.ReadAllText(e.FullPath);
				Console.WriteLine(uuid);
				result = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				result = false;
			}
		}
	}

	public static IHostBuilder CreateHostBuilder(string[] args)
	{
        Console.WriteLine("Service Start: CreateHostBuilder");
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
			

            webBuilder.UseUrls("http://0.0.0.0:888/");
			webBuilder.UseStartup<Startup>();
		});
		
	}
}
