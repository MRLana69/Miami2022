using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AppRefresh;
using Data.Migration;
using Data.Save;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Serialization;
using Kiloo.Common.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Data.DebugUtil
{
	public static class SaveGameDebug
	{
		private static string _lastLoadedSave;

		private static string debugSaveDirectory
		{
			get
			{
				return "..\\..\\tools\\OldPlayerInfoVersions".RelativeToAbsolutePath();
			}
		}

		private static string gameSaveDirectory
		{
			get
			{
				return "..\\EditorCache".RelativeToAbsolutePath();
			}
		}

		private static string saveInfoPath
		{
			get
			{
				return Path.Combine(debugSaveDirectory, "active.txt");
			}
		}

		private static string externalSavePath
		{
			get
			{
				return DeviceUtility.GetAndroidExternalSavePath() + "\\Save";
			}
		}

		private static string actualSavePath
		{
			get
			{
				return DeviceUtility.GetUserDataPath() + "\\Save";
			}
		}

		public static string lastLoadedSave
		{
			get
			{
				if (_lastLoadedSave.IsNullOrEmpty())
				{
					if (File.Exists(saveInfoPath))
					{
						_lastLoadedSave = File.ReadAllText(saveInfoPath);
					}
					else
					{
						_lastLoadedSave = "N/A";
					}
				}
				return _lastLoadedSave;
			}
			private set
			{
				_lastLoadedSave = value;
				File.WriteAllText(saveInfoPath, value);
			}
		}

		private static string tmpPlayerInfoPath
		{
			get
			{
				return Path.Combine(Path.GetTempPath(), "tmp_playerinfo.json");
			}
		}

		private static string tmpCloudPath
		{
			get
			{
				return Path.Combine(Path.GetTempPath(), "tmp_cloud.json");
			}
		}

		private static string tmpLocalPath
		{
			get
			{
				return Path.Combine(Path.GetTempPath(), "tmp_local.json");
			}
		}

		public static IEnumerable<string> GetPossibleSaves()
		{
			string saveDir = debugSaveDirectory;
			string[] directories = Directory.GetDirectories(saveDir);
			return directories.Select((string x) => x.Remove(0, saveDir.Length + 1));
		}

		private static void CopySaveFiles(string from, string to)
		{
			try
			{
				if (!Directory.Exists(to))
				{
					Directory.CreateDirectory(to);
				}
				DirectoryInfo directoryInfo = new DirectoryInfo(to);
				FileInfo[] files = directoryInfo.GetFiles();
				foreach (FileInfo fileInfo in files)
				{
					fileInfo.Delete();
				}
				DirectoryInfo[] directories = directoryInfo.GetDirectories();
				foreach (DirectoryInfo directoryInfo2 in directories)
				{
					directoryInfo2.Delete(true);
				}
				string[] directories2 = Directory.GetDirectories(from, "*", SearchOption.AllDirectories);
				foreach (string text in directories2)
				{
					Directory.CreateDirectory(text.Replace(from, to));
				}
				string[] files2 = Directory.GetFiles(from, "*.*", SearchOption.AllDirectories);
				foreach (string text2 in files2)
				{
					File.Copy(text2, text2.Replace(from, to), true);
				}
			}
			catch (FileNotFoundException)
			{
			}
			catch (Exception message)
			{
				Debug.Log(message);
			}
		}

		public static void SwitchSaveGame(string newSave)
		{
			string text = Path.Combine(debugSaveDirectory, newSave);
			string to = gameSaveDirectory;
			CopySaveFiles(text, to);
			lastLoadedSave = newSave;
			if (Application.isPlaying)
			{
				AppRefreshHandler.StartRefresh(RefreshType.UI | RefreshType.Data);
			}
		}

		public static void StoreSaveGame(string saveFolder)
		{
			string text = gameSaveDirectory;
			string to = Path.Combine(debugSaveDirectory, saveFolder);
			CopySaveFiles(text, to);
			Log(string.Format("Stored save [{0}]", saveFolder));
		}

		public static void OpenDebugSaveDirectory()
		{
			PathHandling.OpenPathInExplorer(debugSaveDirectory);
		}

		public static void OpenGameSaveDirectory()
		{
			PathHandling.OpenPathInExplorer(gameSaveDirectory);
		}

		private static void Log(string msg)
		{
		}

		public static void ExportPlayerInfoToExternal()
		{
			string text = actualSavePath;
			string to = externalSavePath;
			try
			{
				CopySaveFiles(text, to);
			}
			catch (Exception ex)
			{
				SDebug.LogError(ex.ToString());
			}
		}

		public static void ImportPlayerInfoFromExternal()
		{
			string to = actualSavePath;
			string text = externalSavePath;
			try
			{
				CopySaveFiles(text, to);
			}
			catch (Exception ex)
			{
				SDebug.LogError(ex.ToString());
			}
			SaveManager.Load();
			AppRefreshHandler.StartRefresh(RefreshType.UI | RefreshType.Data);
		}

		public static void ExportToFiles()
		{
			KilooSerializer serializer = new KilooSerializer();
			string contents = string.Empty;
			try
			{
				contents = serializer.ToJson(PlayerInfoMigrator.LoadPlayerInfoData());
			}
			catch (FileNotFoundException)
			{
			}
			catch (Exception)
			{
			}
			SaveManager.Load();
			string cloud = DebugSave.debug_saveData.cloud;
			string local = DebugSave.debug_saveData.local;
			File.WriteAllText(tmpPlayerInfoPath, contents);
			File.WriteAllText(tmpCloudPath, cloud);
			File.WriteAllText(tmpLocalPath, local);
		}

		public static void ImportFromFiles()
		{
			KilooSerializer serializer = new KilooSerializer();
			PlayerInfoData playerInfoData = serializer.FromJson<PlayerInfoData>(File.ReadAllText(tmpPlayerInfoPath));
			string cloud = File.ReadAllText(tmpCloudPath);
			string local = File.ReadAllText(tmpLocalPath);
			if (playerInfoData != null)
			{
				PlayerInfoMigrator.SavePlayerInfoData(playerInfoData);
			}
			SaveDataStrings saveDataStrings = new SaveDataStrings();
			saveDataStrings.cloud = cloud;
			saveDataStrings.local = local;
			DebugSave.debug_saveData = saveDataStrings;
			SaveManager.Save();
		}

		public static bool FilesExist()
		{
			return File.Exists(tmpPlayerInfoPath) && File.Exists(tmpCloudPath) && File.Exists(tmpLocalPath);
		}

		public static void OpenFiles()
		{
			using (Process.Start(tmpPlayerInfoPath))
			{
			}
			using (Process.Start(tmpCloudPath))
			{
			}
			using (Process.Start(tmpLocalPath))
			{
			}
		}
	}
}
