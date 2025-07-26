namespace OrderServer;

public enum EnumStatus
{
	STOP,		// 暂停服务
	STANDBY,	// 待机
	BUSY		// 繁忙（办理中）
}


public enum EnumType	// 细分业务类型窗口
{
	SIGN,			// 签合同
	INFOCHANGE,		// 信息变更
	HIGHLEVEL,		// 高优先级窗口
}
