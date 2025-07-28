using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Cryptography;
using System.Threading;

namespace OrderServer;

public class DataClass
{
	public static string desc = "My favourite Band is Roselia,Poppin Party and Afterglow. Like Yukina Best,Yukina I Love You!!! Kasumi I Love You!!!";
	public static string LastestOrder = "0";
    public static int LastestOrderNum = 0;

    public static readonly int highLevelFrontInt = 1000;
	public static List<EnumStatus> workQueueStatus = new List<EnumStatus>();

	public static List<int> workQueueNum = new List<int>();

    public static List<List<EnumType>> workQueuesType = new List<List<EnumType>>();

    public static List<OrderReportObject> OrderReportObject = new List<OrderReportObject>();

	public static List<KasumiHighLevelList> kasumiHighLevelLists = new List<KasumiHighLevelList>();
    public static object lockKasumiHighLevelLists = new object();

    public static object lockOrderReportObject = new object();

    public static object lockWorkQueueStatus = new object();
    public static object lockWorkQueuesType = new object();

    public static object lockWorkQueue = new object();

	public static List<YukinaBoardObject> YukinaBoard = new List<YukinaBoardObject>();	// 采用追加形式 主要服务于转窗 转个几把，不用了

	public static object lockYukinaBoard = new object();

	public static int totalWindowNum = 0;

	public static List<OrderNumber> tempNums = new List<OrderNumber>();
    public static List<int> giveUpNums = new List<int>();	// 放弃取号

    public static List<WorkQueue> workQueues = new List<WorkQueue>();

	public static int uuid = 0;

	private static Random random = new Random();
	public static int highLevelInt = 0;

	public DataClass(int size)
	{
	}

	public static void ChangeType(int a, string type)
	{
		lock (lockWorkQueuesType)
		{
			workQueuesType[a - 1] = ParserType(type);
			Program.Log($"窗口{a} 业务类型变更: {ParserTypeToString(type)}");
        }
	}
	public static void initData(int size)
	{
		Program.Log("开始初始化窗口数据...");
		for (int i = 0; i < size; i++)
		{
			workQueueStatus.Add(EnumStatus.STOP);
			workQueueNum.Add(-1);
			// EnumType[] e = { EnumType.INFOCHANGE,EnumType.SIGN};	// 默认全业务窗口
			string type = "11";
			workQueuesType.Add(DataClass.ParserType(type));	// 11为默认全业务窗口。
			workQueues.Add(new WorkQueue(size));
			totalWindowNum++;
			Program.Log($"窗口初始化：{i+1}号窗口 业务：{ParserTypeToString(type)} 成功.");
			YukinaBoard.Add(new YukinaBoardObject(i));
            Program.Log($"窗口初始化：{i + 1}号窗口 YukinaBoard 成功.");
        }
        Program.Log("窗口数据初始化成功.");
    }

	public static YukinaBoardObject GetYukinaBoard(int i)
	{
		try
		{
			return YukinaBoard[i];
		}
		catch(Exception e) {
            Program.Log($"获取窗口{i}的YukinaBoard失败：{e.Message}");
            return null; }
	}


	public static int GetLastestNum()
	{
		return tempNums[tempNums.Count].id;
	}

	public static WorkQueue getWorkQueue(int i)
	{
		lock (lockWorkQueue)
		{
			return workQueues[i];
		}
	}
	public static bool WriteReport(OrderReportObject o)	// 写入报表
	{
		lock (lockOrderReportObject)
		{
            OrderReportObject.Add(o);
        }
		return true;
	}
    public static OrderReportObject ReadReport(int i) // 读取报表 By id
    {
        lock (lockOrderReportObject)
        {
            foreach(var o in OrderReportObject)
			{
				if (o.num == i) return o;
			}
        }
        return null;
    }

    public static void SetNextID(int index, int uuid)
	{
		lock (lockWorkQueue)
		{
			workQueues[index].SetNextID(uuid);
			Program.history.Add(uuid, index + 1);
			Program.currentId = uuid.ToString();
        }
	}

	public static int GetCurrentNum(int index)
	{
		lock (lockWorkQueue)
		{
			return workQueues[index].GetCurrentNum();
		}
	}

	public static void CallFinished(int index)
	{
		lock (lockWorkQueue)
		{
			workQueues[index].CallFinished();
			Program.Log("窗口：" + (index + 1) + "窗 呼叫下一号");
		}
	}

	public static void CallStart(int index)
	{
		lock (lockWorkQueue)
		{
			workQueues[index].CallStart();
			Program.Log("窗口继续：" + (index + 1) + "窗");
		}
	}

