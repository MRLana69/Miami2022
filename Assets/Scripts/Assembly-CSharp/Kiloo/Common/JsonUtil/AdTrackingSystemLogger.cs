using Kiloo.Common.Ads;

namespace Kiloo.Common.JsonUtil
{
	public class AdTrackingSystemLogger
	{
		private static AdSystemEventLogger _adSystemEventLogger;

		public static void SetAdSystemEventLogger(AdSystemEventLogger adSystemEventLogger)
		{
			_adSystemEventLogger = adSystemEventLogger;
		}

		public static void LogFlow(AdType adType, AdLocationInfo adLocation, string flowId, string result)
		{
			LogFlow(adType, adLocation.id, AdSystemManager.GetNextProviderInWaterfall(adLocation), flowId, result);
		}

		public static void LogFlow(AdType adType, string adLocationId, string providerTier, string flowId, string result)
		{
			string eventName = string.Format("{0}{1};{2}", GetAdPrefix(adType), adLocationId, flowId);
			string argValue = string.Format("{0};{1}", providerTier, result);
			Flurry.LogEventWithAParameter(eventName, "Outcome", argValue);
		}

		public static void LogAttemptToShowButtonFlow(AdLocationInfo adLocation)
		{
			LogAttemptToShowButtonFlow(adLocation, AdSystemManager.IsCached(adLocation));
		}

		public static void LogAttemptToShowButtonFlow(AdLocationInfo adLocation, bool isCached)
		{
			if (_adSystemEventLogger == null)
			{
				return;
			}
			string result = "Fill";
			CacheFailSource cacheFailSource = _adSystemEventLogger.cacheFailSource;
			if (!isCached)
			{
				switch (cacheFailSource)
				{
				case CacheFailSource.Timeout:
					result = "NoFillCacheTimeout";
					break;
				case CacheFailSource.Waterfall:
					result = "NoFillEndOfWaterfall";
					break;
				default:
					result = "NoFill";
					break;
				}
			}
			LogAttemptToShowButtonFlow(adLocation, result);
		}

		public static void LogAttemptToShowButtonFlow(AdLocationInfo adLocation, string result)
		{
			LogFlow(AdType.Video, adLocation, "Attempt to show button", result);
		}

		public static void LogVideoAdButtonClick(AdLocationInfo adLocation)
		{
			Flurry.LogEventWithAParameter(string.Format("{0}_generic", adLocation.id), AdSystemManager.GetNextProviderInWaterfall(adLocation), "Click");
		}

		public static void LogAttemptToCacheResult(AdType adType, string providerTier, string result)
		{
			string eventName = string.Format("{0}{1}", GetAdPrefix(adType), "AttemptToCacheResult");
			string argValue = string.Format("{0};{1}", providerTier, result);
			Flurry.LogEventWithAParameter(eventName, "Outcome", argValue);
		}

		private static string GetAdPrefix(AdType adType)
		{
			switch (adType)
			{
			case AdType.Interstitial:
				return "int_";
			case AdType.Video:
				return "vid_";
			default:
				return string.Empty;
			}
		}
	}
}
