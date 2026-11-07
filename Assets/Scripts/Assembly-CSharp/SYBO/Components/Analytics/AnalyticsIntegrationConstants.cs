namespace SYBO.Components.Analytics
{
	public class AnalyticsIntegrationConstants
	{
		public const string PrefsKeySamplingKey = "AnalyticsComponent_SamplingKey";

		public const string PrefsKeySessionCounter = "AnalyticsComponent_SessionCounter";

		public const int PrefsKeySessionCounterDefault = -1;

		public const string PrefsKeyUserID = "AnalyticsComponent_UserID";

		public const string PrefsKeyClientVersion = "AnalyticsComponent_ClientVersion";

		public const string PrefsKeyDeviceInfo = "AnalyticsComponent_DeviceInfo";

		public const string PrefsKeySessionEventNumber = "AnalyticsComponent_SessionEventNumber";

		public const int PrefsKeySessionEventNumberDefault = -1;

		public const string PrefsKeyLeavesForegroundTime = "LeavesForegroundTime";

		public static readonly string PrefsKeyLeavesForegroundTimeDefault = string.Empty;

		public const string PrefsKeyLeavesForegroundSessionDelta = "LeavesForegroundSessionDelta";

		public const int PrefsKeyLeavesForegroundSessionDeltaDefault = -1;

		public const int EventJsonDeviceIdFieldSize = 44;
	}
}
