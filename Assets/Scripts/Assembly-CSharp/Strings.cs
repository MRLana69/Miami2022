using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using AppRefresh;
using Data.Save;
using Info;
using Kiloo.Common.Extensions;
using Kiloo.Common.LocalizationManager;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Serialization;
using Kiloo.Common.Utility;
using UnityEngine;

public static class Strings
{
	public class StringsLanguage
	{
		public Locale locale;

		public string resourcesPath;
	}

	private static Dictionary<string, string> _currentLanguageMap = new Dictionary<string, string>();

	private static Dictionary<string, string> _defaultLanguageMap = new Dictionary<string, string>();

	public static bool gibberish = false;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action languageChanged__BackingField;

	private static StringsLanguage _language = new StringsLanguage();

	private const string LOCALE_FILE_ENDING = "_locale";

	private const string RESOURCE_FOLDER_PATH = "Kiloo/LocalizationManager/text";

	public static List<StringsLanguage> languages = new List<StringsLanguage>();

	private static bool _hasInited;

	private static readonly IJsonSerializer _jsonConverter = new KilooSerializer
	{
		indent = true
	};

	public static StringsLanguage defaultLanguage { get; private set; }

	public static StringsLanguage language
	{
		get
		{
			return _language;
		}
		set
		{
			StringsLanguage stringsLanguage = ((value == null) ? null : languages.Find((StringsLanguage x) => x.locale == value.locale));
			if (stringsLanguage == null)
			{
				stringsLanguage = defaultLanguage;
			}
			if (_language == stringsLanguage)
			{
				return;
			}
			if (stringsLanguage != null)
			{
				_currentLanguageMap = LoadLanguageMap(stringsLanguage);
				_language = stringsLanguage;
			}
			else
			{
				SDebug.LogError("No language found");
				_currentLanguageMap = new Dictionary<string, string>(StringID.entries.Length);
				string[] entries = StringID.entries;
				foreach (string key in entries)
				{
					_currentLanguageMap.Add(key, string.Empty);
				}
				_language = new StringsLanguage();
			}
			languageChanged__BackingField.SafeInvoke();
		}
	}

	public static CultureInfo cultureInfo
	{
		get
		{
			return new CultureInfo(language.locale.combined);
		}
	}

	public static event Action languageChanged
	{
		add
		{
			Action action = languageChanged__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref languageChanged__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = languageChanged__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref languageChanged__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static void Init(LocaleData data)
	{
		if (_hasInited)
		{
			return;
		}
		_hasInited = true;
		languages = new List<StringsLanguage>(data.supportedLocales.Length);
		Locale[] supportedLocales = data.supportedLocales;
		foreach (Locale locale in supportedLocales)
		{
			StringsLanguage stringsLanguage = new StringsLanguage();
			stringsLanguage.locale = locale;
			stringsLanguage.resourcesPath = "Kiloo/LocalizationManager/text/" + locale.combined + "_locale";
			StringsLanguage item = stringsLanguage;
			languages.Add(item);
			if (defaultLanguage == null && locale == data.defaultLocale)
			{
				defaultLanguage = item;
			}
		}
		_defaultLanguageMap = LoadLanguageMap(defaultLanguage, true);
		AppRefreshHandler.refresh += OnRefresh;
		if (Application.isEditor)
		{
			VerifyLocaleResources();
		}
	}

	private static void OnRefresh(RefreshType refreshType)
	{
		if (refreshType.HasFlag(RefreshType.UI))
		{
			language = languages.Find((StringsLanguage x) => x.locale.combined == PreferencesSave.language);
		}
	}

	public static StringsLanguage DetermineBestLanguage()
	{
		string text = PreferencesSave.language;
		string input = ((!string.IsNullOrEmpty(text)) ? text : DeviceUtility.GetUnfilteredLocale());
		if (Globals.DEBUG_FORCE_SPECIFIC_LOCALE)
		{
			input = "pt-BR";
		}
		Locale locale;
		if (!Locale.TryParse(input, out locale))
		{
			return defaultLanguage;
		}
		StringsLanguage result = defaultLanguage;
		StringsLanguage stringsLanguage = languages.Find((StringsLanguage x) => x.locale == locale);
		if (stringsLanguage != null)
		{
			result = stringsLanguage;
		}
		else
		{
			IEnumerable<StringsLanguage> enumerable = languages.Where((StringsLanguage x) => x.locale.language == locale.language && x.locale.localeType == Locale.LocaleType.Language);
			using (IEnumerator<StringsLanguage> enumerator = enumerable.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					StringsLanguage current = enumerator.Current;
					result = current;
				}
			}
			IEnumerable<StringsLanguage> enumerable2 = languages.Where((StringsLanguage x) => x.locale.language == locale.language && x.locale.extraRegions.Contains(locale.region));
			using (IEnumerator<StringsLanguage> enumerator2 = enumerable2.GetEnumerator())
			{
				if (enumerator2.MoveNext())
				{
					StringsLanguage current2 = enumerator2.Current;
					result = current2;
				}
			}
		}
		return result;
	}

	private static void VerifyLocaleResources()
	{
		List<TextAsset> list = Resources.LoadAll<TextAsset>("Kiloo/LocalizationManager/text").ToList();
		foreach (StringsLanguage language in languages)
		{
			bool flag = false;
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if (list[num].name.Replace("_locale", string.Empty) == language.locale.combined)
				{
					flag = true;
					list.RemoveAt(num);
					break;
				}
			}
			if (!flag)
			{
				SDebug.LogError("Language '" + language.locale.combined + "' is in available languages, but couldn't find matching file in resources folder!");
			}
		}
		if (list.Count > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (TextAsset item in list)
			{
				stringBuilder.Append(item.name);
				stringBuilder.Append(", ");
			}
		}
		if (defaultLanguage == null)
		{
			SDebug.LogError("Strings.Init(): defaultLanguage is null!");
		}
	}

