using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using SYBO.Components.Utilities;
using SYBO.Subway.Level;
using SYBO.Subway.Routes;
using UnityEngine;

namespace SYBO.Subway.Themes
{
	[PrefabPath("Assets/Prefabs/ThemeManager.prefab")]
	public class ThemeManager : MonoBehaviour
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<Theme> ThemeChanged__BackingField;

		[SerializeField]
		private Vector4 _distortion = new Vector4(-0.0005f, -0.0003f, 0f, 0f);

		[SerializeField]
		private Theme _theme;

		private Theme _themeRefDuplicate;

		private CameraCulling _cameraCulling;

		private float _backgroundStartDistance;

		private float _backgroundToFogBlendStartDistance;

		private static bool _applicationQuitting;

		private static ThemeManager _instance;

		public Theme Theme
		{
			get
			{
				return _theme;
			}
		}

		public bool DistortionYFlipped
		{
			set
			{
				if (value)
				{
					_distortion.y = 0.0003f;
				}
				else
				{
					_distortion.y = -0.0003f;
				}
				Shader.SetGlobalVector("_Distort", _distortion);
			}
		}

		public static ThemeManager Instance
		{
			get
			{
				if (_applicationQuitting && Application.isPlaying)
				{
					return null;
				}
				if (_instance == null)
				{
					_instance = Utils.FindObject<ThemeManager>();
				}
				return _instance;
			}
		}

