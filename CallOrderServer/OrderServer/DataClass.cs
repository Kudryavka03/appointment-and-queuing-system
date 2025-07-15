using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;

namespace OrderServer;

public class DataClass
{
	public static List<EnumStatus> workQueueStatus = new List<EnumStatus>();

	public static List<int> workQueueNum = new List<int>();

	public static object lockWorkQueueStatus = new object();

	public static object lockWorkQueue = new object();

	public static int totalWindowNum = 0;

	public static List<int> tempNums = new List<int>();
    public static List<int> giveUpNums = new List<int>();	// 放弃取号

    public static List<WorkQueue> workQueues = new List<WorkQueue>();

	public static int uuid = -1;

	private static Random random = new Random();

	public DataClass(int size)
	{
	}

	public static void initData(int size)
	{
		for (int i = 0; i < size; i++)
		{
			workQueueStatus.Add(EnumStatus.STOP);
			workQueueNum.Add(-1);
			workQueues.Add(new WorkQueue(size));
			totalWindowNum++;
		}
	}

	public static int GetLastestNum()
	{
		return tempNums[tempNums.Count];
	}

	public static WorkQueue getWorkQueue(int i)
	{
		lock (lockWorkQueue)
		{
			return workQueues[i];
		}
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
			Console.WriteLine("[" + DateTime.Now.ToString() + "] 窗口：" + (index + 1) + "窗 呼叫下一号");
		}
	}

	public static void CallStart(int index)
	{
		lock (lockWorkQueue)
		{
			workQueues[index].CallStart();
			Console.WriteLine("[" + DateTime.Now.ToString() + "] 窗口继续：" + (index + 1) + "窗");
		}
	}

	public static void CallStop(int index)
	{
		lock (lockWorkQueue)
		{
			workQueues[index].CallStop();
			Console.WriteLine("[" + DateTime.Now.ToString() + "] 窗口暂停：" + (index + 1) + "窗");
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

	public static bool allocator(int index)
	{
		if (giveUpNums.Contains(index)) return true;	// 放弃排队
		List<int> isAvailableWindow = new List<int>();	// 得闲的窗口
		int i = totalWindowNum;
		string msg = "[" + DateTime.Now.ToString() + "] 分配器: 窗口空闲情况：";
		lock (lockWorkQueueStatus)
		{
			List<EnumStatus> tempStatus = workQueueStatus;
			for (int j = 0; j < totalWindowNum; j++)
			{
				if (GetWorkStatusD(j) == EnumStatus.STANDBY)
				{
					isAvailableWindow.Add(j);
					msg = msg + j + " ";
				}
			}
			if (isAvailableWindow.Count != 0)
			{
				Console.WriteLine(msg);
				int k = getCryptoRandom(0, isAvailableWindow.Count);	// 密码级随机数
				Console.WriteLine("[" + DateTime.Now.ToString() + "] 随机种子：" + (k + 1) + "   范围：1 - " + isAvailableWindow.Count);
				workQueueStatus[isAvailableWindow[k]] = EnumStatus.STANDBY;
				SetNextID(isAvailableWindow[k], index);
				Console.WriteLine("[" + DateTime.Now.ToString() + "] 将" + index + "号分配给" + (isAvailableWindow[k] + 1) + "号窗");
				return true;
			}
			return false;
		}
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

	public static async void Listener()
	{
		int i = 0;	// i 为当前叫号人数
		while (true)
		{
			try
			{
				if (allocator(tempNums[i]))
				{
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
}
