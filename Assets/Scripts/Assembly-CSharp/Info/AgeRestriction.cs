using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using AppRefresh;
using Data.Save;
using Kiloo.Analytics;
using Kiloo.Common.Extensions;
using Kiloo.Common.FlurryAnalytics;
using Kiloo.Common.Plugins.DeviceUtility;

namespace Info
{
	public static class AgeRestriction
	{
		private const string ONLINESETTINGS_AGE_RESTRICTION_THRESHOLD_KEY = "age_restriction_threshold";

		private const string ONLINESETTINGS_AGE_RESTRICTION_VERSION_KEY = "age_restriction_version";

		private const string ONLINESETTINGS_AGE_RESTRICTION_GLOBAL_DEFAULT_KEY = "age_restriction_global_default";

		private const int AGE_RESTRICTION_GLOBAL_THRESHOLD_DEFAULT = 13;

		private const int AGE_RESTRICTION_VERSION_DEFAULT = 0;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action restrictionChanged__BackingField;

		private static bool _appRefreshWasRestricted;

		private static readonly string CACHED_COUNTRY_CODE;

		private static readonly Dictionary<string, int> DEFAULT_COUNTRY_CODE_RESTRICTIONS;

		public static bool isRestricted
		{
			get
			{
				return countryCodeRestricted && (IsAgeRestricted() || !IsConsentGiven());
			}
		}

		public static bool waitForAgeRestriction
		{
			get
			{
				return countryCodeRestricted && (!HasValidData() || (!IsAgeRestricted() && !IsConsentGiven()));
			}
		}

		public static bool shouldDisplayPopup
		{
			get
			{
				return countryCodeRestricted && !HasValidData();
			}
		}

		public static bool shouldShowBehaviouralAdsConsentPopup
		{
			get
			{
				return countryCodeRestricted && !IsAgeRestricted() && !IsConsentGiven();
			}
		}

		public static bool countryCodeRestricted { get; private set; }

