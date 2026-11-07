using SYBO.Components.Settings;

namespace SYBO.Components.BuildSizeMonitor
{
	public class BuildSizeMonitorSettings
	{
		[DebugSetting]
		public string ProjectId = string.Empty;

		[DebugSetting]
		public string BaseLine = string.Empty;

		[DebugSetting]
		public string Server = "http://build.sybogames.local:8080";
	}
}
