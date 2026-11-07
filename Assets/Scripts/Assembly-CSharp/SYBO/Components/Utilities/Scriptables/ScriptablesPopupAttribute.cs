using System;

namespace SYBO.Components.Utilities.Scriptables
{
	public class ScriptablesPopupAttribute : Attribute
	{
		public Type Type;

		public string Namespace;

		public string PathContains;

		public bool HasNoneOption = true;

		public ScriptablesPopupAttribute(Type type)
		{
			Type = type;
		}
	}
}