	public static string Get(string key)
	{
		return GetFromMap(key, _currentLanguageMap);
	}

	public static string GetDefault(string key)
	{
		return GetFromMap(key, _defaultLanguageMap);
	}

	public static string GetLegal(string key)
	{
		return LegalTextData.instance.GetLegalText(key);
	}

	private static string GetFromMap(string key, Dictionary<string, string> map)
	{
		if (string.IsNullOrEmpty(key))
		{
			return null;
		}
		string value;
		if (map.TryGetValue(key, out value))
		{
			if (gibberish)
			{
				string text = value ?? string.Empty;
				return new string('x', text.Length);
			}
			return value ?? string.Empty;
		}
		return string.Empty;
	}

	public static bool IsMapped(string keyString)
	{
		return _hasInited && _currentLanguageMap.ContainsKey(keyString);
	}

	private static Dictionary<string, string> LoadLanguageMap(StringsLanguage lang, bool forceLoad = false)
	{
		if (!forceLoad && lang == defaultLanguage)
		{
			return _defaultLanguageMap;
		}
		TextAsset textAsset = Resources.Load<TextAsset>(lang.resourcesPath);
		return DeserializeObject(textAsset.text) ?? new Dictionary<string, string>();
	}

	public static Dictionary<string, string> DeserializeObject(string s)
	{
		Dictionary<string, object> dictionary = _jsonConverter.FromJson<Dictionary<string, object>>(s);
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		if (dictionary == null)
		{
			SDebug.LogError("Deserialized JSon is null.");
			return null;
		}
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			dictionary2.Add(item.Key, item.Value.ToString());
		}
		return dictionary2;
	}

	public static string SerializeObject(Dictionary<string, string> data)
	{
		return _jsonConverter.ToJson(data);
	}

	[Conditional("ENABLE_STRINGS_LOGS")]
	private static void Log(string text, UnityEngine.Object context = null)
	{
	}

	public static CultureInfo GetLegalCultureInfo()
	{
		return LegalTextData.instance.GetLegalCulture();
	}

	private static string AddLocaleSuffix(string key)
	{
		if (language == null || language == defaultLanguage)
		{
			return key;
		}
		Locale locale = language.locale;
		switch (locale.localeType)
		{
		case Locale.LocaleType.Language:
			return string.Format("{0}_{1}", key, locale.language);
		case Locale.LocaleType.LanguageAndRegion:
			return string.Format("{0}_{1}{2}", key, locale.language, locale.region.ToLower());
		default:
			SDebug.LogError("Unhandled localeType: " + locale.localeType);
			return key;
		}
	}

	public static bool HasLocalizedOnlineSettings(string key)
	{
		return OnlineSettings.instance.HasValue(AddLocaleSuffix(key));
	}

	public static string GetLocalizedOnlineSettings(string key, string defaultValue)
	{
		return OnlineSettings.instance.GetValue(AddLocaleSuffix(key), defaultValue);
	}

	public static bool TryGetLocalizedOnlineSettings(string key, out string valueString)
	{
		return OnlineSettings.instance.TryGetValue(AddLocaleSuffix(key), out valueString);
	}
}
