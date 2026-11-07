using System;

namespace SYBO.Components.Json
{
	public class SerializedSizeLimitAttribute : Attribute
	{
		public readonly bool FailOnLimit;

		public readonly int Size;

		public SerializedSizeLimitAttribute(int size, bool failOnLimit = false)
		{
			Size = size;
			FailOnLimit = failOnLimit;
		}
	}
}
