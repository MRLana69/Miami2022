using System;
using System.Security.Cryptography;
using System.Text;
using SYBO.Components.Json;
using SYBO.Components.Native;
using UnityEngine;

namespace SYBO.Components.Analytics
{
	public abstract class BaseSessionEvent : AnalyticsEvent
	{
		[SerializeField]
		[SerializedAs("session_no")]
		protected int _sessionID;

		[SerializeField]
		[SerializedAs("session_delta")]
		protected int _sessionDelta;

		[SerializeField]
		[SerializedAs("event_no")]
		protected int _sessionEventID;

		public static Func<int> GetSessionID;

		public static Func<int> GetSessionDelta;

		public static Func<int> GetIncrementedSessionEventNumber;

		public static Func<string> GetDeviceID = GetDeviceIDDefault;

		public static Func<string> GetOperatingSystem = GetOperatingSystemDefault;

		public static Func<string> GetDeviceModel = GetDeviceModelDefault;

		public static Func<string> GetISOThreeLetterLanguageCode = GetISOThreeLetterLanguageCodeDefault;

		public int SessionId
		{
			get
			{
				return _sessionID;
			}
		}

		public int SessionDelta
		{
			get
			{
				return _sessionDelta;
			}
		}

		public int SessionEventId
		{
			get
			{
				return _sessionEventID;
			}
		}

		public BaseSessionEvent()
			: this(true)
		{
		}

		protected BaseSessionEvent(DateTime eventTime, int sessionDelta)
			: this(true, eventTime, sessionDelta)
		{
		}

		protected BaseSessionEvent(bool initializeValues)
			: base(initializeValues)
		{
			if (initializeValues)
			{
				InitializeCommon();
				_sessionDelta = GetSessionDelta();
			}
		}

		protected BaseSessionEvent(bool initializeValues, DateTime eventTime, int sessionDelta)
			: base(initializeValues, eventTime)
		{
			if (initializeValues)
			{
				InitializeCommon();
				_sessionDelta = sessionDelta;
			}
		}

		private void InitializeCommon()
		{
			_sessionID = GetSessionID();
			_sessionEventID = GetIncrementedSessionEventNumber();
		}

		private static string GetDeviceIDDefault()
		{
			string deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
			if (!string.IsNullOrEmpty(deviceUniqueIdentifier))
			{
				using (SHA256Managed sHA256Managed = new SHA256Managed())
				{
					byte[] bytes = Encoding.Unicode.GetBytes(deviceUniqueIdentifier);
					byte[] inArray = sHA256Managed.ComputeHash(bytes);
					return Convert.ToBase64String(inArray);
				}
			}
			return null;
		}

		private static string GetDeviceModelDefault()
		{
			return SystemInfo.deviceModel;
		}

		private static string GetOperatingSystemDefault()
		{
			return SystemInfo.operatingSystem;
		}

		private static string GetISOThreeLetterLanguageCodeDefault()
		{
			return Locale.GetISOThreeLetterLanguageCode();
		}
	}
}
