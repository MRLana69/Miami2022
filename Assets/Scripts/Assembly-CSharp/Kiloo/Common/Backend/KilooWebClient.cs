using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Kiloo.Common.HTTP;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Security;
using Kiloo.Common.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.Backend
{
	public static class KilooWebClient
	{
		private const string TAGS_PLAYERPREFS_KEY = "kiloowebclient_tags";

		private static Dictionary<string, string> _clientInfoHeaders;

		private static IDictionary<string, object> _tags;

		public static float defaultRequestTimeout { get; set; }

		public static float defaultKeepAlive { get; set; }

		public static float defaultMinBackOffTime { get; set; }

		public static float defaultMaxBackOffTime { get; set; }

		static KilooWebClient()
		{
			_tags = new Dictionary<string, object>();
			defaultRequestTimeout = 0f;
			defaultKeepAlive = 5f;
			defaultMinBackOffTime = 1f;
			defaultMaxBackOffTime = 0f;
		}

		public static void InitAutomatically(string appId)
		{
			string language;
			string country;
			GetLanguageAndCountry(out language, out country);
			float deviceTimeZone = (float)DateTime.Now.GetCurrentTimeZoneUTCOffset().TotalHours;
			string text = DeviceUtility.GetBundleVersion();
			if (string.IsNullOrEmpty(text))
			{
				text = "0.0";
			}
			InitWithClientInfo(appId, text, GetPlatformIdentifier(), SystemInfo.deviceModel, GetOsIdentifier(), GetOsVersion(), language, country, deviceTimeZone);
		}

		public static void InitWithClientInfo(string appId, string appVersion, string platformId, string deviceModel, string osId, string osVersion, string deviceLanguage, string deviceCountry, float deviceTimeZone)
		{
			if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appVersion) || string.IsNullOrEmpty(platformId) || string.IsNullOrEmpty(deviceModel) || string.IsNullOrEmpty(osId) || string.IsNullOrEmpty(osVersion) || string.IsNullOrEmpty(deviceLanguage) || string.IsNullOrEmpty(deviceCountry) || deviceTimeZone < -12f || deviceTimeZone > 14f)
			{
				LogError("Initialize: Invalid initialization parameters");
				return;
			}
			int num = (int)deviceTimeZone;
			float num2 = deviceTimeZone - (float)num;
			int num3 = Mathf.Abs(Mathf.RoundToInt(num2 * 60f));
			string value = string.Format("{0:+00;-00}:{1:00}", num, num3);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Kiloo-App-Identifier", appId);
			dictionary.Add("Kiloo-App-Version", appVersion);
			dictionary.Add("Kiloo-Platform-Identifier", platformId);
			dictionary.Add("Kiloo-Build-Identifier", platformId);
			dictionary.Add("Kiloo-Device-Model", "\"" + deviceModel + "\"");
			dictionary.Add("Kiloo-Os-Identifier", osId);
			dictionary.Add("Kiloo-Os-Version", osVersion);
			dictionary.Add("Kiloo-Device-Language", deviceLanguage);
			dictionary.Add("Kiloo-Device-Country", deviceCountry);
			dictionary.Add("Kiloo-Device-Time-Zone", value);
			dictionary.Add("Kiloo-Device-Time", string.Empty);
			_clientInfoHeaders = dictionary;
			_tags = LoadTags();
		}

		public static IRequest CreatePost(Uri uri)
		{
			return CreateRequest("POST", uri);
		}

		public static IRequest CreateGet(Uri uri)
		{
			return CreateRequest("GET", uri);
		}

		public static IRequest CreatePut(Uri uri)
		{
			return CreateRequest("PUT", uri);
		}

		public static IRequest CreateDelete(Uri uri)
		{
			return CreateRequest("DELETE", uri);
		}

		public static IRequest WithKilooAntiCheat(this IRequest request)
		{
			request.headers.Set("KAC-TOTP", TOTP.NewTOTP());
			return request;
		}

		public static IRequest WithKilooDeviceId(this IRequest request)
		{
			request.headers.Set("Kiloo-Device-Id", DeviceUtility.GetDeviceIdentifier());
			return request;
		}

		public static IRequest WithKilooTags(this IRequest request)
		{
			request.headers.Set("Kiloo-Tags", GetTags());
			return request;
		}

		public static bool HasTag(string tag)
		{
			return _tags.ContainsKey(tag);
		}

		public static string GetTags()
		{
			if (_tags.Count > 0)
			{
				string[] array = new string[_tags.Count];
				_tags.Keys.CopyTo(array, 0);
				return string.Join(",", array);
			}
			return string.Empty;
		}

		public static void AddTags(params string[] tags)
		{
			Regex regex = new Regex("^\\w+$");
			foreach (string text in tags)
			{
				if (!regex.IsMatch(text))
				{
					throw new ArgumentException("Invalid character in tag");
				}
				_tags[text] = null;
			}
			SaveTags();
		}

		public static void RemoveTags(params string[] tags)
		{
			foreach (string key in tags)
			{
				_tags.Remove(key);
			}
			SaveTags();
		}

		private static IRequest CreateRequest(string method, Uri uri)
		{
			IRequest request = HTTPFactory.CreateRequest(method, uri).WithTimeout(defaultRequestTimeout).WithKeepAlive(defaultKeepAlive)
				.WithMinBackOffTime(defaultMinBackOffTime)
				.WithMaxBackOffTime(defaultMaxBackOffTime);
			Headers headers = request.headers;
			if (_clientInfoHeaders != null)
			{
				headers.SetAll(_clientInfoHeaders);
			}
			else
			{
				LogError("KilooWebClient not initialized");
			}
			return request;
		}

		private static void GetLanguageAndCountry(out string language, out string country)
		{
			string locale = DeviceUtility.GetLocale();
			string[] array = locale.Split('-');
			language = array[0].ToLowerInvariant();
			if (string.IsNullOrEmpty(language))
			{
				language = "en";
			}
			country = ((array.Length <= 1) ? null : array[1].ToUpperInvariant());
			if (string.IsNullOrEmpty(country))
			{
				country = "US";
			}
		}

		private static string GetOsIdentifier()
		{
			return "android";
		}

		private static string GetOsVersion()
		{
			string operatingSystem = SystemInfo.operatingSystem;
			Regex regex = new Regex("(?:\\s|^)(\\d+\\.\\d+(?:\\.\\d+)?)(?:\\s|$)");
			Match match = regex.Match(operatingSystem);
			if (match.Success)
			{
				return match.Groups[1].ToString();
			}
			return "0.0";
		}

		private static bool IsKindleDevice()
		{
			return false;
		}

		private static string GetPlatformIdentifier()
		{
			return (!IsKindleDevice()) ? GetOsIdentifier() : "kindle";
		}

		private static IDictionary<string, object> LoadTags()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			string text = PlayerPrefs.GetString("kiloowebclient_tags", string.Empty);
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(',');
				string[] array2 = array;
				foreach (string key in array2)
				{
					dictionary[key] = null;
				}
			}
			return dictionary;
		}

		private static void SaveTags()
		{
			PlayerPrefs.SetString("kiloowebclient_tags", GetTags());
			PlayerPrefs.Save();
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[KilooWebClient] " + msg);
		}

		[Conditional("HTTP_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[KilooWebClient] " + msg);
		}
	}
}
