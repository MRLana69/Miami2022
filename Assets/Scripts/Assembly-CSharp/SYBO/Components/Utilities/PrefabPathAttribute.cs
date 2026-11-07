using System;

namespace SYBO.Components.Utilities
{
	public class PrefabPathAttribute : Attribute
	{
		public string Path { get; private set; }

		public PrefabPathAttribute(string path)
		{
			Path = path;
		}
	}
}
