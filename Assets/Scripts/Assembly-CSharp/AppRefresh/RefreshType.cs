using System;

namespace AppRefresh
{
	[Flags]
	public enum RefreshType
	{
		UI = 1,
		Data = 2,
		MemoryWarning = 4
	}
}
