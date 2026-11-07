using System;

namespace SYBO.Components.Analytics
{
	public class AppLeavesForegroundEvent : BaseSessionEvent
	{
		public override string EventName
		{
			get
			{
				return "app_leaves_foreground";
			}
		}

		public AppLeavesForegroundEvent()
		{
		}

		public AppLeavesForegroundEvent(DateTime eventTime, int sessionDelta)
			: base(eventTime, sessionDelta)
		{
		}
	}
}
