using System;

namespace SYBO.Components.Json
{
	[AttributeUsage(AttributeTargets.Field)]
	public class SerializeFileAttribute : Attribute
	{
		public string SerializedFilename;

		public SerializeFileAttribute(string serializedFilename)
		{
			SerializedFilename = serializedFilename;
		}
	}
}
