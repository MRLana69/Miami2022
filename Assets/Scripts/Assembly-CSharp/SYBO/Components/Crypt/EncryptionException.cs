using System;

namespace SYBO.Components.Crypt
{
	public class EncryptionException : Exception
	{
		public EncryptionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
