using System;
using System.Collections.Generic;
using SYBO.Components.Utilities;
using SYBO.Subway.Level;
using SYBO.Subway.Level.Environments;
using SYBO.Subway.Utilities;
using SYBO.Subway.Visuals;
using UnityEngine;
using UnityEngine.Serialization;

namespace SYBO.Subway.Themes
{
	public class Theme : ScriptableObject
	{
		[Serializable]
		private class ThemePreloadSetting
		{
			[HideInInspector]
			[SerializeField]
			private string _prefabName;

			[SerializeField]
			private Transform _prefab;

			[SerializeField]
			private int _preloadAmountLowPerf;

			[SerializeField]
			private int _preloadAmountMediumPerf;

			[SerializeField]
			private int _preloadAmountHighPerf;

			public Transform Prefab
			{
				get
				{
					return _prefab;
				}
			}

			public ThemePreloadSetting(Transform prefab)
			{
				_prefab = prefab;
				_prefabName = prefab.name;
			}

			public void SetPreloadAmount(DeviceInfo.PerformanceLevel perfLevel, int amount)
			{
				Debug.Log(string.Concat(perfLevel, " : ", amount, " : ", _prefabName));
				switch (perfLevel)
				{
				case DeviceInfo.PerformanceLevel.Low:
					_preloadAmountLowPerf = amount;
					break;
				case DeviceInfo.PerformanceLevel.Medium:
					_preloadAmountMediumPerf = amount;
					break;
				case DeviceInfo.PerformanceLevel.High:
					_preloadAmountHighPerf = amount;
					break;
				default:
					SDebug.LogErrorFormat("Unhandled performance level in preload settings");
					break;
				}
			}

			public int GetPreloadAmount(DeviceInfo.PerformanceLevel perfLevel)
			{
				switch (perfLevel)
				{
				case DeviceInfo.PerformanceLevel.Low:
					return _preloadAmountLowPerf;
				case DeviceInfo.PerformanceLevel.Medium:
					return _preloadAmountMediumPerf;
				case DeviceInfo.PerformanceLevel.High:
					return _preloadAmountHighPerf;
				default:
					SDebug.LogErrorFormat("Unhandled performance level in preload settings");
					return 0;
				}
			}
		}

		public const string ThemesPath = "Assets/Themes";

		private const float MinFogTransitionDistance = 1f;

		public const string GenericAssetDirectoryName = "0.00_Generic";

		[Header("Background")]
		[SerializeField]
		private Background _background = new Background();

		[Header("Distant Environment")]
		[SerializeField]
		private DistantEnvironmentController.DistantObjectGlobalConfig _distantObjectGlobalConfig = new DistantEnvironmentController.DistantObjectGlobalConfig();

		[SerializeField]
		[FormerlySerializedAs("_distantEnvironmentGroups")]
		private DistantEnvironmentController.DistantObjectConfig[] _distantObjectConfigs = new DistantEnvironmentController.DistantObjectConfig[0];

		[Header("Environment Transitions")]
		[SerializeField]
		private TransitionConfig[] _environmentTransitionConfigs = new TransitionConfig[0];

		[Header("Visuals")]
		[SerializeField]
		private Color _fogColor;

		[Range(0f, 850f)]
		[SerializeField]
		private float _fogStartDistance = 700f;

		[Range(1f, 850f)]
		[SerializeField]
		private float _fogEndDistance = 300f;

		[Range(850f, 2000f)]
		[SerializeField]
		private float _backgroundStartDistance = 1000f;

		[Range(-5000f, 5000f)]
		[SerializeField]
		private float _distantFogStartDistance = 850f;

		[Range(0f, 5000f)]
		[SerializeField]
		private float _distantFogEndDistance = 850f;

		[Range(850f, 7000f)]
		[SerializeField]
		private float _distantBackgroundStartDistance = 1500f;

		[SerializeField]
		private Color _glowGoldColor;

		[SerializeField]
		private Material _glowGold;

		[SerializeField]
		private float _glowGoldFalloff = 200f;

		[Header("Effects")]
		[FormerlySerializedAs("_themeEffects")]
		[SerializeField]
		private ThemeEffect[] _effects = new ThemeEffect[0];

		[SerializeField]
		private bool _roofedCullingEnabled;

		[SerializeField]
		private EnvironmentType[] _roofedCullingEnvironmentsHigh = new EnvironmentType[0];

		[SerializeField]
		private EnvironmentType[] _roofedCullingEnvironmentsLow = new EnvironmentType[0];

		[SerializeField]
		private RoofedCulling.CustomRoofedCulling[] _roofedCullingEnvironmentsCustom = new RoofedCulling.CustomRoofedCulling[0];

