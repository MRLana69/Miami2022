using System;
using System.Reflection;
using Info;
using UI.Popups.WelcomeBackRewardPopupInternal;
using UI.Screens.RewardManagement;

public class WelcomeBackRewardItemPayload
{
	public int itemAmount { get; set; }

	public CelebrationRewardType itemType { get; set; }

	public UpgradeType upgradeType { get; set; }

	public bool requiresWatchingAdForItem { get; set; }

	public bool isOffer { get; set; }

	public int offerMultiplier { get; set; }

	public Action OnItemGiven { get; set; }

	public OnItemButtonClicked OnButtonClicked { get; set; }

	public int multipliedItemAmount
	{
		get
		{
			return itemAmount * offerMultiplier;
		}
	}

	public WelcomeBackRewardItemPayload()
	{
	}

	public WelcomeBackRewardItemPayload(WelcomeBackRewardItem item)
	{
		UpdateItemValues(item);
	}

	public void UpdateItemValues(WelcomeBackRewardItem item)
	{
		itemAmount = item.amount;
		itemType = item.itemType;
		upgradeType = item.upgradeType;
	}

	public void InvokeOnButtonClicked()
	{
		if (OnButtonClicked != null)
		{
			OnButtonClicked(this);
		}
	}

	public string DebugToString()
	{
		return string.Format("{0} {1} {2} {3} {4} {5} {6} {7}", PropertyToString("itemAmount"), PropertyToString("itemType"), PropertyToString("upgradeType"), PropertyToString("requiresWatchingAdForItem"), PropertyToString("isOffer"), PropertyToString("offerMultiplier"), PropertyToString("unlockItemSource"), PropertyToString("multipliedItemAmount"));
	}

	private string PropertyToString(string propertyName)
	{
		PropertyInfo property = GetType().GetProperty(propertyName);
		return string.Format("{0}: {1}", property.Name, property.GetValue(this, null));
	}
}
