using System.Threading;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Http;
using Microsoft.Extensions.Primitives;

namespace SmsForwardWeixin.CallOrder
{
    public class CallOrder
    {
        HttpClient callOrderHttpClient = new HttpClient();
        public CallOrder() { }
        private List<string> tempQueues = new List<string>(); // 这里存tokens，虽然没什么用但是估计到时候可能会有用。
        private object lockTempQueue = new object();
        public async void Listener()
        {
            int i = 0;  // i 为当前叫号人数
            while (true)
            {
                int nowtime = Convert.ToInt32(DateTime.Now.ToString("HHmmss"));
                if (nowtime < 10)
                {
                    if (tempQueues.Count != 0)
                        tempQueues.Clear(); // 每天0时清除队列数据，防止内存爆满
                }
                try
                {
                    if (tempQueues.Count != 0 && tempQueues.Count != i)
                    {
                        i++;
                        var token = tempQueues[i]; // 其实没什么用，叫个号而已
                        // 这里应该跟叫号机联动了。
                        string url = "http://127.0.0.1:666/GetStatus/GenNewUuid";
                        HttpResponseMessage response = await callOrderHttpClient.GetAsync(url);
                        string responseBody = await response.Content.ReadAsStringAsync();
                        try
                        {
                            int num;
                            var isVaildNumber = int.TryParse(responseBody, out num);
                            if (isVaildNumber)
                            {
                                Program.DbOperator.AddOrder(token, responseBody);
                            }
                            else Console.WriteLine($"[{DateTime.Now}] 叫号系统返回了错误的结果，预约系统无法解析此结果：{responseBody}");
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[{DateTime.Now}] 与叫号系统通讯失败:{e.Message}。请检查叫号服务器配置是否正确。");
                }
                Thread.Sleep(50);
            }
        }

        public bool AddTokenOrder(string token)
        {
            lock (lockTempQueue)
            {
                tempQueues.Add(token);
            }
            return true;
        }
    }
}
