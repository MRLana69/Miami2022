using Data.Save;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Utility;
using NaughtyAttributes;
using TopRun;
using UnityEngine;

namespace UI.Screens.FrontScreenInternal
{
	public class GuidelineButton : MonoBehaviour
	{
		public enum GuidelineButtonType
		{
			NotSet = 0,
			Hoverboard = 1,
			FacebookOrCollect = 2,
			Mission1 = 3,
			PlayerMenuIntroduction = 4,
			ShopMenuIntroduction = 5
		}

		private enum AnimationState
		{
			None = 0,
			Scaling = 1,
			Blinking = 2,
			Waiting = 3
		}

		[SerializeField]
		private UISprite _infoSprite;

		[SerializeField]
		private UISprite _infoSpriteOverlay;

		[SerializeField]
		private GameObject _breadcrumb;

		[SerializeField]
		private GameObject _discount;

		private bool _showIcon;

		public GuidelineButtonType guidelineButtonType;

		private bool _hasInited;

		private const float SCALE_DURATION = 1f;

		private const float WAIT_DURATION = 2f;

		private const float BLINK_DURATION = 1f;

		private readonly Vector3 _startScale = Vector3.zero;

		private readonly Vector3 _endScale = Vector3.one;

		private AnimationState _animationState;

		private float _currentAnimationTime;

		private void Awake()
		{
			_infoSprite.gameObject.SetActive(false);
			_infoSpriteOverlay.gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			if (!_hasInited)
			{
				InitAnimation();
				_hasInited = true;
			}
			_infoSprite.gameObject.SetActive(false);
			_infoSpriteOverlay.gameObject.SetActive(false);
			DetermineVisibility();
		}

		public void DetermineVisibility()
		{
			bool flag = ShouldInfoIconShow();
			if (flag)
			{
				EnableInfoIcon();
			}
			if (_discount != null && flag)
			{
				_discount.SetActive(false);
			}
			if (_breadcrumb != null)
			{
				_breadcrumb.SetActive(!flag);
			}
		}

		[UsedImplicitly]
		private void OnClick()
		{
			if (_showIcon)
			{
				_showIcon = false;
				_infoSprite.enabled = false;
				_infoSpriteOverlay.enabled = false;
				_infoSpriteOverlay.alpha = 0f;
				_animationState = AnimationState.None;
				if (guidelineButtonType == GuidelineButtonType.FacebookOrCollect && OnboardingSave.shouldShowCollectPopup)
				{
					UIScreenController.Instance.PushScreen("FriendsUI");
					UIScreenController.Instance.QueuePopup("TutorialCollectFromFriendsPopup");
					OnboardingSave.hasShownCollectPopup = true;
					OnboardingSave.shouldShowCollectPopup = false;
				}
				else if (guidelineButtonType == GuidelineButtonType.FacebookOrCollect && OnboardingSave.shouldShowFacebookPopup && !AgeRestriction.isRestricted)
				{
					UIScreenController.Instance.PushScreen("FriendsUI");
					UIScreenController.Instance.QueuePopup("TutorialFacebookPopup");
					OnboardingSave.hasShownFacebookPopup = true;
					OnboardingSave.shouldShowFacebookPopup = false;
				}
				else if (guidelineButtonType == GuidelineButtonType.FacebookOrCollect && OnboardingSave.lastShownTopRunIntroPopupVersionNumber != 2 && !TopRunMaintenance.IsActive())
				{
					UIScreenController.Instance.PushScreen("TopRunUI");
					UIScreenController.Instance.QueuePopup("TopRunPopup");
					OnboardingSave.lastShownTopRunIntroPopupVersionNumber = 2;
				}
				else if (guidelineButtonType == GuidelineButtonType.Hoverboard)
				{
					UIScreenController.Instance.QueuePopup("TutorialHoverboardsPopup");
					OnboardingSave.hasShownHoverboardPopup = true;
					OnboardingSave.shouldShowHoverboardPopup = false;
				}
				else if (guidelineButtonType == GuidelineButtonType.Mission1)
				{
					if (OnboardingSave.shouldShowMissionIntroPopup)
					{
						UIScreenController.Instance.QueuePopup("TutorialMissionPopup");
						OnboardingSave.hasShownMissionIntroPopup = true;
						OnboardingSave.shouldShowMissionIntroPopup = false;
					}
					else
					{
						UIScreenController.Instance.PushScreen("ChallengesScreen");
					}
				}
				else if (guidelineButtonType == GuidelineButtonType.NotSet)
				{
					SDebug.LogError("A guideline button wiggler is not set properly.", base.gameObject);
				}
			}
			else if (guidelineButtonType == GuidelineButtonType.FacebookOrCollect)
			{
				UIScreenController.Instance.PushScreen(UIScreens.friendsMenu_lastScreen);
				Flurry.LogEventWithAParameter("FrontScreenButtonWithFooterClicked", "Screen Name", UIScreens.friendsMenu_lastScreen);
			}
			else if (guidelineButtonType == GuidelineButtonType.Hoverboard)
			{
				UIScreenController.Instance.QueuePopup("ConsumablesPopup");
			}
			else if (guidelineButtonType == GuidelineButtonType.Mission1)
			{
				UIScreenController.Instance.PushScreen("ChallengesScreen");
				Flurry.LogEventWithAParameter("FrontScreenButtonWithFooterClicked", "Screen Name", ((ChallengesScreen.PageIndex)ChallengesScreen.GetPageIndex()/*cast due to constrained. prefix*/).ToString());
			}
			else if (guidelineButtonType == GuidelineButtonType.PlayerMenuIntroduction)
			{
				UIScreenController.Instance.PushScreen(UIScreens.meMenu_lastScreen);
				Flurry.LogEventWithAParameter("FrontScreenButtonWithFooterClicked", "Screen Name", UIScreens.meMenu_lastScreen);
			}
			else if (guidelineButtonType == GuidelineButtonType.ShopMenuIntroduction)
			{
				UIScreenController.Instance.PushScreen(UIScreens.shopMenu_lastScreen);
				Flurry.LogEventWithAParameter("FrontScreenButtonWithFooterClicked", "Screen Name", UIScreens.shopMenu_lastScreen);
			}
		}

