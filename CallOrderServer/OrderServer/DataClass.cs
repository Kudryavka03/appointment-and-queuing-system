using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

namespace OrderServer;

public class DataClass
{
    public static string desc = "My favourite Band is Roselia,Poppin Party and Afterglow. Like Yukina Best,Yukina I Love You!!! Kasumi I Love You!!!";
    public static string LastestOrder = "0";
    public static int LastestOrderNum = 0;
    public static int speed = 0;
    public static int reportSpeed = 0;
    public static readonly int highLevelFrontInt = 1000;

    public static List<EnumStatus> workQueueStatus = new List<EnumStatus>();
    public static List<int> workQueueNum = new List<int>();
    public static List<List<EnumType>> workQueuesType = new List<List<EnumType>>();
    public static List<OrderReportObject> OrderReportObject = new List<OrderReportObject>();
    public static ConcurrentDictionary<int, List<EnumType>> id2type = new ConcurrentDictionary<int, List<EnumType>>();
    public static List<KasumiHighLevelList> kasumiHighLevelLists = new List<KasumiHighLevelList>();
    public static List<YukinaBoardObject> YukinaBoard = new List<YukinaBoardObject>();
    public static List<OrderNumber> tempNums = new List<OrderNumber>();
    public static List<int> giveUpNums = new List<int>();
    public static List<WorkQueue> workQueues = new List<WorkQueue>();

    public static object lockKasumiHighLevelLists = new object();
    public static object lockOrderReportObject = new object();
    public static object lockWorkQueueStatus = new object();
    public static object lockWorkQueuesType = new object();
    public static object lockWorkQueue = new object();
    public static object lockYukinaBoard = new object();
    public static object lockTempNums = new object();

    public static int totalWindowNum = 0;
    public static int uuid = 0;
    public static int highLevelInt = 0;

    private static readonly Random random = new Random();
    private static readonly object lockAllocator = new object();
    private static readonly List<int> allocatorBlockList = new List<int>();
    private static int allocatorCursor = 0;
    private static bool suppressNotifications = false;
    public DataClass(int size)
    {
    }

    public static void initData(int size)
    {
        ArchiveStateAndReset(size);
    }

    public static void ArchiveStateAndReset(int defaultWindowNum)
    {
        suppressNotifications = true;
        try
        {
            OrderStateStore.ArchiveExistingStateForStartup();
            ResetData(defaultWindowNum);
        }
        finally
        {
            suppressNotifications = false;
        }

        NotifyStateChanged(false);
    }

    public static bool LoadStateOrDefault(int defaultWindowNum)
    {
        suppressNotifications = true;
        bool loaded = false;
        try
        {
            var state = OrderStateStore.Load();
            if (state != null && (state.TotalWindowNum > 0 || state.WorkQueues.Count > 0))
            {
                ApplyPersistedState(state);
                loaded = true;
            }
            else
            {
                Program.Log("没有可加载的历史状态，保持当前全新状态。");
            }
        }
        finally
        {
            suppressNotifications = false;
        }
        NotifyStateChanged(false);
        return loaded;
    }

