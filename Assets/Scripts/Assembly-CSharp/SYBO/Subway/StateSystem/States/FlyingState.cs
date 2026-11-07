using System;
using System.Collections.Generic;
using Info;
using SYBO.Common.StateSystem;
using SYBO.Components.Utilities;
using SYBO.Subway.Cameras;
using SYBO.Subway.Inputs;
using SYBO.Subway.Level;
using SYBO.Subway.Routes;
using SYBO.Subway.Routes.Builders;
using SYBO.Subway.StateSystem.States.Settings;
using UnityEngine;

namespace SYBO.Subway.StateSystem.States
{
	public abstract class FlyingState : InGameState
	{
		private const float MaxJetpackFlightDurationIncreaseSeconds = 4f;

		private FlyingStateSettings _settings;

		public Action OnStart;

		public Action OnStop;

		public Action OnFlyAheadStart;

		public Action<float> OnFlyAheadUpdate;

		private ActivePowerup _powerup;

		private List<Action> _startHandlers = new List<Action>();

		protected float _flyAheadDistance;

		protected float _speedPositionAdjustment;

		private float _startGamePositionZ;

		protected int _targetGamePositionZ;

		private float _startY;

		private Vector3 _startCameraOffset = Vector3.zero;

		private float _startCameraAimOffset;

		private SmoothDampFloat _smoothCameraX;

		private Vector3 _cameraPositionStart = Vector3.zero;

		private Vector3 _cameraTargetStart = Vector3.zero;

		private bool _hasExploded;

		protected FlyingStateSettings FlyingStateSettings
		{
			get
			{
				return _settings;
			}
		}

		protected abstract UpgradeType UpgradeType { get; }

		public int TargetGamePositionZ
		{
			get
			{
				return _targetGamePositionZ;
			}
		}

		public FlyingState(int id)
			: base(id)
		{
			_settings = ScriptableSingleton<FlyingStateSettings>.instance;
		}

		public override void Enter(State fromState)
		{
			base.Enter(fromState);
			GamePositionRoot.Instance.AddLock(this);
			_powerup = GameStats.Instance.TriggerPowerup(UpgradeType);
			_characterCamera.SetCameraState(CharacterCamera.CameraState.Custom);
			_character.CharacterRendering.ResetTeleportFx();
			_character.IsGrounded.Value = false;
			NotifyOnStart();
			_game.Modifiers.PauseInJetpackMode();
			if (_character.IsStumbling)
			{
				_character.StopStumble();
			}
			_character.CharacterController.detectCollisions = false;
			_character.CharacterTrigger.enabled = false;
			_startY = _character.transform.position.y;
			float flyingDuration = GetFlyingDuration();
			float num = GetFlyingSpeed();
			float num2 = num * flyingDuration;
			_flyAheadDistance = num * _settings.FlyAheadDuration;
			float num3 = _flyAheadDistance + num2;
			DefaultRouteBuilder defaultRouteBuilder = SingletonMonoBehaviour<RouteManager>.Instance.CharacterRoute.Builder as DefaultRouteBuilder;
			float lastSectionMinEndGamePosition = defaultRouteBuilder.GetLastSectionMinEndGamePosition();
			float num4 = _character.GamePosition.z + num3;
			int targetGamePositionZ = Mathf.CeilToInt(num4 / 180f) * 180;
			if (lastSectionMinEndGamePosition > num4)
			{
				_targetGamePositionZ = (int)lastSectionMinEndGamePosition;
			}
			else
			{
				_targetGamePositionZ = targetGamePositionZ;
			}
			_speedPositionAdjustment = _settings.StopBeforeLandingChunkDistance * Game.Instance.CurrentSpeedNormalized;
			float num5 = (float)_targetGamePositionZ - _character.GamePosition.z - _speedPositionAdjustment;
			defaultRouteBuilder.EnqueueJetpackEndSection(_targetGamePositionZ);
			float num6 = num5 / num;
			float num7 = flyingDuration + 4f;
			if (num7 < num6)
			{
				float num8 = num6 / num7;
				num *= num8;
			}
			_startGamePositionZ = _character.GamePosition.z;
			_game.SetCurrentSpeed(num);
			_startCameraOffset = _characterCamera.transform.position - _character.transform.position;
			_startCameraAimOffset = _characterCamera.CameraData.CameraTargetOffset.y;
			_smoothCameraX = new SmoothDampFloat(_characterCamera.transform.position.x, _settings.SmoothCameraXDuration);
			_cameraPositionStart = _characterCamera.Position;
			_cameraTargetStart = _characterCamera.Target;
			if (OnFlyAheadStart != null)
			{
				OnFlyAheadStart();
			}
			_hasExploded = false;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			float ratioRemainingForFlyAhead = GetRatioRemainingForFlyAhead();
			if (ratioRemainingForFlyAhead < 1f)
			{
				UpdateFlyAhead(ratioRemainingForFlyAhead);
				return;
			}
			if (!_character.CharacterTrigger.enabled)
			{
				_character.CharacterTrigger.enabled = true;
			}
			ratioRemainingForFlyAhead = GetRatioRemainingForFlying();
			if (ratioRemainingForFlyAhead < 1f)
			{
				UpdateFlying(ratioRemainingForFlyAhead);
			}
			else
			{
				GameStateMachine.Instance.ActivateTrigger(102);
			}
		}

