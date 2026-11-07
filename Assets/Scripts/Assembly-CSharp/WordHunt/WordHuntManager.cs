#define ENABLE_ERROR_LOGS
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;
using AppRefresh;
using Data.Save;
using Info;
using Kiloo;
using Kiloo.Common.Extensions;
using Kiloo.Common.Utility;
using Missions;
using SYBO.Components.Utilities;
using SYBO.Subway.Pickups;
using SYBO.Subway.Pickups.Pickers;
using UI.Screens.RewardManagement;
using UI.SlideIns;
using UnityEngine;

namespace WordHunt
{
	public class WordHuntManager
	{
		private enum WordHuntServerData
		{
			wordIndex = 0,
			dailyWord = 1,
			year = 2,
			checksum = 3,
			month = 4,
			day = 5,
			hour = 6,
			minute = 7,
			second = 8,
			expireSeconds = 9
		}

		public class WordHuntPayload
		{
			public bool todayCompleted;

			public int activeDayIndex;
		}

		public enum WordHuntRewardType
		{
			Coins = 0,
			MysteryBox = 1
		}

		public struct WordHuntReward
		{
			public WordHuntRewardType type;

			public int coins;

			public MysteryBox.Type mysteryBoxType;
		}

		private static readonly WordHuntReward[] REWARDS = new WordHuntReward[5]
		{
			new WordHuntReward
			{
				type = WordHuntRewardType.MysteryBox,
				mysteryBoxType = MysteryBox.Type.Normal
			},
			new WordHuntReward
			{
				type = WordHuntRewardType.MysteryBox,
				mysteryBoxType = MysteryBox.Type.Normal
			},
			new WordHuntReward
			{
				type = WordHuntRewardType.Coins,
				coins = 1050
			},
			new WordHuntReward
			{
				type = WordHuntRewardType.Coins,
				coins = 1500
			},
			new WordHuntReward
			{
				type = WordHuntRewardType.MysteryBox,
				mysteryBoxType = MysteryBox.Type.Super
			}
		};

		private string[] _lastRawData;

		private DateTime _lastRequestTime = DateTime.MinValue;

		private static WordHuntManager _instance;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action OnPickedUpLetter__BackingField = null;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action OnDailyWordInRowChanged__BackingField = null;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<int> OnDailyLetterCoinsRewarded__BackingField;

		public static WordHuntManager instance
		{
			get
			{
				return _instance ?? (_instance = new WordHuntManager());
			}
		}

