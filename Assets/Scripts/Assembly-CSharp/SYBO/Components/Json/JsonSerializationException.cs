using System;

namespace SYBO.Components.Json
{
	public class JsonSerializationException : Exception
	{
		public JsonSerializationException()
			: base("Failed to serialize to JSON.")
		{
		}

		public JsonSerializationException(string message)
			: base(message)
		{
		}
	}
}
