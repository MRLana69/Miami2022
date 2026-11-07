using Kiloo.Common.Extensions;
using UnityEngine;

namespace UI.ModelCamera
{
	public class RenderTextureCamera
	{
		private GameObject _cameraGo;

		private readonly Camera _camera;

		public UnityEngine.RenderTexture renderTexture;

		public Camera camera
		{
			get
			{
				return _camera;
			}
		}

		public bool isActive { get; private set; }

		public int cameraLayer
		{
			get
			{
				return _cameraGo.layer;
			}
		}

		public RenderTextureCamera(Camera camera, GameObject cameraGo)
		{
			_camera = camera;
			_cameraGo = cameraGo;
			Deactivate();
		}

		public Transform GetAnchor()
		{
			return _cameraGo.transform.GetChild(0);
		}

		public void Activate(CameraSettings overrideCameraSettings)
		{
			CameraSettings.ApplyRenderTextureCameraSettings(_camera, overrideCameraSettings);
			isActive = true;
			_cameraGo.SetActive(true);
		}

		public void Deactivate()
		{
			isActive = false;
			_cameraGo.SetActive(false);
			GetAnchor().KillChildren();
			ClearRenderTexture(renderTexture);
		}

		private void ClearRenderTexture(UnityEngine.RenderTexture renderTextureToClear)
		{
			if (renderTextureToClear != null)
			{
				UnityEngine.RenderTexture active = UnityEngine.RenderTexture.active;
				UnityEngine.RenderTexture.active = renderTextureToClear;
				GL.Clear(true, true, Color.clear);
				UnityEngine.RenderTexture.active = active;
			}
		}
	}
}
