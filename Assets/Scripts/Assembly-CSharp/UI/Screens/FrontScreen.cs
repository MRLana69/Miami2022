using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ABTest;
using AppRefresh;
using Cloud;
using Cloud.Internal;
using Data.Save;
using InApp;
using Info;
using JetBrains.Annotations;
using Kiloo;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Common.JsonUtil;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Timer;
using Kiloo.Common.Utility;
using NaughtyAttributes;
using SYBO.Subway;
using SYBO.Subway.Promotion;
using TopRun;
using UI.Generic.Layout;
using UI.MessageCenter;
using UI.Popups;
using UI.Screens.FrontScreenInternal;
using UI.SlideIns;
using UI.VideoAdPrizes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Utilities;

namespace UI.Screens
{
	public class FrontScreen : RunStartScreen
	{
		private enum AnimatingButton
		{
			MyTourButton = 0,
			BottomMenuButtons = 1,
			EventButton = 2,
			QuickVideoButton = 3,
			OfferwallButton = 4,
			BoardBundleButton = 5,
			FirstPurchaseBonusButton = 6,
			PromotionButton = 7,
			WelcomePackButton = 8,
			TokenMysteryBoxButton = 9
		}

		[SerializeField]
		private GameObject[] _tweenButtons;

		[SerializeField]
		private UIWidget[] _widgetFrames;

		[FormerlySerializedAs("discountStickerNormal")]
		[SerializeField]
		private GameObject _discountStickerNormal;

		[FormerlySerializedAs("discountStickerSpecialSale")]
		[SerializeField]
		private GameObject _discountStickerSpecialSale;

		[FormerlySerializedAs("sticker")]
		[SerializeField]
		private UISprite _sticker;

		[FormerlySerializedAs("stickerSpecialSale")]
		[SerializeField]
		private UISprite _stickerSpecialSale;

		[FormerlySerializedAs("stickerSpecialSaleOutline")]
		[SerializeField]
		private UISprite _stickerSpecialSaleOutline;

		[FormerlySerializedAs("startButtonCollider")]
		[SerializeField]
		private BoxCollider _startButtonCollider;

		[Header("TopLeft")]
		[SerializeField]
		private Layout _topLeftLayout;

		[SerializeField]
		private GameObject _showEventsPopupButton;

		[Header("Top")]
		[SerializeField]
		private UIWidget _myTourContainer;

		[SerializeField]
		private GameObject _myTourButton;

		[Header("TopRight")]
		[SerializeField]
		private Layout _topRightLayout;

		[SerializeField]
		private GameObject _quickVideoButton;

		[SerializeField]
		private GameObject _quickVideoButtonPrefab;

		[SerializeField]
		private GameObject _offerWallButton;

		[SerializeField]
		private GameObject _firstPurchaseBonusButtonPrefab;

		[SerializeField]
		private GameObject _welcomePackButtonPrefab;

		[SerializeField]
		private GameObject _tokenMysteryBoxButtonPrefab;

		private GameObject _promotionButton;

		private GameObject _firstPurchaseBonusButton;

		private GameObject _welcomePackButton;

		private GameObject _tokenMysteryBoxButton;

		private QuickVideoButtonHelper _quickVideoButtonHelper;

		[Header("Bottom")]
		[SerializeField]
		private GameObject _bottomParent;

		[FormerlySerializedAs("shopDiscountLabel1")]
		[SerializeField]
		private UILabel _shopDiscountLabel1;

		[FormerlySerializedAs("shopDiscountLabel2")]
		[SerializeField]
		private UILabel _shopDiscountLabel2;

		[FormerlySerializedAs("shopDiscountLabelSingleLine")]
		[SerializeField]
		private UILabel _shopDiscountLabelSingleLine;

		[FormerlySerializedAs("shopDiscountLabelSpecialSale")]
		[SerializeField]
		private UILabel _shopDiscountLabelSpecialSale;

		[SerializeField]
		private GuidelineButton _guidelineButtonShop;

		[Header("TapPanel")]
		[FormerlySerializedAs("tapToPlayLabel")]
		[SerializeField]
		private UILabel _tapToPlayLabel;

		private const int MY_TOUR_WIDE_RIGHT_OFFSET = -106;

		private DateTime _frontScreenShowTimeStamp;

		private const float DEFAULT_INTERSTITIAL_SHOW_ONCACHE_TIMEFRAME = 1.5f;

		private const string INTERSTITIAL_FRONTSCREEN_SHOW_ONCACHE_TIMEFRAME = "interstitial_frontscreen_show_oncache_timeframe";

		private bool _buttonsHaveTweened;

		private Dictionary<AnimatingButton, IEnumerator> _buttonAnimationCoroutines = new Dictionary<AnimatingButton, IEnumerator>();

		private bool _hasShownConflictStateThisSession;

		private static bool _firstTimeShown = true;

		private const float LEFT_SLIDEIN_BUTTON_START_OFFSET = -60f;

		private const float RIGHT_SLIDEIN_BUTTON_START_OFFSET = 60f;

		private const float TOP_SLIDEIN_BUTTON_START_OFFSET = 70f;

		private const float BOTTOM_SLIDEIN_BUTTON_START_OFFSET = -110f;

		private float _safeAreaTopOffset;

		private float _safeAreaBottomOffset;

		private bool _hasShownBefore;

