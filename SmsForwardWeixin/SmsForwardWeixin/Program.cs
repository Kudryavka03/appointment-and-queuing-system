using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmsForwardWeixin.CRUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmsForwardWeixin
{
    public class Program
    {
        public static string MessageData = "";
        private static DBOperator _dbOperator;

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

        public static void Main(string[] args)
        {
            
            string a = DbOperator.AddApointmentFromWxid("20250716", "wxid_555666", "Weixin");
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
}
