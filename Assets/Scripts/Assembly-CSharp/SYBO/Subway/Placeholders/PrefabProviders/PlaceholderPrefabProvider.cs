using UnityEngine;

namespace SYBO.Subway.Placeholders.PrefabProviders
{
	public class PlaceholderPrefabProvider : PlaceholderPrefabProviderBase
	{
		[SerializeField]
		[Tooltip("Will spawn this prefab when track object activates. If prefab is used in Theme as generic and mapped to theme specific one, then theme specific will be spawned instead")]
		protected Transform _prefab;

		[SerializeField]
		public DeviceInfo.PerformanceLevel MinDevicePerformance;

		protected bool evaluated;

		public Transform Prefab
		{
			get
			{
				return _prefab;
			}
			set
			{
				_prefab = value;
			}
		}

		internal override int PrefabCount
		{
			get
			{
				return 1;
			}
		}

		internal override void Evaluate()
		{
			InitializeCheck();
			if (!evaluated)
			{
				if (DeviceInfo.Instance.IsDeviceCapablePerformance(MinDevicePerformance))
				{
					_prefabsToSpawn.Add(GetThemedPrefab(_prefab));
				}
				evaluated = true;
			}
		}

		internal override Transform GetPrefab(int prefabIndex)
		{
			return _prefab;
		}
	}
}
