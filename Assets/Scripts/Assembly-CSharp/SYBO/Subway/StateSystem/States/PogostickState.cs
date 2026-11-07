using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Info;
using Kiloo.Common.Utility;
using SYBO.Common.StateSystem;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using SYBO.Subway.Cameras;
using SYBO.Subway.Coins;
using SYBO.Subway.Inputs;
using SYBO.Subway.Level;
using SYBO.Subway.Pickups;
using SYBO.Subway.Routes;
using SYBO.Subway.StateSystem.States.Settings;
using UnityEngine;

namespace SYBO.Subway.StateSystem.States
{
	public class PogostickState : InGameState
	{
		private PogostickStateSettings _settings;

		private PickupType[] _pickupWeightedList;

		private GameObject _pickupSpawnPointPrefabInstance;

		public Action OnStart;

		public Action OnHangtime;

		public Action OnStop;

		private List<Action> _startHandlers = new List<Action>();

		private Vector3 _startGamePosition = Vector3.zero;

		private float _endGamePositionZ;

		private bool _reachedHangtime;

		private SmoothDampFloat _smoothCameraX;

		private Vector3 _cameraPositionStart = Vector3.zero;

		private Vector3 _cameraTargetStartOffset = Vector3.zero;

		public PogostickState(int id)
			: base(id)
		{
			_settings = ScriptableSingleton<PogostickStateSettings>.instance;
			InitializeSpawnList();
		}

		public override void Enter(State fromState)
		{
			base.Enter(fromState);
			_characterCamera.SetCameraState(CharacterCamera.CameraState.Custom);
			GamePositionRoot.Instance.AddLock(this);
			_game.ResetEnemy();
			_character.inAirJump = false;
			_character.IsGrounded.Value = false;
			GameStats.Instance.PickedUpPowerupsIncrement();
			GameStats.Instance.TriggerPowerup(UpgradeType.pogostick);
			NotifyOnStart();
			_game.Modifiers.PauseInJetpackMode();
			if (_character.IsStumbling)
			{
				_character.StopStumble();
			}
			_character.CharacterController.detectCollisions = false;
			_startGamePosition = _character.GamePosition;
			_endGamePositionZ = _startGamePosition.z + _settings.JumpDistance;
			SpawnCoinRows();
			SpawnPickup(_endGamePositionZ);
			_reachedHangtime = false;
			_smoothCameraX = new SmoothDampFloat(_characterCamera.transform.position.x, _settings.SmoothCameraXDuration);
			_cameraPositionStart = _characterCamera.transform.position;
			_cameraTargetStartOffset = _characterCamera.Position - _characterCamera.Target;
		}

		public override void OnUpdate()
		{
			float z = _character.GamePosition.z;
			if (z < _endGamePositionZ)
			{
				float num = z + _game.CurrentSpeed * Time.deltaTime;
				float num2 = (num - _startGamePosition.z) / _settings.JumpDistance;
				Vector3 gamePosition = Route.GetPosition(_character.x, num) + Vector3.up * _settings.JumpCurve.Evaluate(num2) * _settings.JumpHeight;
				if (num2 <= _settings.FadeInPosition)
				{
					gamePosition.y = Mathf.Lerp(_startGamePosition.y, gamePosition.y, num2 / _settings.FadeInPosition);
				}
				if (num2 > _settings.HangtimePosition && !_reachedHangtime)
				{
					NotifyHangtime();
					_reachedHangtime = true;
				}
				_character.SetGamePosition(gamePosition);
				base.OnUpdate();
				Vector3 transformPosition = _character.TransformPosition;
				_smoothCameraX.Update();
				_smoothCameraX.Target = transformPosition.x * 0.75f;
				float t = _settings.CameraCurve.Evaluate(num2);
				Vector3 vector = new Vector3(_smoothCameraX.Value, gamePosition.y, transformPosition.z) + _characterCamera.CameraData.CameraPositionOffset;
				Vector3 vector2 = new Vector3(vector.x, Mathf.Lerp(_cameraPositionStart.y, vector.y, t), vector.z);
				vector2.x = _smoothCameraX.Value;
				Vector3 target = Vector3.Lerp(vector2 - _cameraTargetStartOffset, transformPosition + _settings.CameraAimOffset, _settings.CameraAimFollowCurve.Evaluate(num2));
				target.x = _smoothCameraX.Value;
				_characterCamera.Position = vector2;
				_characterCamera.Target = target;
			}
			else
			{
				EndPogo();
			}
		}

		public override void Leave(State toState)
		{
			base.Leave(toState);
			if (toState is MenuState)
			{
				SetActiveForPickupPrefabInstance(false);
			}
			else
			{
				CoroutineUtil.StartCoroutine(RemovePickups(_character.GamePosition.z));
			}
		}

		public override void HandleSwipe(SwipeDir swipeDir)
		{
			switch (swipeDir)
			{
			case SwipeDir.None:
				break;
			case SwipeDir.Left:
				_character.ChangeTrackBy(-1, _settings.CharacterChangeTrackLength / _game.CurrentSpeed);
				break;
			case SwipeDir.Right:
				_character.ChangeTrackBy(1, _settings.CharacterChangeTrackLength / _game.CurrentSpeed);
				break;
			case SwipeDir.Down:
				EndPogo();
				_character.Roll();
				break;
			}
		}

