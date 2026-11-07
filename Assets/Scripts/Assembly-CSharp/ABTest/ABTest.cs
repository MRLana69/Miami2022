using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Data.Save;
using Info;
using Kiloo.Common.Extensions;
using Kiloo.Common.Utility;
using SYBO.Components.Analytics;
using UnityEngine;

namespace ABTest
{
	public class ABTest
	{
		private const string ONLINESETTINGS_ABTEST_PREFIX = "abtest";

		private const string ONLINESETTINGS_ABTEST_VERSION_KEY = "version";

		private const string ONLINESETTINGS_ABTEST_DEFAULT_PAYLOAD_KEY = "default_payload_override";

		private const string ONLINESETTINGS_ABTEST_ACTIVE_KEY = "active";

		private const string ONLINESETTINGS_ABTEST_MIN_INSTALL_DATETIME_KEY = "min_install_datetime";

		private const string ONLINESETTINGS_ABTEST_MAX_INSTALL_DATETIME_KEY = "max_install_datetime";

		private const string ONLINESETTINGS_ABTEST_EXCLUDED_TAGS_KEY = "excluded_tags";

		private const string ONLINESETTINGS_ABTEST_SEGMENT_OFFSET_KEY = "segment_offset";

		private const string ONLINESETTINGS_ABTEST_SEGMENT_SIZE_KEY = "segment_size";

		private const string ONLINESETTINGS_ABTEST_SEGMENT_PAYLOAD_KEY = "segment_payload";

		private const string ONLINESETTINGS_ABTEST_TAG_KEY = "tag";

		private const string ONLINESETTINGS_ABTEST_INCLUDE_ANALYTICS_SAMPLING_RANGE_KEY = "include_analytics_sampling_key_range";

		private const string ONLINESETTINGS_ABTEST_EXCLUDE_ANALYTICS_SAMPLING_RANGE_KEY = "exclude_analytics_sampling_key_range";

		private const string ONLINESETTINGS_ABTEST_DEBUG_ENABLE_ANALYTICS_SAMPLING_KEY_CHECK = "debug_enable_analytics_sampling_key_range";

		private const int DEFAULT_ABTEST_VERSION = 0;

		private const bool DEFAULT_ABTEST_ACTIVE = false;

		private const bool DEFAULT_ABTEST_DEBUG_ENABLE_ANALYTICS_SAMPLING_KEY_CHECK = false;

		private const string DEFAULT_ABTEST_DEFAULT = "";

		private const string DEFAULT_ABTEST_MIN_INSTALL_DATETIME = "";

		private const string DEFAULT_ABTEST_MAX_INSTALL_DATETIME = "";

		private const string DEFAULT_ABTEST_EXCLUDED_TAGS = "";

		private const string DEFAULT_ABTEST_SEGMENT_OFFSET = "";

		private const string DEFAULT_ABTEST_SEGMENT_SIZE = "";

		private const string DEFAULT_ABTEST_SEGMENT_PAYLOAD = "";

		private const string DEFAULT_ABTEST_INCLUDE_ANALYTICS_SAMPLING_RANGE = "";

		private const string DEFAULT_ABTEST_EXCLUDE_ANALYTICS_SAMPLING_RANGE = "";

		private const string DEBUG_PAYLOAD_PREFIX = "debug_payload_";

		private static readonly FloatRange[] KILOO_ANALYTICS_SAMPLING_KEY_TEST_RANGES = new FloatRange[2]
		{
			new FloatRange(0f, 0.05f),
			new FloatRange(0.1f, 1f)
		};

		private static readonly FloatRange[] SYBO_ANALYTICS_SAMPLING_KEY_TEST_RANGES = new FloatRange[1]
		{
			new FloatRange(0.05f, 0.1f)
		};

		private readonly string _identifier;

		private readonly float _sample;

		private readonly TestRange _analyticsSamplingKeyRange;

		private readonly float _analyticsSamplingKey;

		private int _version;

		private int _segmentIndex = -1;

		public int segmentIndex
		{
			get
			{
				return _segmentIndex;
			}
		}

		public bool isForcedDisabled { get; private set; }

		public bool isActive { get; private set; }

		public string payload { get; private set; }

		public string defaultPayload { get; private set; }

		public string tag { get; private set; }

