using System.Collections;
using System.Diagnostics;
using JetBrains.Annotations;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Common.JsonUtil;
using Kiloo.Common.Utility;
using SYBO.Subway.Info;
using UI.Generic;
using UI.Generic.Layout;
using UI.Helpers;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace UI.FreeStuff
{
	public class RewardForVideoButton : MonoBehaviour
	{
		private enum VideoState
		{
			Ready = 0,
			Loading = 1,
			NoFill = 2
		}

		[SerializeField]
		private UISprite _firstFill;

		[SerializeField]
		private UISprite _firstFillHighlight;

		[SerializeField]
		private UISprite _secondFill;

		[SerializeField]
		private UISprite _secondFillHighlight;

		[SerializeField]
		private UISprite _rewardIcon;

		[SerializeField]
		private UISprite _rewardIconBackground;

		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _description;

		[SerializeField]
		private UILabel _loadingLabel;

		[SerializeField]
		private UILabel _noVideosLabel;

		[SerializeField]
		private UILabel _noVideosTryAgainLabel;

		[SerializeField]
		private GameObject _activeState;

		[SerializeField]
		private GameObject _loadingState;

		[SerializeField]
		private GameObject _noFillState;

		[Space(10f)]
		[SerializeField]
		private UILabel _bonusSticker;

		[SerializeField]
		private HorizontalLayout _multiplierLayout;

		[SerializeField]
		private UILabel _multiplierRewardLabel;

		[SerializeField]
		private UISprite _rewardSprite;

		[SerializeField]
		private int _rewardSpriteWidth = 32;

		[SerializeField]
		private UILabel _btnGetLabel;

		private VideoButtonPayload _payload;

		private VideoState _state = VideoState.NoFill;

		private bool _subscribed;

		private int _cachedRewardAmount;

		private ButtonPressedColorChange[] _colorPressedManager;

		private IEnumerator _cacheCoroutine;

		private bool _buttonClicked;

		private bool _canAutoplay;

		private const float CHECK_CACHE_STATE_TIMEOUT = 10f;

		private const int CHECK_CACHE_STATE_COUNT = 3;

		private const string FLURRY_PARAMETER_VALUE_FORMAT = "{0};{1}";

		public int cachedRewardAmount
		{
			get
			{
				return _cachedRewardAmount;
			}
		}

		public void Init(VideoButtonPayload payload)
		{
			_buttonClicked = false;
			_canAutoplay = false;
			_colorPressedManager = GetComponents<ButtonPressedColorChange>();
			_payload = payload;
			_cachedRewardAmount = payload.getRewardAmount.SafeInvoke();
			SetupBasic();
			RefreshState();
			Subscribe();
		}

		private void SetupBasic()
		{
			if (_rewardIcon != null && _rewardIconBackground != null)
			{
				_rewardIcon.spriteName = _payload.iconName;
				_rewardIcon.MakePixelPerfect();
				_rewardIconBackground.spriteName = ((!string.IsNullOrEmpty(_payload.iconBackgroundName)) ? _payload.iconBackgroundName : "icon_background_normal");
			}
			if (_title != null)
			{
				_title.text = Strings.Get(_payload.title);
				_title.color = _payload.titleColor;
			}
			if (_description != null)
			{
				_description.text = Strings.Get(_payload.description).Populate(_cachedRewardAmount);
				_description.color = _payload.descriptionColor;
			}
			_loadingLabel.text = Strings.Get("EARN_COINS_LOADING");
			_noVideosLabel.text = Strings.Get("EARN_COINS_LOADING_FAILED");
			_noVideosTryAgainLabel.text = Strings.Get("BUTTON_TRY_AGAIN");
			if (_btnGetLabel != null)
			{
				_btnGetLabel.text = Strings.Get("CELEBRATION_POPUP_GET");
			}
			_firstFill.color = _payload.firstColor;
			_firstFillHighlight.color = _payload.firstHighlightColor;
			_secondFill.color = _payload.secondColor;
			_secondFillHighlight.color = _payload.secondHighlightColor;
			_colorPressedManager[0].normalColor = _payload.firstColor;
			_colorPressedManager[0].pressedColor = _payload.firstColorDark;
			_colorPressedManager[1].normalColor = _payload.secondColor;
			_colorPressedManager[1].pressedColor = _payload.secondColorDark;
		}

		public void RefreshState()
		{
			bool flag = AdSystemManager.IsCached(_payload.adLocation);
			if (_payload.alwaysOn && !_buttonClicked)
			{
				SetState(VideoState.Ready);
			}
			else
			{
				SetState((!flag) ? VideoState.Loading : VideoState.Ready);
			}
		}

		private void SetState(VideoState state)
		{
			if (_state == state)
			{
				return;
			}
			_state = state;
			_activeState.SetActive(false);
			_loadingState.SetActive(false);
			_noFillState.SetActive(false);
			if (state == VideoState.Ready && _payload.autoPlayOnFill && _canAutoplay)
			{
				PlayVideo();
				return;
			}
			if (_state == VideoState.Ready)
			{
				_activeState.SetActive(true);
			}
			else if (_state == VideoState.Loading)
			{
				_loadingState.SetActive(true);
			}
			else if (_state == VideoState.NoFill)
			{
				_noFillState.SetActive(true);
			}
			_firstFill.enabled = state != VideoState.Loading;
			_secondFill.enabled = state != VideoState.Loading;
			_firstFillHighlight.enabled = state != VideoState.Loading;
			_secondFillHighlight.enabled = state != VideoState.Loading;
			switch (state)
			{
			case VideoState.Ready:
				LogButtonShownToFlurry();
				break;
			case VideoState.Loading:
				CoroutineUtil.StopCoroutine(_cacheCoroutine);
				_cacheCoroutine = CheckCacheState();
				CoroutineUtil.StartCoroutine(_cacheCoroutine);
				break;
			case VideoState.NoFill:
				LogButtonHasNoFill();
				UpdateNoFillState();
				break;
			}
		}

		private void UpdateNoFillState()
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				_noVideosLabel.text = Strings.Get("CLOUDSAVE_STATUS_OFFLINE_INFO");
			}
			else
			{
				_noVideosLabel.text = Strings.Get("EARN_COINS_LOADING_FAILED");
			}
		}

		[UsedImplicitly]
		private void OnClick()
		{
			_buttonClicked = true;
			_canAutoplay = true;
			_payload.onButtonClicked.SafeInvoke();
			switch (_state)
			{
			case VideoState.Ready:
				PlayVideo();
				break;
			case VideoState.NoFill:
				RefreshState();
				break;
			}
		}

		private void PlayVideo()
		{
			if (AdSystemManager.IsCached(_payload.adLocation))
			{
				_canAutoplay = false;
				LogButtonClickedToFlurry();
				AdSystemManager.Show(_payload.adLocation, OnVideoWatched);
			}
			SetState(VideoState.Loading);
		}

		private void ResetButtonState()
		{
			if (_payload != null && _payload.alwaysOn)
			{
				SetState(VideoState.Ready);
				return;
			}
			_state = VideoState.NoFill;
			_activeState.SetActive(false);
			_loadingState.SetActive(false);
			_noFillState.SetActive(true);
		}

		private void OnVideoWatched()
		{
			_payload.onVideoWatched.SafeInvoke();
			if (_payload.rewardType != CelebrationRewardType._notset)
			{
				CelebrationReward celebrationReward = new CelebrationReward();
				celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.UnlockItem;
				celebrationReward.rewardType = _payload.rewardType;
				celebrationReward.upgradeType = _payload.upgradeType;
				celebrationReward.amount = _cachedRewardAmount;
				celebrationReward.syboAnalyticsSourceMetaOrNull = _payload.unlockItemSource.ToString();
				celebrationReward.unlockItemSource = _payload.unlockItemSource;
				CelebrationReward reward = celebrationReward;
				RewardManager.AddRewardToUnlock(reward);
				UIScreenController.Instance.PushCelebrationPopup();
			}
		}

		private void OnEnable()
		{
			ResetButtonState();
			Subscribe();
		}

		private void OnDisable()
		{
			AdTrackingSystemLogger.LogAttemptToShowButtonFlow(_payload.adLocation, AdSystemManager.IsCached(_payload.adLocation));
			CoroutineUtil.StopCoroutine(_cacheCoroutine);
			Unsubscribe();
		}

		private void Subscribe()
		{
			if (!_subscribed)
			{
				_subscribed = true;
				AdSystemManager.onCachedVideo += OnCachedVideo;
				AdSystemManager.onDismissedVideo += OnDismissedVideo;
				UIScreenController.OnApplicationResumed += OnApplicationResumed;
			}
		}

		private void OnCachedVideo(AdCacheSlotInfo adCacheSlotInfo, bool wasAlreadyCached)
		{
			SetState(VideoState.Ready);
		}

		private void OnApplicationResumed()
		{
			_canAutoplay = false;
			RefreshState();
		}

		private void OnDismissedVideo(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
		{
			RefreshState();
		}

		private void Unsubscribe()
		{
			if (_subscribed)
			{
				_subscribed = false;
				AdSystemManager.onCachedVideo -= OnCachedVideo;
				AdSystemManager.onDismissedVideo -= OnDismissedVideo;
				UIScreenController.OnApplicationResumed -= OnApplicationResumed;
			}
		}

		private IEnumerator CheckCacheState()
		{
			float delay = 3.3333333f;
			for (int i = 0; i < 3; i++)
			{
				yield return new WaitForSeconds(delay);
				bool hasFill = AdSystemManager.IsCached(_payload.adLocation);
				if (i == 0 && !string.IsNullOrEmpty(_payload.flurryName))
				{
					Flurry.LogEventWithAParameter("Attempt to fill {0}".Populate(_payload.flurryName), "Result", (!hasFill) ? "NoFill" : "Fill");
				}
				if (hasFill)
				{
					SetState(VideoState.Ready);
					yield break;
				}
				if (AdSystemManager.HaveAllCachePoolPriorityListsWrapped(_payload.adLocation))
				{
					break;
				}
			}
			SetState(VideoState.NoFill);
		}

		private void LogButtonShownToFlurry()
		{
			if (AdSystemManager.IsCached(_payload.adLocation))
			{
				AdTrackingSystemLogger.LogAttemptToShowButtonFlow(_payload.adLocation, true);
				LogToFlurryWithParameter("Shown");
			}
		}

		private void LogButtonClickedToFlurry()
		{
			if (AdSystemManager.IsCached(_payload.adLocation))
			{
				AdTrackingSystemLogger.LogVideoAdButtonClick(_payload.adLocation);
				LogToFlurryWithParameter("Clicked");
			}
		}

		private void LogButtonHasNoFill()
		{
			AdTrackingSystemLogger.LogAttemptToShowButtonFlow(_payload.adLocation, "NoFillButtonTimeout");
			LogToFlurryWithParameter("NoFill");
		}

		private void LogToFlurryWithParameter(string parameter)
		{
			string text = ((_payload.rewardType != CelebrationRewardType.powerup) ? _payload.rewardType.ToString() : _payload.upgradeType.ToString());
			string text2 = _cachedRewardAmount.ToString();
			string argValue = "{0};{1}".Populate(text, text2);
			Flurry.LogEventWithAParameter("Free Stuff Video Button", parameter, argValue);
		}

		[Conditional("VIDEO_BUTTON_LOGS")]
		private static void Log(string msg, Object obj = null)
		{
		}

		public void UpdateText(MysteryBox.Multipliers multiplier, CelebrationReward reward)
		{
			base.gameObject.SetActive(true);
			switch (multiplier)
			{
			case MysteryBox.Multipliers.x2:
				_title.gameObject.SetActive(true);
				_multiplierLayout.gameObject.SetActive(false);
				break;
			case MysteryBox.Multipliers.x3:
			case MysteryBox.Multipliers.x4:
				_title.gameObject.SetActive(false);
				_multiplierLayout.gameObject.SetActive(true);
				_multiplierRewardLabel.text = (reward.amount * (int)multiplier).ToString();
				_rewardSprite.spriteName = GetIconName(reward);
				_rewardSprite.width = _rewardSpriteWidth;
				_rewardSprite.height = _rewardSpriteWidth * _rewardSprite.GetAtlasSprite().height / _rewardSprite.GetAtlasSprite().width;
				_multiplierLayout.Align();
				break;
			case MysteryBox.Multipliers.NotSet:
				base.gameObject.SetActive(false);
				break;
			}
			_bonusSticker.text = multiplier.ToString();
		}

		private string GetIconName(CelebrationReward reward)
		{
			if (reward.rewardType == CelebrationRewardType.token)
			{
				return Characters.characterData[reward.characterId].tokenSprite2dName;
			}
			return RewardToIcon.GetSpriteNameFromType(reward.rewardType, reward.upgradeType);
		}
	}
}
