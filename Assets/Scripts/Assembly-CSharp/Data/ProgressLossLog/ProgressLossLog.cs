using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using AppRefresh;
using Cloud;
using Cloud.Internal;
using Data.Managers;
using Data.Save;
using Info;
using Kiloo.Common.Utility;
using UnityEngine;

namespace Data.ProgressLossLog
{
	public static class ProgressLossLog
	{
		private const string PROGRESS_KEY = "ProgressChecker_progress_score";

		private const string CLOUD_SAVE_STATE = "ProgressCheck_oldCloudState";

		private const int MINIMUM_SCORE_THRESHOLD = 2;

		private static bool _corruptedSave;

		private static bool _appRefreshed;

		private static bool _didAccountSwitch;

		private static string _migrationDone = "None";

		private static bool _routineInProgress;

		private static int currentProgressScore
		{
			get
			{
				return PlayerSave.completedRunCount + ScoreMultiplierInfo.rawMultiplier + InventorySave.unlockedBoards.Count + InventorySave.unlockedCharacters.Count + InventorySave.GetOutfitCount() + BoardManager.CountBoardThemesUnlocked() + Upgrades.CountUpgradeTiers();
			}
		}

		public static void Init()
		{
			SaveManager.saveGameCorrupted += OnProgressCorrupted;
			AppRefreshHandler.hasRefreshed += OnAppRefresh;
			SaveManager.saveComplete += OnSaveComplete;
			SaveManager.saveFileMigrated += OnSaveMigrated;
			SaveManager.accountSwitched += OnAccountSwitch;
		}

		private static void OnSaveMigrated(bool firstTime, bool remigrated)
		{
			if (firstTime)
			{
				_migrationDone = "firstTime";
			}
			else if (remigrated)
			{
				_migrationDone = "remigrated";
			}
		}

		private static IEnumerator LogToFlurryRoutine(string oldCloudState, int savedProgressScore)
		{
			while (AppRefreshHandler.pendingAppRefresh || AppRefreshHandler.refreshInProgress || CloudSave.operationInProgress)
			{
				yield return null;
			}
			if (savedProgressScore <= currentProgressScore)
			{
				_routineInProgress = false;
				yield break;
			}
			string currentCloudState = CloudSave.state.ToString();
			bool wasFullReset = currentProgressScore <= 2;
			if (CloudSave.state == CloudSaveState.Error)
			{
				currentCloudState = currentCloudState + "|" + CloudSave.errorState;
			}
			Dictionary<string, string> parameters = new Dictionary<string, string>
			{
				{ "Cloud State New", currentCloudState },
				{ "Cloud State Old", oldCloudState },
				{
					"Did account switch",
					_didAccountSwitch.ToString()
				},
				{
					"Corrupted File",
					_corruptedSave.ToString()
				},
				{
					"App Refreshed",
					_appRefreshed.ToString()
				},
				{ "Migration done", _migrationDone }
			};
			string eventName = ((!wasFullReset) ? "Progress Loss Partial" : "Progress Loss Full");
			Flurry.LogEventWithSeveralParameters(eventName, parameters);
			ResetState();
		}

		private static void CheckForLostProgress()
		{
			int num = PlayerPrefs.GetInt("ProgressChecker_progress_score", 0);
			if (num > 0 && num > currentProgressScore && !_routineInProgress)
			{
				_routineInProgress = true;
				string oldCloudState = PlayerPrefs.GetString("ProgressCheck_oldCloudState", "n/a");
				CoroutineUtil.StartCoroutine(LogToFlurryRoutine(oldCloudState, num));
			}
			else
			{
				SetProgressScore();
			}
		}

		private static void SetProgressScore()
		{
			PlayerPrefs.SetInt("ProgressChecker_progress_score", currentProgressScore);
			string text = CloudSave.state.ToString();
			if (CloudSave.state == CloudSaveState.Error)
			{
				text = text + "|" + CloudSave.errorState;
			}
			PlayerPrefs.SetString("ProgressCheck_oldCloudState", text);
		}

		private static void ResetState()
		{
			PlayerPrefs.DeleteKey("ProgressChecker_progress_score");
			PlayerPrefs.DeleteKey("ProgressCheck_oldCloudState");
			_corruptedSave = false;
			_appRefreshed = false;
			_didAccountSwitch = false;
			_migrationDone = "None";
			_routineInProgress = false;
		}

		private static void OnAccountSwitch()
		{
			_didAccountSwitch = true;
		}

		private static void OnSaveComplete()
		{
			CheckForLostProgress();
		}

		private static void OnAppRefresh(RefreshType refreshType)
		{
			if (refreshType.HasFlag(RefreshType.Data))
			{
				_appRefreshed = true;
			}
		}

		private static void OnProgressCorrupted(DataCorruptionType corruptionType)
		{
			if (corruptionType == DataCorruptionType.CloudDataCorruption)
			{
				_corruptedSave = true;
			}
		}

		[Conditional("PROGRESS_LOSS_LOGS")]
		private static void Log(string msg, Object obj = null)
		{
		}
	}
}