		public ABTest(ABTestData.ABTestDataItem testDataItem)
		{
			_identifier = testDataItem.identifier;
			_analyticsSamplingKeyRange = testDataItem.testRange;
			_analyticsSamplingKey = AnalyticsSave.analyticsSamplingKey;
			string value = StringUtility.ToHexString(Checksum.ComputeSHA256(AnalyticsSave.abTestPlayerSeed + _identifier)).Substring(0, 8);
			_sample = new SyboRandom(new MTRandomGen(Convert.ToUInt32(value, 16), 0)).NextFloat() * 100f;
			_version = -1;
			defaultPayload = testDataItem.defaultPayload;
			isForcedDisabled = testDataItem.forceDisable;
		}

		public void Update(List<string> tags)
		{
			if (HasDebugPayload())
			{
				isActive = true;
				payload = GetDebugPayload();
				_version = -1;
				return;
			}
			int value = OnlineSettings.instance.GetValue(GetOnlineSettingsKey(_identifier, "version"), 0);
			if (value == _version)
			{
				return;
			}
			_version = value;
			string value2 = OnlineSettings.instance.GetValue(GetOnlineSettingsKey(_identifier, "default_payload_override"), string.Empty);
			if (!value2.IsNullOrEmpty())
			{
				defaultPayload = value2;
			}
			isActive = OnlineSettings.instance.GetValue(GetOnlineSettingsKey(_identifier, "active"), false);
			if (!isActive)
			{
				return;
			}
			string value3 = OnlineSettings.instance.GetValue(GetOnlineSettingsKey(_identifier, "min_install_datetime"), string.Empty);
			DateTime result;
			if (!value3.IsNullOrEmpty() && DateTime.TryParse(value3, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out result) && UserSave.firstInstallDate < result)
			{
				isActive = false;
				return;
			}
			string value4 = OnlineSettings.instance.GetValue(GetOnlineSettingsKey(_identifier, "max_install_datetime"), string.Empty);
			DateTime result2;
			if (!value4.IsNullOrEmpty() && DateTime.TryParse(value4, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out result2) && UserSave.firstInstallDate > result2)
			{
				isActive = false;
				return;
			}
			string value5 = OnlineSettings.instance.GetValue(GetOnlineSettingsKey(_identifier, "excluded_tags"), string.Empty);
			if (!value5.IsNullOrEmpty())
			{
				List<string> list = value5.Split(';').ToList();
				foreach (string item in list)
				{
					if (tags.Contains(item.Trim()))
					{
						isActive = false;
						return;
					}
				}
			}
			if (IsCheckForAnalyticsSamplingKeyRangeEnabled() && IsExcludedByAnalyticsSamplingKeyTestRanges())
			{
				isActive = false;
				return;
			}
			if (IsExcludedByExcludeAnalyticsSamplingKeyRange() || IsExcludedByIncludeAnalyticsSamplingKeyRange())
			{
				isActive = false;
				return;
			}
			string value6 = OnlineSettings.instance.GetValue(GetOnlineSettingsKey(_identifier, "segment_offset"), string.Empty);
			List<float> list2 = ParseFloatList(value6, ';');
			if (list2 == null)
			{
				isActive = false;
				return;
			}
			string value7 = OnlineSettings.instance.GetValue(GetOnlineSettingsKey(_identifier, "segment_size"), string.Empty);
			List<float> list3 = ParseFloatList(value7, ';');
			if (list3 == null)
			{
				isActive = false;
				return;
			}
			int num = -1;
			int num2 = Math.Min(list2.Count, list3.Count);
			for (int i = 0; i < num2; i++)
			{
				if (_sample > list2[i] && _sample < list2[i] + list3[i])
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				isActive = false;
				return;
			}
			_segmentIndex = num;
			payload = null;
			string value8 = OnlineSettings.instance.GetValue(GetOnlineSettingsKey(_identifier, "segment_payload"), string.Empty);
			if (!value8.IsNullOrEmpty())
			{
				List<string> list4 = value8.Split(';').ToList();
				if (list4.Count > num)
				{
					payload = list4[num];
				}
			}
			if (payload == null)
			{
				isActive = false;
			}
			else
			{
				tag = OnlineSettings.instance.GetValue(GetOnlineSettingsKey(_identifier, "tag"), _identifier).Trim();
			}
		}

