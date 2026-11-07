using System;
using System.Collections;
using System.Collections.Generic;
using Info;
using Kiloo.Common.Utility;
using SYBO.Subway;
using UnityEngine;

namespace Kiloo.Analytics
{
	internal static class RunTracker
	{
		private static bool _initialized;

		private static bool _initialMenuScreenShown;

		private static float _runLengthInSeconds;

		private static IEnumerator _runLengthTracker;

		private static int _saveMeCountInRun;

		private static bool _hasUsedSaveMe;

		private static float? _runLengthUntilFirstSaveMe;

		private static int _baseScoreMultiplierAtStartOfRun;

		private static int _hoverboardCountInRun;

		private static bool _wasFirstMysteryBoxPicked;

		private static int _MBCount;

		private static float _firstMysteryBoxCollectedAfterSeconds;

		private static bool _runDidEnd;

		private static bool _runWasAborted;

		private static bool _isReadyToLog;

		private static IEnumerator _runEnded;

		public static void LogRunEndedData(Action<Event> onLogReady)
		{
			if (_runEnded == null)
			{
				_runEnded = RunEnded(onLogReady);
				CoroutineUtil.StartCoroutine(_runEnded);
			}
		}

		private static IEnumerator RunEnded(Action<Event> onLogReady)
		{
			yield return _isReadyToLog;
			onLogReady(GetRunEndedData());
			_runEnded = null;
		}

		private static Event GetRunEndedData()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("len_sec", Math.Round(_runLengthInSeconds, 1));
			dictionary.Add("save_count", _saveMeCountInRun);
			dictionary.Add("len_save", (!_runLengthUntilFirstSaveMe.HasValue) ? null : ((object)Math.Round(_runLengthUntilFirstSaveMe.Value, 1)));
			dictionary.Add("multiply", _baseScoreMultiplierAtStartOfRun);
			dictionary.Add("hovb_count", _hoverboardCountInRun);
			dictionary.Add("aborted", Convert.ToInt32(_runWasAborted));
			dictionary.Add("MB_cnt", _MBCount);
			dictionary.Add("MB1_sec", Math.Round(_firstMysteryBoxCollectedAfterSeconds, 1));
			dictionary.Add("d_let_cnt", GameStats.Instance.DailyLetterPickupsCollectedRun);
			dictionary.Add("c_let_cnt", GameStats.Instance.CharacterLetterPickupsCollectedRun);
			Dictionary<string, object> dictionary2 = dictionary;
			Dictionary<string, string> dictionary3 = new Dictionary<string, string>(dictionary2.Count);
			foreach (KeyValuePair<string, object> item in dictionary2)
			{
				dictionary3.Add(item.Key, (item.Value == null) ? string.Empty : item.Value.ToString());
			}
			Flurry.LogEventWithSeveralParameters("run_ended", dictionary3);
			return new Event("run_ended", dictionary2);
		}

		public static void Initialize()
		{
			if (!_initialized)
			{
				Game.OnGameStarted += OnGameStarted;
				Game.OnGameEnded += OnGameEnded;
				Game.OnRunEnded += OnRunEnded;
				Game.OnMenuScreenShown += OnMenuScreenShown;
				_initialized = true;
			}
		}

		private static void OnGameStarted()
		{
			_runLengthInSeconds = 0f;
			_saveMeCountInRun = 0;
			_hasUsedSaveMe = false;
			_runLengthUntilFirstSaveMe = null;
			_baseScoreMultiplierAtStartOfRun = ScoreMultiplierInfo.rawMultiplier;
			_hoverboardCountInRun = 0;
			_wasFirstMysteryBoxPicked = false;
			_MBCount = 0;
			_firstMysteryBoxCollectedAfterSeconds = -1f;
			_runDidEnd = false;
			_runWasAborted = false;
			_isReadyToLog = false;
			_runLengthTracker = TrackRunLengthInSeconds();
			CoroutineUtil.StartCoroutine(_runLengthTracker);
			Revive.Instance.OnRevive += OnRevive;
			GameStats.Instance.PowerupTriggered += OnPowerupTriggered;
			GameStats.Instance.OnMysteryBoxPickup += OnMysteryOrSuperMysteryBoxPickup;
			GameStats.Instance.OnSuperMysteryBoxPickup += OnMysteryOrSuperMysteryBoxPickup;
		}

		private static void OnGameEnded()
		{
			Revive.Instance.OnRevive -= OnRevive;
			GameStats.Instance.PowerupTriggered -= OnPowerupTriggered;
			GameStats.Instance.OnMysteryBoxPickup -= OnMysteryOrSuperMysteryBoxPickup;
			GameStats.Instance.OnSuperMysteryBoxPickup -= OnMysteryOrSuperMysteryBoxPickup;
		}

		private static void OnRunEnded(bool playerWasAutoRevived)
		{
			_runDidEnd = true;
		}

		private static void OnMenuScreenShown()
		{
			if (!_initialMenuScreenShown)
			{
				_initialMenuScreenShown = true;
				return;
			}
			CoroutineUtil.StopCoroutine(_runLengthTracker);
			_runLengthTracker = null;
			GameStats.Instance.PowerupTriggered -= OnPowerupTriggered;
			Revive.Instance.OnRevive -= OnRevive;
			GameStats.Instance.OnMysteryBoxPickup -= OnMysteryOrSuperMysteryBoxPickup;
			GameStats.Instance.OnSuperMysteryBoxPickup -= OnMysteryOrSuperMysteryBoxPickup;
			_runWasAborted = !_runDidEnd;
			_isReadyToLog = true;
		}

		private static IEnumerator TrackRunLengthInSeconds()
		{
			while (true)
			{
				yield return null;
				_runLengthInSeconds += Time.deltaTime;
			}
		}

		private static void OnRevive()
		{
			_saveMeCountInRun++;
			_runDidEnd = false;
			if (!_hasUsedSaveMe)
			{
				_hasUsedSaveMe = true;
				_runLengthUntilFirstSaveMe = _runLengthInSeconds;
			}
		}

		private static void OnPowerupTriggered(UpgradeType upgradeType)
		{
			if (upgradeType == UpgradeType.hoverboard && !Hoverboard.Instance.IsARewardFromMysteryPowerup)
			{
				_hoverboardCountInRun++;
			}
		}

		private static void OnMysteryOrSuperMysteryBoxPickup()
		{
			_MBCount++;
			if (!_wasFirstMysteryBoxPicked)
			{
				_firstMysteryBoxCollectedAfterSeconds = _runLengthInSeconds;
				_wasFirstMysteryBoxPicked = true;
			}
		}
	}
}
