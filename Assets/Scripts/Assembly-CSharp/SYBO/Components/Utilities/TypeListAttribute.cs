using System;

namespace SYBO.Components.Utilities
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class TypeListAttribute : Attribute
	{
		public Type[] Types { get; set; }

		public TypeListAttribute(params Type[] types)
		{
			Types = types;
		}
	}
}
