using SYBO.Common.Utilities;
using SYBO.Subway.Themes;
using UnityEngine;

namespace SYBO.Subway.Level
{
	[RequireComponent(typeof(Renderer))]
	[RequireComponent(typeof(MeshFilter))]
	public class PerformanceLevelModelSwap : MonoBehaviour
	{
		[Header("If no mesh AND material info set on object, will look for replacement material in Theme")]
		public PerformanceLevelModelInfo performanceLevelModel;

		private bool _initialized;

		private bool _swapEvaluated;

		private MeshFilter _meshFilter;

		private Renderer _renderer;

		private ThemeManager _themeManager;

		private void InitializeCheck()
		{
			if (!_initialized)
			{
				_renderer = GetComponent<Renderer>();
				_meshFilter = GetComponent<MeshFilter>();
				_themeManager = ThemeManager.Instance;
				_initialized = true;
			}
		}

		private void Start()
		{
			InitializeCheck();
			if (!_swapEvaluated)
			{
				SwapMaterials();
			}
		}

		public void SwapMaterials()
		{
			if (_swapEvaluated)
			{
				return;
			}
			InitializeCheck();
			Theme theme = _themeManager.Theme;
			DeviceInfo.PerformanceLevel performanceLevel = DeviceInfo.Instance.performanceLevel;
			if (!SwapMaterials(performanceLevel) && DeviceInfo.Instance.performanceLevel != DeviceInfo.PerformanceLevel.High)
			{
				Material[] sharedMaterials = _renderer.sharedMaterials;
				Material performanceRelative = null;
				for (int i = 0; i < sharedMaterials.Length; i++)
				{
					if (theme.TryGetDevicePerformanceRelativeMaterial(sharedMaterials[i], out performanceRelative))
					{
						sharedMaterials[i] = performanceRelative;
					}
				}
				_renderer.sharedMaterials = sharedMaterials;
			}
			_swapEvaluated = true;
		}

		private bool SwapMaterials(DeviceInfo.PerformanceLevel perfLevel)
		{
			switch (perfLevel)
			{
			case DeviceInfo.PerformanceLevel.Low:
				return UpdateModel(performanceLevelModel.LowPerformance);
			case DeviceInfo.PerformanceLevel.Medium:
				return UpdateModel(performanceLevelModel.MediumPerformance);
			case DeviceInfo.PerformanceLevel.High:
				return UpdateModel(performanceLevelModel.HighPerformance);
			default:
				return false;
			}
		}

		private bool UpdateModel(ModelInfo meshInfo)
		{
			if (meshInfo.Mesh == null)
			{
				return false;
			}
			_meshFilter.mesh = meshInfo.Mesh;
			_renderer.materials = meshInfo.Materials;
			return true;
		}
	}
}
