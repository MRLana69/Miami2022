using System.Diagnostics;
using Info;
using Kiloo.Common.Extensions;
using UI;
using UI.FreeStuff;
using UI.Generic.Layout;
using UI.Popups.WelcomeBackRewardPopupInternal;
using UI.Screens.RewardManagement;
using UnityEngine;

public class WelcomeBackRewardItemButton : MonoBehaviour
{
	public enum ItemState
	{
		FreeItem = 0,
		WatchAdForItem = 1
	}

	private const ItemState DEFAULT_STATE = ItemState.FreeItem;

	private WelcomeBackRewardItemPayload _payload;

	private VideoButtonPayload _videoButtonPayload;

	private ItemState _currentItemState;

	private Color _buttonNormalColor;

	private Color _buttonPressedColor;

	[SerializeField]
	private RewardForVideoButton _rewardForVideoButton;

	[SerializeField]
	private GameObject _itemButtonColliderGo;

	[SerializeField]
	private GameObject _stickerGo;

	[SerializeField]
	private GameObject _watchAdIconGo;

	[SerializeField]
	private UILabel _getLabel;

	[SerializeField]
	private UILabel _amountLabel;

	[SerializeField]
	private UILabel _stickerLabel;

	[SerializeField]
	private UISprite _itemIconSprite;

	[SerializeField]
	private HorizontalLayout _horizontalLayout;

	[SerializeField]
	private WelcomeBackRewardButtonManager _welcomeBackRewardButtonManager;

	[SerializeField]
	private ItemIconSetup[] _itemIconSetups;

	private void Awake()
	{
		SetDefaultValues();
		CreateDefaultVideoButtonPayload();
		_welcomeBackRewardButtonManager.OnClicked += OnClicked;
	}

	private void OnDestroy()
	{
		_welcomeBackRewardButtonManager.OnClicked -= OnClicked;
	}

	private void SetDefaultValues()
	{
		_currentItemState = ItemState.FreeItem;
		_buttonNormalColor = ColorData.instance.grass;
		_buttonPressedColor = ColorData.instance.avocado;
	}

	private void CreateDefaultVideoButtonPayload()
	{
		_videoButtonPayload = new VideoButtonPayload
		{
			adLocation = AdLocations.WELCOME_BACK_VIDEOS,
			alwaysOn = true,
			unlockItemSource = UnlockItemSource.WelcomeBackVideoReward,
			firstColor = _buttonNormalColor,
			firstColorDark = _buttonPressedColor,
			firstHighlightColor = ColorData.instance.lime,
			secondColor = _buttonNormalColor,
			secondColorDark = _buttonPressedColor,
			secondHighlightColor = ColorData.instance.lime
		};
	}

	public void UpdateItemValues(WelcomeBackRewardItem newItem)
	{
		_payload.UpdateItemValues(newItem);
		_videoButtonPayload.rewardType = _payload.itemType;
		_videoButtonPayload.upgradeType = _payload.upgradeType;
		Setup();
	}

	public void Setup(WelcomeBackRewardItemPayload payload)
	{
		_payload = payload;
		Setup();
	}

	private void Setup()
	{
		_watchAdIconGo.SetActive(_payload.requiresWatchingAdForItem);
		_stickerGo.SetActive(_payload.isOffer);
		SetupButtonColors();
		SetupItemIcon();
		SetupVideoButtonPayload();
		UpdateLabels();
		UpdateButtonState();
		_horizontalLayout.Align();
	}

	private void SetupButtonColors()
	{
		_welcomeBackRewardButtonManager.SetButtonColors(_buttonNormalColor, _buttonPressedColor);
	}

	private void SetupItemIcon()
	{
		ItemIconSetup itemIconSetup = FindIconSetup(_payload.itemType, _payload.upgradeType);
		itemIconSetup.Activate(_itemIconSprite);
	}

	private ItemIconSetup FindIconSetup(CelebrationRewardType itemType, UpgradeType upgradeType)
	{
		ItemIconSetup[] itemIconSetups = _itemIconSetups;
		foreach (ItemIconSetup itemIconSetup in itemIconSetups)
		{
			if (itemIconSetup.itemType == itemType && (itemIconSetup.upgradeType == UpgradeType._notset || itemIconSetup.upgradeType == upgradeType))
			{
				return itemIconSetup;
			}
		}
		LogError(string.Format("Couldn't find an icon setup that matched itemType: {0} and upgradeType: {1}", itemType, upgradeType));
		return null;
	}

	private void SetupVideoButtonPayload()
	{
		_videoButtonPayload.getRewardAmount = () => _payload.multipliedItemAmount;
		_videoButtonPayload.rewardType = _payload.itemType;
		_videoButtonPayload.upgradeType = _payload.upgradeType;
		_videoButtonPayload.onVideoWatched = _payload.OnItemGiven;
		_videoButtonPayload.onButtonClicked = _payload.InvokeOnButtonClicked;
	}

	private void UpdateLabels()
	{
		_getLabel.text = Strings.Get("COIN_BUTTON_EARNER_GET");
		_amountLabel.text = _payload.multipliedItemAmount.ToString();
		_stickerLabel.text = "x" + _payload.offerMultiplier;
	}

	private void UpdateButtonState()
	{
		if (_payload.requiresWatchingAdForItem)
		{
			SetState(ItemState.WatchAdForItem);
		}
		else
		{
			SetState(ItemState.FreeItem);
		}
	}

	private void SetState(ItemState newItemState)
	{
		switch (newItemState)
		{
		case ItemState.FreeItem:
			_rewardForVideoButton.gameObject.SetActive(false);
			_itemButtonColliderGo.SetActive(true);
			break;
		case ItemState.WatchAdForItem:
			_itemButtonColliderGo.SetActive(false);
			ActivateRewardForVideoButton();
			break;
		}
		_currentItemState = newItemState;
	}

	private void ActivateRewardForVideoButton()
	{
		_rewardForVideoButton.gameObject.SetActive(true);
		_rewardForVideoButton.Init(_videoButtonPayload);
	}

	public void OnClicked()
	{
		_payload.InvokeOnButtonClicked();
		GiveItem();
	}

	private void GiveItem()
	{
		_payload.OnItemGiven.SafeInvoke();
		CelebrationReward celebrationReward = new CelebrationReward();
		celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.UnlockItem;
		celebrationReward.amount = _payload.multipliedItemAmount;
		celebrationReward.rewardType = _payload.itemType;
		celebrationReward.upgradeType = _payload.upgradeType;
		celebrationReward.unlockItemSource = UnlockItemSource.WelcomeBackFreeReward;
		celebrationReward.syboAnalyticsSourceMetaOrNull = UnlockItemSource.WelcomeBackFreeReward.ToString();
		RewardManager.AddRewardToUnlock(celebrationReward);
		UIScreenController.Instance.PushCelebrationPopup();
	}

	[Conditional("ENABLE_ITEM_BUTTON_LOGS")]
	public static void Log(object message)
	{
	}

	public static void LogWarning(object message)
	{
	}

	public static void LogError(object message)
	{
		SDebug.LogError("[WelcomeBackRewardManager] " + message);
	}
}
