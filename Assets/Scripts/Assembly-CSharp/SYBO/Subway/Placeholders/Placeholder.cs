using System.Collections.Generic;
using SYBO.Common.Utilities;
using SYBO.Components.Utilities;
using SYBO.Subway.Level;
using SYBO.Subway.Placeholders.Lifecycles;
using SYBO.Subway.Placeholders.PrefabProviders;
using SYBO.Subway.Utilities;
using UnityEngine;

namespace SYBO.Subway.Placeholders
{
	[DisallowMultipleComponent]
	public class Placeholder : MonoBehaviour
	{
		protected Transform _transform;

		private bool _initialized;

		private List<TrackObject> _childTrackObjects = new List<TrackObject>();

		private IPlaceholderInitializer[] _initializers;

		public bool HasSpawnedInstances
		{
			get
			{
				return _transform != null && _transform.childCount != 0;
			}
		}

		protected virtual void Initialize()
		{
			_transform = base.transform;
			_initializers = GetComponents<IPlaceholderInitializer>();
			_initialized = true;
		}

		protected void InitializeCheck()
		{
			if (!_initialized)
			{
				Initialize();
			}
		}

		protected virtual void OnValidate()
		{
			PlaceholderLifecycle component = base.gameObject.GetComponent<PlaceholderLifecycle>();
			if (component == null)
			{
				SDebug.LogError("No lifecycle2 for placeholder " + this.GetLongName(), this);
			}
		}

		public void GetSpawnedObjects(List<Selector> selectors)
		{
			InitializeCheck();
			bool flag = false;
			for (int i = 0; i < base.transform.childCount; i++)
			{
				PlaceholderPrefab component = base.transform.GetChild(i).GetComponent<PlaceholderPrefab>();
				if (component != null)
				{
					flag = true;
					selectors.AddRange(component.Selectors);
				}
			}
			if (!flag)
			{
				this.GetComponentsInChildrenBFS(true, selectors, true);
			}
		}

		internal virtual GameObject SpawnInstance(Transform prefab)
		{
			if (prefab == null)
			{
				return null;
			}
			Transform transform = null;
			if (Application.isPlaying)
			{
				transform = PoolManager.Spawn(prefab);
				if (transform == null)
				{
					SDebug.LogError("Placeholder on [" + base.name + "] failed to get object");
					return null;
				}
			}
			else if (Application.isEditor)
			{
				GameObject gameObject = Object.Instantiate(prefab.gameObject);
				transform = gameObject.transform;
				Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(true);
				Transform[] array = componentsInChildren;
				foreach (Transform transform2 in array)
				{
					transform2.gameObject.hideFlags = HideFlags.DontSave;
				}
			}
			InitializeSpawnedInstance(transform, prefab.name);
			if (!transform.gameObject.activeSelf)
			{
			}
			transform.gameObject.SetActive(true);
			GetTrackObjects_Cached(transform);
			return transform.gameObject;
		}

		protected virtual Transform InitializeSpawnedInstance(Transform objectTransform, string newName)
		{
			objectTransform.name = newName;
			Vector3 localScale = objectTransform.localScale;
			objectTransform.parent = _transform;
			objectTransform.localScale = localScale;
			objectTransform.localPosition = Vector3.zero;
			objectTransform.localRotation = Quaternion.identity;
			IPlaceholderInitializer[] initializers = _initializers;
			foreach (IPlaceholderInitializer placeholderInitializer in initializers)
			{
				placeholderInitializer.Initialize(this, objectTransform);
			}
			return objectTransform;
		}

		protected void GetTrackObjects_Cached(Transform t)
		{
			PlaceholderPrefab component = t.GetComponent<PlaceholderPrefab>();
			if (component != null)
			{
				_childTrackObjects.AddRange(component.TrackObjects);
			}
			else
			{
				GetTrackObjects(t);
			}
		}

		protected void GetTrackObjects(Transform t, bool addRootTrackObject = true)
		{
			if (addRootTrackObject)
			{
				TrackObject component = t.GetComponent<TrackObject>();
				if (component != null)
				{
					_childTrackObjects.Add(component);
				}
			}
			for (int i = 0; i < t.childCount; i++)
			{
				Transform child = t.GetChild(i);
				TrackObject component2 = child.GetComponent<TrackObject>();
				if (component2 != null)
				{
					_childTrackObjects.Add(component2);
				}
				if (child.GetComponent<Placeholder>() == null)
				{
					GetTrackObjects(child, false);
				}
			}
		}

		internal virtual void SpawnInstances(PlaceholderPrefabProviderBase prefabProvider)
		{
			InitializeCheck();
			if (HasSpawnedInstances)
			{
				if (Application.isEditor)
				{
					for (int i = 0; i < base.transform.childCount; i++)
					{
						Transform child = base.transform.GetChild(i);
					}
				}
				return;
			}
			List<Transform> prefabsToSpawn = prefabProvider.PrefabsToSpawn;
			for (int j = 0; j < prefabsToSpawn.Count; j++)
			{
				GameObject gameObject = SpawnInstance(prefabsToSpawn[j]);
				if (Application.isEditor && !Application.isPlaying)
				{
					PlaceholderLifecycle[] componentsInChildren = gameObject.GetComponentsInChildren<PlaceholderLifecycle>();
					PlaceholderLifecycle[] array = componentsInChildren;
					foreach (PlaceholderLifecycle placeholderLifecycle in array)
					{
					}
				}
			}
		}

		internal void DespawnInstances()
		{
			InitializeCheck();
			if (Application.isPlaying)
			{
				int num = _transform.childCount - 1;
				while (0 <= num)
				{
					Transform child = _transform.GetChild(num);
					Vector3 localScale = child.localScale;
					PoolManager.Despawn(child);
					child.localScale = localScale;
					num--;
				}
			}
			else if (Application.isEditor)
			{
				while (_transform.childCount != 0)
				{
					Object.DestroyImmediate(_transform.GetChild(0).gameObject);
				}
			}
			_childTrackObjects.Clear();
		}

		internal void Activate()
		{
			int num = 0;
			while (num < _childTrackObjects.Count)
			{
				TrackObject trackObject = _childTrackObjects[num];
				if (trackObject != null)
				{
					num++;
					if (trackObject.gameObject.activeInHierarchy)
					{
						trackObject.Activate();
					}
				}
				else
				{
					if (Application.isEditor)
					{
					}
					_childTrackObjects.RemoveAt(num);
				}
			}
		}

		internal void Deactivate()
		{
			for (int i = 0; i < _childTrackObjects.Count; i++)
			{
				TrackObject trackObject = _childTrackObjects[i];
				if (trackObject.Active)
				{
					trackObject.Deactivate();
				}
			}
		}
	}
}
