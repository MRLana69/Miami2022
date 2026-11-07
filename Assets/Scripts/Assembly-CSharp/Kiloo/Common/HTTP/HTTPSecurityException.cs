using System;

namespace Kiloo.Common.HTTP
{
	public class HTTPSecurityException : HTTPException
	{
		public HTTPSecurityException(string message)
			: base(message)
		{
		}

		public HTTPSecurityException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
