using SYBO.Subway.Managers;
using UnityEngine;

namespace SYBO.Subway.Cameras
{
	[RequireComponent(typeof(Camera))]
	public class MainCamera : MonoBehaviour
	{
		[SerializeField]
		private Transform _skyPlaneTransform;

		[SerializeField]
		private Transform _reviveEffectTransform;

		[SerializeField]
		private Transform _speedStripesTransform;

		private Camera _camera;

		public Transform SkyPlane
		{
			get
			{
				return _skyPlaneTransform;
			}
		}

		public Transform ReviveEffectTransform
		{
			get
			{
				return _reviveEffectTransform;
			}
		}

		protected void Awake()
		{
			_camera = GetComponent<Camera>();
			_skyPlaneTransform.gameObject.SetActive(true);
			_reviveEffectTransform.gameObject.SetActive(true);
			_speedStripesTransform.gameObject.SetActive(true);
			MeshRenderer component = _skyPlaneTransform.GetComponent<MeshRenderer>();
			int num = 1000;
			int renderQueue = component.sharedMaterial.renderQueue;
			if (num < renderQueue)
			{
				component.sharedMaterial.renderQueue = 1000;
			}
			if (DemoVideoModeManager.IsEnabled)
			{
				_camera.farClipPlane = 3000f;
				Vector3 position = _skyPlaneTransform.position;
				_skyPlaneTransform.position = new Vector3(position.x, position.y, 2900f);
				Vector3 localScale = _skyPlaneTransform.localScale;
				localScale.x = 700f;
				_skyPlaneTransform.localScale = localScale;
			}
		}
	}
}
