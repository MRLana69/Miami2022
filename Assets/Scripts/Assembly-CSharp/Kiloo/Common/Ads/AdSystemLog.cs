using System.Diagnostics;
using UnityEngine;

namespace Kiloo.Common.Ads
{
	public static class AdSystemLog
	{
		public static bool isRunningTests;

		[Conditional("ENABLE_AD_SYSTEM_LOGS")]
		public static void Log(object message)
		{
		}

		public static void LogWarning(object message)
		{
		}

		public static void LogError(object message)
		{
			if (!isRunningTests)
			{
				SDebug.LogError("[AdSystem] [" + Time.frameCount + "] " + message);
			}
		}
	}
}