		public event Action<Theme> ThemeChanged
		{
			add
			{
				Action<Theme> action = ThemeChanged__BackingField;
				Action<Theme> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref ThemeChanged__BackingField, (Action<Theme>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<Theme> action = ThemeChanged__BackingField;
				Action<Theme> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref ThemeChanged__BackingField, (Action<Theme>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		private void Awake()
		{
			if (_instance != null && _instance != this)
			{
				SDebug.LogError("Instatiated multiple ThemeManagers!");
			}
			_applicationQuitting = false;
			_instance = this;
			UIScreenController.OnApplicationResumed += UIScreenController_OnApplicationResumed;
		}

		private void Start()
		{
			if (_theme != null && ThemeChanged__BackingField != null)
			{
				ThemeChanged__BackingField(_theme);
			}
		}

		public void PrepoolThemePrefabs()
		{
			if (Application.isPlaying)
			{
				_theme.PrepoolPrefabs();
			}
		}

		private void Update()
		{
			if (_theme != _themeRefDuplicate && _theme != null)
			{
				_themeRefDuplicate = _theme;
				ResetTheme();
				PrepoolThemePrefabs();
			}
		}

		private void OnApplicationQuit()
		{
			_applicationQuitting = true;
		}

		private void OnValidate()
		{
			ResetTheme();
		}

		private void OnDestroy()
		{
			UIScreenController.OnApplicationResumed -= UIScreenController_OnApplicationResumed;
			if (_instance == this)
			{
				_applicationQuitting = true;
			}
		}

		public void ForceRefresh()
		{
			ResetTheme();
		}

		private void ResetTheme()
		{
			SetShaderStates();
			RenderSettings.fog = false;
			if (Camera.main != null)
			{
				Camera.main.backgroundColor = _theme.BackgroundGradientBottomColor;
			}
			_theme.GlowGoldMaterial.SetColor("_MainColor", _theme.GlowGoldColor);
			_theme.GlowGoldMaterial.SetFloat("_Falloff", _theme.GlowGoldFalloff);
			if (_cameraCulling == null)
			{
				_cameraCulling = UnityEngine.Object.FindObjectOfType(typeof(CameraCulling)) as CameraCulling;
			}
			if (_cameraCulling != null)
			{
				_cameraCulling.TransparentFXCullingDistance = _theme.GlowGoldFalloff;
			}
			Resources.UnloadUnusedAssets();
		}

		private void SetShaderStates()
		{
			Shader.SetGlobalVector("_Distort", _distortion);
			Shader.SetGlobalColor("_BackgroundGradientTopColor", _theme.BackgroundGradientTopColor);
			Shader.SetGlobalColor("_BackgroundGradientBottomColor", _theme.BackgroundGradientBottomColor);
			Shader.SetGlobalFloat("_BackgroundGradientBottom", _theme.BackgroundGradientBottom);
			Shader.SetGlobalFloat("_BackgroundGradientTop", _theme.BackgroundGradientTop);
			Shader.SetGlobalColor("_FogColor", _theme.FogColor);
			Shader.SetGlobalFloat("_BackgroundStartDistance", _theme.BackgroundStartDistance);
			Shader.SetGlobalFloat("_FogEndDistance", _theme.FogEndDistance);
			Shader.SetGlobalFloat("_FogStartDistance", _theme.FogStartDistance);
			float num = _theme.FogEndDistance - _theme.FogStartDistance;
			float value = ((num != 0f) ? (1f / num) : 0f);
			Shader.SetGlobalFloat("_FogTransitionLengthReciprocal", value);
			float num2 = _theme.BackgroundStartDistance - _theme.FogEndDistance;
			float value2 = ((num2 != 0f) ? (1f / num2) : 0f);
			Shader.SetGlobalFloat("_BackgroundToFogTransitionLengthReciprocal", value2);
			Shader.SetGlobalFloat("_DistantBackgroundStartDistance", _theme.DistantBackgroundStartDistance);
			Shader.SetGlobalFloat("_DistantFogEndDistance", _theme.DistantFogEndDistance);
			Shader.SetGlobalFloat("_DistantFogStartDistance", _theme.DistantFogStartDistance);
			float num3 = _theme.DistantFogEndDistance - _theme.DistantFogStartDistance;
			float value3 = ((num3 != 0f) ? (1f / num3) : 0f);
			Shader.SetGlobalFloat("_DistantFogTransitionLengthReciprocal", value3);
			float num4 = _theme.DistantBackgroundStartDistance - _theme.DistantFogEndDistance;
			float value4 = ((num4 != 0f) ? (1f / num4) : 0f);
			Shader.SetGlobalFloat("_DistantBackgroundToFogTransitionLengthReciprocal", value4);
		}

		private void UIScreenController_OnApplicationResumed()
		{
			SetShaderStates();
		}

		public int GetEnvironmentModelVariationCount(RouteCategory routeCategory, EnvironmentKind envTypeInfo)
		{
			if (_theme.RouteEnvironmentModelAssets.Count() == 0)
			{
				return 0;
			}
			RouteEnvironmentModelAssets routeEnvironmentModelAssets = null;
			for (int i = 0; i < _theme.RouteEnvironmentModelAssets.Length; i++)
			{
				if (_theme.RouteEnvironmentModelAssets[i].RouteCategory == routeCategory)
				{
					routeEnvironmentModelAssets = _theme.RouteEnvironmentModelAssets[i];
					break;
				}
			}
			if (routeEnvironmentModelAssets == null)
			{
				return 0;
			}
			EnvironmentModelAssets environmentModelAssets = null;
			for (int j = 0; j < routeEnvironmentModelAssets.EnvironmentAssets.Length; j++)
			{
				if (routeEnvironmentModelAssets.EnvironmentAssets[j].Environment == envTypeInfo)
				{
					environmentModelAssets = routeEnvironmentModelAssets.EnvironmentAssets[j];
					break;
				}
			}
			if (environmentModelAssets == null)
			{
				return 0;
			}
			return environmentModelAssets.SequenceVariations.Length;
		}

		public bool NeedSameVariationForAllSegments(EnvironmentType envType)
		{
			return envType == EnvironmentType.Epic;
		}

		public Transform GetEnvironmentSegmentModel(RouteCategory routeCategory, EnvironmentKind envKind, EnvironmentSequence sequence, EnvironmentSide side, int variation)
		{
			if (_theme.RouteEnvironmentModelAssets.Count() == 0)
			{
				return null;
			}
			RouteEnvironmentModelAssets routeEnvironmentModelAssets = null;
			for (int i = 0; i < _theme.RouteEnvironmentModelAssets.Length; i++)
			{
				if (_theme.RouteEnvironmentModelAssets[i].RouteCategory == routeCategory)
				{
					routeEnvironmentModelAssets = _theme.RouteEnvironmentModelAssets[i];
					break;
				}
			}
			if (routeEnvironmentModelAssets == null)
			{
				return null;
			}
			EnvironmentModelAssets environmentModelAssets = null;
			for (int j = 0; j < routeEnvironmentModelAssets.EnvironmentAssets.Length; j++)
			{
				if (routeEnvironmentModelAssets.EnvironmentAssets[j].Environment == envKind)
				{
					environmentModelAssets = routeEnvironmentModelAssets.EnvironmentAssets[j];
					break;
				}
			}
			if (environmentModelAssets == null)
			{
				return null;
			}
			int num = variation;
			int num2 = environmentModelAssets.SequenceVariations.Length;
			if (num2 < variation)
			{
				SDebug.LogError(string.Concat("Models for ", envKind.Type, " do not have ", variation, " variations! Use last variation ", num2));
				num = num2;
			}
			int num3 = num - 1;
			EnvironmentSegmentSequence segments = environmentModelAssets.SequenceVariations[num3];
			EnvironmentSegmentParts? sequenceModel = GetSequenceModel(segments, sequence);
			if (!sequenceModel.HasValue)
			{
				return null;
			}
			switch (side)
			{
			case EnvironmentSide.Left:
				return sequenceModel.Value.Left;
			case EnvironmentSide.Right:
				return sequenceModel.Value.Right;
			default:
				return sequenceModel.Value.Joint;
			}
		}

		private EnvironmentSegmentParts? GetSequenceModel(EnvironmentSegmentSequence segments, EnvironmentSequence sequence)
		{
			switch (sequence)
			{
			case EnvironmentSequence.Start:
				return segments.Start;
			case EnvironmentSequence.Mid:
				return segments.Mid;
			case EnvironmentSequence.End:
				return segments.End;
			default:
				return null;
			}
		}
	}
}
