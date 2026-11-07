using System;
using JetBrains.Annotations;

namespace NaughtyAttributes
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	[MeansImplicitUse]
	public class ButtonAttribute : DrawerAttribute
	{
		private string text;

		public string Text
		{
			get
			{
				return text;
			}
		}

		public ButtonAttribute(string text = null)
		{
			this.text = text;
		}
	}
}
