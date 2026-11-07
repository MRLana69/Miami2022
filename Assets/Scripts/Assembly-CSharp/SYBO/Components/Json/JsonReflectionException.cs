using System;

namespace SYBO.Components.Json
{
	public class JsonReflectionException : Exception
	{
		public JsonReflectionException()
			: base("An error occurred in JSON reflection.")
		{
		}

		public JsonReflectionException(string message)
			: base(message)
		{
		}
	}
}
