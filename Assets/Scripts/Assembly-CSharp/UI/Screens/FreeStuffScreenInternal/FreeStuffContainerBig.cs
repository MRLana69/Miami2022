using System.Collections;
using System.Collections.Generic;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using UI.FreeStuff;
using UI.Generic;
using UI.Generic.Layout;
using UI.RenderTexture;
using UnityEngine;

namespace UI.Screens.FreeStuffScreenInternal
{
	public class FreeStuffContainerBig : MonoBehaviour
	{
		private enum ButtonState
		{
			Loading = 0,
			NoVideo = 1,
			VideoReady = 2
		}

		[Header("Basic")]
		[SerializeField]
		private UISprite _backgroundSprite;

		[SerializeField]
		private UISprite _outlineSprite;

		[SerializeField]
		private UISprite _glowSprite;

		[SerializeField]
		private UISprite _iconSprite;

		[SerializeField]
		private GameObject _dinoTexture;

		[SerializeField]
		private UIWidget _iconWidget;

		[Header("Title and Description")]
		[SerializeField]
		private UILabel _titleLabel;

		[SerializeField]
		private UILabel _descriptionLabel;

		[SerializeField]
		private UILabel _buttonLabel;

		[Header("Button")]
		[SerializeField]
		private HorizontalLayout _buttonLayout;

		[SerializeField]
		private GameObject _facebookIcon;

		[SerializeField]
		private GameObject _tvIconWidget;

		[SerializeField]
		private GameObject _loadingSpinner;

		[SerializeField]
		private GameObject _noConnectionSprite;

		[SerializeField]
		private GameObject _coinSprites;

		[SerializeField]
		private UIButtonOverlayOff _buttonOverlayOff;

		[Header("Stars")]
		[SerializeField]
		private List<GameObject> _starPosition;

		[SerializeField]
		private GameObject _glintStarPrefab;

		[SerializeField]
		private float _starAnimationPause;

		[SerializeField]
		private float _starCountDown;

		[SerializeField]
		private RenderTextureObject _starObject;

		[SerializeField]
		private UITexture _starTexture;

		[SerializeField]
		private Animator _tvAnimator;

		private List<GameObject> _stars = new List<GameObject>();

		private bool _runAnimations;

		private BigFreeStuffContainerPayload _payload;

		private RewardForVideoButtonController _videoHandler;

		private ButtonState _buttonState = ButtonState.NoVideo;

		private const int ICON_WIDGET_SIZE_SMALL = 65;

		private const int ICON_WIDGET_SIZE_BIG = 85;

		private void OnEnable()
		{
			if (_payload != null)
			{
				SetupStars();
			}
		}

		private void OnDisable()
		{
			_runAnimations = false;
		}

		private void OnDestroy()
		{
			if (_videoHandler != null)
			{
				_videoHandler.setButtonToActiveState = null;
				_videoHandler.setButtonToLoadingState = null;
				_videoHandler.setButtonToInactiveState = null;
			}
			FreeStuffScreen.onGainFocus -= AlignText;
		}

		private void SetupIconStars()
		{
			DefaultRenderTextureSetup defaultRenderTextureSetup = new DefaultRenderTextureSetup();
			defaultRenderTextureSetup.Setup(_starTexture, new List<RenderTextureObject> { _starObject }, new CameraSettings
			{
				backgroundColor = new Color(1f, 1f, 1f, 0f),
				nearClipPlane = 0.5f,
				farClipPlane = 2f
			});
		}

		public void Setup(BigFreeStuffContainerPayload payload)
		{
			_payload = payload;
			SetupContainer();
			if (payload.containerType == BigShopContainerType.FacebookLogin)
			{
				FreeStuffFBManager.instance.setButtonToActiveState = SetActiveState;
				FreeStuffFBManager.instance.setButtonToLoadingState = SetLoadingState;
				SetupIconStars();
			}
			else if (payload.containerType == BigShopContainerType.FacebookGift)
			{
				SetupIconStars();
			}
			else if (payload.containerType == BigShopContainerType.FreeCoins)
			{
				FreeStuffScreen.tvIconsBig.Add(this);
				_videoHandler = new RewardForVideoButtonController(_payload.adPayload);
				_videoHandler.setButtonToActiveState = SetActiveState;
				_videoHandler.setButtonToLoadingState = SetLoadingState;
				_videoHandler.setButtonToInactiveState = SetInactiveState;
				_payload.clickAction = () =>
				{
					_videoHandler.OnClick();
				};
				_videoHandler.Init();
			}
			FreeStuffScreen.onGainFocus += AlignText;
		}

		private void AlignText()
		{
			_buttonLayout.Align();
		}

