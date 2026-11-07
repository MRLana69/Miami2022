using System;

namespace SYBO.Components.Json
{
	public class JsonReferenceException : Exception
	{
		public JsonReferenceException(string message)
			: base(message)
		{
		}
	}
}
