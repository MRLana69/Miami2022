namespace SYBO.Components.Utilities
{
	public static class CopyFolderUtility
	{
		public enum CopyMode
		{
			OverwriteParentAndKeepChildren = 0,
			OverwriteParentAndChildren = 2
		}

		public static bool CopyFolder(string sourcePath, string destinationPath, CopyMode copyMode)
		{
			return false;
		}

		private static bool CopyFilesInFolder(string sourcePath, string destinationPath, bool willOverwriteChildFiles, bool displayProgressBar = true)
		{
			return false;
		}
	}
}
