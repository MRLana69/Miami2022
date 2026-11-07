using System;

namespace SYBO.Components.Crypt
{
	public class DecryptionException : Exception
	{
		public DecryptionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
