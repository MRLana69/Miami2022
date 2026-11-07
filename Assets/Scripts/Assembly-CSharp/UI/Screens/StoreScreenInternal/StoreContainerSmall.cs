using System;
using System.Collections;
using System.Collections.Generic;
using Kiloo.Common.Extensions;
using NaughtyAttributes;
using UI.Generic;
using UI.Generic.Layout;
using UI.RenderTexture;
using UnityEngine;

namespace UI.Screens.StoreScreenInternal
{
	public class StoreContainerSmall : MonoBehaviour
	{
		public class StoreContainerSmallPayload
		{
			public Color frameTopColor;

			public Color frameBottomColor;

			public Color fillTopColor;

			public Color fillBottomColor;

			public Color valueColor;

			public Color valueEffectColor;

			public Color lineThroughColor;

			public Color lineThroughShadowColor;

			public Color oldValueColor;

			public Color oldValueEffectColor;

			public Color bannerTopColor;

			public Color bannerBottomColor;

			public string iconName;

			public string valueText;

			public string priceText;

			public string oldValueText;

			public string priceTypeIconName;

			public bool showMostPopular;

			public bool showSale;

			public bool clickable = true;

			public bool showAdIcon;

			public bool showEventSubIcon;

			public bool showcheckedSubIcon;

			public bool showTimerIcon;

			public bool showStars;

			public Vector3[] starGlintPositions;

			public Action onButtonClicked;

			public Action onInfoButtonClicked;

			public Action<StoreContainerSmall> onContainerShouldRefresh;
		}

		[SerializeField]
		private UISprite _frame;

		[SerializeField]
		private UISprite _fill;

		[SerializeField]
		private UISprite _icon;

		[SerializeField]
		private FitSpriteToWidget _iconFitter;

		[SerializeField]
		private UISprite _subIconCheckMark;

		[SerializeField]
		private UISprite _subIconEventIcon;

		[SerializeField]
		private UILabel _valueLabel;

		[SerializeField]
		private Layout _buttonLayout;

		[SerializeField]
		private AlignTimer _alignTimer;

		[SerializeField]
		private GameObject _clockIcon;

		[SerializeField]
		private UILabel _priceLabel;

		[SerializeField]
		private UISprite _priceIcon;

		[SerializeField]
		private GameObject _adIcon;

		[SerializeField]
		private FitSpriteToWidget _priceIconFitter;

		[SerializeField]
		private UIButtonOverlayOff _buttonStateColor;

		[SerializeField]
		private UILabel _mostPopularLabel;

		[SerializeField]
		private UISprite _mostPopularBackground;

		[SerializeField]
		private GameObject _mostPopularSection;

		[SerializeField]
		private Layout _mostPopularLayout;

		[SerializeField]
		private GameObject _starGlintPrefab;

		[SerializeField]
		private float _starAnimationPause;

		[SerializeField]
		private Layout _valueLayout;

		[SerializeField]
		private GameObject _oldValueSection;

		[SerializeField]
		private UILabel _oldValueLabel;

		[SerializeField]
		private UISprite _lineThrough;

		[SerializeField]
		private UISprite _lineThroughShadow;

		[SerializeField]
		private GameObject _infoIcon;

		[SerializeField]
		private RenderTextureObject _starObject;

		[SerializeField]
		private UITexture _starTexture;

		private DefaultRenderTextureSetup _backgroundStars;

		private StoreContainerSmallPayload _payload;

		private List<GameObject> _starGlints = new List<GameObject>();

		private IEnumerator _starAnimation;

		public StoreContainerSmallPayload payload
		{
			get
			{
				return _payload;
			}
		}

