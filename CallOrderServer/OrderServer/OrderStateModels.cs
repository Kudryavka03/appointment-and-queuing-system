using System.Collections.Generic;

namespace OrderServer;

public class PersistedOrderState
{
    public int Version { get; set; } = 1;
    public string SavedAt { get; set; } = "";
    public string LastestOrder { get; set; } = "0";
    public int LastestOrderNum { get; set; }
    public int Speed { get; set; }
    public int ReportSpeed { get; set; }
    public int Uuid { get; set; }
    public int HighLevelInt { get; set; }
    public int TotalWindowNum { get; set; }
    public int AllocatorCursor { get; set; }
    public string CurrentId { get; set; } = "0";
    public List<int> AllocatorBlockList { get; set; } = new List<int>();
    public List<WorkQueueState> WorkQueues { get; set; } = new List<WorkQueueState>();
    public List<string> WorkQueueTypeMasks { get; set; } = new List<string>();
    public List<OrderNumberState> TempNums { get; set; } = new List<OrderNumberState>();
    public List<int> GiveUpNums { get; set; } = new List<int>();
    public List<OrderReportState> Reports { get; set; } = new List<OrderReportState>();
    public List<HighLevelOrderState> HighLevelOrders { get; set; } = new List<HighLevelOrderState>();
    public Dictionary<int, string> Id2TypeMasks { get; set; } = new Dictionary<int, string>();
    public Dictionary<int, int> History { get; set; } = new Dictionary<int, int>();
    public Dictionary<string, int> Uuid2Id { get; set; } = new Dictionary<string, int>();
    public Dictionary<int, string> Id2Uuid { get; set; } = new Dictionary<int, string>();
    public List<string> Logs { get; set; } = new List<string>();
}

public class WorkQueueState
{
    public int CurrentNum { get; set; } = -1;
    public int NextNum { get; set; } = -1;
    public string Status { get; set; } = EnumStatus.STOP.ToString();
}

public class OrderNumberState
{
    public int Id { get; set; }
    public string TypeMask { get; set; } = "";
}

public class OrderReportState
{
    public long Time { get; set; }
    public int Window { get; set; }
    public string TypeMask { get; set; } = "";
    public int Num { get; set; }
}

public class HighLevelOrderState
{
    public int OriginalOperator { get; set; }
    public int TargetOperator { get; set; }
    public string Message { get; set; } = "";
    public int Num { get; set; }
}

public class RealtimeOrderSnapshot
{
    public string MessageType { get; set; } = "snapshot";
    public string ServerTime { get; set; } = "";
    public string LastestOrder { get; set; } = "0";
    public int LastestOrderNum { get; set; }
    public int Uuid { get; set; }
    public int WaitingCount { get; set; }
    public int ReportSpeed { get; set; }
    public int TotalWindowNum { get; set; }
    public bool HasArchivedState { get; set; }
    public string ArchivedStatePath { get; set; } = "";
    public List<string> TypeNames { get; set; } = new List<string>();
    public List<RealtimeWindowSnapshot> Windows { get; set; } = new List<RealtimeWindowSnapshot>();
    public List<string> Logs { get; set; } = new List<string>();
}

public class RealtimeWindowSnapshot
{
    public int Window { get; set; }
    public string Status { get; set; } = EnumStatus.STOP.ToString();
    public int CurrentNum { get; set; } = -1;
    public string DisplayText { get; set; } = "暂停服务";
    public string TypeText { get; set; } = "[]";
    public string TypeMask { get; set; } = "";
    public bool IsBusy { get; set; }
    public bool IsPaused { get; set; }
}
