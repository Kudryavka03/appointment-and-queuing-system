namespace OrderServer;

public class WorkQueue
{
	private int currentNum = -1;

	private int nextNum = -1;

	private EnumStatus status;

	private int index;

	private int[] tempList;

	public WorkQueue(int index)
	{
		this.index = index;
	}

	public EnumStatus GetWorkStatusD()
	{
		return status;
	}

	public int GetCurrentNum()
	{
		return currentNum;
	}

	public void CallStop()
	{
		currentNum = -1;
		SetWorkStatus(EnumStatus.STOP);
	}

	public void CallStart()
	{
		SetWorkStatus(EnumStatus.STANDBY);
		CallNextID();
	}

	public void CallNextID()
	{
		if (nextNum > -1 && status != 0)
		{
			currentNum = nextNum;
			nextNum = -1;
			SetWorkStatus(EnumStatus.BUSY);
		}
		else if (nextNum == -1)
		{
			SetWorkStatus(EnumStatus.STANDBY);
			currentNum = -1;
		}
	}

	public void CallFinished()
	{
		if (status != 0)
		{
			SetWorkStatus(EnumStatus.STANDBY);
			currentNum = -1;
		}
	}

	public void SetNextID(int i)
	{
		nextNum = i;
		CallNextID();
	}

	public void SetWorkStatus(EnumStatus e)
	{
		status = e;
		DataClass.SetStatusData(index - 1, e);
	}
}
