using System;
using System.Collections.Generic;
using System.Diagnostics;
using ABTest;
using Data.Save;
using InAppPurchase;
using Info;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Kiloo.Analytics
{
	internal static class Tracker
	{
		internal const string EVENT_NAME_RUN_ENDED = "run_ended";

		internal const string PARAMETER_RUN_LENGTH = "len_sec";

		internal const string PARAMETER_SAVE_ME_COUNT_IN_RUN = "save_count";

		internal const string PARAMETER_RUN_LENGTH_UNTIL_FIRST_SAVE_ME = "len_save";

		internal const string PARAMETER_BASE_MULTIPLIER_START_OF_RUN = "multiply";

		internal const string PARAMETER_HOVERBOARD_COUNT_IN_RUN = "hovb_count";

		internal const string PARAMETER_RUN_WAS_ABORTED = "aborted";

		internal const string PARAMETER_MYSTERY_BOXES_COLLECTED_IN_RUN = "MB_cnt";

		internal const string PARAMETER_FIRST_MYSTERY_BOX_COLLECTED_IN_RUN = "MB1_sec";

		internal const string PARAMETER_DAILY_LETTERS_COLLECTED_IN_RUN = "d_let_cnt";

		internal const string PARAMETER_CHARACTER_LETTERS_COLLECTED_IN_RUN = "c_let_cnt";

		private const string EVENT_NAME_STATIC_DATA = "static";

		private const string PARAMETER_DATA_INSTALL_DATE = "inst_date";

		private const string PARAMETER_DATA_FLURRY_ID = "flurry";

		private const string EVENT_NAME_DAILY_DATA = "daily";

		private const string PARAMETER_DEVICE_LOCALE = "locale";

		private const string EVENT_NAME_MADE_ONE_PURCHASE = "one_iap";

		private const string EVENT_NAME_IN_APP_PURCHASE = "iap_val";

		private const string PARAMETER_IAP_PRODUCT_ID = "pid";

		private const string PARAMETER_IAP_PRODUCT_LOCALIZED_PRICE = "loc_price";

		private const string PARAMETER_IAP_INSTALL_DATE = "inst_date";

		private const string EVENT_NAME_ABTEST_DAILY_REPORT = "abtest_day";

		private const string PARAMETER_ABTEST_IDENTIFIER = "id";

		private const string PARAMETER_ABTEST_PAYLOAD = "pl";

		private const string EVENT_NAME_SYBO_USER_ID = "sybo_id";

		private const string PARAMETER_SYBO_USER_ID = "uid";

		private const string EVENT_NAME_PLAYER_BIRTHDAY = "bday";

		private const string PARAMETER_PLAYER_BIRTHDAY_YEAR = "year";

		private const string PARAMETER_PLAYER_BIRTHDAY_MONTH = "month";

		private const string EVENT_NAME_CONSENT_GIVEN = "cons_give";

		private const string EVENT_NAME_CONSENT_WITHDRAWN = "cons_with";

		private const string PARAMETER_CONSENT_RESULT = "res";

		private const string EVENT_NAME_CONSENT_PRIVACY_POLICY_LINK_OPENED = "cons_privacy";

		private static Event GetStaticData()
		{
			if (AnalyticsSave.hasLoggedStaticData)
			{
				return null;
			}
			AnalyticsSave.hasLoggedStaticData = true;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("inst_date", UserSave.firstInstallDate.ToJSON());
			dictionary.Add("flurry", Flurry.GetFlurryUserIdHash());
			Dictionary<string, object> parameters = dictionary;
			return new Event("static", parameters);
		}

		private static Event GetDailyData()
		{
			if (IsDateToday(AnalyticsSave.lastLoggedDailyData))
			{
				return null;
			}
			AnalyticsSave.lastLoggedDailyData = CDateTime.UtcNow;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("locale", DeviceUtility.GetLocale());
			Dictionary<string, object> parameters = dictionary;
			return new Event("daily", parameters);
		}

		private static Event GetSyboUserId()
		{
			if (SyboAnalyticsSave.kilooAnalyticsHasLoggedSyboUserId)
			{
				return null;
			}
			string syboAnalyticsUserId = SyboAnalyticsSave.syboAnalyticsUserId;
			if (!string.IsNullOrEmpty(syboAnalyticsUserId) && SyboAnalyticsSave.isSyboAnalyticsUserIdAssigned)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("uid", syboAnalyticsUserId);
				Dictionary<string, object> parameters = dictionary;
				SyboAnalyticsSave.kilooAnalyticsHasLoggedSyboUserId = true;
				return new Event("sybo_id", parameters);
			}
			return null;
		}

		private static Event GetSingleValidPurchase()
		{
			return new Event("one_iap");
		}

		private static Event GetValidInAppPurchase(string productId)
		{
			if (productId.IsNullOrEmpty())
			{
				SDebug.LogError("Product ID not set");
				return null;
			}
			string text = "NULL";
			ProductRequestResultInfo productInfo;
			if (InAppPurchaseProvider.instance.TryGetLatestRequestedProductInfoForProductId(productId, out productInfo))
			{
				text = productInfo.localizedPrice;
			}
			else
			{
				SDebug.LogError("No localized price found for productId, localizedPrice will have default value: \"" + text + "\"");
			}
			productId = productId.Replace("com.kiloo.subways.", string.Empty).Replace("com.kiloo.subwaysurfers.", string.Empty).Replace("com.kiloo.subway", string.Empty);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("pid", productId);
			dictionary.Add("loc_price", text);
			dictionary.Add("inst_date", UserSave.firstInstallDate.ToString("yyyy-MM-dd"));
			Dictionary<string, object> parameters = dictionary;
			return new Event("iap_val", parameters);
		}

		private static Event GetPlayerBirthday()
		{
			DateTime userBirthday = AgeRestriction.GetUserBirthday();
			if (userBirthday != default(DateTime))
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("year", userBirthday.Year);
				dictionary.Add("month", userBirthday.Month);
				Dictionary<string, object> parameters = dictionary;
				return new Event("bday", parameters);
			}
			return null;
		}

		private static Event GetConsentGivenEvent()
		{
			return new Event("cons_give");
		}

		private static Event GetConsentWithdrawnEvent(string result)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("res", result);
			Dictionary<string, object> parameters = dictionary;
			return new Event("cons_with", parameters);
		}

		private static Event GetConsentPrivacyLinkClickedEvent()
		{
			return new Event("cons_privacy");
		}

		private static Event GetABTestDailyReportValue(string identifier)
		{
			string payloadString = ABTestManager.GetPayloadString(identifier);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("id", identifier);
			dictionary.Add("pl", payloadString);
			Dictionary<string, object> parameters = dictionary;
			return new Event("abtest_day", parameters);
		}

		public static void CreateEvent(EventType eventType, Action<Event> onLogReady, string extra = null)
		{
			switch (eventType)
			{
			case EventType.RunEnded:
				RunTracker.LogRunEndedData(onLogReady);
				break;
			case EventType.StaticData:
				onLogReady(GetStaticData());
				break;
			case EventType.DailyData:
				onLogReady(GetDailyData());
				break;
			case EventType.SyboUserId:
				onLogReady(GetSyboUserId());
				break;
			case EventType.SingleValidPurchase:
				onLogReady(GetSingleValidPurchase());
				break;
			case EventType.MadeValidPurchase:
				onLogReady(GetValidInAppPurchase(extra));
				break;
			case EventType.PlayerBirthdayEntered:
				onLogReady(GetPlayerBirthday());
				break;
			case EventType.ABTestDailyReport:
				onLogReady(GetABTestDailyReportValue(extra));
				break;
			case EventType.ConsentGiven:
				onLogReady(GetConsentGivenEvent());
				break;
			case EventType.ConsentWithdrawn:
				onLogReady(GetConsentWithdrawnEvent(extra));
				break;
			case EventType.ConsentPrivacyPolicyOpened:
				onLogReady(GetConsentPrivacyLinkClickedEvent());
				break;
			}
		}

		private static bool IsDateToday(DateTime lastDate)
		{
			return lastDate.Date == CDateTime.UtcNow.Date;
		}

		[Conditional("KILOO_ANALYTICS_TRACKER")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}
	}
}
