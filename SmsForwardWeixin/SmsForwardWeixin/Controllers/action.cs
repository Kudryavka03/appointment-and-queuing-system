using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System;
using System.Xml.Linq;
using System.Xml;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Diagnostics;
using SmsForwardWeixin.BandoriStation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SmsForwardWeixin.Controllers
{
    public class action : Controller
    {
        [HttpPost("action")]
        public async Task<IActionResult> Index()
        {
            bool isVerify = false;
            using StreamReader reader = new StreamReader(Request.Body);
            string text = await reader.ReadToEndAsync();
            Console.WriteLine(text);
            XDocument doc = XDocument.Parse(text);
            string fromUsers = (string)doc.Root.Element("FromUserName");
            string contents = (string)doc.Root.Element("Content");
            string ctms = (string)doc.Root.Element("CreateTime");
            if (fromUsers == "o2-vT7ZaixzCHgZZ57lks_xgyqE4" || fromUsers == "o2-vT7elz_cP5daIFfwWx4UeMNpc")
            {
                isVerify = true;
            }
            else
            {
                return Ok("");
            }
            XDocument reply = new XDocument(
    new XElement("xml",
        new XElement("ToUserName", new XCData(fromUsers)),
        new XElement("FromUserName", new XCData("gh_29fa404ee99e")),
        new XElement("CreateTime", ctms),
        new XElement("MsgType", new XCData("text")),
        new XElement("Content", new XCData(Program.MessageData))

    )
);
            return Ok(reply.ToString());
        }
        [HttpGet("action")]
        public async Task<IActionResult> weixinVerify()
        {
            var returnData = Request.Query["echostr"].ToString();
            return Ok(returnData);
        }

        [HttpGet("SendRoom")]
        public async Task<IActionResult> SendRoom()
        {
            /*
            var room = Request.Query["room"].ToString();
            var desc = Request.Query["desc"].ToString();
            int num = Convert.ToInt32(room);
            Program.messageSender.SendMessage(new GarupaStationRoom(room, desc));
            
            return Ok($"ROOM:{room} DESC:{desc}");
            */
            return Ok("Current Env is not in dev env.");
        }

        [HttpGet("ycx")]
        public async Task<string> ycx()
        {
            var server = Request.Query["server"].ToString();
            var eId = Request.Query["event"].ToString();
            var tier = Request.Query["tier"].ToString();
            if (tier == "20" || tier == "50" || tier == "100" || tier == "500" || tier == "1000" || tier == "2000" || tier == "5000" || tier == "10000")
                return await CutOffUtils.CutOffCalcAsync(server, eId, tier);
            return await CutOffUtils.CutOffCalcAsync(server, eId, "false");
        }

        [HttpGet("ycxtest")]
        public async Task<string> ycxtest()
        {
            JToken cutoffDetails = null;
            using var httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync($"https://bestdori.com/api/tracker/data?server=03&event=293&tier=1000");
            string responseBody = await response.Content.ReadAsStringAsync();
            cutoffDetails = JObject.Parse(responseBody)["cutoffs"];
            var cutoffDetailCounts = cutoffDetails.Count();
            List<CutoffsItem> coi = new List<CutoffsItem>();
            coi.Clear();
            for (int i = 0; i < 100; i++)
            {
                CutOffObject cutOffObject = new CutOffObject();
                cutOffObject.time = ConvertToLong(cutoffDetails[i]["time"].ToString());
                cutOffObject.ep = ConvertToLong(cutoffDetails[i]["ep"].ToString());
                CutoffsItem cutoffsItem = new CutoffsItem();
                cutoffsItem.time = cutOffObject.time;
                cutoffsItem.ep = cutOffObject.ep;
                coi.Add(cutoffsItem);
            }
            ResultCutOffObject resultCutOffObject = new ResultCutOffObject();
            resultCutOffObject.result = true;
            resultCutOffObject.cutoffs = coi;
            string resultJson = JsonConvert.SerializeObject(resultCutOffObject, Newtonsoft.Json.Formatting.Indented);
            return resultJson;
        }
        public long ConvertToLong(string time)
        {
            return Convert.ToInt64(time);
        }
        [HttpGet("cutoffs")]
        public async Task<IActionResult> cutoffsCache()
        {
            var server = Request.Query["server"].ToString();
            var eId = Request.Query["event"].ToString();
            var tier = Request.Query["tier"].ToString();
            var responseBody = "";
            if (server == "3")      // 国服
            {
                if (eId == Program.eventId)     // 如果ID一样
                {
                    if (tier == "20" || tier == "50" || tier == "100" || tier == "500" || tier == "1000" || tier == "2000" || tier == "5000" || tier == "10000")
                    {
                        switch (tier)
                        {
                            case "50":
                                responseBody = Program.ycx50;
                                break;
                            case "20":
                                responseBody = Program.ycx20;
                                break;
                            case "100":
                                responseBody = Program.ycx100;
                                break;
                            case "500":
                                responseBody = Program.ycx500;
                                break;
                            case "1000":
                                responseBody = Program.ycx1000;
                                break;
                            case "2000":
                                responseBody = Program.ycx2000;
                                break;
                            case "5000":
                                responseBody = Program.ycx5000;
                                break;
                            case "10000":
                                responseBody = Program.ycx10000;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            if (responseBody == "")
            {
                return Redirect($"http://bestdori.com/api/tracker/data?server={server}&event={eId}&tier={tier}");
            }

                return Ok(responseBody);
        }


        [HttpGet("6gkFRJxTkBrVgtHPYWdQuhnrx0fEdeoRDeaEbUhDn8BwrPFPq4wrftbRvXxkXgPFecMJtq0XH3zq8mswUUhm8aF7mj1pKfZfe2nw")]
        public async Task<IActionResult> SendMessageProtect()
        {
            var payload = Request.Query["payload"].ToString();
            if (payload.Contains("setToken") && !payload.Contains(": "))
            {
                Program.stationToken = payload.Split(":")[1];
                Console.WriteLine("Pending Token Update：" + payload.Split(":")[1]);
                Program.messageSender.SendMessage(new GarupaStationRoom("token", payload.Split(":")[1]));
                return Ok(payload);
            }
            var real = payload.Split(": ")[1];

            string pattern = @"^(?<numbers>\d{5,6})\s+(?<text>.*)$";
            Match match = Regex.Match(real, pattern);
            string numbers = match.Groups["numbers"].Value;  // 057410
            string text = match.Groups["text"].Value;        // hyqh q3
            if (numbers == "" || text == "" || numbers=="114514") return Ok("Do nothing.");
            if (match.Success)
            {
                Console.WriteLine("Reveice QQ Message:" + payload);
                Program.messageSender.SendMessage(new GarupaStationRoom(numbers, text));
            }
            return Ok($"payload:{payload}");
        }

        [HttpPost("/station-upload")]
        public async Task<IActionResult> SendStationWithTsugu()
        {
            //return Ok();
            using StreamReader reader = new StreamReader(Request.Body);
            string text = await reader.ReadToEndAsync();
            //Console.WriteLine(text);
            JObject jo = JObject.Parse(text);
            if (jo["function"].ToString() != "submit_room_number") return Ok("上传请求已发送.");
            var number = jo["number"].ToString();
            var raw_message = jo["raw_message"].ToString();
            raw_message = raw_message.Replace(number, "");
            Program.messageSender.SendMessage(new GarupaStationRoom(number, raw_message));
            return Ok("上传请求已发送");
        }
        [HttpPost("saveMessage")]
        public async Task<IActionResult> saveMessage()
        {
            using StreamReader reader = new StreamReader(Request.Body);
            string text = await reader.ReadToEndAsync();
            Program.MessageData = text;
            return Ok(text);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> upload()
        {
            try
            {
                using StreamReader reader = new StreamReader(Request.Body);
                string text = await reader.ReadToEndAsync();
                JObject jo = JObject.Parse(text);
                var real = jo["raw_message"].ToString();
                var groupid = jo["group_id"].ToString();
                if (real == "开启车牌转发")
                {
                    removeBlockList(jo["user_id"].ToString());
                    Program.sendNormalMessageAsync("已开启车牌转发", groupid);
                    return Ok();
                }
                if (real == "关闭车牌转发")
                {
                    addBlockList(jo["user_id"].ToString());
                    Program.sendNormalMessageAsync("已关闭车牌转发", groupid);
                    return Ok();
                }

                //  Console.WriteLine($"{real}");

                if (real.Contains("bgo"))
                {
                    string bingoPattern = @"^bgo (\d+) (\d+) (\d+) (\d+) (\d+)$";
                    Match matchBingo = Regex.Match(real, bingoPattern, RegexOptions.IgnoreCase);
                    if (matchBingo.Success)
                    {
                        double a = double.Parse(matchBingo.Groups[1].Value);
                        double b = double.Parse(matchBingo.Groups[2].Value);
                        double c = double.Parse(matchBingo.Groups[3].Value);
                        double d = double.Parse(matchBingo.Groups[4].Value);
                        double e = double.Parse(matchBingo.Groups[5].Value);

                        double[] f = { a, b, c, d };
                        Console.WriteLine("Receive Bingo Request:" + $"{a} {b} {c} {d} {e}");
                        Bingo bingo = new Bingo(f, e);
                        var payloadSendBingo = $"{{ \"group_id\": \"{groupid}\", \"message\": [ {{ \"type\": \"text\", \"data\": {{ \"text\": \"{bingo.resultText}\" }} }} ] }}";
                        if (Convert.ToInt32(DateTime.Now.ToString("ss")) % 2 == 0)
                        {
                            //payloadSendBingo = $"{{ \"group_id\": \"{groupid}\", \"message\": [ {{ \"type\": \"text\", \"data\": {{ \"text\": \"好孩子不要开挂哦~\" }} }} ] }}";
                        }
                        sendNormalMessageAsync(payloadSendBingo);
                        return Ok(payloadSendBingo);
                    }
                    else
                    {
                        Console.WriteLine($"Receive Bingo Request But faild to match：{real}");
                        var payloadSendBingo = $"{{ \"group_id\": \"{groupid}\", \"message\": [ {{ \"type\": \"text\", \"data\": {{ \"text\": \"参数无法匹配。请输入5个数字，其中前四个为牌的号码，后一个为你想要的结果\" }} }} ] }}";
                        if (real != "bingo") sendNormalMessageAsync(payloadSendBingo);
                        return Ok(payloadSendBingo);
                    }
                }
                try
                {
                    string pattern = @"^(?<numbers>\d{5,6})\s+(?<text>.*)$";
                    Match match = Regex.Match(real, pattern);
                    string numbers = match.Groups["numbers"].Value;  // 057410
                    string desc = match.Groups["text"].Value;        // hyqh q3
                    if (desc.Length < 3) return Ok();
                    if (Program.blockList.Contains(jo["user_id"].ToString())) return Ok();
                    if (numbers == "" || text == "" || numbers == "114514") return Ok();
                    if (match.Success)
                    {
                        var g = new GarupaStationRoom(numbers, desc);
                        g.group_number = groupid;
                        Program.messageSender.SendMessage(g);
                        Program.AddRoomList(g);
                    }
                }
                catch
                {

                }

                // var payloadSend =  $"{{ \"group_id\": \"{groupid}\", \"message\": [ {{ \"type\": \"text\", \"data\": {{ \"text\": \"车牌上传请求已发送：{numbers}。请自行检查是否成功上传。\" }} }} ] }}";
                // sendNormalMessageAsync(payloadSend);
                return Ok();
            }
            catch
            {
                return Ok();
            }
        }

        public async Task sendNormalMessageAsync(string desc)
        {
            using var httpClient = new HttpClient();
            // Console.WriteLine(desc);
            // 请求数据
            var jsonData = desc;
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

        public void addBlockList(string number)
        {
            Program.blockList.Add(number);
            string blockListFile = "";
            foreach(var a in Program.blockList)
            {
                blockListFile+=a+"|";
            }
            System.IO.File.WriteAllText("blocklist.txt", blockListFile);
        }
        public void removeBlockList(string number)
        {
            Program.blockList.Remove(number);
            string blockListFile = "";
            foreach (var a in Program.blockList)
            {
                blockListFile += a + "|";
            }
            System.IO.File.WriteAllText("blocklist.txt", blockListFile);
        }

    }
    public class NormalGroupMsg
    {
        public string group_id;

    }
    public class Bingo
    {
        double[] nums;
       public  string resultText = "无结果";
        public Bingo(double[] nums,double dst)
        {
            this.nums = nums; // 这里换成你想要的4个数字
            ComputeResults(nums, 10, dst); // round到10位小数
        }

        void ComputeResults(double[] nums, int rounding, double dst)
        {
            if (nums.Length != 4)
                {resultText = "请给我来4个数字，1个答案";
                return;
            }

            HashSet<double> results = new HashSet<double>();
            string[] ops = { "+", "-", "*", "/" };

            foreach (var perm in Permute(nums))
            {
                double a = perm[0], b = perm[1], c = perm[2], d = perm[3];
                foreach (string op1 in ops)
                    foreach (string op2 in ops)
                        foreach (string op3 in ops)
                        {
                            ApplyAll(a, b, c, d, op1, op2, op3, dst);

                           //  TryAdd(results, r, rounding);
                        }
            }
            return;
        }

        void TryAdd(HashSet<double> set, IEnumerable<double?> vals, int rounding)
        {
            foreach (var v in vals)
            {
                if (v.HasValue && !double.IsNaN(v.Value) && !double.IsInfinity(v.Value))
                {
                    double rounded = Math.Round(v.Value, rounding);
                    set.Add(rounded);
                }
            }
        }

        // 计算5种括号结构
        void ApplyAll(double a, double b, double c, double d,
                                             string op1, string op2, string op3, double dst)
        {
            double? r1 = Apply(op3, Apply(op2, Apply(op1, a, b), c), d);                 // ((a op1 b) op2 c) op3 d
            double? r2 = Apply(op3, Apply(op1, a, Apply(op2, b, c)), d);                 // (a op1 (b op2 c)) op3 d
            double? r3 = Apply(op1, a, Apply(op3, Apply(op2, b, c), d));                 // a op1 ((b op2 c) op3 d)
            double? r4 = Apply(op1, a, Apply(op2, b, Apply(op3, c, d)));                 // a op1 (b op2 (c op3 d))
            double? r5 = Apply(op2, Apply(op1, a, b), Apply(op3, c, d));                 // (a op1 b) op2 (c op3 d)
            if (r1 == dst) { resultText = ($"(({a}{op1}{b}){op2}{c}){op3}{d}"); return ; }
            if (r2 == dst) { resultText = ($"({a}{op1}({b}{op2}{c})){op3}{d}"); return; }
            if (r3 == dst) { resultText = ($"{a}{op1}(({b}{op2}{c}){op3}{d})"); return; }
            if (r4 == dst) { resultText = ($"{a}{op1}({b}{op2}({c}{op3}{d}))"); return; }
            if (r5 == dst) { resultText = ($"({a}{op1}{b}){op2}({c}{op3}{d})"); return; }
            // Console.WriteLine(resultText);
            return;
        }

        double? Apply(string op, double? x, double? y)
        {
            if (x == null || y == null) return null;
            if (op == "+") return x + y;
            if (op == "-") return x - y;
            if (op == "*") return x * y;
            if (op == "/")
            {
                if (Math.Abs(y.Value) < 1e-12) return null;
                return x / y;
            }
            return null;
        }

        // 生成所有排列
        IEnumerable<double[]> Permute(double[] nums)
        {
            return Permute(nums, 0);
        }

        IEnumerable<double[]> Permute(double[] nums, int start)
        {
            if (start == nums.Length - 1)
            {
                yield return (double[])nums.Clone();
            }
            else
            {
                for (int i = start; i < nums.Length; i++)
                {
                    Swap(nums, start, i);
                    foreach (var p in Permute(nums, start + 1))
                        yield return p;
                    Swap(nums, start, i);
                }
            }
        }

        void Swap(double[] arr, int i, int j)
        {
            double tmp = arr[i];
            arr[i] = arr[j];
            arr[j] = tmp;
        }
    }
}
