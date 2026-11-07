using SYBO.Common.Utilities;
using UnityEngine;

namespace SYBO.Common.Components
{
	public class InstantiatePrefabComponent : MonoBehaviour
	{
		[SerializeField]
		private GameObject _parent;

		[SerializeField]
		private GameObject _prefab;

		[SerializeField]
		private Vector3 _prefabLocalPosition;

		[SerializeField]
		private bool _willInstantiateOnAwake = true;

		private GameObject _prefabInstance;

		protected void Awake()
		{
			if (_willInstantiateOnAwake)
			{
				InstantiatePrefab();
			}
		}

		public virtual GameObject InstantiatePrefab()
		{
			if (_prefab != null)
			{
				_prefabInstance = Object.Instantiate(_prefab);
				HierarchyUtility.Reparent(_prefabInstance.transform, _parent.transform);
				_prefabInstance.transform.localPosition = _prefabLocalPosition;
			}
			return _prefabInstance;
		}
	}
}