	public static void CallStop(int index)
	{
		lock (lockWorkQueue)
		{
			workQueues[index].CallStop();
			Program.Log("窗口暂停：" + (index + 1) + "窗");
		}
	}

	public static void FixID(int index)
	{
		uuid = index;
	}

	public static EnumStatus GetWorkStatusD(int index)
	{
		lock (lockWorkQueue)
		{
			return workQueues[index].GetWorkStatusD();
		}
	}

	public static void AddKasumiHighLevelOrder(KasumiHighLevelList kasumiHighLevelList)
	{
		lock (lockKasumiHighLevelLists)
		{
            kasumiHighLevelLists.Add(kasumiHighLevelList);
        }
	}
	public static int Allocator(OrderNumber index)	// 分配器
	{
		
		if (giveUpNums.Contains(index.id)) return 0; // 放弃排队
		List<EnumType> type = index.eType;					// 新取号的业务类型
		List<int> isAvailableWindow = new List<int>();	// 得闲的窗口
		int i = totalWindowNum;
		string msg = "分配器: 窗口空闲情况：";

		{
			List<EnumStatus> tempStatus = workQueueStatus;
			for (int j = 0; j < totalWindowNum; j++)
			{
				if (GetWorkStatusD(j) == EnumStatus.STANDBY && CheckTypeWindowReady(j,index.eType))	// 窗口空闲情况
				{
					isAvailableWindow.Add(j);
					msg = msg + (j+1) + " ";
				}
			}
			if (isAvailableWindow.Count != 0)
			{
				Program.Log(msg);
				int k = getCryptoRandom(0, isAvailableWindow.Count);	// 密码级随机数
				Program.Log("随机数：" + (k + 1) + "   范围：1 - " + isAvailableWindow.Count);
				workQueueStatus[isAvailableWindow[k]] = EnumStatus.STANDBY;
				SetNextID(isAvailableWindow[k], index.id);
				Program.Log("将" + index.id + "号 业务："+ParserTypeToString(index.typeStr)+ " 分配给" + (isAvailableWindow[k] + 1) + "号窗");
				LastestOrderNum++;
                WriteReport(new OrderReportObject(Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss")), isAvailableWindow[k] + 1,index.eType, index.id));
				return 1;
			}
			return 0; 
		}
	}

    public static bool AllocatorHighLevel() // 高优先级分配器
    {
        List<int> isAvailableWindow = new List<int>();  // 得闲的窗口
        for (int i = 0;i< kasumiHighLevelLists.Count; i++)
		{
			int win = kasumiHighLevelLists[i].getTragetOperator()-1;
			int num = kasumiHighLevelLists[i].getNum();
            List<EnumType> enumTypes = new List<EnumType>();
			enumTypes.Add(EnumType.HIGHLEVEL);
            List<EnumStatus> tempStatus = workQueueStatus;
            if (GetWorkStatusD(win) == EnumStatus.STANDBY)	// 实际窗口是需要-1
			{
                SetNextID(win, num);
                Program.Log($"高优先级分配：号码{num} 分配至固定窗口{win+1}");
				string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                WriteReport(new OrderReportObject(Convert.ToInt64(time), win+1, enumTypes, num));
				lock (lockKasumiHighLevelLists)
				{
					kasumiHighLevelLists.RemoveAt(i);   // 移除，性能优化
				}
                return true;
            }
        }
		return false;
    }

    public static int genRamdomNum(int s, int e)
	{
		lock (random)
		{
			return random.Next(s, e);
		}
	}
	public static int getCryptoRandom(int minInclusive, int maxExclusive)	// 确保真随机
	{
		if (minInclusive >= maxExclusive)
			return 0;

        // 计算范围大小
        int range = maxExclusive - minInclusive;
        byte[] randomNumber = new byte[1];
        int maxRejection = 256 - (256 % range); // 拒绝阈值

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

	public static bool CheckTypeWindowReady(int index,List<EnumType> eType)	// 如果后续需要同时办理多个业务的就需要将这里及叫号改为数组
	{
		lock (lockWorkQueuesType)
		{
			foreach (EnumType type in eType)
			{
				if (!workQueuesType[index].Contains(type))
				{
					return false;	// 有一个不符合，都不允许分配，但是通常情况下都不会出现这种情况。因为只要没有办理贷款业务，则就业信息变更是可以自己改的
					// 如果需要办理就业信息变更，则通常情况下说明该生以前是来办理过生源地助学贷款的。22年及之前是需要线下办理，但是23年之后可以通过国家助学贷款app自助申请
					// 此时当就学信息变更完毕后，应该引导该生自己在app中申请续贷。
				}
			}
			return true;
		}
	}
	public static  bool AnyAvailableWindow()
	{
		List<WorkQueue> tmp = null;
		lock (lockWorkQueue)
		{
			tmp = workQueues;
        }
        foreach (var a in tmp)
        {
            if (a.GetWorkStatusD() == EnumStatus.STANDBY) return true;
        }
        return false;

    }

    public static async void Listener()
	{
		Program.Log("开始监听取号队列...");
		int i = 0;  // i 为当前低优先级叫号人数
		List<int> blockList = new List<int>();	// 阻塞的号码，普通优先级中依次优先分配阻塞号码
		while (true)
		{
			try
			{
				if (AnyAvailableWindow())
				{
                    AllocatorHighLevel();   // 先检查高优先级列表是否有分配，如果有则优先分配高优先级
                    int i3 = blockList.Count;
                    for (int i2 = 0; i2 < i3; i2++)
                    {
                        if (Allocator(tempNums[blockList[i2]]) == 1)
                        {
                            blockList.RemoveAt(i2);
                            i3--;
                            i2--;
                        };
                    }
                    if (Allocator(tempNums[i]) == 0)     // 其中一个业务阻塞都会使得分配阻塞
                    {
                        blockList.Add(i);
                    }
                    i++;
                }
			}
			catch (Exception)
			{
			}
			Thread.Sleep(50);
		}
	}

	public static void SetStatusData(int index, EnumStatus enumStatus)
	{
		lock (lockWorkQueueStatus)
		{
			EnumStatus currentWorkQueueStatus = workQueueStatus[index];
			switch (enumStatus)
			{
			case EnumStatus.STOP:
				switch (currentWorkQueueStatus)
				{
				case EnumStatus.STANDBY:
					workQueueStatus[index] = EnumStatus.STOP;
					break;
				case EnumStatus.BUSY:
					workQueueStatus[index] = EnumStatus.STOP;
					break;
				}
				break;
			case EnumStatus.BUSY:
				if (currentWorkQueueStatus == EnumStatus.STOP)
				{
					workQueueStatus[index] = EnumStatus.STANDBY;
				}
				break;
			}
		}
	}

	public static EnumStatus GetStatusData(int index)
	{
		lock (lockWorkQueueStatus)
		{
			return workQueueStatus[index];
		}
	}

	public static void SetWindowNum(int index, int num)
	{
		lock (lockWorkQueueStatus)
		{
			workQueueNum[index] = num;
		}
	}

	public static int GetWindowNum(int index)
	{
		lock (lockWorkQueueStatus)
		{
			return workQueueNum[index];
		}
	}
	public static List<EnumType> ParserType(string a)
	{
		List<EnumType> result = new List<EnumType>();
		for (int i = 0; i < a.Length; i++)
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
            if (i < type.Count()-1) result += ",";

        }
        result += "]";
        return result;
    }

    public static string ParserTypeToString(string a)
    {
		string result = "[";
        for (int i = 0; i < a.Length; i++)
        {
			var b = (EnumType)i;
			if (a[i] == '1')
			{
				if (i>0 && a[i-1] != '0') result += ",";
				result += b.ToString();
			}
			
        }
		result += "]";
        return result;
    }
}

public class OrderNumber
{
	public int id;
	public List<EnumType> eType;
	public string typeStr = "";
	public OrderNumber(int id, List<EnumType> eType,string t)
	{
		this.id = id;
		this.eType = eType;
		this.typeStr = t;
	}
}

public class OrderReportObject	// 用于导出日一次性报表
{
	public long time;			// 时间
	public int Window;			// 窗口
	public List<EnumType> eType;        // 业务类型
	public int num;
	public OrderReportObject(long time,int Win, List<EnumType> enumType,int num)
	{
		this.time = time;
		this.Window = Win;
		this.eType = enumType;
		this.num = num;
	}
}

public class YukinaBoardObject
{
	public int num;				// 窗口号，其实可以不需要
	public string notice ="";       // 通知（转窗通知/管理端发送通知）

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

public class KasumiHighLevelList	// 高优先级窗口。这里只分了两个优先级。该优先级可以指定窗口分配。
{	
	private int originalOperator;		// 转窗人/追加人。0为管理端追加
	private int tragetOperator;			// 目标转窗
	private string message;             // 转窗消息
	private string time;
    public List<EnumType> eType;
	public int num;
    public KasumiHighLevelList(int originalOperator, int tragetOperator,string message,int num)
    {
        this.originalOperator = originalOperator;
        this.tragetOperator = tragetOperator;
		this.message = message;
		time = DateTime.Now.ToString("yyyyMMddHHmmss");
		this.num=num;
    }
	public int getOriginalOperator()
	{

		return originalOperator; 
	}
    public int getTragetOperator()
    {

        return tragetOperator;
    }
	public int getNum() { return num; }
}