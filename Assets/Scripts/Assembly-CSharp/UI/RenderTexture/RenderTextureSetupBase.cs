using System;
using System.Collections.Generic;
using Kiloo.Common.Extensions;
using SYBO.Common.Utility;
using UI.Generic;
using UnityEngine;

namespace UI.RenderTexture
{
	[Serializable]
	public abstract class RenderTextureSetupBase
	{
		[Tooltip("Reference to the UI texture that will eventually serve as the UI target for the render texture")]
		[SerializeField]
		protected UITexture texture;

		[Tooltip("List of objects that will be instantiated and rendered into the render texture, incl. their position, rotation, scale and desired animation")]
		[SerializeField]
		protected List<RenderTextureObject> renderTextureObjects;

		protected List<GameObject> _objectInstances = new List<GameObject>();

		protected List<GameObject> _objectAnchorsInstances = new List<GameObject>();

		protected RenderTextureManager _renderTextureManager;

		private CameraSettings _cameraSettingsOverride;

		private bool _renderTextureHasBeenInitialized;

		public virtual void Setup(UITexture texture, List<RenderTextureObject> renderTextureObjects, CameraSettings overrideCameraSettings = null, bool copySettings = true)
		{
			foreach (GameObject objectAnchorsInstance in _objectAnchorsInstances)
			{
				objectAnchorsInstance.SetActive(false);
			}
			this.renderTextureObjects = renderTextureObjects;
			this.texture = texture;
			Setup(overrideCameraSettings, copySettings);
		}

		public virtual void Setup(CameraSettings overrideCameraSettings = null, bool copySettings = true)
		{
			if (!_renderTextureHasBeenInitialized)
			{
				_renderTextureHasBeenInitialized = true;
				SetupInternal(overrideCameraSettings, copySettings);
			}
		}

		private void SetupInternal(CameraSettings overrideCameraSettings = null, bool copySettings = true)
		{
			_renderTextureManager = RenderTextureManager.instance;
			if (overrideCameraSettings != null)
			{
				_cameraSettingsOverride = overrideCameraSettings;
			}
			for (int i = 0; i < renderTextureObjects.Count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(renderTextureObjects[i].prefab);
				gameObject.transform.localPosition = renderTextureObjects[i].position;
				gameObject.transform.localRotation = Quaternion.Euler(renderTextureObjects[i].rotation);
				gameObject.transform.localScale = renderTextureObjects[i].scale;
				GameObject item = gameObject;
				if (renderTextureObjects[i].animationClip != null)
				{
					GameObject gameObject2 = new GameObject(gameObject.name + "_AnimationAnchor");
					gameObject2.transform.parent = gameObject.transform.parent;
					gameObject.transform.parent = gameObject2.transform;
					UnscaledAnimation unscaledAnimation = gameObject2.AddComponent<UnscaledAnimation>();
					unscaledAnimation.loop = true;
					unscaledAnimation.playAutomatically = true;
					unscaledAnimation.clip = renderTextureObjects[i].animationClip;
					unscaledAnimation.StartAnimating(renderTextureObjects[i].animationDelay);
					item = gameObject2;
				}
				_objectInstances.Add(gameObject);
				_objectAnchorsInstances.Add(item);
			}
			if (texture != null)
			{
				texture.enabled = true;
			}
			OnSetup();
			if (texture != null)
			{
				texture.mainTexture = _renderTextureManager.GetRenderTextureFor(_objectAnchorsInstances, texture, _cameraSettingsOverride, copySettings);
			}
			if (texture != null)
			{
				SubscribableMonobehaviour component = texture.GetComponent<SubscribableMonobehaviour>();
				if (component == null)
				{
					component = texture.AddComponent<SubscribableMonobehaviour>();
					component.OnOnEnable = OnRenderTextureEnable;
					component.OnOnDisable = OnRenderTextureDisable;
				}
			}
		}

		private void OnRenderTextureDisable()
		{
			ReleaseRenderTexture();
		}

		private void OnRenderTextureEnable()
		{
			SetupInternal();
		}

		protected virtual void OnSetup()
		{
		}

		private void ReleaseRenderTexture()
		{
			_objectInstances.Clear();
			_objectAnchorsInstances.Clear();
			_renderTextureManager.ReleaseTexture((UnityEngine.RenderTexture)texture.mainTexture);
			texture.mainTexture = null;
			texture.enabled = false;
		}

		protected virtual void OnReleaseRenderTexture()
		{
		}
	}
}
