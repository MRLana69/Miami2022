using SYBO.Subway.Placeholders.Lifecycles;
using SYBO.Subway.Placeholders.PrefabProviders;
using UnityEngine;

namespace SYBO.Subway.Placeholders
{
	public class SpawnOnceOnStartPlaceholderLifecycle : PlaceholderLifecycle
	{
		protected void Start()
		{
			if (Application.isPlaying)
			{
				SpawnInstances(true);
			}
		}

		private void SpawnInstances(bool spawnInChildrenLifecycles = false)
		{
			_prefabProvider.Evaluate();
			SpawnInstances(_prefabProvider, spawnInChildrenLifecycles);
		}

		protected virtual void SpawnInstances(PlaceholderPrefabProviderBase prefabProvider, bool spawnInChildrenLifecycles = false)
		{
			if (_placeholder.HasSpawnedInstances)
			{
				return;
			}
			_placeholder.SpawnInstances(prefabProvider);
			if (!spawnInChildrenLifecycles)
			{
				return;
			}
			SpawnOnceOnStartPlaceholderLifecycle[] componentsInChildren = GetComponentsInChildren<SpawnOnceOnStartPlaceholderLifecycle>(true);
			SpawnOnceOnStartPlaceholderLifecycle[] array = componentsInChildren;
			foreach (SpawnOnceOnStartPlaceholderLifecycle spawnOnceOnStartPlaceholderLifecycle in array)
			{
				if (!(spawnOnceOnStartPlaceholderLifecycle == this))
				{
					spawnOnceOnStartPlaceholderLifecycle.SpawnInstances(true);
				}
			}
		}
	}
}