		public bool ShouldInfoIconShow()
		{
			bool result = false;
			if (guidelineButtonType == GuidelineButtonType.Hoverboard && OnboardingSave.shouldShowHoverboardPopup && !OnboardingSave.hasShownHoverboardPopup)
			{
				result = true;
			}
			else if (guidelineButtonType == GuidelineButtonType.FacebookOrCollect && OnboardingSave.lastShownTopRunIntroPopupVersionNumber != 2 && !TopRunMaintenance.IsActive() && !UserSave.isFreshInstall)
			{
				result = true;
			}
			else if (guidelineButtonType == GuidelineButtonType.FacebookOrCollect && OnboardingSave.shouldShowFacebookPopup && !OnboardingSave.hasShownFacebookPopup && !AgeRestriction.isRestricted)
			{
				if (OnboardingSave.shouldShowHoverboardPopup)
				{
				}
				result = true;
			}
			else if (guidelineButtonType == GuidelineButtonType.FacebookOrCollect && OnboardingSave.shouldShowCollectPopup && !OnboardingSave.hasShownCollectPopup && !AgeRestriction.isRestricted)
			{
				if (OnboardingSave.shouldShowHoverboardPopup || OnboardingSave.shouldShowFacebookPopup)
				{
				}
				result = true;
			}
			else if (guidelineButtonType == GuidelineButtonType.Mission1)
			{
				if (OnboardingSave.shouldShowMissionIntroPopup && !OnboardingSave.hasShownMissionIntroPopup)
				{
					if (OnboardingSave.shouldShowHoverboardPopup || OnboardingSave.shouldShowFacebookPopup || OnboardingSave.shouldShowCollectPopup)
					{
					}
					result = true;
				}
			}
			else if (guidelineButtonType == GuidelineButtonType.NotSet)
			{
				SDebug.LogError("A guideline button wiggler is not set properly.", base.gameObject);
			}
			return result;
		}

		[Button(null)]
		private void ForceAnimation()
		{
			InitAnimation();
			_animationState = AnimationState.Scaling;
			_currentAnimationTime = 0f;
		}

		private void EnableInfoIcon()
		{
			_showIcon = true;
			_infoSprite.gameObject.SetActive(true);
			_infoSpriteOverlay.gameObject.SetActive(true);
			if (_animationState == AnimationState.None)
			{
				if (!PlayerPrefs.HasKey(string.Concat(guidelineButtonType, "hasScaled")))
				{
					_animationState = AnimationState.Scaling;
					_currentAnimationTime = 0f;
					PlayerPrefs.SetInt(string.Concat(guidelineButtonType, "hasScaled"), 1);
				}
				else if (guidelineButtonType == GuidelineButtonType.FacebookOrCollect && PlayerPrefs.GetInt(string.Concat(guidelineButtonType, "hasScaled")) == 1)
				{
					_animationState = AnimationState.Scaling;
					_currentAnimationTime = 0f;
					PlayerPrefs.SetInt(string.Concat(guidelineButtonType, "hasScaled"), 2);
				}
				else
				{
					_animationState = AnimationState.Blinking;
					_currentAnimationTime = 0f;
				}
			}
		}

		private void InitAnimation()
		{
			_infoSprite.alpha = 1f;
			_infoSpriteOverlay.alpha = 0f;
		}

		private void Update()
		{
			if (_animationState == AnimationState.None)
			{
				return;
			}
			if (_animationState == AnimationState.Scaling)
			{
				if (_currentAnimationTime < 1f)
				{
					float num = Mathf.Max(0f, Easings.ElasticEaseOut(_currentAnimationTime / 1f) * 2f - 1f);
					Vector3 localScale = Vector3.LerpUnclamped(_startScale, _endScale, num);
					_infoSprite.cachedTransform.localScale = localScale;
					_infoSpriteOverlay.cachedTransform.localScale = localScale;
					float alpha = num - 1f;
					_infoSpriteOverlay.alpha = alpha;
					_currentAnimationTime += Time.unscaledDeltaTime;
				}
				else
				{
					_infoSprite.cachedTransform.localScale = _endScale;
					_infoSpriteOverlay.cachedTransform.localScale = _endScale;
					_infoSpriteOverlay.alpha = 0f;
					_animationState = AnimationState.Waiting;
					_currentAnimationTime = 0f;
				}
			}
			if (_animationState == AnimationState.Blinking)
			{
				if (_currentAnimationTime < 1f)
				{
					float alpha2 = ((!(_currentAnimationTime < 0.5f)) ? Mathf.SmoothStep(2f, 0f, _currentAnimationTime / 1f) : Mathf.SmoothStep(0f, 2f, _currentAnimationTime / 1f));
					_infoSpriteOverlay.alpha = alpha2;
					_currentAnimationTime += Time.unscaledDeltaTime;
				}
				else
				{
					_infoSpriteOverlay.alpha = 0f;
					_animationState = AnimationState.Waiting;
					_currentAnimationTime = 0f;
				}
			}
			if (_animationState == AnimationState.Waiting)
			{
				if (_currentAnimationTime < 2f)
				{
					_currentAnimationTime += Time.unscaledDeltaTime;
					return;
				}
				_animationState = AnimationState.Blinking;
				_currentAnimationTime = 0f;
			}
		}
	}
}
