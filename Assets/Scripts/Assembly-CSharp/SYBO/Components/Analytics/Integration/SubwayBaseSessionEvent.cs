using System;

namespace SYBO.Components.Analytics.Integration
{
	public abstract class SubwayBaseSessionEvent : BaseSessionEvent, ISubwayBaseEvent
	{
		public SubwayBaseSessionEvent()
		{
		}

		public SubwayBaseSessionEvent(DateTime eventTime, int sessionDelta)
			: base(eventTime, sessionDelta)
		{
		}
	}
}
