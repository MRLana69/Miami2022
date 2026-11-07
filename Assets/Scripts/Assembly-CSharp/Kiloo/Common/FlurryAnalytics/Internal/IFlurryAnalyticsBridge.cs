using System.Collections.Generic;

namespace Kiloo.Common.FlurryAnalytics.Internal
{
	internal interface IFlurryAnalyticsBridge
	{
		void StartSession(string apiKey);

		void EndSession();

		void LogEvent(string eventName);

		void LogEventWithParams(string eventName, Dictionary<string, string> parameters);

		void LogTimedEvent(string eventName);

		void LogTimedEventWithParams(string eventName, Dictionary<string, string> parameters);

		void EndTimedEvent(string eventName);

		void LogPageView();

		void LogError(string errorId, string message, string errorClass);

		void SetUserId(string userId);

		void SetAge(int age);

		void SetGender(Gender gender);

		void SetContinueSessionMillis(long milliseconds);

		void SetCaptureUncaughtExceptions(bool value);

		void SetDebugLogsEnabled(bool value);
	}
}
