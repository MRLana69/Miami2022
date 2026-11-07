using System;
using SYBO.Common.Utilities.Flags;
using SYBO.Components.Utilities;
using SYBO.Subway.Level;
using UnityEngine;

namespace SYBO.Subway.Pickups.Pickers
{
	[Serializable]
	public class PickerData
	{
		[SerializeField]
		internal PickerType _pickerType;

		[SerializeField]
		[Tooltip("Manually set to to true for most. Others are programatically set based on (...user progress?).")]
		private PickerEnabledState _pickerEnabledState;

		[Tooltip("The Pickup Types allowed to spawn")]
		[SerializeField]
		[EnumFlagProperty(typeof(PickupType))]
		internal PickupType _pickupType;

		[SerializeField]
		[Tooltip("Determines spawning cooldown resetting behaviour")]
		private SpawnDistanceMinToPreviousMode _spawnDistanceMinToPreviousMode;

		[SerializeField]
		[Range(0f, 100000f)]
		[Tooltip("Spawning: Minimum distance between ex. 'AnythingThisPickerSpawns' and previous 'AnythingThisPickerSpawns'...")]
		private float _spawnDistanceMinToPrevious;

		[Tooltip("0 = never ATTEMPT, 1 = always ATTEMPT, etc...")]
		[Range(0f, 1f)]
		[SerializeField]
		internal float _probabilityToSpawn;

		[Tooltip("Used to order active pickers. Lowest order eavluates first.")]
		[SerializeField]
		protected int _executionOrder;

		public PickerType PickerType
		{
			get
			{
				return _pickerType;
			}
		}

		public PickerEnabledState PickerEnabledState
		{
			get
			{
				return _pickerEnabledState;
			}
			set
			{
				_pickerEnabledState = value;
			}
		}

		public PickupType PickupType
		{
			get
			{
				return _pickupType;
			}
		}

		public SpawnDistanceMinToPreviousMode SpawnDistanceMinToPreviousMode
		{
			get
			{
				return _spawnDistanceMinToPreviousMode;
			}
			set
			{
				_spawnDistanceMinToPreviousMode = value;
			}
		}

		public float SpawnDistanceMinToPrevious
		{
			get
			{
				return _spawnDistanceMinToPrevious;
			}
		}

		public float ProbabilityToSpawn
		{
			get
			{
				return _probabilityToSpawn;
			}
		}

		public int ExecutionOrder
		{
			get
			{
				return _executionOrder;
			}
		}

		public PickerData()
		{
			FlagsHelper.SetEverything(ref _pickupType);
			_executionOrder = 0;
			_probabilityToSpawn = 1f;
			_pickerType = PickerType.Default;
			_spawnDistanceMinToPreviousMode = SpawnDistanceMinToPreviousMode.ResetAtRunStart;
			_spawnDistanceMinToPrevious = 0f;
		}

		public bool IsCompatibleWithSpawnPoint(PickupSpawnPoint pickupSpawnPoint)
		{
			bool flag = false;
			if (!SingletonMonoBehaviour<PickupManager>.IsInstantiated())
			{
				return false;
			}
			PickupManager instance = SingletonMonoBehaviour<PickupManager>.Instance;
			float value = 0f;
			if (!instance.PickerLastSpawnZGamePosition.TryGetValue((int)_pickerType, out value))
			{
				return true;
			}
			if (value == 0f)
			{
				return true;
			}
			float num = Game.ConvertUnityUnitsToGameMeters(value);
			float num2 = Game.ConvertUnityUnitsToGameMeters(GamePositionRoot.Instance.GetGamePosition(pickupSpawnPoint.transform).z);
			float num3 = num2 - num;
			return _spawnDistanceMinToPrevious < num3;
		}

		public override string ToString()
		{
			return string.Format("[PickerData: PickerType={0}, PickupType={1}]", _pickerType, _pickupType);
		}

		private void DebugLog(string message)
		{
			if (SingletonMonoBehaviour<PickupManager>.IsInstantiated() && !SingletonMonoBehaviour<PickupManager>.Instance.WillDebugLogPickupData)
			{
			}
		}
	}
}
