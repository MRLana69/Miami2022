using System;
using System.Collections;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using Kiloo.Common.Utility;
using Missions;
using NaughtyAttributes;
using SYBO.Subway.Placeholders.Lifecycles;
using UnityEngine;

namespace UI.Pages
{
	public class PageTop : MonoBehaviour
	{
		public enum TopSplatType
		{
			Bubble = 0,
			Star = 1
		}

		public class EventsPageTopPayload
		{
			public string mainTitle;

			public string subTitle;

			public string multiplierText = string.Empty;

			public Color mainColor;

			public Color subTitleColor;

			public Color splatColor;

			public Color splatShadowColor;

			public TopSplatType topSplatType;
		}

		public class TopIconPayload
		{
			public GameObject iconPrefab;

			public Vector3 localPositionOffset = Vector3.zero;

			public Vector3 localRotationOffsetEuler = Vector3.zero;

			public Vector3 localScaleOffset = Vector3.one;
		}

		[SerializeField]
		private UILabel _mainTitle;

		[SerializeField]
		private UILabel _subtitle;

		[SerializeField]
		private UIWidget _slotForItemLoad;

		[SerializeField]
		private UISprite _bubbleSplat;

		[SerializeField]
		private UISprite _bubbleSplatShadow;

		[SerializeField]
		private UISprite _starSplat;

		[SerializeField]
		private UISprite _starSplatShadow;

		[SerializeField]
		private UILabel _multiplierText;

		[SerializeField]
		private UILabel _shadowAnimText;

		[SerializeField]
		private UIWidget _iconGuideWidget;

		[SerializeField]
		private Transform _iconAnchor;

		[Header("Effects")]
		[Tooltip("A curve to use as a scaling effect on the top icons of events pages (used by eg. missions when the mission setchanges)")]
		[SerializeField]
		private AnimationCurve _iconScaleEffectCurve;

		[SerializeField]
		private float _iconScaleEffectDuration = 0.5f;

		[SerializeField]
		private Vector3 _iconScaleEffectAmount = new Vector3(0.1f, 0.1f, 0.1f);

		[SerializeField]
		private float _iconScaleEffectDelay;

		[Tooltip("A curve which controls the fading effect for the shadow")]
		[SerializeField]
		private AnimationCurve _shadowAlphaEffectCurve;

		[SerializeField]
		private float _shadowAlphaEffectDuration = 0.5f;

		[SerializeField]
		private float _shadowAlphaEffectAmount = 1f;

		[SerializeField]
		private float _shadowAlphaEffectDelay = 0.5f;

		[Tooltip("A curve which controls the shadow expansion effect")]
		[SerializeField]
		private AnimationCurve _shadowExpansionEffectCurve;

		[SerializeField]
		private float _shadowExpansionEffectDuration = 0.5f;

		[SerializeField]
		private Vector3 _shadowExpansionEffectAmount = new Vector3(0.1f, 0.1f, 0.1f);

		[SerializeField]
		private float _shadowExpansionEffectDelay = 0.5f;

		[SerializeField]
		private ParticleSystem _starBurstVFX;

		[SerializeField]
		private float _blinkEffectDuration = 0.05f;

		private Transform _icon;

		private UILabel _blinkImage;

		public void Setup(EventsPageTopPayload payload)
		{
			_mainTitle.text = payload.mainTitle;
			_subtitle.text = payload.subTitle;
			_mainTitle.color = payload.mainColor;
			_subtitle.color = payload.subTitleColor;
			_bubbleSplat.color = payload.splatColor;
			_starSplat.color = payload.splatColor;
			_bubbleSplatShadow.color = payload.splatShadowColor;
			_starSplatShadow.color = payload.splatShadowColor;
			UILabel multiplierText = _multiplierText;
			string multiplierText2 = payload.multiplierText;
			_shadowAnimText.text = multiplierText2;
			multiplierText.text = multiplierText2;
			_bubbleSplat.gameObject.SetActive(payload.topSplatType == TopSplatType.Bubble);
			_starSplat.gameObject.SetActive(payload.topSplatType == TopSplatType.Star);
			ResetAnimations();
		}

