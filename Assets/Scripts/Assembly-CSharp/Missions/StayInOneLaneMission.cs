using System.Collections;
using Kiloo.Common.Utility;
using SYBO.Subway;
using SYBO.Subway.Characters;
using UnityEngine;

namespace Missions
{
	public class StayInOneLaneMission
	{
		private const int PROGRESS_INCREMENTS = 1;

		private bool _hasInited;

		private Character _character;

		private IEnumerator _countProgressCoroutine;

		private bool _coroutineIsRunning;

		private int _currentProgress;

		private static StayInOneLaneMission _instance;

		public static StayInOneLaneMission instance
		{
			get
			{
				return _instance ?? (_instance = new StayInOneLaneMission());
			}
		}

		public void Init()
		{
			if (!_hasInited)
			{
				_character = Character.Instance;
				if (_character != null)
				{
					_character.OnChangeTrack += OnTrackChanged;
				}
				Game.OnRunEnded += OnRunEnded;
				Game.OnMenuScreenShown += OnMenuScreenShown;
				Game.OnGameStarted += OnRunStarted;
				Game.OnGameRevived += OnRevived;
				MissionsManager.onMissionSetComplete += OnMissionSetCompleted;
				StartCountingProgressIfNeeded();
				_hasInited = true;
			}
		}

		private void OnRunStarted()
		{
			StartCountingProgressIfNeeded();
		}

		private void OnRunEnded(bool playerWasAutoRevived)
		{
			if (_coroutineIsRunning && !playerWasAutoRevived)
			{
				CoroutineUtil.StopCoroutine(_countProgressCoroutine);
				_coroutineIsRunning = false;
				_currentProgress = 0;
				int goal = 0;
				if (IsMissionActive(MissionType.StayInOneLane, out goal))
				{
					MissionsManager.Instance.RemoveProgressForThis(MissionTarget.StayInOneLane);
				}
			}
		}

		private void OnRevived()
		{
			StartCountingProgressIfNeeded();
		}

		private void OnMenuScreenShown()
		{
			if (_coroutineIsRunning)
			{
				OnRunEnded(false);
			}
		}

		private void OnTrackChanged(Character.OnChangeTrackDirection direction)
		{
			if (_coroutineIsRunning)
			{
				_currentProgress = 0;
				MissionsManager.Instance.RemoveProgressForThis(MissionTarget.StayInOneLane);
			}
		}

		private void OnMissionSetCompleted()
		{
			if (Game.Instance != null && Game.Instance.IsInGame.Value)
			{
				StartCountingProgressIfNeeded();
			}
		}

		private void StartCountingProgressIfNeeded()
		{
			int goal = 0;
			if (IsMissionActive(MissionType.StayInOneLane, out goal))
			{
				CoroutineUtil.StopCoroutine(_countProgressCoroutine);
				_countProgressCoroutine = CountMissionProgressCoroutine(goal);
				CoroutineUtil.StartCoroutine(_countProgressCoroutine);
			}
		}

		private IEnumerator CountMissionProgressCoroutine(int goal)
		{
			_coroutineIsRunning = true;
			while (_currentProgress < goal)
			{
				yield return new WaitForSeconds(1f);
				_currentProgress++;
				MissionsManager.Instance.PlayerDidThis(MissionTarget.StayInOneLane);
			}
			MissionsManager.Instance.PlayerDidThis(MissionTarget.StayInOneLane, goal);
			_coroutineIsRunning = false;
		}

		private bool IsMissionActive(MissionType missionType, out int goal)
		{
			goal = 0;
			bool result = false;
			MissionInfo[] missionInfo = MissionsManager.Instance.GetMissionInfo();
			MissionInfo[] array = missionInfo;
			for (int i = 0; i < array.Length; i++)
			{
				MissionInfo missionInfo2 = array[i];
				if (missionInfo2.mission.type == missionType && !missionInfo2.complete)
				{
					goal = missionInfo2.mission.goal;
					result = true;
				}
			}
			return result;
		}
	}
}
