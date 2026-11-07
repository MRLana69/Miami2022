using System;

namespace SYBO.Components.Utilities
{
	public static class StringUtility
	{
		public static string[] SplitStringOnNewlines(string source)
		{
			return source.Split(new string[4]
			{
				"\r\n",
				"\r",
				"\n",
				Environment.NewLine
			}, StringSplitOptions.None);
		}
	}
}
