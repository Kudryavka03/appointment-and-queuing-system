using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace SmsForwardWeixin.BandoriStation
{
    public class CutOffUtils
    {
        public static async Task<string> CutOffCalcAsync(string server, string eventId, string tier)
        {

            using var httpClient = new HttpClient();
            try
            {
                if (tier == "false")
                {
                    throw new Exception("预测线未在缓存列表中");
                }
                string responseBody = "";
                string responseBodyAll = Program.allEvent;
                if (responseBodyAll == "")
                {
                    HttpResponseMessage responseAll = await httpClient.GetAsync($"http://bestdori.com/api/events/all.6.json");
                    responseBodyAll = await responseAll.Content.ReadAsStringAsync();
                }
                var eventAll = JObject.Parse(responseBodyAll)[eventId];
                string endTimeStr = eventAll["endAt"][Convert.ToInt32(server)].ToString();
                string startTimeStr = eventAll["startAt"][Convert.ToInt32(server)].ToString();
                if (server == "3")      // 国服
                {
                    if (eventId == Program.eventId)     // 如果ID一样
                    {
                        if (tier == "20" || tier =="50" || tier=="100" || tier=="500" ||tier == "1000" || tier =="2000" || tier=="5000" || tier == "10000")
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
                    else
                    {
                        Program.updateApiCache(eventId);    // 不一样就更新ID
                        Console.WriteLine("Need Update Event ID,Please wait......" + eventId);
                        throw new Exception("Need Update Event ID,Please wait......");
                    }
                }
                if (responseBody == "")
                {
                    Console.WriteLine("responseBody == \"\"" + eventId);
                    HttpResponseMessage response = await httpClient.GetAsync($"http://bestdori.com/api/tracker/data?server={server}&event={eventId}&tier={tier}");
                    responseBody = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine("使用缓存数据进行预测，提升速度");
                }

                string eventType = eventAll["eventType"].ToString();
                // Console.WriteLine($"响应内容: {responseBody}");
                // var cutoffDetailsToken = JObject.Parse(responseBody)["cutoffs"];
                var cutoffDetails = JObject.Parse(responseBody)["cutoffs"];
                var cutoffDetailCounts = cutoffDetails.Count();
                // cutoffDetails从0开始，每一个包含一个time和ep
                long startTime = ConvertToLong(startTimeStr);    // 开活时间，因为服务器是15分才会
                long endTime = ConvertToLong(endTimeStr);    // 结活时间
                var startRatio = 15;    // 开活段占总时长的多少
                var microUpRatio = 60;    // 渐变抬升
                var endRatio = 98;     // 结活段占总时长的多少
                var upRatio = 85;       // 开始逐渐抬升的时长
                long oneRatio = (endTime - startTime) / 100;  // 1个比例大概是多少时间，方便计算
                List<CutOffObject> CutOffTB = new List<CutOffObject>();
                List<long> oneRatioAvgEp = new List<long>();    // 一个时间段（30分钟）的均速
                var oneRatioAvgEpRatio = 50;    // 均速权重
                List<long> lastRatioAvgEp = new List<long>();   // 最后一个均速
                var lastRatioAvgEpRatio = 100 - oneRatioAvgEpRatio;  // 最后一个记录均速的权重
                List<CutoffsItem> coi = new List<CutoffsItem>();
                long lastRatioAddEp = 0; // 最后一次提升的Ep，给冲刺阶段使用。
                for (int i = 0; i < cutoffDetails.Count(); i++)
                {
                    CutOffObject cutOffObject = new CutOffObject();
                    cutOffObject.time = ConvertToLong(cutoffDetails[i]["time"].ToString());
                    cutOffObject.ep = ConvertToLong(cutoffDetails[i]["ep"].ToString());
                    cutOffObject.day = (int)GetInEventDays(startTime, cutOffObject.time);   // 这里用int是安全的
                    cutOffObject.time_in_day = (int)GetInEventDayTimes(startTime, cutOffObject.time);   // 计算0~47哪个时间段的，好匹配
                    CutOffTB.Add(cutOffObject);
                    CutoffsItem cutoffsItem = new CutoffsItem();
                    cutoffsItem.time = cutOffObject.time;
                    cutoffsItem.ep = cutOffObject.ep;
                    coi.Add(cutoffsItem);
                }
                for (int i = 0; i < 48; i++)        // Bestdori一天会统计48次即每30分钟统计一次，一个24小时内能统计48次
                {
                    long AvgEp = 0;
                    int count = 0;
                    if (CutOffTB.Count == 0) break;  // 没有数据，无法计算
                    for (int j = 1; j < CutOffTB.Count; j++)   // 这里使用增量计算来统计均速ep，防止溢出。-1同理，因为会读取上一个下标。
                    {
                        if (CutOffTB[j].time_in_day == (i))
                        {
                            // 这里要看一下两个数据相隔，如果相差太远的话(没有记录在内)则break。
                            if (!(CutOffTB[j].time - CutOffTB[j - 1].time > 2400000))    // 超过1个小时的记录为无效记录,不允许参与记录，因此可能会参差不齐
                            {
                                var AddEp = CutOffTB[j].ep - CutOffTB[j - 1].ep;       // 计算平均增速
                                lastRatioAddEp = AddEp;// 记录增速，如果预测的时候已经进入冲刺阶段了，就直接用冲刺阶段的速度
                                count++;
                                AvgEp += (AddEp - AvgEp) / count;
                            }
                        }
                    }
                    oneRatioAvgEp.Add(AvgEp);
                }   // 因为bestdori记录还是有些问题的，所以可能会出现空分的情况，但是不影响。
                    // 接下来还要算最后一次增加的EP，这个权重是7.

                for (int i = 0; i <= 47; i++)        // Bestdori一天会统计48次即每30分钟统计一次，一个24小时内能统计48次
                {
                    for (int j = CutOffTB.Count; j > 1; j--)   // 是往回读取的。
                    {

                        if (CutOffTB[j - 1].time_in_day == (i))
                        {
                            var ep = CutOffTB[j - 1].ep - CutOffTB[j - 2].ep;
                            // Console.WriteLine(j-1);
                            if (!(CutOffTB[j - 1].time - CutOffTB[j - 2].time > 2000000))
                            {
                                lastRatioAvgEp.Add(ep);
                                break;
                            }
                        }
                    }
                }
                // 平均段
                var predictEpEndTimeDays = GetInEventDays(startTime, endTime);  // 获取活动结束时间是活动的第几期
                var normalEpRatio = 100;
                var upEpRatio = 250;    // 165
                var endEpRatio = 360;
                var microUpRatioEp = 1.2;
                if (Convert.ToInt32(tier) < 500 && eventType != "medley") { upEpRatio = 120; microUpRatio = 110; upRatio = 100; endRatio = 100; }    // t500以下的档线不做任何处理
                if (Convert.ToInt32(tier) < 500 && eventType == "medley") { upEpRatio = 250; microUpRatio = 110; upRatio = 70; endRatio = 85; }    // t500以下的档线medley清CP特殊处理
                if (tier == "500") { upEpRatio = 120; microUpRatio = 80; upRatio = 87; } // t500的曲线相对较平，不抬升，由end拉高
                if (tier == "1000") { endRatio = 96; upRatio = 70; microUpRatio = 1000; } // t1000后面冲刺阶段会比较猛，阈值给多

                if (eventType == "medley" || eventType== "challenge") { endRatio = 97; upRatio = 82; upEpRatio = 165; microUpRatio = 60; }
                if (Convert.ToInt32(tier) > 1999) { upEpRatio = 120; microUpRatio = 110; upRatio = 82; endRatio = 97; } // t2000的曲线不再介入处理，仅保留冲刺抬升
                if (Convert.ToInt32(tier) > 2999) { upEpRatio = 110; microUpRatio = 110; upRatio = 82; endRatio = 98; } // t2000的曲线不再介入处理，仅保留冲刺抬升
                if (Convert.ToInt32(tier) > 5001) { upEpRatio = 100; microUpRatio = 110; upRatio = 105; endRatio = 100; } // t5000后不再做任何处理
                // 到这一步，均速计算完成了。开始读取最后一条记录的分数......
                var lastRecord = cutoffDetails[cutoffDetailCounts - 1];
                var lastRecordEp = ConvertToLong(cutoffDetails[cutoffDetailCounts - 1]["ep"].ToString());
                var lastRecordTime = ConvertToLong(cutoffDetails[cutoffDetailCounts - 1]["time"].ToString());
                var lastRecordTimeInDays = GetInEventDayTimes(startTime, ConvertToLong(cutoffDetails[cutoffDetailCounts - 1]["time"].ToString())); // 最新一条记录位于当天的什么时间段
                var lastRecordDays = GetInEventDays(startTime, ConvertToLong(cutoffDetails[cutoffDetailCounts - 1]["time"].ToString()));       // 最新一条记录位于开活的哪一天
                var endRatioTime = (long)(double)((endTime - startTime) * endRatio * 0.01) + startTime; // 冲刺阶段，计算到冲刺阶段就结束
                Console.WriteLine(endRatioTime);
                var upRatioTime = (long)(double)((endTime - startTime) * upRatio * 0.01) + startTime; // 冲刺阶段，计算到冲刺阶段就结束
                var microRatioTime = (long)(double)((endTime - startTime) * microUpRatio * 0.01) + startTime; // 冲刺阶段，计算到冲刺阶段就结束
                bool isStopPredictFlags = false;
                var predictEp = lastRecordEp;
                var predictEpTime = lastRecordTime;
                var predictEpCutOffDetails = cutoffDetails; // 暂定：预测Object，用于返回
                var currentRatio = normalEpRatio;
                var tierLevel = Convert.ToInt32(tier);
                // 11.3 这里结果偏低。为什么？因为没有刷新平均分数表。正常情况下，我们应该要刷新平均分数表，但是这里没有刷新。要在循环里边刷新分数表。一切的数据都是根据已有的数据进行推断的
                // 
                int microUpProfessCount = 0;
                int currentMicroInProgressInEventDays = 0;
                while (!isStopPredictFlags)
                {
                    if (predictEpTime > endRatioTime)
                    {
                        isStopPredictFlags = true;
                        break;
                    }
                    predictEpTime = predictEpTime + 1800000;
                    oneRatioAvgEpRatio = 80;    // 平均一次提升的权重
                    lastRatioAvgEpRatio = 20;   // 最后一次提升的权重
                    // todo:判断是否进入冲刺阶段。进入冲刺阶段后，将最后一次分数提升的权重提升到50%，以更好地反映冲刺阶段的分数提升情况
                    if (predictEpTime > upRatioTime)
                    {
                        currentRatio = upEpRatio;
                        oneRatioAvgEpRatio = 50;
                        lastRatioAvgEpRatio = 50;
                        Debug.WriteLine($"抬升阶段参数生效 pT：{predictEpTime} uRT：{upRatioTime}");
                    }
                    var time_in_days = (int)GetInEventDayTimes(startTime, predictEpTime);
                    var eventInDays = (int)GetInEventDays(startTime, predictEpTime);
                    // Console.WriteLine(time_in_days);
                    var avgAddEp = (((oneRatioAvgEp[time_in_days] * oneRatioAvgEpRatio) / 100 + (lastRatioAvgEp[time_in_days] * lastRatioAvgEpRatio) / 100) * currentRatio) / 100;
                    if (predictEpTime > microRatioTime)
                    {
                        if (eventInDays != currentMicroInProgressInEventDays)
                        {
                            currentMicroInProgressInEventDays = eventInDays;
                            microUpProfessCount++;
                        }
                        avgAddEp = (long)(avgAddEp * Math.Pow(microUpRatioEp, microUpProfessCount));
                        // Debug.WriteLine($"{avgAddEp}  {microUpRatioEp} {microUpProfessCount} {currentMicroInProgressInEventDays}");
                    }
                    predictEp += avgAddEp;
                    lastRatioAddEp = avgAddEp; // 预测最后一次提升的Ep，预测也会算进去。
                    CutoffsItem cutoffsItem = new CutoffsItem();
                    cutoffsItem.time = predictEpTime;
                    cutoffsItem.ep = predictEp;
                    coi.Add(cutoffsItem);
                    if (true) // predictEpTime > microRatioTime
                    {
                        CutOffObject cutOffObject = new CutOffObject();
                        cutOffObject.time = predictEpTime;
                        cutOffObject.ep = predictEp;
                        cutOffObject.day = (int)GetInEventDays(startTime, predictEpTime);
                        cutOffObject.time_in_day = (int)GetTimeStamp0000(startTime, predictEpTime);
                        CutOffTB.Add(cutOffObject);
                        oneRatioAvgEp.Clear(); // 清空均值
                        lastRatioAvgEp.Clear();
                        for (int i = 0; i < 48; i++)        // Bestdori一天会统计48次即每30分钟统计一次，一个24小时内能统计48次
                        {
                            long AvgEp = 0;
                            int count = 0;
                            if (CutOffTB.Count == 0) break;  // 没有数据，无法计算
                            for (int j = 1; j < CutOffTB.Count; j++)   // 这里使用增量计算来统计均速ep，防止溢出。-1同理，因为会读取上一个下标。
                            {
                                if (CutOffTB[j].time_in_day == (i))
                                {
                                    // Debug.WriteLine("数据间隔：" + $"{CutOffTB[j].time - CutOffTB[j - 1].time}");
                                    // 这里要看一下两个数据相隔，如果相差太远的话(没有记录在内)则break。

                                    if (!(CutOffTB[j].time - CutOffTB[j - 1].time > 2000000))    // 超过1个小时的记录为无效记录,不允许参与记录，因此可能会参差不齐
                                    {
                                        var AddEp = CutOffTB[j].ep - CutOffTB[j - 1].ep;       // 计算平均增速
                                        lastRatioAddEp = AddEp;// 记录增速，如果预测的时候已经进入冲刺阶段了，就直接用冲刺阶段的速度
                                        count++;
                                        AvgEp += (AddEp - AvgEp) / count;
                                    }
                                }
                            }
                            oneRatioAvgEp.Add(AvgEp);
                        }   // 因为bestdori记录还是有些问题的，所以可能会出现空分的情况，但是不影响。
                            // 接下来还要算最后一次增加的EP，这个权重是7.

                        for (int i = 0; i <= 47; i++)        // Bestdori一天会统计48次即每30分钟统计一次，一个24小时内能统计48次
                        {
                            for (int j = CutOffTB.Count; j > 1; j--)   // 是往回读取的。
                            {

                                if (CutOffTB[j - 1].time_in_day == (i))
                                {
                                    var ep = CutOffTB[j - 1].ep - CutOffTB[j - 2].ep;
                                    // Console.WriteLine(j-1);
                                    if (!(CutOffTB[j - 1].time - CutOffTB[j - 2].time > 2000000))
                                    {
                                        lastRatioAvgEp.Add(ep);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                int endCalcCount = 1;
                isStopPredictFlags = false;

                // 冲刺阶段处理
                while (!isStopPredictFlags)
                {
                    if (predictEpTime > endTime)
                    {
                        isStopPredictFlags = true;
                        break;
                    }
                    if (predictEpTime < endRatioTime)
                    {
                        isStopPredictFlags = true;
                        break;
                    }
                    // 处理倍数叠不上去的问题，解决到后面预测偏低的问题
                    if (predictEpTime - endRatioTime > 0)
                    {
                        endCalcCount = (int)(predictEpTime - endRatioTime) / 1800000;
                    }
                    Debug.WriteLine($"Enter 冲刺阶段 {predictEpTime}");
                    predictEpTime = predictEpTime + 1800000;
                    long oneRatioHighestEp = 0;
                    long lastRatioHighestEp = 0;
                    int oneRatioHighestEpRatio = 1;
                    int lastRatioHighestEpRatioRatio = 7;
                    int lastRatioAddEpRatio = 2;

                    foreach (var a in oneRatioAvgEp)
                    {
                        if (a > oneRatioHighestEp)
                        {
                            oneRatioHighestEp = a;
                        }
                    }
                    foreach (var a in lastRatioAvgEp)
                    {
                        if (a > lastRatioHighestEp)
                        {
                            lastRatioHighestEp = a;
                        }
                    }
                    var highest_score = (oneRatioHighestEp * oneRatioHighestEpRatio + lastRatioHighestEp * lastRatioHighestEpRatioRatio + lastRatioAddEp * lastRatioAddEpRatio) /
                        (oneRatioHighestEpRatio + lastRatioHighestEpRatioRatio + lastRatioAddEpRatio);
                    var endUpRatio = SetEndUpRatio(tierLevel);
                    if (endCalcCount > 3)
                    {
                        if (eventType == "medley" || eventType == "challenge")
                        {
                            if (endCalcCount > 8)
                                endCalcCount = 8;
                        }
                        else endCalcCount = 3;
                    }// 因为是ROW，不允许无限制的增长下去，最多允许2.5倍增长。对于组曲Live，允许10次的增长以应对活动结活清CP的异常增长
                    var avgAddEp = highest_score * Math.Pow(endUpRatio, endCalcCount);
                    if (GetInEventDays(startTime, predictEpTime) >= 11)     // 超长活一般到最后都不算猛烈，给个1.1就好
                    {
                        avgAddEp = highest_score * 1.1;
                    }

                    // lastRatioAddEp = (long)avgAddEp;
                    Debug.WriteLine($"{oneRatioHighestEp}  {lastRatioHighestEp}  {lastRatioAddEp}");

                    //endCalcCount++;
                    predictEp += (long)avgAddEp;
                    CutoffsItem cutoffsItem = new CutoffsItem();
                    cutoffsItem.time = predictEpTime;
                    cutoffsItem.ep = predictEp;
                    coi.Add(cutoffsItem);
                }
                ResultCutOffObject resultCutOffObject = new ResultCutOffObject();
                resultCutOffObject.result = true;
                resultCutOffObject.cutoffs = coi;
                string resultJson = JsonConvert.SerializeObject(resultCutOffObject, Newtonsoft.Json.Formatting.Indented);
                Console.WriteLine($"活动Id：{eventId} 预测线：{tier} 预测结果：{predictEp}");
                //return $"活动Id：{eventId} 预测线：{tier} 预测结果：{predictEp}";
                return resultJson;
            }
            catch (Exception e)
            {
                CutoffsItem cutoffsItem = new CutoffsItem();
                cutoffsItem.time = 0;
                cutoffsItem.ep = 0;
                ResultCutOffObject resultCutOffObject = new ResultCutOffObject();
                resultCutOffObject.result = true;
                resultCutOffObject.cutoffs = new List<CutoffsItem>{ cutoffsItem};
                string resultJson = JsonConvert.SerializeObject(resultCutOffObject, Formatting.Indented);
                return resultJson;
            }
        }

        public static double SetEndUpRatio(int tierLevel)
        {
            // 20不作抬升处理，能全程冲的。
            var endUpRatio = 1.00;
            if (tierLevel == 50)
            {
                endUpRatio = 1.1;
            }
            if (tierLevel == 100)
            {
                endUpRatio = 1.1;
            }
            if (tierLevel == 200)
            {
                endUpRatio = 1.1;
            }
            if (tierLevel == 300 || tierLevel == 400 || tierLevel == 500)    // 300/400均为1.04
            {
                endUpRatio = 1.2;
            }
            if (tierLevel == 1000)
            {
                endUpRatio = 1.2;
            }
            if (tierLevel == 2000)
            {
                endUpRatio = 1.05;
            }
            if (tierLevel == 3000)
            {
                endUpRatio = 1.01;
            }
            if (tierLevel == 4000)
            {
                endUpRatio = 1.00;
            }
            return endUpRatio;
        }
        public static long ConvertToLong(string time)
        {
            return Convert.ToInt64(time);
        }
        public static long GetTime(JObject o)
        {
            return Convert.ToInt64(o["time"].ToString());
        }
        public static long GetTimeStamp0000(long startTime, long calcTime)  
        {
            DateTime UtcStart = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1)).ToLocalTime();//19700101
            DateTime sTime = DateTimeOffset.FromUnixTimeMilliseconds(startTime).DateTime.ToLocalTime().Date;
            long sTimeStamp = new DateTimeOffset(sTime).ToUnixTimeMilliseconds();
            return (long)sTimeStamp;
        }
        public static long GetInEventDays(long startTime,long calcTime)
        {
            long duringMillionSeconds = calcTime - GetTimeStamp0000(startTime,calcTime);  // 取得活动时间当天
            return (duringMillionSeconds / 86400000)+1;         // 除以一天的总毫秒数，获得是第几天。我们不关系邦邦刷新时间，只关注时间占比。
        }
        public static long GetInEventDayTimes(long startTime, long calcTime)
        {
            long time_in_day = calcTime - GetTimeStamp0000(calcTime, calcTime);   // during
            return (time_in_day / 1800000);         // 除以半小时取余数，范围是0~47
        }

    }
    public class CutOffObject
    {
        public int day;             // 活动的第几天的数据
        public int time_in_day;     // 范围是1-48。指的是这个分数段在当天什么时段。
        public long ep;     // ep
        public long time;   // 这个分数点的时间
    }
    public class CutoffsItem
    {
        public long time { get; set; }
        public long ep { get; set; }
    }

    public class ResultCutOffObject
    {
        public bool result { get; set; }
        public List<CutoffsItem> cutoffs { get; set; }
    }
}