    public static PersistedOrderState CreatePersistedState()
    {
        var state = new PersistedOrderState
        {
            SavedAt = DateTime.Now.ToString("O"),
            LastestOrder = LastestOrder,
            LastestOrderNum = LastestOrderNum,
            Speed = speed,
            ReportSpeed = reportSpeed,
            Uuid = uuid,
            HighLevelInt = highLevelInt,
            TotalWindowNum = totalWindowNum,
            AllocatorCursor = allocatorCursor,
            CurrentId = Program.currentId
        };

        lock (lockAllocator)
        {
            state.AllocatorBlockList = allocatorBlockList.ToList();
        }

        lock (lockWorkQueue)
        {
            state.WorkQueues = workQueues.Select(q => q.CreateState()).ToList();
        }

        lock (lockWorkQueuesType)
        {
            state.WorkQueueTypeMasks = workQueuesType.Select(TypeListToMask).ToList();
        }

        lock (lockTempNums)
        {
            state.TempNums = tempNums.Select(n => new OrderNumberState
            {
                Id = n.id,
                TypeMask = string.IsNullOrEmpty(n.typeStr) ? TypeListToMask(n.eType) : n.typeStr
            }).ToList();
            state.GiveUpNums = giveUpNums.ToList();
        }

        lock (lockOrderReportObject)
        {
            state.Reports = OrderReportObject.Select(r => new OrderReportState
            {
                Time = r.time,
                Window = r.Window,
                TypeMask = TypeListToMask(r.eType),
                Num = r.num
            }).ToList();
        }

        lock (lockKasumiHighLevelLists)
        {
            state.HighLevelOrders = kasumiHighLevelLists.Select(k => new HighLevelOrderState
            {
                OriginalOperator = k.getOriginalOperator(),
                TargetOperator = k.getTragetOperator(),
                Message = k.getMessage(),
                Num = k.getNum()
            }).ToList();
        }

        state.Id2TypeMasks = id2type.ToDictionary(pair => pair.Key, pair => TypeListToMask(pair.Value));
        state.History = HashtableToIntDictionary(Program.history);
        state.Uuid2Id = HashtableToDictionary(Program.uuid2id);
        state.Id2Uuid = HashtableToIntStringDictionary(Program.id2uuid);
        state.Logs = Program.logs.Count > 1000 ? Program.logs.Skip(Program.logs.Count - 1000).ToList() : Program.logs.ToList();

        return state;
    }

    public static RealtimeOrderSnapshot CreateRealtimeSnapshot()
    {
        var snapshot = new RealtimeOrderSnapshot
        {
            ServerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            LastestOrder = LastestOrder,
            LastestOrderNum = LastestOrderNum,
            Uuid = uuid,
            WaitingCount = uuid == 0 ? 0 : Math.Max(0, uuid - LastestOrderNum),
            ReportSpeed = reportSpeed,
            TotalWindowNum = totalWindowNum,
            HasArchivedState = File.Exists(OrderStateStore.GetLoadCandidatePath()),
            ArchivedStatePath = OrderStateStore.GetLoadCandidatePath(),
            TypeNames = Enum.GetNames(typeof(EnumType)).Where(name => name != EnumType.HIGHLEVEL.ToString()).ToList(),
            Logs = Program.logs.Count > 300 ? Program.logs.Skip(Program.logs.Count - 300).ToList() : Program.logs.ToList()
        };

        for (int i = 0; i < totalWindowNum; i++)
        {
            EnumStatus status = GetWorkStatusD(i);
            int currentNum = GetCurrentNum(i);
            string displayText = status == EnumStatus.STOP ? "暂停服务" : (currentNum == -1 ? "暂无分配" : currentNum.ToString());
            string typeMask;
            string typeText;
            lock (lockWorkQueuesType)
            {
                typeMask = i < workQueuesType.Count ? TypeListToMask(workQueuesType[i]) : "11";
                typeText = i < workQueuesType.Count ? ParserTypeToString(workQueuesType[i]) : "[]";
            }

            snapshot.Windows.Add(new RealtimeWindowSnapshot
            {
                Window = i + 1,
                Status = status.ToString(),
                CurrentNum = currentNum,
                DisplayText = displayText,
                TypeText = typeText,
                TypeMask = typeMask,
                IsBusy = status == EnumStatus.BUSY,
                IsPaused = status == EnumStatus.STOP
            });
        }

        return snapshot;
    }

    public static void ChangeType(int a, string type)
    {
        if (!IsValidWindow(a - 1)) return;
        lock (lockWorkQueuesType)
        {
            workQueuesType[a - 1] = ParserType(type);
            Program.Log($"窗口{a} 业务类型变更: {ParserTypeToString(type)}");
        }
        NotifyStateChanged(true);
    }

