using Kiloo.Common.Ads;
using UnityEngine;

public class AdProviderTesterCell : MonoBehaviour
{
	[SerializeField]
	private UILabel _providerName;

	[SerializeField]
	private GameObject _cachedIcon;

	[SerializeField]
	private GameObject _cachingIcon;

	[SerializeField]
	private GameObject _noFillIcon;

	private AdProviderInfo _adProviderInfo;

	private AdCacheSlotInfo _adCacheSlotInfo;

	private const string MAIN_TEXT_COLOR = "[004A80FF]";

	private const string SECONDARY_TEXT_COLOR = "[0374A5FF]";

	private string _cacheID;

	public void Init(AdProviderInfo providerInfo, AdCacheSlotInfo adCacheSlotInfo, string cacheID)
	{
		_adProviderInfo = providerInfo;
		_adCacheSlotInfo = adCacheSlotInfo;
		_cacheID = cacheID;
		UpdateStatusLabel(_adProviderInfo);
	}

	public void Cache()
	{
		AdSystemManager.DebugCacheAdForProvider(_adProviderInfo, _adCacheSlotInfo);
	}

	public void Show()
	{
		AdSystemManager.DebugShowAdForProvider(_adProviderInfo, _adCacheSlotInfo, GiveDebugReward);
	}

	private void OnEnable()
	{
		AdSystemManager.onCachedVideo += OnCacheStateChanged;
		AdSystemManager.onCachedInterstitial += OnCacheStateChanged;
		AdSystemManager.onFailedToCacheVideo += OnCacheStateChanged;
		AdSystemManager.onFailedToCacheInterstitial += OnCacheStateChanged;
		AdSystemManager.onShowedVideo += OnCacheStateChanged;
		AdSystemManager.onShowedInterstitial += OnCacheStateChanged;
		AdSystemManager.onFailedToShowVideo += OnCacheStateChanged;
		AdSystemManager.onFailedToShowInterstitial += OnCacheStateChanged;
		AdSystemManager.onDismissedVideo += OnCacheStateChanged;
		AdSystemManager.onDismissedInterstitial += OnCacheStateChanged;
		AdSystemManager.onPriorityListWrapped += OnCacheStateChanged;
		AdSystemManager.onChangedProvider += OnCacheStateChanged;
		UpdateStatusLabel(_adProviderInfo);
	}

	private void OnDisable()
	{
		AdSystemManager.onCachedVideo -= OnCacheStateChanged;
		AdSystemManager.onCachedInterstitial -= OnCacheStateChanged;
		AdSystemManager.onFailedToCacheVideo -= OnCacheStateChanged;
		AdSystemManager.onFailedToCacheInterstitial -= OnCacheStateChanged;
		AdSystemManager.onShowedVideo -= OnCacheStateChanged;
		AdSystemManager.onShowedInterstitial -= OnCacheStateChanged;
		AdSystemManager.onFailedToShowVideo -= OnCacheStateChanged;
		AdSystemManager.onFailedToShowInterstitial -= OnCacheStateChanged;
		AdSystemManager.onDismissedVideo -= OnCacheStateChanged;
		AdSystemManager.onDismissedInterstitial -= OnCacheStateChanged;
		AdSystemManager.onPriorityListWrapped -= OnCacheStateChanged;
		AdSystemManager.onChangedProvider -= OnCacheStateChanged;
	}

	private void OnCacheStateChanged(string waterfallId)
	{
		if (_adCacheSlotInfo != null)
		{
			OnCacheStateChanged(_adCacheSlotInfo, false);
		}
	}

	private void OnCacheStateChanged(AdCacheSlotInfo adCacheSlotInfo, AdLocationInfo adLocationInfo)
	{
		OnCacheStateChanged(adCacheSlotInfo, false);
	}

	private void OnCacheStateChanged(AdCacheSlotInfo adCacheSlotInfo)
	{
		OnCacheStateChanged(adCacheSlotInfo, false);
	}

	private void OnCacheStateChanged(AdCacheSlotInfo adCacheSlotInfo, bool wasAlreadyCached)
	{
		if (_adCacheSlotInfo == adCacheSlotInfo)
		{
			UpdateStatusLabel(_adProviderInfo);
		}
	}

	private void UpdateStatusLabel(AdProviderInfo adProviderInfo)
	{
		if (adProviderInfo != null)
		{
			_providerName.text = string.Format("[c][004A80FF]{0}\n[0374A5FF][sub]cid : {1}", adProviderInfo.providerTier, _cacheID);
			bool flag = AdSystemManager.IsProviderCachedDebug(adProviderInfo, _adCacheSlotInfo);
			bool flag2 = AdSystemManager.IsProviderCachingDebug(adProviderInfo, _adCacheSlotInfo);
			_cachedIcon.SetActive(flag);
			_cachingIcon.SetActive(flag2);
			_noFillIcon.SetActive(!flag2 && !flag);
		}
	}

	private string GetProviderIDForCacheSlot(AdCacheSlotInfo adCacheSlotInfo)
	{
		string currentProviderIDForCacheSlot = AdSystemManager.GetCurrentProviderIDForCacheSlot(adCacheSlotInfo);
		string text = "none";
		if (currentProviderIDForCacheSlot == text)
		{
			return text;
		}
		if (!string.IsNullOrEmpty(currentProviderIDForCacheSlot))
		{
			text = currentProviderIDForCacheSlot;
		}
		return text;
	}

	private void GiveDebugReward()
	{
		UIScreenController.Instance.QueueErrorMessageSlidein("<Empty Debug Reward>");
	}
}
