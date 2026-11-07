using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Components.Utilities
{
	public class PooledInstanceData
	{
		public bool inUse;

		public LinkedListNode<PooledInstanceData> node { get; private set; }

		public int instanceId { get; private set; }

		public Transform transform { get; private set; }

		public PrefabPool pool { get; private set; }

		public PooledInstanceData(Transform transform, PrefabPool pool)
		{
			this.pool = pool;
			this.transform = transform;
			node = new LinkedListNode<PooledInstanceData>(this);
			instanceId = transform.GetInstanceID();
		}
	}
}
