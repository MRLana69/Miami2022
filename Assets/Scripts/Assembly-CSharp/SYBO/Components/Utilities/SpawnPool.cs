using System;
using System.Collections;
using System.Collections.Generic;
using SYBO.Components.Logging;
using UnityEngine;

namespace SYBO.Components.Utilities
{
	[AddComponentMenu("Path-o-logical/PoolManager/SpawnPool")]
	public sealed class SpawnPool : MonoBehaviour, IList<Transform>, IEnumerable, ICollection<Transform>, IEnumerable<Transform>
	{
		[Serializable]
		public class PrefabPoolGroup
		{
			private class Comparer : IComparer<PrefabPoolGroup>
			{
				int IComparer<PrefabPoolGroup>.Compare(PrefabPoolGroup x, PrefabPoolGroup y)
				{
					return string.Compare(x.name, y.name);
				}
			}

			public bool expandPrefabs = true;

			public string name;

			public List<PrefabPool> prefabPools = new List<PrefabPool>();

			private static IComparer<PrefabPoolGroup> _comparer;

			public static IComparer<PrefabPoolGroup> comparer
			{
				get
				{
					if (_comparer == null)
					{
						_comparer = new Comparer();
					}
					return _comparer;
				}
			}
		}

		public delegate GameObject InstantiateDelegate(GameObject prefab, Vector3 pos, Quaternion rot);

		public delegate void DestroyDelegate(GameObject instance);

		public int spawnPriority;

		public int despawnPriority;

		public int logLevelAdjust;

		public string poolName = string.Empty;

		public bool dontReparent;

		public bool _dontDestroyOnLoad;

		public bool sendUnityEvents;

		public List<PrefabPoolGroup> _prefabPoolGroups = new List<PrefabPoolGroup>();

		public Dictionary<object, bool> prefabsFoldOutStates = new Dictionary<object, bool>();

		public float maxParticleDespawnTime = 300f;

		public PrefabsDict prefabs = new PrefabsDict();

		public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

		private List<PrefabPool> _prefabPools = new List<PrefabPool>();

		public InstantiateDelegate instantiateDelegates;

		public DestroyDelegate destroyDelegates;

		public bool dontDestroyOnLoad
		{
			get
			{
				return _dontDestroyOnLoad;
			}
			set
			{
				_dontDestroyOnLoad = value;
				if (group != null)
				{
					UnityEngine.Object.DontDestroyOnLoad(group.gameObject);
				}
			}
		}

		public Transform group { get; private set; }

		public Transform this[int index]
		{
			get
			{
				for (int i = 0; i < _prefabPools.Count; i++)
				{
					PrefabPool prefabPool = _prefabPools[i];
					if (index < prefabPool.spawned.Count)
					{
						LinkedListNode<PooledInstanceData> linkedListNode = prefabPool.spawned.First;
						while (index-- > 0)
						{
							linkedListNode = linkedListNode.Next;
						}
						return linkedListNode.Value.transform;
					}
					index -= prefabPool.spawned.Count;
				}
				throw new IndexOutOfRangeException();
			}
			set
			{
				throw new NotImplementedException("Read-only.");
			}
		}