    public static void ResizeWindows(int size)
    {
        if (size < 1) size = 1;

        lock (lockWorkQueue)
        lock (lockWorkQueueStatus)
        lock (lockWorkQueuesType)
        lock (lockYukinaBoard)
        {
            int current = workQueues.Count;
            if (size > current)
            {
                for (int i = current; i < size; i++)
                {
                    workQueueStatus.Add(EnumStatus.STOP);
                    workQueueNum.Add(-1);
                    workQueuesType.Add(ParserType("11"));
                    workQueues.Add(new WorkQueue(i + 1));
                    YukinaBoard.Add(new YukinaBoardObject(i));
                    Program.Log($"窗口初始化：{i + 1}号窗口 业务：{ParserTypeToString("11")} 成功.");
                }
            }
            else if (size < current)
            {
                workQueueStatus.RemoveRange(size, workQueueStatus.Count - size);
                workQueueNum.RemoveRange(size, workQueueNum.Count - size);
                workQueuesType.RemoveRange(size, workQueuesType.Count - size);
                workQueues.RemoveRange(size, workQueues.Count - size);
                YukinaBoard.RemoveRange(size, YukinaBoard.Count - size);
            }

            totalWindowNum = size;
        }

        lock (lockKasumiHighLevelLists)
        {
            kasumiHighLevelLists.RemoveAll(item => item.getTragetOperator() > size);
        }

        Program.Log($"窗口数量已设置为：{size}");
        NotifyStateChanged(true);
    }

    public static YukinaBoardObject GetYukinaBoard(int i)
    {
        try
        {
            return YukinaBoard[i];
        }
        catch (Exception e)
        {
            Program.Log($"获取窗口{i}的YukinaBoard失败：{e.Message}");
            return null;
        }
    }

    public static int GetLastestNum()
    {
        lock (lockTempNums)
        {
            return tempNums.Count == 0 ? 0 : tempNums[tempNums.Count - 1].id;
        }
    }

    public static WorkQueue getWorkQueue(int i)
    {
        lock (lockWorkQueue)
        {
            return workQueues[i];
        }
    }

    public static bool WriteReport(OrderReportObject o, bool notify = true)
    {
        lock (lockOrderReportObject)
        {
            OrderReportObject.Add(o);
        }
        if (notify) NotifyStateChanged(true);
        return true;
    }

    public static OrderReportObject ReadReport(int i)
    {
        lock (lockOrderReportObject)
        {
            foreach (var o in OrderReportObject)
            {
                if (o.num == i) return o;
            }
        }
        return null;
    }

    public static void SetNextID(int index, int number, bool notify = true)
    {
        if (!IsValidWindow(index)) return;
        lock (lockWorkQueue)
        {
            workQueues[index].SetNextID(number);
            Program.history[number] = index + 1;
            Program.currentId = number.ToString();
        }
        if (notify) NotifyStateChanged(true);
    }

    public static int GetCurrentNum(int index)
    {
        if (!IsValidWindow(index)) return -1;
        lock (lockWorkQueue)
        {
            return workQueues[index].GetCurrentNum();
        }
    }

    public static string GetCurrentNumType(int number)
    {
        return id2type.TryGetValue(number, out var type) ? ParserTypeToString(type) : "[HIGHLEVEL]";
    }

    public static void CallFinished(int index)
    {
        if (!IsValidWindow(index)) return;
        lock (lockWorkQueue)
        {
            workQueues[index].CallFinished();
            Program.Log("窗口：" + (index + 1) + "窗 呼叫下一号");
        }
        NotifyStateChanged(true);
    }

    public static void CallStart(int index)
    {
        if (!IsValidWindow(index)) return;
        lock (lockWorkQueue)
        {
            workQueues[index].CallStart();
            Program.Log("窗口继续：" + (index + 1) + "窗");
        }
        NotifyStateChanged(true);
    }

    public static void CallStop(int index)
    {
        if (!IsValidWindow(index)) return;
        lock (lockWorkQueue)
        {
            workQueues[index].CallStop();
            Program.Log("窗口暂停：" + (index + 1) + "窗");
        }
        NotifyStateChanged(true);
    }

    public static void FixID(int index)
    {
        uuid = index;
        NotifyStateChanged(true);
    }