		private bool _animationsAllowed;

		private bool _wasAnimationInterrupted;

		private readonly List<AnimatingButton> _completedButtonAnimations = new List<AnimatingButton>();

		private readonly List<AnimatingButton> _isButtonAnimatingNow = new List<AnimatingButton>();

		private bool _hasFlashedVideoButtonInSessionOnce;

		private bool _videoButtonIsFlashing;

		private bool _isApplicationResuming;

		public override string GetFlurryName()
		{
			return "FrontUI";
		}

		protected override void Init()
		{
			base.Init();
			if (Application.isEditor && SceneManager.sceneCount == 1)
			{
				BillboardAdsManager.Initialize();
				HouseKeeper.InitOnlineServicesIfNeeded();
				if (!AgeRestriction.waitForAgeRestriction)
				{
					InitializeAdSystemForEditor();
					BillboardAdsManager.InitializeAdvertySdk();
				}
				else
				{
					AgeRestriction.restrictionChanged += InitializeAdSystemForEditor;
				}
			}
			_shopDiscountLabelSingleLine.text = Strings.Get("COOL_DEAL_SINGLE_LINE");
			_tapToPlayLabel.text = Strings.Get("FRONT_UI_TAP_TO_PLAY");
			SetDefaultSaleStickerColors();
			_discountStickerSpecialSale.SetActive(false);
			_discountStickerNormal.SetActive(false);
			HandleSafeArea();
			if (TopRunRoot.manager != null)
			{
				TopRunRoot.manager.tournamentChanged += TournamentChanged;
			}
			GameObject[] tweenButtons = _tweenButtons;
			foreach (GameObject gameObject in tweenButtons)
			{
				gameObject.SetActive(false);
			}
			if (DeviceUtility.isKindleDevice)
			{
				InAppManager.instance.RestoreNonConsumablePurchases(InAppData.GetAllProductIds(), false);
			}
			_quickVideoButtonHelper = _quickVideoButton.AddChild(_quickVideoButtonPrefab).GetComponent<QuickVideoButtonHelper>();
			_myTourContainer.rightAnchor.absolute = -106;
			_myTourContainer.ResetAndUpdateAnchors();
			_quickVideoButtonHelper.onVideoStarted += HandleOnQuickVideoStarted;
			if (_firstPurchaseBonusButtonPrefab != null)
			{
				_firstPurchaseBonusButton = _topRightLayout.gameObject.AddChild(_firstPurchaseBonusButtonPrefab);
				_topRightLayout.Align();
				_firstPurchaseBonusButton.SetActive(false);
			}
			_welcomePackButton = _topRightLayout.gameObject.AddChild(_welcomePackButtonPrefab);
			_welcomePackButton.SetActive(false);
			PromotionDefinition promotionDefinition = PromotionManager.Instance.TryGetCurrentPromotion();
			if (promotionDefinition != null)
			{
				_promotionButton = _topRightLayout.gameObject.AddChild(promotionDefinition.ButtonPrefab);
				_promotionButton.GetComponent<UIButtonChangeScreen>().PopupName = promotionDefinition.PopupId;
				_promotionButton.SetActive(false);
			}
			_tokenMysteryBoxButton = _topRightLayout.gameObject.AddChild(_tokenMysteryBoxButtonPrefab);
			_tokenMysteryBoxButton.SetActive(false);
			_topRightLayout.Align();
		}

		private void OnDestroy()
		{
			if (TopRunRoot.manager != null)
			{
				TopRunRoot.manager.tournamentChanged -= TournamentChanged;
			}
			foreach (KeyValuePair<AnimatingButton, IEnumerator> buttonAnimationCoroutine in _buttonAnimationCoroutines)
			{
				StopCoroutine(buttonAnimationCoroutine.Value);
			}
			_buttonAnimationCoroutines.Clear();
		}

