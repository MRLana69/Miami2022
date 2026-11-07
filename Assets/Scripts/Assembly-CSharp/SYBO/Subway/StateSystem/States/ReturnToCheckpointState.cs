using SYBO.Common.StateSystem;
using SYBO.Subway.Characters;
using SYBO.Subway.Routes;
using UnityEngine;

namespace SYBO.Subway.StateSystem.States
{
	public class ReturnToCheckpointState : InGameState
	{
		private const float ReturnToCheckpointDelay = 0.5f;

		private const float ReturnToCheckpointDuration = 1f;

		private float _returnTime;

		private bool _startedMovingBack;

		private Vector3 _initialPosition = Vector3.zero;

		private Vector3 _targetPosition = Vector3.zero;

		public ReturnToCheckpointState(int id)
			: base(id)
		{
		}

		public override void Enter(State fromState)
		{
			base.Enter(fromState);
			_character.KillSwitchEnabled = false;
			_returnTime = 0f;
			_startedMovingBack = false;
			Character character = _character;
			int num = 1;
			_character.TrackIndexTarget = num;
			num = num;
			_character.TrackIndexCurrent = num;
			character.TrackIndexPosition = num;
			_character.TrackMovement = 0;
			_character.TrackMovementNext = 0;
			_character.IgnoredMovement = 0;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			_returnTime += Time.deltaTime;
			if (_returnTime < 0.5f)
			{
				_character.MoveWithGravity();
				return;
			}
			if (!_startedMovingBack)
			{
				_character.CharacterController.enabled = false;
				_character.ImmuneToCriticalHit = true;
				_character.StopColliding = true;
				_character.NotifyOnTutorialMoveBackToCheckPoint(1f);
				_initialPosition = _character.GamePosition;
				_targetPosition = new Vector3(0f, _initialPosition.y, _character.LastCheckpointGamePositionZ);
				_startedMovingBack = true;
			}
			if (_returnTime - 0.5f < 1f)
			{
				float t = (_returnTime - 0.5f) / 1f;
				Vector3 gamePosition = new Vector3(Mathf.SmoothStep(_initialPosition.x, _targetPosition.x, t), Mathf.SmoothStep(_initialPosition.y, _targetPosition.y, t), Mathf.SmoothStep(_initialPosition.z, _targetPosition.z, t));
				_character.SetGamePosition(gamePosition);
			}
			else
			{
				GameStateMachine.Instance.ActivateTrigger(102);
			}
		}

		public override void Leave(State toState)
		{
			base.Leave(toState);
			_character.x = Route.GetTrackX(1);
			_character.KillSwitchEnabled = true;
			_character.StopColliding = false;
			_character.ImmuneToCriticalHit = false;
			_character.CharacterController.enabled = true;
			if (toState is RunningState)
			{
				_character.NotifyOnTutorialStartFromCheckPoint();
			}
		}
	}
}
