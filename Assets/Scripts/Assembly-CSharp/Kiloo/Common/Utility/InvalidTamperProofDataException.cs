using System;
using System.IO;

namespace Kiloo.Common.Utility
{
	public class InvalidTamperProofDataException : IOException
	{
		public InvalidTamperProofDataException(string message)
			: base(message)
		{
		}

		public InvalidTamperProofDataException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
