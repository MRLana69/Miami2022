using System;
using System.Collections;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Common.JsonUtil;
using Kiloo.Common.Utility;
using UI.Generic;
using UI.Screens;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace UI.FreeStuff
{
	public class RewardForVideoButtonController
	{
		private enum VideoState
		{
			Ready = 0,
			Loading = 1,
			NoFill = 2
		}

		private VideoButtonPayload _payload;

		private VideoState _state = VideoState.NoFill;

		private bool _subscribed;

		private int _cachedRewardAmount;

		private ButtonPressedColorChange[] _colorPressedManager;

		private IEnumerator _cacheCoroutine;

		public Action setButtonToActiveState;

		public Action setButtonToLoadingState;

		public Action setButtonToInactiveState;

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

		public RewardForVideoButtonController(VideoButtonPayload payload)
		{
			_payload = payload;
			_cachedRewardAmount = payload.getRewardAmount.SafeInvoke();
		}

		public void Init()
		{
			RefreshState();
			Subscribe();
		}

		private void RefreshState()
		{
			bool flag = AdSystemManager.IsCached(_payload.adLocation);
			SetState((!flag) ? VideoState.Loading : VideoState.Ready);
		}

		private void SetState(VideoState state)
		{
			if (_state != state)
			{
				_state = state;
				switch (state)
				{
				case VideoState.Ready:
					setButtonToActiveState.SafeInvoke();
					LogButtonShownToFlurry();
					break;
				case VideoState.Loading:
					setButtonToLoadingState.SafeInvoke();
					CoroutineUtil.StopCoroutine(_cacheCoroutine);
					_cacheCoroutine = CheckCacheState();
					CoroutineUtil.StartCoroutine(_cacheCoroutine);
					break;
				case VideoState.NoFill:
					setButtonToInactiveState.SafeInvoke();
					LogButtonHasNoFill();
					break;
				}
			}
		}

		public void OnClick()
		{
			switch (_state)
			{
			case VideoState.Ready:
			{
				bool flag = AdSystemManager.IsCached(_payload.adLocation);
				SetState(VideoState.Loading);
				if (flag)
				{
					LogButtonClickedToFlurry();
					AdSystemManager.Show(_payload.adLocation, OnVideoWatched);
				}
				break;
			}
			case VideoState.NoFill:
				RefreshState();
				break;
			}
		}

		private void OnVideoWatched()
		{
			_payload.onVideoWatched.SafeInvoke();
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
			if (!_payload.isAvailable.SafeInvoke())
			{
				FreeStuffScreen.updateScreen = true;
			}
		}

		private void OnEnable()
		{
			Subscribe();
		}

		private void OnDisable()
		{
			AdTrackingSystemLogger.LogAttemptToShowButtonFlow(_payload.adLocation, _state == VideoState.Ready);
			CoroutineUtil.StopCoroutine(_cacheCoroutine);
			Unsubscribe();
		}

		public void Subscribe()
		{
			if (!_subscribed)
			{
				_subscribed = true;
				AdSystemManager.onCachedVideo += OnCachedVideo;
				AdSystemManager.onDismissedVideo += OnDismissedVideo;
				AdSystemManager.onFailedToCacheVideo += OnFailedToCacheVideo;
				UIScreenController.OnApplicationResumed += RefreshState;
			}
		}

		private void OnCachedVideo(AdCacheSlotInfo adCacheSlotInfo, bool wasAlreadyCached)
		{
			SetState(VideoState.Ready);
		}

		private void OnFailedToCacheVideo(AdCacheSlotInfo adCacheSlotInfo)
		{
			if (_state == VideoState.Loading)
			{
				SetState(VideoState.NoFill);
			}
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
				AdSystemManager.onFailedToCacheVideo -= OnFailedToCacheVideo;
				UIScreenController.OnApplicationResumed -= RefreshState;
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
			}
			SetState(VideoState.NoFill);
		}

		private void LogButtonShownToFlurry()
		{
			AdTrackingSystemLogger.LogAttemptToShowButtonFlow(_payload.adLocation, true);
			LogToFlurryWithParameter("Shown");
		}

		private void LogButtonClickedToFlurry()
		{
			AdTrackingSystemLogger.LogVideoAdButtonClick(_payload.adLocation);
			LogToFlurryWithParameter("Clicked");
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
	}
}