		private void SetupContainer()
		{
			BigShopContainerType containerType = _payload.containerType;
			AddColors();
			AddText();
			SetupStars();
			_iconSprite.gameObject.SetActive(_payload.containerType != BigShopContainerType.FacebookLogin);
			_dinoTexture.SetActive(_payload.containerType == BigShopContainerType.FacebookLogin);
			if (!string.IsNullOrEmpty(_payload.iconSpriteName))
			{
				_iconWidget.height = ((!_payload.useBigIconSize) ? 65 : 85);
				_iconSprite.spriteName = _payload.iconSpriteName;
				_iconSprite.GetComponent<FitSpriteToWidget>().FitNow();
			}
			_facebookIcon.SetActive(containerType == BigShopContainerType.FacebookLogin);
			_coinSprites.SetActive(containerType == BigShopContainerType.FacebookLogin);
			_tvIconWidget.SetActive(containerType == BigShopContainerType.FreeCoins);
			_buttonLayout.Align();
		}

		private void AddColors()
		{
			_backgroundSprite.gradientTop = _payload.bgGradientTopColor;
			_backgroundSprite.gradientBottom = _payload.bgGradientBottomColor;
			_outlineSprite.gradientTop = _payload.outlineGradientTopColor;
			_outlineSprite.gradientBottom = _payload.outlineGradientBottomColor;
			_glowSprite.color = _payload.glowColor;
			_titleLabel.color = _payload.titleTextColor;
			_titleLabel.effectColor = _payload.titleTextShadow;
			_descriptionLabel.color = _payload.descriptionTextColor;
			_descriptionLabel.effectColor = _payload.descriptionTextShadow;
		}

		private void AddText()
		{
			_titleLabel.text = Strings.Get(_payload.headerString);
			if (_payload.containerType == BigShopContainerType.FacebookLogin)
			{
				_descriptionLabel.text = string.Format(Strings.Get(_payload.descriptionString), FacebookRewardsInfo.facebookRewardPack[FacebookRewardsInfo.FacebookRewardType.Coins]);
			}
			else
			{
				_descriptionLabel.text = Strings.Get(_payload.descriptionString);
			}
			_buttonLabel.text = Strings.Get(_payload.buttonString);
		}

		private void SetupStars()
		{
			if (_payload.containerType == BigShopContainerType.FacebookLogin && _starPosition.Count > 0 && !_runAnimations)
			{
				_runAnimations = true;
				if (base.gameObject.activeInHierarchy)
				{
					StartCoroutine(ActivateStars());
				}
			}
		}

		[UsedImplicitly]
		public void OnButtonClick()
		{
			_payload.clickAction.SafeInvoke();
		}

		private void SetActiveState()
		{
			SetButtonState(ButtonState.VideoReady);
		}

		private void SetLoadingState()
		{
			SetButtonState(ButtonState.Loading);
		}

		private void SetInactiveState()
		{
			SetButtonState(ButtonState.NoVideo);
		}

		public void RefreshVideoLogo()
		{
			if (_buttonState == ButtonState.VideoReady)
			{
				_tvAnimator.SetBool("Pause", true);
				Timer.ExecuteWithDelay(0.5f, () =>
				{
					_tvAnimator.SetBool("Pause", false);
				});
			}
		}

		private void SetButtonState(ButtonState state)
		{
			if (_buttonState != state)
			{
				_buttonState = state;
				switch (state)
				{
				case ButtonState.VideoReady:
					_buttonOverlayOff.GoToNormalState();
					_loadingSpinner.SetActive(false);
					_noConnectionSprite.SetActive(false);
					_tvIconWidget.SetActive(_payload.containerType == BigShopContainerType.FreeCoins);
					_facebookIcon.SetActive(_payload.containerType == BigShopContainerType.FacebookLogin);
					_buttonLabel.text = Strings.Get(_payload.buttonString);
					FreeStuffScreen.updateTvIcons = true;
					break;
				case ButtonState.Loading:
					_buttonOverlayOff.GoToNotAvailableState();
					_loadingSpinner.SetActive(true);
					_noConnectionSprite.SetActive(false);
					_tvIconWidget.SetActive(false);
					_facebookIcon.SetActive(false);
					_buttonLabel.text = Strings.Get(_payload.buttonLoadingString);
					break;
				case ButtonState.NoVideo:
					_buttonOverlayOff.GoToNotAvailableState();
					_loadingSpinner.SetActive(false);
					_noConnectionSprite.SetActive(true);
					_tvIconWidget.SetActive(false);
					_facebookIcon.SetActive(false);
					_buttonLabel.text = ((Application.internetReachability != NetworkReachability.NotReachable) ? Strings.Get("EARN_COINS_LOADING_FAILED") : Strings.Get("FREE_STUFF_NO_CONNECTION"));
					break;
				}
				_buttonLayout.Align();
			}
		}

		private IEnumerator ActivateStars()
		{
			if (_stars.Count == 0)
			{
				foreach (GameObject item in _starPosition)
				{
					_stars.Add(Object.Instantiate(_glintStarPrefab, item.transform.position, Quaternion.Euler(0f, 0f, 0f), item.transform));
				}
			}
			yield return new WaitForSeconds(2f);
			while (_runAnimations)
			{
				foreach (GameObject star in _stars)
				{
					int children = star.transform.childCount;
					for (int i = 0; i < children; i++)
					{
						star.transform.GetChild(i).GetComponent<Animation>().Play();
					}
					yield return new WaitForSeconds(_starCountDown);
				}
				yield return new WaitForSeconds(_starAnimationPause);
			}
		}
	}
}
