using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SYBO.Common.Exceptions;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using SYBO.Subway.Level;
using SYBO.Subway.Pickups.Picker.Pickers;
using SYBO.Subway.Pickups.Pickers;
using UnityEngine;

namespace SYBO.Subway.Pickups
{
	[PrefabPath("Prefabs/Managers/PickupManager")]
	public class PickupManager : SingletonMonoBehaviour<PickupManager>
	{
		private enum ReasonToResetHistory
		{
			SESSION_STARTED = 0,
			GAME_STARTED = 1,
			RUN_STARTED = 2
		}

		public class PickupEventArgs : EventArgs
		{
			public readonly PickupSpawnPoint SpawnPoint;

			public readonly PickupData PickupData;

			public readonly SpawnPointMode SpawnPointMode;

			public PickupEventArgs(PickupSpawnPoint spawnPoint, PickupData pickupData, SpawnPointMode spawnPointMode)
			{
				SpawnPoint = spawnPoint;
				PickupData = pickupData;
				SpawnPointMode = spawnPointMode;
			}
		}

		private const int PickerTypeShortNameLength = 8;

		private const int PickupTypeShortNameLength = 9;

		private const int DelayBeforeUpdatingPickerCollectionCountMax = 60;

		public static readonly float SpawnDistanceMinToCharacter = 112.5f;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<PickupEventArgs> OnPickupSpawned__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<PickupEventArgs> OnPickupDespawned__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<PickupEventArgs> OnPickupCollected__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<PickupSpawnPoint> OnPickupSpawnPointRegistered__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<PickupSpawnPoint> OnPickupSpawnPointDeregistered__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<PickerRemovedEventArgs> OnPickerRemoved__BackingField;

		[Tooltip("Enable/disable logging from pickers")]
		[SerializeField]
		private bool _willDebugLogPickers;

		[Tooltip("Enable/disable logging from PickupData")]
		[SerializeField]
		private bool _willDebugLogPickupData;

		[SerializeField]
		internal GameObject _pickupSpawnPointPrefab;

		[Range(0f, 10000f)]
		[SerializeField]
		[Tooltip("Minimum distance between pickup and the start of the game.")]
		private float _spawnDistanceMinToGameStart = 360f;

		[Tooltip("The list of all possible pickup items.")]
		[SerializeField]
		internal PickupData[] _pickupData;

		[Tooltip("The list of pickers which choose the items to spawn.")]
		[SerializeField]
		public PickerData[] _pickerData;

		private List<PickupSpawnPoint> _pickupSpawnPoints;

		internal PickerCollection _pickerCollection;

		private int _delayBeforeUpdatingPickerCollectionCount;

		public readonly Dictionary<int, float> PickupLastSpawnZGamePosition = new Dictionary<int, float>();

		public readonly Dictionary<int, float> PickerLastSpawnZGamePosition = new Dictionary<int, float>();

		public float SpawnDistanceMinToGameStart
		{
			get
			{
				return _spawnDistanceMinToGameStart;
			}
		}

		internal List<PickupSpawnPoint> PickupSpawnPointList
		{
			get
			{
				return _pickupSpawnPoints;
			}
		}

		public bool WillDebugLogPickers
		{
			get
			{
				return _willDebugLogPickers;
			}
		}

		public bool WillDebugLogPickupData
		{
			get
			{
				return _willDebugLogPickupData;
			}
		}

