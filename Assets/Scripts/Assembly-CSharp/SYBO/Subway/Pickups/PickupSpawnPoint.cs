using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using SYBO.Subway.Characters;
using SYBO.Subway.Level;
using SYBO.Subway.Pickups.Pickers;
using SYBO.Subway.Placeholders.Lifecycles;
using SYBO.Subway.Routes;
using SYBO.Subway.StateSystem;
using SYBO.Subway.StateSystem.States;
using UnityEngine;

namespace SYBO.Subway.Pickups
{
	[RequireComponent(typeof(TrackObject))]
	[RequireComponent(typeof(SnapObject))]
	[RequireComponent(typeof(Collider))]
	public class PickupSpawnPoint : MonoBehaviour
	{
		public const string PickupSpawnPointPrefabProjectPath = "Assets/Resources/Prefabs/Pickups/Refactored/PickupSpawnPoint.prefab";

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<PickupSpawnPoint> OnRegistered__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<PickupSpawnPoint> OnDeregistered__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<PickupSpawnPoint> OnPickupDespawned__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<PickupSpawnPoint> OnPickupCollected__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<PickupSpawnPoint, PickerData> OnPickupSpawned__BackingField;

		[SerializeField]
		internal GameObject _pickupParent;

		[SerializeField]
		internal SpawnPointMode __spawnPointMode;

		[SerializeField]
		internal PickupType __forceSpawnPickupType = PickupType.PogoStick;

		[SerializeField]
		internal SpawnPointDifficulty _spawnPointDifficulty = SpawnPointDifficulty.Easy;

		[SerializeField]
		internal SpawnPointHeight _spawnPointHeight = SpawnPointHeight.OnGround;

		private PickupRotate _pickupRotate;

		private bool _ignoreJetpackZ;

		private PickupData _pickupData;

		internal bool __willIgnoreZPositionBeforeSpawn;

		private SpawnPointMode _spawnPointModeOriginal;

		private bool _willIgnoreZPositionBeforeSpawnOriginal;

		private PickupType _forceSpawnPickupTypeOriginal;

		private SpawnPointState __spawnPointState;

		private SpawnPointDifficulty _spawnPointDifficultyOriginal;

		private SpawnPointHeight _spawnPointHeightOriginal;

		private GameObject _spawnedPickupVisuals;

		private Collider _trigger;

		private TrackObject _trackObject;

		private List<TrackObject> _visualsTrackObjects;

		public SpawnPointMode SpawnPointMode
		{
			get
			{
				return __spawnPointMode;
			}
			internal set
			{
				__spawnPointMode = value;
			}
		}

		public bool WillIgnoreZPositionBeforeSpawn
		{
			get
			{
				return __willIgnoreZPositionBeforeSpawn;
			}
			internal set
			{
				__willIgnoreZPositionBeforeSpawn = value;
			}
		}

		public PickupType ForceSpawnPickupType
		{
			get
			{
				return __forceSpawnPickupType;
			}
			internal set
			{
				__forceSpawnPickupType = value;
			}
		}

		public SpawnPointState SpawnPointState
		{
			get
			{
				return __spawnPointState;
			}
			private set
			{
				__spawnPointState = value;
			}
		}

		public SpawnPointDifficulty SpawnPointDifficulty
		{
			get
			{
				return _spawnPointDifficulty;
			}
		}

		public SpawnPointHeight SpawnPointHeight
		{
			get
			{
				return _spawnPointHeight;
			}
		}

		public bool CanTriggerCollisionWithCharacter
		{
			get
			{
				return SpawnPointState == SpawnPointState.Spawned;
			}
		}

		public Route ContainingRoute { get; private set; }

		public PickupData PickupData
		{
			get
			{
				return _pickupData;
			}
		}

		private bool IsAvailableAsTrackObject
		{
			get
			{
				return _trackObject.Active;
			}
		}

		public GameObject SpawnedPickupIcon
		{
			get
			{
				return _spawnedPickupVisuals;
			}
		}

