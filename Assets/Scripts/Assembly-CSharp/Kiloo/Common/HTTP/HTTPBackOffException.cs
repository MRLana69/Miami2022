using System;

namespace Kiloo.Common.HTTP
{
	public class HTTPBackOffException : HTTPException
	{
		public HTTPBackOffException(string message)
			: base(message)
		{
		}

		public HTTPBackOffException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
