using System;
using System.Collections.Generic;
using System.IO;
using SYBO.Components.Logging;

namespace SYBO.Components.Utilities
{
	public static class FileUtility
	{
		public const string LocalPathPrefix = "file://";

		public static string ConvertToPathWithBackSlashes(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}
			return path.Replace("/", "\\");
		}

		public static string ConvertToPathWithForwardSlashes(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}
			return path.Replace("\\", "/");
		}

		public static bool IsLocalPath(string path)
		{
			path = ConvertToPathWithForwardSlashes(path);
			return !path.StartsWith("http://") && !path.StartsWith("https://") && !path.StartsWith("ftp://");
		}

		public static string ConvertSlashesToSystemSeperatorChar(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}
			return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
		}

		public static string CombinePaths(string pathA, string pathB, bool fixSlashesToMatchSystem = true)
		{
			string text = Path.Combine(pathA, pathB);
			return (!fixSlashesToMatchSystem) ? text : ConvertSlashesToSystemSeperatorChar(text);
		}

		public static string ConvertAbsolutePathToAssetFolderRelativePath(string absolutePath, bool fixSlashesToMatchSystem = true)
		{
			if (string.IsNullOrEmpty(absolutePath))
			{
				return string.Empty;
			}
			if (!Path.IsPathRooted(absolutePath))
			{
			}
			Uri uri = new Uri(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar);
			Uri uri2 = new Uri(absolutePath);
			string text = uri.MakeRelativeUri(uri2).ToString();
			return (!fixSlashesToMatchSystem) ? text : ConvertSlashesToSystemSeperatorChar(text);
		}

		public static string ConvertAssetFolderRelativePathToAbsolutePath(string assetFolderRelativePath, bool fixSlashesToMatchSystem = true)
		{
			if (string.IsNullOrEmpty(assetFolderRelativePath))
			{
				return string.Empty;
			}
			if (Path.IsPathRooted(assetFolderRelativePath))
			{
				return (!fixSlashesToMatchSystem) ? assetFolderRelativePath : ConvertSlashesToSystemSeperatorChar(assetFolderRelativePath);
			}
			if (!assetFolderRelativePath.StartsWith("Assets"))
			{
			}
			return CombinePaths(Directory.GetCurrentDirectory(), assetFolderRelativePath, fixSlashesToMatchSystem);
		}

		public static IEnumerable<string> GetFiles(string directoryPath, string fileSearchPattern)
		{
			Queue<string> queue = new Queue<string>();
			queue.Enqueue(directoryPath);
			while (queue.Count > 0)
			{
				directoryPath = queue.Dequeue();
				try
				{
					string[] directories = Directory.GetDirectories(directoryPath);
					foreach (string item in directories)
					{
						queue.Enqueue(item);
					}
				}
				catch (Exception arg)
				{
					Log.Utilities.LogErrorFormat("{0}", arg);
				}
				string[] files = null;
				try
				{
					files = Directory.GetFiles(directoryPath, fileSearchPattern);
				}
				catch (Exception arg2)
				{
					Log.Utilities.LogErrorFormat("{0}", arg2);
				}
				if (files != null)
				{
					for (int j = 0; j < files.Length; j++)
					{
						yield return files[j];
					}
				}
			}
		}

		public static void MoveDirectory(string path, string newPath)
		{
			if (string.Equals(path, newPath, StringComparison.CurrentCultureIgnoreCase))
			{
				string text = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
				Directory.Move(path, text);
				Directory.Move(text, newPath);
			}
			else
			{
				Directory.Move(path, newPath);
			}
		}
	}
}
