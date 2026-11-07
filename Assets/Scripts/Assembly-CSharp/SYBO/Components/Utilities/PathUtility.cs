using System.IO;

namespace SYBO.Components.Utilities
{
	public static class PathUtility
	{
		public static string CombinePaths(string[] paths)
		{
			string text = string.Empty;
			for (int i = 0; i < paths.Length; i++)
			{
				text = Path.Combine(text, paths[i]);
			}
			return text;
		}
	}
}
