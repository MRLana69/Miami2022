using System;

namespace Kiloo.Common.HTTP
{
	public class HTTPRequestCanceledException : Exception
	{
		public HTTPRequestCanceledException(string msg)
			: base(msg)
		{
		}
	}
}
