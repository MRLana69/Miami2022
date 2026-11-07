using System;

namespace SYBO.Components.Json
{
	public class SizeLimitException : Exception
	{
		public SizeLimitException(string message)
			: base(message)
		{
		}
	}
}
