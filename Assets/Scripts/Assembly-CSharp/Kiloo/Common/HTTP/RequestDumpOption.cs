using System;

namespace Kiloo.Common.HTTP
{
	[Flags]
	public enum RequestDumpOption
	{
		Status = 1,
		RequestHeaders = 2,
		RequestBody = 4,
		ResponseHeaders = 8,
		ResponseBody = 0x10,
		AllRequest = RequestHeaders | RequestBody,
		AllResponse = ResponseHeaders | ResponseBody,
		All = AllRequest | AllResponse | Status
	}
}
