using System;
using System.Collections.Generic;
using Data.Save;
using Kiloo.Common.Extensions;
using Missions;
using UI.Breadcrumbs;
using UnityEngine;

namespace Data.Migration
{
	public class PlayerPrefsMigrator
	{
		private const int MIGRATE_VERSION = 1;

		private const string OPTION_SFX_KEY = "OPTION_SFX";

		private const string OPTION_MUSIC_KEY = "OPTION_MUSIC";

		private const string OPTION_TOPRUNCHALLENGERS_KEY = "OPTION_TOPRUNCHALLENGERS";

		private const string OPTION_REMINDER_DEFAULT_OFF_KEY = "OPTION_REMINDER_WITH_DEFAULT_OFF";

		private const string OPTION_REMINDER_PREFERENCE_KEY = "OPTION_REMINDER_PREFERENCE";

		private const string HAS_LOGGED_GAMECENTER_LOGIN_KEY = "flurry_has_logged_gc";

		private const string HAS_LOGGED_FACEBOOK_LOGIN_KEY = "flurry_has_logged_fb";

		private const string HAS_LOGGED_DAILY_EVENT_SOCIAL_PREFIX = "socflurdaily";

		private const string AWARD_ISNEW_KEY_PREFIX = "isAwardNew";

		private const string AWARD_HASPAYED_OUT_KEY_PREFIX = "hasPayed";

		private const string AWARDS_FIRST_LOADED_KEY = "awardsFirstLoaded";

		private const string KEY_FIRST_TIME_RUN = "SetShouldShowPopupToFalse";

		private const string BREADCRUMBS_KEY = "breadcrumbs";

		private const string LAST_EVENT_TYPE_SHOWN_KEY = "lastEventTypeShown";

		private const string LAST_EVENT_SHOWN_TIMESTAMP_KEY = "lastEventShownTimestamp";

		private const string LAST_SEEN_BUNDLE_VERSION_KEY = "lastSeenBundleVersionKey";

		private const string PrefsKeyPersistedSessionEventNumber = "SessionEventNumber";

		private static string _lastKeyRead = string.Empty;

		public static void ResetMigratedVersion()
		{
			UserSave.migratedPlayerPrefsVersion = 0;
		}

