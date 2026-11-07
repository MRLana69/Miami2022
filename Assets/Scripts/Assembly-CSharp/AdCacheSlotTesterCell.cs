using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using UnityEngine;

public class AdCacheSlotTesterCell : MonoBehaviour
{
	[SerializeField]
	private UILabel _statusLabel;

	[SerializeField]
	private GameObject _successIcon;

	private AdCacheSlotInfo _adCacheSlotInfo;

	private const string MAIN_TEXT_COLOR = "[004A80FF]";

	private const string SECONDARY_TEXT_COLOR = "[0374A5FF]";

	public void Init(AdCacheSlotInfo adCacheSlotInfo)
	{
		_adCacheSlotInfo = adCacheSlotInfo;
		UpdateStatusLabel(_adCacheSlotInfo);
	}

	public void Cache()
	{
		AdSystemManager.DebugCacheAdOnSlot(_adCacheSlotInfo);
	}

	public void Show()
	{
		AdSystemManager.DebugShowAdOnSlot(_adCacheSlotInfo, GiveDebugReward);
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
		UpdateStatusLabel(_adCacheSlotInfo);
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
			UpdateStatusLabel(_adCacheSlotInfo);
		}
	}

	private void UpdateStatusLabel(AdCacheSlotInfo adSlotInfo)
	{
		if (adSlotInfo != null)
		{
			_statusLabel.text = string.Format("[c][004A80FF]{0}\n[0374A5FF]cp: {1}", adSlotInfo.id, GetProviderTierForCacheSlot(adSlotInfo));
			_successIcon.SetActive(AdSystemManager.IsCachedDebug(adSlotInfo));
		}
	}

	private string GetProviderTierForCacheSlot(AdCacheSlotInfo adCacheSlotInfo)
	{
		string currentProviderTierForCacheSlot = AdSystemManager.GetCurrentProviderTierForCacheSlot(adCacheSlotInfo);
		return currentProviderTierForCacheSlot.IsNullOrEmpty() ? "none" : currentProviderTierForCacheSlot;
	}

	private void GiveDebugReward()
	{
		UIScreenController.Instance.QueueErrorMessageSlidein("<Empty Debug Reward>");
	}
}
