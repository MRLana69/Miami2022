using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Kiloo.Common.Utility
{
	public static class PathHandling
	{
		public static string UnifyDirectorySeparators(this string path)
		{
			return path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
		}

		public static string RelativeToAbsolutePath(this string filePath)
		{
			return filePath.RelativeToAbsolutePath(Application.dataPath);
		}

		public static string RelativeToAbsolutePath(this string filePath, string basePath)
		{
			string text = string.Empty;
			string path = filePath ?? string.Empty;
			string path2 = basePath;
			path = path.UnifyDirectorySeparators();
			path2 = path2.UnifyDirectorySeparators();
			List<string> list = path.Split(Path.DirectorySeparatorChar).ToList();
			List<string> list2 = path2.Split(Path.DirectorySeparatorChar).ToList();
			int num = list.Count((string s) => string.Compare(s, "..", StringComparison.Ordinal) == 0);
			if (num == 0)
			{
				return basePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar) + path;
			}
			for (int num2 = 0; num2 < list2.Count() - num; num2++)
			{
				text = text + list2[num2] + Path.DirectorySeparatorChar;
			}
			list.RemoveAll((string s) => string.Compare(s, "..", StringComparison.Ordinal) == 0);
			for (int num3 = 0; num3 < list.Count(); num3++)
			{
				text = ((num3 != 0) ? (text + Path.DirectorySeparatorChar + list[num3]) : (text + list[num3]));
			}
			return text;
		}

		public static string AbsoluteToRelativePath(this string filePath)
		{
			return filePath.AbsoluteToRelativePath(Application.dataPath);
		}

		public static string AbsoluteToRelativePath(this string filePath, string basePath)
		{
			string path = filePath;
			string path2 = basePath;
			string text = string.Empty;
			path = path.UnifyDirectorySeparators();
			path2 = path2.UnifyDirectorySeparators();
			string[] array = path.Split(Path.DirectorySeparatorChar);
			string[] array2 = path2.Split(Path.DirectorySeparatorChar);
			if (path.Contains(path2))
			{
				return path.Replace(path2, string.Empty);
			}
			int num = 0;
			for (int i = 0; i < array.Count(); i++)
			{
				if (string.Compare(array[i], array2[i], StringComparison.Ordinal) != 0)
				{
					num = i;
					break;
				}
			}
			int num2 = array2.Count() - num;
			for (int j = 0; j < num2; j++)
			{
				text = text + ".." + Path.DirectorySeparatorChar;
			}
			for (int k = num; k < array.Count(); k++)
			{
				text = ((k >= array.Count() - 1) ? (text + array[k]) : (text + array[k] + Path.DirectorySeparatorChar));
			}
			return text;
		}

		public static void OpenPathInExplorer(string path)
		{
			bool flag = Directory.Exists(path);
			if (File.Exists(path) || flag)
			{
				switch (Application.platform)
				{
				case RuntimePlatform.WindowsEditor:
					Process.Start("explorer.exe", ((!flag) ? "/select," : string.Empty) + "\"" + path + "\"");
					break;
				case RuntimePlatform.OSXEditor:
					Process.Start("open", ((!flag) ? "-R " : string.Empty) + "'" + path + "'");
					break;
				}
			}
		}
	}
}
