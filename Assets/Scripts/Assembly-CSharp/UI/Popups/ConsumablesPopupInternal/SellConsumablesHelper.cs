using System;
using System.Collections.Generic;
using System.Diagnostics;
using Data.Save;
using InApp;
using Info;
using Kiloo.Common.Ads;
using Kiloo.Common.Extensions;
using SYBO.Subway;
using UnityEngine;

namespace UI.Popups.ConsumablesPopupInternal
{
	public class SellConsumablesHelper
	{
		private int _purchaseCount;

		private bool _purchaseInProgress;

		private readonly UIScreenController _screenController;

		private static Dictionary<UpgradeType, int> _defaultVideoRewardAmounts = new Dictionary<UpgradeType, int>
		{
			{
				UpgradeType.hoverboard,
				3
			},
			{
				UpgradeType.headstart2000,
				1
			},
			{
				UpgradeType.scorebooster,
				1
			}
		};

		private static Dictionary<UpgradeType, string> _videoRewardAmountOSKeys = new Dictionary<UpgradeType, string>
		{
			{
				UpgradeType.hoverboard,
				"consumable_video_reward_hoverboard"
			},
			{
				UpgradeType.headstart2000,
				"consumable_video_reward_headstart"
			},
			{
				UpgradeType.scorebooster,
				"consumable_video_reward_score_booster"
			}
		};

		public static Action<InAppItemModel.ItemType, int> OnRewardWithVideo;

		public static Dictionary<UpgradeType, int> defaultVideoRewardAmounts
		{
			get
			{
				return _defaultVideoRewardAmounts;
			}
		}

		public static Dictionary<UpgradeType, string> videoRewardAmountOSKeys
		{
			get
			{
				return _videoRewardAmountOSKeys;
			}
		}

		private SellableConsumable activeConsumable
		{
			get
			{
				return SellConsumable.activeConsumable;
			}
		}

		public SellConsumablesHelper()
		{
			_screenController = UIScreenController.Instance;
		}

		public void DidShowConsumablesPopup()
		{
			Game.Instance.TriggerPause(true);
			IngameScreen.instance.HideIfActive();
			_purchaseCount = 0;
		}

		public void DidHideConsumablesPopup()
		{
			_screenController.PushScreen("IngameUI");
			LogPurchases();
			SellConsumable.CompleteSelling(activeConsumable.upgradeType, _purchaseCount);
		}

		public void MakePurchase(bool isInRun, Action OnPurchaseSuccessful = null, Action OnPurchaseFailed = null)
		{
			if (_purchaseInProgress)
			{
				return;
			}
			_purchaseInProgress = true;
			if (isInRun)
			{
				PurchaseHandler.Instance.PurchaseUpgrade(activeConsumable.upgradeType, () =>
				{
					_purchaseCount++;
					_purchaseInProgress = false;
					OnPurchaseSuccessful.SafeInvoke();
				}, () =>
				{
					_purchaseInProgress = false;
					OnPurchaseFailed.SafeInvoke();
				});
			}
			else
			{
				PurchaseHandler.Instance.PurchaseUpgrade(UpgradeType.hoverboard, () =>
				{
					_purchaseInProgress = false;
					OnPurchaseSuccessful.SafeInvoke();
				}, () =>
				{
					_purchaseInProgress = false;
					OnPurchaseFailed.SafeInvoke();
				});
			}
		}

		public bool IsConsumableVideoCached(bool isInRun)
		{
			AdLocationInfo adLocationInfo = GetAdLocationInfo(isInRun);
			return adLocationInfo != null && AdSystemManager.IsCached(adLocationInfo);
		}

		public bool IsConsumableVideoLocationAvailable(bool isInRun)
		{
			string videoIdForConsumable = GetVideoIdForConsumable(isInRun);
			if (!videoIdForConsumable.IsNullOrEmpty())
			{
				return AdSystemManager.GetAvailabilityForLocation(videoIdForConsumable);
			}
			return false;
		}

		private string GetVideoIdForConsumable(bool isInRun)
		{
			string result = string.Empty;
			AdLocationInfo adLocationInfo = GetAdLocationInfo(isInRun);
			if (adLocationInfo != null)
			{
				result = adLocationInfo.id;
			}
			return result;
		}

		public AdLocationInfo GetAdLocationInfo(bool isInRun)
		{
			AdLocationInfo result = null;
			UpgradeType upgradeType = UpgradeType.hoverboard;
			if (isInRun)
			{
				upgradeType = activeConsumable.upgradeType;
			}
			switch (upgradeType)
			{
			case UpgradeType.hoverboard:
				result = ((!isInRun) ? AdLocations.HOVERBOARD_POPUP_VIDEOS : AdLocations.HOVERBOARD_RUN_VIDEOS);
				break;
			case UpgradeType.headstart2000:
				result = AdLocations.HEADSTART_RUN_VIDEOS;
				break;
			case UpgradeType.scorebooster:
				result = AdLocations.SCOREBOOSTER_RUN_VIDEOS;
				break;
			}
			return result;
		}

		public int GetConsumableRewardForVideo(bool isInRun)
		{
			SellableConsumable consumableReward = GetConsumableReward(isInRun);
			int num = 0;
			if (consumableReward != null && _defaultVideoRewardAmounts.ContainsKey(consumableReward.upgradeType))
			{
				num = _defaultVideoRewardAmounts[consumableReward.upgradeType];
				if (videoRewardAmountOSKeys.ContainsKey(consumableReward.upgradeType))
				{
					num = OnlineSettings.instance.GetValue(videoRewardAmountOSKeys[consumableReward.upgradeType], num);
				}
			}
			return num;
		}

		public SellableConsumable GetConsumableReward(bool isInRun)
		{
			if (!isInRun)
			{
				return SellableConsumable.hoverboard;
			}
			return activeConsumable;
		}

		public UpgradeType GetConsumableUpgradeType(bool isInRun)
		{
			SellableConsumable consumableReward = GetConsumableReward(isInRun);
			if (consumableReward != null)
			{
				return consumableReward.upgradeType;
			}
			return UpgradeType._notset;
		}

		public void ShowVideoForConsumable(bool isInRun, Action onVideoFinished)
		{
			UpgradeType upgradeType = UpgradeType.hoverboard;
			if (isInRun)
			{
				upgradeType = activeConsumable.upgradeType;
			}
			AdSystemManager.Show(GetAdLocationInfo(isInRun), () =>
			{
				_purchaseCount++;
				int consumableRewardForVideo = GetConsumableRewardForVideo(isInRun);
				InventorySave.IncreaseAmount(upgradeType, consumableRewardForVideo);
				onVideoFinished();
				if (OnRewardWithVideo != null)
				{
					InAppItemModel.ItemType itemTypeFromPowerup = InAppItemModel.GetItemTypeFromPowerup(upgradeType);
					OnRewardWithVideo(itemTypeFromPowerup, consumableRewardForVideo);
				}
			});
		}

		private void LogPurchases()
		{
			string argKeys = string.Concat(activeConsumable.upgradeType, ";Revive");
			string argValues = _purchaseCount + ";" + IngameScreen.instance.reviveCount;
			Flurry.LogEventWithSeveralParameters("Consumables Sold During Run", argKeys, argValues);
		}

		[Conditional("SELL_CONSUMABLES_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}

		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("[SellConsumablesHelper] " + msg, context);
		}
	}
}
