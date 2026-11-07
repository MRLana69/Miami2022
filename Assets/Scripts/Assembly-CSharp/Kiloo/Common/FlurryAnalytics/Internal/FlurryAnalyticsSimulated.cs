using System.Collections.Generic;

namespace Kiloo.Common.FlurryAnalytics.Internal
{
	internal class FlurryAnalyticsSimulated : IFlurryAnalyticsBridge
	{
		private static FlurryAnalyticsSimulated _instance;

		private FlurryAnalyticsSimulated()
		{
		}

		public static FlurryAnalyticsSimulated CreatePlugin()
		{
			return _instance ?? (_instance = new FlurryAnalyticsSimulated());
		}

		public void StartSession(string apiKey)
		{
		}

		public void EndSession()
		{
		}

		public void LogEvent(string eventName)
		{
		}

		public void LogEventWithParams(string eventName, Dictionary<string, string> parameters)
		{
		}

		public void LogTimedEvent(string eventName)
		{
		}

		public void LogTimedEventWithParams(string eventName, Dictionary<string, string> parameters)
		{
		}

		public void EndTimedEvent(string eventName)
		{
		}

		public void LogPageView()
		{
		}

		public void LogError(string errorId, string message, string errorClass)
		{
		}

		public void SetUserId(string userId)
		{
		}

		public void SetAge(int age)
		{
		}

		public void SetGender(Gender gender)
		{
		}

		public void SetContinueSessionMillis(long milliseconds)
		{
		}

		public void SetCaptureUncaughtExceptions(bool value)
		{
		}

		public void SetDebugLogsEnabled(bool value)
		{
		}
	}
}