		[SerializeField]
		private Color _rollImpactRockColor = Color.white;

		[SerializeField]
		private Color _rollImpactDustColorMin = Color.white;

		[SerializeField]
		private Color _rollImpactDustColorMax = Color.white;

		[Header("Themed assets")]
		[SerializeField]
		private MaterialGroup[] _devicePerformanceRelativeMaterials;

		[SerializeField]
		private GenericAndThemePrefabGroup[] _prefabMappingsGeneric;

		[SerializeField]
		private RouteEnvironmentModelAssets[] _routeEnvironmentModelAssets;

		public const string FieldPatternGroundAsset = "_groundAssets.";

		[SerializeField]
		private GroundAssetGroup _groundAssets;

		[SerializeField]
		private List<EnvironmentKind> _baseIgnoredEnvironmentKinds = new List<EnvironmentKind>();

		[SerializeField]
		private AudioClip _gameMusic;

		[Header("Preload settings")]
		[SerializeField]
		private List<ThemePreloadSetting> _themePreloadSettings = new List<ThemePreloadSetting>();

		[NonSerialized]
		private bool _initialized;

		private Dictionary<GameObject, GameObject> _genericToThemePrefabDict;

		private Dictionary<Material, Material> _devicePerformanceRelativeMaterialsDict;

		public Shader BackgroundLayerCommonMaterialShader
		{
			get
			{
				return _background.LayerCommonMaterialShader;
			}
		}

		public float BackgroundLayerSpacing
		{
			get
			{
				return _background.LayerSpacing;
			}
		}

		public float BackgroundSkylineLeftLimit
		{
			get
			{
				return _background.SkylineLeftLimit;
			}
		}

		public float BackgroundSkylineRightLimit
		{
			get
			{
				return _background.SkylineRightLimit;
			}
		}

		public BackgroundLayer[] BackgroundLayers
		{
			get
			{
				return _background.Layers;
			}
		}

		public BackgroundCulling[] BackgroundCulling
		{
			get
			{
				return _background.Culling;
			}
		}

		public Color BackgroundCullingColor
		{
			get
			{
				return _background.BackgroundCullingColor;
			}
		}

		public DistantEnvironmentController.DistantObjectGlobalConfig DistantObjectGlobalConfig
		{
			get
			{
				return _distantObjectGlobalConfig;
			}
		}

		public DistantEnvironmentController.DistantObjectConfig[] DistantObjectConfigs
		{
			get
			{
				return _distantObjectConfigs;
			}
		}

		public Color FogColor
		{
			get
			{
				return _fogColor;
			}
		}

		public Color BackgroundGradientTopColor
		{
			get
			{
				return _background.GradientTopColor;
			}
		}

		public Color BackgroundGradientBottomColor
		{
			get
			{
				return _background.GradientBottomColor;
			}
		}

		public float FogStartDistance
		{
			get
			{
				return _fogStartDistance;
			}
		}

		public float FogEndDistance
		{
			get
			{
				return _fogEndDistance;
			}
		}

		public float BackgroundStartDistance
		{
			get
			{
				return _backgroundStartDistance;
			}
		}

		public float DistantFogStartDistance
		{
			get
			{
				return _distantFogStartDistance;
			}
		}

		public float DistantFogEndDistance
		{
			get
			{
				return _distantFogEndDistance;
			}
		}

		public float DistantBackgroundStartDistance
		{
			get
			{
				return _distantBackgroundStartDistance;
			}
		}

		public float BackgroundGradientBottom
		{
			get
			{
				return _background.GradientBottom;
			}
		}

		public float BackgroundGradientTop
		{
			get
			{
				return _background.GradientTop;
			}
		}

		public Material GlowGoldMaterial
		{
			get
			{
				return _glowGold;
			}
		}

		public Color GlowGoldColor
		{
			get
			{
				return _glowGoldColor;
			}
		}

		public float GlowGoldFalloff
		{
			get
			{
				return _glowGoldFalloff;
			}
		}

		public ThemeEffect[] Effects
		{
			get
			{
				return _effects;
			}
		}

		public bool RoofedCullingEnabled
		{
			get
			{
				return _roofedCullingEnabled;
			}
		}

		public EnvironmentType[] RoofedCullingEnvironmentsHigh
		{
			get
			{
				return _roofedCullingEnvironmentsHigh;
			}
		}

		public EnvironmentType[] RoofedCullingEnvironmentsLow
		{
			get
			{
				return _roofedCullingEnvironmentsLow;
			}
		}

		public RoofedCulling.CustomRoofedCulling[] RoofedCullingEnvironmentsCustom
		{
			get
			{
				return _roofedCullingEnvironmentsCustom;
			}
		}

