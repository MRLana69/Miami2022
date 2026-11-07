using System;
using SYBO.Components.BuildSizeMonitor;
using SYBO.Components.ComponentsManager;
using SYBO.Components.Json;
using SYBO.Components.Logging;
using UnityEngine;

namespace SYBO.Components.Settings
{
	public static class StaticSettings
	{
		[DebugSetting]
		[SerializeFile("BuildSizeMonitor")]
		public static BuildSizeMonitorSettings BuildSizeMonitor;

		[DebugSetting]
		[SerializeFile("ComponentsManager")]
		[Tooltip("ComponentsManager settings")]
		public static ComponentsManagerSettings ComponentsManager;

		[Tooltip("Log specific settings")]
		[SerializeFile("LogSettings")]
		public static LogSettings Log;

		[Tooltip("Runtime settings for the client")]
		public static RuntimeSettings Runtime;

		[NonSerialized]
		public static bool active;

		static StaticSettings()
		{
			BuildSizeMonitor = new BuildSizeMonitorSettings();
			ComponentsManager = new ComponentsManagerSettings();
			Runtime = new RuntimeSettings();
			SettingsEngine.Load();
		}
	}
}
