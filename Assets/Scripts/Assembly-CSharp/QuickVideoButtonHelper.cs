using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Data.Save;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using Kiloo.Common.JsonUtil;
using Kiloo.Common.Timer;
using Kiloo.Common.Utility;
using NaughtyAttributes;
using UI.Screens.RewardManagement;
using UI.VideoAdPrizes;
using UnityEngine;

public class QuickVideoButtonHelper : MonoBehaviour
{
	[SerializeField]
	private UISprite _defaultIconSprite;

	[SerializeField]
	private UIPanel _defaultMaskPanel;

	[SerializeField]
	private Transform _defaultRotateGlint;

	[SerializeField]
	private List<QuickVideoButtonPrizeIconSetup> _prizeIconSetups;

	[SerializeField]
	private UIWidget _flashOverlay;

	[SerializeField]
	private Animation _animation;

	private VideoAdPrize _currentVideoAdPrize;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action onVideoStarted__BackingField;

	private const string EVENT_QUICKVIDEO_BUTTON_SHOWN = "Quick Video Ads Button Shown";

	private const string EVENT_ARG_QUICKVIDEO_VALUE_FORMAT = "step{0}|sinceDailyKeys{1}|{2}";

	private const string EVENT_ARG_QUICKVIDEO_SINCE_DAILY_KEYS = "SinceDailyKeys";

	public event Action onVideoStarted
	{
		add
		{
			Action action = onVideoStarted__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onVideoStarted__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = onVideoStarted__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onVideoStarted__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private void Start()
	{
		_animation.Play();
		Kiloo.Common.Timer.Timer.ExecuteNextFrame(() =>
		{
			_animation.Stop();
		});
	}

	[UsedImplicitly]
	private void OnClick()
	{
		Flurry.LogEventWithAParameter("Quick Video Ads button clicked", "Reward", _currentVideoAdPrize.AsString());
		AdTrackingSystemLogger.LogVideoAdButtonClick(AdLocations.HOME_VIDEOS);
		StartCoroutine(StartVideoWithDelay());
	}

	private IEnumerator StartVideoWithDelay()
	{
		yield return new WaitForEndOfFrame();
		AdSystemManager.Show(AdLocations.HOME_VIDEOS, () =>
		{
			VideoAdPrizeManager.instance.GiveReward(UnlockItemSource.VideoAdPrizeHome);
		});
		onVideoStarted__BackingField.SafeInvoke();
	}

	public void SetPrize(VideoAdPrize prize)
	{
		_currentVideoAdPrize = prize;
		List<QuickVideoButtonPrizeIconSetup> prizeIconSetupsWithTypeAndDeactivateTheRest = PrizeIconSetup.GetPrizeIconSetupsWithTypeAndDeactivateTheRest(_prizeIconSetups, prize.prizeType);
		foreach (QuickVideoButtonPrizeIconSetup item in prizeIconSetupsWithTypeAndDeactivateTheRest)
		{
			item.Setup();
		}
		LogShownButtonToFlurry();
	}

	public void PlayAnimation()
	{
		_animation.Play();
	}

	public void FlashOverlay()
	{
		StartCoroutine(FlashOverlayCoroutine());
	}

	private IEnumerator FlashOverlayCoroutine()
	{
		float fadeInDuration = 0.2f;
		float fadeOutDuration = 0.3f;
		float currentDuration = 0f;
		float minAlpha = 0f;
		float maxAlpha = 1f;
		while (currentDuration < fadeInDuration)
		{
			_flashOverlay.alpha = Mathf.Lerp(minAlpha, maxAlpha, currentDuration / fadeInDuration);
			currentDuration += Time.unscaledDeltaTime;
			yield return null;
		}
		currentDuration = 0f;
		while (currentDuration < fadeOutDuration)
		{
			_flashOverlay.alpha = maxAlpha - Easings.QuadraticEaseOut(currentDuration / fadeOutDuration);
			currentDuration += Time.unscaledDeltaTime;
			yield return null;
		}
		DisableFlashOverlay();
	}

	public void DisableFlashOverlay()
	{
		_flashOverlay.alpha = 0f;
		_flashOverlay.gameObject.SetActive(false);
	}

	[Button("Set default prize icon properties(if not set)")]
	private void SetDefaultPrizeIconProperties()
	{
		foreach (QuickVideoButtonPrizeIconSetup prizeIconSetup in _prizeIconSetups)
		{
			if (prizeIconSetup.iconSprite == null)
			{
				prizeIconSetup.iconSprite = _defaultIconSprite;
			}
			if (prizeIconSetup.maskPanel == null)
			{
				prizeIconSetup.maskPanel = _defaultMaskPanel;
			}
			if (prizeIconSetup.unrotateGlint == null)
			{
				prizeIconSetup.unrotateGlint = _defaultRotateGlint;
			}
		}
	}

	private void LogShownButtonToFlurry()
	{
		if (_currentVideoAdPrize.prizeType == VideoAdPrizeType.DailyVideoPrize)
		{
			AdsSave.videosWatchedSinceDailyKeys = 0;
		}
		VideoAdPrizeManager instance = VideoAdPrizeManager.instance;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		PrizePoolState prizePoolState = instance.prizePoolState;
		int num = ((prizePoolState != PrizePoolState.Intro) ? AdsSave.currentRandomVideoAdPrizeIndex : AdsSave.currentIntroVideoAdPrizeIndex);
		int videosWatchedSinceDailyKeys = AdsSave.videosWatchedSinceDailyKeys;
		dictionary.Add(prizePoolState.ToString(), "step{0}|sinceDailyKeys{1}|{2}".Populate(num, videosWatchedSinceDailyKeys, _currentVideoAdPrize.AsString()));
		dictionary.Add("SinceDailyKeys" + prizePoolState, AdsSave.videosWatchedSinceDailyKeys.ToString());
		Flurry.LogEventWithSeveralParameters("Quick Video Ads Button Shown", dictionary);
	}
}
