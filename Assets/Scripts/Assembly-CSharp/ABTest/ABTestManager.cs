using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AppRefresh;
using Data.Save;
using Info;
using Kiloo;
using Kiloo.Analytics;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace ABTest
{
	public static class ABTestManager
	{
		private static List<string> _tags = new List<string>();

		private static Dictionary<string, ABTest> _tests = new Dictionary<string, ABTest>();

		public static Action<string, string> onRegisterDailyReport;

		public static Action<string, int> onTagAdded;

		private static uint _boostsScreen = 0u;

		private static uint _earnCoinsScreen = 0u;

		private static uint _storeScreen = 0u;

		private static uint _quickStorePopup = 0u;

		public static bool isInitialized { get; private set; }

		public static void Initialize()
		{
			if (!isInitialized)
			{
				isInitialized = true;
				OnlineSettings.instance.AddOnFinishedDownloadingOnlineSettingsHandler(OnFinishedDownloadingOnlineSettings);
				AppRefreshHandler.hasRefreshed += OnHasRefreshed;
				SetupTestData();
			}
		}

		private static void OnHasRefreshed(RefreshType refreshType)
		{
			if (refreshType.HasFlag(RefreshType.Data))
			{
				SetupTestData();
			}
		}

		private static void SetupTestData()
		{
			LoadTags();
			_tests = ABTestData.testData.ToDictionary((ABTestData.ABTestDataItem x) => x.identifier, (ABTestData.ABTestDataItem y) => new ABTest(y));
			Update();
		}

		public static bool IsActive(string identifier)
		{
			return _tests.ContainsKey(identifier) && !_tests[identifier].isForcedDisabled && _tests[identifier].isActive;
		}

		public static string GetTag(string identifier)
		{
			return (!IsActive(identifier)) ? string.Empty : _tests[identifier].tag;
		}

		public static string GetDefaultPayload(string identifier)
		{
			if (isInitialized)
			{
				return _tests[identifier].defaultPayload;
			}
			return ABTestData.testData.Find((ABTestData.ABTestDataItem x) => x.identifier == identifier).defaultPayload;
		}

		public static string GetPayloadString(string identifier)
		{
			return (!IsActive(identifier)) ? GetDefaultPayload(identifier) : _tests[identifier].GetPayload();
		}

		public static bool GetPayloadBool(string identifier)
		{
			return (!IsActive(identifier)) ? bool.Parse(GetDefaultPayload(identifier)) : _tests[identifier].GetPayload(bool.Parse(GetDefaultPayload(identifier)));
		}

		public static uint GetPayloadUInt(string identifier)
		{
			return (!IsActive(identifier)) ? uint.Parse(GetDefaultPayload(identifier)) : _tests[identifier].GetPayload(uint.Parse(GetDefaultPayload(identifier)));
		}

		public static int GetPayloadInt(string identifier)
		{
			return (!IsActive(identifier)) ? int.Parse(GetDefaultPayload(identifier)) : _tests[identifier].GetPayload(int.Parse(GetDefaultPayload(identifier)));
		}

		private static void OnFinishedDownloadingOnlineSettings()
		{
			Update();
			if (!(AnalyticsSave.abTestLastDailyEventsReportDate != CDateTime.UtcNow.Date))
			{
				return;
			}
			foreach (KeyValuePair<string, ABTest> test in _tests)
			{
				RegisterDailyReport(test.Key);
			}
			AnalyticsSave.abTestLastDailyEventsReportDate = CDateTime.UtcNow;
		}

		private static void Update()
		{
			foreach (KeyValuePair<string, ABTest> test in _tests)
			{
				Update(test.Value);
			}
		}

		private static void Update(ABTest test)
		{
			test.Update(_tags);
			if (test.isActive && !test.tag.IsNullOrEmpty() && !_tags.Contains(test.tag))
			{
				_tags.Add(test.tag);
				onTagAdded.SafeInvoke(test.tag, test.segmentIndex);
				SaveTags();
			}
		}

		private static void RegisterDailyReport(string identifier)
		{
			if (IsActive(identifier))
			{
				string payloadString = GetPayloadString(identifier);
				string tag = GetTag(identifier);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Identifier", identifier);
				dictionary.Add("Payload", payloadString + "_" + GetTag(identifier));
				dictionary.Add("Tag", tag);
				Flurry.LogEventWithSeveralParameters("A/B test daily report", dictionary);
				Manager.LogEvent(Kiloo.Analytics.EventType.ABTestDailyReport, false, identifier);
				onRegisterDailyReport.SafeInvoke(identifier, payloadString);
			}
		}

		private static void LoadTags()
		{
			_tags = AnalyticsSave.abTestTagData.Split(';').ToList();
		}

		private static void SaveTags()
		{
			AnalyticsSave.abTestTagData = string.Join(";", _tags.ToArray());
		}

		private static bool IsDateToday(string previousDate)
		{
			return Utils.StringToDateTime(previousDate, DateTime.MinValue).Date == CDateTime.Now.Date;
		}

		public static void CountPages(string screenName)
		{
			switch (screenName)
			{
			case "FreeStuffScreen":
				_earnCoinsScreen++;
				break;
			case "UpgradeScreen":
				_boostsScreen++;
				break;
			case "StoreScreen":
				_storeScreen++;
				break;
			case "StorePopup":
				_quickStorePopup++;
				break;
			}
		}

		public static void SendPageCountFlurry()
		{
			if (_boostsScreen + _earnCoinsScreen + _storeScreen + _quickStorePopup != 0)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("store_visits", _storeScreen.ToString());
				dictionary.Add("earn_coins_visits", _earnCoinsScreen.ToString());
				dictionary.Add("boosts_visits", _boostsScreen.ToString());
				dictionary.Add("quick_store_visits", _quickStorePopup.ToString());
				Dictionary<string, string> parameters = dictionary;
				Flurry.LogEventWithSeveralParameters("A/B Screen Opened", parameters);
				_boostsScreen = 0u;
				_earnCoinsScreen = 0u;
				_storeScreen = 0u;
				_quickStorePopup = 0u;
			}
		}

		public static bool HasDebugPayload(string identifier)
		{
			return _tests[identifier].HasDebugPayload();
		}

		public static string GetDebugPayload(string identifier)
		{
			return _tests[identifier].GetDebugPayload();
		}

		public static void EnableDebugPayload(string identifier, string value)
		{
			_tests[identifier].EnableDebugPayload(value);
			Update(_tests[identifier]);
		}

		public static void DisableDebugPayload(string identifier)
		{
			_tests[identifier].DisableDebugPayload();
			Update(_tests[identifier]);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarn(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogErr(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("[ABTestManager] " + msg, context);
		}

		[Conditional("ENABLE_ABTEST_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}
	}
}
