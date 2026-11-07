using System.Collections;
using Data.Save;
using JetBrains.Annotations;
using Kiloo.Common.Ads;
using Kiloo.Common.JsonUtil;
using Missions;
using SYBO.Components.Utilities;
using SYBO.Subway.Inputs;
using UI.Generic;
using UI.Generic.Layout;
using UI.Popups.SaveMePopupInternal;
using UnityEngine;

namespace UI.Popups
{
	public class SaveMePopup : UIPopup
	{
		public const float TIMEOUT_ANIMATION_DURATION = 4f;

		[SerializeField]
		private SaveMeAnimateClock _saveMeAnimateClock;

		[SerializeField]
		private UILabel _saveMeButtonLabel;

		[SerializeField]
		private UILabel _keyCostLabel;

		[SerializeField]
		private HorizontalLayout _keyHorizontalLayout;

		[SerializeField]
		private UILabel _freeLabel;

		[SerializeField]
		private UIWidget _bgFill;

		[SerializeField]
		private VerticalLayout _buttonLayout;

		[SerializeField]
		private GameObject _watchAdButton;

		[SerializeField]
		private GameObject _keyButton;

		[SerializeField]
		private UIWidget _animatedTVIconContainer;

		[SerializeField]
		private GameObject _animatedTVIconPrefab;

		private Coroutine _countdownCoroutine;

		private float _timeOutAnimationDuration;

		private readonly Vector3 _coinboxOffset = new Vector2(25f, 4f);

		private GameObject _coinboxInstance;

		private const int BACKGROUND_DEFAULT_HEIGHT = 233;

		private const int VIDEO_BUTTON_HEIGHT = 76;

		private const int KEY_BUTTON_HEIGHT = 62;

		public float animTimeLeft { get; private set; }

		protected override void Init()
		{
			_saveMeButtonLabel.text = Strings.Get("SAVE_ME_POPUP_BUTTON_LABEL");
			_saveMeButtonLabel.color = ColorData.instance.midnight;
			_freeLabel.text = Strings.Get("FREE");
			_timeOutAnimationDuration = SaveMeManager.GetTimeOutAnimationDuration();
			CreateTVIcon();
			CreateCoinbox();
		}

		protected override void Show()
		{
			SaveMeManager.IncrementSaveMePopupCountInCurrentRun();
			string videoStatus = GetVideoStatus();
			Flurry.LogEventWithSeveralParameters("Save Me shown", "Video status;Count", videoStatus + ";" + SaveMeManager.saveMePopupCountInCurrentRun);
			StartCountdown();
			UIScreenController instance = UIScreenController.Instance;
			IngameScreen ingameScreen = instance.GetScreenFromCache(instance.GetTopScreenName()) as IngameScreen;
			if (ingameScreen == null)
			{
				SDebug.LogError("IngameScreen == NULL");
			}
			else
			{
				ingameScreen.SetPauseButtonVisibility(false);
			}
			AdSystemManager.onFailedToShowVideo += OnShowVideoFailure;
			UIScreenController.Instance.OnOverlayManuallyClosed += OnVideoOverlayClosed;
			UIScreenController.OnApplicationResumed += OnApplicationResumed;
			_keyCostLabel.text = SaveMeManager.GetNumberOfKeysToSaveMe() + string.Empty;
			_keyHorizontalLayout.Align();
			bool revivedWithAds = (SaveMeManager.adsFailed = false);
			SaveMeManager.revivedWithAds = revivedWithAds;
			SaveMeManager.adsNotAvailable = SaveMeManager.IsReviveWithAdsAvailable();
			SaveMeManager.videoIsPlaying = false;
			UpdateButtonStates();
		}

		private string GetVideoStatus()
		{
			if (SaveMeManager.HasNotReachedMaxNumberOfTimesToReviveWithAdsInCurrentRun() && SaveMeManager.isRevivalVideoCached)
			{
				return "Available";
			}
			if (!SaveMeManager.HasNotReachedMaxNumberOfTimesToReviveWithAdsInCurrentRun())
			{
				return "NotShown";
			}
			return "NoFill";
		}

		protected override void Hide()
		{
			AdSystemManager.onFailedToShowVideo -= OnShowVideoFailure;
			UIScreenController.Instance.OnOverlayManuallyClosed -= OnVideoOverlayClosed;
			UIScreenController.OnApplicationResumed -= OnApplicationResumed;
		}

		public override void BackButtonPressed()
		{
			if (!UIScreenController.Instance.IsVideoAdsOverlayVisible())
			{
				SaveMeCancelButton.CloseSaveMe();
			}
		}

		public override bool ShouldShowTopBar()
		{
			return false;
		}

		private void CreateTVIcon()
		{
			GameObject gameObject = _animatedTVIconContainer.gameObject.AddChild(_animatedTVIconPrefab);
			gameObject.GetComponent<FitSpriteToWidget>().widgetContainer = _animatedTVIconContainer;
			gameObject.GetComponent<FitSpriteToWidget>().FitNow();
		}

