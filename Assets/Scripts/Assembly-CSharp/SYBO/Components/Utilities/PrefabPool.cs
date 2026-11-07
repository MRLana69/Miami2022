using System;
using System.Collections.Generic;
using SYBO.Components.Logging;
using UnityEngine;

namespace SYBO.Components.Utilities
{
	[Serializable]
	public class PrefabPool
	{
		private class Comparer : IComparer<PrefabPool>
		{
			int IComparer<PrefabPool>.Compare(PrefabPool x, PrefabPool y)
			{
				return string.Compare(x.prefab.name, y.prefab.name);
			}
		}

		public Transform prefab;

		internal GameObject prefabGO;

		public int preloadAmount = 1;

		public bool limitInstances;

		public int limitAmount = 100;

		public bool limitFIFO;

		public bool cullDespawned;

		public int cullAbove = 50;

		public SpawnPool spawnPool;

		[NonSerialized]
		public int maxActive;

		[NonSerialized]
		public int loadedOnDemand;

		private bool initialized;

		internal LinkedList<PooledInstanceData> _spawned = new LinkedList<PooledInstanceData>();

		internal LinkedList<PooledInstanceData> _despawned = new LinkedList<PooledInstanceData>();

		private static IComparer<PrefabPool> _comparer;

		public LinkedList<PooledInstanceData> spawned
		{
			get
			{
				return new LinkedList<PooledInstanceData>(_spawned);
			}
		}

		public LinkedList<PooledInstanceData> despawned
		{
			get
			{
				return new LinkedList<PooledInstanceData>(_despawned);
			}
		}

		public int totalCount
		{
			get
			{
				int num = 0;
				num += _spawned.Count;
				return num + _despawned.Count;
			}
		}

		public static IComparer<PrefabPool> comparer
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

		public PrefabPool(Transform prefab)
		{
			this.prefab = prefab;
			prefabGO = prefab.gameObject;
		}

		public PrefabPool()
		{
		}

		internal void inspectorInstanceConstructor()
		{
			prefabGO = prefab.gameObject;
			_spawned = new LinkedList<PooledInstanceData>();
			_despawned = new LinkedList<PooledInstanceData>();
		}