		public void Setup(StoreContainerSmallPayload payload)
		{
			_payload = payload;
			_frame.gradientTop = payload.frameTopColor;
			_frame.gradientBottom = payload.frameBottomColor;
			_fill.gradientTop = payload.fillTopColor;
			_fill.gradientBottom = payload.fillBottomColor;
			_valueLabel.color = payload.valueColor;
			_valueLabel.effectColor = payload.valueEffectColor;
			_clockIcon.SetActive(payload.showTimerIcon);
			if (_payload.showStars)
			{
				_starTexture.gameObject.SetActive(true);
				_backgroundStars = new DefaultRenderTextureSetup();
				_backgroundStars.Setup(_starTexture, new List<RenderTextureObject> { _starObject }, new CameraSettings
				{
					backgroundColor = new Color(1f, 1f, 1f, 0f),
					nearClipPlane = 0.5f,
					farClipPlane = 2f
				});
			}
			else if (_backgroundStars != null)
			{
				_starTexture.gameObject.SetActive(false);
			}
			_icon.spriteName = payload.iconName;
			_iconFitter.FitNow();
			_subIconCheckMark.gameObject.SetActive(payload.showcheckedSubIcon);
			_subIconEventIcon.gameObject.SetActive(payload.showEventSubIcon);
			_valueLabel.text = payload.valueText;
			_priceLabel.text = payload.priceText;
			_adIcon.SetActive(payload.showAdIcon);
			_priceIconFitter.gameObject.SetActive(!payload.priceTypeIconName.IsNullOrEmpty());
			_priceIcon.spriteName = payload.priceTypeIconName;
			_priceIconFitter.FitNow();
			_buttonLayout.Align(true);
			UpdateButtonColor();
			if (_payload.starGlintPositions != null && _payload.starGlintPositions.Length > 0)
			{
				Vector3[] starGlintPositions = _payload.starGlintPositions;
				foreach (Vector3 localPosition in starGlintPositions)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(_starGlintPrefab, _iconFitter.transform);
					gameObject.transform.localPosition = localPosition;
					_starGlints.Add(gameObject);
				}
				_starAnimation = ActivateStars();
				StartCoroutine(_starAnimation);
			}
			_oldValueSection.SetActive(payload.showSale);
			if (payload.showSale)
			{
				_oldValueLabel.color = payload.oldValueColor;
				_oldValueLabel.effectColor = payload.oldValueEffectColor;
				_lineThrough.color = payload.lineThroughColor;
				_lineThroughShadow.color = payload.lineThroughShadowColor;
				_oldValueLabel.text = payload.oldValueText;
			}
			_valueLayout.Align();
			_mostPopularSection.SetActive(payload.showMostPopular);
			if (payload.showMostPopular)
			{
				_mostPopularBackground.gradientTop = payload.bannerTopColor;
				_mostPopularBackground.gradientBottom = payload.bannerBottomColor;
				_mostPopularLabel.text = Strings.Get("MOST_POPULAR_STICKER_TITLE").ToUpper();
				_mostPopularLayout.Align();
			}
			_infoIcon.SetActive(payload.onInfoButtonClicked != null);
		}

		private void UpdateButtonColor()
		{
			if (_payload != null)
			{
				if (_payload.clickable)
				{
					_buttonStateColor.GoToNormalState();
				}
				else
				{
					_buttonStateColor.GoToNotAvailableState();
				}
			}
		}

		private IEnumerator ActivateStars()
		{
			yield return new WaitForSeconds(2f);
			while (true)
			{
				foreach (GameObject starGlint in _starGlints)
				{
					int childCount = starGlint.transform.childCount;
					for (int i = 0; i < childCount; i++)
					{
						starGlint.transform.GetChild(i).GetComponent<Animation>().Play();
					}
				}
				yield return new WaitForSeconds(_starAnimationPause);
			}
		}

		private void OnEnable()
		{
			UpdateButtonColor();
			if (_starAnimation != null)
			{
				StartCoroutine(_starAnimation);
			}
		}

		private void OnDisable()
		{
			if (_starAnimation != null)
			{
				StopCoroutine(_starAnimation);
			}
		}

		public void SetButtonTimeLeft(TimeSpan time)
		{
			_alignTimer.SetTimeRemaining(time);
		}

		private void Update()
		{
			_payload.onContainerShouldRefresh.SafeInvoke(this);
		}

		[Button(null)]
		private void SetupBasic()
		{
			ColorData instance = ColorData.instance;
			_priceLabel.color = instance.white;
			_priceLabel.effectColor = instance.shadow;
		}

		public void OnButtonClicked()
		{
			_payload.onButtonClicked.SafeInvoke();
		}

		public void OnInfoButtonClicked()
		{
			_payload.onInfoButtonClicked.SafeInvoke();
		}
	}
}
