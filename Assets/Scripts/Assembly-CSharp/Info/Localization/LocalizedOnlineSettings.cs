using System.Collections.Generic;
using Kiloo.Common.LocalizationManager;

namespace Info.Localization
{
	public class LocalizedOnlineSettings
	{
		private readonly string _defaultLocale;

		private readonly string _combinedLocale;

		private readonly OnlineSettingsData _data;

		public string loadScreenTitle
		{
			get
			{
				return FetchOrDefault(_data.loadScreenTitle);
			}
		}

		public UserTip[] userTips
		{
			get
			{
				return FetchOrDefault(_data.userTips);
			}
		}

		public string latestVersionChangelog
		{
			get
			{
				return FetchOrDefault(_data.latestVersionChangelog);
			}
		}

		public LocalizedOnlineSettings(OnlineSettingsData data)
		{
			_combinedLocale = Strings.language.locale.combined;
			_data = data;
			_defaultLocale = LocaleData.instance.defaultLocale.combined;
		}

		public string getLinkLocalizedTitle(string Id)
		{
			return FetchOrDefault(_data.socialLinkOverrides[Id].title);
		}

		public string getLinkLocalizedButtonString(string Id)
		{
			return FetchOrDefault(_data.socialLinkOverrides[Id].buttonString);
		}

		private T FetchOrDefault<T>(IDictionary<string, T> localizedFieldValue)
		{
			T value = default(T);
			if (localizedFieldValue == null)
			{
				return value;
			}
			if (!localizedFieldValue.TryGetValue(_combinedLocale, out value))
			{
				localizedFieldValue.TryGetValue(_defaultLocale, out value);
			}
			return value;
		}
	}
}
