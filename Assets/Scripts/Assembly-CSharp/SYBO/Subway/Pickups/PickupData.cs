using System;
using SYBO.Common.Utilities.Flags;
using SYBO.Components.Utilities;
using SYBO.Subway.Level;
using SYBO.Subway.Routes.Builders;
using UnityEngine;

namespace SYBO.Subway.Pickups
{
	[Serializable]
	public class PickupData : ICloneable
	{
		[SerializeField]
		private PickupType _pickupType;

		[SerializeField]
		[Tooltip("The pickup's visuals prefab with no code components")]
		private GameObject _pickupVisualsPrefab;

		[SerializeField]
		[Tooltip("Manually set to to TRUE for most. Others are programatically set based on Game Events.")]
		private PickupEnabledState _pickupEnabledState;

		[SerializeField]
		[EnumFlagProperty(typeof(SpawnPointDifficulty))]
		[Tooltip("Determines in which difficulty spawn points pickup can spawn.")]
		private SpawnPointDifficulty _spawnPointDifficulty;

		[SerializeField]
		[EnumFlagProperty(typeof(SpawnPointHeight))]
		[Tooltip("Determines in what height spawn points pickup can spawn.")]
		private SpawnPointHeight _spawnPointHeight;

		[SerializeField]
		[Tooltip("Determines spawning cooldown resetting behaviour.")]
		private SpawnDistanceMinToPreviousMode _spawnDistanceMinToPreviousMode;

		[SerializeField]
		[Range(0f, 10000f)]
		[Tooltip("Determines minimums cooldown distance before this space pickups can be spawned.")]
		private float _spawnDistanceMinToPrevious = 400f;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Determines probability to spawn this pickups relative to total weight of all pickups of a picker")]
		private float _spawnWeight;

		public PickupType PickupType
		{
			get
			{
				return _pickupType;
			}
		}

		public GameObject PickupVisualsPrefab
		{
			get
			{
				return _pickupVisualsPrefab;
			}
		}

		public PickupEnabledState PickupEnabledState
		{
			get
			{
				return _pickupEnabledState;
			}
			set
			{
				_pickupEnabledState = value;
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

		public float SpawnWeight
		{
			get
			{
				return _spawnWeight;
			}
			set
			{
				_spawnWeight = value;
			}
		}

		public PickupData()
			: this(PickupType.CoinMagnet, null, PickupEnabledState.Enabled, (SpawnPointHeight)0, (SpawnPointDifficulty)0, SpawnDistanceMinToPreviousMode.ResetAtRunStart, 400f)
		{
		}

		public PickupData(PickupType pickupType, GameObject pickupVisualsPrefab, PickupEnabledState pickupEnabledState, SpawnPointHeight spawnPointHeight, SpawnPointDifficulty spawnPointDifficulty, SpawnDistanceMinToPreviousMode spawnDistanceMinToPreviousMode, float spawnDistanceMinToPrevious)
		{
			_pickupType = pickupType;
			_pickupVisualsPrefab = pickupVisualsPrefab;
			_pickupEnabledState = pickupEnabledState;
			_spawnPointHeight = spawnPointHeight;
			_spawnPointDifficulty = spawnPointDifficulty;
			_spawnDistanceMinToPreviousMode = spawnDistanceMinToPreviousMode;
			_spawnDistanceMinToPrevious = spawnDistanceMinToPrevious;
		}

		public PickupData(PickupData other)
		{
			_pickupType = other._pickupType;
			_pickupVisualsPrefab = other._pickupVisualsPrefab;
			_pickupEnabledState = other._pickupEnabledState;
			_spawnPointHeight = other._spawnPointHeight;
			_spawnPointDifficulty = other._spawnPointDifficulty;
			_spawnDistanceMinToPreviousMode = other._spawnDistanceMinToPreviousMode;
			_spawnDistanceMinToPrevious = other._spawnDistanceMinToPrevious;
		}

		public object Clone()
		{
			return new PickupData(this);
		}

		public override string ToString()
		{
			string arg = ((!_pickupVisualsPrefab) ? _pickupVisualsPrefab.name : "NA");
			return string.Format("[PickupData (PickupType={0},PickupIconPrefab={1})]", PickupType, arg);
		}

		public bool IsCompatibleWithSpawnPoint(PickupSpawnPoint pickupSpawnPoint)
		{
			return IsCompatibleWithSpawnPointZPosition(pickupSpawnPoint) && IsCompatibleWithSpawnPointDifficulty(pickupSpawnPoint) && IsCompatibleWithSpawnPointHeight(pickupSpawnPoint) && IsCompatibleWithConstrainingBuilder(pickupSpawnPoint);
		}

		private bool IsCompatibleWithSpawnPointDifficulty(PickupSpawnPoint pickupSpawnPoint)
		{
			return FlagsHelper.IsSet(SpawnPointDifficulty, pickupSpawnPoint.SpawnPointDifficulty);
		}

		private bool IsCompatibleWithSpawnPointHeight(PickupSpawnPoint pickupSpawnPoint)
		{
			return FlagsHelper.IsSet(SpawnPointHeight, pickupSpawnPoint.SpawnPointHeight);
		}

		private bool IsCompatibleWithSpawnPointZPosition(PickupSpawnPoint pickupSpawnPoint)
		{
			bool flag = false;
			if (!SingletonMonoBehaviour<PickupManager>.IsInstantiated())
			{
				return false;
			}
			PickupManager instance = SingletonMonoBehaviour<PickupManager>.Instance;
			float value = 0f;
			if (!instance.PickupLastSpawnZGamePosition.TryGetValue((int)_pickupType, out value))
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

		private bool IsCompatibleWithConstrainingBuilder(PickupSpawnPoint pickupSpawnPoint)
		{
			if (_pickupType == PickupType.JetPack && pickupSpawnPoint.ContainingRoute != null)
			{
				RouteBuilder builder = pickupSpawnPoint.ContainingRoute.Builder;
				for (int i = 0; i < builder.ConstrainingBuilders.Count; i++)
				{
					ConstrainingRouteBuilder constrainingRouteBuilder = (ConstrainingRouteBuilder)builder.ConstrainingBuilders[i];
					int rangeStartZ = (int)pickupSpawnPoint.transform.position.z;
					if (constrainingRouteBuilder.BuildersBelow.Contains(builder) && constrainingRouteBuilder.IsConstrainingStartAt(rangeStartZ))
					{
						return false;
					}
				}
			}
			return true;
		}

		private void DebugLog(string message)
		{
			if (SingletonMonoBehaviour<PickupManager>.IsInstantiated() && !SingletonMonoBehaviour<PickupManager>.Instance.WillDebugLogPickupData)
			{
			}
		}
	}
}
