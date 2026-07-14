using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GenApplyFormServer
{
    public class Program
    {
        public static string defaultAvgAi = "        无         ";
        public static string s1 = "      无      ";
        public static string s2 = "                     无                  ";
        public static string s3 = "          无          ";
        public static string s4 = "           无          ";
        public static string s5 = "                             无                                    ";
        public static string s6 = "                              无                                    ";
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        public static void Log(string log, [CallerMemberName] string callerName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int lineNumber = 0)  // 日志记录系统
        {
                var msg = $"[{DateTime.Now}] [{callerName}] {log}";
            Console.WriteLine(msg);
        }

        public static bool TestCardId(string cardId)
        {
            string pattern = @"^\d{17}(?:\d|X)$";
            string birth = cardId.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            // 加权数组,用于验证最后一位的校验数字
            int[] arr_weight = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            // 最后一位计算出来的校验数组，如果不等于这些数字将不正确
            string[] id_last = { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };
            int sum = 0;
            //通过循环前16位计算出最后一位的数字
            for (int i = 0; i < 17; i++)
            {
                sum += arr_weight[i] * int.Parse(cardId[i].ToString());
            }
            // 实际校验位的值
            int result = sum % 11;
            // 首先18位格式检查
            if (Regex.IsMatch(cardId, pattern))
            {   // 出生日期检查
                if (DateTime.TryParse(birth, out time))
                {
                    // 校验位检查
                    if (id_last[result] == cardId[17].ToString())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
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
            }).ConfigureWebHostDefaults(delegate (IWebHostBuilder webBuilder)
            {
                string urls = "http://0.0.0.0:80/";
                webBuilder.UseUrls(urls);
                Program.Log($"UseUrls: {urls}");
                webBuilder.UseStartup<Startup>();
                Program.Log("叫号后端启动成功。");
            });

        }
    }
}
