using System.Collections.Generic;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Serialization;
using Kiloo.Common.Utility;

namespace Kiloo.Common.FlurryAnalytics.Internal
{
	internal class FlurryAnalyticsIOS : IFlurryAnalyticsBridge
	{
		private static readonly IJsonSerializer _jsonConverter = new KilooSerializer();

		private static FlurryAnalyticsIOS _instance = null;

		private static FlurryAnalyticsIOS instance
		{
			get
			{
				return _instance ?? (_instance = new FlurryAnalyticsIOS());
			}
		}

		private FlurryAnalyticsIOS()
		{
		}

		public static FlurryAnalyticsIOS CreatePlugin()
		{
			return instance;
		}

		private static void flurryStartSession(string apiKey, bool crashReportingEnabled, string appVersion)
		{
		}

		private static void flurrySetUserId(string userId)
		{
		}

		private static void flurrySetAge(int age)
		{
		}

		private static void flurrySetGender(int gender)
		{
		}

		private static void flurryLogEvent(string eventName)
		{
		}

		private static void flurryLogEventWithParams(string eventName, string jsonParams)
		{
		}

		private static void flurryLogTimedEvent(string eventName)
		{
		}

		private static void flurryLogTimedEventWithParams(string eventName, string jsonParams)
		{
		}

		private static void flurryEndTimedEvent(string eventName)
		{
		}

		private static void flurryOnPageView()
		{
		}

		private static void flurrySetContinueSessionMillis(int milliseconds)
		{
		}

		private static void flurrySetDebugLogsEnabled(bool value)
		{
		}

		private static void flurryLogError(string errorName, string message)
		{
		}

		public void StartSession(string apiKey)
		{
			flurryStartSession(apiKey, true, DeviceUtility.GetBundleVersion());
		}

		public void EndSession()
		{
		}

		public void LogEvent(string eventName)
		{
			flurryLogEvent(eventName);
		}

		public void LogEventWithParams(string eventName, Dictionary<string, string> parameters)
		{
			string jsonParams = _jsonConverter.ToJson(parameters);
			flurryLogEventWithParams(eventName, jsonParams);
		}

		public void LogTimedEvent(string eventName)
		{
			flurryLogTimedEvent(eventName);
		}

		public void LogTimedEventWithParams(string eventName, Dictionary<string, string> parameters)
		{
			string jsonParams = _jsonConverter.ToJson(parameters);
			flurryLogTimedEventWithParams(eventName, jsonParams);
		}

		public void EndTimedEvent(string eventName)
		{
			flurryEndTimedEvent(eventName);
		}

		public void LogPageView()
		{
			flurryOnPageView();
		}

		public void LogError(string errorId, string message, string errorClass)
		{
			flurryLogError(errorId, message);
		}

		public void SetUserId(string userId)
		{
			flurrySetUserId(userId);
		}

		public void SetAge(int age)
		{
			flurrySetAge(age);
		}

		public void SetGender(Gender gender)
		{
			flurrySetGender((int)gender);
		}

		public void SetContinueSessionMillis(long milliseconds)
		{
			flurrySetContinueSessionMillis((int)milliseconds);
		}

		public void SetCaptureUncaughtExceptions(bool value)
		{
		}

		public void SetDebugLogsEnabled(bool value)
		{
			flurrySetDebugLogsEnabled(value);
		}
	}
}
