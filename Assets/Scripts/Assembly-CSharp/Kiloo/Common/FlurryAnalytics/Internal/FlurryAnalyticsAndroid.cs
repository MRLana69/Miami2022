using System.Collections.Generic;
using Kiloo.Common.Serialization;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Kiloo.Common.FlurryAnalytics.Internal
{
	internal class FlurryAnalyticsAndroid : IFlurryAnalyticsBridge
	{
		private static readonly IJsonSerializer _jsonConverter = new KilooSerializer();

		private static FlurryAnalyticsAndroid _instance = null;

		private AndroidJavaClass _bridge;

		private static FlurryAnalyticsAndroid instance
		{
			get
			{
				return _instance ?? (_instance = new FlurryAnalyticsAndroid());
			}
		}

		private FlurryAnalyticsAndroid()
		{
			_bridge = new AndroidJavaClass("com.kiloo.flurryanalytics.FlurryAnalyticsBridge");
		}

		public static FlurryAnalyticsAndroid CreatePlugin()
		{
			return instance;
		}

		public void StartSession(string apiKey)
		{
			_bridge.CallStatic("startSession", apiKey);
		}

		public void EndSession()
		{
			_bridge.CallStatic("endSession");
		}

		public void LogEvent(string eventName)
		{
			_bridge.CallStatic("logEvent", eventName);
		}

		public void LogEventWithParams(string eventName, Dictionary<string, string> parameters)
		{
			string text = _jsonConverter.ToJson(parameters);
			_bridge.CallStatic("logEventWithParams", eventName, text);
		}

		public void LogTimedEvent(string eventName)
		{
			_bridge.CallStatic("logTimedEvent", eventName);
		}

		public void LogTimedEventWithParams(string eventName, Dictionary<string, string> parameters)
		{
			string text = _jsonConverter.ToJson(parameters);
			_bridge.CallStatic("logTimedEventWithParams", eventName, text);
		}

		public void EndTimedEvent(string eventName)
		{
			_bridge.CallStatic("endTimedEvent", eventName);
		}

		public void LogPageView()
		{
			_bridge.CallStatic("onPageView");
		}

		public void LogError(string errorId, string message, string errorClass)
		{
			_bridge.CallStatic("onError", errorId, message, errorClass);
		}

		public void SetUserId(string userId)
		{
			_bridge.CallStatic("setUserId", userId);
		}

		public void SetAge(int age)
		{
			_bridge.CallStatic("setAge", age);
		}

		public void SetGender(Gender gender)
		{
			_bridge.CallStatic("setGender", (int)gender);
		}

		public void SetContinueSessionMillis(long milliseconds)
		{
			_bridge.CallStatic("setContinueSessionMillis", milliseconds);
		}

		public void SetCaptureUncaughtExceptions(bool value)
		{
			_bridge.CallStatic("setCaptureUncaughtExceptions", value);
		}

		public void SetDebugLogsEnabled(bool value)
		{
			_bridge.CallStatic("setDebugLogsEnabled", value);
		}
	}
}
