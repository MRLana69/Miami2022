namespace SYBO.Components.Analytics.Integration
{
	public class SessionTracker
	{
		private int _sessionTimeoutSeconds;

		private long _msTicksWhenLastActive = -1L;

		public SessionTracker(int sessionTimeoutSeconds)
		{
			_sessionTimeoutSeconds = sessionTimeoutSeconds;
			RegisterLastActiveMsTicks();
		}

		public void RegisterLastActiveMsTicks()
		{
			_msTicksWhenLastActive = AnalyticsManager.GetMsTicksSinceDeviceStartup();
		}

		public long GetMSTicksSinceLastActive()
		{
			return AnalyticsManager.GetMsTicksSinceDeviceStartup() - _msTicksWhenLastActive;
		}

		public bool ShouldStartNewSession()
		{
			return ShouldStartNewSession(GetMSTicksSinceLastActive());
		}

		public bool ShouldStartNewSession(long msTicksSinceLastActive)
		{
			long num = _sessionTimeoutSeconds * 1000;
			RegisterLastActiveMsTicks();
			return num < msTicksSinceLastActive;
		}
	}
}