		public AudioClip GameMusic
		{
			get
			{
				return _gameMusic;
			}
		}

		public Color RollImpactRockColor
		{
			get
			{
				return _rollImpactRockColor;
			}
		}

		public Color RollImpactDustColorMin
		{
			get
			{
				return _rollImpactDustColorMin;
			}
		}

		public Color RollImpactDustColorMax
		{
			get
			{
				return _rollImpactDustColorMax;
			}
		}

		public GenericAndThemePrefabGroup[] GenericToThemePrefabMappingGroups
		{
			get
			{
				return _prefabMappingsGeneric;
			}
		}

		public RouteEnvironmentModelAssets[] RouteEnvironmentModelAssets
		{
			get
			{
				return _routeEnvironmentModelAssets;
			}
		}

		public GroundAssetGroup GroundAssets
		{
			get
			{
				return _groundAssets;
			}
		}

		public string ThemePath
		{
			get
			{
				return "Assets/Themes/" + base.name;
			}
		}

		public TransitionConfig[] EnvironmentTransitionConfigs
		{
			get
			{
				return _environmentTransitionConfigs;
			}
		}

		private void OnEnable()
		{
			_initialized = false;
			Initialize();
		}

		private void EditorInitData()
		{
			_genericToThemePrefabDict.Clear();
			if (_prefabMappingsGeneric == null || _prefabMappingsGeneric.Length == 0)
			{
				_prefabMappingsGeneric = new GenericAndThemePrefabGroup[5]
				{
					new GenericAndThemePrefabGroup
					{
						Name = "Segments/Fillers",
						PrefabMappings = new GenericAndThemePrefab[0]
					},
					new GenericAndThemePrefabGroup
					{
						Name = "Props",
						PrefabMappings = new GenericAndThemePrefab[0]
					},
					new GenericAndThemePrefabGroup
					{
						Name = "Segments/Specials",
						PrefabMappings = new GenericAndThemePrefab[0]
					},
					new GenericAndThemePrefabGroup
					{
						Name = "Segments/Tracks",
						PrefabMappings = new GenericAndThemePrefab[0]
					},
					new GenericAndThemePrefabGroup
					{
						Name = "Segments/Tunnels",
						PrefabMappings = new GenericAndThemePrefab[0]
					}
				};
			}
		}

		public void Initialize()
		{
			if (_initialized)
			{
				return;
			}
			_genericToThemePrefabDict = new Dictionary<GameObject, GameObject>();
			_devicePerformanceRelativeMaterialsDict = new Dictionary<Material, Material>();
			if (Application.isEditor)
			{
				EditorInitData();
			}
			if (_prefabMappingsGeneric != null)
			{
				for (int i = 0; i < _prefabMappingsGeneric.Length; i++)
				{
					GenericAndThemePrefabGroup genericAndThemePrefabGroup = _prefabMappingsGeneric[i];
					if (genericAndThemePrefabGroup.PrefabMappings == null)
					{
						continue;
					}
					for (int j = 0; j < genericAndThemePrefabGroup.PrefabMappings.Length; j++)
					{
						GenericAndThemePrefab genericAndThemePrefab = genericAndThemePrefabGroup.PrefabMappings[j];
						if (genericAndThemePrefab.GenericPrefab != null && genericAndThemePrefab.ThemePrefab != null)
						{
							_genericToThemePrefabDict.Add(genericAndThemePrefab.GenericPrefab, genericAndThemePrefab.ThemePrefab);
							genericAndThemePrefab.ThemePrefab.AddMissingComponent<PlaceholderPrefab>();
						}
					}
				}
			}
			DeviceInfo.PerformanceLevel performanceLevel = ((!Application.isPlaying) ? DeviceInfo.PerformanceLevel.High : DeviceInfo.Instance.performanceLevel);
			if (_devicePerformanceRelativeMaterials != null && performanceLevel != DeviceInfo.PerformanceLevel.High)
			{
				for (int k = 0; k < _devicePerformanceRelativeMaterials.Length; k++)
				{
					MaterialGroup materialGroup = _devicePerformanceRelativeMaterials[k];
					if (!(materialGroup.HighPerformance == null))
					{
						Material material = null;
						switch (performanceLevel)
						{
						case DeviceInfo.PerformanceLevel.Low:
							material = materialGroup.LowPerformance;
							break;
						case DeviceInfo.PerformanceLevel.Medium:
							material = materialGroup.MediumPerformance;
							break;
						}
						if (!(material == null))
						{
							_devicePerformanceRelativeMaterialsDict[materialGroup.HighPerformance] = material;
						}
					}
				}
			}
			_initialized = true;
		}