    public static string TryFixID(int fixNum)
    {
        int oldNum = uuid;
        if (fixNum <= uuid)
        {
            return "修正失败，修正号数要比当前号数大。";
        }

        uuid = fixNum;
        Program.Log("修正叫号 " + oldNum + " > " + uuid);
        NotifyStateChanged(true);
        return "修正成功";
    }

    public static EnumStatus GetWorkStatusD(int index)
    {
        if (!IsValidWindow(index)) return EnumStatus.STOP;
        lock (lockWorkQueue)
        {
            return workQueues[index].GetWorkStatusD();
        }
    }

    public static void AddKasumiHighLevelOrder(KasumiHighLevelList kasumiHighLevelList, bool notify = true)
    {
        lock (lockKasumiHighLevelLists)
        {
            kasumiHighLevelLists.Add(kasumiHighLevelList);
        }
        if (notify) NotifyStateChanged(true);
    }

    public static int GenNewUuid(string typeString)
    {
        List<EnumType> e = ParserType(typeString);
        int newId;
        lock (lockTempNums)
        {
            newId = ++uuid;
            tempNums.Add(new OrderNumber(newId, e, typeString));
        }

        var timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds().ToString();
        Program.uuid2id[timestamp] = newId;
        Program.id2uuid[newId] = timestamp;
        Program.Log("新取号：" + newId + "号 " + $"业务类型：{ParserTypeToString(typeString)}" + " - 校验码：" + timestamp);
        NotifyStateChanged(true);
        return newId;
    }

    public static int GenHighLevel(int originalOperator, int targetWindow, string message)
    {
        int n = ++highLevelInt;
        int number = n + highLevelFrontInt;
        AddKasumiHighLevelOrder(new KasumiHighLevelList(originalOperator, targetWindow, message, number), false);
        Program.Log("新高优先级取号：" + number + "号 - 固定窗口分配：" + targetWindow + " 操作人：" + originalOperator + " 说明：" + message);
        NotifyStateChanged(true);
        return number;
    }

    public static int GenHighLevelPass(int number)
    {
        var report = ReadReport(number);
        if (report == null) return 0;
        int n = ++highLevelInt;
        int newNumber = n + highLevelFrontInt;
        AddKasumiHighLevelOrder(new KasumiHighLevelList(0, report.Window, "过号", newNumber), false);
        Program.Log("新高优先级取号[过号]：" + newNumber + "号 - 固定窗口分配：" + report.Window + " 操作人：0 说明：过号");
        NotifyStateChanged(true);
        return newNumber;
    }

    public static int Allocator(OrderNumber index)
    {
        return Allocator(index, true);
    }

