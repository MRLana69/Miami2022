#define ENABLE_ERROR_LOGS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Kiloo;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Info
{
	public class OnlineSettings
	{
		public const string DISABLED_SETTING = "disabled";

		private const string PLATFORM = "android";

		private const string URL = "/newonlinesettings.php";

		private static readonly char[] LINEBREAK_CHARS = new char[2] { '\n', '\r' };

		private const string FILE_SECRET = "pdvshbhkndf92k19zvbckawd92fjk";

		private const int FILEFORMAT_VERSION = 1;

		private static OnlineSettings _instance;

		private Dictionary<string, string> _settings = new Dictionary<string, string>();

		private bool _isDownloading;

		private GameObject _downloadGameObject;

		private Action _onFinishedDownloadingOnlineSettingsHandler;

		public Action<bool> onOnlineSettingsDownloadResult;

		private string _checksumToShow;

		public Dictionary<string, string> rawSettings
		{
			get
			{
				return _settings;
			}
		}

		public static bool isInstanced
		{
			get
			{
				return _instance != null;
			}
		}

		public static OnlineSettings instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new OnlineSettings();
				}
				return _instance;
			}
		}

		public bool isDownloading
		{
			get
			{
				return _isDownloading;
			}
		}

		private OnlineSettings()
		{
			LoadFromFile();
		}

		public void AddOnFinishedDownloadingOnlineSettingsHandler(Action handler)
		{
			_onFinishedDownloadingOnlineSettingsHandler = (Action)Delegate.Combine(_onFinishedDownloadingOnlineSettingsHandler, handler);
		}

		public void RemoveOnFinishedDownloadingOnlineSettingsHandler(Action handler)
		{
			_onFinishedDownloadingOnlineSettingsHandler = (Action)Delegate.Remove(_onFinishedDownloadingOnlineSettingsHandler, handler);
		}

		public void DownloadNow()
		{
			if (!_isDownloading)
			{
				if (_downloadGameObject != null)
				{
					UnityEngine.Object.Destroy(_downloadGameObject);
				}
				_downloadGameObject = new GameObject("OnlineSettingsDownloader");
				UnityEngine.Object.DontDestroyOnLoad(_downloadGameObject);
				_downloadGameObject.AddComponent<NewMonoBehaviour>().StartCoroutine(DownloadCoroutine());
			}
			else
			{
				LogWarning("OnlineSettings.DownloadNow(): Already downloading");
			}
		}

		public double GetValue(string key, double defaultValue)
		{
			string value;
			if (_settings.TryGetValue(key, out value))
			{
				try
				{
					return double.Parse(value, NumberFormatInfo.InvariantInfo);
				}
				catch
				{
					LogError("OnlineSettings.GetValue() " + key + " returns the defaultValue because send string was " + value);
				}
			}
			return defaultValue;
		}

		public float GetValue(string key, float defaultValue)
		{
			string value;
			if (_settings.TryGetValue(key, out value))
			{
				try
				{
					return float.Parse(value, NumberFormatInfo.InvariantInfo);
				}
				catch
				{
					LogError("OnlineSettings.GetValue() " + key + " returns the defaultValue because send string was " + value);
				}
			}
			return defaultValue;
		}

		public int GetValue(string key, int defaultValue)
		{
			string value;
			if (_settings.TryGetValue(key, out value))
			{
				try
				{
					return int.Parse(value, NumberFormatInfo.InvariantInfo);
				}
				catch
				{
					LogError("OnlineSettings.GetValue() " + key + " returns the defaultValue because send string was " + value);
				}
			}
			return defaultValue;
		}

		public uint GetValue(string key, uint defaultValue)
		{
			string value;
			if (_settings.TryGetValue(key, out value))
			{
				try
				{
					return uint.Parse(value, NumberFormatInfo.InvariantInfo);
				}
				catch
				{
					SDebug.LogError("OnlineSettings.GetValue() " + key + " returns the defaultValue because send string was " + value);
				}
			}
			return defaultValue;
		}

		public string GetValue(string key, string defaultValue)
		{
			string value;
			if (_settings.TryGetValue(key, out value))
			{
				return value;
			}
			return defaultValue;
		}

		public bool GetValue(string key, bool defaultValue)
		{
			string value;
			if (_settings.TryGetValue(key, out value))
			{
				try
				{
					return bool.Parse(value);
				}
				catch
				{
					LogError("OnlineSettings.GetValue() " + key + " returns the defaultValue because send string was " + value);
				}
			}
			return defaultValue;
		}

		public bool TryGetValue(string key, out string valueString)
		{
			return _settings.TryGetValue(key, out valueString);
		}

		public bool HasValue(string key)
		{
			return _settings.ContainsKey(key);
		}

		public string GetCurrentPlatform()
		{
			return "android";
		}

		public string GetCurrentPlatformPretty()
		{
			string currentPlatform = GetCurrentPlatform();
			string text = currentPlatform[0].ToString();
			if (currentPlatform == "ios")
			{
				return "iOS";
			}
			return (currentPlatform.Length <= 0) ? currentPlatform : (text.ToUpper() + currentPlatform.Substring(1));
		}

		public string GetChecksumToShow()
		{
			return _checksumToShow;
		}

		private IEnumerator DownloadCoroutine()
		{
			_isDownloading = true;
			if (Globals.DEBUG_USE_FAKE_ONLINE_SETTINGS)
			{
				_settings = StringUtility.ParseProperties("efe40d19b11ddd485b4188664e9969f4bc89de0d\n[latestversion] 1.7.4\n[latestversion_changelist] \n\n\n* Subway Surfers in the Snow!\n* Limited Holiday character\n* Discover special Boards\n* Receive a free GIFT!\n* Optimized for iPhone 5!\n[in_app_tier_1] 2500\n[in_app_tier_2] 15000\n[in_app_tier_3] -40\n[in_app_tier_4] 250000\n[in_app_tier_5] -30\n[in_app_tier_6] -30\n[in_app_tier_7] -30\n[in_app_tier_8] -30\n[in_app_double_coins] -5\n[discount_end_time] 1496318400\n[videoads_providerlist] vungleclips\n[hoverboard_expiretime_liberty] 23-12-2012 22:00:00\n[hoverboard_starttime_liberty] 20-11-2012 22:00:00\n[character_expiretime_nychar] 23-12-2012 22:00:00\n[character_starttime_nychar] 13-10-2012 22:00:00\n[character_expiretime_elftricky] 23-11-2012 22:00:00\n[character_starttime_elftricky] 23-11-2012 22:00:00\n[enable_local_notifications] true[save_me_locales] en_US;en_UK;[offerwall_number_of_offers] 4[offerwall_session_timeout] 24[offerwall_offers_timeout] 24[offerwall_balance_timeout] 24[offerwal_min_reward_allowed] 10[offerwall_max_reward_allowed] 10000[offerwall_filter_free] true[save_me_locales] all[event_id]1496318400[event_value] 0;0[event_type] CharacterName[event_start] 1404694800[event_end] 1496318400[event_ribbon_text]ad3;3;6[number_of_fails_for_interstitial] 1[interstitials_limit_per_hour] 10[interstitials_limit_per_day] 40[intesrtitials_delay_seconds] 10[keys_rewarded_for_video_ad] 2[sybo_analytics_enabled] true[sybo_analytics_endpoint_baseurl] https://subwaysurf.dev.bladesofbrim.com[sybo_analytics_sampling_threshold] 1[sybo_analytics_max_games_without_flush] 1[sybo_analytics_games_in_session_before_first_flush] 1[home_interstitials_list] supersonic:1_100,inmobi:2_100,chartboost:3_100,supersonic:4_100,facebook:5_100[home_videos_list] supersonic:1_100,adcolony:2_100,tapjoy:3_100,inmobi:4_100,flurry:5_100,chartboost:7_100[default_interstitial_list] supersonic:1_100,inmobi:2_100,chartboost:3_100,supersonic:4_100,facebook:5_100[default_video_list] supersonic:1_100,adcolony:2_100,tapjoy:3_100,inmobi:4_100,flurry:5_100,chartboost:7_100[ad_location_home_videos_enabled] true[ad_location_earn_coins_videos_enabled] true[ad_location_double_up_videos_enabled] true[ad_location_home_interstitials_enabled] true[ad_location_game_over_interstitials_enabled] true");
				SaveToFile();
				Action<bool> action = onOnlineSettingsDownloadResult;
				if (action != null)
				{
					action(true);
				}
			}
			else
			{
				bool downloadSucceded = false;
				StringBuilder url = ((!Globals.DEBUG_OVERRIDE_ONLINE_SETTINGS_URL) ? new StringBuilder("http://hoodrunner.kiloo.com") : new StringBuilder("http://10.0.0.169/hoodrunner"));
				url.Append("/newonlinesettings.php");
				url.Append("?platform=").Append("android");
				url.Append("&locale=").Append(DeviceUtility.GetLocale());
				string timezone = CDateTime.Now.GetCurrentTimeZoneUTCOffset().TotalHours.ToString("F2", CultureInfo.InvariantCulture);
				url.Append("&timezone=").Append(timezone);
				string bundleVersion = DeviceUtility.GetBundleVersion();
				if (!string.IsNullOrEmpty(bundleVersion))
				{
					url.Append("&version=").Append(bundleVersion);
				}
				if (Globals.DEBUG_USE_ONLINESETTINGS_SET && !string.IsNullOrEmpty(Globals.ONLINESETTINGS_SET))
				{
					url.Append("&set=").Append(WWW.EscapeURL(Globals.ONLINESETTINGS_SET));
				}
				if (Globals.DEBUG_USE_ONLINESETTINGS_TESTTIME)
				{
					url.Append("&testtime=").Append(WWW.EscapeURL(CDateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture)));
				}
				WWW www = new WWW(url.ToString());
				yield return www;
				if (www.error != null)
				{
					LogWarning("OnlineSettings download failed: " + www.error);
				}
				else
				{
					downloadSucceded = true;
					string text = www.text;
					if (text.Contains("<html"))
					{
						LogError("OnlineSettings download failed: " + text);
					}
					else
					{
						SetCurrentChecksum(text);
						if (ValidateChecksum(text))
						{
							_settings = StringUtility.ParseProperties(www.text);
							SaveToFile();
						}
						else
						{
							SDebug.LogError("OnlineSettings checksum verification failed");
						}
					}
				}
				Action<bool> handler = onOnlineSettingsDownloadResult;
				if (handler != null)
				{
					handler(downloadSucceded);
				}
			}
			if (_onFinishedDownloadingOnlineSettingsHandler != null)
			{
				_onFinishedDownloadingOnlineSettingsHandler();
			}
			UnityEngine.Object.Destroy(_downloadGameObject);
			_downloadGameObject = null;
			_isDownloading = false;
		}

		private void SetCurrentChecksum(string response)
		{
			int num = response.IndexOfAny(LINEBREAK_CHARS);
			if (num > 0)
			{
				_checksumToShow = response.Substring(0, num);
			}
			else
			{
				_checksumToShow = null;
			}
		}

		private static bool ValidateChecksum(string response)
		{
			if (Globals.DEBUG_USE_FAKE_ONLINE_SETTINGS)
			{
				return true;
			}
			int num = response.IndexOfAny(LINEBREAK_CHARS);
			if (num > 0)
			{
				string strA = response.Substring(0, num);
				string text = response.Substring(num + 1).Trim();
				string strB = StringUtility.ToHexString(Checksum.ComputeSHA1(text + "resxrctrv7tgv7gb8h9h9u0909kllfmolkjnhghgjjkhjghg"));
				if (string.Compare(strA, strB, Utils.stringComparisonIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}

		private void LoadFromFile()
		{
			try
			{
				string saveDataPath = GetSaveDataPath();
				byte[] buffer = FileUtil.Load(saveDataPath, "pdvshbhkndf92k19zvbckawd92fjk");
				MemoryStream memoryStream = new MemoryStream(buffer);
				BinaryReader binaryReader = new BinaryReader(memoryStream);
				int num = binaryReader.ReadInt32();
				_settings = FileUtil.ReadStringStringDictionary(binaryReader);
				memoryStream.Close();
			}
			catch (FileNotFoundException)
			{
			}
			catch (Exception)
			{
			}
		}

		private void SaveToFile()
		{
			try
			{
				MemoryStream memoryStream = new MemoryStream(4096);
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write(1);
				FileUtil.WriteStringStringDictionary(binaryWriter, _settings);
				string saveDataPath = GetSaveDataPath();
				FileUtil.Save(saveDataPath, memoryStream.GetBuffer(), "pdvshbhkndf92k19zvbckawd92fjk", 0, (int)memoryStream.Length);
				memoryStream.Close();
			}
			catch (Exception ex)
			{
				LogError("Failed to save OnlineSettings to file: " + ex);
			}
		}

		private static string GetSaveDataPath()
		{
			return DeviceUtility.GetUserDataPath() + "/onlinesettings";
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError(msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_ONLINESETTINGS_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}
	}
}
