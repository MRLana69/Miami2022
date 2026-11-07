using System;
using System.IO;
using JetBrains.Annotations;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Utility;

namespace Kiloo.Common.Backend
{
	public class InstallationInfo : IInstallationInfo
	{
		private class InstallationInfoModel
		{
			public string id;
		}

		public const string STORAGE_KEY = ".installationInfo";

		private readonly IStorage _storage;

		private readonly string _deviceId;

		private InstallationInfoModel _info;

		public string id
		{
			get
			{
				return GetInfo().id + _deviceId;
			}
		}

		public InstallationInfo([NotNull] IStorage storage, [NotNull] string deviceId)
		{
			if (storage == null)
			{
				throw new ArgumentNullException("storage");
			}
			if (deviceId == null)
			{
				throw new ArgumentNullException("deviceId");
			}
			_storage = storage;
			_deviceId = deviceId;
		}

		[NotNull]
		private InstallationInfoModel GetInfo()
		{
			if (_info == null && !_storage.TryLoad<InstallationInfoModel>(".installationInfo", out _info))
			{
				_info = new InstallationInfoModel
				{
					id = Guid.NewGuid().ToString("N")
				};
				_storage.Save(".installationInfo", _info);
			}
			return _info;
		}

		public static void DeleteInfo()
		{
			string path = Path.Combine(DeviceUtility.GetUserDataPath(), ".installationInfo");
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}
	}
}
