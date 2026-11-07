using System;
using System.Collections.Generic;
using SYBO.Components.Utilities;
using SYBO.Subway.Characters;
using SYBO.Subway.Routes;
using SYBO.Subway.Routes.Builders;
using SYBO.Subway.Themes;
using UnityEngine;

namespace SYBO.Subway.Level.Environments
{
	public class DistantEnvironmentController : SingletonMonoBehaviour<DistantEnvironmentController>
	{
		public enum EnvironmentSide
		{
			Left = 0,
			Right = 1,
			Both = 2
		}

		[Serializable]
		public class DistantObjectGlobalConfig
		{
			[SerializeField]
			private float _globalMinDistanceFirstSpawn;

			[SerializeField]
			private float _globalMinDistanceBetween;

			public float GlobalMinDistanceFirstSpawn
			{
				get
				{
					return _globalMinDistanceFirstSpawn;
				}
			}

			public float GlobalMinDistanceBetween
			{
				get
				{
					return _globalMinDistanceBetween;
				}
			}
		}

		[Serializable]
		public class DistantObjectConfig
		{
			[SerializeField]
			private string _name = string.Empty;

			[SerializeField]
			private Transform[] _prefabs;

			[SerializeField]
			private EnvironmentSide _environmentSide = EnvironmentSide.Both;

			[SerializeField]
			private EnvironmentKind[] _environmentKind = new EnvironmentKind[0];

			[SerializeField]
			[Range(250f, 1500f)]
			[Tooltip("Min offset for distant position.")]
			private float _projectionSourceOffsetXMin;

			[SerializeField]
			[Range(250f, 1500f)]
			[Tooltip("Max offset for distant position.")]
			private float _projectionSourceOffsetXMax;

			[SerializeField]
			private float _minDistanceFirstSpawn;

			[SerializeField]
			private float _minDistanceBetween;

			[SerializeField]
			[Range(0f, 1f)]
			private float _spawnProbability;

			public string Name
			{
				get
				{
					return _name;
				}
			}

			public Transform[] Prefabs
			{
				get
				{
					return _prefabs;
				}
			}

			public EnvironmentSide EnvironmentSide
			{
				get
				{
					return _environmentSide;
				}
			}

			public EnvironmentKind[] EnvironmentKind
			{
				get
				{
					return _environmentKind;
				}
			}

			public float ProjectionSourceOffsetXMin
			{
				get
				{
					return _projectionSourceOffsetXMin;
				}
			}

			public float ProjectionSourceOffsetXMax
			{
				get
				{
					return _projectionSourceOffsetXMax;
				}
			}

			public float MinDistanceFirstSpawn
			{
				get
				{
					return _minDistanceFirstSpawn;
				}
			}

			public float MinDistanceBetween
			{
				get
				{
					return _minDistanceBetween;
				}
			}

			public float SpawnProbability
			{
				get
				{
					return _spawnProbability;
				}
			}

			public Transform GetRandomPrefab()
			{
				return _prefabs[UnityEngine.Random.Range(0, _prefabs.Length)];
			}
		}

		private class DistantObjectSpawner
		{
			private List<Transform> _spawnedTransforms = new List<Transform>();

			private float _lastSpawnGamePositionZ = -1260f;

			public DistantObjectConfig Config { get; private set; }

			public DistantObjectSpawner(DistantObjectConfig config)
			{
				Config = config;
				Reset();
			}

			public void Reset()
			{
				for (int i = 0; i < _spawnedTransforms.Count; i++)
				{
					PoolManager.Despawn(_spawnedTransforms[i]);
				}
				_spawnedTransforms.Clear();
				_lastSpawnGamePositionZ = -1260f;
			}

			public bool CanSpawnAt(float gamePositionZ, EnvironmentKind environmentKind)
			{
				if (CanSpawnAtEnvironmentKind(environmentKind) && gamePositionZ >= Config.MinDistanceFirstSpawn && gamePositionZ >= _lastSpawnGamePositionZ + Config.MinDistanceBetween)
				{
					return true;
				}
				return false;
			}