		private bool IsExcludedByIncludeAnalyticsSamplingKeyRange()
		{
			string value = OnlineSettings.instance.GetValue(GetOnlineSettingsKey(_identifier, "include_analytics_sampling_key_range"), string.Empty);
			FloatRange floatRange = ParseKeyRange(value, '-');
			if (floatRange != null && !floatRange.IsValueWithinRange(_analyticsSamplingKey))
			{
				return true;
			}
			return false;
		}

		private bool IsExcludedByExcludeAnalyticsSamplingKeyRange()
		{
			string value = OnlineSettings.instance.GetValue(GetOnlineSettingsKey(_identifier, "exclude_analytics_sampling_key_range"), string.Empty);
			FloatRange floatRange = ParseKeyRange(value, '-');
			if (floatRange != null && floatRange.IsValueWithinRange(_analyticsSamplingKey))
			{
				return true;
			}
			return false;
		}

		private bool IsExcludedByAnalyticsSamplingKeyTestRanges()
		{
			foreach (FloatRange analyticsSamplingKeyTestRange in GetAnalyticsSamplingKeyTestRanges())
			{
				if (analyticsSamplingKeyTestRange.IsValueWithinRange(_analyticsSamplingKey))
				{
					return false;
				}
			}
			return true;
		}

		private bool IsCheckForAnalyticsSamplingKeyRangeEnabled()
		{
			bool value = OnlineSettings.instance.GetValue(GetOnlineSettingsKey(_identifier, "debug_enable_analytics_sampling_key_range"), false);
			if (!Globals.DEBUG || value)
			{
				return true;
			}
			return false;
		}

		private string RangesToText(IEnumerable<FloatRange> ranges)
		{
			string text = string.Empty;
			foreach (FloatRange range in ranges)
			{
				text += range;
				if (ranges.Last() != range)
				{
					text += ", ";
				}
			}
			return text;
		}

		private IEnumerable<FloatRange> GetAnalyticsSamplingKeyTestRanges()
		{
			switch (_analyticsSamplingKeyRange)
			{
			case TestRange.Kiloo:
				return KILOO_ANALYTICS_SAMPLING_KEY_TEST_RANGES;
			case TestRange.Sybo:
				return SYBO_ANALYTICS_SAMPLING_KEY_TEST_RANGES;
			default:
				return new FloatRange[0];
			}
		}

		public string GetPayload()
		{
			return payload;
		}

		public bool GetPayload(bool defaultValue)
		{
			bool result;
			if (bool.TryParse(payload, out result))
			{
				return result;
			}
			return defaultValue;
		}

		public uint GetPayload(uint defaultValue)
		{
			uint result;
			if (uint.TryParse(payload, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
			{
				return result;
			}
			return defaultValue;
		}

		public int GetPayload(int defaultValue)
		{
			int result;
			if (int.TryParse(payload, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
			{
				return result;
			}
			return defaultValue;
		}

		private static string GetOnlineSettingsKey(string identifier, string key)
		{
			return "abtest_" + identifier + "_" + key;
		}

		public FloatRange ParseKeyRange(string str, char separator)
		{
			List<float> list = ParseFloatList(str, separator);
			if (list != null && list.Count >= 2)
			{
				return new FloatRange(list[0], list[1]);
			}
			return null;
		}

		private static List<float> ParseFloatList(string str, char separator)
		{
			List<float> list = new List<float>();
			string[] array = str.Split(separator);
			string[] array2 = array;
			foreach (string s in array2)
			{
				float result;
				if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
				{
					list.Add(result);
					continue;
				}
				return null;
			}
			return list;
		}

		public bool HasDebugPayload()
		{
			return PlayerPrefs.HasKey("debug_payload_" + _identifier);
		}

		public string GetDebugPayload()
		{
			return PlayerPrefs.GetString("debug_payload_" + _identifier, null);
		}

		public void EnableDebugPayload(string value)
		{
			PlayerPrefs.SetString("debug_payload_" + _identifier, value);
		}

		public void DisableDebugPayload()
		{
			PlayerPrefs.DeleteKey("debug_payload_" + _identifier);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarn(string msg, string identifier, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogErr(string msg, string identifier, UnityEngine.Object context = null)
		{
			SDebug.LogError("[ABTest:" + identifier + "] " + msg, context);
		}

		[Conditional("ENABLE_ABTEST_LOGS")]
		private static void Log(string msg, string identifier, UnityEngine.Object context = null)
		{
		}
	}
}
