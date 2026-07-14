using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace OrderServer.Controller;

public class GetStatusController : ControllerBase
{
	public int getOperatorID(string s)
	{
		return Convert.ToInt32(s) - 1;
	}

	[Route("GetStatus/{OperatorID}")]
	public async Task<string> GetStatusNumber(string OperatorID)
	{
		EnumStatus a = DataClass.GetWorkStatusD(getOperatorID(OperatorID));
		int b = DataClass.GetCurrentNum(getOperatorID(OperatorID));
		if (a == EnumStatus.STOP)
		{
			return "暂停分配";
		}
		return (b == -1) ? "暂无分配" : b.ToString();
	}
    [Route("GetStatus/GetType/{OperatorID}")]
    public async Task<string> GetStatusNumberType(string OperatorID)
    {
		try
		{
			EnumStatus a = DataClass.GetWorkStatusD(getOperatorID(OperatorID));
			int b = DataClass.GetCurrentNum(getOperatorID(OperatorID));
			if (a == EnumStatus.STOP)
			{
				return "";
			}
			return (b == -1) ? "" : DataClass.GetCurrentNumType(b);
		}
		catch (Exception e)
		{
			return "[HIGHLEVEL]";
		}
    }


	[Route("SetIDs/{num}")]
	public async Task<string> SetIDsIndex(string num)
	{
		try
		{
			return DataClass.TryFixID(Convert.ToInt32(num));
		}
		catch (Exception)
		{
			return "修正失败，请检查是否输入了正确的数字。";
		}
	}

	[Route("SetStatus/{OperatorID}/{status}")]
	public async Task<string> Index1(string OperatorID, string status)
	{
		if (status == "COMPLETED")
		{
			DataClass.CallFinished(getOperatorID(OperatorID));
		}
		return "200 OK!";
	}

	[Route("GetStatus/GenNewUuid/{typeString}")]
	public async Task<string> GenNewUuid(string typeString)
	{
		return DataClass.GenNewUuid(typeString).ToString();
	}
    [Route("GetStatus/GetAllWindows")]
	public async Task<string> GetAllWindows()
	{
		var result = "";
		int maxWindow = DataClass.workQueues.Count;
		for (int i = 1; i <= maxWindow; i++)
		{
			result += i;
			if (i >=1 && i< maxWindow) result += ",";
		}
		return result;
    }
    [Route("GetStatus/GetAllType")]
	public async Task<string> GetAllTypes()
	{
		var a = Enum.GetNames(typeof(EnumType));
		string b = "";
		for (int i = 0; i < a.Length-1; i++)
		{
			b=b + a[i];
			if (i >= 0 && i < a.Length-2) b += ",";
		}
		return b;
	}


    [Route("GetStatus/GenNewHighLevelUuid/{typeString}")]
    public async Task<string> GenNewHighLevelUuid(string typeString)	// 格式是（操作人，目标固定窗口，说明）
    {
		try
		{
			var l = typeString.Split(',');
			int o = Convert.ToInt32(l[0]);
			int t = Convert.ToInt32(l[1]);
			string d = l[2];
			return DataClass.GenHighLevel(o, t, d).ToString();
		}
		catch(Exception e)
		{
			Program.Log("高优先级取号出现错误：" + e.Message);
			return "0";
		}
    }

    [Route("GetStatus/GenNewHighLevelPassUuid/{typeString}")]
    public async Task<string> GenNewHighLevelPassUuid(string typeString)	// 格式是（操作人，目标固定窗口，号码）
    {
        try
        {
            return DataClass.GenHighLevelPass(Convert.ToInt32(typeString)).ToString();
        }
        catch (Exception e)
        {
            Program.Log("高优先级取号出现错误：" + e.Message);
            return "0";
        }
    }



    [Route("GetStatus/GenNewUuid/")]
    public async Task<string> OldGenNewUuid()
    {
		string typeString = "10";
		Program.Log("注意：当前正在使用旧版取号台接口进行取号，目前该接口已废弃，请尽快使用新接口进行取号。");
        return DataClass.GenNewUuid(typeString).ToString();
    }