			private bool CanSpawnAtEnvironmentKind(EnvironmentKind environmentKind)
			{
				if (Config.EnvironmentKind.Length == 0)
				{
					return true;
				}
				for (int i = 0; i < Config.EnvironmentKind.Length; i++)
				{
					if (Config.EnvironmentKind[i] == environmentKind)
					{
						return true;
					}
				}
				return false;
			}

			public void Spawn(float spawnGamePositionZ, Vector3 cameraGamePosition, Transform parent)
			{
				Transform transform = PoolManager.Spawn(Config.GetRandomPrefab());
				transform.parent = parent;
				float x = Game.Instance.RandomGenerator.Range(Config.ProjectionSourceOffsetXMin, Config.ProjectionSourceOffsetXMax);
				Vector3 trueSpawnGamePosition = new Vector3(x, 0f, spawnGamePositionZ);
				if (Config.EnvironmentSide == EnvironmentSide.Left || (Config.EnvironmentSide == EnvironmentSide.Both && Game.Instance.RandomGenerator.Value() >= 0.5f))
				{
					trueSpawnGamePosition.x *= -1f;
				}
				float num = Mathf.Abs(trueSpawnGamePosition.x);
				float num2 = 250f + (num - 250f) * (1f / 3f);
				float num3 = num / num2;
				if (num3 > 1f)
				{
					float num4 = trueSpawnGamePosition.z - cameraGamePosition.z;
					trueSpawnGamePosition.z = cameraGamePosition.z + num4 * num3;
				}
				transform.GetComponent<DistantObject>().Initialize(trueSpawnGamePosition, cameraGamePosition, num3);
				_spawnedTransforms.Add(transform);
				_lastSpawnGamePositionZ = spawnGamePositionZ;
			}

			public void CheckForDespawn(float cameraGamePositionZ)
			{
				int num = 0;
				while (num < _spawnedTransforms.Count)
				{
					if (GamePositionRoot.Instance.GetGamePosition(_spawnedTransforms[num]).z < cameraGamePositionZ - -90f)
					{
						PoolManager.Despawn(_spawnedTransforms[num]);
						_spawnedTransforms.RemoveAt(num);
					}
					else
					{
						num++;
					}
				}
			}

			public bool HasSpawned(Transform instance)
			{
				for (int i = 0; i < _spawnedTransforms.Count; i++)
				{
					if (_spawnedTransforms[i] == instance)
					{
						return true;
					}
				}
				return false;
			}
		}

		private const float SpawnDistance = 1260f;

		private const float SpawnCheckDistance = 90f;

		private const float DistantObjectDespawnDistance = -90f;

		private const float MinimumXOffset = 250f;

		private const float MaximumXOffset = 1500f;

		private const float MaximumProjectedX = 500f;

		private bool _distantEnvironmentActive;

		private DistantObjectGlobalConfig _distantObjectGlobalConfig;

		private DistantObjectSpawner[] _distantObjectSpawners;

		private float _lastCharacterGamePositionZ;

		private float _distanceSinceLastSpawnCheck;

		private float _lastSpawnGamePositionZ;

		private List<DistantObject> _activeDistantObjects = new List<DistantObject>();

		private List<DistantObjectSpawner> _spawnableSpawners = new List<DistantObjectSpawner>();

		private void Start()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			Theme theme = ThemeManager.Instance.Theme;
			_distantEnvironmentActive = theme.DistantObjectConfigs.Length > 0;
			if (!_distantEnvironmentActive)
			{
				return;
			}
			_distantObjectGlobalConfig = theme.DistantObjectGlobalConfig;
			_distantObjectSpawners = new DistantObjectSpawner[theme.DistantObjectConfigs.Length];
			for (int i = 0; i < _distantObjectSpawners.Length; i++)
			{
				_distantObjectSpawners[i] = new DistantObjectSpawner(theme.DistantObjectConfigs[i]);
				Transform[] prefabs = _distantObjectSpawners[i].Config.Prefabs;
				foreach (Transform prefab in prefabs)
				{
					PoolManager.GetPrefabPool(prefab).preloadAmount = Mathf.FloorToInt(1260f / _distantObjectSpawners[i].Config.MinDistanceBetween);
				}
			}
			PoolManager.TriggerPoolBalancing();
			Game.OnMenuScreenShown += Game_OnMenuScreenShown;
		}

