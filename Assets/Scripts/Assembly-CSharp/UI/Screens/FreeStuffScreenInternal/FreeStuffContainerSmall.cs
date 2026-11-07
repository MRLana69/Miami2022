using System.Collections.Generic;
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
	public class FreeStuffContainerSmall : MonoBehaviour
	{
		private enum ButtonState
		{
			Loading = 0,
			NoVideo = 1,
			VideoReady = 2
		}

		[SerializeField]
		private UISprite _backgroundSprite;

		[SerializeField]
		private UISprite _outlineSprite;

		[SerializeField]
		private UISprite _glowSprite;

		[SerializeField]
		private UISprite _iconSprite;

		[SerializeField]
		private UILabel _titleLabel;

		[SerializeField]
		private UILabel _descriptionLabel;

		[SerializeField]
		private UILabel _buttonLabel;

		[SerializeField]
		private HorizontalLayout _buttonLayout;

		[SerializeField]
		private GameObject _tvIconWidget;

		[SerializeField]
		private GameObject _loadingSpinner;

		[SerializeField]
		private GameObject _noConnectionSprite;

		[SerializeField]
		private UIButtonOverlayOff _buttonOverlayOff;

		[SerializeField]
		private RenderTextureObject _starObject;

		[SerializeField]
		private UITexture _starTexture;

		[SerializeField]
		private Animator _tvAnimator;

		private SmallFreeStuffContainerPayload _payload;

		private RewardForVideoButtonController _videoHandler;

		private ButtonState _buttonState = ButtonState.NoVideo;

		public void Setup(SmallFreeStuffContainerPayload payload)
		{
			_payload = payload;
			SetupContainer();
			FreeStuffScreen.tvIconsSmall.Add(this);
			_videoHandler = new RewardForVideoButtonController(_payload.adPayload);
			_videoHandler.setButtonToActiveState = SetActiveState;
			_videoHandler.setButtonToLoadingState = SetLoadingState;
			_videoHandler.setButtonToInactiveState = SetInactiveState;
			_payload.clickAction = () =>
			{
				_videoHandler.OnClick();
			};
			_videoHandler.Init();
			FreeStuffScreen.onGainFocus += AlignText;
		}

		private void AlignText()
		{
			_buttonLayout.Align();
		}

		private void OnDestroy()
		{
			_videoHandler.setButtonToActiveState = null;
			_videoHandler.setButtonToLoadingState = null;
			_videoHandler.setButtonToInactiveState = null;
			FreeStuffScreen.onGainFocus -= AlignText;
		}

		private void SetupContainer()
		{
			AddColors();
			_iconSprite.spriteName = _payload.iconSpriteName;
			_iconSprite.GetComponent<FitSpriteToWidget>().FitNow();
			_titleLabel.text = Strings.Get(_payload.headerString);
			_descriptionLabel.text = _payload.descriptionString;
			_buttonLabel.text = Strings.Get(_payload.buttonString);
			_buttonLayout.Align();
			DefaultRenderTextureSetup defaultRenderTextureSetup = new DefaultRenderTextureSetup();
			defaultRenderTextureSetup.Setup(_starTexture, new List<RenderTextureObject> { _starObject }, new CameraSettings
			{
				backgroundColor = new Color(1f, 1f, 1f, 0f),
				nearClipPlane = 0.5f,
				farClipPlane = 2f
			});
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
					_tvIconWidget.SetActive(true);
					_buttonLabel.text = Strings.Get(_payload.buttonString);
					FreeStuffScreen.updateTvIcons = true;
					break;
				case ButtonState.Loading:
					_buttonOverlayOff.GoToNotAvailableState();
					_loadingSpinner.SetActive(true);
					_noConnectionSprite.SetActive(false);
					_tvIconWidget.SetActive(false);
					_buttonLabel.text = Strings.Get(_payload.buttonLoadingString);
					break;
				case ButtonState.NoVideo:
					_buttonOverlayOff.GoToNotAvailableState();
					_loadingSpinner.SetActive(false);
					_noConnectionSprite.SetActive(true);
					_tvIconWidget.SetActive(false);
					_buttonLabel.text = ((Application.internetReachability != NetworkReachability.NotReachable) ? Strings.Get("FREE_STUFF_NO_VIDEO_SMALL_CELL") : Strings.Get("FREE_STUFF_NO_CONNECTION"));
					break;
				}
				_buttonLayout.Align();
			}
		}
	}
}
