using SYBO.Components.Analytics;
using SYBO.Components.BuildSizeMonitor;
using SYBO.Components.ComponentsManager;
using SYBO.Components.Compression;
using SYBO.Components.Crypt;
using SYBO.Components.EditorExtensions;
using SYBO.Components.Json;
using SYBO.Components.Native;
using SYBO.Components.Settings;
using SYBO.Components.Singletons;
using SYBO.Components.Utilities;

namespace SYBO.Components.Logging
{
	public static class Log
	{
		public static AnalyticsLogCategory Analytics;

		public static BuildSizeMonitorLogCategory BuildSizeMonitor;

		public static ComponentsManagerLogCategory ComponentsManager;

		public static CompressionLogCategory Compression;

		public static CryptLogCategory Crypt;

		public static EditorExtensionsLogCategory EditorExtensions;

		public static JsonLogCategory Json;

		public static LogDefault Default;

		public static LogSystemLogCategory LogSystem;

		public static NativeLogCategory Native;

		public static SettingsLogCategory Settings;

		public static SingletonsLogCategory Singletons;

		public static PoolManagerLogCategory PoolManager;

		public static UtilitiesCategory Utilities;

		static Log()
		{
			Singleton<LogManager>.GetInstance();
		}
	}
}