		private void EndPogo()
		{
			_character.CharacterController.detectCollisions = true;
			if (!_character.IsGliderboard)
			{
				_character.verticalSpeed = _character.CalculateJumpVerticalSpeed(_settings.FinalJumpSpeed);
			}
			GameStateMachine.Instance.ActivateTrigger(102);
			_game.Modifiers.Resume();
			NotifyOnStop();
		}

		private void InitializeSpawnList()
		{
			int num = 0;
			PogostickStateSettings.PickupWeight[] pickupList = _settings.PickupList;
			foreach (PogostickStateSettings.PickupWeight pickupWeight in pickupList)
			{
				num += Upgrades.upgrades[pickupWeight.UpgradeType].spawnProbability;
			}
			_pickupWeightedList = new PickupType[num];
			int num2 = 0;
			for (int j = 0; j < _settings.PickupList.Length; j++)
			{
				for (int k = 0; k < Upgrades.upgrades[_settings.PickupList[j].UpgradeType].spawnProbability; k++)
				{
					_pickupWeightedList[num2] = _settings.PickupList[j].PickupType;
					num2++;
				}
			}
		}

		public void RegisterStartHandler(Action handler)
		{
			_startHandlers.Add(handler);
		}

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

		private void NotifyHangtime()
		{
			if (OnHangtime != null)
			{
				OnHangtime();
			}
		}

		private void NotifyOnStop()
		{
			if (OnStop != null)
			{
				OnStop();
			}
		}

		private void SpawnCoinRows()
		{
			float num = _settings.JumpDistance / (float)_settings.Rows;
			for (int i = 0; i < _settings.EndRowPosition; i++)
			{
				if (i > _settings.StartRowPosition)
				{
					Vector3 vector = new Vector3(0f, _settings.JumpCurve.Evaluate((float)i / (float)_settings.Rows) * _settings.JumpHeight, _character.TransformPosition.z + (float)i * num);
					SpawnCoinRow(vector.z, vector.y);
				}
			}
		}

		private void SpawnCoinRow(float z, float height)
		{
			CoinManager instance = SingletonMonoBehaviour<CoinManager>.Instance;
			for (int i = 0; i < 3; i++)
			{
				Vector3 position = new Vector3(Route.GetTrackX(i - 1), height, z);
				instance.SpawnCoin(position);
			}
		}

		private void SpawnPickup(float endGamePositionZ)
		{
			float x = Route.GetTrackX(UnityEngine.Random.Range(0, 3) - 1);
			int index = UnityEngine.Random.Range(0, _pickupWeightedList.Length);
			PickupType pickupType = _pickupWeightedList.ElementAt(index);
			Vector3 newWorldPosition = new Vector3(x, _settings.JumpCurve.Evaluate(1f) * _settings.JumpHeight, GamePositionRoot.Instance.GetWorldPositionZ(endGamePositionZ) + 20f) + new Vector3(0f, 10f, 0f);
			if (_pickupSpawnPointPrefabInstance != null && SingletonMonoBehaviour<PickupManager>.Instance.IsSpawned(_pickupSpawnPointPrefabInstance.transform))
			{
				SingletonMonoBehaviour<PickupManager>.Instance.Despawn(_pickupSpawnPointPrefabInstance.transform);
				_pickupSpawnPointPrefabInstance = null;
			}
			_pickupSpawnPointPrefabInstance = SingletonMonoBehaviour<PickupManager>.Instance.SpawnPickupSpawnPoint();
			HierarchyUtility.ReparentByHierarchyPathType(_pickupSpawnPointPrefabInstance.transform, HierarchyPathType.Dynamic, newWorldPosition);
			_pickupSpawnPointPrefabInstance.GetComponent<PickupSpawnPoint>().SetSpawnPointForced(pickupType, SpawnPointDifficulty.Easy, false, true, false);
			SingletonMonoBehaviour<PickupManager>.Instance.UpdatePickerCollectionForced();
			SetActiveForPickupPrefabInstance(true);
		}

		private void SetActiveForPickupPrefabInstance(bool isActive)
		{
			if (_pickupSpawnPointPrefabInstance != null)
			{
				_pickupSpawnPointPrefabInstance.SetActive(isActive);
			}
		}

		private IEnumerator RemovePickups(float endPosition)
		{
			while (_character.GamePosition.z < endPosition + 100f)
			{
				yield return null;
			}
			GamePositionRoot.Instance.RemoveLock(this);
			if (_pickupSpawnPointPrefabInstance != null && SingletonMonoBehaviour<PickupManager>.Instance.IsSpawned(_pickupSpawnPointPrefabInstance.transform))
			{
				SingletonMonoBehaviour<PickupManager>.Instance.Despawn(_pickupSpawnPointPrefabInstance.transform);
				_pickupSpawnPointPrefabInstance = null;
			}
		}
	}
}
