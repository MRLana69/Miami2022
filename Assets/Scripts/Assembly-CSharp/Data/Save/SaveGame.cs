using System;
using System.IO;
using Data.Save.Internal;
using Kiloo.Common.Extensions;

namespace Data.Save
{
	public class SaveGame
	{
		private const string DEBUG_SUFFIX = "_debug";

		private readonly SaveData<LocalData> _local;

		private readonly SaveData<CloudData> _cloud;

		private readonly SaveData<LocalDebugData> _localDebug;

		private readonly Action<DataCorruptionType> _onSaveGameCorrupted;

		public LocalData local
		{
			get
			{
				return _local.diskData;
			}
		}

		public CloudData cloud
		{
			get
			{
				return _cloud.diskData;
			}
		}

		public LocalDebugData localDebug
		{
			get
			{
				return _localDebug.diskData;
			}
		}

		public SaveDataStrings debug_dataSerialized
		{
			get
			{
				SaveDataStrings saveDataStrings = new SaveDataStrings();
				saveDataStrings.local = _local.debug_dataSerialized;
				saveDataStrings.cloud = _cloud.debug_dataSerialized;
				SaveDataStrings saveDataStrings2 = saveDataStrings;
				saveDataStrings2.localDebug = _localDebug.debug_dataSerialized;
				return saveDataStrings2;
			}
			set
			{
				if (!string.IsNullOrEmpty(value.local))
				{
					_local.debug_dataSerialized = value.local;
				}
				if (!string.IsNullOrEmpty(value.cloud))
				{
					_cloud.debug_dataSerialized = value.cloud;
				}
				if (!string.IsNullOrEmpty(value.localDebug))
				{
					_localDebug.debug_dataSerialized = value.localDebug;
				}
			}
		}

		public SaveGame(string localPath, string cloudPath, Action<DataCorruptionType> onSaveFileCorrupted)
		{
			_local = new SaveData<LocalData>(localPath);
			_cloud = new SaveData<CloudData>(cloudPath);
			_localDebug = new SaveData<LocalDebugData>(localPath + "_debug");
			_onSaveGameCorrupted = onSaveFileCorrupted;
		}

		public void Save()
		{
			_local.Save();
			_cloud.Save();
			if (Globals.DEBUG)
			{
				_localDebug.Save();
			}
		}

		public void Load()
		{
			TryLoad(_local, DataCorruptionType.LocalDataCorruption);
			TryLoad(_cloud, DataCorruptionType.CloudDataCorruption);
			TryLoad(_localDebug, DataCorruptionType.LocalDebugDataCorruption);
		}

		public void MigrateToCloud(string newCloudPath)
		{
			_cloud.ChangeSavePath(newCloudPath);
			_cloud.Save();
		}

		private void TryLoad<T>(SaveData<T> data, DataCorruptionType dataCorruptionType) where T : DiskData, new()
		{
			try
			{
				data.Load();
			}
			catch (Exception ex)
			{
				if (ex is IOException && !(ex is FileNotFoundException))
				{
					data.Delete();
					_onSaveGameCorrupted.SafeInvoke(dataCorruptionType);
				}
			}
		}
	}
}
