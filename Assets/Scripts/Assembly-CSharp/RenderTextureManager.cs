using System.Collections.Generic;
using Kiloo.Common.Extensions;
using UI;
using UI.ModelCamera;
using UnityEngine;

public class RenderTextureManager : MonoBehaviour
{
	private const float CAMERA_Y_OFFSET = 10000f;

	private static RenderTextureManager _instance;

	[SerializeField]
	private GameObject _cameraPrefab;

	[SerializeField]
	private Transform _renderTextureCamerasRoot;

	private List<RenderTextureCamera> _renderTextureCameras;

	[SerializeField]
	private RenderTexture _renderTexturePrefab;

	public static RenderTextureManager instance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
		_renderTextureCameras = new List<RenderTextureCamera>();
	}

	public RenderTexture GetRenderTextureFor(List<GameObject> renderTextureGos, UITexture targetUITexture = null, CameraSettings overrideCameraSettings = null, bool copySettings = true)
	{
		RenderTextureCamera availableCamera = GetAvailableCamera(copySettings, targetUITexture);
		availableCamera.Activate(overrideCameraSettings);
		for (int i = 0; i < renderTextureGos.Count; i++)
		{
			Vector3 localPosition = renderTextureGos[i].transform.localPosition;
			Quaternion localRotation = renderTextureGos[i].transform.localRotation;
			Vector3 localScale = renderTextureGos[i].transform.localScale;
			renderTextureGos[i].transform.parent = availableCamera.GetAnchor();
			Utility.SetLayerRecursively(renderTextureGos[i].transform, availableCamera.cameraLayer);
			renderTextureGos[i].transform.localPosition = localPosition;
			renderTextureGos[i].transform.localRotation = localRotation;
			renderTextureGos[i].transform.localScale = localScale;
		}
		return availableCamera.renderTexture;
	}

	public void ReleaseTexture(RenderTexture texture)
	{
		RenderTextureCamera renderTextureCamera = _renderTextureCameras.Find((RenderTextureCamera item) => item.renderTexture == texture);
		if (renderTextureCamera != null)
		{
			renderTextureCamera.Deactivate();
		}
	}

	private RenderTextureCamera GetAvailableCamera(bool copySettings, UITexture targetUITexture)
	{
		RenderTextureCamera renderTextureCamera = null;
		foreach (RenderTextureCamera renderTextureCamera2 in _renderTextureCameras)
		{
			if (!renderTextureCamera2.isActive)
			{
				renderTextureCamera = renderTextureCamera2;
				break;
			}
		}
		if (renderTextureCamera == null)
		{
			renderTextureCamera = CreateNewCamera();
		}
		renderTextureCamera.camera.targetTexture = null;
		Object.Destroy(renderTextureCamera.renderTexture);
		RenderTexture targetTexture = (renderTextureCamera.renderTexture = CreateRenderTexture(copySettings, targetUITexture));
		renderTextureCamera.camera.targetTexture = targetTexture;
		return renderTextureCamera;
	}

	private RenderTextureCamera CreateNewCamera()
	{
		GameObject gameObject = Object.Instantiate(_cameraPrefab, _renderTextureCamerasRoot);
		Camera component = gameObject.GetComponent<Camera>();
		Object.Destroy(component.GetComponent<FlareLayer>());
		component.transform.SetLocalPosY((float)_renderTextureCameras.Count * 10000f);
		RenderTextureCamera renderTextureCamera = new RenderTextureCamera(component, gameObject);
		_renderTextureCameras.Add(renderTextureCamera);
		return renderTextureCamera;
	}

	private RenderTexture CreateRenderTexture(bool copySettings, UITexture targetUITexture = null)
	{
		int width;
		int height;
		if (targetUITexture != null)
		{
			width = targetUITexture.width * 2;
			height = targetUITexture.height * 2;
		}
		else
		{
			width = _renderTexturePrefab.width;
			height = _renderTexturePrefab.height;
		}
		RenderTexture dst = new RenderTexture(width, height, _renderTexturePrefab.depth, _renderTexturePrefab.format);
		dst.name = "RenderTexture" + _renderTextureCameras.Count;
		if (copySettings)
		{
			CopyRenderTextureSettings(ref dst, _renderTexturePrefab);
		}
		return dst;
	}

	private void CopyRenderTextureSettings(ref RenderTexture dst, RenderTexture src)
	{
		dst.dimension = src.dimension;
		dst.antiAliasing = src.antiAliasing;
		dst.mipMapBias = src.mipMapBias;
		dst.wrapMode = src.wrapMode;
		dst.filterMode = src.filterMode;
		dst.anisoLevel = src.anisoLevel;
		dst.autoGenerateMips = src.autoGenerateMips;
		dst.volumeDepth = src.volumeDepth;
	}
}