		public GameObject MakeSlotItem(Func<UIWidget, GameObject> maker)
		{
			if ((bool)_slotForItemLoad)
			{
				return maker(_slotForItemLoad);
			}
			Debug.LogError("No slot for item" + maker);
			return null;
		}

		public void MakeIcon(TopIconPayload payload)
		{
			if (_icon != null)
			{
				UnityEngine.Object.Destroy(_icon);
			}
			_icon = UnityEngine.Object.Instantiate(payload.iconPrefab, _iconAnchor, false).transform;
			Timer.ExecuteNextFrame(() =>
			{
				_icon.gameObject.SetLayerRecursively(_iconAnchor.gameObject.layer);
				UIWidget iconGuideWidget = _iconGuideWidget;
				_icon.FitHeightToWidget(iconGuideWidget);
				_icon.localPosition = Vector3.zero;
				_icon.OffsetToCenter();
				AdjustIcon(payload.localPositionOffset, payload.localRotationOffsetEuler, payload.localScaleOffset);
			});
			PlaceholderLifecycle componentInChildren = _icon.GetComponentInChildren<PlaceholderLifecycle>();
			if (componentInChildren != null)
			{
				componentInChildren.Spawn();
			}
		}

		private void AdjustIcon(Vector3 localPositionOffset, Vector3 localRotationOffsetEuler, Vector3 localScaleOffset)
		{
			Transform icon = _icon;
			icon.localPosition += localPositionOffset;
			icon.localRotation *= Quaternion.Euler(localRotationOffsetEuler);
			icon.localScale = Vector3.Scale(icon.localScale, localScaleOffset);
		}

		public void UpdateMultiplierText(string newText, bool useEffect = false)
		{
			if (!(_multiplierText != null))
			{
				return;
			}
			if (useEffect)
			{
				Timer.ExecuteWithDelay(_shadowAlphaEffectDelay, () =>
				{
					UILabel multiplierText2 = _multiplierText;
					string text2 = newText;
					_shadowAnimText.text = text2;
					multiplierText2.text = text2;
				});
				Timer.ExecuteWithDelay(_iconScaleEffectDelay, () =>
				{
					StartCoroutine(AnimateIconScale());
				});
				Timer.ExecuteWithDelay(_shadowAlphaEffectDelay, () =>
				{
					StartCoroutine(AnimateShadowAlpha());
				});
				Timer.ExecuteWithDelay(_shadowExpansionEffectDelay, () =>
				{
					StartCoroutine(AnimateShadowExpansion());
				});
			}
			else
			{
				UILabel multiplierText = _multiplierText;
				string text = newText;
				_shadowAnimText.text = text;
				multiplierText.text = text;
			}
		}

		[Button(null)]
		public void PlayMultiplierAnimations()
		{
			Delay(_iconScaleEffectDelay + _iconScaleEffectDuration, () =>
			{
				string text = _multiplierText.text;
				int num = Convert.ToInt32(MissionsManager.Instance.currentMissionSet);
				int num2 = ((num < 30) ? (num + 1) : 30);
				string text2 = "x" + num2;
				UILabel multiplierText = _multiplierText;
				string text3 = text2;
				_shadowAnimText.text = text3;
				multiplierText.text = text3;
				_blinkImage = UnityEngine.Object.Instantiate(_shadowAnimText, _shadowAnimText.transform.parent);
				_blinkImage.depth++;
				_blinkImage.color = new Color(1f, 1f, 1f, 1f);
				_blinkImage.fontSize = 43;
				_blinkImage.transform.localScale = Vector3.one;
				_blinkImage.alpha = 0.5f;
				_blinkImage.gameObject.SetActive(true);
				if (_starBurstVFX.gameObject.activeInHierarchy)
				{
					_starBurstVFX.gameObject.SetActive(false);
				}
				_starBurstVFX.gameObject.SetActive(true);
				StartCoroutine(FadeOutBlinkImage());
			});
			Delay(_iconScaleEffectDelay, () =>
			{
				StartCoroutine(AnimateIconScale());
			});
			Delay(_shadowAlphaEffectDelay, () =>
			{
				StartCoroutine(AnimateShadowAlpha());
			});
			Delay(_shadowExpansionEffectDelay, () =>
			{
				StartCoroutine(AnimateShadowExpansion());
			});
		}

