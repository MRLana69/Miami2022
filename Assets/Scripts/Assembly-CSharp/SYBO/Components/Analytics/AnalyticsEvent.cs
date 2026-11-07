using System;
using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics
{
	public abstract class AnalyticsEvent
	{
		[SerializeField]
		[SerializedSizeLimit(36, false)]
		[SerializedAs("uid")]
		protected string _userID;

		[SerializeField]
		[SerializedSizeLimit(36, false)]
		[SerializedAs("id")]
		protected string _eventID;

		[SerializeField]
		[SerializedAs("client_version")]
		[SerializedSizeLimit(12, true)]
		protected string _gameVersion;

		[SerializeField]
		[SerializedAs("created_at")]
		private string _eventTime;

		public static Func<string> GetGameVersion;

		public static Func<DateTime> GetDeviceStartupTime;

		public static Func<string> GetUserID;

		public static Func<float> GetSamplingKey;

		public string UserId
		{
			get
			{
				return _userID;
			}
		}

		public string EventId
		{
			get
			{
				return _eventID;
			}
		}

		public string GameVersion
		{
			get
			{
				return _gameVersion;
			}
		}

		public string EventTime
		{
			get
			{
				return _eventTime;
			}
		}

		public abstract string EventName { get; }

		public AnalyticsEvent()
			: this(true, GetEventTime())
		{
		}

		protected AnalyticsEvent(bool initializeValues)
			: this(initializeValues, GetEventTime())
		{
		}

		protected AnalyticsEvent(bool initializeValues, DateTime eventTime)
		{
			if (initializeValues)
			{
				_eventID = Guid.NewGuid().ToString();
				_userID = GetUserID();
				_gameVersion = GetGameVersion();
				_eventTime = GetTimeText(eventTime);
			}
		}

		public static DateTime GetEventTime()
		{
			return GetDeviceStartupTime().AddMilliseconds(AnalyticsManager.GetMsTicksSinceDeviceStartup());
		}

		public static string GetTimeText(DateTime time)
		{
			return string.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}.{6:000}Z", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
		}

		public override string ToString()
		{
			return string.Format("{0} for user {1}", GetType().Name, _userID);
		}
	}
}
