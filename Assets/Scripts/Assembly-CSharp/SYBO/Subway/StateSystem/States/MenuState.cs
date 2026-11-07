using SYBO.Common.Managers;
using SYBO.Common.StateSystem;
using SYBO.Components.Utilities;
using SYBO.Subway.Cameras;
using SYBO.Subway.Characters;
using SYBO.Subway.Coins;
using SYBO.Subway.Routes;
using UnityEngine;

namespace SYBO.Subway.StateSystem.States
{
	public class MenuState : State
	{
		private Game _game;

		private Character _character;

		private FollowingGuard _followingGuard;

		public MenuState(int id)
			: base(id)
		{
			_game = Game.Instance;
			_character = Character.Instance;
			_followingGuard = FollowingGuard.Instance;
		}

		public override void Enter(State fromState)
		{
			base.Enter(fromState);
			if (_game.IsInGame.Value)
			{
				Game.InvokeOnGameEnded();
			}
			SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale = SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScaleInMenuDefault;
			_game.ResetGameSpeed();
			_game.IsInGame.Value = false;
			_game.IsInTopMenu.Value = true;
			_game.Modifiers.StopWithNoEnding();
			_game.Modifiers.Update();
			_game.Modifiers.Reset();
			_game.IsLockedGameSpeed = false;
			SingletonMonoBehaviour<AudioStateLoop>.Instance.ChangeLoop(AudioState.Menu);
			SingletonMonoBehaviour<CoinManager>.Instance.ResetCoins();
			SingletonMonoBehaviour<RouteManager>.Instance.Reset();
			_followingGuard.MuteProximityLoop();
			_followingGuard.ShowEnemies(false);
			_followingGuard.enabled = false;
			_followingGuard.ShowEnemies(false);
			_followingGuard.StopAllCoroutines();
			_character.StopAllCoroutines();
			_character.Reset();
			_character.SetGamePosition(Vector3.zero);
			SetMenuCamera();
			_game.InvokeOnStageMenuSequence();
			if (_character.SuperSneakers.isActive)
			{
				_character.SuperSneakers.StopHack();
				_character.IsJumpingHigher = false;
			}
			GameStats.Instance.ClearPowerups();
			GameStats.Instance.IsPausedPowerups = false;
			Animation component = CharacterCamera.Instance.transform.parent.GetComponent<Animation>();
			component.Play("menuIdle");
			component.Sample();
			component.enabled = false;
			Game.InvokeOnMenuScreenShown();
		}

		private void SetMenuCamera()
		{
			CharacterCamera instance = CharacterCamera.Instance;
			instance.SetCameraState(CharacterCamera.CameraState.Menu);
			Vector3 cameraLocalPosition = _character.TransformPosition + instance.CameraData.CameraPositionOffset + Vector3.up * 0.8f;
			Quaternion cameraLocalRotation = Quaternion.Euler(21.50143f, 0f, 0f);
			CameraData defaultCameraDataForMenu = instance.GetDefaultCameraDataForMenu();
			instance.SetCameraDataForcefully(defaultCameraDataForMenu, cameraLocalPosition, cameraLocalRotation);
		}
	}
}
