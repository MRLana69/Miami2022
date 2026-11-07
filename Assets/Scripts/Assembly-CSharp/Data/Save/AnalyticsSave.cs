using System;
using System.Collections.Generic;

namespace Data.Save
{
	public class AnalyticsSave : DataManager<AnalyticsSave>
	{
		public static bool hasLoggedStaticData
		{
			get
			{
				return DataManager<AnalyticsSave>.cloud.hasLoggedStaticData;
			}
			set
			{
				DataManager<AnalyticsSave>.cloud.hasLoggedStaticData = value;
			}
		}

		public static float analyticsSamplingKey
		{
			get
			{
				return DataManager<AnalyticsSave>.cloud.analyticsSamplingKey;
			}
			set
			{
				DataManager<AnalyticsSave>.cloud.analyticsSamplingKey = value;
			}
		}

		public static DateTime lastLoggedDailyData
		{
			get
			{
				return DataManager<AnalyticsSave>.cloud.lastLoggedDailyData;
			}
			set
			{
				DataManager<AnalyticsSave>.cloud.lastLoggedDailyData = value;
			}
		}

		public static DateTime abTestLastDailyEventsReportDate
		{
			get
			{
				return DataManager<AnalyticsSave>.cloud.abTestLastDailyEventsReportDate;
			}
			set
			{
				DataManager<AnalyticsSave>.cloud.abTestLastDailyEventsReportDate = value;
			}
		}

		public static string abTestPlayerSeed
		{
			get
			{
				return DataManager<AnalyticsSave>.cloud.abTestPlayerSeed;
			}
			set
			{
				DataManager<AnalyticsSave>.cloud.abTestPlayerSeed = value;
			}
		}

		public static string abTestTagData
		{
			get
			{
				return DataManager<AnalyticsSave>.cloud.abTestTagData;
			}
			set
			{
				DataManager<AnalyticsSave>.cloud.abTestTagData = value;
			}
		}

		public static string flurryUserId
		{
			get
			{
				return DataManager<AnalyticsSave>.cloud.flurryUserId;
			}
			set
			{
				DataManager<AnalyticsSave>.cloud.flurryUserId = value;
			}
		}

		public static string syboUserId
		{
			get
			{
				return DataManager<AnalyticsSave>.cloud.syboAnalyticsUserId;
			}
			set
			{
				DataManager<AnalyticsSave>.cloud.syboAnalyticsUserId = value;
			}
		}

		public static bool hasLoggedGameCenterLogin
		{
			get
			{
				return DataManager<AnalyticsSave>.cloud.hasLoggedGameCenterLogin;
			}
			set
			{
				DataManager<AnalyticsSave>.cloud.hasLoggedGameCenterLogin = value;
			}
		}

		public static bool hasLoggedFacebookLogin
		{
			get
			{
				return DataManager<AnalyticsSave>.cloud.hasLoggedFacebookLogin;
			}
			set
			{
				DataManager<AnalyticsSave>.cloud.hasLoggedFacebookLogin = value;
			}
		}

		public static Dictionary<string, int> hasLoggedFlurryDailyEventSocial
		{
			get
			{
				return DataManager<AnalyticsSave>.cloud.hasLoggedFlurryDailyEventSocial;
			}
			set
			{
				DataManager<AnalyticsSave>.cloud.hasLoggedFlurryDailyEventSocial = value;
			}
		}
	}
}