		protected override void Show()
		{
			base.Show();
			HookEvents();
			AssetBundleManager.Instance.ResumeAssetBundlesDownload();
			AssetBundleManager.Instance.PerformAvailabilityCheck();
			if (AgeRestriction.shouldDisplayPopup)
			{
				UIScreenController.Instance.QueuePopup("AgeRestrictionPopup");
			}
			else if (AgeRestriction.shouldShowBehaviouralAdsConsentPopup)
			{
				UIScreenController.Instance.QueuePopup("ConsentPopup");
			}
			CheckForCloudSaveConflict();
			// No navegador o save é regravado a cada abertura, então o "Boas-vindas a Miami" aparecia sempre: não mostra
			bool flag = ShouldDisplayChangeLog() && Application.platform != RuntimePlatform.WebGLPlayer;
			bool flag2 = OnboardingSave.isTutorialCompleted || Application.isEditor;
			bool flag3 = false;
			if (flag && flag2)
			{
				UIScreenController.Instance.QueuePopup("WelcomePopup");
				UserSave.lastTimeAppWasUpdatedOrInstalled = CDateTime.UtcNow;
				flag3 = true;
			}
			else if (!Game.Instance.IsInGame.Value && CanShowInterstitial())
			{
				AdSystemManager.Show(AdLocations.HOME_INTERSTITIALS);
			}
			ABTestManager.SendPageCountFlurry();
			if (_hasShownBefore)
			{
				if (_wasAnimationInterrupted)
				{
					OnAnimationsComplete();
				}
				WelcomeBackRewardManager.ShowPopupIfNeeded();
				AnimateButtons();
			}
			else
			{
				_hasShownBefore = true;
				if (!flag3)
				{
					Timer.ExecuteWithDelay(0.5f, AnimateButtons);
				}
			}
			CheckForDiscountButton();
			if (!OnboardingSave.isTutorialCompleted && !UserSave.hasSeenFrontScreenFirstTime)
			{
				Flurry.LogEvent("First front screen");
				UserSave.hasSeenFrontScreenFirstTime = true;
			}
			if (OnboardingSave.shouldShowEndGameMissionPopup && !OnboardingSave.hasShownEndGameMissionPopup)
			{
				OnboardingSave.shouldShowEndGameMissionPopup = false;
				OnboardingSave.hasShownEndGameMissionPopup = true;
				UIScreenController.Instance.QueuePopup("TutorialEndGameMissionsPopup");
			}
			CheckForCloudSaveIntroduction();
			CheckForCloudSaveReminder();
			if (OfferWallManager.instance.IsOfferWallAllowed())
			{
				OfferWallManager.instance.GetPendingRewards();
			}
			OfferWallManager.instance.GetPendingRewards();
			_topLeftLayout.Align();
			_frontScreenShowTimeStamp = DateTime.Now;
			if (!OnboardingSave.isTutorialCompleted)
			{
				Timer.ExecuteWithDelay(1f, () =>
				{
					if ((bool)_startButtonCollider)
					{
						_startButtonCollider.enabled = true;
					}
				});
			}
			else
			{
				FacebookRewardsInfo.PayoutRewardIfPossible();
			}
		}

		protected override void Hide()
		{
			base.Hide();
			UnhookEvents();
			InterruptCurrentButtonAnimations();
			_firstTimeShown = false;
		}

		public override void BackButtonPressed()
		{
			ConfirmationPopup.variant = ConfirmationPopupVariant.QuitApplication;
			UIScreenController.Instance.PushPopup("ConfirmationPopup");
		}

		private void HookEvents()
		{
			SSEventsManagerController.OnSSEventChangedHandler += AnimateEventButton;
			UIScreenController.OnLastPopupClosed += HandleOnLastPopupClosed;
			OfferWallManager.instance.onOfferWallReceived += OnOfferWallOffersRecieved;
			OnlineSettings.instance.AddOnFinishedDownloadingOnlineSettingsHandler(OnOnlineSettingsFinishedDownloading);
			UIScreenController.OnPopupClosed += OnPopupClosed;
			AdSystemManager.onCachedVideo += HandleOnVideoStateChanged;
			AdSystemManager.onFailedToCacheVideo += HandleOnVideoStateChanged;
			AdSystemManager.onFinishedWatchingVideo += HandleOnVideoStateChanged;
			UIScreenController.Instance.OnOverlayManuallyClosed += AnimateQuickVideoButton;
			AdSystemManager.onCachedInterstitial += HandleOnCachedInterstitial;
			Game.OnMenuScreenShown += HandleOnMenuScreenShown;
			PromotionManager.onPromotionStartedHandler += AnimatePromotionButton;
			PromotionManager.onPromotionExpiredHandler += AnimatePromotionButton;
			InAppManager.instance.onProductRequestDone += CheckForDiscountButton;
			InAppManager.instance.onItemRestored += OnItemRestored;
		}

		private void UnhookEvents()
		{
			SSEventsManagerController.OnSSEventChangedHandler -= AnimateEventButton;
			UIScreenController.OnLastPopupClosed -= HandleOnLastPopupClosed;
			OfferWallManager.instance.onOfferWallReceived -= OnOfferWallOffersRecieved;
			OnlineSettings.instance.RemoveOnFinishedDownloadingOnlineSettingsHandler(OnOnlineSettingsFinishedDownloading);
			UIScreenController.OnPopupClosed -= OnPopupClosed;
			AdSystemManager.onCachedVideo -= HandleOnVideoStateChanged;
			AdSystemManager.onFailedToCacheVideo -= HandleOnVideoStateChanged;
			AdSystemManager.onFinishedWatchingVideo -= HandleOnVideoStateChanged;
			UIScreenController.Instance.OnOverlayManuallyClosed -= AnimateQuickVideoButton;
			AdSystemManager.onCachedInterstitial -= HandleOnCachedInterstitial;
			Game.OnMenuScreenShown -= HandleOnMenuScreenShown;
			PromotionManager.onPromotionStartedHandler -= AnimatePromotionButton;
			PromotionManager.onPromotionExpiredHandler -= AnimatePromotionButton;
			InAppManager.instance.onProductRequestDone -= CheckForDiscountButton;
			InAppManager.instance.onItemRestored -= OnItemRestored;
		}

		private void CheckForCloudSaveReminder()
		{
			if (CloudSave.state != CloudSaveState.Active && CloudSave.state != CloudSaveState.Inactive && !(UserSave.latestSuccessfulCloudSaveSyncTime == DateTime.MinValue) && !(CDateTime.Today < UserSave.latestSuccessfulCloudSaveSyncTime.AddDays(1.0)) && !_firstTimeShown && !(CDateTime.Today < UserSave.lastCloudSaveReminderShownTime.AddDays(1.0)))
			{
				UIScreenController.Instance.QueuePopup("CloudSaveReminderPopup");
			}
		}