		public static void Migrate()
		{
			int migratedPlayerPrefsVersion = UserSave.migratedPlayerPrefsVersion;
			string empty = string.Empty;
			for (int i = migratedPlayerPrefsVersion + 1; i <= 1; i++)
			{
				try
				{
					if (i == 1)
					{
						if (HasPlayerPrefsKey("OPTION_SFX"))
						{
							PreferencesSave.soundEffectsEnabled = PlayerPrefs.GetInt("OPTION_SFX") != 0;
						}
						if (HasPlayerPrefsKey("OPTION_MUSIC"))
						{
							PreferencesSave.musicEnabled = PlayerPrefs.GetInt("OPTION_MUSIC") != 0;
						}
						if (HasPlayerPrefsKey("OPTION_TOPRUNCHALLENGERS"))
						{
							PreferencesSave.topRunChallengersEnabled = PlayerPrefs.GetInt("OPTION_TOPRUNCHALLENGERS") != 0;
						}
						if (HasPlayerPrefsKey("OPTION_REMINDER_PREFERENCE"))
						{
							PreferencesSave.localNotificationsEnabled = PlayerPrefs.GetInt("OPTION_REMINDER_PREFERENCE");
						}
						else if (HasPlayerPrefsKey("OPTION_REMINDER_WITH_DEFAULT_OFF") && PlayerPrefs.GetInt("OPTION_REMINDER_WITH_DEFAULT_OFF", 0) == 1)
						{
							PreferencesSave.localNotificationsEnabled = 1;
						}
						if (HasPlayerPrefsKey("flurry_has_logged_gc"))
						{
							AnalyticsSave.hasLoggedGameCenterLogin = PlayerPrefs.GetInt("flurry_has_logged_gc") != 0;
						}
						if (HasPlayerPrefsKey("flurry_has_logged_fb"))
						{
							AnalyticsSave.hasLoggedFacebookLogin = PlayerPrefs.GetInt("flurry_has_logged_fb") != 0;
						}
						empty = "socflurdailySocial GameCenter Friends Retrieved";
						if (HasPlayerPrefsKey(empty))
						{
							AnalyticsSave.hasLoggedFlurryDailyEventSocial.AddOrSet("Social GameCenter Friends Retrieved", PlayerPrefs.GetInt(empty));
						}
						empty = "socflurdailySocial Facebook Friends Retrieved";
						if (HasPlayerPrefsKey(empty))
						{
							AnalyticsSave.hasLoggedFlurryDailyEventSocial.AddOrSet("Social Facebook Friends Retrieved", PlayerPrefs.GetInt(empty));
						}
						empty = "socflurdailySocial Friends Consolidated";
						if (HasPlayerPrefsKey(empty))
						{
							AnalyticsSave.hasLoggedFlurryDailyEventSocial.AddOrSet("Social Friends Consolidated", PlayerPrefs.GetInt(empty));
						}
						Array names = Enum.GetNames(typeof(AwardId));
						foreach (string item in names)
						{
							empty = "isAwardNew" + item;
							if (HasPlayerPrefsKey(empty))
							{
								bool value = PlayerPrefs.GetInt(empty, 1) != 0;
								if (Enum.IsDefined(typeof(AwardId), item))
								{
									PlayerSave.awardIsNew.AddOrSet((AwardId)Enum.Parse(typeof(AwardId), item), value);
								}
							}
							empty = "hasPayed" + item;
							if (HasPlayerPrefsKey(empty))
							{
								bool value2 = PlayerPrefs.GetInt(empty, 0) != 0;
								if (Enum.IsDefined(typeof(AwardId), item))
								{
									PlayerSave.awardHasPayedOut.AddOrSet((AwardId)Enum.Parse(typeof(AwardId), item), value2);
								}
							}
						}
						if (HasPlayerPrefsKey("awardsFirstLoaded"))
						{
							PlayerSave.awardsFirstLoaded = PlayerPrefs.GetInt("awardsFirstLoaded", 0) != 0;
						}
						if (HasPlayerPrefsKey("SetShouldShowPopupToFalse"))
						{
							UserSave.hasUserRunAppBefore = true;
						}
						if (HasPlayerPrefsKey("breadcrumbs"))
						{
							string json = PlayerPrefs.GetString("breadcrumbs");
							UserSave.breadcrumbs = JsonUtility.FromJson<Breadcrumb.BreadcrumbSave>(json);
						}
						if (HasPlayerPrefsKey("lastEventTypeShown"))
						{
							foreach (SSEventType value4 in Enum.GetValues(typeof(SSEventType)))
							{
								if (PlayerPrefs.GetString("lastEventTypeShown") == value4.ToString())
								{
									UserSave.lastEventTypeShown = value4;
								}
							}
						}
						if (HasPlayerPrefsKey("lastEventShownTimestamp"))
						{
							string s = PlayerPrefs.GetString("lastEventShownTimestamp");
							DateTime result = DateTime.MinValue;
							if (DateTime.TryParse(s, out result))
							{
								UserSave.lastEventShownTimestamp = result;
							}
						}
						if (HasPlayerPrefsKey("lastSeenBundleVersionKey"))
						{
							UserSave.lastSeenBundleVersion = PlayerPrefs.GetString("lastSeenBundleVersionKey");
						}
						if (HasPlayerPrefsKey("SessionEventNumber"))
						{
							SyboAnalyticsSave.persistedSessionEventNumber = PlayerPrefs.GetInt("SessionEventNumber");
						}
					}
				}
				catch (Exception ex)
				{
					string value3 = string.Empty;
					if (HasPlayerPrefsKey("lastSeenBundleVersionKey"))
					{
						value3 = PlayerPrefs.GetString("lastSeenBundleVersionKey", string.Empty);
					}
					string text2 = ex.GetType().ToString();
					string lastKeyRead = _lastKeyRead;
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("Exception Type", text2);
					dictionary.Add("Last loaded key", lastKeyRead);
					dictionary.Add("Migrated from", value3);
					dictionary.Add(text2, lastKeyRead);
					Dictionary<string, string> parameters = dictionary;
					Flurry.LogEventWithSeveralParameters("PlayerPrefs Migration error", parameters);
					break;
				}
				UserSave.migratedPlayerPrefsVersion = i;
			}
			SaveManager.Save();
		}

		public static bool HasPlayerPrefsKey(string key)
		{
			_lastKeyRead = key;
			return PlayerPrefs.HasKey(_lastKeyRead);
		}

		public static bool CanMigrate()
		{
			if (UserSave.migratedPlayerPrefsVersion == 1)
			{
				return false;
			}
			return true;
		}

		public static void TryCleanPlayerPrefsKeys()
		{
			int migratedPlayerPrefsVersion = UserSave.migratedPlayerPrefsVersion;
			for (int i = 0; i < migratedPlayerPrefsVersion; i++)
			{
				if (i == 1)
				{
				}
			}
		}
	}
}
