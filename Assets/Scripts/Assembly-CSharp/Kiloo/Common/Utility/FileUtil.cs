#define ENABLE_ERROR_LOGS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;

namespace Kiloo.Common.Utility
{
	public static class FileUtil
	{
		public static void CopyFileOrDirectory(string from, string to, bool copySubDirs = true)
		{
			FileAttributes attributes = File.GetAttributes(from);
			if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
			{
				CopyDirectory(from, to, copySubDirs);
			}
			else
			{
				File.Copy(from, to);
			}
		}

		public static void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirName);
			if (!directoryInfo.Exists)
			{
				throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}
			FileInfo[] files = directoryInfo.GetFiles();
			FileInfo[] array = files;
			foreach (FileInfo fileInfo in array)
			{
				string destFileName = Path.Combine(destDirName, fileInfo.Name);
				fileInfo.CopyTo(destFileName, false);
			}
			if (copySubDirs)
			{
				DirectoryInfo[] array2 = directories;
				foreach (DirectoryInfo directoryInfo2 in array2)
				{
					string destDirName2 = Path.Combine(destDirName, directoryInfo2.Name);
					CopyDirectory(directoryInfo2.FullName, destDirName2, copySubDirs);
				}
			}
		}

		public static void WriteAllBytes(string filename, byte[] bytes)
		{
			Stream stream = new FileStream(filename, FileMode.Create);
			stream.Write(bytes, 0, bytes.Length);
			stream.Close();
		}

		[NotNull]
		public static byte[] ReadAllBytes(string filename)
		{
			FileInfo fileInfo = new FileInfo(filename);
			long length = fileInfo.Length;
			byte[] array = new byte[length];
			FileStream fileStream = fileInfo.OpenRead();
			fileStream.ReadFully(array);
			fileStream.Close();
			return array;
		}

		public static void SetLastWriteTimeUtc([NotNull] this FileInfo fileInfo, DateTime time)
		{
			fileInfo.LastWriteTimeUtc = time;
		}

		public static DateTime GetLastWriteTimeUtc([NotNull] this FileInfo fileInfo)
		{
			return fileInfo.LastWriteTimeUtc;
		}

		public static bool ArraysAreEqual<T>(T[] a, T[] b)
		{
			if (a == null)
			{
				return b == null;
			}
			if (b == null)
			{
				return false;
			}
			if (a.Length != b.Length)
			{
				return false;
			}
			for (int i = 0; i < a.Length; i++)
			{
				if (!object.Equals(a[i], b[i]))
				{
					return false;
				}
			}
			return true;
		}

		[NotNull]
		public static Dictionary<E, int> ReadEnumIntDictionary<E>(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			Dictionary<E, int> dictionary = new Dictionary<E, int>(num);
			Type typeFromHandle = typeof(E);
			for (int i = 0; i < num; i++)
			{
				string value = reader.ReadString();
				int value2 = reader.ReadInt32();
				E key;
				try
				{
					key = (E)Enum.Parse(typeFromHandle, value, true);
				}
				catch (Exception ex)
				{
					LogError("Enum Parse Failed " + ex.Message);
					continue;
				}
				dictionary[key] = value2;
			}
			return dictionary;
		}

		public static void WriteEnumIntDictionary<E>(BinaryWriter writer, IDictionary<E, int> dict)
		{
			writer.Write(dict.Count);
			foreach (KeyValuePair<E, int> item in dict)
			{
				string name = Enum.GetName(typeof(E), item.Key);
				if (name == null)
				{
					throw new IOException("Key is not enum constant");
				}
				writer.Write(name);
				writer.Write(item.Value);
			}
		}

		[NotNull]
		public static Dictionary<E, string> ReadEnumStringDictionary<E>(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			Dictionary<E, string> dictionary = new Dictionary<E, string>(num);
			Type typeFromHandle = typeof(E);
			for (int i = 0; i < num; i++)
			{
				string value = reader.ReadString();
				string value2 = reader.ReadString();
				if (Enum.IsDefined(typeFromHandle, value))
				{
					E key = (E)Enum.Parse(typeFromHandle, value, true);
					dictionary[key] = value2;
				}
			}
			return dictionary;
		}

		public static void WriteEnumStringDictionary<E>(BinaryWriter writer, IDictionary<E, string> dict)
		{
			writer.Write(dict.Count);
			foreach (KeyValuePair<E, string> item in dict)
			{
				string name = Enum.GetName(typeof(E), item.Key);
				if (name == null)
				{
					throw new IOException("Key is not enum constant");
				}
				writer.Write(name);
				writer.Write(item.Value);
			}
		}

		[NotNull]
		public static Dictionary<string, string> ReadStringStringDictionary(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			Dictionary<string, string> dictionary = new Dictionary<string, string>(num);
			for (int i = 0; i < num; i++)
			{
				string key = reader.ReadString();
				string value = reader.ReadString();
				dictionary[key] = value;
			}
			return dictionary;
		}

		public static void WriteStringStringDictionary(BinaryWriter writer, IDictionary<string, string> dict)
		{
			writer.Write(dict.Count);
			foreach (KeyValuePair<string, string> item in dict)
			{
				writer.Write(item.Key);
				writer.Write(item.Value);
			}
		}

		private static int GetSlotForPath(string path)
		{
			string extension = Path.GetExtension(path);
			int result;
			if (!string.IsNullOrEmpty(extension) && int.TryParse(extension.Substring(1), out result))
			{
				return result;
			}
			return -1;
		}

		public static FileInfo[] GetFilesForPath(string path, bool slotsOnly = false)
		{
			string directoryName = Path.GetDirectoryName(path);
			DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
			if (directoryInfo.Exists)
			{
				string name = Path.GetFileName(path);
				FileInfo[] files = directoryInfo.GetFiles(name + ((!slotsOnly) ? "*" : ".*"));
				int num = files.Length;
				if (num > 0)
				{
					files = Array.FindAll(files, (FileInfo file) =>
					{
						string name2 = file.Name;
						return (!slotsOnly && name2.Equals(name, StringComparison.OrdinalIgnoreCase)) || GetSlotForPath(name2) >= 0;
					});
					Array.Sort(files, (FileInfo a, FileInfo b) => b.GetLastWriteTimeUtc().CompareTo(a.GetLastWriteTimeUtc()));
					return files;
				}
			}
			return new FileInfo[0];
		}

		private static int GetMostRecentSlot(string path, bool fixTimestamps)
		{
			FileInfo[] filesForPath = GetFilesForPath(path);
			int num = filesForPath.Length;
			if (num > 0)
			{
				FileInfo fileInfo = filesForPath[0];
				if (fixTimestamps)
				{
					DateTime utcNow = CDateTime.UtcNow;
					if (fileInfo.GetLastWriteTimeUtc() > utcNow)
					{
						TimeSpan timeSpan = fileInfo.GetLastWriteTimeUtc() - utcNow;
						timeSpan += TimeSpan.FromMinutes(1.0);
						for (int i = 0; i < num; i++)
						{
							filesForPath[i].SetLastWriteTimeUtc(filesForPath[i].GetLastWriteTimeUtc() - timeSpan);
						}
					}
				}
				return GetSlotForPath(fileInfo.Name);
			}
			return -1;
		}

		private static byte[] TryLoadFile(string path, byte[] secretBytes = null)
		{
			try
			{
				byte[] b;
				byte[] array;
				using (FileStream input = new FileStream(path, FileMode.Open))
				{
					BinaryReader binaryReader = new BinaryReader(input);
					int count = binaryReader.ReadInt32();
					b = binaryReader.ReadBytes(count);
					int count2 = binaryReader.ReadInt32();
					array = binaryReader.ReadBytes(count2);
					binaryReader.Close();
				}
				byte[] array2;
				if (secretBytes != null)
				{
					array2 = new byte[secretBytes.Length + array.Length];
					Array.Copy(secretBytes, array2, secretBytes.Length);
					Array.Copy(array, 0, array2, secretBytes.Length, array.Length);
				}
				else
				{
					array2 = array;
				}
				byte[] a = Checksum.ComputeSHA1(array2);
				if (ArraysAreEqual(a, b))
				{
					return array;
				}
			}
			catch (Exception ex)
			{
				LogError("FileUtil.TryLoadFile: " + ex);
			}
			return null;
		}

		public static byte[] Load([NotNull] string path, string secret = null, string alternatePath = null, bool useLastModifiedPathIfNoSlots = false)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			byte[] secretBytes = ((secret != null) ? Encoding.UTF8.GetBytes(secret) : null);
			bool flag = false;
			byte[] array = null;
			useLastModifiedPathIfNoSlots = useLastModifiedPathIfNoSlots && alternatePath != null;
			FileInfo[] filesForPath = GetFilesForPath(path, useLastModifiedPathIfNoSlots);
			FileInfo[] array2 = filesForPath;
			foreach (FileInfo fileInfo in array2)
			{
				array = TryLoadFile(fileInfo.FullName, secretBytes);
				if (array != null)
				{
					break;
				}
				flag = true;
			}
			if (array == null && alternatePath != null)
			{
				FileInfo[] filesForPath2 = GetFilesForPath(alternatePath, useLastModifiedPathIfNoSlots);
				FileInfo[] array3 = filesForPath2;
				foreach (FileInfo fileInfo2 in array3)
				{
					array = TryLoadFile(fileInfo2.FullName, secretBytes);
					if (array != null)
					{
						break;
					}
					flag = true;
				}
			}
			if (array == null && useLastModifiedPathIfNoSlots)
			{
				FileInfo[] array4 = new FileInfo[2]
				{
					new FileInfo(path),
					new FileInfo(alternatePath)
				};
				array4 = Array.FindAll(array4, (FileInfo file) => file.Exists);
				Array.Sort(array4, (FileInfo a, FileInfo b) => b.GetLastWriteTimeUtc().CompareTo(a.GetLastWriteTimeUtc()));
				FileInfo[] array5 = array4;
				foreach (FileInfo fileInfo3 in array5)
				{
					array = TryLoadFile(fileInfo3.FullName, secretBytes);
					if (array != null)
					{
						break;
					}
					flag = true;
				}
			}
			if (array == null)
			{
				if (flag)
				{
					throw new IOException("File is corrupt along with all redundant files");
				}
				throw new FileNotFoundException();
			}
			return array;
		}

		public static void Save([NotNull] string path, [NotNull] byte[] data, string secret = null, int offset = 0, int length = -1, int redundancy = 0, int slots = 0, string alternatePath = null, int alternateRedundancy = 0, int alternateSlots = 0)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (length == -1)
			{
				length = data.Length;
				if (offset > 0)
				{
					length -= offset;
				}
			}
			if (length < 0 || offset + length > data.Length)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			byte[] array;
			if (string.IsNullOrEmpty(secret))
			{
				array = data;
			}
			else
			{
				byte[] bytes = Encoding.UTF8.GetBytes(secret);
				array = new byte[bytes.Length + length];
				Array.Copy(bytes, array, bytes.Length);
				Array.Copy(data, offset, array, bytes.Length, length);
			}
			byte[] array2 = Checksum.ComputeSHA1(array);
			int num = 1 + redundancy;
			if (alternatePath != null)
			{
				num += 1 + alternateRedundancy;
			}
			string[] array3 = new string[num];
			int num2 = 0;
			if (redundancy > 0 || slots > 0)
			{
				int num3 = GetMostRecentSlot(path, true);
				for (int i = 0; i <= redundancy; i++)
				{
					num3++;
					if (num3 >= slots)
					{
						num3 = 0;
					}
					array3[num2++] = path + "." + num3;
				}
			}
			else
			{
				array3[num2++] = path;
			}
			if (alternatePath != null)
			{
				if (alternateRedundancy > 0 || alternateSlots > 0)
				{
					int num4 = GetMostRecentSlot(alternatePath, true);
					for (int j = 0; j <= alternateRedundancy; j++)
					{
						num4++;
						if (num4 >= alternateSlots)
						{
							num4 = 0;
						}
						array3[num2++] = alternatePath + "." + num4;
					}
				}
				else
				{
					array3[num2++] = alternatePath;
				}
			}
			bool flag = false;
			for (int k = 0; k < array3.Length; k++)
			{
				try
				{
					using (FileStream fileStream = new FileStream(array3[k], FileMode.Create))
					{
						BinaryWriter binaryWriter = new BinaryWriter(fileStream);
						binaryWriter.Write(array2.Length);
						binaryWriter.Write(array2);
						binaryWriter.Write(length);
						binaryWriter.Write(data, offset, length);
						fileStream.Close();
						flag = true;
					}
				}
				catch (Exception ex)
				{
					int num5 = array3[k].LastIndexOf('/');
					if (num5 >= 0 && num5 < array3[k].Length - 1)
					{
						num5++;
						string argKey = array3[k].Substring(num5);
						Flurry.LogEventWithAParameter("FileUtil Save Exception", argKey, ex.ToString());
					}
					LogError("Error saving file: " + array3[k] + ": " + ex.Message);
				}
			}
			if (!flag)
			{
				throw new IOException("All attempts to write failed");
			}
		}

		public static void SafeSave([NotNull] string path, [NotNull] byte[] data, string secret = null, int offset = 0, int length = -1)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (length == -1)
			{
				length = data.Length;
				if (offset > 0)
				{
					length -= offset;
				}
			}
			if (length < 0 || offset + length > data.Length)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			byte[] array;
			if (string.IsNullOrEmpty(secret))
			{
				array = data;
			}
			else
			{
				byte[] bytes = Encoding.UTF8.GetBytes(secret);
				array = new byte[bytes.Length + length];
				Array.Copy(bytes, array, bytes.Length);
				Array.Copy(data, offset, array, bytes.Length, length);
			}
			byte[] array2 = Checksum.ComputeSHA1(array);
			string text = path + "_new";
			string text2 = path + "_old";
			string text3 = string.Empty;
			string argValue = string.Empty;
			try
			{
				using (FileStream fileStream = new FileStream(text, FileMode.Create))
				{
					BinaryWriter binaryWriter = new BinaryWriter(fileStream);
					binaryWriter.Write(array2.Length);
					binaryWriter.Write(array2);
					binaryWriter.Write(length);
					binaryWriter.Write(data, offset, length);
					fileStream.Close();
				}
			}
			catch (Exception ex)
			{
				int num = text.LastIndexOf('/');
				if (num >= 0 && num < text.Length - 1)
				{
					num++;
					string text4 = text.Substring(num);
					text3 = "File Write : " + text4;
					argValue = ex.Message;
				}
			}
			try
			{
				if (File.Exists(text))
				{
					if (File.Exists(text2))
					{
						File.Delete(text2);
					}
					if (File.Exists(path))
					{
						File.Move(path, text2);
					}
					File.Move(text, path);
				}
			}
			catch (Exception ex2)
			{
				int num2 = text.LastIndexOf('/');
				if (num2 >= 0 && num2 < text.Length - 1)
				{
					num2++;
					string text5 = text.Substring(num2);
					text3 = "File Rename: " + text5;
					argValue = ex2.Message;
				}
			}
			if (!text3.IsNullOrEmpty())
			{
				Flurry.LogEventWithAParameter("FileUtil SafeSave Exception", text3, argValue);
				throw new IOException("FileUtil SafeSave : Attempt to write failed");
			}
		}

		public static void DebugCorruptFile(string path)
		{
			if (!Globals.DEBUG)
			{
				return;
			}
			using (FileStream fileStream = new FileStream(path, FileMode.Create))
			{
				BinaryWriter binaryWriter = new BinaryWriter(fileStream);
				binaryWriter.Write(1);
				binaryWriter.Write(1);
				binaryWriter.Write(1);
				binaryWriter.Write(new byte[1], 0, 1);
				fileStream.Close();
			}
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		public static void LogError(string msg)
		{
			SDebug.LogError("[FileUtil] " + msg);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		public static void LogWarning(string msg)
		{
		}

		[Conditional("FILEUTIL_DEBUG_LOGS")]
		public static void Log(string msg)
		{
		}
	}
}