		private void LateUpdate()
		{
			if (!_distantEnvironmentActive || !(Camera.main != null))
			{
				return;
			}
			float z = Character.Instance.GamePosition.z;
			float num = z - _lastCharacterGamePositionZ;
			Vector3 gamePosition = GamePositionRoot.Instance.GetGamePosition(Camera.main.transform);
			if (num > 0f)
			{
				UpdateDistantEnvironment(num, gamePosition);
			}
			if (_activeDistantObjects.Count > 0)
			{
				for (int i = 0; i < _activeDistantObjects.Count; i++)
				{
					_activeDistantObjects[i].UpdateDistantObject(gamePosition);
				}
			}
			_lastCharacterGamePositionZ = z;
		}

		private void Game_OnMenuScreenShown()
		{
			for (int i = 0; i < _distantObjectSpawners.Length; i++)
			{
				_distantObjectSpawners[i].Reset();
			}
			_lastSpawnGamePositionZ = 0f;
			_lastCharacterGamePositionZ = -1260f;
			UpdateDistantEnvironment(1260f, Vector3.zero);
			_lastCharacterGamePositionZ = 0f;
		}

		private void UpdateDistantEnvironment(float characterDeltaZ, Vector3 cameraGamePosition)
		{
			_distanceSinceLastSpawnCheck += characterDeltaZ;
			while (_distanceSinceLastSpawnCheck > 90f)
			{
				float num = _lastCharacterGamePositionZ + characterDeltaZ;
				float num2 = num - _distanceSinceLastSpawnCheck + 1260f;
				bool flag = num2 >= _distantObjectGlobalConfig.GlobalMinDistanceFirstSpawn;
				bool flag2 = num2 >= _lastSpawnGamePositionZ + _distantObjectGlobalConfig.GlobalMinDistanceBetween;
				if (flag && flag2)
				{
					RouteBuilder builder = SingletonMonoBehaviour<RouteManager>.Instance.MainRoute.Builder;
					for (int i = 0; i < _distantObjectSpawners.Length; i++)
					{
						DistantObjectSpawner distantObjectSpawner = _distantObjectSpawners[i];
						if (distantObjectSpawner.CanSpawnAt(num2, builder.GetEnvironmentKindAt(num2)) && Game.Instance.RandomGenerator.Value() < distantObjectSpawner.Config.SpawnProbability)
						{
							_spawnableSpawners.Add(distantObjectSpawner);
						}
					}
					for (int j = 0; j < _spawnableSpawners.Count; j++)
					{
						_spawnableSpawners[j].Spawn(num2, cameraGamePosition, base.transform);
						_lastSpawnGamePositionZ = num2;
					}
					_spawnableSpawners.Clear();
				}
				_distanceSinceLastSpawnCheck -= 90f;
			}
			for (int k = 0; k < _distantObjectSpawners.Length; k++)
			{
				_distantObjectSpawners[k].CheckForDespawn(cameraGamePosition.z);
			}
		}

		public int GetObjectSettingIndex(Transform instance)
		{
			for (int i = 0; i < _distantObjectSpawners.Length; i++)
			{
				if (_distantObjectSpawners[i].HasSpawned(instance))
				{
					return i;
				}
			}
			return 0;
		}

		public void RegisterDistantObject(DistantObject obj)
		{
			_activeDistantObjects.Add(obj);
		}

		public void DeregisterDistantObject(DistantObject obj)
		{
			_activeDistantObjects.Remove(obj);
		}
	}
}