		public event EventHandler<PickupEventArgs> OnPickupSpawned
		{
			add
			{
				EventHandler<PickupEventArgs> eventHandler = OnPickupSpawned__BackingField;
				EventHandler<PickupEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnPickupSpawned__BackingField, (EventHandler<PickupEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<PickupEventArgs> eventHandler = OnPickupSpawned__BackingField;
				EventHandler<PickupEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnPickupSpawned__BackingField, (EventHandler<PickupEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public event EventHandler<PickupEventArgs> OnPickupDespawned
		{
			add
			{
				EventHandler<PickupEventArgs> eventHandler = OnPickupDespawned__BackingField;
				EventHandler<PickupEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnPickupDespawned__BackingField, (EventHandler<PickupEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<PickupEventArgs> eventHandler = OnPickupDespawned__BackingField;
				EventHandler<PickupEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnPickupDespawned__BackingField, (EventHandler<PickupEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public event EventHandler<PickupEventArgs> OnPickupCollected
		{
			add
			{
				EventHandler<PickupEventArgs> eventHandler = OnPickupCollected__BackingField;
				EventHandler<PickupEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnPickupCollected__BackingField, (EventHandler<PickupEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<PickupEventArgs> eventHandler = OnPickupCollected__BackingField;
				EventHandler<PickupEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnPickupCollected__BackingField, (EventHandler<PickupEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public event Action<PickupSpawnPoint> OnPickupSpawnPointRegistered
		{
			add
			{
				Action<PickupSpawnPoint> action = OnPickupSpawnPointRegistered__BackingField;
				Action<PickupSpawnPoint> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPickupSpawnPointRegistered__BackingField, (Action<PickupSpawnPoint>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<PickupSpawnPoint> action = OnPickupSpawnPointRegistered__BackingField;
				Action<PickupSpawnPoint> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPickupSpawnPointRegistered__BackingField, (Action<PickupSpawnPoint>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<PickupSpawnPoint> OnPickupSpawnPointDeregistered
		{
			add
			{
				Action<PickupSpawnPoint> action = OnPickupSpawnPointDeregistered__BackingField;
				Action<PickupSpawnPoint> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPickupSpawnPointDeregistered__BackingField, (Action<PickupSpawnPoint>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<PickupSpawnPoint> action = OnPickupSpawnPointDeregistered__BackingField;
				Action<PickupSpawnPoint> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPickupSpawnPointDeregistered__BackingField, (Action<PickupSpawnPoint>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<PickerRemovedEventArgs> OnPickerRemoved
		{
			add
			{
				Action<PickerRemovedEventArgs> action = OnPickerRemoved__BackingField;
				Action<PickerRemovedEventArgs> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPickerRemoved__BackingField, (Action<PickerRemovedEventArgs>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<PickerRemovedEventArgs> action = OnPickerRemoved__BackingField;
				Action<PickerRemovedEventArgs> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPickerRemoved__BackingField, (Action<PickerRemovedEventArgs>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		protected override void SubclassInitialize()
		{
			HierarchyUtility.ReparentByHierarchyPathType(base.transform, HierarchyPathType.DynamicManagers);
			ResetPickupAndPickerLastPositions(ReasonToResetHistory.SESSION_STARTED);
			_pickupSpawnPoints = new List<PickupSpawnPoint>();
			_pickerCollection = new PickerCollection(this);
			_pickerCollection.OnPickerRemoveRequest += PickerCollection_OnPickerRemoveRequest;
			PickerData[] pickerData = _pickerData;
			foreach (PickerData pickerData2 in pickerData)
			{
				if (pickerData2.PickerEnabledState == PickerEnabledState.Enabled)
				{
					switch (pickerData2.PickerType)
					{
					case PickerType.Default:
						_pickerCollection.Add(new DefaultPicker());
						break;
					case PickerType.LetterHuntDaily:
						_pickerCollection.Add(new LetterHuntDailyPicker());
						break;
					case PickerType.TokenHunt:
						_pickerCollection.Add(new TokenHuntPicker());
						break;
					case PickerType.SuperMysteryBox:
						_pickerCollection.Add(new SuperMysteryBoxPicker());
						break;
					case PickerType.MysteryPowerUp:
						_pickerCollection.Add(new MysteryPowerupPicker());
						break;
					case PickerType.LetterHuntCharacter:
						_pickerCollection.Add(new LetterHuntCharacterPicker());
						break;
					case PickerType.XTest:
						_pickerCollection.Add(new XTestPicker());
						break;
					default:
						throw new SwitchDefaultException(pickerData2.PickerType.ToString());
					}
				}
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (_pickerCollection != null)
			{
				_pickerCollection.OnPickerRemoveRequest -= PickerCollection_OnPickerRemoveRequest;
			}
		}

		protected void FixedUpdate()
		{
			if (Game.IsInstantiated && Game.Instance.IsInGame.Value && !Game.Instance.IsPaused)
			{
				UpdatePickerCollection(false);
			}
		}

		public PickupData GetPickupDataForPickupType(PickupType pickupType)
		{
			if (_pickupData != null)
			{
				for (int i = 0; i < _pickupData.Length; i++)
				{
					if (_pickupData[i].PickupType == pickupType)
					{
						return _pickupData[i];
					}
				}
			}
			return null;
		}

		public bool HasPickupDataForPickupType(PickupType pickupType)
		{
			return GetPickupDataForPickupType(pickupType) != null;
		}

		internal PickerData GetPickerDataForPickerType(PickerType pickerType)
		{
			PickerData result = null;
			for (int i = 0; i < _pickerData.Length; i++)
			{
				PickerData pickerData = _pickerData[i];
				if (pickerData.PickerType == pickerType)
				{
					result = pickerData;
					break;
				}
			}
			return result;
		}

		private void UpdatePickerCollection(bool isForcedUpdate)
		{
			if (isForcedUpdate)
			{
				_delayBeforeUpdatingPickerCollectionCount = 60;
			}
			if (++_delayBeforeUpdatingPickerCollectionCount < 60)
			{
				return;
			}
			_delayBeforeUpdatingPickerCollectionCount = 0;
			_pickerCollection.UpdatePickers();
			foreach (PickupSpawnPoint pickupSpawnPoint in _pickupSpawnPoints)
			{
				if (pickupSpawnPoint.IsAvailableForSpawning())
				{
					pickupSpawnPoint.MarkAsUnavailable();
				}
			}
		}

		public void UpdatePickerCollectionForced()
		{
			UpdatePickerCollection(true);
		}

		public void SetPickerState(PickerType pickerType, PickerEnabledState state)
		{
			PickerData pickerDataForPickerType = GetPickerDataForPickerType(pickerType);
			pickerDataForPickerType.PickerEnabledState = state;
		}

		public void RegisterPickupSpawnPoint(PickupSpawnPoint pickupSpawnPoint)
		{
			if (!_pickupSpawnPoints.Contains(pickupSpawnPoint))
			{
				pickupSpawnPoint.SetContainingRoute();
				_pickupSpawnPoints.Add(pickupSpawnPoint);
				_pickupSpawnPoints.Sort(SortSpawnPointsClosestFirst);
				pickupSpawnPoint.OnPickupSpawned += PickupSpawnPoint_OnPickupSpawned;
				pickupSpawnPoint.OnPickupCollected += PickupSpawnPoint_OnPickupCollected;
				pickupSpawnPoint.OnPickupDespawned += PickupSpawnPoint_OnPickupDespawned;
				pickupSpawnPoint.OnRegistered += PickupSpawnPoint_OnRegistered;
				pickupSpawnPoint.OnDeregistered += PickupSpawnPoint_OnDeregistered;
				pickupSpawnPoint.Register();
			}
		}

		public void DeregisterPickupSpawnPoint(PickupSpawnPoint pickupSpawnPoint)
		{
			if (_pickupSpawnPoints.Contains(pickupSpawnPoint))
			{
				_pickupSpawnPoints.Remove(pickupSpawnPoint);
			}
			pickupSpawnPoint.Deregister();
		}

		public GameObject SpawnPickupSpawnPoint()
		{
			return Spawn(_pickupSpawnPointPrefab.transform, Vector3.zero, Quaternion.identity);
		}

		public void Despawn(Transform transformToDespawn)
		{
			PoolManager.Despawn(transformToDespawn);
		}

		public bool IsSpawned(Transform transformToDespawn)
		{
			PooledInstanceData instanceData = PoolManager.GetInstanceData(transformToDespawn);
			return instanceData != null && instanceData.inUse;
		}

		public GameObject Spawn(Transform transformToSpawn, Vector3 position, Quaternion rotation)
		{
			return PoolManager.Spawn(transformToSpawn).gameObject;
		}

		private void ResetPickupAndPickerLastPositions(ReasonToResetHistory reasonToResetHistory)
		{
			ResetPickupLastSpawnPositions(reasonToResetHistory);
			ResetPickerLastSpawnPositions(reasonToResetHistory);
		}

		private void ResetPickupLastSpawnPositions(ReasonToResetHistory reasonToResetHistory)
		{
			PickupData[] pickupData = _pickupData;
			foreach (PickupData pickupData2 in pickupData)
			{
				if ((reasonToResetHistory == ReasonToResetHistory.GAME_STARTED && pickupData2.SpawnDistanceMinToPreviousMode == SpawnDistanceMinToPreviousMode.ResetAtGameStart) || (reasonToResetHistory == ReasonToResetHistory.RUN_STARTED && pickupData2.SpawnDistanceMinToPreviousMode == SpawnDistanceMinToPreviousMode.ResetAtRunStart))
				{
					PickupLastSpawnZGamePosition[(int)pickupData2.PickupType] = 0f;
				}
			}
		}

		private void ResetPickerLastSpawnPositions(ReasonToResetHistory reasonToResetHistory)
		{
			PickerData[] pickerData = _pickerData;
			foreach (PickerData pickerData2 in pickerData)
			{
				if ((reasonToResetHistory == ReasonToResetHistory.GAME_STARTED && pickerData2.SpawnDistanceMinToPreviousMode == SpawnDistanceMinToPreviousMode.ResetAtGameStart) || (reasonToResetHistory == ReasonToResetHistory.RUN_STARTED && pickerData2.SpawnDistanceMinToPreviousMode == SpawnDistanceMinToPreviousMode.ResetAtRunStart))
				{
					PickerLastSpawnZGamePosition[(int)pickerData2.PickerType] = 0f;
				}
			}
		}

		public void Game_OnGameStarted()
		{
			ResetPickupAndPickerLastPositions(ReasonToResetHistory.GAME_STARTED);
		}

		public void Game_OnRunStarted(bool wasPlayerRevived, bool revivedWithAds = false, bool adsWasAvailable = true, bool adsFailed = false)
		{
			ResetPickupAndPickerLastPositions(ReasonToResetHistory.RUN_STARTED);
		}

		private void PickerCollection_OnPickerRemoveRequest(PickerRemoveRequestEventArgs pickerRemoveRequestEvent)
		{
			_pickerCollection.Remove(pickerRemoveRequestEvent.IPicker);
			if (OnPickerRemoved__BackingField != null)
			{
				OnPickerRemoved__BackingField(new PickerRemovedEventArgs(pickerRemoveRequestEvent.IPicker));
			}
		}

		private void PickupSpawnPoint_OnPickupSpawned(PickupSpawnPoint pickupSpawnPoint, PickerData pickerData)
		{
			PickupType pickupType = pickupSpawnPoint.PickupData.PickupType;
			if (0f < pickupSpawnPoint.PickupData.SpawnDistanceMinToPrevious)
			{
				PickupLastSpawnZGamePosition[(int)pickupType] = GamePositionRoot.Instance.GetGamePosition(pickupSpawnPoint.transform).z;
			}
			if (pickupSpawnPoint.SpawnPointMode != SpawnPointMode.WillForcePickupType && 0f < pickerData.SpawnDistanceMinToPrevious)
			{
				PickerLastSpawnZGamePosition[(int)pickerData.PickerType] = GamePositionRoot.Instance.GetGamePosition(pickupSpawnPoint.transform).z;
			}
			if (OnPickupSpawned__BackingField != null)
			{
				PickupEventArgs e = new PickupEventArgs(pickupSpawnPoint, pickupSpawnPoint.PickupData, pickupSpawnPoint.SpawnPointMode);
				OnPickupSpawned__BackingField(this, e);
			}
		}

		private void PickupSpawnPoint_OnPickupCollected(PickupSpawnPoint pickupSpawnPoint)
		{
			if (OnPickupCollected__BackingField != null)
			{
				PickupEventArgs e = new PickupEventArgs(pickupSpawnPoint, pickupSpawnPoint.PickupData, pickupSpawnPoint.SpawnPointMode);
				OnPickupCollected__BackingField(this, e);
			}
		}

		private void PickupSpawnPoint_OnPickupDespawned(PickupSpawnPoint pickupSpawnPoint)
		{
			if (OnPickupDespawned__BackingField != null)
			{
				PickupEventArgs e = new PickupEventArgs(pickupSpawnPoint, pickupSpawnPoint.PickupData, pickupSpawnPoint.SpawnPointMode);
				OnPickupDespawned__BackingField(this, e);
			}
		}

		private void PickupSpawnPoint_OnRegistered(PickupSpawnPoint pickupSpawnPoint)
		{
			if (OnPickupSpawnPointRegistered__BackingField != null)
			{
				OnPickupSpawnPointRegistered__BackingField(pickupSpawnPoint);
			}
		}

		private void PickupSpawnPoint_OnDeregistered(PickupSpawnPoint pickupSpawnPoint)
		{
			pickupSpawnPoint.OnPickupSpawned -= PickupSpawnPoint_OnPickupSpawned;
			pickupSpawnPoint.OnPickupCollected -= PickupSpawnPoint_OnPickupCollected;
			pickupSpawnPoint.OnPickupDespawned -= PickupSpawnPoint_OnPickupDespawned;
			pickupSpawnPoint.OnRegistered -= PickupSpawnPoint_OnRegistered;
			pickupSpawnPoint.OnDeregistered -= PickupSpawnPoint_OnDeregistered;
			if (OnPickupSpawnPointDeregistered__BackingField != null)
			{
				OnPickupSpawnPointDeregistered__BackingField(pickupSpawnPoint);
			}
		}

		private static int SortSpawnPointsClosestFirst(PickupSpawnPoint left, PickupSpawnPoint right)
		{
			return left.transform.position.z.CompareTo(right.transform.position.z);
		}
	}
}
