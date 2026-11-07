using System;
using Info;
using Kiloo.Common.Utility;
using SYBO.Common.StateSystem;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using SYBO.Subway.Level;
using SYBO.Subway.Pickups;
using SYBO.Subway.Routes;
using SYBO.Subway.Routes.Builders;
using SYBO.Subway.StateSystem.States.Settings;
using UnityEngine;

namespace SYBO.Subway.StateSystem.States
{
	public class HeadstartState : FlyingState
	{
		private const float ChangeHeightSpeed = 50f;

		private HeadstartStateSettings _settings;

		public Action OnActivateTurboHeadstart;

		public Action OnDeactivateTurboHeadstart;

		public Action OnHidTurboHeadstartButtons;

		private GameObject[] _pickupSpawnPointPrefabInstances;

		private bool _activateTurboHeadstart;

		private bool _deactivateTurboHeadstart;

		private int _turboHeadstart;

		protected override UpgradeType UpgradeType
		{
			get
			{
				return UpgradeType.headstart2000;
			}
		}

		public int TurboHeadstart
		{
			get
			{
				return _turboHeadstart;
			}
		}

		public HeadstartState(int id)
			: base(id)
		{
			_settings = ScriptableSingleton<HeadstartStateSettings>.instance;
			InitializePickups();
		}

		public override void Enter(State fromState)
		{
			base.Enter(fromState);
			PlacePickups((float)_targetGamePositionZ - _speedPositionAdjustment);
			SetFOV();
			if (OnActivateTurboHeadstart != null)
			{
				OnActivateTurboHeadstart();
			}
		}

		public override void Leave(State toState)
		{
			base.Leave(toState);
			ResetPickups();
			_activateTurboHeadstart = false;
			if (_turboHeadstart >= 0 && _turboHeadstart <= 2)
			{
				_game.ChangeCurrentSpeed(_game.speed.rampUpDuration * Upgrades.HEADSTART_TIERS[_turboHeadstart].landSpeed);
			}
			ResetTurboHeadstart();
			RevertFOV();
		}

		protected override void UpdateFlying(float remainingRatio)
		{
			base.UpdateFlying(remainingRatio);
			if (remainingRatio > 0.8f && !_deactivateTurboHeadstart && OnHidTurboHeadstartButtons != null)
			{
				OnHidTurboHeadstartButtons();
			}
			if (remainingRatio > 0.9f && !_deactivateTurboHeadstart)
			{
				_deactivateTurboHeadstart = true;
				if (OnDeactivateTurboHeadstart != null)
				{
					OnDeactivateTurboHeadstart();
				}
			}
			if (_activateTurboHeadstart)
			{
				ExtraHeadstart();
				_activateTurboHeadstart = false;
			}
		}

		public void ActivateLongHeadstart()
		{
			if (!_deactivateTurboHeadstart)
			{
				_activateTurboHeadstart = true;
				_turboHeadstart++;
			}
		}

		private void ExtraHeadstart()
		{
			if (_turboHeadstart == 1 || _turboHeadstart == 2)
			{
				UpdateHeadstart(Upgrades.HEADSTART_TIERS[_turboHeadstart]);
			}
		}

		private void UpdateHeadstart(HeadstartTier tier)
		{
			float num = tier.duration - base.FlyingStateSettings.FlyAheadDuration;
			float speed = tier.speed;
			float num2 = num * speed;
			_game.SetCurrentSpeed(speed);
			_targetGamePositionZ = Mathf.CeilToInt((num2 + _character.GamePosition.z) / 180f) * 180;
			(SingletonMonoBehaviour<RouteManager>.Instance.CharacterRoute.Builder as DefaultRouteBuilder).EnqueueJetpackEndSection(_targetGamePositionZ);
			PlacePickups(_targetGamePositionZ);
		}

		protected override float GetFlyHeight()
		{
			if (_turboHeadstart == 0)
			{
				return base.GetFlyHeight();
			}
			float flyHeightFromHeadstart = GetFlyHeightFromHeadstart();
			return Mathf.Min(_character.GamePosition.y + Time.deltaTime * 50f, flyHeightFromHeadstart);
		}

		protected override float GetFlyingSpeed()
		{
			return Upgrades.upgrades[UpgradeType].speed;
		}

