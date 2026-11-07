using System.Collections;
using Kiloo.Common.Utility;
using Missions;
using SYBO.Common.Managers;
using SYBO.Common.StateSystem;
using SYBO.Components.Utilities;
using UI.Screens;
using UnityEngine;

namespace SYBO.Subway.StateSystem.States
{
	public class DeathState : State
	{
		private const float WaitTimeBeforeSkip = 0.5f;

		private const float WaitTimeBeforeScreen = 3f;

		private float _currentWaitTime;

		private bool _hasShownSaveMePopup;

		public DeathState(int id)
			: base(id)
		{
		}

		public override void Enter(State fromState)
		{
			base.Enter(fromState);
			SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale = SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScaleInMenuDefault;
			Game.InvokeReviveOffer();
			_currentWaitTime = 0f;
			_hasShownSaveMePopup = false;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			_currentWaitTime += Time.deltaTime;
			if (_currentWaitTime < 0.5f || PauseScreen.pauseScreenIsShown)
			{
				return;
			}
			MissionsManager.Instance.PlayerDidThis(MissionTarget.TimeDeath, Mathf.FloorToInt(Game.Instance.ElapsedGameDuration));
			if (!_hasShownSaveMePopup && SaveMeManager.CanQueueSaveMePopup())
			{
				UIScreenController.Instance.QueuePopup("SaveMePopup");
				_hasShownSaveMePopup = true;
			}
			if (_currentWaitTime >= 3f)
			{
				ReturnToTopMenu();
			}
			else if (Input.GetMouseButtonUp(0))
			{
				ReturnToTopMenu();
			}
			else if (Input.touchCount > 0)
			{
				Touch touch = Input.touches[0];
				if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
				{
					ReturnToTopMenu();
				}
			}
			Game.Instance.ingameTouchDetection = false;
		}

		private void ReturnToTopMenu()
		{
			if (!_hasShownSaveMePopup)
			{
				CoroutineUtil.StartCoroutine(DelayGameOverScreen());
				Game.Instance.canRevive = false;
			}
		}

		private IEnumerator DelayGameOverScreen()
		{
			yield return null;
			UIScreenController.Instance.GameOverTriggered();
			GameStateMachine.Instance.ActivateTrigger(100);
		}
	}
}
