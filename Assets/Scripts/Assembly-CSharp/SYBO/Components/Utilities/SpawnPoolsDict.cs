using System;
using System.Collections;
using System.Collections.Generic;
using SYBO.Components.Logging;
using UnityEngine;

namespace SYBO.Components.Utilities
{
	public class SpawnPoolsDict : IDictionary<string, SpawnPool>, IEnumerable, ICollection<KeyValuePair<string, SpawnPool>>, IEnumerable<KeyValuePair<string, SpawnPool>>
	{
		private Dictionary<string, SpawnPool> _pools = new Dictionary<string, SpawnPool>();

		bool ICollection<KeyValuePair<string, SpawnPool>>.IsReadOnly
		{
			get
			{
				return System_002ECollections_002EGeneric_002EICollection_003CSystem_002ECollections_002EGeneric_002EKeyValuePair_003Cstring_002CSYBO_002EComponents_002EUtilities_002ESpawnPool_003E_003E_002Eget_IsReadOnly();
			}
		}

		public int Count
		{
			get
			{
				return _pools.Count;
			}
		}

		public SpawnPool this[string key]
		{
			get
			{
				try
				{
					return _pools[key];
				}
				catch (KeyNotFoundException)
				{
					string message = string.Format("A Pool with the name '{0}' not found. \nPools={1}", key, ToString());
					throw new KeyNotFoundException(message);
				}
			}
			set
			{
				string message = "Cannot set PoolManager.Pools[key] directly. SpawnPools add themselves to PoolManager.Pools when created, so there is no need to set them explicitly. Create pools using PoolManager.Pools.Create() or add a SpawnPool component to a GameObject.";
				throw new NotImplementedException(message);
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				string message = "If you need this, please request it.";
				throw new NotImplementedException(message);
			}
		}

		public ICollection<SpawnPool> Values
		{
			get
			{
				string message = "If you need this, please request it.";
				throw new NotImplementedException(message);
			}
		}

		private bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public SpawnPool Create(string poolName)
		{
			if (!assertValidPoolName(poolName))
			{
				return null;
			}
			GameObject gameObject = new GameObject(poolName + "Pool");
			return gameObject.AddComponent<SpawnPool>();
		}

		public SpawnPool Create(string poolName, GameObject owner)
		{
			if (!assertValidPoolName(poolName))
			{
				return null;
			}
			string name = owner.gameObject.name;
			try
			{
				owner.gameObject.name = poolName;
				return owner.AddComponent<SpawnPool>();
			}
			finally
			{
				owner.gameObject.name = name;
			}
		}

		private bool assertValidPoolName(string poolName)
		{
			string text = poolName.Replace("Pool", string.Empty);
			if (text != poolName)
			{
				poolName = text;
			}
			if (ContainsKey(poolName))
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			string[] array = new string[_pools.Count];
			_pools.Keys.CopyTo(array, 0);
			return string.Format("[{0}]", string.Join(", ", array));
		}

		public bool Destroy(string poolName)
		{
			SpawnPool value;
			if (!_pools.TryGetValue(poolName, out value))
			{
				Log.PoolManager.LogErrorFormat("PoolManager: Unable to destroy '{0}'. Not in PoolManager", poolName);
				return false;
			}
			UnityEngine.Object.Destroy(value.gameObject);
			return true;
		}

		public void DestroyAll()
		{
			foreach (KeyValuePair<string, SpawnPool> pool in _pools)
			{
				UnityEngine.Object.Destroy(pool.Value);
			}
			_pools.Clear();
		}

		internal void Add(SpawnPool spawnPool)
		{
			if (ContainsKey(spawnPool.poolName))
			{
				Log.PoolManager.LogErrorFormat(spawnPool.gameObject, "A pool with the name '{0}' already exists. This should only happen if a SpawnPool with this name is added to a scene twice.", spawnPool.poolName);
				return;
			}
			_pools.Add(spawnPool.poolName, spawnPool);
			PoolManager.TriggerPoolBalancing(spawnPool);
		}

		public void Add(string key, SpawnPool value)
		{
			string message = "SpawnPools add themselves to PoolManager.Pools when created, so there is no need to Add() them explicitly. Create pools using PoolManager.Pools.Create() or add a SpawnPool component to a GameObject.";
			throw new NotImplementedException(message);
		}

		internal bool Remove(SpawnPool spawnPool)
		{
			if (!ContainsKey(spawnPool.poolName) & Application.isPlaying)
			{
				Log.PoolManager.LogErrorFormat("PoolManager: Unable to remove '{0}'. Pool not in PoolManager", spawnPool.poolName);
				return false;
			}
			_pools.Remove(spawnPool.poolName);
			return true;
		}

		public bool Remove(string poolName)
		{
			string message = "SpawnPools can only be destroyed, not removed and kept alive outside of PoolManager. There are only 2 legal ways to destroy a SpawnPool: Destroy the GameObject directly, if you have a reference, or use PoolManager.Destroy(string poolName).";
			throw new NotImplementedException(message);
		}

		public bool ContainsKey(string poolName)
		{
			return _pools.ContainsKey(poolName);
		}

		public bool TryGetValue(string poolName, out SpawnPool spawnPool)
		{
			return _pools.TryGetValue(poolName, out spawnPool);
		}

		public bool Contains(KeyValuePair<string, SpawnPool> item)
		{
			string message = "Use PoolManager.Pools.Contains(string poolName) instead.";
			throw new NotImplementedException(message);
		}

		private bool System_002ECollections_002EGeneric_002EICollection_003CSystem_002ECollections_002EGeneric_002EKeyValuePair_003Cstring_002CSYBO_002EComponents_002EUtilities_002ESpawnPool_003E_003E_002Eget_IsReadOnly()
		{
			return true;
		}

		public void Add(KeyValuePair<string, SpawnPool> item)
		{
			string message = "SpawnPools add themselves to PoolManager.Pools when created, so there is no need to Add() them explicitly. Create pools using PoolManager.Pools.Create() or add a SpawnPool component to a GameObject.";
			throw new NotImplementedException(message);
		}

		public void Clear()
		{
			string message = "Use PoolManager.Pools.DestroyAll() instead.";
			throw new NotImplementedException(message);
		}

		private void CopyTo(KeyValuePair<string, SpawnPool>[] array, int arrayIndex)
		{
			string message = "PoolManager.Pools cannot be copied";
			throw new NotImplementedException(message);
		}

		void ICollection<KeyValuePair<string, SpawnPool>>.CopyTo(KeyValuePair<string, SpawnPool>[] array, int arrayIndex)
		{
			string message = "PoolManager.Pools cannot be copied";
			throw new NotImplementedException(message);
		}

		public bool Remove(KeyValuePair<string, SpawnPool> item)
		{
			string message = "SpawnPools can only be destroyed, not removed and kept alive outside of PoolManager. There are only 2 legal ways to destroy a SpawnPool: Destroy the GameObject directly, if you have a reference, or use PoolManager.Destroy(string poolName).";
			throw new NotImplementedException(message);
		}

		public IEnumerator<KeyValuePair<string, SpawnPool>> GetEnumerator()
		{
			return _pools.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _pools.GetEnumerator();
		}
	}
}