		public void InitializeEnvironmentTransitions()
		{
			TransitionConfig[] environmentTransitionConfigs = _environmentTransitionConfigs;
			foreach (TransitionConfig transitionConfig in environmentTransitionConfigs)
			{
				transitionConfig.Initialize();
			}
			SingletonMonoBehaviour<EnvironmentTransitionController>.Instantiate();
		}

		public void PrepoolPrefabs()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			for (int i = 0; i < _prefabMappingsGeneric.Length; i++)
			{
				GenericAndThemePrefabGroup genericAndThemePrefabGroup = _prefabMappingsGeneric[i];
				if (genericAndThemePrefabGroup.PrefabMappings == null)
				{
					continue;
				}
				for (int j = 0; j < genericAndThemePrefabGroup.PrefabMappings.Length; j++)
				{
					GenericAndThemePrefab genericAndThemePrefab = genericAndThemePrefabGroup.PrefabMappings[j];
					if (genericAndThemePrefab.GenericPrefab != null && genericAndThemePrefab.ThemePrefab != null)
					{
						PreloadSetting component = genericAndThemePrefab.GenericPrefab.GetComponent<PreloadSetting>();
						if (component != null)
						{
							PoolManager.GetPrefabPool(genericAndThemePrefab.ThemePrefab.transform).preloadAmount = component.PreloadAmount;
						}
					}
				}
			}
			DeviceInfo.PerformanceLevel[] array = (DeviceInfo.PerformanceLevel[])Enum.GetValues(typeof(DeviceInfo.PerformanceLevel));
			for (int k = 0; k < _themePreloadSettings.Count; k++)
			{
				int num = 0;
				foreach (DeviceInfo.PerformanceLevel perfLevel in array)
				{
					if (DeviceInfo.Instance.IsDeviceCapablePerformance(perfLevel))
					{
						num += _themePreloadSettings[k].GetPreloadAmount(perfLevel);
					}
				}
				if (num > 0)
				{
					PoolManager.GetPrefabPool(_themePreloadSettings[k].Prefab).preloadAmount = num;
				}
			}
			PoolManager.TriggerPoolBalancing();
		}

		public bool TryGetDevicePerformanceRelativeMaterial(Material material, out Material performanceRelative)
		{
			Initialize();
			return _devicePerformanceRelativeMaterialsDict.TryGetValue(material, out performanceRelative);
		}

		public bool TryGetThemePrefabFromGeneric(GameObject genericPrefab, out GameObject themePrefab)
		{
			Initialize();
			if (_genericToThemePrefabDict.ContainsKey(genericPrefab))
			{
				themePrefab = _genericToThemePrefabDict[genericPrefab];
				if (themePrefab == null)
				{
					SDebug.LogError("Theme.TryGetThemePrefabFromGeneric(): generic prefab is set, but theme specific missing. Setting theme prefab to generic " + genericPrefab.name);
					themePrefab = genericPrefab;
				}
				return true;
			}
			if (TryGetSynchronizedThemeAssetFromGeneric(genericPrefab, out themePrefab))
			{
				return true;
			}
			themePrefab = null;
			return false;
		}

		public bool TryGetSynchronizedThemeAssetFromGeneric(GameObject genericPrefab, out GameObject themePrefab)
		{
			Initialize();
			if (genericPrefab == null)
			{
				themePrefab = null;
				return false;
			}
			if (_genericToThemePrefabDict.TryGetValue(genericPrefab, out themePrefab))
			{
				if (themePrefab == null)
				{
					SDebug.LogErrorFormat(this, "Theme.TryGetSharedThemePrefabFromGeneric(): generic prefab is set, but theme specific missing. Generic: {0}", genericPrefab.name);
					return false;
				}
				return true;
			}
			return false;
		}

		public bool ShouldIgnoreBase(EnvironmentKind envKind)
		{
			for (int i = 0; i < _baseIgnoredEnvironmentKinds.Count; i++)
			{
				if (_baseIgnoredEnvironmentKinds[i] == envKind)
				{
					return true;
				}
			}
			return false;
		}

		public float GetCameraDistanceToClosestBackgroundLayer()
		{
			float num = 1000f;
			BackgroundLayer backgroundLayer = null;
			int num2 = -1;
			for (int i = 0; i < _background.Layers.Length; i++)
			{
				if (_background.Layers[i].LayerIndex > num2)
				{
					num2 = _background.Layers[i].LayerIndex;
					backgroundLayer = _background.Layers[i];
				}
			}
			if (backgroundLayer != null)
			{
				num += (float)backgroundLayer.LayerIndex * (0f - _background.LayerSpacing) + backgroundLayer.OffsetFromLayerDefault.z;
			}
			return num;
		}
	}
}
