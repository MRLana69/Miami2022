using System;

namespace SYBO.Components.Analytics.Integration
{
	public class SubwayAppLeavesForegroundEvent : SubwayBaseSessionEvent
	{
		public override string EventName
		{
			get
			{
				return "app_leaves_foreground";
			}
		}

		public SubwayAppLeavesForegroundEvent()
		{
		}

		public SubwayAppLeavesForegroundEvent(DateTime eventTime, int sessionDelta)
			: base(eventTime, sessionDelta)
		{
		}
	}
}
