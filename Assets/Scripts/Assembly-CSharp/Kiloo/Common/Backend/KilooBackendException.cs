using System;

namespace Kiloo.Common.Backend
{
	public class KilooBackendException : Exception
	{
		public Error backendError { get; private set; }

		public KilooBackendException(string message)
			: base(message)
		{
		}

		public KilooBackendException(Error error)
			: base(error.ToString())
		{
			backendError = error;
		}
	}
}
