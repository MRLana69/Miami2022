using Data.Save;
using Info;
using SYBO.Common.StateSystem;
using SYBO.Subway.Cameras;
using SYBO.Subway.Inputs;

namespace SYBO.Subway.StateSystem.States
{
	public class RunningState : InGameState
	{
		public RunningState(int id)
			: base(id)
		{
		}

		public override void Enter(State fromState)
		{
			base.Enter(fromState);
			_characterCamera.SetCameraState(CharacterCamera.CameraState.DefaultRunning);
			if (fromState is PogostickState || fromState is FlyingState)
			{
				_character.TransitioningFromHeight = true;
			}
			else
			{
				_character.TransitioningFromHeight = false;
			}
			_character.CharacterTrigger.enabled = true;
			_character.LastGroundedY = _character.GamePosition.y;
		}

		public override void OnUpdate()
		{
			if (!_game.IsLockedGameSpeed)
			{
				_game.SetCurrentSpeed(_game.CurrentLevelSpeed);
			}
			_character.ApplyGravity();
			_character.MoveForward();
			if (_character.TransitioningFromHeight && (_character.IsGrounded.Value || (_character.inAirJump && _character.CharacterController.isGrounded)))
			{
				_character.TransitioningFromHeight = false;
				_character.ForceLeaveSubway();
			}
			_character.CheckInAirJump();
			_character.UpdateInAirRunPosition();
			base.OnUpdate();
		}

		public override void HandleCriticalHit()
		{
			_game.Die();
		}

		public override void HandleSwipe(SwipeDir swipeDir)
		{
			base.HandleSwipe(swipeDir);
			float duration = ((!_character.IsTeleboard) ? 30f : 20f) / _game.CurrentSpeed;
			switch (swipeDir)
			{
			case SwipeDir.None:
				break;
			case SwipeDir.Left:
				_character.ChangeTrackBy(-1, duration);
				break;
			case SwipeDir.Right:
				_character.ChangeTrackBy(1, duration);
				break;
			case SwipeDir.Up:
				_character.Jump();
				break;
			case SwipeDir.Down:
				_character.Roll();
				break;
			}
		}

		public override void HandleDoubleTap()
		{
			base.HandleDoubleTap();
			if (!Globals.DEBUG_DISABLE_HOVERBOARD)
			{
				int amount = InventorySave.GetAmount(UpgradeType.hoverboard);
				bool flag = _game.Modifiers.IsActive(_game.Modifiers.Hoverboard);
				if (amount > 0 && !flag)
				{
					_game.Modifiers.Add(_game.Modifiers.Hoverboard);
				}
				else if (amount == 0 && !_game.Modifiers.IsActive(_game.Modifiers.Hoverboard) && !_game.Modifiers.Hoverboard.IsInCooldown)
				{
					SellConsumable.Sell(SellableConsumable.hoverboard);
				}
				if (!flag && !_game.Modifiers.Hoverboard.IsInCooldown && Hoverboard.Instance.IsAllowed)
				{
					LogEventForHoverboardStartAttempt(amount > 0);
				}
			}
		}

		private void LogEventForHoverboardStartAttempt(bool successful)
		{
			if (successful)
			{
				if (!OnboardingSave.isTutorialCompleted)
				{
					Flurry.LogEvent("Tutorial hoverboard activated");
				}
				Flurry.LogEventWithAParameter("Boost hoverboard activated", "successfully", "yes");
			}
			else
			{
				Flurry.LogEventWithAParameter("Boost hoverboard activated", "successfully", "no");
			}
		}
	}
}
