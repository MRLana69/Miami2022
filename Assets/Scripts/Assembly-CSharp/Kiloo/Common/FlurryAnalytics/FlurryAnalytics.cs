using System.Collections.Generic;
using System.Diagnostics;
using Kiloo.Common.FlurryAnalytics.Internal;
using UnityEngine;

namespace Kiloo.Common.FlurryAnalytics
{
	public static class FlurryAnalytics
	{
		private static bool _initialized;

		private static IFlurryAnalyticsBridge _platformBridge;

		private static IFlurryAnalyticsBridge platformBridge
		{
			get
			{
				if (_platformBridge == null)
				{
					switch (Application.platform)
					{
					case RuntimePlatform.Android:
						return _platformBridge = FlurryAnalyticsAndroid.CreatePlugin();
					case RuntimePlatform.IPhonePlayer:
						return _platformBridge = FlurryAnalyticsIOS.CreatePlugin();
					case RuntimePlatform.OSXEditor:
					case RuntimePlatform.WebGLPlayer:
					case RuntimePlatform.WindowsEditor:
						return _platformBridge = FlurryAnalyticsSimulated.CreatePlugin();
					}
					LogError("Not supported on current platform");
				}
				return _platformBridge;
			}
		}

		public static bool isSupportedOnPlatform
		{
			get
			{
				switch (Application.platform)
				{
				case RuntimePlatform.OSXEditor:
				case RuntimePlatform.WebGLPlayer:
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.IPhonePlayer:
				case RuntimePlatform.Android:
					return true;
				default:
					return false;
				}
			}
		}

		public static bool wasInitialized
		{
			get
			{
				return _initialized;
			}
		}

		public static void StartSession(string apiKey)
		{
			if (platformBridge != null)
			{
				platformBridge.StartSession(apiKey);
				_initialized = true;
			}
		}

		public static void EndSession()
		{
			if (_initialized)
			{
				platformBridge.EndSession();
			}
		}

		public static void LogEvent(string eventName)
		{
			if (_initialized)
			{
				platformBridge.LogEvent(eventName);
			}
		}

		public static void LogEventWithParams(string eventName, Dictionary<string, string> parameters)
		{
			if (_initialized)
			{
				platformBridge.LogEventWithParams(eventName, parameters);
			}
		}

		public static void LogTimedEvent(string eventName)
		{
			if (_initialized)
			{
				platformBridge.LogTimedEvent(eventName);
			}
		}

		public static void LogTimedEventWithParams(string eventName, Dictionary<string, string> parameters)
		{
			if (_initialized)
			{
				platformBridge.LogTimedEventWithParams(eventName, parameters);
			}
		}

		public static void EndTimedEvent(string eventName)
		{
			if (_initialized)
			{
				platformBridge.EndTimedEvent(eventName);
			}
		}

		public static void LogPageView()
		{
			if (_initialized)
			{
				platformBridge.LogPageView();
			}
		}

		public static void LogError(string errorId, string message, string errorClass)
		{
			if (_initialized)
			{
				platformBridge.LogError(errorId, message, errorClass);
			}
		}

		public static void SetUserId(string userId)
		{
			if (platformBridge != null)
			{
				platformBridge.SetUserId(userId);
			}
		}

		public static void SetAge(int age)
		{
			if (platformBridge != null)
			{
				platformBridge.SetAge(age);
			}
		}

		public static void SetGender(Gender gender)
		{
			if (platformBridge != null)
			{
				platformBridge.SetGender(gender);
			}
		}

		public static void SetContinueSessionMillis(long milliseconds)
		{
			if (platformBridge != null)
			{
				platformBridge.SetContinueSessionMillis(milliseconds);
			}
		}

		public static void SetCaptureUncaughtExceptions(bool value)
		{
			if (platformBridge != null)
			{
				platformBridge.SetCaptureUncaughtExceptions(value);
			}
		}

		[Conditional("FLURRY_ANALYTICS_LOGS")]
		private static void Log(string msg, Object context = null)
		{
		}

		[Conditional("FLURRY_ANALYTICS_LOGS")]
		private static void EnableNativeLogs(bool value)
		{
			platformBridge.SetDebugLogsEnabled(value);
		}

		private static void LogError(string msg, Object context = null)
		{
			SDebug.LogError("[FlurryAnalytics] " + msg, context);
		}
	}
}