		public static event Action OnPickedUpLetter
		{
			add
			{
				Action action = OnPickedUpLetter__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPickedUpLetter__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnPickedUpLetter__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPickedUpLetter__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action OnDailyWordInRowChanged
		{
			add
			{
				Action action = OnDailyWordInRowChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnDailyWordInRowChanged__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnDailyWordInRowChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnDailyWordInRowChanged__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<int> OnDailyLetterCoinsRewarded
		{
			add
			{
				Action<int> action = OnDailyLetterCoinsRewarded__BackingField;
				Action<int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnDailyLetterCoinsRewarded__BackingField, (Action<int>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<int> action = OnDailyLetterCoinsRewarded__BackingField;
				Action<int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnDailyLetterCoinsRewarded__BackingField, (Action<int>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		private WordHuntManager()
		{
			UIScreenController.OnChangedScreen += AutoSyncer;
			AppRefreshHandler.hasRefreshed += AppHasRefreshed;
		}

		private void AppHasRefreshed(RefreshType obj)
		{
			if (obj.HasFlag(RefreshType.Data))
			{
				if (!IsAllowedToMakeRequest(CDateTime.UtcNow) && _lastRawData != null)
				{
					LoadFromRawData(_lastRawData);
				}
				else
				{
					DownloadDaily();
				}
			}
		}

		private void AutoSyncer(string screenName)
		{
			if (screenName == "FrontScreen" || screenName == "GameOverScreen")
			{
				DownloadDailyIfExpired();
			}
		}

		public void DownloadDaily()
		{
			CoroutineUtil.StartCoroutine(DownloadDailyCoroutine());
		}

		public void DownloadDailyIfExpired()
		{
			if (EventsSave.wordHuntTimeLeft.TotalSeconds <= 0.0)
			{
				CoroutineUtil.StartCoroutine(DownloadDailyCoroutine());
			}
		}

		private IEnumerator DownloadDailyCoroutine()
		{
			// O servidor da caça-palavras diária (http, Kiloo) não existe mais; no navegador o http ainda é bloqueado
			if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WebGLPlayer)
			{
				yield break;
			}
			DateTime time = CDateTime.UtcNow;
			if (!IsAllowedToMakeRequest(time))
			{
				yield break;
			}
			_lastRequestTime = time;
			string key = GenerateKey();
			WWWForm postData = new WWWForm();
			postData.AddField("key", key);
			WWW www = new WWW("http://hoodrunner.kiloo.com/hr_dailyquests2.php", postData);
			yield return www;
			if (www.error == null)
			{
				string[] array = www.text.Split(';');
				if (SHA1HashCheck(array, key))
				{
					_lastRawData = array;
					LoadFromRawData(array);
				}
				else
				{
					LogError("DailyWord.DownloadDailyCoroutine: Checksum failed");
				}
			}
			else
			{
				LogWarning("DailyWord.DownloadDailyCoroutine: www.error: " + www.error);
			}
		}

		private bool IsAllowedToMakeRequest(DateTime time)
		{
			return (time - _lastRequestTime).TotalSeconds > 120.0;
		}

		private void LoadFromRawData(string[] rawData)
		{
			string word = rawData[1];
			int secondsLeft = Convert.ToInt32(rawData[9]);
			CalculateAndSaveLastKnownServerTime(rawData[2], rawData[4], rawData[5], rawData[6], rawData[7], rawData[8]);
			InitWordHunt(word, secondsLeft);
		}

		private void CalculateAndSaveLastKnownServerTime(string year, string month, string day, string hour, string minute, string second)
		{
			try
			{
				DateTime lastKnownServerTime = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), int.Parse(minute), int.Parse(second), DateTimeKind.Utc);
				UserSave.lastKnownServerTime = lastKnownServerTime;
			}
			catch (Exception ex)
			{
				LogError("DailyWord.CalculateAndSaveLastKnownServerTime: Was not able to parse the date received from the server. Exception: " + ex.Message);
			}
		}

		private bool SHA1HashCheck(string[] rawData, string key)
		{
			if (rawData.Length < 10)
			{
				return false;
			}
			string a = rawData[3];
			string b = Kiloo.Common.Utility.StringUtility.ToHexString(Checksum.ComputeSHA1(rawData[0] + rawData[1] + rawData[2] + key + "aIN0UXP4NNoANVGi5w3raGAFN1n5OLQZFDhwjs6HoX" + rawData[4] + rawData[5] + rawData[6] + rawData[7] + rawData[8] + rawData[9]), true);
			if (string.Equals(a, b, Utils.stringComparisonIgnoreCase))
			{
				return true;
			}
			return false;
		}

		private static string RandomString(int size)
		{
			StringBuilder stringBuilder = new StringBuilder();
			System.Random random = new System.Random();
			for (int i = 0; i < size; i++)
			{
				char value = Convert.ToChar(Convert.ToInt32(Math.Floor(26.0 * random.NextDouble() + 65.0)));
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		private static int RandomNumber(int min, int max)
		{
			System.Random random = new System.Random();
			return random.Next(min, max);
		}

		private static string GenerateKey()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(RandomString(4));
			stringBuilder.Append(RandomNumber(0, 999));
			stringBuilder.Append(RandomString(2));
			return stringBuilder.ToString();
		}

		public static WordHuntPayload GetWordHuntPayload()
		{
			bool todayCompleted;
			int num = Mathf.Clamp(GetWordHuntDaysInARow(out todayCompleted), 0, 5);
			int activeDayIndex = ((!todayCompleted) ? num : (num - 1));
			WordHuntPayload wordHuntPayload = new WordHuntPayload();
			wordHuntPayload.activeDayIndex = activeDayIndex;
			wordHuntPayload.todayCompleted = todayCompleted;
			return wordHuntPayload;
		}

		public static WordHuntReward GetRewardForDay(int dayIndex)
		{
			if (dayIndex >= REWARDS.Length)
			{
				dayIndex = REWARDS.Length - 1;
			}
			return REWARDS[dayIndex];
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

		[Conditional("ENABLE_DAILYWORD_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}

		private static void InitWordHunt(string word, int secondsLeft)
		{
			if (Globals.DEBUG_USE_DEBUG_DAILYWORD)
			{
				word = "Day";
				secondsLeft++;
			}
			if (!EventsSave.wordHuntDailyWord.Equals(word))
			{
				EventsSave.wordHuntDailyWord = word;
				EventsSave.wordHuntPayedOut = false;
				EventsSave.wordHuntUnlockedMask = 0;
			}
			if (!EventsSave.wordHuntPayedOut)
			{
				SingletonMonoBehaviour<PickupManager>.Instance.SetPickerState(PickerType.LetterHuntDaily, PickerEnabledState.Enabled);
				DailyLetterPickupManager.Instance.UpdateLetter();
			}
			DateTime dateTime = UserSave.lastKnownServerTime.AddSeconds(secondsLeft);
			DateTime wordHuntExpireDeviceTime = CDateTime.UtcNow.AddSeconds(secondsLeft);
			if (!EventsSave.wordHuntExpireTime.Equals(dateTime))
			{
				EventsSave.wordHuntExpireTime = dateTime;
				EventsSave.wordHuntExpireDeviceTime = wordHuntExpireDeviceTime;
			}
		}

		public static void PickedupLetter(char letter)
		{
			for (int i = 0; i < EventsSave.wordHuntDailyWord.Length; i++)
			{
				if (EventsSave.wordHuntDailyWord[i] == letter && !EventsSave.wordHuntUnlockedMask[i])
				{
					IntMask wordHuntUnlockedMask = EventsSave.wordHuntUnlockedMask;
					wordHuntUnlockedMask[i] = true;
					EventsSave.wordHuntUnlockedMask = wordHuntUnlockedMask;
					OnPickedUpLetter__BackingField.SafeInvoke();
					break;
				}
			}
			if (!IsDailyWordComplete() || EventsSave.wordHuntPayedOut)
			{
				return;
			}
			bool todayCompleted;
			EventsSave.wordHuntWordsInRow = GetWordHuntDaysInARow(out todayCompleted) + 1;
			EventsSave.wordHuntLastPayoutDayOfYear = EventsSave.wordHuntExpireTime.DayOfYear;
			Action onDailyWordInRowChanged__BackingField = OnDailyWordInRowChanged__BackingField;
			if (onDailyWordInRowChanged__BackingField != null)
			{
				onDailyWordInRowChanged__BackingField();
			}
			MissionsManager.Instance.PlayerDidThis(MissionTarget.DailyQuests);
			MissionsManager.Instance.PlayerDidThis(MissionTarget.DailyQuestInRow);
			WordHuntReward rewardForDay = GetRewardForDay(EventsSave.wordHuntWordsInRow - 1);
			if (rewardForDay.type == WordHuntRewardType.Coins)
			{
				if (rewardForDay.coins > 0)
				{
					InventorySave.IncreaseCoins(rewardForDay.coins, "WeeklyHunt");
					UIScreenController.Instance.SetCoinRewardAmount(rewardForDay.coins);
					UIScreenController.Instance.QueueSlideIn(new SlideIn
					{
						type = SlideInData.SlideInType.LettersCompleteCoins
					});
					OnDailyLetterCoinsRewarded__BackingField.SafeInvoke(rewardForDay.coins);
				}
			}
			else if (rewardForDay.type == WordHuntRewardType.MysteryBox)
			{
				CelebrationRewardOrigin origin = ((rewardForDay.mysteryBoxType == MysteryBox.Type.Normal) ? CelebrationRewardOrigin.MysteryBox : CelebrationRewardOrigin.SuperMysteryBox);
				RewardManager.AddRewardToUnlock(origin, ObtainableItemSource.DailyLetters);
				UIScreenController.Instance.QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.LettersCompleteMysteryBox,
					mysteryBoxType = rewardForDay.mysteryBoxType
				});
				EventsSave.willShowSMBAfterDailyChallenge = true;
			}
			else
			{
				SDebug.LogError("Unhandled DailyWordRewardType: " + rewardForDay.type);
			}
			EventsSave.wordHuntPayedOut = true;
			Flurry.LogEventWithAParameter("Daily Challenge completed", "Id", EventsSave.wordHuntDailyWord);
		}

		public static char GetNewDailyLetter()
		{
			for (int i = 0; i < EventsSave.wordHuntDailyWord.Length; i++)
			{
				if (!EventsSave.wordHuntUnlockedMask[i])
				{
					return EventsSave.wordHuntDailyWord[i];
				}
			}
			return '\0';
		}

		public static bool IsDailyWordComplete()
		{
			if (string.IsNullOrEmpty(EventsSave.wordHuntDailyWord))
			{
				return false;
			}
			return (1 << EventsSave.wordHuntDailyWord.Length) - 1 == (int)EventsSave.wordHuntUnlockedMask;
		}

		public static int GetWordHuntDaysInARow(out bool todayCompleted)
		{
			int dayOfYear = EventsSave.wordHuntExpireTime.DayOfYear;
			if (EventsSave.wordHuntLastPayoutDayOfYear == dayOfYear)
			{
				todayCompleted = true;
				return EventsSave.wordHuntWordsInRow;
			}
			if (EventsSave.wordHuntLastPayoutDayOfYear == dayOfYear - 1 || (dayOfYear == 1 && EventsSave.wordHuntLastPayoutDayOfYear >= 365))
			{
				todayCompleted = false;
				return EventsSave.wordHuntWordsInRow;
			}
			todayCompleted = false;
			return 0;
		}

		public static void CheckIfWeShouldRemoveProgressForDailyQuestInRow()
		{
			bool todayCompleted;
			if (GetWordHuntDaysInARow(out todayCompleted) == 0 && !todayCompleted)
			{
				MissionsManager.Instance.RemoveProgressForThis(MissionTarget.DailyQuestInRow);
			}
		}

		public static bool IsActiveDailyLetterEvent()
		{
			return GetNewDailyLetter() != '\0';
		}

		public static bool HasTimeLeft()
		{
			return IsActiveDailyLetterEvent() && EventsSave.wordHuntTimeLeft.TotalSeconds > 0.0;
		}
	}
}