		public static event Action restrictionChanged
		{
			add
			{
				Action action = restrictionChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref restrictionChanged__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = restrictionChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref restrictionChanged__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		static AgeRestriction()
		{
			DEFAULT_COUNTRY_CODE_RESTRICTIONS = new Dictionary<string, int>
			{
				{ "au", 15 },
				{ "at", 14 },
				{ "be", 16 },
				{ "br", 18 },
				{ "bg", 16 },
				{ "hr", 16 },
				{ "cy", 16 },
				{ "cz", 13 },
				{ "dk", 13 },
				{ "ee", 16 },
				{ "fi", 13 },
				{ "fr", 16 },
				{ "de", 16 },
				{ "gr", 15 },
				{ "hu", 16 },
				{ "ie", 13 },
				{ "it", 16 },
				{ "lv", 13 },
				{ "lt", 16 },
				{ "lu", 16 },
				{ "mt", 16 },
				{ "nl", 16 },
				{ "no", 13 },
				{ "pl", 13 },
				{ "pt", 16 },
				{ "ro", 16 },
				{ "sk", 16 },
				{ "si", 16 },
				{ "za", 18 },
				{ "es", 13 },
				{ "se", 13 },
				{ "gb", 13 },
				{ "us", 13 }
			};
			CACHED_COUNTRY_CODE = Utils.GetContryCodeFromLocale(DeviceUtility.GetLocale());
			countryCodeRestricted = IsCountryRestricted();
			OnlineSettings.instance.AddOnFinishedDownloadingOnlineSettingsHandler(OnlineSettingsDownloaded);
			AppRefreshHandler.willRefresh += (RefreshType type) =>
			{
				if (type.HasFlag(RefreshType.Data))
				{
					_appRefreshWasRestricted = isRestricted;
				}
			};
			AppRefreshHandler.hasRefreshed += (RefreshType type) =>
			{
				if (type.HasFlag(RefreshType.Data) && isRestricted != _appRefreshWasRestricted)
				{
					restrictionChanged__BackingField.SafeInvoke();
				}
			};
		}

		private static void OnlineSettingsDownloaded()
		{
			countryCodeRestricted = IsCountryRestricted();
		}

		private static bool IsCountryRestricted()
		{
			if (OnlineSettings.instance.HasValue("age_restriction_threshold"))
			{
				int value = OnlineSettings.instance.GetValue("age_restriction_threshold", 0);
				return value >= 0;
			}
			if (DEFAULT_COUNTRY_CODE_RESTRICTIONS.ContainsKey(CACHED_COUNTRY_CODE))
			{
				return DEFAULT_COUNTRY_CODE_RESTRICTIONS[CACHED_COUNTRY_CODE] >= 0;
			}
			if (OnlineSettings.instance.HasValue("age_restriction_global_default"))
			{
				int value2 = OnlineSettings.instance.GetValue("age_restriction_global_default", 0);
				return value2 >= 0;
			}
			return true;
		}

		private static bool HasValidData()
		{
			int ageRestrictionInputMonth = UserSave.ageRestrictionInputMonth;
			int ageRestrictionInputYear = UserSave.ageRestrictionInputYear;
			if (ageRestrictionInputMonth > 0 && ageRestrictionInputYear > 0)
			{
				int ageRestrictionInputVersion = UserSave.ageRestrictionInputVersion;
				int value = OnlineSettings.instance.GetValue("age_restriction_version", 0);
				return ageRestrictionInputVersion >= value;
			}
			return false;
		}

		private static void LogBirthdayChanged()
		{
			DateTime userBirthday = GetUserBirthday();
			if (userBirthday != default(DateTime))
			{
				FlurryAnalytics.SetAge(GetUserAge());
				Manager.LogEvent(EventType.PlayerBirthdayEntered);
				string text = ((userBirthday.Month <= 6) ? "h1" : "h2");
				Flurry.LogEventWithAParameter("Player Birthday entered", "Player birthday", "{0}-{1}".Populate(userBirthday.Year, text));
			}
		}

		private static bool IsConsentGiven()
		{
			return AdsSave.behavioralAdsAllowed;
		}

		public static void RegisterConsent(bool given)
		{
			AdsSave.behavioralAdsAllowed = given;
			FireRestrictionChangedEvent();
		}

		private static bool IsAgeRestricted()
		{
			if (!HasValidData())
			{
				return false;
			}
			int userAge = GetUserAge();
			int ageThreshold = GetAgeThreshold();
			return userAge < ageThreshold;
		}

		private static int GetUserAge()
		{
			int ageRestrictionInputMonth = UserSave.ageRestrictionInputMonth;
			int ageRestrictionInputYear = UserSave.ageRestrictionInputYear;
			DateTime dateTime = new DateTime(ageRestrictionInputYear, ageRestrictionInputMonth, DateTime.DaysInMonth(ageRestrictionInputYear, ageRestrictionInputMonth));
			DateTime today = DateTime.Today;
			int num = today.Year - dateTime.Year;
			if (dateTime > today.AddYears(-num))
			{
				num--;
			}
			return Math.Max(num, 0);
		}

		private static int GetAgeThreshold()
		{
			if (OnlineSettings.instance.HasValue("age_restriction_threshold"))
			{
				int defaultValue = ((!DEFAULT_COUNTRY_CODE_RESTRICTIONS.ContainsKey(CACHED_COUNTRY_CODE)) ? 13 : DEFAULT_COUNTRY_CODE_RESTRICTIONS[CACHED_COUNTRY_CODE]);
				return OnlineSettings.instance.GetValue("age_restriction_threshold", defaultValue);
			}
			if (DEFAULT_COUNTRY_CODE_RESTRICTIONS.ContainsKey(CACHED_COUNTRY_CODE))
			{
				return DEFAULT_COUNTRY_CODE_RESTRICTIONS[CACHED_COUNTRY_CODE];
			}
			return OnlineSettings.instance.GetValue("age_restriction_global_default", 13);
		}

		public static void RegisterUserBirthday(DateTime birthday)
		{
			if (HasValidData())
			{
			}
			UserSave.ageRestrictionInputVersion = OnlineSettings.instance.GetValue("age_restriction_version", 0);
			UserSave.ageRestrictionInputMonth = birthday.Month;
			UserSave.ageRestrictionInputYear = birthday.Year;
			LogBirthdayChanged();
			FireRestrictionChangedEvent();
		}

		private static void FireRestrictionChangedEvent()
		{
			if (IsAgeRestricted())
			{
				restrictionChanged__BackingField.SafeInvoke();
			}
			else if (IsConsentGiven())
			{
				restrictionChanged__BackingField.SafeInvoke();
			}
		}

		public static DateTime GetUserBirthday()
		{
			if (HasValidData())
			{
				int ageRestrictionInputMonth = UserSave.ageRestrictionInputMonth;
				int ageRestrictionInputYear = UserSave.ageRestrictionInputYear;
				return new DateTime(ageRestrictionInputYear, ageRestrictionInputMonth, 1);
			}
			return default(DateTime);
		}

		[Conditional("AGE_RESTRICTION_LOGS")]
		private static void Log(string msg)
		{
		}
	}
}