		public void ResetTurboHeadstart()
		{
			_deactivateTurboHeadstart = false;
			_activateTurboHeadstart = false;
			_turboHeadstart = 0;
		}

		private void InitializePickups()
		{
			_pickupSpawnPointPrefabInstances = new GameObject[3];
			for (int i = 0; i < 3; i++)
			{
				_pickupSpawnPointPrefabInstances[i] = SingletonMonoBehaviour<PickupManager>.Instance.SpawnPickupSpawnPoint();
				HierarchyUtility.ReparentByHierarchyPathType(_pickupSpawnPointPrefabInstances[i].transform, HierarchyPathType.Dynamic, new Vector3(0f, 0f, 0f));
			}
		}

		private void PlacePickups(float gamePositionZ)
		{
			ResetPickups();
			float num = GetFlyHeightFromHeadstart() - 5f;
			float worldPositionZ = GamePositionRoot.Instance.GetWorldPositionZ(gamePositionZ);
			int[] array = new int[3] { -1, -1, -1 };
			for (int i = 0; i < 3; i++)
			{
				int num2 = -1;
				while (num2 == array[0] || num2 == array[1] || num2 == array[2])
				{
					num2 = UnityEngine.Random.Range(0, _settings.PickupTypes.Length);
					if (num2 != array[0] && num2 != array[1] && num2 != array[2])
					{
						array[i] = num2;
						break;
					}
				}
			}
			for (int j = 0; j < 3; j++)
			{
				PickupType pickupType = _settings.PickupTypes[array[j]];
				PickupSpawnPoint component = _pickupSpawnPointPrefabInstances[j].GetComponent<PickupSpawnPoint>();
				component.SetSpawnPointForced(pickupType, SpawnPointDifficulty.Easy, false, true, true);
				SingletonMonoBehaviour<PickupManager>.Instance.UpdatePickerCollectionForced();
				_pickupSpawnPointPrefabInstances[j].transform.position = new Vector3(-20 + 20 * j, num + 10f, worldPositionZ);
				SetActivePickupSpawnPointPrefabInstance(j, true);
			}
		}

		private float GetFlyHeightFromHeadstart()
		{
			if (_turboHeadstart == 1)
			{
				return _settings.TurboHeadstart1FlyHeight;
			}
			if (_turboHeadstart == 2)
			{
				return _settings.TurboHeadstart2FlyHeight;
			}
			return base.FlyingStateSettings.FlyHeight;
		}

		private void ResetPickups()
		{
			for (int i = 0; i < _pickupSpawnPointPrefabInstances.Length; i++)
			{
				SetActivePickupSpawnPointPrefabInstance(i, false);
			}
		}

		private void SetActivePickupSpawnPointPrefabInstance(int i, bool isActive)
		{
			_pickupSpawnPointPrefabInstances[i].SetActive(isActive);
		}

		private void SetFOV()
		{
			float fov_start = _characterCamera.Camera.fieldOfView;
			float fov_end = 68f + _settings.MegaHeadstartFOVIncrease;
			CoroutineUtil.StartCoroutine(pTween.To(_settings.FovTransitionTime, (float t) =>
			{
				_characterCamera.Camera.fieldOfView = Mathf.Lerp(fov_start, fov_end, t);
			}));
		}

		private void RevertFOV()
		{
			float fov_start = _characterCamera.Camera.fieldOfView;
			float fov_end = 0f;
			if (Hoverboard.Instance.isActive && Hoverboard.Instance.IsHighSpeedBoard)
			{
				fov_end = 68f + _game.HighSpeedFOVIncrease;
			}
			else if (Hoverboard.Instance.isActive && Hoverboard.Instance.IsSpeedBoard)
			{
				fov_end = 68f + _game.SpeedFOVIncrease;
			}
			else
			{
				fov_end = 68f;
			}
			CoroutineUtil.StartCoroutine(pTween.To(_settings.FovTransitionTime, (float t) =>
			{
				if (GameStateMachine.Instance.IsInGame())
				{
					_characterCamera.Camera.fieldOfView = Mathf.Lerp(fov_start, fov_end, t);
				}
				else
				{
					_characterCamera.Camera.fieldOfView = 68f;
				}
			}));
		}
	}
}
