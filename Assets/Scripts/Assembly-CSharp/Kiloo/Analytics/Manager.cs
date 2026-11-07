using System;
using System.Collections.Generic;
using System.Diagnostics;
using AppRefresh;
using Data.Save;
using Info;
using Kiloo.Common.Backend;
using Kiloo.Common.Plugins.DeviceUtility;
using UnityEngine;

namespace Kiloo.Analytics
{
	public static class Manager
	{
		private const string ONLINESETTINGS_ANALYTICS_ENABLED_KEY = "kiloo_analytics_enabled";

		private const string ONLINESETTINGS_ANALYTICS_SERVER_URL_KEY = "kiloo_analytics_url";

		private const string ONLINESETTINGS_ANALYTICS_SAMPLE_SIZE_KEY = "kiloo_analytics_sample";

		private const string ONLINESETTINGS_ANALYTICS_MAXIMUM_SEND_TIME_KEY = "kiloo_analytics_maximum_send_time";

		private const string ONLINESETTINGS_ANALYTICS_REQUEST_TIME_WINDOW_KEY = "kiloo_analytics_request_time_window";

		private const string ONLINESETTINGS_ANALYTICS_SAVE_ENABLED_KEY = "kiloo_analytics_save_enabled";

		private const string ONLINESETTINGS_ANALYTICS_SEND_ENABLED_KEY = "kiloo_analytics_send_enabled";

		private const bool DEFAULT_ANALYTICS_ENABLED = false;

		private const string DEFAULT_ANALYTICS_SERVER_URL = "http://logger.kiloo-games.com/";

		private const float DEFAULT_ANALYTICS_SAMPLE_SIZE = 0f;

		private const float DEFAULT_ANALYTICS_MAXIMUM_SEND_TIME = 3f;

		private const float DEFAULT_ANALYTICS_REQUEST_TIME_WINDOW = 2f;

		private const bool DEFAULT_ANALYTICS_SAVE_ENABLED = true;

		private const bool DEFAULT_ANALYTICS_SEND_ENABLED = true;

		private const int SESSION_PAUSE_SECONDS = 120;

		private static readonly EventRemoteLogger _eventRemoteLogger;

		private static bool _isPartOfSample;

		private static bool _initialized;

		public static bool isPartOfSample
		{
			get
			{
				return _isPartOfSample;
			}
		}

		public static float userSampleValue
		{
			get
			{
				return AnalyticsSave.analyticsSamplingKey * 100f;
			}
		}

		public static bool isSupportedOnPlatform
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				if (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android)
				{
					if (DeviceUtility.isKindleDevice)
					{
						return false;
					}
					return true;
				}
				return false;
			}
		}

		static Manager()
		{
			_eventRemoteLogger = EventRemoteLogger.instance;
			_isPartOfSample = false;
			_initialized = false;
			if (isSupportedOnPlatform)
			{
				OnlineSettings.instance.AddOnFinishedDownloadingOnlineSettingsHandler(OnFinishedDownloadingOnlineSettings);
				AppRefreshHandler.hasRefreshed += OnHasRefreshed;
			}
		}

		private static void OnHasRefreshed(RefreshType refreshType)
		{
			if (refreshType.HasFlag(RefreshType.Data))
			{
				InitializeOrUpdate();
			}
		}

		public static void LogEvent(EventType eventType, bool overrideSample = false, string extra = null)
		{
			if (!_initialized && overrideSample)
			{
				Initialize(true);
			}
			if (!_initialized || (!isPartOfSample && !overrideSample))
			{
				return;
			}
			Tracker.CreateEvent(eventType, (Event data) =>
			{
				if (data != null && overrideSample)
				{
					UpdateSettings();
				}
				LogEventData(data);
			}, extra);
		}

		private static void LogEventData(Event eventData)
		{
			if (eventData != null)
			{
				LogEvent(eventData.name, eventData.parameters);
			}
		}

		private static void UpdateSettings(bool overrideSample = false)
		{
			_isPartOfSample = overrideSample || Sample();
			Uri baseUri = new Uri(OnlineSettings.instance.GetValue("kiloo_analytics_url", "http://logger.kiloo-games.com/"));
			float value = OnlineSettings.instance.GetValue("kiloo_analytics_maximum_send_time", 3f);
			float value2 = OnlineSettings.instance.GetValue("kiloo_analytics_request_time_window", 2f);
			bool value3 = OnlineSettings.instance.GetValue("kiloo_analytics_save_enabled", true);
			bool value4 = OnlineSettings.instance.GetValue("kiloo_analytics_send_enabled", true);
			_eventRemoteLogger.isPartOfSample = _isPartOfSample;
			_eventRemoteLogger.allowSaving = value3;
			_eventRemoteLogger.allowSending = value4;
			_eventRemoteLogger.SendOnPause(true, value, value2);
			_eventRemoteLogger.SetServiceInfo(baseUri, 120);
			_eventRemoteLogger.CheckSessionRemoval();
		}

		public static void Initialize(bool overrideSample = false)
		{
			if (_initialized || !isSupportedOnPlatform)
			{
				return;
			}
			_isPartOfSample = overrideSample || Sample();
			if (_isPartOfSample)
			{
				UpdateSettings(overrideSample);
				_initialized = true;
				LogEvent(EventType.StaticData);
				LogEvent(EventType.DailyData);
				LogEvent(EventType.SyboUserId);
				_eventRemoteLogger.onResume += () =>
				{
					LogEvent(EventType.DailyData);
				};
			}
		}

		public static void LogEvent(string eventName, Dictionary<string, object> parameters = null)
		{
			if (_initialized && isPartOfSample)
			{
				_eventRemoteLogger.LogEvent(eventName, parameters);
			}
		}

		private static bool Sample()
		{
			if (!OnlineSettings.instance.GetValue("kiloo_analytics_enabled", false))
			{
				return false;
			}
			float value = OnlineSettings.instance.GetValue("kiloo_analytics_sample", 0f);
			if (UserSave.hasMadeOneValidPurchase)
			{
				return true;
			}
			if (UserSave.firstInstallDate == DateTime.MinValue)
			{
				return false;
			}
			float num = userSampleValue;
			return num >= 0f && num <= value;
		}

		private static void OnFinishedDownloadingOnlineSettings()
		{
			InitializeOrUpdate();
		}

		private static void InitializeOrUpdate()
		{
			if (!_initialized)
			{
				Initialize();
			}
			else
			{
				UpdateSettings();
			}
		}

		[Conditional("KILOO_ANALYTICS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("KILOO_ANALYTICS")]
		private static void LogWarn(string msg, UnityEngine.Object context = null)
		{
		}

		private static void LogErr(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("[KilooAnalyticsManager] " + msg, context);
		}
	}
}
