using System.Collections;
using System.Collections.Generic;
using Data.Save;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using Kiloo.Common.JsonUtil;
using Kiloo.Common.Utility;
using UI;
using UI.Generic.Layout;
using UI.VideoAdPrizes;
using UnityEngine;

public class CoinEarnerVideoAds : MonoBehaviour
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
	private GameObject _loadingSprite;

	[SerializeField]
	private GameObject _noConnectionSprite;

	[SerializeField]
	private GameObject _tvObject;

	[SerializeField]
	private GameObject _starPrefab;

	[SerializeField]
	private UIButtonOverlayOff _buttonOverlayController;

	[SerializeField]
	private GameObject _prizeContent;

	[SerializeField]
	private UILabel _title;

	[SerializeField]
	private UILabel _description;

	[SerializeField]
	private UILabel _buttonText;

	[SerializeField]
	private HorizontalLayout _buttonLayout;

	[SerializeField]
	private int _prizeIconDepth;

	[SerializeField]
	private UISprite _spritePrefab;

	[SerializeField]
	private float _starAnimationPause;

	[SerializeField]
	private List<EarnCoinButtonPrizeIconSetup> _prizeIconSetups;

	private const int EARN_CURRENCY_PROFILE_INDEX = 0;

	private ButtonState _currentButtonState;

	private List<UISprite> _prizeIcons;

	private bool _hasSubscribedToEvents;

	private VideoAdPrize _currentVideoAdPrize;

	private const float CHECK_CACHE_STATE_TIMEOUT = 10f;

	private const int CHECK_CACHE_STATE_COUNT = 3;

	private IEnumerator _checkCacheState;

	private bool _runAnimations;

	private GameObject _star;

	public void Init()
	{
		_prizeIcons = new List<UISprite>();
		BaseSetup();
		SetPrizeIconState();
		SubscribeToEvents();
		RefreshButton();
	}

	private void OnEnable()
	{
		SubscribeToEvents();
		if (_prizeIcons != null)
		{
			SetPrizeIconState();
		}
	}

	private void OnDisable()
	{
		AdTrackingSystemLogger.LogAttemptToShowButtonFlow(AdLocations.EARN_COINS_VIDEOS, _currentButtonState == ButtonState.VideoReady);
		UnsubscribeToEvents();
		CoroutineUtil.StopCoroutine(_checkCacheState);
		_runAnimations = false;
	}

	private void OnDestroy()
	{
		UnsubscribeToEvents();
		CoroutineUtil.StopCoroutine(_checkCacheState);
	}

	private void SubscribeToEvents()
	{
		if (!_hasSubscribedToEvents)
		{
			_hasSubscribedToEvents = true;
			UIScreenController.OnApplicationResumed += UIScreenController_OnApplicationResumed;
			AdSystemManager.onFinishedWatchingVideo += OnFinishedWatchingVideo;
			AdSystemManager.onCachedVideo += OnVideoCached;
		}
	}

	private void UnsubscribeToEvents()
	{
		if (_hasSubscribedToEvents)
		{
			_hasSubscribedToEvents = false;
			UIScreenController.OnApplicationResumed -= UIScreenController_OnApplicationResumed;
			AdSystemManager.onFinishedWatchingVideo -= OnFinishedWatchingVideo;
			AdSystemManager.onCachedVideo -= OnVideoCached;
		}
	}

	private void OnFinishedWatchingVideo(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
	{
		SetPrizeIconState();
		RefreshButton();
	}

	private void OnVideoCached(AdCacheSlotInfo adCacheSlotInfo, bool wasAlreadyCached)
	{
		CoroutineUtil.StopCoroutine(_checkCacheState);
		SetButtonState(ButtonState.VideoReady);
	}

	private ButtonState RefreshButton()
	{
		ButtonState buttonState = (AdSystemManager.IsCached(AdLocations.EARN_COINS_VIDEOS) ? ButtonState.VideoReady : ButtonState.Loading);
		SetButtonState(buttonState);
		return buttonState;
	}

	private void UIScreenController_OnApplicationResumed()
	{
		RefreshButton();
	}

	[UsedImplicitly]
	public void OnClick()
	{
		switch (_currentButtonState)
		{
		case ButtonState.NoVideo:
			Flurry.LogEventWithAParameter("Video Ads button clicked", "State;Reward", string.Concat(ButtonState.NoVideo, ";No Reward"));
			RefreshButton();
			break;
		case ButtonState.VideoReady:
			if (AdSystemManager.IsCached(AdLocations.EARN_COINS_VIDEOS))
			{
				AdTrackingSystemLogger.LogVideoAdButtonClick(AdLocations.EARN_COINS_VIDEOS);
				SetButtonState(ButtonState.Loading);
				Flurry.LogEventWithAParameter("Video Ads button clicked", "State;Reward", string.Concat(ButtonState.VideoReady, ";", _currentVideoAdPrize.AsString()));
				EarnCoinsData.Trigger(0);
				_runAnimations = false;
			}
			else
			{
				RefreshButton();
			}
			break;
		}
	}

	private void BaseSetup()
	{
		_backgroundSprite.gradientTop = NGUIMath.HexToColor(4286841343u);
		_backgroundSprite.gradientBottom = NGUIMath.HexToColor(4291559679u);
		_outlineSprite.gradientTop = NGUIMath.HexToColor(3596747007u);
		_outlineSprite.gradientBottom = NGUIMath.HexToColor(3985834239u);
		_glowSprite.color = NGUIMath.HexToColor(4294084095u);
		_title.color = ColorData.instance.mahogany;
		_title.effectColor = NGUIMath.HexToColor(4291199231u);
		_description.color = NGUIMath.HexToColor(uint.MaxValue);
		_description.effectColor = ColorData.instance.shadow;
		_title.text = Strings.Get("FREE_STUFF_DAILY_VIDEO");
		_description.text = Strings.Get("FREE_STUFF_DAILY_VIDEO_DESC");
		_buttonText.text = Strings.Get("FREE_STUFF_WATCH_VIDEO");
		_buttonLayout.Align();
	}

	private void SetButtonState(ButtonState state)
	{
		if (!(base.gameObject == null) && base.gameObject.activeInHierarchy)
		{
			_currentButtonState = state;
			switch (state)
			{
			case ButtonState.VideoReady:
				_buttonOverlayController.GoToNormalState();
				_loadingSprite.SetActive(false);
				_noConnectionSprite.SetActive(false);
				_tvObject.SetActive(true);
				_buttonText.text = Strings.Get("FREE_STUFF_WATCH_VIDEO");
				AdTrackingSystemLogger.LogAttemptToShowButtonFlow(AdLocations.EARN_COINS_VIDEOS, true);
				break;
			case ButtonState.Loading:
				_buttonOverlayController.GoToNotAvailableState();
				_loadingSprite.SetActive(true);
				_noConnectionSprite.SetActive(false);
				_tvObject.SetActive(false);
				_buttonText.text = Strings.Get("EARN_COINS_LOADING");
				CoroutineUtil.StopCoroutine(_checkCacheState);
				_checkCacheState = CheckCacheState();
				CoroutineUtil.StartCoroutine(_checkCacheState);
				break;
			case ButtonState.NoVideo:
				_buttonOverlayController.GoToNotAvailableState();
				_loadingSprite.SetActive(false);
				_noConnectionSprite.SetActive(true);
				_tvObject.SetActive(false);
				_buttonText.text = ((Application.internetReachability != NetworkReachability.NotReachable) ? Strings.Get("EARN_COINS_LOADING_FAILED") : Strings.Get("FREE_STUFF_NO_CONNECTION"));
				AdTrackingSystemLogger.LogAttemptToShowButtonFlow(AdLocations.EARN_COINS_VIDEOS, "NoFillButtonTimeout");
				break;
			}
			_buttonLayout.Align();
			if (state == ButtonState.VideoReady)
			{
				LogButtonShownToFlurry(_currentVideoAdPrize);
			}
		}
	}

	private IEnumerator CheckCacheState()
	{
		float delay = 3.3333333f;
		for (int i = 0; i < 3; i++)
		{
			yield return new WaitForSeconds(delay);
			if (AdSystemManager.IsCached(AdLocations.EARN_COINS_VIDEOS))
			{
				SetButtonState(ButtonState.VideoReady);
				yield break;
			}
			if (AdSystemManager.HaveAllCachePoolPriorityListsWrapped(AdLocations.EARN_COINS_VIDEOS))
			{
				break;
			}
		}
		SetButtonState(ButtonState.NoVideo);
	}

	private void SetPrizeIconState()
	{
		_currentVideoAdPrize = VideoAdPrizeManager.instance.GetVideoAdPrize();
		List<EarnCoinButtonPrizeIconSetup> prizeIconSetupsWithTypeAndDeactivateTheRest = PrizeIconSetup.GetPrizeIconSetupsWithTypeAndDeactivateTheRest(_prizeIconSetups, _currentVideoAdPrize.prizeType);
		if (_star == null)
		{
			_star = Object.Instantiate(_starPrefab, _prizeContent.transform);
		}
		_star.transform.localPosition = prizeIconSetupsWithTypeAndDeactivateTheRest[0].starPosition;
		for (int i = 0; i < prizeIconSetupsWithTypeAndDeactivateTheRest.Count; i++)
		{
			prizeIconSetupsWithTypeAndDeactivateTheRest[i].iconSprite = GetPrizeIcon(i);
			prizeIconSetupsWithTypeAndDeactivateTheRest[i].Setup();
		}
		_runAnimations = true;
		StartCoroutine(ActivateStars());
	}

	private UISprite GetPrizeIcon(int index)
	{
		while (index >= _prizeIcons.Count)
		{
			UISprite uISprite = Object.Instantiate(_spritePrefab, _prizeContent.transform);
			uISprite.depth = _prizeIconDepth;
			_prizeIcons.Add(uISprite);
		}
		return _prizeIcons[index];
	}

	private IEnumerator ActivateStars()
	{
		yield return new WaitForSeconds(1.6f);
		while (_runAnimations)
		{
			int children = _star.transform.childCount;
			for (int i = 0; i < children; i++)
			{
				_star.transform.GetChild(i).GetComponent<Animation>().Play();
			}
			yield return new WaitForSeconds(_starAnimationPause);
		}
	}

	private void LogButtonShownToFlurry(VideoAdPrize videoAdPrize)
	{
		if (videoAdPrize.prizeType == VideoAdPrizeType.DailyVideoPrize)
		{
			AdsSave.videosWatchedSinceDailyKeys = 0;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		PrizePoolState prizePoolState = VideoAdPrizeManager.instance.prizePoolState;
		int num = ((prizePoolState != PrizePoolState.Intro) ? AdsSave.currentRandomVideoAdPrizeIndex : AdsSave.currentIntroVideoAdPrizeIndex);
		int videosWatchedSinceDailyKeys = AdsSave.videosWatchedSinceDailyKeys;
		dictionary.Add(prizePoolState.ToString(), "step{0}|sinceDailyKeys{1}|{2}".Populate(num, videosWatchedSinceDailyKeys, videoAdPrize.AsString()));
		dictionary.Add("SinceDailyKeys" + prizePoolState, AdsSave.videosWatchedSinceDailyKeys.ToString());
		Flurry.LogEventWithSeveralParameters("Free Stuff Quick Video Ads Button Shown", dictionary);
	}
}
