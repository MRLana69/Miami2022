using System.Collections.Generic;
using SYBO.Subway.Themes;
using UnityEngine;

namespace SYBO.Subway.Placeholders.PrefabProviders
{
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	public abstract class PlaceholderPrefabProviderBase : MonoBehaviour
	{
		protected bool _initialized;

		private ThemeManager _themeManager;

		protected List<Transform> _prefabsToSpawn = new List<Transform>(1);

		internal List<Transform> PrefabsToSpawn
		{
			get
			{
				return _prefabsToSpawn;
			}
		}

		internal abstract int PrefabCount { get; }

		private void Awake()
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
			_themeManager = ThemeManager.Instance;
		}

		internal abstract void Evaluate();

		internal abstract Transform GetPrefab(int prefabIndex);

		internal Transform GetThemedPrefab(Transform prefabTransform)
		{
			InitializeCheck();
			GameObject themePrefab;
			_themeManager.Theme.TryGetThemePrefabFromGeneric(prefabTransform.gameObject, out themePrefab);
			return (!(themePrefab != null)) ? prefabTransform : themePrefab.transform;
		}
	}
}