		private void CreateCoinbox()
		{
			UIWidget bgFill = _bgFill;
			Vector3 coinboxOffset = _coinboxOffset;
			float x = coinboxOffset.x;
			Vector3 coinboxOffset2 = _coinboxOffset;
			_coinboxInstance = InitializeCoinbox(bgFill, false, false, true, x, coinboxOffset2.y);
		}

		private void OnApplicationResumed()
		{
			UpdateButtonStates();
		}

		private void UpdateButtonStates()
		{
			bool flag = SaveMeManager.IsReviveWithAdsAvailable();
			bool flag2 = !SaveMeManager.IsOnlyIntroductionVideosAllowed();
			LogAttemptToShowButton();
			_watchAdButton.gameObject.SetActive(flag);
			_keyButton.gameObject.SetActive(flag2);
			_buttonLayout.Align();
			_bgFill.height = 233;
			if (!flag)
			{
				_bgFill.height -= 76;
			}
			if (!flag2)
			{
				_bgFill.height -= 62;
			}
			BroadcastMessage("ResetAndUpdateAnchors");
			if ((bool)_coinboxInstance)
			{
				GameObject coinboxInstance = _coinboxInstance;
				UIWidget bgFill = _bgFill;
				Vector3 coinboxOffset = _coinboxOffset;
				float x = coinboxOffset.x;
				Vector3 coinboxOffset2 = _coinboxOffset;
				UpdateCoinboxPosition(coinboxInstance, bgFill, x, coinboxOffset2.y);
			}
		}

		private void LogAttemptToShowButton()
		{
			if (SaveMeManager.HasNotReachedMaxNumberOfTimesToReviveWithAdsInCurrentRun())
			{
				AdTrackingSystemLogger.LogAttemptToShowButtonFlow(AdLocations.REVIVAL_VIDEOS);
			}
		}

		public void StartCountdown()
		{
			_countdownCoroutine = StartCoroutine(StartClockAnimation());
		}

		private IEnumerator StartClockAnimation()
		{
			animTimeLeft = _timeOutAnimationDuration;
			while (animTimeLeft > -0.2f)
			{
				float spriteAmount = Mathf.Clamp01(animTimeLeft / _timeOutAnimationDuration);
				_saveMeAnimateClock.FillSpriteAmount(spriteAmount);
				animTimeLeft -= Time.deltaTime;
				yield return null;
			}
			UIScreenController.Instance.CloseActivePopups(screenName);
			Revive.Instance.SendSkipRevive();
		}

		private void OnVideoOverlayClosed()
		{
			OnShowVideoFailure(AdCacheSlots.UNDETERMINED_SLOT, AdLocations.UNDETERMINED_LOCATION);
		}

		private void OnShowVideoFailure(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
		{
			SaveMeManager.adsFailed = true;
			SaveMeManager.OnFinishedWatchingVideo("SaveMePopup");
		}

		[UsedImplicitly]
		public void OnWatchAdClicked()
		{
			if (SaveMeManager.isRevivalVideoCached)
			{
				SaveMeManager.PostSaveMeClickEvent(SaveMeManager.ClickAction.WatchAd);
				SaveMeManager.revivedWithAds = true;
				AdTrackingSystemLogger.LogVideoAdButtonClick(AdLocations.REVIVAL_VIDEOS);
				StopCoroutine(_countdownCoroutine);
				AdSystemManager.Show(AdLocations.REVIVAL_VIDEOS, () =>
				{
					SaveMeManager.OnFinishedWatchingVideo("SaveMePopup");
				});
				SaveMeManager.videoIsPlaying = true;
			}
			else if (SaveMeManager.buttonAlwaysOnEnabled)
			{
				ClosePopup();
				UIScreenController.Instance.PushPopup("SaveMeVideoPopup");
			}
		}

		[UsedImplicitly]
		public void OnSpendKeysClicked()
		{
			SaveMeManager.PostSaveMeClickEvent(SaveMeManager.ClickAction.SpendKeys);
			ReviveWithKeys();
		}

		private void ReviveWithKeys()
		{
			SingletonMonoBehaviour<InputManager>.Instance.WasButtonClicked = true;
			int numberOfKeysToSaveMe = SaveMeManager.GetNumberOfKeysToSaveMe();
			if (numberOfKeysToSaveMe <= InventorySave.keys)
			{
				InventorySave.DecreaseKeys(numberOfKeysToSaveMe, "Revive");
				if (InventorySave.keys < 0)
				{
					InventorySave.IncreaseKeys(-InventorySave.keys, "Revive");
				}
				SaveMeManager.IncrementNumberOfUsedKeys(SaveMeManager.ClickAction.SpendKeys);
				SaveMeManager.isPurchaseMadeFromIngame = false;
				SaveMeManager.isPurchaseRunningIngame = false;
				MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendKeys, numberOfKeysToSaveMe);
				SaveMeManager.RevivePlayer();
				UIScreenController.Instance.CloseActivePopups("SaveMePopup");
			}
			else
			{
				SaveMeManager.isPurchaseMadeFromIngame = true;
				SaveMeManager.isPurchaseRunningIngame = true;
				UIScreenController.Instance.ClosePopup();
				PurchaseHandler.Instance.PurchaseKeysIfNeeded(numberOfKeysToSaveMe);
			}
		}
	}
}