		private void Delay(float delay, Action action)
		{
			StartCoroutine(DelayCoroutine(delay, action));
		}

		private IEnumerator DelayCoroutine(float delay, Action action)
		{
			yield return new WaitForSecondsRealtime(delay);
			action.SafeInvoke();
		}

		public float GetMultiplierAnimationDuration()
		{
			return _iconScaleEffectDelay + _iconScaleEffectDuration + _blinkEffectDuration;
		}

		private IEnumerator FadeOutBlinkImage()
		{
			float currentDuration = 0f;
			float startAlpha = _blinkImage.alpha;
			while (currentDuration <= _blinkEffectDuration)
			{
				float p = currentDuration / _blinkEffectDuration;
				float plot = Easings.QuadraticEaseOut(p);
				_blinkImage.alpha = Mathf.Lerp(startAlpha, 0f, plot);
				currentDuration += Time.unscaledDeltaTime;
				yield return null;
			}
			UnityEngine.Object.Destroy(_blinkImage.gameObject);
		}

		private IEnumerator AnimateIconScale()
		{
			float currentDuration = 0f;
			while (currentDuration <= _iconScaleEffectDuration)
			{
				currentDuration += Time.unscaledDeltaTime;
				float p = currentDuration / _iconScaleEffectDuration;
				float plot = _iconScaleEffectCurve.Evaluate(p);
				_multiplierText.transform.localScale = Vector3.one + plot * _iconScaleEffectAmount;
				yield return null;
			}
		}

		private IEnumerator AnimateShadowAlpha()
		{
			float currentDuration = 0f;
			_shadowAnimText.gameObject.SetActive(true);
			while (currentDuration <= _shadowAlphaEffectDuration)
			{
				float p = currentDuration / _shadowAlphaEffectDuration;
				float plot = _shadowAlphaEffectCurve.Evaluate(p);
				_shadowAnimText.alpha = plot * _shadowAlphaEffectAmount;
				currentDuration += Time.unscaledDeltaTime;
				yield return null;
			}
			_shadowAnimText.alpha = 0f;
		}

		private IEnumerator AnimateShadowExpansion()
		{
			float currentDuration = 0f;
			_shadowAnimText.gameObject.SetActive(true);
			while (currentDuration <= _shadowExpansionEffectDuration)
			{
				float p = currentDuration / _shadowExpansionEffectDuration;
				float plot = _shadowExpansionEffectCurve.Evaluate(p);
				_shadowAnimText.transform.localScale = Vector3.one + plot * _shadowExpansionEffectAmount;
				currentDuration += Time.unscaledDeltaTime;
				yield return null;
			}
			_shadowAnimText.transform.localScale = Vector3.one;
		}

		[Button(null)]
		private void ResetAnimations()
		{
			_multiplierText.transform.localScale = Vector3.one;
			_shadowAnimText.gameObject.SetActive(false);
			_shadowAnimText.transform.localScale = Vector3.one;
			_starBurstVFX.gameObject.SetActive(false);
			_shadowAnimText.alpha = 1f;
			if (_blinkImage != null)
			{
				UnityEngine.Object.Destroy(_blinkImage.gameObject);
			}
		}
	}
}