    [Route("GetStatus/GetVerifyCode/{id}")]
    public async Task<string> Index2(string id)
    {
      
		var t = Program.id2uuid[Convert.ToInt32(id)];
        return t.ToString();
    }

    [Route("GetStatus/FromVerifyCode/{id}")]
    public async Task<string> Index22(string id)
    {

        var t = Program.uuid2id[id];
        return t.ToString();
    }

    [Route("GetStatus/GetCurrentUuid")]
	public async Task<string> GetCurrentUuid()
	{
		return (DataClass.uuid == 0) ? "请先取号" : DataClass.uuid.ToString();
	}
    [Route("GetStatus/GetCurrentId/{id}")]
    public async Task<string> getid(string id)
    {
        int number = Convert.ToInt32(id);
		var result = Program.history[number];
        if (result != null)
		{
			return $"请你现在立马到{result}号窗口办理业务。\n你的号码：{id}";
		}
        int current = 0;
        int.TryParse(Program.currentId, out current);
		return $"当前正在办理的号码为：{Program.currentId}，还有{Math.Max(0, number-current)}人在等待。";
    }

    [Route("CallAction/{a}/{b}")]
	public async Task<string> Index4(string a, string b)
	{
		if (!(b == "STOP"))
		{
			if (b == "START")
			{
				DataClass.CallStart(Convert.ToInt32(a) - 1);
			}
		}
		else
		{
			DataClass.CallStop(Convert.ToInt32(a) - 1);
		}
		return "200";
	}

    [Route("GetStatus/GetNoticBoard/{id}")]
    public async Task<string> GetNoticBoard(string id)
    {
		return "null";
    }

    [Route("GetStatus/GetLog")]
    public async Task<string> GetLog()
    {
		var logs = Program.logs;
		var result = "";
	 foreach (var log in logs)
		{
			result += log;
			result += "\r\n";
		}
	 return result;
    }
    [Route("SetStatus/SetTypes/{Window}/{Type}")]
	public async Task<string> SetTypes(string Window,string Type)
	{
		if (Type == "") return "您至少需要为窗口设置一个业务。";
		DataClass.ChangeType(Convert.ToInt32(Window), Type);
		return "Kasumi：Yeah!!!!!";
	}

    [Route("SetStatus/SetWindowCount/{Count}")]
    public async Task<string> SetWindowCount(string Count)
    {
        DataClass.ResizeWindows(Convert.ToInt32(Count));
        return "窗口数量已设置为 " + DataClass.totalWindowNum;
    }

    [Route("GetStatus/SaveState")]
    public async Task<string> SaveState()
    {
        OrderStateStore.SaveNow();
        return "状态已保存";
    }

    [Route("GetStatus/LoadState")]
    public async Task<string> LoadState()
    {
        return DataClass.LoadStateOrDefault(DataClass.totalWindowNum) ? "已加载上一次归档状态" : "未找到可加载的历史状态";
    }

    [Route("GetStatus/Snapshot")]
    public async Task<string> Snapshot()
    {
        return JsonSerializer.Serialize(DataClass.CreateRealtimeSnapshot());
    }

    [Route("GetWindowType/{id}")]
	public async Task<string> GetWindowType(string id)
	{
		var i = Convert.ToInt32(id);
		var r = DataClass.workQueuesType[i - 1];
		return DataClass.ParserTypeToString(r);
    }
    [Route("GetStatus/GetCurrentLastestOrder")]

	public async Task<string> GetCurrentLastestOrder()	 // 准备 总人数 预计
	{
		return DataClass.LastestOrder + ","+((DataClass.uuid == 0) ? "0" : DataClass.uuid.ToString()) + "," + ((DataClass.uuid == 0) ? "0" : (DataClass.uuid - DataClass.LastestOrderNum).ToString());
	}
    [Route("GetStatus/RemoteGetStatus")]
    public async Task<string> GetSpeed()
	{
        return DataClass.LastestOrderNum + "," + ((DataClass.uuid == 0) ? "0" : DataClass.uuid.ToString())+","+DataClass.reportSpeed;
    }

}
