using System;
using System.Diagnostics;
using System.Threading;
using AppRefresh;
using Data.Save;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.Notifications;
using UnityEngine;

namespace Info
{
	public static class Settings
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<bool> onTopRunChallengerSettingChanged__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onSfxSettingChanged__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onMusicSettingsChanged__BackingField;

		public static bool optionSfx
		{
			get
			{
				return PreferencesSave.soundEffectsEnabled;
			}
			set
			{
				PreferencesSave.soundEffectsEnabled = value;
				onSfxSettingChanged__BackingField.SafeInvoke();
				NGUITools.soundVolume = (optionSfx ? 1 : 0);
			}
		}

		public static bool optionMusic
		{
			get
			{
				return PreferencesSave.musicEnabled;
			}
			set
			{
				PreferencesSave.musicEnabled = value;
				onMusicSettingsChanged__BackingField.SafeInvoke();
			}
		}

		public static bool optionTopRunChallengers
		{
			get
			{
				return PreferencesSave.topRunChallengersEnabled;
			}
			set
			{
				PreferencesSave.topRunChallengersEnabled = value;
				onTopRunChallengerSettingChanged__BackingField.SafeInvoke(value);
			}
		}

		public static bool anySoundEnabled
		{
			get
			{
				return optionMusic || optionSfx;
			}
		}

		public static bool optionReminder
		{
			get
			{
				EnableReminderIfOSAllowsNotifications();
				return PreferencesSave.localNotificationsEnabled > 0;
			}
			set
			{
				PreferencesSave.localNotificationsEnabled = (value ? 1 : 0);
			}
		}

		public static event Action<bool> onTopRunChallengerSettingChanged
		{
			add
			{
				Action<bool> action = onTopRunChallengerSettingChanged__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onTopRunChallengerSettingChanged__BackingField, (Action<bool>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<bool> action = onTopRunChallengerSettingChanged__BackingField;
				Action<bool> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onTopRunChallengerSettingChanged__BackingField, (Action<bool>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onSfxSettingChanged
		{
			add
			{
				Action action = onSfxSettingChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onSfxSettingChanged__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onSfxSettingChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onSfxSettingChanged__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onMusicSettingsChanged
		{
			add
			{
				Action action = onMusicSettingsChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onMusicSettingsChanged__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onMusicSettingsChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onMusicSettingsChanged__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		static Settings()
		{
			RefreshSettings();
			AppRefreshHandler.hasRefreshed += (RefreshType refreshType) =>
			{
				if (refreshType.HasFlag(RefreshType.Data))
				{
					RefreshSettings();
				}
			};
			HookUpEvents();
			InitNotificationsIfNotInited();
		}

		private static void HookUpEvents()
		{
			if (PreferencesSave.localNotificationsEnabled == -1)
			{
				UIScreenController.OnAwake += UpdateLocalNotificationsEnabledState;
			}
			UIScreenController.OnApplicationPaused += OnApplicationPaused;
		}

		private static void InitNotificationsIfNotInited()
		{
			if (Notifications.instance == null && Application.isEditor)
			{
				Notifications.Init(false);
			}
		}

		private static void OnApplicationPaused()
		{
			UserSave.wereNotificationsEnabled = Notifications.instance.areNotificationsEnabled;
		}

		private static void UpdateLocalNotificationsEnabledState()
		{
			optionReminder = Notifications.instance.areNotificationsEnabled;
		}

		private static void RefreshSettings()
		{
			optionSfx = optionSfx;
			optionMusic = optionMusic;
			optionTopRunChallengers = optionTopRunChallengers;
		}

		private static void EnableReminderIfOSAllowsNotifications()
		{
			if (!UserSave.wereNotificationsEnabled && Notifications.instance.areNotificationsEnabled)
			{
				optionReminder = true;
			}
			UserSave.wereNotificationsEnabled = Notifications.instance.areNotificationsEnabled;
		}

		public static void ToggleCurrentSfxOption()
		{
			optionSfx = !optionSfx;
		}

		public static void ToggleCurrentMusicOption()
		{
			optionMusic = !optionMusic;
		}

		public static void ToggleOptionReminders()
		{
			optionReminder = !optionReminder;
		}

		public static void EnableAllSoundOptions()
		{
			optionSfx = true;
			optionMusic = true;
		}

		public static void DisableAllSoundOptions()
		{
			optionSfx = false;
			optionMusic = false;
		}
	}
}
