using System.Linq;

namespace OrderServer;

public class WorkQueue
{
    private int currentNum = -1;
    private int nextNum = -1;
    private EnumStatus status = EnumStatus.STOP;
    private EnumType[] type;
    private readonly int index;
    private int[] tempList;

    public WorkQueue(int index)
    {
        this.index = index;
    }

    public WorkQueue(int index, WorkQueueState state)
    {
        this.index = index;
        currentNum = state?.CurrentNum ?? -1;
        nextNum = state?.NextNum ?? -1;
        status = ParseStatus(state?.Status);
    }

    public WorkQueueState CreateState()
    {
        return new WorkQueueState
        {
            CurrentNum = currentNum,
            NextNum = nextNum,
            Status = status.ToString()
        };
    }

    public EnumStatus GetWorkStatusD()
    {
        return status;
    }

    public bool GetReadyEnumType(EnumType eTypeTest)
    {
        return type != null && type.Contains(eTypeTest);
    }

    public EnumType[] GetWorkType()
    {
        return type;
    }

    public int GetCurrentNum()
    {
        return currentNum;
    }

    public void CallStop()
    {
        currentNum = -1;
        nextNum = -1;
        SetWorkStatus(EnumStatus.STOP);
    }

    public void CallStart()
    {
        var currentStatus = GetWorkStatusD();
        if (currentStatus == EnumStatus.STANDBY) return;
        if (currentStatus == EnumStatus.BUSY) return;

        SetWorkStatus(EnumStatus.STANDBY);
        CallNextID();
    }

    public void CallNextID()
    {
        if (nextNum > -1 && status != EnumStatus.STOP)
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
        if (status != EnumStatus.STOP)
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

    private static EnumStatus ParseStatus(string statusText)
    {
        if (string.IsNullOrWhiteSpace(statusText)) return EnumStatus.STOP;
        return System.Enum.TryParse(statusText, out EnumStatus parsed) ? parsed : EnumStatus.STOP;
    }
}
