using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace SYBO.Components.Utilities
{
	public static class PoolManager
	{
		private class PoolPriority
		{
			public int Priority;

			public bool IsSpawnPriority;

			public SpawnPool Pool;
		}

		private class PoolPriorityComparer : IComparer<PoolPriority>
		{
			public int Compare(PoolPriority x, PoolPriority y)
			{
				return x.Priority - y.Priority;
			}
		}

		public static Action<Transform> OnObjectInstantiated;

		public static readonly SpawnPoolsDict Pools = new SpawnPoolsDict();

		private static bool _balancing;

		private static bool _restartBalancing;

		private static bool _forceRestartBalancing;

		private static Action<float> _onProgress;

		public static int BalancingBudgetMS = 5;

		private static List<PoolPriority> _priorities;

		private static PoolPriorityComparer _comparer;

		private static int _frameCount;

		private static Stopwatch _sw = new Stopwatch();

		private static Dictionary<Transform, PrefabPool> _prefabPoolLookup = new Dictionary<Transform, PrefabPool>();

		private static Dictionary<int, PooledInstanceData> _pooledLookup = new Dictionary<int, PooledInstanceData>();

		public static bool Working
		{
			get
			{
				return _balancing;
			}
		}

		public static void TriggerPoolBalancing(MonoBehaviour coroutineHost = null)
		{
			if (coroutineHost == null)
			{
				coroutineHost = GetDefaultSpawnPool();
			}
			_restartBalancing = true;
			if (!_balancing)
			{
				_balancing = true;
				coroutineHost.StartCoroutine(PoolBalancing());
			}
		}

		public static void BeginBalancing(Action<float> onProgress)
		{
			_onProgress = onProgress;
			if (onProgress != null)
			{
				onProgress(0f);
			}
			_forceRestartBalancing = true;
			_restartBalancing = true;
			TriggerPoolBalancing();
		}

		public static void StartClock()
		{
			if (_frameCount != Time.frameCount)
			{
				_sw.Reset();
				_frameCount = Time.frameCount;
			}
			_sw.Start();
		}

		public static void StopClock()
		{
			_sw.Stop();
		}

		public static void RegisterInstance(PooledInstanceData instanceData)
		{
			_pooledLookup.Add(instanceData.instanceId, instanceData);
			if (OnObjectInstantiated != null)
			{
				OnObjectInstantiated(instanceData.transform);
			}
		}

		public static void UnregisterInstance(PooledInstanceData instanceData)
		{
			_pooledLookup.Remove(instanceData.instanceId);
		}

		public static PooledInstanceData GetInstanceData(Transform instance)
		{
			int instanceID = instance.GetInstanceID();
			PooledInstanceData value;
			_pooledLookup.TryGetValue(instanceID, out value);
			return value;
		}

		public static Transform Spawn(Transform prefab)
		{
			PrefabPool value;
			if (!_prefabPoolLookup.TryGetValue(prefab, out value))
			{
				SpawnPool defaultSpawnPool = GetDefaultSpawnPool();
				return defaultSpawnPool.Spawn(prefab);
			}
			return value.spawnPool.Spawn(prefab);
		}

		private static SpawnPool GetDefaultSpawnPool()
		{
			SpawnPool spawnPool;
			if (!Pools.TryGetValue("Default", out spawnPool))
			{
				GameObject gameObject = new GameObject("DefaultPool");
				return gameObject.AddComponent<SpawnPool>();
			}
			return spawnPool;
		}

		public static void Despawn(Transform instance)
		{
			PooledInstanceData instanceData = GetInstanceData(instance);
			instanceData.pool.spawnPool.Despawn(instance);
		}

		private static IEnumerator PoolBalancing()
		{
			yield return null;
			while (_restartBalancing)
			{
				_restartBalancing = false;
				_forceRestartBalancing = false;
				Action<float> thisProgress = _onProgress;
				_onProgress = null;
				StartClock();
				float totalBalanceOperations = UpdatePriorityList();
				float balanceOperation = 0f;
				for (int i = 0; i < _priorities.Count; i++)
				{
					PoolPriority priority = _priorities[i];
					for (int j = 0; j < priority.Pool.PrefabPoolCount; j++)
					{
						PrefabPool prefabPool = priority.Pool.GetPrefabPool(j);
						priority.Pool.logLevelAdjust++;
						if (priority.IsSpawnPriority)
						{
							while (prefabPool.totalCount < prefabPool.preloadAmount)
							{
								Transform inst = prefabPool.SpawnNew();
								prefabPool.DespawnInstance(inst, false);
								balanceOperation++;
								if (_sw.ElapsedMilliseconds >= BalancingBudgetMS)
								{
									priority.Pool.logLevelAdjust--;
									StopClock();
									if (thisProgress != null)
									{
										thisProgress(balanceOperation / totalBalanceOperations);
									}
									yield return null;
									StartClock();
									priority.Pool.logLevelAdjust++;
									if (_forceRestartBalancing)
									{
										break;
									}
								}
							}
						}
						else
						{
							if (!prefabPool.cullDespawned)
							{
								continue;
							}
							while (prefabPool.totalCount > prefabPool.cullAbove)
							{
								if (prefabPool._despawned.Count == 0)
								{
									_restartBalancing = true;
									break;
								}
								LinkedListNode<PooledInstanceData> node = prefabPool._despawned.Last;
								Transform inst = node.Value.transform;
								prefabPool._despawned.Remove(node);
								UnregisterInstance(node.Value);
								prefabPool.spawnPool.DestroyInstance(inst.gameObject);
								balanceOperation++;
								if (_sw.ElapsedMilliseconds >= BalancingBudgetMS)
								{
									priority.Pool.logLevelAdjust--;
									StopClock();
									if (thisProgress != null)
									{
										thisProgress(balanceOperation / totalBalanceOperations);
									}
									yield return null;
									StartClock();
									priority.Pool.logLevelAdjust++;
									if (_forceRestartBalancing)
									{
										break;
									}
								}
							}
						}
						priority.Pool.logLevelAdjust--;
						if (_forceRestartBalancing)
						{
							break;
						}
					}
					if (_forceRestartBalancing)
					{
						break;
					}
				}
				if (thisProgress != null)
				{
					thisProgress(1f);
				}
			}
			StopClock();
			_balancing = false;
		}

		private static int UpdatePriorityList()
		{
			int num = Pools.Count * 2;
			if (_priorities == null)
			{
				_priorities = new List<PoolPriority>(num);
				_comparer = new PoolPriorityComparer();
			}
			while (_priorities.Count < num)
			{
				_priorities.Add(new PoolPriority());
			}
			if (_priorities.Count > num)
			{
				_priorities.RemoveRange(num, _priorities.Count - num);
			}
			int num2 = 0;
			int num3 = 0;
			foreach (KeyValuePair<string, SpawnPool> pool in Pools)
			{
				SpawnPool value = pool.Value;
				num3 += value.GetBalanceOperations();
				PoolPriority poolPriority = _priorities[num2++];
				poolPriority.IsSpawnPriority = true;
				poolPriority.Priority = value.spawnPriority;
				poolPriority.Pool = value;
				poolPriority = _priorities[num2++];
				poolPriority.IsSpawnPriority = false;
				poolPriority.Priority = value.despawnPriority;
				poolPriority.Pool = value;
			}
			_priorities.Sort(_comparer);
			return num3;
		}

		public static void AddPool(PrefabPool prefabPool)
		{
			_prefabPoolLookup.Add(prefabPool.prefab, prefabPool);
		}

		public static PrefabPool GetPrefabPool(Transform prefab)
		{
			PrefabPool value;
			if (!_prefabPoolLookup.TryGetValue(prefab, out value))
			{
				return GetDefaultSpawnPool().GetPrefabPool(prefab);
			}
			return value;
		}

		public static bool PrefabPoolExists(Transform prefab)
		{
			return _prefabPoolLookup.ContainsKey(prefab);
		}

		public static void Remove(SpawnPool spawnPool)
		{
			for (int i = 0; i < spawnPool.PrefabPoolCount; i++)
			{
				PrefabPool prefabPool = spawnPool.GetPrefabPool(i);
				_prefabPoolLookup.Remove(prefabPool.prefab);
			}
			Pools.Remove(spawnPool);
		}
	}
}
