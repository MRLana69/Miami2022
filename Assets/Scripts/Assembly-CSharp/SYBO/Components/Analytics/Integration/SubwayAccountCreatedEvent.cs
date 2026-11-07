using System;
using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class SubwayAccountCreatedEvent : AccountCreatedEvent, ISubwayBaseEvent
	{
		[SerializeField]
		[SerializedSizeLimit(36, false)]
		[SerializedAs("uid_kiloo")]
		protected string KilooUserID;

		[SerializeField]
		[SerializedAs("first_startup_at")]
		protected string FirstStartupAt;

		public string KilooId
		{
			get
			{
				return KilooUserID;
			}
		}

		public SubwayAccountCreatedEvent(string kilooUserID, string platformConfig, DateTime firstStartupAt)
		{
			KilooUserID = kilooUserID;
			FirstStartupAt = AnalyticsEvent.GetTimeText(firstStartupAt);
		}
	}
}
