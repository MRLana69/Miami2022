using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using AppRefresh;
using Authentication;
using Cloud;
using Data.Migration;
using Data.ProgressLossLog;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Data.Save
{
	public static class SaveManager
	{
		private const string LOCAL_FILE = "local";

		private const string CLOUD_FILE_PREFIX = "cloud";

		private const string SAVE_FOLDER_NAME = "Save";

		private const string LAST_USED_KILOOTAG_KEY = "CloudSaveLastUsedKilooTag";

		private const string OLD_KILOOTAGS_KEY = "CloudSaveOldKilooTags";

		private static readonly string SAVE_PATH;

		private static readonly string LOCAL_FILE_PATH;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action saveGameChanged__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<DataCorruptionType> saveGameCorrupted__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<bool, bool> saveFileMigrated__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action accountSwitched__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action saveComplete__BackingField;

		private static SaveGame _activeSaveGame;

		private static bool _hasInited;

		public static bool hasInited
		{
			get
			{
				return _hasInited;
			}
		}

		public static event Action saveGameChanged
		{
			add
			{
				Action action = saveGameChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref saveGameChanged__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = saveGameChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref saveGameChanged__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<DataCorruptionType> saveGameCorrupted
		{
			add
			{
				Action<DataCorruptionType> action = saveGameCorrupted__BackingField;
				Action<DataCorruptionType> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref saveGameCorrupted__BackingField, (Action<DataCorruptionType>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<DataCorruptionType> action = saveGameCorrupted__BackingField;
				Action<DataCorruptionType> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref saveGameCorrupted__BackingField, (Action<DataCorruptionType>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<bool, bool> saveFileMigrated
		{
			add
			{
				Action<bool, bool> action = saveFileMigrated__BackingField;
				Action<bool, bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref saveFileMigrated__BackingField, (Action<bool, bool>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<bool, bool> action = saveFileMigrated__BackingField;
				Action<bool, bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref saveFileMigrated__BackingField, (Action<bool, bool>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action accountSwitched
		{
			add
			{
				Action action = accountSwitched__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref accountSwitched__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = accountSwitched__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref accountSwitched__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action saveComplete
		{
			add
			{
				Action action = saveComplete__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref saveComplete__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = saveComplete__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref saveComplete__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		static SaveManager()
		{
			SAVE_PATH = Path.Combine(DeviceUtility.GetUserDataPath(), "Save");
			LOCAL_FILE_PATH = Path.Combine(SAVE_PATH, "local");
			CreateSaveFolderIfNeeded(SAVE_PATH);
			GjApplyUnlockSave();
			Auth.authenticated += OnAuthenticated;
			AppRefreshHandler.refresh += OnAppRefresh;
			saveGameCorrupted += CloudSave.OnSaveGameCorrupted;
			Data.ProgressLossLog.ProgressLossLog.Init();
			SetupSaveGame(false, string.Empty);
		}

		// Save do site (tudo liberado, igual ao unlock-all.js): no navegador é gravado sempre, antes do jogo ler o save,
		// então vale desde a primeira vez que o jogo abre, sem precisar de F5.
		private static void GjApplyUnlockSave()
		{
			if (Application.platform != RuntimePlatform.WebGLPlayer)
			{
				return;
			}
			try
			{
				TextAsset save = Resources.Load<TextAsset>("gj_unlock_save");
				if (save == null)
				{
					return;
				}
				byte[] bytes = save.bytes;
				File.WriteAllBytes(LOCAL_FILE_PATH, bytes);
				File.WriteAllBytes(Path.Combine(SAVE_PATH, "cloud"), bytes);
				string tag = GetSavedKilooTag();
				if (!string.IsNullOrEmpty(tag))
				{
					File.WriteAllBytes(Path.Combine(SAVE_PATH, "cloud" + tag), bytes);
				}
				GjSaveApplied = true;
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[save] não foi possível gravar o save: " + ex.Message);
			}
		}

		public static bool GjSaveApplied { get; private set; }

		private static void OnAppRefresh(RefreshType refreshType)
		{
			if (refreshType.HasFlag(RefreshType.Data))
			{
				Init();
			}
		}

		private static string GetSavedKilooTag()
		{
			return PlayerPrefs.GetString("CloudSaveLastUsedKilooTag", string.Empty);
		}

		public static string GetOldKilooTags()
		{
			return PlayerPrefs.GetString("CloudSaveOldKilooTags", string.Empty);
		}

		public static void Init()
		{
			Load();
			_hasInited = true;
			if (PlayerInfoMigrator.CanMigrate())
			{
				PlayerInfoMigrator.Migrate();
			}
			if (PlayerPrefsMigrator.CanMigrate())
			{
				PlayerPrefsMigrator.Migrate();
			}
			saveGameChanged__BackingField.SafeInvoke();
		}

		private static void SaveKilooTag(string s)
		{
			if (PlayerPrefs.GetString("CloudSaveLastUsedKilooTag", string.Empty) != s)
			{
				string text = PlayerPrefs.GetString("CloudSaveOldKilooTags", string.Empty);
				if (text != string.Empty)
				{
					text += ", ";
				}
				text += PlayerPrefs.GetString("CloudSaveLastUsedKilooTag");
				PlayerPrefs.SetString("CloudSaveOldKilooTags", text);
			}
			PlayerPrefs.SetString("CloudSaveLastUsedKilooTag", s);
			PlayerPrefs.Save();
		}

		public static string GetCurrentCloudFilename()
		{
			return "cloud" + GetSavedKilooTag();
		}

		public static string GetCloudFolderFullPath()
		{
			return SAVE_PATH;
		}

		public static string GetCloudFileFullPath(string kilooTag = "")
		{
			if (kilooTag.IsNullOrEmpty())
			{
				kilooTag = GetKilooTag();
			}
			return Path.Combine(SAVE_PATH, "cloud" + kilooTag);
		}

		private static void SetupSaveGame(bool shouldRemigrate = false, string fakeTag = "")
		{
			string text = GetKilooTag();
			if (!fakeTag.IsNullOrEmpty())
			{
				text = fakeTag;
			}
			bool flag = GetSavedKilooTag().IsNullOrEmpty() && !text.IsNullOrEmpty();
			bool flag2 = flag || shouldRemigrate;
			if (flag2)
			{
				string oldPath = Path.Combine(SAVE_PATH, "cloud" + GetSavedKilooTag());
				string newPath = Path.Combine(SAVE_PATH, "cloud" + text);
				MigrateCloudSaveFile(oldPath, newPath);
				saveFileMigrated__BackingField.SafeInvoke(flag, shouldRemigrate);
			}
			else
			{
				_activeSaveGame = new SaveGame(LOCAL_FILE_PATH, GetCloudFileFullPath(text), saveGameCorrupted__BackingField);
				Load();
			}
			bool flag3 = GetSavedKilooTag() != text;
			SaveKilooTag(text);
			saveGameChanged__BackingField.SafeInvoke();
			if (flag3)
			{
				if (!flag2)
				{
					accountSwitched__BackingField.SafeInvoke();
				}
				StartAppRefreshIfReady();
			}
		}

		private static void StartAppRefreshIfReady()
		{
			if (CloudSave.operationInProgress)
			{
				AppRefreshHandler.QueueAppRefresh();
			}
			else
			{
				AppRefreshHandler.StartRefresh(RefreshType.UI | RefreshType.Data);
			}
		}

		public static string GetKilooTag()
		{
			string text = Auth.kilooTag;
			if (text.IsNullOrEmpty())
			{
				text = GetSavedKilooTag();
			}
			return text;
		}

		private static void MigrateCloudSaveFile(string oldPath, string newPath)
		{
			if (_activeSaveGame == null)
			{
				_activeSaveGame = new SaveGame(LOCAL_FILE_PATH, oldPath, saveGameCorrupted__BackingField);
				Load();
			}
			_activeSaveGame.Save();
			_activeSaveGame.MigrateToCloud(newPath);
		}

		private static void OnAuthenticated()
		{
			if (GetKilooTag() != GetSavedKilooTag() && !PreferencesSave.cloudSaveOptedOut)
			{
				bool shouldRemigrate = !UserSave.cloudSaveEverEnabled;
				SetupSaveGame(shouldRemigrate, string.Empty);
			}
		}

		private static void CreateSaveFolderIfNeeded(string basePath)
		{
			if (!Directory.Exists(basePath))
			{
				Directory.CreateDirectory(basePath);
			}
		}

		public static void Inject<T>(DataManager<T> manager) where T : DataManager<T>, new()
		{
			if (!_hasInited)
			{
				Init();
				if (!Application.isEditor)
				{
					SDebug.LogError("SaveManager lazy initialized while not in editor - this should not happen! " + Utils.stackTrace);
				}
			}
			manager.SetSaveGame(_activeSaveGame);
		}

		public static void Save()
		{
			_activeSaveGame.Save();
			saveComplete__BackingField.SafeInvoke();
		}

		public static void Load()
		{
			_activeSaveGame.Load();
		}

		public static void DebugSaveGameChange(string fakeTag)
		{
			SetupSaveGame(false, fakeTag);
			OnAuthenticated();
		}

		public static void DeleteFolderDebug()
		{
			if (!Globals.DEBUG)
			{
				SDebug.LogError("DEBUG is not enabled - you cannot delete files with current project settings!");
				return;
			}
			string path = Path.Combine(DeviceUtility.GetUserDataPath(), "Save");
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
				_activeSaveGame = new SaveGame(LOCAL_FILE_PATH, GetCloudFileFullPath(GetKilooTag()), saveGameCorrupted__BackingField);
				saveGameChanged__BackingField.SafeInvoke();
			}
		}

		public static void DeleteFilesDebug(bool deleteLocal = true, bool deleteCloud = true)
		{
			if (!Globals.DEBUG)
			{
				SDebug.LogError("DEBUG is not enabled - you cannot delete files with current project settings!");
				return;
			}
			if (deleteLocal)
			{
				FileInfo[] filesForPath = FileUtil.GetFilesForPath(LOCAL_FILE_PATH);
				FileInfo[] array = filesForPath;
				foreach (FileInfo fileInfo in array)
				{
					fileInfo.Delete();
					Debug.Log("File deleted: " + fileInfo.FullName);
				}
			}
			if (deleteCloud)
			{
				FileInfo[] filesForPath2 = FileUtil.GetFilesForPath(GetCloudFileFullPath(GetKilooTag()));
				FileInfo[] array2 = filesForPath2;
				foreach (FileInfo fileInfo2 in array2)
				{
					fileInfo2.Delete();
					Debug.Log("File deleted: " + fileInfo2.FullName);
				}
			}
			_activeSaveGame = new SaveGame(LOCAL_FILE_PATH, GetCloudFileFullPath(GetKilooTag()), saveGameCorrupted__BackingField);
			saveGameChanged__BackingField.SafeInvoke();
		}

		[Conditional("SAVE_GAME_LOGS")]
		private static void Log(string msg, UnityEngine.Object obj = null)
		{
		}

		private static void LogError(string msg, UnityEngine.Object obj = null)
		{
			SDebug.LogError("[SaveGame] " + msg, obj);
		}
	}
}
