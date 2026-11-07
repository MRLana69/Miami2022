using System.Diagnostics;
using SYBO.Subway.Placeholders.PrefabProviders;
using UnityEngine;

namespace SYBO.Subway.Placeholders.Lifecycles
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Placeholder))]
	[RequireComponent(typeof(PlaceholderPrefabProviderBase))]
	public class PlaceholderLifecycle : MonoBehaviour
	{
		protected bool _initialized;

		protected PlaceholderPrefabProviderBase _prefabProvider;

		protected Placeholder _placeholder;

		public Placeholder Placeholder
		{
			get
			{
				InitializeCheck();
				return _placeholder;
			}
		}

		protected void Awake()
		{
			InitializeCheck();
		}

		protected virtual void InitializeCheck()
		{
			if (!_initialized)
			{
				Initialize();
				_initialized = true;
			}
		}

		protected virtual void Initialize()
		{
			_prefabProvider = GetComponent(typeof(PlaceholderPrefabProviderBase)) as PlaceholderPrefabProviderBase;
			_placeholder = GetComponent(typeof(Placeholder)) as Placeholder;
			if (_placeholder.HasSpawnedInstances)
			{
				_placeholder.DespawnInstances();
			}
		}

		[Conditional("UNITY_EDITOR")]
		public void SpawnInstancesEditor()
		{
			InitializeCheck();
			_prefabProvider.Evaluate();
			if (_placeholder.HasSpawnedInstances)
			{
				return;
			}
			_placeholder.SpawnInstances(_prefabProvider);
			PlaceholderLifecycle[] componentsInChildren = GetComponentsInChildren<PlaceholderLifecycle>(true);
			PlaceholderLifecycle[] array = componentsInChildren;
			foreach (PlaceholderLifecycle placeholderLifecycle in array)
			{
				if (placeholderLifecycle == this)
				{
				}
			}
		}

		[Conditional("UNITY_EDITOR")]
		public void DespawnInstancesEditor()
		{
			InitializeCheck();
			_placeholder.DespawnInstances();
		}

		public void Spawn()
		{
			InitializeCheck();
			_prefabProvider.Evaluate();
			_placeholder.SpawnInstances(_prefabProvider);
		}

		public void Despawn()
		{
			InitializeCheck();
			DespawnInstances();
		}

		protected virtual void DespawnInstances()
		{
			if (_placeholder.HasSpawnedInstances)
			{
				_placeholder.DespawnInstances();
			}
		}

		protected void Activate()
		{
			_placeholder.Activate();
		}

		protected void Deactivate()
		{
			_placeholder.Deactivate();
		}

		private void OnValidate()
		{
			PlaceholderLifecycle[] components = GetComponents<PlaceholderLifecycle>();
			if (1 < components.Length)
			{
				SDebug.LogError("'" + base.name + "' has two PlaceholderLifecycle components at " + Utils.GetLongName(base.transform));
			}
		}
	}
}
