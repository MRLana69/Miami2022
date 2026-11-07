using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
	public class CameraSettings
	{
		private Dictionary<Type, object> _defaultValues = new Dictionary<Type, object>
		{
			{
				typeof(int),
				-1
			},
			{
				typeof(float),
				-1f
			},
			{
				typeof(Color),
				Color.clear
			},
			{
				typeof(CameraClearFlags),
				CameraClearFlags.Nothing
			}
		};

		private static CameraSettings _renderTextureCameraSettings = new CameraSettings
		{
			clearFlags = CameraClearFlags.Skybox,
			farClipPlane = 15f,
			nearClipPlane = 0.3f,
			fieldOfView = 35,
			backgroundColor = Color.clear,
			depth = 20f,
			layer = Layers.Instance._RENDERTEXTURE
		};

		public int layer { get; set; }

		public CameraClearFlags clearFlags { get; set; }

		public float nearClipPlane { get; set; }

		public float farClipPlane { get; set; }

		public Color backgroundColor { get; set; }

		public float depth { get; set; }

		public int fieldOfView { get; set; }

		public CameraSettings()
		{
			layer = GetDefaultValue<int>();
			clearFlags = GetDefaultValue<CameraClearFlags>();
			nearClipPlane = GetDefaultValue<float>();
			farClipPlane = GetDefaultValue<float>();
			backgroundColor = GetDefaultValue<Color>();
			depth = GetDefaultValue<float>();
			fieldOfView = GetDefaultValue<int>();
		}

		public void ApplySettingsToCamera(Camera camera, CameraSettings overrideCameraSettings)
		{
			camera.gameObject.layer = GetValue(overrideCameraSettings.layer, layer);
			camera.cullingMask = 1 << GetValue(overrideCameraSettings.layer, layer);
			camera.clearFlags = GetValue(overrideCameraSettings.clearFlags, clearFlags);
			camera.farClipPlane = GetValue(overrideCameraSettings.farClipPlane, farClipPlane);
			camera.nearClipPlane = GetValue(overrideCameraSettings.nearClipPlane, nearClipPlane);
			camera.fieldOfView = GetValue(overrideCameraSettings.fieldOfView, fieldOfView);
			camera.depth = GetValue(overrideCameraSettings.depth, depth);
			camera.backgroundColor = GetValue(overrideCameraSettings.backgroundColor, backgroundColor);
		}

		private T GetValue<T>(T overrideValue, T defaultCameraValue)
		{
			if (!overrideValue.Equals(GetDefaultValue<T>()))
			{
				return overrideValue;
			}
			return defaultCameraValue;
		}

		private T GetDefaultValue<T>()
		{
			object value = null;
			_defaultValues.TryGetValue(typeof(T), out value);
			return (T)value;
		}

		public static void ApplyRenderTextureCameraSettings(Camera camera, CameraSettings overrideCameraSettings = null)
		{
			if (overrideCameraSettings == null)
			{
				overrideCameraSettings = new CameraSettings();
			}
			_renderTextureCameraSettings.ApplySettingsToCamera(camera, overrideCameraSettings);
		}
	}
}
