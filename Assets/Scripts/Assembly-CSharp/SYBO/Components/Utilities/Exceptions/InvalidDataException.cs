using System;

namespace SYBO.Components.Utilities.Exceptions
{
	public class InvalidDataException : Exception
	{
		public InvalidDataException(string message)
			: base(message)
		{
		}
	}
}
