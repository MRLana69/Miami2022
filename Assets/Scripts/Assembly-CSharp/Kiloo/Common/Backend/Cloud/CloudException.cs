using System;

namespace Kiloo.Common.Backend.Cloud
{
	public class CloudException : Exception
	{
		public CloudException(string msg)
			: base(msg)
		{
		}

		public CloudException(string msg, Exception inner)
			: base(msg, inner)
		{
		}
	}
}