		public int Count
		{
			get
			{
				int num = 0;
				for (int i = 0; i < _prefabPools.Count; i++)
				{
					PrefabPool prefabPool = _prefabPools[i];
					num += prefabPool.spawned.Count;
				}
				return num;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int PrefabPoolCount
		{
			get
			{
				return _prefabPools.Count;
			}
		}

		private void Awake()
		{
			if (_dontDestroyOnLoad)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			group = base.transform;
			if (poolName == string.Empty)
			{
				poolName = group.name.Replace("Pool", string.Empty);
				poolName = poolName.Replace("(Clone)", string.Empty);
			}
			int num = 0;
			for (int i = 0; i < _prefabPoolGroups.Count; i++)
			{
				List<PrefabPool> prefabPools = _prefabPoolGroups[i].prefabPools;
				for (int j = 0; j < prefabPools.Count; j++)
				{
					PrefabPool prefabPool = prefabPools[j];
					if (!(prefabPool.prefab == null))
					{
						prefabPool.inspectorInstanceConstructor();
						CreatePrefabPool(prefabPool);
						if (prefabPool.cullDespawned && prefabPool.cullAbove < prefabPool.preloadAmount)
						{
							prefabPool.preloadAmount = prefabPool.cullAbove;
						}
						num += Mathf.Max(prefabPool.preloadAmount, prefabPools[j].limitAmount);
					}
				}
			}
			_prefabPoolGroups.Add(new PrefabPoolGroup
			{
				name = "Runtime Group"
			});
			PoolManager.Pools.Add(this);
		}

		public int GetBalanceOperations()
		{
			int num = 0;
			for (int i = 0; i < PrefabPoolCount; i++)
			{
				PrefabPool prefabPool = GetPrefabPool(i);
				if (prefabPool.totalCount < prefabPool.preloadAmount)
				{
					num += prefabPool.preloadAmount - prefabPool.totalCount;
				}
				if (prefabPool.cullDespawned && prefabPool.totalCount > prefabPool.cullAbove)
				{
					num += prefabPool.totalCount - prefabPool.cullAbove;
				}
			}
			return num;
		}

		internal GameObject InstantiatePrefab(GameObject prefab, Vector3 pos, Quaternion rot)
		{
			if (instantiateDelegates != null)
			{
				return instantiateDelegates(prefab, pos, rot);
			}
			return InstanceHandler.InstantiatePrefab(prefab, pos, rot);
		}

		internal void DestroyInstance(GameObject instance)
		{
			if (destroyDelegates != null)
			{
				destroyDelegates(instance);
			}
			else
			{
				InstanceHandler.DestroyInstance(instance);
			}
		}

		private void OnDestroy()
		{
			PoolManager.Remove(this);
			StopAllCoroutines();
			foreach (PrefabPool prefabPool in _prefabPools)
			{
				prefabPool.SelfDestruct();
			}
			_prefabPools.Clear();
			prefabs._Clear();
		}

		public void CreatePrefabPool(PrefabPool prefabPool)
		{
			if (PoolManager.PrefabPoolExists(prefabPool.prefab))
			{
				PrefabPool prefabPool2 = PoolManager.GetPrefabPool(prefabPool.prefab);
				throw new Exception(string.Format("Prefab '{0}' is already pooled in  SpawnPool '{1}'.Prefabs cannot be in more than 1 SpawnPool.", prefabPool.prefab, prefabPool2.spawnPool.poolName));
			}
			prefabPool.spawnPool = this;
			_prefabPools.Add(prefabPool);
			prefabs._Add(prefabPool.prefab.name, prefabPool.prefab);
			PoolManager.AddPool(prefabPool);
		}

		public void Add(Transform instance, string prefabName, bool despawn, bool parent)
		{
			for (int i = 0; i < _prefabPools.Count; i++)
			{
				if (_prefabPools[i].prefabGO == null)
				{
					Log.PoolManager.LogErrorFormat(base.gameObject, logLevelAdjust, "Unexpected Error: PrefabPool.prefabGO is null");
					return;
				}
				if (_prefabPools[i].prefabGO.name == prefabName)
				{
					_prefabPools[i].AddUnpooled(instance, despawn);
					if (parent)
					{
						bool worldPositionStays = !(instance is RectTransform);
						instance.SetParent(group, worldPositionStays);
					}
					return;
				}
			}
			Log.PoolManager.LogErrorFormat(base.gameObject, logLevelAdjust, "SpawnPool {0}: PrefabPool {1} not found.", poolName, prefabName);
		}

		public void Add(Transform item)
		{
			string message = "Use SpawnPool.Spawn() to properly add items to the pool.";
			throw new NotImplementedException(message);
		}

		public void Remove(Transform item)
		{
			string message = "Use Despawn() to properly manage items that should remain in the pool but be deactivated.";
			throw new NotImplementedException(message);
		}

		public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			PrefabPool prefabPool = GetPrefabPool(prefab);
			Transform transform = prefabPool.SpawnInstance(pos, rot);
			if (transform == null)
			{
				return null;
			}
			bool worldPositionStays = !(transform is RectTransform);
			if (parent != null)
			{
				transform.SetParent(parent, worldPositionStays);
			}
			else if (!dontReparent && transform.parent != group)
			{
				transform.SetParent(group, worldPositionStays);
			}
			if (sendUnityEvents)
			{
				transform.gameObject.BroadcastMessage("OnSpawned", this, SendMessageOptions.DontRequireReceiver);
			}
			return transform;
		}

		public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
		{
			Transform transform = Spawn(prefab, pos, rot, null);
			if (transform == null)
			{
				return null;
			}
			return transform;
		}

		public Transform Spawn(Transform prefab)
		{
			return Spawn(prefab, Vector3.zero, Quaternion.identity);
		}

		public T Spawn<T>(T prefab) where T : Component
		{
			Transform transform = Spawn(prefab.transform);
			return (!(transform != null)) ? ((T)null) : transform.GetComponent<T>();
		}

		public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component
		{
			Transform transform = Spawn(prefab.transform, position, rotation, parent);
			return (!(transform != null)) ? ((T)null) : transform.GetComponent<T>();
		}

		public T Spawn<T>(T prefab, Transform parent) where T : Component
		{
			Transform transform = Spawn(prefab.transform, parent);
			return (!(transform != null)) ? ((T)null) : transform.GetComponent<T>();
		}

		public Transform Spawn(Transform prefab, Transform parent)
		{
			return Spawn(prefab, Vector3.zero, Quaternion.identity, parent);
		}

		public Transform Spawn(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			return Spawn(prefab.transform, pos, rot, parent);
		}

		public Transform Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
		{
			return Spawn(prefab.transform, pos, rot);
		}

		public Transform Spawn(GameObject prefab)
		{
			return Spawn(prefab.transform);
		}

		public Transform Spawn(GameObject prefab, Transform parent)
		{
			return Spawn(prefab.transform, parent);
		}

		public Transform Spawn(string prefabName)
		{
			Transform prefab = prefabs[prefabName];
			return Spawn(prefab);
		}

		public Transform Spawn(string prefabName, Transform parent)
		{
			Transform prefab = prefabs[prefabName];
			return Spawn(prefab, parent);
		}

		public Transform Spawn(string prefabName, Vector3 pos, Quaternion rot)
		{
			Transform prefab = prefabs[prefabName];
			return Spawn(prefab, pos, rot);
		}

		public Transform Spawn(string prefabName, Vector3 pos, Quaternion rot, Transform parent)
		{
			Transform prefab = prefabs[prefabName];
			return Spawn(prefab, pos, rot, parent);
		}

		public AudioSource Spawn(AudioSource prefab, Vector3 pos, Quaternion rot)
		{
			return Spawn(prefab, pos, rot, null);
		}

		public AudioSource Spawn(AudioSource prefab)
		{
			return Spawn(prefab, Vector3.zero, Quaternion.identity, null);
		}

		public AudioSource Spawn(AudioSource prefab, Transform parent)
		{
			return Spawn(prefab, Vector3.zero, Quaternion.identity, parent);
		}

		public AudioSource Spawn(AudioSource prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			Transform transform = Spawn(prefab.transform, pos, rot, parent);
			if (transform == null)
			{
				return null;
			}
			AudioSource component = transform.GetComponent<AudioSource>();
			component.Play();
			StartCoroutine(ListForAudioStop(component));
			return component;
		}

		public void Despawn(Transform instance)
		{
			PooledInstanceData instanceData = PoolManager.GetInstanceData(instance);
			if (instanceData == null)
			{
				Log.PoolManager.LogErrorFormat(base.gameObject, logLevelAdjust, "SpawnPool {0}: {1} not found in SpawnPool", poolName, instance.name);
			}
			else if (instanceData.inUse)
			{
				instanceData.pool.DespawnInstance(instance);
			}
			else
			{
				Log.PoolManager.LogErrorFormat(base.gameObject, logLevelAdjust, "SpawnPool {0}: {1} has already been despawned. You cannot despawn something more than once!", poolName, instance.name);
			}
		}

		[Obsolete("Despawn(Transform instance, Transform parent), all despawn move the object to the SpawnPool. Use Despawn(Transform instance)!")]
		public void Despawn(Transform instance, Transform parent)
		{
			Despawn(instance);
		}

		public void Despawn(Transform instance, float seconds)
		{
			StartCoroutine(DoDespawnAfterSeconds(instance, seconds));
		}

		[Obsolete("Despawn(Transform instance, float seconds, Transform parent), all despawn move the object to the SpawnPool. Use Despawn(Transform instance, float seconds)!")]
		public void Despawn(Transform instance, float seconds, Transform parent)
		{
			StartCoroutine(DoDespawnAfterSeconds(instance, seconds));
		}

		private IEnumerator DoDespawnAfterSeconds(Transform instance, float seconds)
		{
			GameObject go = instance.gameObject;
			while (seconds > 0f)
			{
				yield return null;
				if (!go.activeInHierarchy)
				{
					yield break;
				}
				seconds -= Time.deltaTime;
			}
			Despawn(instance);
		}

		public void DespawnAll()
		{
			foreach (PrefabPool prefabPool in _prefabPools)
			{
				while (prefabPool._spawned.First != null)
				{
					Despawn(prefabPool._spawned.First.Value.transform);
				}
			}
		}

		public bool IsSpawned(Transform instance)
		{
			PooledInstanceData instanceData = PoolManager.GetInstanceData(instance);
			if (instanceData != null)
			{
				return instanceData.inUse;
			}
			return false;
		}

		public PrefabPool GetPrefabPool(Transform prefab)
		{
			if (PoolManager.PrefabPoolExists(prefab))
			{
				return PoolManager.GetPrefabPool(prefab);
			}
			PrefabPool prefabPool = new PrefabPool(prefab);
			CreatePrefabPool(prefabPool);
			_prefabPoolGroups[_prefabPoolGroups.Count - 1].prefabPools.Add(prefabPool);
			return prefabPool;
		}

		public PrefabPool GetPrefabPool(GameObject prefab)
		{
			return GetPrefabPool(prefab.transform);
		}

		public Transform GetPrefab(Transform instance)
		{
			PooledInstanceData instanceData = PoolManager.GetInstanceData(instance);
			if (instanceData != null)
			{
				return instanceData.pool.prefab;
			}
			return null;
		}

		public GameObject GetPrefab(GameObject instance)
		{
			PooledInstanceData instanceData = PoolManager.GetInstanceData(instance.transform);
			if (instanceData != null)
			{
				return instanceData.pool.prefab.gameObject;
			}
			return null;
		}

		private IEnumerator ListForAudioStop(AudioSource src)
		{
			yield return null;
			GameObject srcGameObject = src.gameObject;
			while (src.isPlaying)
			{
				yield return null;
			}
			if (!srcGameObject.activeInHierarchy)
			{
				src.Stop();
			}
			else
			{
				Despawn(src.transform);
			}
		}

		public override string ToString()
		{
			List<string> list = new List<string>();
			foreach (PrefabPool prefabPool in _prefabPools)
			{
				foreach (PooledInstanceData item in prefabPool._spawned)
				{
					list.Add(item.transform.name);
				}
			}
			return string.Join(", ", list.ToArray());
		}

		public bool Contains(Transform item)
		{
			string message = "Use IsSpawned(Transform instance) instead.";
			throw new NotImplementedException(message);
		}

		public void CopyTo(Transform[] array, int arrayIndex)
		{
			for (int i = 0; i < _prefabPools.Count; i++)
			{
				PrefabPool prefabPool = _prefabPools[i];
				for (LinkedListNode<PooledInstanceData> linkedListNode = prefabPool.spawned.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
				{
					array[arrayIndex++] = linkedListNode.Value.transform;
				}
			}
		}

		public IEnumerator<Transform> GetEnumerator()
		{
			for (int j = 0; j < _prefabPools.Count; j++)
			{
				PrefabPool prefabPool = _prefabPools[j];
				for (LinkedListNode<PooledInstanceData> node = prefabPool.spawned.First; node != null; node = node.Next)
				{
					yield return node.Value.transform;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int j = 0; j < _prefabPools.Count; j++)
			{
				PrefabPool prefabPool = _prefabPools[j];
				for (LinkedListNode<PooledInstanceData> node = prefabPool.spawned.First; node != null; node = node.Next)
				{
					yield return node.Value;
				}
			}
		}

		public int IndexOf(Transform item)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, Transform item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public PrefabPool GetPrefabPool(int idx)
		{
			return _prefabPools[idx];
		}

		bool ICollection<Transform>.Remove(Transform item)
		{
			throw new NotImplementedException();
		}
	}
}
