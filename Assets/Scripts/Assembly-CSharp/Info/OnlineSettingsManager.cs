using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Info.Localization;
using Kiloo;
using Kiloo.Common.Backend;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Serialization;
using UnityEngine;

namespace Info
{
	public static class OnlineSettingsManager
	{
		private const string DEBUG_LOGS_TAG = "[OnlineSettingsManager]: ";

		private const string BASE_URL = "https://onlinesettings.kiloo-games.com/";

		private const string FILENAME = "settings";

		private static OnlineSettings<OnlineSettingsData> _onlineSettings = null;

		private static LocalizedOnlineSettings _localizedOnlineSettings = null;

		private static bool _initialized = false;

		private static string _filePath = null;

		public static Action onNewSettingsDownloaded;

		private static readonly Globals.BuildPurpose[] TEST_TIME_BUILDS_SUPPORTED = new Globals.BuildPurpose[3]
		{
			Globals.BuildPurpose.DEV,
			Globals.BuildPurpose.QA,
			Globals.BuildPurpose.REGRESSION
		};

		public static OnlineSettingsData data
		{
			get
			{
				CheckInitialization();
				return _onlineSettings.data;
			}
		}

		public static LocalizedOnlineSettings localizedData
		{
			get
			{
				CheckInitialization();
				return _localizedOnlineSettings;
			}
		}

		public static void Initialize()
		{
			if (_initialized)
			{
				LogWarning("You are trying to manually initialize Online Settings, however they are already initialized.\nIf data is accessed previous to manual initialization, the class will be initialized lazily.");
				return;
			}
			string userDataPath = DeviceUtility.GetUserDataPath();
			_filePath = ((!string.IsNullOrEmpty(userDataPath)) ? Path.Combine(userDataPath, "settings") : null);
			_onlineSettings = new OnlineSettings<OnlineSettingsData>(new Uri("https://onlinesettings.kiloo-games.com/"), _filePath, new KilooSerializer());
			if (Globals.DEBUG_USE_ONLINESETTINGS_SET && !string.IsNullOrEmpty(Globals.ONLINESETTINGS_SET))
			{
				_onlineSettings.parameters.Add("set", Globals.ONLINESETTINGS_SET);
			}
			_onlineSettings.downloaded += OnNewSettingsDownloaded;
			_onlineSettings.Download();
			Strings.languageChanged += InitializeLocalizedSettings;
			InitializeLocalizedSettings();
			_initialized = true;
		}

		public static void DownloadNow()
		{
			if (_initialized && !_onlineSettings.isDownloading)
			{
				if (Globals.DEBUG && TEST_TIME_BUILDS_SUPPORTED.Contains(Globals.BUILD_PURPOSE))
				{
					_onlineSettings.parameters["time"] = WWW.EscapeURL(CDateTime.UtcNow.ToString("u", CultureInfo.InvariantCulture));
				}
				_onlineSettings.Download();
			}
		}

		private static void CheckInitialization()
		{
			if (!_initialized)
			{
				LogWarning("OnlineSettings are not initialized but you are trying to access them. Initializing lazily.\nPlease avoid this scenario by explicitly using the Initialize() method.");
				Initialize();
			}
		}

		private static void OnNewSettingsDownloaded()
		{
			InitializeLocalizedSettings();
			Action action = onNewSettingsDownloaded;
			if (action != null)
			{
				action();
			}
		}

		private static void InitializeLocalizedSettings()
		{
			_localizedOnlineSettings = new LocalizedOnlineSettings(_onlineSettings.data);
		}

		private static void LogWarning(string msg)
		{
			SDebug.LogError("[OnlineSettingsManager]: " + msg);
		}

		[Conditional("ONLINE_SETTINGS_MANAGER_DEBUG_LOGS")]
		private static void Log(string msg)
		{
		}
	}
}
