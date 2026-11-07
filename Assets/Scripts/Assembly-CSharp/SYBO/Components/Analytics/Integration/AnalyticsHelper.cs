using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Data.Managers;
using Data.Save;
using InApp;
using Info;
using SYBO.Components.Logging;
using SYBO.Subway.Info;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class AnalyticsHelper
	{
		private static Dictionary<string, string> AndroidOsVersionByApiVersion = new Dictionary<string, string>
		{
			{ "14", "4.0.x" },
			{ "15", "4.0.x" },
			{ "16", "4.1.x" },
			{ "17", "4.2.x" },
			{ "18", "4.3.x" },
			{ "19", "4.4.x" },
			{ "21", "5.0.x" },
			{ "22", "5.1.x" },
			{ "23", "6.0.x" },
			{ "24", "7.0.x" },
			{ "25", "7.1.x" },
			{ "26", "8.0.x" },
			{ "27", "8.1.x" },
			{ "28", "9.0.x" }
		};

		private static List<KeyValuePair<string, string>> AndroidApiVersionByOsVersion = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("4.0.1", "14"),
			new KeyValuePair<string, string>("4.0.2", "14"),
			new KeyValuePair<string, string>("4.0.3", "15"),
			new KeyValuePair<string, string>("4.0.4", "15"),
			new KeyValuePair<string, string>("4.1", "16"),
			new KeyValuePair<string, string>("4.2", "17"),
			new KeyValuePair<string, string>("4.3", "18"),
			new KeyValuePair<string, string>("4.4.0", "19"),
			new KeyValuePair<string, string>("4.4.1", "19"),
			new KeyValuePair<string, string>("4.4.2", "19"),
			new KeyValuePair<string, string>("4.4.3", "19"),
			new KeyValuePair<string, string>("4.4.4", "19"),
			new KeyValuePair<string, string>("5.0", "21"),
			new KeyValuePair<string, string>("5.1", "22"),
			new KeyValuePair<string, string>("6.0", "23"),
			new KeyValuePair<string, string>("7.0", "24"),
			new KeyValuePair<string, string>("7.1", "25"),
			new KeyValuePair<string, string>("8.0", "26"),
			new KeyValuePair<string, string>("8.1", "27"),
			new KeyValuePair<string, string>("9.0", "28")
		};

		public static float GetLocalCurrencyPriceAsFloat(string localPriceWithCurrency, InAppItemModel.ItemType itemType, string itemTitleId)
		{
			float num = -1f;
			CultureInfo cultureInfo = new CultureInfo("en-US");
			string text = string.Empty;
			foreach (char c in localPriceWithCurrency)
			{
				if (char.IsDigit(c) || char.IsWhiteSpace(c) || c == ',' || c == '.')
				{
					text += c;
				}
				else if (0 < text.Length)
				{
					break;
				}
			}
			text = text.Replace(" ", string.Empty).Trim();
			int num2 = text.Count((char c5) => c5 == '.');
			int num3 = text.Count((char c5) => c5 == ',');
			int num4 = -1;
			string text2 = string.Empty;
			char c2 = '.';
			char c3 = ',';
			for (int num5 = 0; num5 < text.Length; num5++)
			{
				char c4 = text[num5];
				if (char.IsDigit(c4))
				{
					text2 += c4;
				}
				else if (c4 == c2 || c4 == c3)
				{
					if (num4 > 0 && num5 - num4 < 3)
					{
						return num;
					}
					if (text.Length - num5 < 3)
					{
						return num;
					}
					num4 = num5;
					if (c4 == c2 && 1 < num2)
					{
						num2--;
					}
					else if (c4 == c3 && 1 < num3)
					{
						num3--;
					}
					else if (text.Length - num5 == 3 && ContainsDigits(text.Substring(num5 + 1)))
					{
						text2 += c2;
					}
				}
			}
			float result = num;
			if (!float.TryParse(text2, NumberStyles.Float, cultureInfo.NumberFormat, out result))
			{
				Log.Analytics.LogErrorFormat(0, "[SYBO Analytics] - {0:0.00}s : Could not parse the price and currency for the item type of {1} with item id {2} with price string {3}", Time.time, itemType, itemTitleId, text2);
			}
			return result;
		}

		private static bool ContainsDigits(string text)
		{
			return 0 < text.Count(char.IsDigit);
		}

		public static InAppItemModel.ItemType ConvertFacebookRewardToItemType(FacebookRewardsInfo.FacebookRewardType rewardType)
		{
			if (rewardType == FacebookRewardsInfo.FacebookRewardType.Coins)
			{
				return InAppItemModel.ItemType.Coins;
			}
			Log.Analytics.LogErrorFormat("Unhandled reward type {0} received when converting to item type.", rewardType);
			return InAppItemModel.ItemType.Undefined;
		}

		public static bool ShouldTrackRewardPayout(CelebrationReward reward)
		{
			if (reward.CelebrationRewardOrigin == CelebrationRewardOrigin.NewHighScore || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.TopRun)
			{
				return false;
			}
			if (reward.unlockItemSource == UnlockItemSource.FillerSale && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.SuperMysteryBox && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.MysteryBox && reward.CelebrationRewardOrigin != CelebrationRewardOrigin.MysteryBoxMini)
			{
				return false;
			}
			if ((reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterUnlock && Characters.characterData[reward.characterId].unlockType != PurchaseData.UnlockType.Tokens) || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.BoardUnlock || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.BoardBundleUnlock || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.BundleUnlock)
			{
				return false;
			}
			return true;
		}

		public static void ConvertRewardToItemAddedParameters(CelebrationReward reward, out ObtainableItemSource itemSource, out long sourceId, out string sourceMeta, out string itemTitleId, out InAppItemModel.ItemType itemType, out long itemBalanceAfter, out long itemDelta)
		{
			sourceId = reward.syboAnalyticsSourceId;
			sourceMeta = ((!string.IsNullOrEmpty(reward.syboAnalyticsSourceMetaOrNull)) ? reward.syboAnalyticsSourceMetaOrNull : null);
			switch (reward.CelebrationRewardOrigin)
			{
			case CelebrationRewardOrigin.MysteryBox:
			case CelebrationRewardOrigin.SuperMysteryBox:
			case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
			case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
			case CelebrationRewardOrigin.MysteryBoxMini:
			case CelebrationRewardOrigin.CharacterEventSuperMysteryBox:
			case CelebrationRewardOrigin.TokenMysteryBox:
			case CelebrationRewardOrigin.WeeklyChallengeTokenMysteryBox:
				itemSource = ObtainableItemSource.Gachabox;
				sourceId = reward.Uid;
				sourceMeta = null;
				break;
			case CelebrationRewardOrigin.CharacterUnlock:
				itemSource = ObtainableItemSource.CharacterTokens;
				break;
			case CelebrationRewardOrigin.Award:
				itemSource = ObtainableItemSource.Award;
				break;
			case CelebrationRewardOrigin.WeeklyChallengeNormal:
			case CelebrationRewardOrigin.WeeklyChallengeHiddenPrize:
				itemSource = ObtainableItemSource.WeeklyHunt;
				break;
			case CelebrationRewardOrigin.UnlockItem:
				itemSource = ObtainableItemSource.None;
				if (IsVideoReward(reward))
				{
					itemSource = ObtainableItemSource.RewardedVideo;
				}
				if (reward.unlockItemSource == UnlockItemSource.WelcomeBackFreeReward)
				{
					itemSource = ObtainableItemSource.FreeUnlock;
				}
				break;
			case CelebrationRewardOrigin.PromoCode:
				itemSource = ObtainableItemSource.PromotionCode;
				break;
			default:
				itemSource = ObtainableItemSource.None;
				Log.Analytics.LogErrorFormat("Undefined item source from origin {0}", reward.CelebrationRewardOrigin);
				break;
			}
			switch (reward.rewardType)
			{
			case CelebrationRewardType.coins:
			case CelebrationRewardType.doubleCoinsVideo:
				itemType = InAppItemModel.ItemType.Coins;
				itemTitleId = ItemInfo.GetItemTitleId(itemType);
				break;
			case CelebrationRewardType.keys:
				itemType = InAppItemModel.ItemType.Keys;
				itemTitleId = ItemInfo.GetItemTitleId(itemType);
				break;
			case CelebrationRewardType.token:
				itemType = InAppItemModel.ItemType.CharacterToken;
				itemTitleId = string.Format("token_{0}", ItemInfo.GetItemTitleId(reward.characterId));
				break;
			case CelebrationRewardType.powerup:
				itemType = InAppItemModel.GetItemTypeFromPowerup(reward.upgradeType);
				itemTitleId = ItemInfo.GetItemTitleId(itemType);
				break;
			case CelebrationRewardType.trophy:
				itemType = InAppItemModel.ItemType.Trophy;
				itemTitleId = reward.trophyType.ToString();
				break;
			case CelebrationRewardType.character:
				itemType = InAppItemModel.ItemType.Character;
				itemTitleId = ItemInfo.GetItemTitleId(reward.characterId, Outfits.GetOutfitForCharacter(reward.characterId, reward.characterOutfitIndex));
				break;
			case CelebrationRewardType.specialBoard:
			case CelebrationRewardType.welcomePackBoard:
				itemType = InAppItemModel.ItemType.Board;
				itemTitleId = ItemInfo.GetItemTitleId(reward.boardId);
				break;
			default:
				itemType = InAppItemModel.ItemType.Undefined;
				itemTitleId = string.Empty;
				Log.Analytics.LogErrorFormat("Undefined item type & item source for reward type {0}", reward.rewardType);
				break;
			}
			itemTitleId = itemTitleId.ToLowerInvariant();
			itemBalanceAfter = GetItemBalance(itemType, reward.characterId);
			itemDelta = reward.amount;
		}

		public static InAppItemModel.ItemType ItemTypeForIngameShopItemType(IngameShopItems.IngameShopItemType shopItemType)
		{
			switch (shopItemType)
			{
			case IngameShopItems.IngameShopItemType.fillercharactertokens:
				return InAppItemModel.ItemType.CharacterToken;
			case IngameShopItems.IngameShopItemType.fillersupermysterybox:
				return InAppItemModel.ItemType.Gachabox;
			default:
				SDebug.LogErrorFormat("Missing logic for deriving Item Title ID from shop item type '{0}'. Returning empty.", shopItemType);
				return InAppItemModel.ItemType.Undefined;
			}
		}

		private static bool IsVideoReward(CelebrationReward reward)
		{
			return reward.unlockItemSource == UnlockItemSource.VideoAdPrizeEarn || reward.unlockItemSource == UnlockItemSource.VideoAdPrizeHome || reward.unlockItemSource == UnlockItemSource.GameOver || reward.unlockItemSource == UnlockItemSource.FreeCoins || reward.unlockItemSource == UnlockItemSource.NotEnoughCurrencyPopup || reward.unlockItemSource == UnlockItemSource.FreeHoverboards || reward.unlockItemSource == UnlockItemSource.FreeHeadstart || reward.unlockItemSource == UnlockItemSource.WelcomeBackVideoReward || reward.unlockItemSource == UnlockItemSource.FreeScoreBooster;
		}

		public static int GetItemBalanceForIngameShopItem(IngameShopItems.IngameShopItemType shopItemType)
		{
			InAppItemModel.ItemType itemType = ItemTypeForIngameShopItemType(shopItemType);
			switch (shopItemType)
			{
			case IngameShopItems.IngameShopItemType.fillercharactertokens:
				return GetItemBalance(itemType, UserSave.currentStoreFillerDeal.fillerDealTokenType);
			case IngameShopItems.IngameShopItemType.fillersupermysterybox:
				return 0;
			default:
				SDebug.LogErrorFormat("Missing logic for deriving Item Balance from shop item type '{0}'. Returning -1.", shopItemType);
				return -1;
			}
		}

		public static int GetItemBalance(InAppItemModel.ItemType itemType, Characters.CharacterId characterId = Characters.CharacterId.fresh)
		{
			int num = -1;
			switch (itemType)
			{
			case InAppItemModel.ItemType.Character:
			case InAppItemModel.ItemType.CharacterOutfit:
			case InAppItemModel.ItemType.Board:
			case InAppItemModel.ItemType.BoardAbility:
			case InAppItemModel.ItemType.Trophy:
			case InAppItemModel.ItemType.AdRemoval:
				num = 1;
				break;
			case InAppItemModel.ItemType.Coins:
			case InAppItemModel.ItemType.DoubleCoins:
				num = InventorySave.coins;
				break;
			case InAppItemModel.ItemType.Keys:
				num = InventorySave.keys;
				break;
			case InAppItemModel.ItemType.Headstarts:
				num = InventorySave.GetAmount(UpgradeType.headstart2000);
				break;
			case InAppItemModel.ItemType.Hoverboards:
				num = InventorySave.GetAmount(UpgradeType.hoverboard);
				break;
			case InAppItemModel.ItemType.Scoreboosters:
				num = InventorySave.GetAmount(UpgradeType.scorebooster);
				break;
			case InAppItemModel.ItemType.CharacterToken:
				num = CharacterManager.GetCollectedTokens(characterId);
				break;
			default:
				Log.Analytics.LogErrorFormat(0, "[SYBO Analytics] - {0:0.00}s : Undefined item balance for item type '{1}'. Returning '{2}'.", Time.time, itemType, num);
				break;
			}
			return num;
		}

		public static int GetItemPayoutAmount(InAppItemModel item)
		{
			int num = -1;
			switch (item.itemType)
			{
			case InAppItemModel.ItemType.Character:
			case InAppItemModel.ItemType.CharacterOutfit:
			case InAppItemModel.ItemType.Board:
			case InAppItemModel.ItemType.BoardAbility:
			case InAppItemModel.ItemType.DoubleCoins:
			case InAppItemModel.ItemType.AdRemoval:
				num = 1;
				break;
			case InAppItemModel.ItemType.Coins:
			case InAppItemModel.ItemType.Keys:
				num = InAppManager.instance.GetItemPayoutAmount(item.itemId);
				break;
			default:
				Log.Analytics.LogErrorFormat(0, "[SYBO Analytics] - {0:0.00}s : Undefined purchased item count for product ID '{1}'. Returning '{2}'.", Time.time, item.itemId, num);
				break;
			}
			return num;
		}

		public static string GetSyboAnalyticsMissionSetString()
		{
			return string.Format("{0}_{1}", PlayerSave.currentMissionSet + 1, PlayerSave.missionSetCompletedCount);
		}

		public static string GetAndroidDeviceOsString(string deviceOs)
		{
			Regex regex = new Regex("Android OS(?>[a-zA-Z]| |_|\\[|\\])+([0-9]{1,2}\\.[0-9]\\.[0-9]|[0-9]{1,2}\\.[0-9]).*\\/ API-([0-9]{2})");
			Match match = regex.Match(deviceOs);
			if (match.Success)
			{
				string value = match.Groups[1].Value;
				string value2 = match.Groups[2].Value;
				deviceOs = GetAndroidDeviceOsString(value, value2);
			}
			else
			{
				Regex regex2 = new Regex("\\/ API-([0-9]{2})");
				Match match2 = regex2.Match(deviceOs);
				if (match2.Success)
				{
					string value3 = match2.Groups[1].Value;
					string androidOsVersionFromApiVersion = GetAndroidOsVersionFromApiVersion(value3);
					deviceOs = GetAndroidDeviceOsString(androidOsVersionFromApiVersion, value3);
				}
				else
				{
					deviceOs = "Android OS 0.0.0 / API-0";
				}
			}
			return deviceOs;
		}

		private static string GetAndroidDeviceOsString(string osVersion, string apiVersion)
		{
			string arg = ValidateAndroidOsVersionFromApiVersion(osVersion, apiVersion);
			return string.Format("Android OS {0} / API-{1}", arg, apiVersion);
		}

		private static string GetAndroidOsVersionFromApiVersion(string apiVersion)
		{
			string value = null;
			if (!AndroidOsVersionByApiVersion.TryGetValue(apiVersion, out value))
			{
				value = "0.0.0";
			}
			return value;
		}

		private static string ValidateAndroidOsVersionFromApiVersion(string osVersion, string apiVersion)
		{
			bool flag = false;
			for (int i = 0; i < AndroidApiVersionByOsVersion.Count; i++)
			{
				KeyValuePair<string, string> keyValuePair = AndroidApiVersionByOsVersion[i];
				flag = osVersion.StartsWith(keyValuePair.Key) && apiVersion.Equals(keyValuePair.Value);
				if (flag)
				{
					break;
				}
			}
			if (!flag)
			{
				return GetAndroidOsVersionFromApiVersion(apiVersion) + " fx";
			}
			return osVersion;
		}
	}
}
