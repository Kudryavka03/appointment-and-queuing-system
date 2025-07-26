using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;

namespace OrderServer.Controller;

public class GetStatusController : ControllerBase
{
	public int getOperatorID(string s)
	{
		return Convert.ToInt32(s) - 1;
	}

	[Route("GetStatus/{OperatorID}")]
	public async Task<string> Index(string OperatorID)
	{
		EnumStatus a = DataClass.GetWorkStatusD(getOperatorID(OperatorID));
		int b = DataClass.GetCurrentNum(getOperatorID(OperatorID));
		if (a == EnumStatus.STOP)
		{
			return "暂停分配";
		}
		return (b == -1) ? "暂无分配" : b.ToString();
	}

	[Route("SetIDs/{num}")]
	public async Task<string> SetIDsIndex(string num)
	{
		int oldNum = DataClass.uuid;
		try
		{
			int fixNum = Convert.ToInt32(num);
			if (fixNum <= DataClass.uuid)
			{
				return "修正失败，修正号数要比当前号数大。";
			}
			DataClass.FixID(Convert.ToInt32(num));
		}
		catch (Exception)
		{
			return "修正失败，请检查是否输入了正确的数字。";
		}
		Program.Log("修正叫号 " + oldNum + " > " + DataClass.uuid);
		return "修正成功";
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
		List<EnumType> e = DataClass.ParserType(typeString);
		DataClass.tempNums.Add(new OrderNumber(++DataClass.uuid,e, typeString));

        var Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds().ToString();
		Program.uuid2id.Add(Timestamp, DataClass.uuid);
        Program.id2uuid.Add(DataClass.uuid,Timestamp);
        Program.Log("新取号：" + DataClass.uuid + "号 - 校验码："+ Timestamp);
        return (DataClass.uuid).ToString();
	}
    [Route("GetStatus/GenNewHighLevelUuid/{typeString}")]
    public async Task<string> GenNewHighLevelUuid(string typeString)	// 格式是（操作人，目标固定窗口，号码）
    {
		try
		{
			int o = 0;
			int t = 0;
			int n = 0;
			var l = typeString.Split(',');
			o = Convert.ToInt32(l[0]);
			t = Convert.ToInt32(l[1]);
			string d = l[2];
			n = ++DataClass.highLevelInt;
			DataClass.kasumiHighLevelLists.Add(new KasumiHighLevelList(o, t, d, (n + DataClass.highLevelFrontInt)));
			Program.Log("新高优先级取号：" + DataClass.highLevelInt + "号 - 固定窗口分配：" + t + " 操作人：" + o + " 说明：" + d);
			return (n+DataClass.highLevelFrontInt).ToString();
		}
		catch(Exception e)
		{
			Program.Log("高优先级取号出现错误：" + e.Message);
			return "0";
		}
    }

    [Route("GetStatus/GenNewUuid/")]
    public async Task<string> OldGenNewUuid()
    {
		string typeString = "10";
        List<EnumType> e = DataClass.ParserType(typeString);
        DataClass.tempNums.Add(new OrderNumber(++DataClass.uuid, e, typeString));

        var Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds().ToString();
        Program.uuid2id.Add(Timestamp, DataClass.uuid);
        Program.id2uuid.Add(DataClass.uuid, Timestamp);
		Program.Log("注意：当前正在使用旧版取号台接口进行取号，目前该接口已废弃，请尽快使用新接口进行取号。");
        Program.Log("新取号：" + DataClass.uuid + "号 " + $"业务类型：{DataClass.ParserTypeToString(typeString)}" + " - 校验码：" + Timestamp );
        return (DataClass.uuid).ToString();
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
	public async Task<string> Index3()
	{
		return (DataClass.uuid == -1) ? "请先取号" : DataClass.uuid.ToString();
	}
    [Route("GetStatus/GetCurrentId/{id}")]
    public async Task<string> getid(string id)
    {
		var result = Program.history[Convert.ToInt32(id)];
        if (result != null)
		{
			return $"请你现在立马到{result}号窗口办理业务。\n你的号码：{id}";
		}
		return $"当前正在办理的号码为：{Program.currentId}，还有{Convert.ToInt32(id)-(Convert.ToInt32(Program.currentId))}人在等待。";	// 配合前端逻辑。
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

}