		private void UpdateFlyAhead(float remainingRatio)
		{
			Vector3 vector = Vector3.Lerp(_startCameraOffset, _characterCamera.CameraData.CameraPositionOffset, Mathf.SmoothStep(0f, 1f, remainingRatio));
			float num = Mathf.Lerp(_startCameraAimOffset, _settings.CameraAimOffsetY, Mathf.SmoothStep(0f, 1f, remainingRatio));
			_character.SetGamePosition(_character.GamePosition + _game.CurrentSpeed * Time.deltaTime * Vector3.forward);
			Vector3 vector2 = Route.GetPosition(_character.x, _character.GamePosition.z) + Vector3.up * Mathf.SmoothStep(_startY, _settings.FlyHeight, remainingRatio);
			_character.SetGamePosition(vector2);
			_smoothCameraX.Update();
			_smoothCameraX.Target = _character.GamePosition.x * 0.9f;
			Vector3 vector3 = vector2;
			vector3.z = _character.TransformPosition.z;
			Vector3 b = new Vector3(_smoothCameraX.Value, vector3.y, _character.TransformPosition.z) + vector;
			Vector3 b2 = new Vector3(_smoothCameraX.Value, vector3.y, vector3.z) + Vector3.up * num;
			float t = _settings.FlyAheadCameraMovement.Evaluate(remainingRatio);
			Vector3 position = Vector3.Lerp(_cameraPositionStart, b, t);
			Vector3 target = Vector3.Lerp(_cameraTargetStart, b2, t);
			position.x = _smoothCameraX.Value;
			target.x = _smoothCameraX.Value;
			_characterCamera.Position = position;
			_characterCamera.Target = target;
			if (OnFlyAheadUpdate != null)
			{
				OnFlyAheadUpdate(remainingRatio);
			}
			if (!_hasExploded && vector2.y > _character.SubwayMaxY && _character.IsInsideSubway)
			{
				_character.TurboHeadstartParticleSystem.Play();
				_hasExploded = true;
				_character.ForceLeaveSubway();
			}
		}

		protected virtual void UpdateFlying(float remainingRatio)
		{
			Vector3 vector = _character.GamePosition + _game.CurrentSpeed * Time.deltaTime * Vector3.forward;
			vector = Route.GetPosition(_character.x, vector.z) + Vector3.up * GetFlyHeight();
			_character.SetGamePosition(vector);
			_smoothCameraX.Update();
			_smoothCameraX.Target = _character.GamePosition.x * 0.9f;
			Vector3 transformPosition = _character.TransformPosition;
			Vector3 position = transformPosition + _characterCamera.CameraData.CameraPositionOffset;
			position.x = _smoothCameraX.Value;
			_characterCamera.Position = position;
			transformPosition.x = _smoothCameraX.Value;
			_characterCamera.Target = transformPosition + Vector3.up * _settings.CameraAimOffsetY;
		}

		private float GetRatioRemainingForFlyAhead()
		{
			return (_character.GamePosition.z - _startGamePositionZ) / _flyAheadDistance;
		}

		private float GetRatioRemainingForFlying()
		{
			return (_character.GamePosition.z - _startGamePositionZ - _flyAheadDistance) / ((float)_targetGamePositionZ - _startGamePositionZ - _flyAheadDistance);
		}

		protected virtual float GetFlyHeight()
		{
			return _settings.FlyHeight;
		}

		public override void HandleSwipe(SwipeDir swipeDir)
		{
			base.HandleSwipe(swipeDir);
			switch (swipeDir)
			{
			case SwipeDir.Left:
				_character.ChangeTrackBy(-1, _settings.CharacterChangeTrackLength / _game.CurrentSpeed);
				break;
			case SwipeDir.Right:
				_character.ChangeTrackBy(1, _settings.CharacterChangeTrackLength / _game.CurrentSpeed);
				break;
			}
		}

		public override void Leave(State toState)
		{
			base.Leave(toState);
			_targetGamePositionZ = 0;
			_characterCamera.ResetShake();
			_character.CharacterController.detectCollisions = true;
			_character.verticalSpeed = _character.CalculateJumpVerticalSpeed(0f);
			_game.Modifiers.Resume();
			NotifyOnStop();
			GamePositionRoot.Instance.RemoveLock(this);
		}

		public void RegisterStartHandler(Action handler)
		{
			_startHandlers.Add(handler);
		}

		protected float GetFlyingDuration()
		{
			return Upgrades.GetUpgradeDuration(_powerup.type) - FlyingStateSettings.FlyAheadDuration;
		}

		public float GetTotalRemainingRatio()
		{
			return ((float)_targetGamePositionZ - _character.GamePosition.z) / ((float)_targetGamePositionZ - _startGamePositionZ);
		}

		protected abstract float GetFlyingSpeed();

		private void NotifyOnStart()
		{
			if (OnStart != null)
			{
				OnStart();
			}
			foreach (Action startHandler in _startHandlers)
			{
				startHandler();
			}
		}

		private void NotifyOnStop()
		{
			if (OnStop != null)
			{
				OnStop();
			}
		}
	}
}