		private bool IsPositionAcceptableBeforeSpawn
		{
			get
			{
				return base.gameObject != null && Game.ConvertUnityUnitsToGameMeters(GamePositionRoot.Instance.GetGamePosition(base.transform).z - Character.Instance.GamePosition.z) > 0f;
			}
		}

		protected bool IsZPositionToGameStartAcceptableBeforeSpawn
		{
			get
			{
				return 0f < SingletonMonoBehaviour<PickupManager>.Instance.SpawnDistanceMinToGameStart && Game.ConvertUnityUnitsToGameMeters(base.transform.position.z) >= SingletonMonoBehaviour<PickupManager>.Instance.SpawnDistanceMinToGameStart;
			}
		}

		protected bool IsZAvailableToJetpackState
		{
			get
			{
				FlyingState flyingState = GameStateMachine.Instance.TryGetCurrentState<FlyingState>();
				return _ignoreJetpackZ || flyingState == null || GamePositionRoot.Instance.GetGamePosition(base.transform).z > (float)flyingState.TargetGamePositionZ;
			}
		}

		public event Action<PickupSpawnPoint> OnRegistered
		{
			add
			{
				Action<PickupSpawnPoint> action = OnRegistered__BackingField;
				Action<PickupSpawnPoint> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnRegistered__BackingField, (Action<PickupSpawnPoint>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<PickupSpawnPoint> action = OnRegistered__BackingField;
				Action<PickupSpawnPoint> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnRegistered__BackingField, (Action<PickupSpawnPoint>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<PickupSpawnPoint> OnDeregistered
		{
			add
			{
				Action<PickupSpawnPoint> action = OnDeregistered__BackingField;
				Action<PickupSpawnPoint> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnDeregistered__BackingField, (Action<PickupSpawnPoint>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<PickupSpawnPoint> action = OnDeregistered__BackingField;
				Action<PickupSpawnPoint> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnDeregistered__BackingField, (Action<PickupSpawnPoint>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<PickupSpawnPoint> OnPickupDespawned
		{
			add
			{
				Action<PickupSpawnPoint> action = OnPickupDespawned__BackingField;
				Action<PickupSpawnPoint> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPickupDespawned__BackingField, (Action<PickupSpawnPoint>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<PickupSpawnPoint> action = OnPickupDespawned__BackingField;
				Action<PickupSpawnPoint> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPickupDespawned__BackingField, (Action<PickupSpawnPoint>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<PickupSpawnPoint> OnPickupCollected
		{
			add
			{
				Action<PickupSpawnPoint> action = OnPickupCollected__BackingField;
				Action<PickupSpawnPoint> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPickupCollected__BackingField, (Action<PickupSpawnPoint>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<PickupSpawnPoint> action = OnPickupCollected__BackingField;
				Action<PickupSpawnPoint> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPickupCollected__BackingField, (Action<PickupSpawnPoint>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<PickupSpawnPoint, PickerData> OnPickupSpawned
		{
			add
			{
				Action<PickupSpawnPoint, PickerData> action = OnPickupSpawned__BackingField;
				Action<PickupSpawnPoint, PickerData> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPickupSpawned__BackingField, (Action<PickupSpawnPoint, PickerData>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<PickupSpawnPoint, PickerData> action = OnPickupSpawned__BackingField;
				Action<PickupSpawnPoint, PickerData> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPickupSpawned__BackingField, (Action<PickupSpawnPoint, PickerData>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		protected void Awake()
		{
			_trackObject = GetComponent<TrackObject>();
			_trackObject.OnActivate += TrackObject_OnActivate;
			_trackObject.OnDeactivate += TrackObject_OnDeactivate;
			_trigger = GetComponent<Collider>();
			_trigger.enabled = false;
			_spawnPointModeOriginal = SpawnPointMode;
			_forceSpawnPickupTypeOriginal = ForceSpawnPickupType;
			_spawnPointDifficultyOriginal = _spawnPointDifficulty;
			_spawnPointHeightOriginal = _spawnPointHeight;
			InitializeAllValues();
		}

		private void InitializeAllValues()
		{
			DeactivateAndDespawnChild();
			SpawnPointState = SpawnPointState.UnspawnedAndAvailable;
			SpawnPointMode = _spawnPointModeOriginal;
			ForceSpawnPickupType = _forceSpawnPickupTypeOriginal;
			if (SpawnPointMode == SpawnPointMode.WillForcePickupType)
			{
				_willIgnoreZPositionBeforeSpawnOriginal = true;
			}
			WillIgnoreZPositionBeforeSpawn = _willIgnoreZPositionBeforeSpawnOriginal;
			_spawnPointDifficulty = _spawnPointDifficultyOriginal;
			_spawnPointHeight = _spawnPointHeightOriginal;
			_pickupData = null;
		}

		private void SetForceSpawnPickupTypeAndOriginalTo(PickupType pickupType, bool ignoreJetpackZ)
		{
			ForceSpawnPickupType = (_forceSpawnPickupTypeOriginal = pickupType);
			_ignoreJetpackZ = ignoreJetpackZ;
		}

		private void SetSpawnPointModeAndOriginalTo(SpawnPointMode spawnPointMode)
		{
			SpawnPointMode = (_spawnPointModeOriginal = spawnPointMode);
		}

		private void SetWillIgnoreZPositionBeforeSpawnAndOriginalTo(bool willIgnoreZPositionBeforeSpawn)
		{
			WillIgnoreZPositionBeforeSpawn = (_willIgnoreZPositionBeforeSpawnOriginal = willIgnoreZPositionBeforeSpawn);
		}

		public void SetContainingRoute()
		{
			ContainingRoute = GetComponentInParent<Route>();
		}

		public bool IsAvailableForSpawning(bool willShowDebugLog = false)
		{
			if (WillIgnoreZPositionBeforeSpawn)
			{
				return IsStateAvailableForSpawning() && IsAvailableAsTrackObject && IsZAvailableToJetpackState;
			}
			return IsStateAvailableForSpawning(willShowDebugLog) && IsAvailableAsTrackObject && IsPositionAcceptableBeforeSpawn && IsDistanceToCharacterAcceptableBeforeSpawn(willShowDebugLog) && IsZPositionToGameStartAcceptableBeforeSpawn && IsZAvailableToJetpackState;
		}

		private bool IsStateAvailableForSpawning(bool willShowDebugLog = false)
		{
			bool flag = SpawnPointState == SpawnPointState.UnspawnedAndAvailable;
			if (!willShowDebugLog || !flag)
			{
			}
			return flag;
		}

		private bool IsDistanceToCharacterAcceptableBeforeSpawn(bool willShowDebugLog = false)
		{
			if (PickupManager.SpawnDistanceMinToCharacter <= 0f || GamePositionRoot.Instance.GetGamePosition(base.transform).z < Character.Instance.GamePosition.z)
			{
				return false;
			}
			if (willShowDebugLog)
			{
			}
			float num = Game.ConvertUnityUnitsToGameMeters(GamePositionRoot.Instance.GetGamePosition(base.transform).z - Character.Instance.GamePosition.z);
			return PickupManager.SpawnDistanceMinToCharacter <= num;
		}

		private void Update()
		{
			if (_pickupRotate != null && _pickupRotate.WillRotate)
			{
				_pickupRotate.PhasedRotate(Time.time);
			}
		}

		public void SetSpawnPointForced(PickupType pickupType, SpawnPointDifficulty spawnPointDifficulty, bool willOffsetYPosition, bool willIgnoreZPositionBeforeSpawn, bool willIgnoreJetpackZ)
		{
			_ignoreJetpackZ = false;
			InitializeAllValues();
			SetWillIgnoreZPositionBeforeSpawnAndOriginalTo(willIgnoreZPositionBeforeSpawn);
			SetSpawnPointModeAndOriginalTo(SpawnPointMode.WillForcePickupType);
			SetForceSpawnPickupTypeAndOriginalTo(pickupType, willIgnoreJetpackZ);
			_trackObject.Activate();
		}

		public void Register()
		{
			if (OnRegistered__BackingField != null)
			{
				OnRegistered__BackingField(this);
			}
		}

		public void Deregister()
		{
			DeactivateAndDespawnChild();
			if (OnDeregistered__BackingField != null)
			{
				OnDeregistered__BackingField(this);
			}
		}

		public void SpawnPickup(PickupData pickupData, PickerData pickerData)
		{
			if (IsAvailableForSpawning())
			{
				_trigger.enabled = true;
				SpawnPointState = SpawnPointState.Spawned;
				_pickupData = pickupData;
				_spawnedPickupVisuals = SingletonMonoBehaviour<PickupManager>.Instance.Spawn(pickupData.PickupVisualsPrefab.transform, Vector3.zero, Quaternion.identity);
				HierarchyUtility.Reparent(_spawnedPickupVisuals.transform, _pickupParent.transform, Vector3.zero);
				SpawnExistingPlaceholders();
				_trackObject.Hack_PropagateActivationStateToChildrenImmediately();
				_pickupRotate = _spawnedPickupVisuals.GetComponentInChildren<PickupRotate>(true);
				if (OnPickupSpawned__BackingField != null)
				{
					OnPickupSpawned__BackingField(this, pickerData);
				}
			}
		}

		private void SpawnExistingPlaceholders()
		{
			PlaceholderLifecycle component = _spawnedPickupVisuals.GetComponent<PlaceholderLifecycle>();
			if (component != null && !component.Placeholder.HasSpawnedInstances)
			{
				component.Spawn();
			}
		}

		private void DeactivateAndDespawnChild()
		{
			DeactivateChild();
			DespawnChild();
		}

		private void DeactivateChild()
		{
			if (!(_spawnedPickupVisuals != null))
			{
				return;
			}
			if (_visualsTrackObjects == null)
			{
				_visualsTrackObjects = new List<TrackObject>(1);
			}
			_spawnedPickupVisuals.GetComponentsInChildren(true, _visualsTrackObjects);
			foreach (TrackObject visualsTrackObject in _visualsTrackObjects)
			{
				visualsTrackObject.Deactivate();
			}
			_visualsTrackObjects.Clear();
		}

		private void DespawnChild()
		{
			if (_spawnedPickupVisuals != null)
			{
				SingletonMonoBehaviour<PickupManager>.Instance.Despawn(_spawnedPickupVisuals.transform);
				_spawnedPickupVisuals = null;
			}
			SpawnPointState = SpawnPointState.Despawned;
			if (OnPickupDespawned__BackingField != null)
			{
				OnPickupDespawned__BackingField(this);
			}
		}

		public void MarkAsUnavailable()
		{
			if (SpawnPointState == SpawnPointState.UnspawnedAndAvailable)
			{
				SpawnPointState = SpawnPointState.UnspawnedAndUnavailable;
			}
			else
			{
				SDebug.LogError("New value not acceptable. Don't call method in this situation.");
			}
		}

		private void DebugLog(string message)
		{
			if (SingletonMonoBehaviour<PickupManager>.IsInstantiated() && !SingletonMonoBehaviour<PickupManager>.Instance.WillDebugLogPickers)
			{
			}
		}

		private void TrackObject_OnActivate()
		{
			InitializeAllValues();
			if (SingletonMonoBehaviour<PickupManager>.IsInstantiated())
			{
				SingletonMonoBehaviour<PickupManager>.Instance.RegisterPickupSpawnPoint(this);
			}
		}

		private void TrackObject_OnDeactivate()
		{
			if (SingletonMonoBehaviour<PickupManager>.IsInstantiated())
			{
				SingletonMonoBehaviour<PickupManager>.Instance.DeregisterPickupSpawnPoint(this);
			}
		}

		public void TriggerCollisionWithCharacter(CharacterPickupParticles characterPickupParticles)
		{
			if (SpawnPointState != SpawnPointState.SpawnedAndCollected)
			{
				SpawnPointState = SpawnPointState.SpawnedAndCollected;
				_trigger.enabled = false;
				characterPickupParticles.PickedUpPowerUp();
				if (OnPickupCollected__BackingField != null)
				{
					OnPickupCollected__BackingField(this);
				}
				DeactivateAndDespawnChild();
			}
		}
	}
}
