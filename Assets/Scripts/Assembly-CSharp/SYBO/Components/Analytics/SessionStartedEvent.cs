using System;
using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics
{
	public class SessionStartedEvent : AnalyticsEvent
	{
		[SerializeField]
		[SerializedAs("session_no")]
		protected int _sessionID;

		public static Func<int> GetIncrementedSessionID;

		public int SessionId
		{
			get
			{
				return _sessionID;
			}
		}

		public override string EventName
		{
			get
			{
				return "session_started";
			}
		}

		public SessionStartedEvent()
		{
			_sessionID = GetIncrementedSessionID();
		}
	}
}