		internal void SelfDestruct()
		{
			string text = "Unknown";
			if (prefabGO != null)
			{
				text = prefabGO.name;
			}
			for (LinkedListNode<PooledInstanceData> linkedListNode = _despawned.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value.transform != null)
				{
					PoolManager.UnregisterInstance(linkedListNode.Value);
					spawnPool.DestroyInstance(linkedListNode.Value.transform.gameObject);
				}
			}
			for (LinkedListNode<PooledInstanceData> linkedListNode = _spawned.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value.transform != null)
				{
					PoolManager.UnregisterInstance(linkedListNode.Value);
					spawnPool.DestroyInstance(linkedListNode.Value.transform.gameObject);
				}
			}
			_spawned.Clear();
			_despawned.Clear();
			prefab = null;
			prefabGO = null;
			spawnPool = null;
		}

		internal bool DespawnInstance(Transform xform)
		{
			return DespawnInstance(xform, true);
		}

		internal bool DespawnInstance(Transform xform, bool sendEventMessage)
		{
			PooledInstanceData instanceData = PoolManager.GetInstanceData(xform);
			if (instanceData == null)
			{
				throw new NotImplementedException();
			}
			if (!instanceData.inUse)
			{
				throw new NotImplementedException();
			}
			instanceData.inUse = false;
			_spawned.Remove(instanceData.node);
			_despawned.AddLast(instanceData.node);
			if (sendEventMessage && spawnPool.sendUnityEvents)
			{
				xform.gameObject.BroadcastMessage("OnDespawned", spawnPool, SendMessageOptions.DontRequireReceiver);
			}
			xform.gameObject.SetActive(false);
			bool worldPositionStays = !(xform is RectTransform);
			xform.SetParent(spawnPool.group, worldPositionStays);
			if (cullDespawned && totalCount > cullAbove)
			{
				PoolManager.TriggerPoolBalancing(spawnPool);
			}
			return true;
		}

		internal Transform SpawnInstance(Vector3 pos, Quaternion rot)
		{
			if (limitInstances && limitFIFO && _spawned.Count >= limitAmount)
			{
				LinkedListNode<PooledInstanceData> first = _spawned.First;
				Transform transform = first.Value.transform;
				DespawnInstance(transform);
			}
			Transform transform2;
			if (_despawned.Count == 0)
			{
				loadedOnDemand++;
				transform2 = SpawnNew(pos, rot);
			}
			else
			{
				LinkedListNode<PooledInstanceData> last = _despawned.Last;
				transform2 = last.Value.transform;
				last.Value.inUse = true;
				_despawned.Remove(last);
				_spawned.AddLast(last);
				if (transform2 == null)
				{
					string message = "Make sure you didn't delete a despawned instance directly.";
					throw new MissingReferenceException(message);
				}
				transform2.position = pos;
				transform2.rotation = rot;
				transform2.gameObject.SetActive(true);
			}
			if (_spawned.Count > maxActive)
			{
				maxActive = _spawned.Count;
			}
			return transform2;
		}

		public Transform SpawnNew()
		{
			return SpawnNew(Vector3.zero, Quaternion.identity);
		}

		public Transform SpawnNew(Vector3 pos, Quaternion rot)
		{
			if (!initialized)
			{
				initialized = true;
				IPrefabBehaviour component = prefabGO.GetComponent<IPrefabBehaviour>();
				if (component != null)
				{
					component.Initialize();
				}
			}
			if (limitInstances && totalCount >= limitAmount)
			{
				return null;
			}
			if (pos == Vector3.zero)
			{
				pos = spawnPool.group.position;
			}
			if (rot == Quaternion.identity)
			{
				rot = spawnPool.group.rotation;
			}
			GameObject gameObject = spawnPool.InstantiatePrefab(prefabGO, pos, rot);
			Transform transform = gameObject.transform;
			PooledInstanceData pooledInstanceData = new PooledInstanceData(transform, this);
			pooledInstanceData.inUse = true;
			PoolManager.RegisterInstance(pooledInstanceData);
			nameInstance(transform);
			if (!spawnPool.dontReparent)
			{
				bool worldPositionStays = !(transform is RectTransform);
				transform.SetParent(spawnPool.group, worldPositionStays);
			}
			_spawned.AddLast(pooledInstanceData.node);
			return transform;
		}

		private void SetRecursively(Transform xform, int layer)
		{
			xform.gameObject.layer = layer;
			foreach (Transform item in xform)
			{
				SetRecursively(item, layer);
			}
		}

		internal void AddUnpooled(Transform inst, bool despawn)
		{
			nameInstance(inst);
			PooledInstanceData pooledInstanceData = new PooledInstanceData(inst, this);
			pooledInstanceData.inUse = !despawn;
			if (despawn)
			{
				inst.gameObject.SetActive(false);
				_despawned.AddLast(pooledInstanceData.node);
			}
			else
			{
				_spawned.AddLast(pooledInstanceData.node);
			}
			PoolManager.RegisterInstance(pooledInstanceData);
		}

		public bool Contains(Transform transform)
		{
			if (prefabGO == null)
			{
				Log.PoolManager.LogErrorFormat(spawnPool.gameObject, spawnPool.logLevelAdjust, "SpawnPool {0}: PrefabPool.prefabGO is null", spawnPool.poolName);
			}
			PooledInstanceData instanceData = PoolManager.GetInstanceData(transform);
			return instanceData != null && instanceData.pool == this;
		}

		private void nameInstance(Transform instance)
		{
			instance.name += (totalCount + 1).ToString("#000");
		}
	}
}