		private void CheckForCloudSaveConflict()
		{
			if (CloudSave.state == CloudSaveState.Conflict && !_hasShownConflictStateThisSession)
			{
				_hasShownConflictStateThisSession = true;
				if (CloudSave.conflictResolver.recommendation == CloudSaveConflictResolver.Recommendation.Theirs)
				{
					UIScreenController.Instance.QueuePopup("CloudSavePromotionPopup");
					return;
				}
				UIScreenController.Instance.QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.CloudSave,
					cloudSaveState = CloudSave.state
				});
			}
		}

		public void HandleConflictsAfterAppResume()
		{
			if (CloudSave.state != CloudSaveState.Conflict)
			{
				return;
			}
			if (UIScreenController.Instance.GetTopScreenName() == "FrontScreen")
			{
				if (CloudSave.conflictResolver.recommendation == CloudSaveConflictResolver.Recommendation.Theirs)
				{
					UIScreenController.Instance.QueuePopup("CloudSavePromotionPopup");
					return;
				}
				UIScreenController.Instance.QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.CloudSave,
					cloudSaveState = CloudSave.state
				});
			}
			else
			{
				_hasShownConflictStateThisSession = false;
			}
		}

		private void CheckForCloudSaveIntroduction()
		{
			if (CloudSave.state != CloudSaveState.OptedOut && CloudSave.isAvailable && PlayerSave.completedRunCount > 3 && !_firstTimeShown && CloudSave.state == CloudSaveState.Inactive && !UserSave.cloudSaveEnabled && !(CDateTime.UtcNow < OnboardingSave.lastShownCloudSaveIntroduction.AddDays(1.0)))
			{
				UIScreenController.Instance.QueuePopup("CloudSaveIntroductionPopup");
			}
		}

		private void CheckForCloudSaveWelcomeSlideIn()
		{
			if (_firstTimeShown && Application.platform != RuntimePlatform.IPhonePlayer && CloudSave.isAvailable && UserSave.cloudSaveEnabled && CloudSave.state == CloudSaveState.Active)
			{
				CoroutineUtil.StartCoroutine(WelcomeSlideInCoroutine());
			}
		}

		public IEnumerator WelcomeSlideInCoroutine()
		{
			while (AgeRestriction.waitForAgeRestriction)
			{
				yield return null;
			}
			UIScreenController.Instance.QueueSlideIn(new SlideIn
			{
				type = SlideInData.SlideInType.CloudSave,
				cloudSaveState = CloudSave.state
			});
		}

		private void AnimateButtons()
		{
			if (!(this == null))
			{
				AnimateMyTourButton();
				AnimateBottomButtons();
				AnimateEventButton();
				AnimateQuickVideoButton();
				AnimateFirstPurchaseBonusButton();
				AnimateWelcomePackButton();
				AnimatePromotionButton();
				AnimateOfferwallButton();
				AnimateTokenMysteryBoxButton();
			}
		}

		private void AnimateEventButton()
		{
			if (!_animationsAllowed || !OnboardingSave.isTutorialCompleted)
			{
				return;
			}
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			bool flag = activeEventManager != null;
			_showEventsPopupButton.SetActive(flag);
			_topLeftLayout.Align();
			if (flag)
			{
				if (!HasCompletedAnimation(AnimatingButton.EventButton))
				{
					StartAnimation(AnimatingButton.EventButton);
				}
			}
			else
			{
				ResetAnimation(AnimatingButton.EventButton);
			}
		}

		public void AnimateQuickVideoButton()
		{
			if (!_animationsAllowed || !OnboardingSave.isTutorialCompleted)
			{
				return;
			}
			bool flag = AdSystemManager.IsCached(AdLocations.HOME_VIDEOS);
			AdTrackingSystemLogger.LogAttemptToShowButtonFlow(AdLocations.HOME_VIDEOS, flag);
			_quickVideoButton.SetActive(flag);
			_topRightLayout.Align();
			if (flag)
			{
				_quickVideoButtonHelper.SetPrize(VideoAdPrizeManager.instance.GetVideoAdPrize());
				if (!HasCompletedAnimation(AnimatingButton.QuickVideoButton))
				{
					StartAnimation(AnimatingButton.QuickVideoButton);
				}
			}
			else
			{
				ResetAnimation(AnimatingButton.QuickVideoButton);
			}
		}

		private void FlashVideoButtonIfNecessary()
		{
			if (!_hasFlashedVideoButtonInSessionOnce && VideoAdPrizeManager.instance.IsVideoForKeysAvailable())
			{
				_hasFlashedVideoButtonInSessionOnce = true;
				_videoButtonIsFlashing = true;
				StartCoroutine(FlashVideoButtonScale());
			}
			else
			{
				_quickVideoButtonHelper.PlayAnimation();
			}
		}

		private void ResetVideoButtonFlashScale()
		{
			_quickVideoButtonHelper.DisableFlashOverlay();
			_quickVideoButton.transform.localScale = new Vector3(1f, 1f, 0f);
			_videoButtonIsFlashing = false;
			_quickVideoButtonHelper.PlayAnimation();
		}

		private IEnumerator FlashVideoButtonScale()
		{
			yield return new WaitForSeconds(0.6f);
			_quickVideoButtonHelper.FlashOverlay();
			float scaleUpDuration = 0.2f;
			float scaleDownDuration = 0.3f;
			float currentDuration = 0f;
			float normalScale = 1f;
			float maxScale = 1.2f;
			float currentScaleFactor = 0f;
			Vector3 currentScale = new Vector3(0f, 0f, 0f);
			while (currentDuration < scaleUpDuration)
			{
				currentScale.y = (currentScale.x = Mathf.Lerp(normalScale, maxScale, currentDuration / scaleUpDuration));
				_quickVideoButton.transform.localScale = currentScale;
				currentDuration += Time.unscaledDeltaTime;
				yield return null;
			}
			currentDuration = 0f;
			while (currentDuration < scaleDownDuration)
			{
				currentScale.y = (currentScale.x = Mathf.Lerp(maxScale, normalScale, currentDuration / scaleDownDuration));
				_quickVideoButton.transform.localScale = currentScale;
				currentDuration += Time.unscaledDeltaTime;
				yield return null;
			}
			ResetVideoButtonFlashScale();
		}

		private void AnimatePromotionButton()
		{
			if ((bool)_promotionButton && OnboardingSave.isTutorialCompleted && _animationsAllowed)
			{
				bool flag = PromotionManager.Instance.IsCurrentPromotionActive && !WelcomePackManager.IsWelcomePackActive();
				_promotionButton.SetActive(flag);
				_topRightLayout.Align(true);
				if (flag && !HasCompletedAnimation(AnimatingButton.PromotionButton))
				{
					StartAnimation(AnimatingButton.PromotionButton);
				}
			}
		}

		private void AnimateFirstPurchaseBonusButton()
		{
			bool flag = FirstPurchaseBonusManager.isFirstPurchaseBonusActive && OnboardingSave.isTutorialCompleted && !global::Data.Save.SaveManager.GjSaveApplied;
			if (!_animationsAllowed)
			{
				flag = false;
			}
			_firstPurchaseBonusButton.SetActive(flag);
			_topRightLayout.Align();
			if (flag && !HasCompletedAnimation(AnimatingButton.FirstPurchaseBonusButton))
			{
				StartAnimation(AnimatingButton.FirstPurchaseBonusButton);
			}
		}

		private void AnimateWelcomePackButton()
		{
			bool flag = WelcomePackManager.IsWelcomePackActive() && OnboardingSave.isTutorialCompleted && _animationsAllowed && !global::Data.Save.SaveManager.GjSaveApplied;
			_welcomePackButton.SetActive(flag);
			_topRightLayout.Align();
			if (flag && !HasCompletedAnimation(AnimatingButton.WelcomePackButton))
			{
				StartAnimation(AnimatingButton.WelcomePackButton);
			}
		}

		private void OnItemRestored(string result)
		{
			if (result == "com.kiloo.subwaysurfers.welcomepacktier01" || result == "com.kiloo.subwaysurfers.welcomepacktier01")
			{
				AnimateWelcomePackButton();
			}
		}

		private void AnimateOfferwallButton()
		{
			if (!_animationsAllowed || !OnboardingSave.isTutorialCompleted || !OfferWallManager.instance.IsOfferWallAllowed())
			{
				return;
			}
			bool flag = OfferWallManager.instance.IsOfferWallAvailable();
			_offerWallButton.SetActive(flag);
			_topRightLayout.Align();
			if (flag)
			{
				if (!HasCompletedAnimation(AnimatingButton.OfferwallButton))
				{
					StartAnimation(AnimatingButton.OfferwallButton);
				}
			}
			else
			{
				ResetAnimation(AnimatingButton.OfferwallButton);
			}
		}

		private void AnimateMyTourButton()
		{
			if (!HasCompletedAnimation(AnimatingButton.MyTourButton))
			{
				bool isTutorialCompleted = OnboardingSave.isTutorialCompleted;
				_myTourButton.SetActive(isTutorialCompleted);
				if (isTutorialCompleted)
				{
					StartAnimation(AnimatingButton.MyTourButton);
				}
			}
		}

		private void AnimateTokenMysteryBoxButton()
		{
			if (_tokenMysteryBoxButton != null)
			{
				bool flag = !UserSave.hasUserRecievedFreeTokenMysteryBox && OnboardingSave.isTutorialCompleted && MysteryBox.AreTokensAvailable();
				_tokenMysteryBoxButton.SetActive(flag);
				if (!HasCompletedAnimation(AnimatingButton.TokenMysteryBoxButton) && _animationsAllowed && flag)
				{
					StartAnimation(AnimatingButton.TokenMysteryBoxButton);
				}
			}
		}

		private void AnimateBottomButtons()
		{
			if (!HasCompletedAnimation(AnimatingButton.BottomMenuButtons) && _animationsAllowed)
			{
				bool isTutorialCompleted = OnboardingSave.isTutorialCompleted;
				_bottomParent.SetActive(isTutorialCompleted);
				if (isTutorialCompleted)
				{
					StartAnimation(AnimatingButton.BottomMenuButtons);
				}
			}
		}

		private void StartAnimation(AnimatingButton b)
		{
			if (IsAnimating(b))
			{
				return;
			}
			_isButtonAnimatingNow.AddDistinct(b);
			IEnumerator enumerator = null;
			switch (b)
			{
			case AnimatingButton.MyTourButton:
				_animationsAllowed = true;
				enumerator = AnimateButtonVertical(_myTourButton, 0f, 70f + _safeAreaTopOffset, () =>
				{
					CompleteAnimation(AnimatingButton.MyTourButton);
					OnAnimationsComplete();
				});
				break;
			case AnimatingButton.BottomMenuButtons:
				enumerator = AnimateButtonVertical(_bottomParent, 0f, -110f + _safeAreaBottomOffset, () =>
				{
					CompleteAnimation(AnimatingButton.BottomMenuButtons);
				});
				break;
			case AnimatingButton.EventButton:
				enumerator = AnimateButtonHorizontal(_showEventsPopupButton, 0f, -60f, () =>
				{
					CompleteAnimation(AnimatingButton.EventButton);
				});
				break;
			case AnimatingButton.QuickVideoButton:
				enumerator = AnimateButtonHorizontal(_quickVideoButton, 0f, 60f, () =>
				{
					CompleteAnimation(AnimatingButton.QuickVideoButton);
				});
				break;
			case AnimatingButton.OfferwallButton:
				enumerator = AnimateButtonHorizontal(_offerWallButton, 0f, 60f, () =>
				{
					CompleteAnimation(AnimatingButton.OfferwallButton);
				});
				break;
			case AnimatingButton.FirstPurchaseBonusButton:
				enumerator = AnimateButtonHorizontal(_firstPurchaseBonusButton, 0f, 60f, () =>
				{
					CompleteAnimation(AnimatingButton.FirstPurchaseBonusButton);
				});
				break;
			case AnimatingButton.WelcomePackButton:
				enumerator = AnimateButtonHorizontal(_welcomePackButton, 0f, 60f, () =>
				{
					CompleteAnimation(AnimatingButton.WelcomePackButton);
				});
				break;
			case AnimatingButton.PromotionButton:
				enumerator = AnimateButtonHorizontal(_promotionButton, 0f, 60f, () =>
				{
					CompleteAnimation(AnimatingButton.PromotionButton);
				});
				break;
			case AnimatingButton.TokenMysteryBoxButton:
				enumerator = AnimateButtonHorizontal(_tokenMysteryBoxButton, 0f, 60f, () =>
				{
					CompleteAnimation(AnimatingButton.TokenMysteryBoxButton);
				});
				break;
			}
			_buttonAnimationCoroutines[b] = enumerator;
			StartCoroutine(enumerator);
		}

		private void SkipAnimationTween(AnimatingButton b)
		{
			switch (b)
			{
			case AnimatingButton.MyTourButton:
				_myTourButton.transform.SetLocalPosY(0f);
				break;
			case AnimatingButton.BottomMenuButtons:
				_bottomParent.transform.SetLocalPosY(0f);
				break;
			case AnimatingButton.EventButton:
				_showEventsPopupButton.transform.SetLocalPosX(0f);
				break;
			case AnimatingButton.QuickVideoButton:
				_quickVideoButton.transform.SetLocalPosX(0f);
				break;
			case AnimatingButton.OfferwallButton:
				_offerWallButton.transform.SetLocalPosX(0f);
				break;
			case AnimatingButton.PromotionButton:
				_promotionButton.transform.SetLocalPosX(0f);
				break;
			case AnimatingButton.FirstPurchaseBonusButton:
				_firstPurchaseBonusButton.transform.SetLocalPosX(0f);
				break;
			case AnimatingButton.WelcomePackButton:
				_welcomePackButton.transform.SetLocalPosX(0f);
				break;
			case AnimatingButton.TokenMysteryBoxButton:
				_tokenMysteryBoxButton.transform.SetLocalPosX(0f);
				break;
			default:
				SDebug.LogError("A new button was added, but does not exist in SkipAnimationTween!");
				break;
			}
		}

		private void InterruptCurrentButtonAnimations()
		{
			bool wasAnimationInterrupted = false;
			foreach (AnimatingButton item in _isButtonAnimatingNow)
			{
				SkipAnimationTween(item);
				_completedButtonAnimations.AddDistinct(item);
				wasAnimationInterrupted = true;
			}
			_startButtonCollider.enabled = true;
			_wasAnimationInterrupted = wasAnimationInterrupted;
			_isButtonAnimatingNow.Clear();
			if (_videoButtonIsFlashing)
			{
				ResetVideoButtonFlashScale();
			}
		}

		private void OnAnimationsComplete()
		{
			_wasAnimationInterrupted = false;
			_startButtonCollider.enabled = true;
			HandlePopups();
			CheckForDiscountButton();
			RewardManager.PayoutNonMysteryBoxRewards();
			_buttonsHaveTweened = true;
			CheckForCloudSaveWelcomeSlideIn();
			if (CanShowInterstitial())
			{
				AdSystemManager.Show(AdLocations.HOME_INTERSTITIALS);
			}
		}

		private bool IsAnimating(AnimatingButton b)
		{
			return _isButtonAnimatingNow.Contains(b);
		}

		private bool HasCompletedAnimation(AnimatingButton b)
		{
			return _completedButtonAnimations.Contains(b);
		}

		private void CompleteAnimation(AnimatingButton b)
		{
			if (b == AnimatingButton.QuickVideoButton)
			{
				FlashVideoButtonIfNecessary();
			}
			_completedButtonAnimations.AddDistinct(b);
			_isButtonAnimatingNow.Remove(b);
		}

		private void ResetAnimation(AnimatingButton b)
		{
			_completedButtonAnimations.Remove(b);
		}

		[Button(null)]
		private void RetriggerSlideinAnimations()
		{
			_completedButtonAnimations.Clear();
			AnimateButtons();
		}

		private IEnumerator AnimateButtonVertical(GameObject button, float delay, float offset, Action onComplete)
		{
			button.SetActive(true);
			float endY = 0f;
			float startY = endY + offset;
			button.transform.SetLocalPosY(startY);
			yield return new WaitForSeconds(delay);
			yield return StartCoroutine(AnimateSlideInButton(button.transform, startY, endY, false));
			onComplete.SafeInvoke();
		}

		private IEnumerator AnimateButtonHorizontal(GameObject button, float delay, float offset, Action onComplete)
		{
			button.SetActive(true);
			_topLeftLayout.Align();
			_topRightLayout.Align();
			float endX = 0f;
			float startX = endX + offset;
			button.transform.SetLocalPosX(startX);
			yield return new WaitForSeconds(delay);
			yield return StartCoroutine(AnimateSlideInButton(button.transform, startX, endX, true));
			onComplete.SafeInvoke();
		}

		private static IEnumerator AnimateSlideInButton(Transform t, float start, float end, bool horizontal)
		{
			float currentDuration = 0f;
			Action<float> setPos = ((!horizontal) ? new Action<float>(t.SetLocalPosY) : new Action<float>(t.SetLocalPosX));
			while (currentDuration < 0.5f)
			{
				if (t == null)
				{
					yield break;
				}
				currentDuration += Time.deltaTime;
				float p = Easings.QuadraticEaseOut(currentDuration / 0.5f);
				float current = Mathf.Lerp(start, end, p);
				setPos.SafeInvoke(current);
				yield return null;
			}
			setPos.SafeInvoke(end);
		}

		public override bool ShouldShowTopBar()
		{
			return UIScreenController.Instance.GetCurrentPopupName() != "CloudSaveCelebrationPopup";
		}

		private void HandleSafeArea()
		{
			_safeAreaTopOffset = TopBar.GetTopBarOffset();
			UIWidget[] widgetFrames = _widgetFrames;
			foreach (UIWidget uIWidget in widgetFrames)
			{
				uIWidget.topAnchor.absolute = _safeAreaTopOffset.RoundToInt();
			}
			if (SafeArea.IsRestricted)
			{
				_tapToPlayLabel.transform.OffsetLocalPosY(SafeArea.OFFSET_BOTTOM);
				float num = SafeArea.OFFSET_BOTTOM;
				_safeAreaBottomOffset = 0f - num;
				UIWidget[] widgetFrames2 = _widgetFrames;
				foreach (UIWidget uIWidget2 in widgetFrames2)
				{
					uIWidget2.bottomAnchor.absolute = num.RoundToInt();
				}
			}
			GetComponentsInChildren<UIRect>().ToList().ForEach((UIRect r) =>
			{
				r.ResetAndUpdateAnchors();
			});
		}

		private void TournamentChanged(object sender, TournamentChangedEventArgs eventArg)
		{
			TopRunRewardHelper.RefreshPendingResults();
		}

		private static bool ShouldDisplayChangeLog()
		{
			bool result = false;
			string value = FormatVersionNumber(DeviceUtility.GetBundleVersion());
			string text = FormatVersionNumber(UserSave.lastSeenBundleVersion);
			if (!text.Equals(value) && !(text == string.Empty))
			{
				result = true;
				UserSave.isFreshInstall = false;
			}
			if (!text.Equals(value))
			{
				UserSave.previouslySeenBundleVersion = UserSave.lastSeenBundleVersion;
				UserSave.lastSeenBundleVersion = DeviceUtility.GetBundleVersion();
			}
			return result;
		}

		private static string FormatVersionNumber(string oldVersionNumber)
		{
			string result = oldVersionNumber;
			string[] array = oldVersionNumber.Split('.');
			if (array.Length >= 2)
			{
				result = string.Format("{0}.{1}", array[0], array[1]);
			}
			return result;
		}

		private void OnPopupClosed(string popupName)
		{
			AnimateButtons();
		}

		private void HandleOnVideoStateChanged(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
		{
			HandleOnVideoStateChanged(adCacheSlotInfo, adLocationInfo, false);
		}

		private void HandleOnVideoStateChanged(AdCacheSlotInfo adCacheSlotInfo, bool wasAlreadyCached)
		{
			HandleOnVideoStateChanged(adCacheSlotInfo, null, wasAlreadyCached);
		}

		private void HandleOnVideoStateChanged(AdCacheSlotInfo adCacheSlotInfo)
		{
			HandleOnVideoStateChanged(adCacheSlotInfo, null, false);
		}

		private void HandleOnVideoStateChanged(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo, bool wasAlreadyCached)
		{
			AnimateQuickVideoButton();
		}

		private void HandleOnLastPopupClosed()
		{
			string topScreenName = UIScreenController.Instance.GetTopScreenName();
			if (topScreenName != null && topScreenName == "FrontScreen" && CanShowInterstitial())
			{
				AdSystemManager.Show(AdLocations.HOME_INTERSTITIALS);
			}
		}

		private bool CanShowInterstitial()
		{
			return !AppRefreshHandler.refreshInProgress && _buttonsHaveTweened && OnboardingSave.isTutorialCompleted && !UIScreenController.Instance.isShowingPopup && AdSystemManager.isInitialized && AdSystemManager.IsCached(AdLocations.HOME_INTERSTITIALS);
		}

		private void HandleOnCachedInterstitial(AdCacheSlotInfo adCacheSlotInfo)
		{
			if (adCacheSlotInfo == AdCacheSlots.POOL1_INTERSTITIALS && CanShowInterstitial() && (DateTime.Now - _frontScreenShowTimeStamp).TotalSeconds < (double)OnlineSettings.instance.GetValue("interstitial_frontscreen_show_oncache_timeframe", 1.5f))
			{
				AdSystemManager.Show(AdLocations.HOME_INTERSTITIALS);
			}
		}

		private void HandleOnQuickVideoStarted()
		{
			_quickVideoButton.SetActive(false);
			ResetAnimation(AnimatingButton.QuickVideoButton);
		}

		private void HandleOnMenuScreenShown()
		{
			if (CanShowInterstitial())
			{
				AdSystemManager.Show(AdLocations.HOME_INTERSTITIALS);
			}
		}

		private void OnOnlineSettingsFinishedDownloading()
		{
			AnimateWelcomePackButton();
		}

		private void OnOfferWallOffersRecieved()
		{
			AnimateOfferwallButton();
		}

		private void CheckForDiscountButton()
		{
			if ((!InAppManager.IsDoubleCoinsDiscountActive && !InAppManager.IsAnyDiscountAvailable) || _guidelineButtonShop.ShouldInfoIconShow())
			{
				return;
			}
			if (InAppManager.IsSpecialSaleActive)
			{
				_discountStickerSpecialSale.SetActive(true);
				_discountStickerNormal.SetActive(false);
				if (InAppManager.IsWinterHolidaySpecialSaleActive)
				{
					_stickerSpecialSale.color = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_STICKER_COLOR;
					_stickerSpecialSaleOutline.color = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_STICKER_LABEL_COLOR;
					_shopDiscountLabelSpecialSale.color = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_STICKER_LABEL_COLOR;
				}
				else if (InAppManager.IsNewYearSpecialSaleActive)
				{
					_stickerSpecialSale.color = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_STICKER_COLOR;
					_stickerSpecialSaleOutline.color = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_STICKER_LABEL_COLOR;
					_shopDiscountLabelSpecialSale.color = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_STICKER_LABEL_COLOR;
				}
				else if (InAppManager.IsBlackFridaySpecialSaleActive)
				{
					_stickerSpecialSale.color = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_STICKER_COLOR;
					_stickerSpecialSaleOutline.color = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_STICKER_LABEL_COLOR;
					_shopDiscountLabelSpecialSale.color = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_STICKER_LABEL_COLOR;
				}
			}
			else
			{
				_discountStickerNormal.SetActive(true);
				_discountStickerSpecialSale.SetActive(false);
				SetDefaultSaleStickerColors();
			}
		}

		private void SetDefaultSaleStickerColors()
		{
			_sticker.color = GlobalColorsGenerated.DISCOUNT_STICKER_COLOR;
			_shopDiscountLabel1.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
			_shopDiscountLabel2.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
			_shopDiscountLabelSingleLine.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
		}

		private void InitializeAdSystemForEditor()
		{
			AgeRestriction.restrictionChanged -= InitializeAdSystemForEditor;
			AdSystemManager.CreateAdSystemManager();
		}

		private void _PauseApplication(bool paused)
		{
			if (!paused)
			{
				_isApplicationResuming = true;
				AssetBundleManager.Instance.ResumeAssetBundlesDownload();
				CheckForDiscountButton();
				AnimateButtons();
			}
		}

		private void OnApplicationPause(bool paused)
		{
			_PauseApplication(paused);
		}

		private void OnApplicationFocus(bool focus)
		{
			if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor)
			{
				_PauseApplication(!focus);
			}
		}

		private void LateUpdate()
		{
			if (_isApplicationResuming)
			{
				_isApplicationResuming = false;
				if (AdSystemManager.isInitialized)
				{
					AnimateQuickVideoButton();
					CheckForDiscountButton();
				}
			}
		}

		private void HandlePopups()
		{
			UIScreenController.Instance.RequeueDelayedPopups();
			UpdateAppPopup.ShowIfNeeded();
			WelcomeBackRewardManager.ShowPopupIfNeeded();
		}

		[UsedImplicitly]
		public void EventsButtonClicked()
		{
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.CharacterName)
			{
				UIScreenController.Instance.QueuePopup("CharacterNameEventPopup");
			}
			else if (activeEventManager != null)
			{
				UIScreenController.Instance.QueuePopup("EventsPopup");
			}
		}

		[UsedImplicitly]
		public void OfferWallButtonClicked()
		{
			if (OfferWallManager.instance.IsOfferWallAvailable())
			{
				OfferWallManager.instance.ShowOfferWall("FrontScreen");
			}
			else
			{
				AnimateOfferwallButton();
			}
		}

		[UsedImplicitly]
		public void FirstPurchaseButtonClicked()
		{
			UIScreenController.Instance.QueuePopup("FirstPurchaseBonusPopup");
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError(msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_FRONTSCREEN_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}
	}
}