    private static int Allocator(OrderNumber index, bool notify)
    {
        if (index == null) return 0;
        if (giveUpNums.Contains(index.id)) return 0;

        List<int> isAvailableWindow = new List<int>();
        string msg = "分配器: 窗口空闲情况：";

        for (int j = 0; j < totalWindowNum; j++)
        {
            if (GetWorkStatusD(j) == EnumStatus.STANDBY && CheckTypeWindowReady(j, index.eType))
            {
                isAvailableWindow.Add(j);
                msg += (j + 1) + " ";
            }
        }

        if (isAvailableWindow.Count == 0) return 0;

        Program.Log(msg);
        int k = getCryptoRandom(0, isAvailableWindow.Count);
        Program.Log("随机数：" + (k + 1) + "   范围：1 - " + isAvailableWindow.Count);
        int windowIndex = isAvailableWindow[k];
        id2type[index.id] = index.eType;
        SetNextID(windowIndex, index.id, false);
        Program.Log("将" + index.id + "号 业务：" + ParserTypeToString(index.typeStr) + " 分配给" + (windowIndex + 1) + "号窗");
        LastestOrderNum++;
        LastestOrder = index.id.ToString();
        speed++;
        WriteReport(new OrderReportObject(Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss")), windowIndex + 1, index.eType, index.id), false);
        if (notify) NotifyStateChanged(true);
        return 1;
    }

    public static bool AllocatorHighLevel()
    {
        bool allocated = false;
        lock (lockKasumiHighLevelLists)
        {
            for (int i = 0; i < kasumiHighLevelLists.Count; i++)
            {
                int win = kasumiHighLevelLists[i].getTragetOperator() - 1;
                int num = kasumiHighLevelLists[i].getNum();
                if (!IsValidWindow(win)) continue;
                if (GetWorkStatusD(win) != EnumStatus.STANDBY) continue;

                SetNextID(win, num, false);
                Program.Log($"高优先级分配：号码{num} 分配至固定窗口{win + 1}");
                WriteReport(new OrderReportObject(Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss")), win + 1, new List<EnumType> { EnumType.HIGHLEVEL }, num), false);
                speed++;
                kasumiHighLevelLists.RemoveAt(i);
                allocated = true;
                break;
            }
        }
        if (allocated) NotifyStateChanged(true);
        return allocated;
    }

    public static int genRamdomNum(int s, int e)
    {
        lock (random)
        {
            return random.Next(s, e);
        }
    }

    public static int getCryptoRandom(int minInclusive, int maxExclusive)
    {
        if (minInclusive >= maxExclusive) return 0;

        int range = maxExclusive - minInclusive;
        byte[] randomNumber = new byte[1];
        int maxRejection = 256 - (256 % range);

        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            while (true)
            {
                rng.GetBytes(randomNumber);
                if (randomNumber[0] < maxRejection)
                {
                    return minInclusive + (randomNumber[0] % range);
                }
            }
        }
    }

    public static bool CheckTypeWindowReady(int index, List<EnumType> eType)
    {
        lock (lockWorkQueuesType)
        {
            foreach (EnumType type in eType)
            {
                if (!workQueuesType[index].Contains(type)) return false;
            }
            return true;
        }
    }

    public static bool AnyAvailableWindow()
    {
        lock (lockWorkQueue)
        {
            foreach (var a in workQueues)
            {
                if (a.GetWorkStatusD() == EnumStatus.STANDBY) return true;
            }
        }
        return false;
    }

    public static async void Listener()
    {
        Program.Log("开始监听取号队列...");
        while (true)
        {
            try
            {
                if (AnyAvailableWindow())
                {
                    bool changed = AllocatorHighLevel();

                    lock (lockAllocator)
                    {
                        for (int i = 0; i < allocatorBlockList.Count; i++)
                        {
                            int blockedIndex = allocatorBlockList[i];
                            OrderNumber blockedNumber = null;
                            lock (lockTempNums)
                            {
                                if (blockedIndex >= 0 && blockedIndex < tempNums.Count) blockedNumber = tempNums[blockedIndex];
                            }

                            if (Allocator(blockedNumber, false) == 1)
                            {
                                changed = true;
                                allocatorBlockList.RemoveAt(i);
                                i--;
                            }
                        }

                        OrderNumber nextNumber = null;
                        lock (lockTempNums)
                        {
                            if (allocatorCursor < tempNums.Count) nextNumber = tempNums[allocatorCursor];
                        }

                        if (nextNumber != null)
                        {
                            if (Allocator(nextNumber, false) == 1)
                            {
                                changed = true;
                            }
                            else if (!allocatorBlockList.Contains(allocatorCursor))
                            {
                                allocatorBlockList.Add(allocatorCursor);
                                changed = true;
                            }
                            allocatorCursor++;
                            changed = true;
                        }
                    }

                    if (changed) NotifyStateChanged(true);
                }
            }
            catch (Exception e)
            {
                Program.Log("取号队列监听异常：" + e.Message);
            }
            Thread.Sleep(50);
        }
    }

    public static void ReportEx()
    {
        Program.Log("管理员已为此服务器配置自动时速监测，每一次监测记录时长为10分钟.");
        int min = 10;
        while (true)
        {
            Program.Log($"当前业务办理时速: {speed}p / {min} min");
            reportSpeed = speed;
            speed = 0;
            NotifyStateChanged(false);
            Thread.Sleep(min * 60000);
        }
    }

    public static void AutoSaveLog()
    {
        Program.Log("管理员已为此服务器配置自动日志保存，每次保存间隔时长为1分钟.");
        while (true)
        {
            try
            {
                var logFilesName = Program.StartTimestamp + ".txt";
                var logs = Program.logs;
                var result = "";
                foreach (var log in logs)
                {
                    result += log;
                    result += "\r\n";
                }
                File.WriteAllText(logFilesName, result);
            }
            catch (Exception e)
            {
                Program.Log("日志保存失败：" + e.Message);
            }
            Thread.Sleep(60000);
        }
    }

    public static void SetStatusData(int index, EnumStatus enumStatus)
    {
        lock (lockWorkQueueStatus)
        {
            if (index < 0 || index >= workQueueStatus.Count) return;
            workQueueStatus[index] = enumStatus;
        }
    }

    public static EnumStatus GetStatusData(int index)
    {
        lock (lockWorkQueueStatus)
        {
            return index < 0 || index >= workQueueStatus.Count ? EnumStatus.STOP : workQueueStatus[index];
        }
    }

    public static void SetWindowNum(int index, int num)
    {
        lock (lockWorkQueueStatus)
        {
            if (index < 0 || index >= workQueueNum.Count) return;
            workQueueNum[index] = num;
        }
        NotifyStateChanged(true);
    }

    public static int GetWindowNum(int index)
    {
        lock (lockWorkQueueStatus)
        {
            return index < 0 || index >= workQueueNum.Count ? -1 : workQueueNum[index];
        }
    }

    public static List<EnumType> ParserType(string a)
    {
        List<EnumType> result = new List<EnumType>();
        if (string.IsNullOrEmpty(a)) return result;

        int maxBusinessType = Enum.GetNames(typeof(EnumType)).Length - 1;
        for (int i = 0; i < a.Length && i < maxBusinessType; i++)
        {
            if (a[i] == '1') result.Add((EnumType)i);
        }
        return result;
    }

    public static string ParserTypeToString(List<EnumType> type)
    {
        string result = "[";
        for (int i = 0; i < type.Count(); i++)
        {
            result += type[i].ToString();
            if (i < type.Count() - 1) result += ",";
        }
        result += "]";
        return result;
    }

    public static string ParserTypeToString(string a)
    {
        return ParserTypeToString(ParserType(a));
    }

    public static string TypeListToMask(List<EnumType> types)
    {
        int maxBusinessType = Enum.GetNames(typeof(EnumType)).Length - 1;
        char[] chars = Enumerable.Repeat('0', maxBusinessType).ToArray();
        if (types != null)
        {
            foreach (var type in types)
            {
                int index = (int)type;
                if (index >= 0 && index < chars.Length) chars[index] = '1';
            }
        }
        return new string(chars);
    }

    private static void ResetData(int size)
    {
        if (size < 1) size = 1;
        Program.Log("开始初始化窗口数据...");
        LastestOrder = "0";
        LastestOrderNum = 0;
        speed = 0;
        reportSpeed = 0;
        totalWindowNum = 0;
        uuid = 0;
        highLevelInt = 0;
        Program.currentId = "0";
        Program.history = new Hashtable();
        Program.uuid2id = new Hashtable();
        Program.id2uuid = new Hashtable();
        id2type = new ConcurrentDictionary<int, List<EnumType>>();

        workQueueStatus.Clear();
        workQueueNum.Clear();
        workQueuesType.Clear();
        OrderReportObject.Clear();
        kasumiHighLevelLists.Clear();
        YukinaBoard.Clear();
        tempNums.Clear();
        giveUpNums.Clear();
        workQueues.Clear();
        lock (lockAllocator)
        {
            allocatorCursor = 0;
            allocatorBlockList.Clear();
        }

        for (int i = 0; i < size; i++)
        {
            workQueueStatus.Add(EnumStatus.STOP);
            workQueueNum.Add(-1);
            string type = "11";
            workQueuesType.Add(ParserType(type));
            workQueues.Add(new WorkQueue(i + 1));
            totalWindowNum++;
            Program.Log($"窗口初始化：{i + 1}号窗口 业务：{ParserTypeToString(type)} 成功.");
            YukinaBoard.Add(new YukinaBoardObject(i));
            Program.Log($"窗口初始化：{i + 1}号窗口 YukinaBoard 成功.");
        }
        Program.Log("窗口数据初始化成功.");
    }

    private static void ApplyPersistedState(PersistedOrderState state)
    {
        LastestOrder = string.IsNullOrEmpty(state.LastestOrder) ? "0" : state.LastestOrder;
        LastestOrderNum = state.LastestOrderNum;
        speed = state.Speed;
        reportSpeed = state.ReportSpeed;
        uuid = state.Uuid;
        highLevelInt = state.HighLevelInt;
        Program.currentId = string.IsNullOrEmpty(state.CurrentId) ? "0" : state.CurrentId;
        Program.history = IntDictionaryToHashtable(state.History);
        Program.uuid2id = DictionaryToHashtable(state.Uuid2Id);
        Program.id2uuid = IntStringDictionaryToHashtable(state.Id2Uuid);
        Program.logs = state.Logs ?? new List<string>();

        workQueueStatus.Clear();
        workQueueNum.Clear();
        workQueuesType.Clear();
        OrderReportObject.Clear();
        kasumiHighLevelLists.Clear();
        YukinaBoard.Clear();
        tempNums.Clear();
        giveUpNums.Clear();
        workQueues.Clear();
        id2type = new ConcurrentDictionary<int, List<EnumType>>();

        int count = Math.Max(1, Math.Max(state.TotalWindowNum, state.WorkQueues.Count));
        totalWindowNum = count;
        for (int i = 0; i < count; i++)
        {
            WorkQueueState queueState = i < state.WorkQueues.Count ? state.WorkQueues[i] : new WorkQueueState();
            workQueues.Add(new WorkQueue(i + 1, queueState));
            workQueueStatus.Add(ParseStatus(queueState.Status));
            workQueueNum.Add(queueState.CurrentNum);
            string typeMask = i < state.WorkQueueTypeMasks.Count ? state.WorkQueueTypeMasks[i] : "11";
            workQueuesType.Add(ParserType(typeMask));
            YukinaBoard.Add(new YukinaBoardObject(i));
        }

        if (state.TempNums != null)
        {
            foreach (var item in state.TempNums)
            {
                tempNums.Add(new OrderNumber(item.Id, ParserType(item.TypeMask), item.TypeMask));
            }
        }

        giveUpNums = state.GiveUpNums ?? new List<int>();

        if (state.Reports != null)
        {
            foreach (var item in state.Reports)
            {
                OrderReportObject.Add(new OrderReportObject(item.Time, item.Window, ParserType(item.TypeMask), item.Num));
            }
        }

        if (state.HighLevelOrders != null)
        {
            foreach (var item in state.HighLevelOrders)
            {
                kasumiHighLevelLists.Add(new KasumiHighLevelList(item.OriginalOperator, item.TargetOperator, item.Message, item.Num));
            }
        }

        if (state.Id2TypeMasks != null)
        {
            foreach (var item in state.Id2TypeMasks)
            {
                id2type[item.Key] = ParserType(item.Value);
            }
        }

        lock (lockAllocator)
        {
            allocatorCursor = Math.Max(0, state.AllocatorCursor);
            allocatorBlockList.Clear();
            if (state.AllocatorBlockList != null) allocatorBlockList.AddRange(state.AllocatorBlockList);
        }

        Program.Log($"状态恢复成功：窗口 {totalWindowNum} 个，当前号码 {uuid}，等待队列 {tempNums.Count} 条。");
    }

    private static void NotifyStateChanged(bool saveImmediately)
    {
        if (suppressNotifications) return;
        if (saveImmediately) OrderStateStore.SaveNow();
        OrderWebSocketHub.BroadcastSnapshot();
    }

    private static bool IsValidWindow(int index)
    {
        return index >= 0 && index < totalWindowNum && index < workQueues.Count;
    }

    private static EnumStatus ParseStatus(string statusText)
    {
        if (string.IsNullOrWhiteSpace(statusText)) return EnumStatus.STOP;
        return Enum.TryParse(statusText, out EnumStatus parsed) ? parsed : EnumStatus.STOP;
    }

    private static Dictionary<int, int> HashtableToIntDictionary(Hashtable hashtable)
    {
        var result = new Dictionary<int, int>();
        foreach (DictionaryEntry entry in hashtable)
        {
            if (entry.Key == null || entry.Value == null) continue;
            if (int.TryParse(entry.Key.ToString(), out int key) && int.TryParse(entry.Value.ToString(), out int value)) result[key] = value;
        }
        return result;
    }

    private static Dictionary<string, int> HashtableToDictionary(Hashtable hashtable)
    {
        var result = new Dictionary<string, int>();
        foreach (DictionaryEntry entry in hashtable)
        {
            if (entry.Key == null || entry.Value == null) continue;
            if (int.TryParse(entry.Value.ToString(), out int value)) result[entry.Key.ToString()] = value;
        }
        return result;
    }

    private static Dictionary<int, string> HashtableToIntStringDictionary(Hashtable hashtable)
    {
        var result = new Dictionary<int, string>();
        foreach (DictionaryEntry entry in hashtable)
        {
            if (entry.Key == null || entry.Value == null) continue;
            if (int.TryParse(entry.Key.ToString(), out int key)) result[key] = entry.Value.ToString();
        }
        return result;
    }

    private static Hashtable IntDictionaryToHashtable(Dictionary<int, int> dictionary)
    {
        var hashtable = new Hashtable();
        if (dictionary == null) return hashtable;
        foreach (var item in dictionary)
        {
            hashtable[item.Key] = item.Value;
        }
        return hashtable;
    }

    private static Hashtable DictionaryToHashtable(Dictionary<string, int> dictionary)
    {
        var hashtable = new Hashtable();
        if (dictionary == null) return hashtable;
        foreach (var item in dictionary)
        {
            hashtable[item.Key] = item.Value;
        }
        return hashtable;
    }

    private static Hashtable IntStringDictionaryToHashtable(Dictionary<int, string> dictionary)
    {
        var hashtable = new Hashtable();
        if (dictionary == null) return hashtable;
        foreach (var item in dictionary)
        {
            hashtable[item.Key] = item.Value;
        }
        return hashtable;
    }
}

public class OrderNumber
{
    public int id;
    public List<EnumType> eType;
    public string typeStr = "";

    public OrderNumber(int id, List<EnumType> eType, string t)
    {
        this.id = id;
        this.eType = eType;
        typeStr = t;
    }
}

public class OrderReportObject
{
    public long time;
    public int Window;
    public List<EnumType> eType;
    public int num;

    public OrderReportObject(long time, int Win, List<EnumType> enumType, int num)
    {
        this.time = time;
        Window = Win;
        eType = enumType;
        this.num = num;
    }
}

public class YukinaBoardObject
{
    public int num;
    public string notice = "";

    public YukinaBoardObject(int num)
    {
        this.num = num;
    }

    public void ClearNotice()
    {
        notice = "";
    }

    public void AddNotice(string notice)
    {
        this.notice += "\n";
        this.notice += notice;
    }
}

public class KasumiHighLevelList
{
    private int originalOperator;
    private int tragetOperator;
    private string message;
    private string time;
    public List<EnumType> eType;
    public int num;

    public KasumiHighLevelList(int originalOperator, int tragetOperator, string message, int num)
    {
        this.originalOperator = originalOperator;
        this.tragetOperator = tragetOperator;
        this.message = message;
        time = DateTime.Now.ToString("yyyyMMddHHmmss");
        this.num = num;
    }

    public int getOriginalOperator()
    {
        return originalOperator;
    }

    public int getTragetOperator()
    {
        return tragetOperator;
    }

    public string getMessage()
    {
        return message;
    }

    public int getNum()
    {
        return num;
    }
}
