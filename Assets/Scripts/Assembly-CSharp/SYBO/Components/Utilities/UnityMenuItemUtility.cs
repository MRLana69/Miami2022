namespace SYBO.Components.Utilities
{
	public static class UnityMenuItemUtility
	{
		public static string ConvertToMenuPathWithArrows(string path)
		{
			path = FileUtility.ConvertToPathWithBackSlashes(path);
			return path.Replace("\\", "->");
		}
	}
}
