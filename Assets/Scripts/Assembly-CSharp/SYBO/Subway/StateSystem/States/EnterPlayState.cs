using SYBO.Common.Managers;
using SYBO.Common.StateSystem;
using SYBO.Components.Utilities;
using SYBO.Subway.Cameras;
using SYBO.Subway.Characters;
using UnityEngine;

namespace SYBO.Subway.StateSystem.States
{
	public class EnterPlayState : State
	{
		private const string EnterPlayAnimationName = "introRun";

		private const string EnterPlayCameraAnimationName = "startPan";

		private Camera _camera;

		private Animation _characterAnimation;

		private float _fovStart;

		public EnterPlayState(int id)
			: base(id)
		{
		}

		public override void Enter(State fromState)
		{
			base.Enter(fromState);
			SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale = SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScaleInGameDefault;
			GameStats.Instance.ResetGame();
			Game.Instance.Modifiers.Stop();
			Game.Instance.Modifiers.Update();
			Game.Instance.isDead = false;
			Game.Instance.ingameTouchDetection = true;
			Game.Instance.SetInitialSpeed();
			Game.Instance.ResetEnemyForEnterPlay();
			SingletonMonoBehaviour<AudioStateLoop>.Instance.ChangeLoop(AudioState.Ingame);
			Character.Instance.CharacterPickupParticleSystem.CoinEFX.transform.localPosition = CharacterPickupParticles.coinEfxOffset;
			Character.Instance.InitForRun();
			Animation component = CharacterCamera.Instance.transform.parent.GetComponent<Animation>();
			component.enabled = true;
			component.CrossFade("startPan", 0.2f);
			_characterAnimation = CharacterRendering.Instance.CharacterAnimation;
			_characterAnimation.CrossFade("introRun", 0.2f);
			_camera = CharacterCamera.Instance.Camera;
			_fovStart = _camera.fieldOfView;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			float time = _characterAnimation["introRun"].time;
			if (_characterAnimation["introRun"].enabled && time < _characterAnimation["introRun"].length)
			{
				_camera.fieldOfView = Mathf.Lerp(_fovStart, 68f, time * 0.75f);
			}
			else
			{
				GameStateMachine.Instance.ActivateTrigger(102);
			}
		}

		public override void Leave(State toState)
		{
			base.Leave(toState);
			ParticleSystem[] componentsInChildren = CharacterRendering.Instance.CharacterModel.SprayCanModel.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Stop();
			}
			_camera.fieldOfView = 68f;
			if (Game.Instance.IsRunningOnTutorialTrack)
			{
				FollowingGuard.Instance.ResetCatchUp();
				Game.Instance.isReadyForSlideinPowerups = false;
			}
			else if (toState.Id == 2)
			{
				Game.Instance.isReadyForSlideinPowerups = true;
			}
		}
	}
}
