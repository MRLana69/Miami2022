using System;

namespace SYBO.Components.Json
{
	public class SerializedAsAttribute : Attribute
	{
		public readonly string SerializedAsName;

		public SerializedAsAttribute(string serializeAsName)
		{
			SerializedAsName = serializeAsName;
		}
	}
}
