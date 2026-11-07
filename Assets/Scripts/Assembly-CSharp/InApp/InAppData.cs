using System;
using System.Collections.Generic;
using System.Text;
using Info;
using Kiloo.Common.Plugins.DeviceUtility;
using SYBO.Subway.Info;
using UnityEngine;

namespace InApp
{
	public static class InAppData
	{
		public enum BundleTier
		{
			None = 0,
			BundleTier1 = 1,
			BundleTier2 = 2,
			BundleTier3 = 3,
			BundleTier4 = 4,
			BundleTier5 = 5
		}

		private const string IN_APP_PREFIX_DEFAULT = "com.kiloo.subwaysurfers.";

		private const string IN_APP_CHARACTER_PREFIX = "com.kiloo.subwaycharacter.";

		public const string IN_APP_BOARD_PREFIX = "com.kiloo.subwayboard.";

		private const string IN_APP_PREFIX_IOS = "com.kiloo.subwaysurfers.";

		private const string IN_APP_PREFIX_ANDROID = "com.kiloo.subways.";

		public const string IN_APP_PREFIX = "com.kiloo.subways.";

		private const bool IS_PURE_ANDROID = true;

		private const bool IS_IOS = false;

		private static readonly string[] IN_APP_PREFIXES;

		public const string IN_APP_TIER1 = "com.kiloo.subways.coinstier1";

		public const string IN_APP_TIER2 = "com.kiloo.subways.coinstier2";

		public const string IN_APP_TIER2_5 = "com.kiloo.subways.coinstier2.5";

		public const string IN_APP_TIER3 = "com.kiloo.subways.coinstier3";

		public const string IN_APP_TIER4 = "com.kiloo.subways.coinstier4";

		public const string IN_APP_TIER5 = "com.kiloo.subways.coinstier5";

		public const string IN_APP_TIER6 = "com.kiloo.subways.keystier1";

		public const string IN_APP_TIER6_5 = "com.kiloo.subways.keystier1.5";

		public const string IN_APP_TIER7 = "com.kiloo.subways.keystier2";

		public const string IN_APP_TIER8 = "com.kiloo.subways.keystier3";

		public const string IN_APP_TIER9 = "com.kiloo.subways.keystier4";

		public const string IN_APP_TIER1_DISCOUNT = "com.kiloo.subways.coinstier1discount";

		public const string IN_APP_TIER2_DISCOUNT = "com.kiloo.subways.coinstier2_discount";

		public const string IN_APP_TIER2_5_DISCOUNT = "com.kiloo.subways.coinstier2.5discount";

		public const string IN_APP_TIER3_DISCOUNT = "com.kiloo.subways.coinstier3discount";

		public const string IN_APP_TIER4_DISCOUNT = "com.kiloo.subways.coinstier4discount";

		public const string IN_APP_TIER5_DISCOUNT = "com.kiloo.subways.coinstier5discount";

		public const string IN_APP_TIER6_DISCOUNT = "com.kiloo.subways.keystier1discount";

		public const string IN_APP_TIER6_5_DISCOUNT = "com.kiloo.subways.keystier1.5discount";

		public const string IN_APP_TIER7_DISCOUNT = "com.kiloo.subways.keystier2discount";

		public const string IN_APP_TIER8_DISCOUNT = "com.kiloo.subways.keystier3discount";

		public const string IN_APP_TIER9_DISCOUNT = "com.kiloo.subways.keystier4discount";

		public const string IN_APP_TIER1_SPECIAL_DISCOUNT = "com.kiloo.subwaysurfers.coinstier1specialdiscount";

		public const string IN_APP_TIER2_SPECIAL_DISCOUNT = "com.kiloo.subwaysurfers.coinstier2specialdiscount";

		public const string IN_APP_TIER2_5_SPECIAL_DISCOUNT = "com.kiloo.subwaysurfers.coinstier2.5specialdiscount";

		public const string IN_APP_TIER3_SPECIAL_DISCOUNT = "com.kiloo.subwaysurfers.coinstier3specialdiscount";

		public const string IN_APP_TIER4_SPECIAL_DISCOUNT = "com.kiloo.subwaysurfers.coinstier4specialdiscount";

		public const string IN_APP_TIER5_SPECIAL_DISCOUNT = "com.kiloo.subwaysurfers.coinstier5specialdiscount";

		public const string IN_APP_TIER6_SPECIAL_DISCOUNT = "com.kiloo.subwaysurfers.keystier1specialdiscount";

		public const string IN_APP_TIER6_5_SPECIAL_DISCOUNT = "com.kiloo.subwaysurfers.keystier1.5specialdiscount";

		public const string IN_APP_TIER7_SPECIAL_DISCOUNT = "com.kiloo.subwaysurfers.keystier2specialdiscount";

		public const string IN_APP_TIER8_SPECIAL_DISCOUNT = "com.kiloo.subwaysurfers.keystier3specialdiscount";

		public const string IN_APP_TIER9_SPECIAL_DISCOUNT = "com.kiloo.subwaysurfers.keystier4specialdiscount";

		public const string IN_APP_DOUBLE_COINS = "com.kiloo.subways.doublecoins";

		public const string IN_APP_DOUBLE_COINS_DISCOUNT = "com.kiloo.subways.doublecoinsdiscount";

		public const string IN_APP_BUNDLE_TIER1 = "com.kiloo.subways.bundletier1";

		public const string IN_APP_BUNDLE_TIER2 = "com.kiloo.subways.bundletier2";

		public const string IN_APP_BUNDLE_TIER3 = "com.kiloo.subways.bundletier3";

		public const string IN_APP_BUNDLE_TIER4 = "com.kiloo.subways.bundletier4";

		public const string IN_APP_BUNDLE_TIER5 = "com.kiloo.subways.bundletier5";

		public const string IN_APP_AD_REMOVAL_TIER1 = "com.kiloo.subways.adremovaltier1";

		public const string IN_APP_AD_REMOVAL_TIER2 = "com.kiloo.subways.adremovaltier2";

		public const string IN_APP_AD_REMOVAL_TIER3 = "com.kiloo.subways.adremovaltier3";

		public const string IN_APP_AD_REMOVAL_TIER4 = "com.kiloo.subways.adremovaltier4";

		public const string IN_APP_CAIRO_BOARD_BUNDLE_TIER1 = "com.kiloo.subwayboard.cairoboard02.bundletier1";

		public const string IN_APP_CAIRO_BOARD_BUNDLE_TIER2 = "com.kiloo.subwayboard.cairoboard02.bundletier2";

		public const string IN_APP_MUMBAI_BOARD_BUNDLE_TIER1 = "com.kiloo.subwayboard.mumbai.bundletier1";

		public const string IN_APP_MUMBAI_BOARD_BUNDLE_TIER2 = "com.kiloo.subwayboard.mumbai.bundletier2";

		public const string IN_APP_CHICK_BOARD_BUNDLE_TIER1 = "com.kiloo.subwayboard.chick.bundletier1";

		public const string IN_APP_CHICK_BOARD_BUNDLE_TIER2 = "com.kiloo.subwayboard.chick.bundletier2";

		public const string IN_APP_WELCOME_PACK_TIER1 = "com.kiloo.subwaysurfers.welcomepacktier01";

		public const string IN_APP_WELCOME_PACK_TIER2 = "com.kiloo.subwaysurfers.welcomepacktier02";

		public const string IN_APP_NEW_YEAR_BOARD_BUNDLE_TIER1 = "com.kiloo.subwayboard.rioboard03.bundletier1";

		public const string IN_APP_NEW_YEAR_BOARD_BUNDLE_TIER2 = "com.kiloo.subwayboard.rioboard03.bundletier2";

		public const string IN_APP_FOURTH_OF_JULY_BOARD_BUNDLE_TIER1 = "com.kiloo.subwayboard.julyboard.bundletier1";

		public const string IN_APP_FOURTH_OF_JULY_BOARD_BUNDLE_TIER2 = "com.kiloo.subwayboard.julyboard.bundletier2";

		public const string IN_APP_DUBAI_TURBO_TEAM_BUNDLE = "com.kiloo.subways.dubaiturboteam.bundle";

		private static readonly string[] IN_APP_CURRENCY_TIERS;

		private static readonly string[] IN_APP_CURRENCY_DISCOUNT_TIERS;

		private static readonly string[] IN_APP_CURRENCY_SPECIAL_DISCOUNT_TIERS;

		private static readonly string[] IN_APP_AD_REMOVAL_TIERS;

		private static readonly string[] IN_APP_LIMITED_BUNDLE_TIERS;

		private static readonly string[] IN_APP_BUNDLE_TIERS;

		private static readonly string[] IN_APP_BOARD_BUNDLE_TIERS;

		private static readonly string[] inAppSpecialBundles;

		private static readonly Dictionary<string, string> IN_APP_TIERS_ONLINE_SETTINGS_IDS;

		private static readonly Dictionary<string, string> IGNORED_IN_APP_TIERS_ONLINE_SETTINGS_IDS;

		public static readonly List<string> LIST_OF_MANAGED_INAPP_IDS;

		public static Dictionary<string, InAppItemModel> inAppData;

		public static int inAppPurchaseCount
		{
			get
			{
				return inAppData.Count;
			}
		}

		public static string commaSeparatedProductIds
		{
			get
			{
				string text = string.Empty;
				int num = 0;
				foreach (KeyValuePair<string, InAppItemModel> inAppDatum in inAppData)
				{
					if (num > 0)
					{
						text += ",";
					}
					text += inAppDatum.Key;
					num++;
				}
				return text;
			}
		}

		static InAppData()
		{
			IN_APP_PREFIXES = new string[4] { "com.kiloo.subwaysurfers.", "com.kiloo.subways.", "com.kiloo.subwaycharacter.", "com.kiloo.subwayboard." };
			IN_APP_CURRENCY_TIERS = new string[11]
			{
				"com.kiloo.subways.coinstier1", "com.kiloo.subways.coinstier2", "com.kiloo.subways.coinstier2.5", "com.kiloo.subways.coinstier3", "com.kiloo.subways.coinstier4", "com.kiloo.subways.coinstier5", "com.kiloo.subways.keystier1", "com.kiloo.subways.keystier1.5", "com.kiloo.subways.keystier2", "com.kiloo.subways.keystier3",
				"com.kiloo.subways.keystier4"
			};
			IN_APP_CURRENCY_DISCOUNT_TIERS = new string[11]
			{
				"com.kiloo.subways.coinstier1discount", "com.kiloo.subways.coinstier2_discount", "com.kiloo.subways.coinstier2.5discount", "com.kiloo.subways.coinstier3discount", "com.kiloo.subways.coinstier4discount", "com.kiloo.subways.coinstier5discount", "com.kiloo.subways.keystier1discount", "com.kiloo.subways.keystier1.5discount", "com.kiloo.subways.keystier2discount", "com.kiloo.subways.keystier3discount",
				"com.kiloo.subways.keystier4discount"
			};
			IN_APP_CURRENCY_SPECIAL_DISCOUNT_TIERS = new string[11]
			{
				"com.kiloo.subwaysurfers.coinstier1specialdiscount", "com.kiloo.subwaysurfers.coinstier2specialdiscount", "com.kiloo.subwaysurfers.coinstier2.5specialdiscount", "com.kiloo.subwaysurfers.coinstier3specialdiscount", "com.kiloo.subwaysurfers.coinstier4specialdiscount", "com.kiloo.subwaysurfers.coinstier5specialdiscount", "com.kiloo.subwaysurfers.keystier1specialdiscount", "com.kiloo.subwaysurfers.keystier1.5specialdiscount", "com.kiloo.subwaysurfers.keystier2specialdiscount", "com.kiloo.subwaysurfers.keystier3specialdiscount",
				"com.kiloo.subwaysurfers.keystier4specialdiscount"
			};
			IN_APP_AD_REMOVAL_TIERS = new string[4] { "com.kiloo.subways.adremovaltier1", "com.kiloo.subways.adremovaltier2", "com.kiloo.subways.adremovaltier3", "com.kiloo.subways.adremovaltier4" };
			IN_APP_LIMITED_BUNDLE_TIERS = new string[1] { "com.kiloo.subways.dubaiturboteam.bundle" };
			IN_APP_BUNDLE_TIERS = new string[5] { "com.kiloo.subways.bundletier1", "com.kiloo.subways.bundletier2", "com.kiloo.subways.bundletier3", "com.kiloo.subways.bundletier4", "com.kiloo.subways.bundletier5" };
			IN_APP_BOARD_BUNDLE_TIERS = new string[10] { "com.kiloo.subwayboard.cairoboard02.bundletier1", "com.kiloo.subwayboard.cairoboard02.bundletier2", "com.kiloo.subwayboard.chick.bundletier1", "com.kiloo.subwayboard.chick.bundletier2", "com.kiloo.subwayboard.mumbai.bundletier1", "com.kiloo.subwayboard.mumbai.bundletier2", "com.kiloo.subwayboard.rioboard03.bundletier1", "com.kiloo.subwayboard.rioboard03.bundletier2", "com.kiloo.subwayboard.julyboard.bundletier1", "com.kiloo.subwayboard.julyboard.bundletier2" };
			inAppSpecialBundles = new string[2] { "com.kiloo.subwaysurfers.welcomepacktier01", "com.kiloo.subwaysurfers.welcomepacktier02" };
			IN_APP_TIERS_ONLINE_SETTINGS_IDS = new Dictionary<string, string>
			{
				{ "com.kiloo.subways.coinstier1", "in_app_tier_1" },
				{ "com.kiloo.subways.coinstier2", "in_app_tier_2" },
				{ "com.kiloo.subways.coinstier2.5", "in_app_tier_2_5" },
				{ "com.kiloo.subways.coinstier3", "in_app_tier_3" },
				{ "com.kiloo.subways.coinstier4", "in_app_tier_4" },
				{ "com.kiloo.subways.coinstier5", "in_app_tier_5" },
				{ "com.kiloo.subways.keystier1", "in_app_tier_6" },
				{ "com.kiloo.subways.keystier1.5", "in_app_tier_6_5" },
				{ "com.kiloo.subways.keystier2", "in_app_tier_7" },
				{ "com.kiloo.subways.keystier3", "in_app_tier_8" },
				{ "com.kiloo.subways.keystier4", "in_app_tier_9" },
				{ "com.kiloo.subways.doublecoins", "in_app_double_coins" }
			};
			IGNORED_IN_APP_TIERS_ONLINE_SETTINGS_IDS = new Dictionary<string, string>
			{
				{ "com.kiloo.subways.coinstier1", "ignore_in_app_tier_1" },
				{ "com.kiloo.subways.coinstier2", "ignore_in_app_tier_2" },
				{ "com.kiloo.subways.coinstier2.5", "ignore_in_app_tier_2_5" },
				{ "com.kiloo.subways.coinstier3", "ignore_in_app_tier_3" },
				{ "com.kiloo.subways.coinstier4", "ignore_in_app_tier_4" },
				{ "com.kiloo.subways.coinstier5", "ignore_in_app_tier_5" },
				{ "com.kiloo.subways.keystier1", "ignore_in_app_tier_6" },
				{ "com.kiloo.subways.keystier1.5", "ignore_in_app_tier_6_5" },
				{ "com.kiloo.subways.keystier2", "ignore_in_app_tier_7" },
				{ "com.kiloo.subways.keystier3", "ignore_in_app_tier_8" },
				{ "com.kiloo.subways.keystier4", "ignore_in_app_tier_9" }
			};
			LIST_OF_MANAGED_INAPP_IDS = new List<string> { "com.kiloo.subways.doublecoins", "com.kiloo.subways.doublecoinsdiscount", "com.kiloo.subways.adremovaltier1", "com.kiloo.subways.adremovaltier2", "com.kiloo.subways.adremovaltier3", "com.kiloo.subways.adremovaltier4" };
			inAppData = new Dictionary<string, InAppItemModel>
			{
				{
					"com.kiloo.subways.coinstier1",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.coinstier1",
						payoutAmount = 7500,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Coins,
						itemOfferType = InAppItemModel.OfferType.Normal
					}
				},
				{
					"com.kiloo.subways.coinstier2",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.coinstier2",
						payoutAmount = 45000,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Coins,
						itemOfferType = InAppItemModel.OfferType.Normal
					}
				},
				{
					"com.kiloo.subways.coinstier2.5",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.coinstier2.5",
						payoutAmount = 90000,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Coins,
						itemOfferType = InAppItemModel.OfferType.Normal
					}
				},
				{
					"com.kiloo.subways.coinstier3",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.coinstier3",
						payoutAmount = 180000,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Coins,
						itemOfferType = InAppItemModel.OfferType.Normal
					}
				},
				{
					"com.kiloo.subways.coinstier4",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.coinstier4",
						payoutAmount = 500000,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Coins,
						itemOfferType = InAppItemModel.OfferType.Normal
					}
				},
				{
					"com.kiloo.subways.coinstier5",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.coinstier5",
						payoutAmount = 1200000,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Coins,
						itemOfferType = InAppItemModel.OfferType.Normal
					}
				},
				{
					"com.kiloo.subways.keystier1",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.keystier1",
						payoutAmount = 25,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Keys,
						itemOfferType = InAppItemModel.OfferType.Normal
					}
				},
				{
					"com.kiloo.subways.keystier1.5",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.keystier1.5",
						payoutAmount = 50,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Keys,
						itemOfferType = InAppItemModel.OfferType.Normal
					}
				},
				{
					"com.kiloo.subways.keystier2",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.keystier2",
						payoutAmount = 110,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Keys,
						itemOfferType = InAppItemModel.OfferType.Normal
					}
				},
				{
					"com.kiloo.subways.keystier3",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.keystier3",
						payoutAmount = 300,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Keys,
						itemOfferType = InAppItemModel.OfferType.Normal
					}
				},
				{
					"com.kiloo.subways.keystier4",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.keystier4",
						payoutAmount = 650,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Keys,
						itemOfferType = InAppItemModel.OfferType.Normal
					}
				},
				{
					"com.kiloo.subways.coinstier1discount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.coinstier1discount",
						payoutAmount = 7500,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Coins,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.coinstier1"
					}
				},
				{
					"com.kiloo.subways.coinstier2_discount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.coinstier2_discount",
						payoutAmount = 45000,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Coins,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.coinstier2"
					}
				},
				{
					"com.kiloo.subways.coinstier2.5discount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.coinstier2.5discount",
						payoutAmount = 90000,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Coins,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.coinstier2.5"
					}
				},
				{
					"com.kiloo.subways.coinstier3discount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.coinstier3discount",
						payoutAmount = 180000,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Coins,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.coinstier3"
					}
				},
				{
					"com.kiloo.subways.coinstier4discount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.coinstier4discount",
						payoutAmount = 500000,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Coins,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.coinstier4"
					}
				},
				{
					"com.kiloo.subways.coinstier5discount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.coinstier5discount",
						payoutAmount = 1200000,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Coins,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.coinstier5"
					}
				},
				{
					"com.kiloo.subways.keystier1discount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.keystier1discount",
						payoutAmount = 25,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Keys,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.keystier1"
					}
				},
				{
					"com.kiloo.subways.keystier1.5discount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.keystier1.5discount",
						payoutAmount = 50,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Keys,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.keystier1.5"
					}
				},
				{
					"com.kiloo.subways.keystier2discount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.keystier2discount",
						payoutAmount = 110,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Keys,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.keystier2"
					}
				},
				{
					"com.kiloo.subways.keystier3discount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.keystier3discount",
						payoutAmount = 300,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Keys,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.keystier3"
					}
				},
				{
					"com.kiloo.subways.keystier4discount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.keystier4discount",
						payoutAmount = 650,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Keys,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.keystier4"
					}
				},
				{
					"com.kiloo.subwaysurfers.coinstier1specialdiscount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwaysurfers.coinstier1specialdiscount",
						payoutAmount = 7500,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Coins,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.coinstier1"
					}
				},
				{
					"com.kiloo.subwaysurfers.coinstier2specialdiscount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwaysurfers.coinstier2specialdiscount",
						payoutAmount = 45000,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Coins,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.coinstier2"
					}
				},
				{
					"com.kiloo.subwaysurfers.coinstier2.5specialdiscount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwaysurfers.coinstier2.5specialdiscount",
						payoutAmount = 90000,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Coins,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.coinstier2.5"
					}
				},
				{
					"com.kiloo.subwaysurfers.coinstier3specialdiscount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwaysurfers.coinstier3specialdiscount",
						payoutAmount = 180000,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Coins,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.coinstier3"
					}
				},
				{
					"com.kiloo.subwaysurfers.coinstier4specialdiscount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwaysurfers.coinstier4specialdiscount",
						payoutAmount = 500000,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Coins,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.coinstier4"
					}
				},
				{
					"com.kiloo.subwaysurfers.coinstier5specialdiscount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwaysurfers.coinstier5specialdiscount",
						payoutAmount = 1200000,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Coins,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.coinstier5"
					}
				},
				{
					"com.kiloo.subwaysurfers.keystier1specialdiscount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwaysurfers.keystier1specialdiscount",
						payoutAmount = 25,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Keys,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.keystier1"
					}
				},
				{
					"com.kiloo.subwaysurfers.keystier1.5specialdiscount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwaysurfers.keystier1.5specialdiscount",
						payoutAmount = 50,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Keys,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.keystier1.5"
					}
				},
				{
					"com.kiloo.subwaysurfers.keystier2specialdiscount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwaysurfers.keystier2specialdiscount",
						payoutAmount = 110,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Keys,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.keystier2"
					}
				},
				{
					"com.kiloo.subwaysurfers.keystier3specialdiscount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwaysurfers.keystier3specialdiscount",
						payoutAmount = 300,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Keys,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.keystier3"
					}
				},
				{
					"com.kiloo.subwaysurfers.keystier4specialdiscount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwaysurfers.keystier4specialdiscount",
						payoutAmount = 650,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Keys,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.keystier4"
					}
				},
				{
					"com.kiloo.subways.doublecoins",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.doublecoins",
						payoutAmount = 1,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.DoubleCoins,
						itemOfferType = InAppItemModel.OfferType.Normal
					}
				},
				{
					"com.kiloo.subways.doublecoinsdiscount",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.doublecoinsdiscount",
						payoutAmount = 1,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.DoubleCoins,
						itemOfferType = InAppItemModel.OfferType.Sale,
						normalOfferId = "com.kiloo.subways.doublecoins"
					}
				},
				{
					"com.kiloo.subways.bundletier1",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.bundletier1",
						displayName = "INAPP_DATA_BUNDLE_TIER_ONE",
						payoutAmount = 1,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Bundle,
						itemOfferType = InAppItemModel.OfferType.Normal,
						bundleContents = new List<InAppItemModel.BundleItem>
						{
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Hoverboards, 20),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Coins, 4000),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Keys, 5)
						}
					}
				},
				{
					"com.kiloo.subways.bundletier2",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.bundletier2",
						displayName = "INAPP_DATA_BUNDLE_TIER_TWO",
						payoutAmount = 1,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Bundle,
						itemOfferType = InAppItemModel.OfferType.Normal,
						bundleContents = new List<InAppItemModel.BundleItem>
						{
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Hoverboards, 50),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Keys, 10),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Coins, 30000),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Headstarts, 10)
						}
					}
				},
				{
					"com.kiloo.subways.bundletier3",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.bundletier3",
						displayName = "INAPP_DATA_BUNDLE_TIER_THREE",
						payoutAmount = 1,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Bundle,
						itemOfferType = InAppItemModel.OfferType.Normal,
						bundleContents = new List<InAppItemModel.BundleItem>
						{
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Hoverboards, 100),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Keys, 50),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Coins, 100000),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Headstarts, 20)
						}
					}
				},
				{
					"com.kiloo.subways.bundletier4",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.bundletier4",
						displayName = "INAPP_DATA_BUNDLE_TIER_FOUR",
						payoutAmount = 1,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Bundle,
						itemOfferType = InAppItemModel.OfferType.Normal,
						bundleContents = new List<InAppItemModel.BundleItem>
						{
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Coins, 300000),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Hoverboards, 200),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Headstarts, 30),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Scoreboosters, 30),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Keys, 150)
						}
					}
				},
				{
					"com.kiloo.subways.bundletier5",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.bundletier5",
						displayName = "INAPP_DATA_BUNDLE_TIER_FIVE",
						payoutAmount = 1,
						isConsumable = true,
						itemType = InAppItemModel.ItemType.Bundle,
						itemOfferType = InAppItemModel.OfferType.Normal,
						bundleContents = new List<InAppItemModel.BundleItem>
						{
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Coins, 600000),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Hoverboards, 300),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Headstarts, 50),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Scoreboosters, 50),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Keys, 300)
						}
					}
				},
				{
					"com.kiloo.subways.adremovaltier1",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.adremovaltier1",
						payoutAmount = 15,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.AdRemoval,
						itemOfferType = InAppItemModel.OfferType.Normal
					}
				},
				{
					"com.kiloo.subways.adremovaltier2",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.adremovaltier2",
						payoutAmount = 20,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.AdRemoval,
						itemOfferType = InAppItemModel.OfferType.Normal
					}
				},
				{
					"com.kiloo.subways.adremovaltier3",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.adremovaltier3",
						payoutAmount = 25,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.AdRemoval,
						itemOfferType = InAppItemModel.OfferType.Normal
					}
				},
				{
					"com.kiloo.subways.adremovaltier4",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.adremovaltier4",
						payoutAmount = 50,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.AdRemoval,
						itemOfferType = InAppItemModel.OfferType.Normal
					}
				},
				{
					"com.kiloo.subwayboard.cairoboard02.bundletier1",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwayboard.cairoboard02.bundletier1",
						payoutAmount = 1,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.Bundle,
						itemOfferType = InAppItemModel.OfferType.Normal,
						bundleContents = new List<InAppItemModel.BundleItem>
						{
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Board, Boards.BoardId.cairoBoard02),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.BoardAbility, Boards.BoardId.cairoBoard02, HoverboardThemes.ThemeIdentifier.cairoBoard02Theme02)
						}
					}
				},
				{
					"com.kiloo.subwayboard.cairoboard02.bundletier2",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwayboard.cairoboard02.bundletier2",
						payoutAmount = 1,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.Bundle,
						itemOfferType = InAppItemModel.OfferType.Normal,
						bundleContents = new List<InAppItemModel.BundleItem>
						{
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Board, Boards.BoardId.cairoBoard02),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.BoardAbility, Boards.BoardId.cairoBoard02, HoverboardThemes.ThemeIdentifier.cairoBoard02Theme02),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.BoardAbility, Boards.BoardId.cairoBoard02, HoverboardThemes.ThemeIdentifier.cairoBoard02Theme01)
						}
					}
				},
				{
					"com.kiloo.subwayboard.chick.bundletier1",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwayboard.chick.bundletier1",
						payoutAmount = 1,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.Bundle,
						itemOfferType = InAppItemModel.OfferType.Normal,
						bundleContents = new List<InAppItemModel.BundleItem>
						{
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Board, Boards.BoardId.chick),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.BoardAbility, Boards.BoardId.chick, HoverboardThemes.ThemeIdentifier.chicky01Theme02)
						}
					}
				},
				{
					"com.kiloo.subwayboard.chick.bundletier2",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwayboard.chick.bundletier2",
						payoutAmount = 1,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.Bundle,
						itemOfferType = InAppItemModel.OfferType.Normal,
						bundleContents = new List<InAppItemModel.BundleItem>
						{
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Board, Boards.BoardId.chick),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.BoardAbility, Boards.BoardId.chick, HoverboardThemes.ThemeIdentifier.chicky01Theme02),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.BoardAbility, Boards.BoardId.chick, HoverboardThemes.ThemeIdentifier.chicky01Theme01)
						}
					}
				},
				{
					"com.kiloo.subwaysurfers.welcomepacktier01",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwaysurfers.welcomepacktier01",
						payoutAmount = 1,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.Bundle,
						itemOfferType = InAppItemModel.OfferType.Normal,
						bundleContents = new List<InAppItemModel.BundleItem>
						{
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Board, Boards.BoardId.starterPackBoard01),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Coins, 5000),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Keys, 2),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Hoverboards, 10)
						}
					}
				},
				{
					"com.kiloo.subwaysurfers.welcomepacktier02",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwaysurfers.welcomepacktier02",
						payoutAmount = 1,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.Bundle,
						itemOfferType = InAppItemModel.OfferType.Normal,
						bundleContents = new List<InAppItemModel.BundleItem>
						{
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Board, Boards.BoardId.starterPackBoard01),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Coins, 45000),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Keys, 25),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Hoverboards, 60)
						}
					}
				},
				{
					"com.kiloo.subwayboard.mumbai.bundletier1",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwayboard.mumbai.bundletier1",
						payoutAmount = 1,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.Bundle,
						itemOfferType = InAppItemModel.OfferType.Normal,
						bundleContents = new List<InAppItemModel.BundleItem>
						{
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Board, Boards.BoardId.mumbaiBoard02),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.BoardAbility, Boards.BoardId.mumbaiBoard02, HoverboardThemes.ThemeIdentifier.mumbaiBoard02Theme01)
						}
					}
				},
				{
					"com.kiloo.subwayboard.mumbai.bundletier2",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwayboard.mumbai.bundletier2",
						payoutAmount = 1,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.Bundle,
						itemOfferType = InAppItemModel.OfferType.Normal,
						bundleContents = new List<InAppItemModel.BundleItem>
						{
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Board, Boards.BoardId.mumbaiBoard02),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.BoardAbility, Boards.BoardId.mumbaiBoard02, HoverboardThemes.ThemeIdentifier.mumbaiBoard02Theme01),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.BoardAbility, Boards.BoardId.mumbaiBoard02, HoverboardThemes.ThemeIdentifier.mumbaiBoard02Theme02)
						}
					}
				},
				{
					"com.kiloo.subwayboard.rioboard03.bundletier1",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwayboard.rioboard03.bundletier1",
						payoutAmount = 1,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.Bundle,
						itemOfferType = InAppItemModel.OfferType.Normal,
						bundleContents = new List<InAppItemModel.BundleItem>
						{
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Board, Boards.BoardId.rioBoard03),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.BoardAbility, Boards.BoardId.rioBoard03, HoverboardThemes.ThemeIdentifier.rioBoard03Theme01)
						}
					}
				},
				{
					"com.kiloo.subwayboard.rioboard03.bundletier2",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwayboard.rioboard03.bundletier2",
						payoutAmount = 1,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.Bundle,
						itemOfferType = InAppItemModel.OfferType.Normal,
						bundleContents = new List<InAppItemModel.BundleItem>
						{
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Board, Boards.BoardId.rioBoard03),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.BoardAbility, Boards.BoardId.rioBoard03, HoverboardThemes.ThemeIdentifier.rioBoard03Theme01),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.BoardAbility, Boards.BoardId.rioBoard03, HoverboardThemes.ThemeIdentifier.rioBoard03Theme02)
						}
					}
				},
				{
					"com.kiloo.subwayboard.julyboard.bundletier1",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwayboard.julyboard.bundletier1",
						payoutAmount = 1,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.Bundle,
						itemOfferType = InAppItemModel.OfferType.Normal,
						bundleContents = new List<InAppItemModel.BundleItem>
						{
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Board, Boards.BoardId.julyBoard),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.BoardAbility, Boards.BoardId.julyBoard, HoverboardThemes.ThemeIdentifier.julyBoardTheme01)
						}
					}
				},
				{
					"com.kiloo.subwayboard.julyboard.bundletier2",
					new InAppItemModel
					{
						itemId = "com.kiloo.subwayboard.julyboard.bundletier2",
						payoutAmount = 1,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.Bundle,
						itemOfferType = InAppItemModel.OfferType.Normal,
						bundleContents = new List<InAppItemModel.BundleItem>
						{
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Board, Boards.BoardId.julyBoard),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.BoardAbility, Boards.BoardId.julyBoard, HoverboardThemes.ThemeIdentifier.julyBoardTheme01),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.BoardAbility, Boards.BoardId.julyBoard, HoverboardThemes.ThemeIdentifier.julyBoardTheme02)
						}
					}
				},
				{
					"com.kiloo.subways.dubaiturboteam.bundle",
					new InAppItemModel
					{
						itemId = "com.kiloo.subways.dubaiturboteam.bundle",
						payoutAmount = 1,
						isConsumable = false,
						itemType = InAppItemModel.ItemType.Bundle,
						itemOfferType = InAppItemModel.OfferType.Normal,
						bundleContents = new List<InAppItemModel.BundleItem>
						{
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Character, Characters.CharacterId.dubaiCharacter01),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Board, Boards.BoardId.dubaiBoard01),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Character, Characters.CharacterId.macauCharacter01),
							new InAppItemModel.BundleItem(InAppItemModel.ItemType.Board, Boards.BoardId.macauBoard01)
						}
					}
				}
			};
			foreach (Characters.CharacterId item in Characters.characterOrder)
			{
				if (Characters.characterData[item].hasOnlineSettings)
				{
					string text = ConvertCharacterTypeToDynamicPurchaseId(item);
					InAppItemModel value = new InAppItemModel
					{
						itemId = text,
						itemType = InAppItemModel.ItemType.Character,
						isDynamic = true,
						isConsumable = false,
						isValidated = false
					};
					inAppData.Add(text, value);
				}
			}
			foreach (Boards.BoardId item2 in Boards.boardOrder)
			{
				if (Boards.boardData[item2].hasOnlineTimeLimit || Boards.boardData[item2].unlockType == PurchaseData.UnlockType.HiddenUntilUnlocked)
				{
					string text2 = ConvertBoardTypeToDynamicPurchaseId(item2);
					InAppItemModel value2 = new InAppItemModel
					{
						itemId = text2,
						itemType = InAppItemModel.ItemType.Board,
						isDynamic = true,
						isConsumable = false,
						isValidated = false
					};
					inAppData.Add(text2, value2);
				}
			}
		}

		public static string StripProductPrefixes(string productID)
		{
			string[] iN_APP_PREFIXES = IN_APP_PREFIXES;
			foreach (string oldValue in iN_APP_PREFIXES)
			{
				productID = productID.Replace(oldValue, string.Empty);
			}
			return productID;
		}

		public static string GetBundleId(BundleTier tier)
		{
			switch (tier)
			{
			case BundleTier.None:
				SDebug.LogError("Bundle ID could not be found. BundleTier: " + tier);
				return "TierNotFound";
			case BundleTier.BundleTier1:
				return "com.kiloo.subways.bundletier1";
			case BundleTier.BundleTier2:
				return "com.kiloo.subways.bundletier2";
			case BundleTier.BundleTier3:
				return "com.kiloo.subways.bundletier3";
			case BundleTier.BundleTier4:
				return "com.kiloo.subways.bundletier4";
			case BundleTier.BundleTier5:
				return "com.kiloo.subways.bundletier5";
			default:
				SDebug.LogError("Bundle ID could not be found. BundleTier: " + tier);
				return "TierNotFound";
			}
		}

		public static BundleTier GetBundleTier(string bundleID)
		{
			switch (bundleID)
			{
			case "com.kiloo.subways.bundletier1":
				return BundleTier.BundleTier1;
			case "com.kiloo.subways.bundletier2":
				return BundleTier.BundleTier2;
			case "com.kiloo.subways.bundletier3":
				return BundleTier.BundleTier3;
			case "com.kiloo.subways.bundletier4":
				return BundleTier.BundleTier4;
			case "com.kiloo.subways.bundletier5":
				return BundleTier.BundleTier5;
			default:
				SDebug.LogError("Bundle Tier could not be found. BundleID: " + bundleID + ". Returning None");
				return BundleTier.None;
			}
		}

		public static string GetInAppOnlineSettingsId(string inAppId)
		{
			if (IN_APP_TIERS_ONLINE_SETTINGS_IDS.ContainsKey(inAppId))
			{
				return IN_APP_TIERS_ONLINE_SETTINGS_IDS[inAppId];
			}
			return null;
		}

		public static string GetIgnoredInAppOnlineSettingsId(string inAppId)
		{
			if (IGNORED_IN_APP_TIERS_ONLINE_SETTINGS_IDS.ContainsKey(inAppId))
			{
				return IGNORED_IN_APP_TIERS_ONLINE_SETTINGS_IDS[inAppId];
			}
			return null;
		}

		public static string GetDiscountIdForNormalOffer(string normalOffer)
		{
			string[] currencyDiscountTiers = GetCurrencyDiscountTiers();
			string[] array = currencyDiscountTiers;
			foreach (string text in array)
			{
				if (inAppData[text].normalOfferId == normalOffer)
				{
					return text;
				}
			}
			return string.Empty;
		}

		public static string GetSpecialDiscountIdForNormalOffer(string normalOffer)
		{
			string[] currencySpecialDiscountTiers = GetCurrencySpecialDiscountTiers();
			string[] array = currencySpecialDiscountTiers;
			foreach (string text in array)
			{
				if (inAppData[text].normalOfferId == normalOffer)
				{
					return text;
				}
			}
			return string.Empty;
		}

		public static string[] GetInAppBundleTiers()
		{
			return IN_APP_BUNDLE_TIERS;
		}

		public static string[] GetInAppLimitedBundleTiers()
		{
			return IN_APP_LIMITED_BUNDLE_TIERS;
		}

		public static string[] GetInAppBoardBundleTiers()
		{
			return IN_APP_BOARD_BUNDLE_TIERS;
		}

		public static string[] GetInAppSpecialBundles()
		{
			return inAppSpecialBundles;
		}

		public static bool InappidIsDynamicLimitedCharacterPurchase(string inappId)
		{
			return inappId.StartsWith("com.kiloo.subwaycharacter.");
		}

		public static bool InappidIsDynamicLimitedBoardPurchase(string inappId)
		{
			return inappId.StartsWith("com.kiloo.subwayboard.");
		}

		public static string[] GetCurrencyTiers()
		{
			return IN_APP_CURRENCY_TIERS;
		}

		public static string[] GetCurrencyDiscountTiers()
		{
			return IN_APP_CURRENCY_DISCOUNT_TIERS;
		}

		public static string[] GetCurrencySpecialDiscountTiers()
		{
			return IN_APP_CURRENCY_SPECIAL_DISCOUNT_TIERS;
		}

		public static string[] GetAdRemovalTiers()
		{
			return IN_APP_AD_REMOVAL_TIERS;
		}

		public static string[] GetAllProductIds()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, InAppItemModel> inAppDatum in inAppData)
			{
				list.Add(inAppDatum.Key);
			}
			return list.ToArray();
		}

		public static bool TryConvertDynamicPurchaseIdToCharacterType(string purchaseId, out Characters.CharacterId charReturned)
		{
			string value = purchaseId.Replace("com.kiloo.subwaycharacter.", string.Empty);
			try
			{
				charReturned = (Characters.CharacterId)Enum.Parse(typeof(Characters.CharacterId), value, true);
				return true;
			}
			catch (Exception)
			{
				foreach (KeyValuePair<Characters.CharacterId, Characters.Model> characterDatum in Characters.characterData)
				{
					Characters.Model value2 = characterDatum.Value;
					string dynamicIapId = value2.dynamicIapId;
					if (!string.IsNullOrEmpty(dynamicIapId) && dynamicIapId.ToLower() == purchaseId.ToLower() && IsUsingCustomIapIdForCharacter(value2))
					{
						charReturned = characterDatum.Key;
						return true;
					}
				}
			}
			charReturned = Characters.CharacterId.slick;
			return false;
		}

		public static string ConvertCharacterTypeToDynamicPurchaseId(Characters.CharacterId charId)
		{
			Characters.Model model = Characters.characterData[charId];
			string text = ((!IsUsingCustomIapIdForCharacter(model)) ? ("com.kiloo.subwaycharacter." + charId) : model.dynamicIapId);
			return text.ToLower();
		}

		private static bool IsUsingCustomIapIdForCharacter(Characters.Model model)
		{
			bool result = false;
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				if (model.overrideDynamicIapIdiOS)
				{
					result = true;
				}
				break;
			case RuntimePlatform.Android:
				if (DeviceUtility.isKindleDevice)
				{
					if (model.overrideDynamicIapIdKindle)
					{
						result = true;
					}
				}
				else if (model.overrideDynamicIapIdAndroid)
				{
					result = true;
				}
				break;
			}
			return result;
		}

		public static IEnumerable<InAppItemModel> GetAllWithItemAndOfferType(InAppItemModel.ItemType itemType, InAppItemModel.OfferType offerType)
		{
			foreach (InAppItemModel inAppItemModel in inAppData.Values)
			{
				if (inAppItemModel.itemType == itemType && inAppItemModel.itemOfferType == offerType)
				{
					yield return inAppItemModel;
				}
			}
		}

		public static void PrintDynamicIdsForCharacterTypeForAllPlatforms(Characters.CharacterId characterId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Characters.Model model = Characters.characterData[characterId];
			string text = Strings.Get(model.name);
			string dynamicIapId = model.dynamicIapId;
			string text2 = "com.kiloo.subwaycharacter." + characterId;
			stringBuilder.AppendLine(string.Format("iOS Id for " + text + ": {0}", (!model.overrideDynamicIapIdiOS) ? text2 : dynamicIapId));
			stringBuilder.AppendLine(string.Format("Android Id for " + text + ": {0}", (!model.overrideDynamicIapIdAndroid) ? text2.ToLower() : dynamicIapId.ToLower()));
			stringBuilder.AppendLine(string.Format("Kindle Id for " + text + ": {0}", (!model.overrideDynamicIapIdKindle) ? text2 : dynamicIapId));
		}

		public static bool TryConvertDynamicBoardIdToBoardType(string purchaseId, out Boards.BoardId boardReturned)
		{
			string value = purchaseId.Replace("com.kiloo.subwayboard.", string.Empty);
			try
			{
				boardReturned = (Boards.BoardId)Enum.Parse(typeof(Boards.BoardId), value, true);
				return true;
			}
			catch (Exception ex)
			{
				SDebug.LogError("InAppData: ConvertDynamicBoardIdToBoardType conversion failed. Must be a custom dynamic ID: " + ex);
				foreach (KeyValuePair<Boards.BoardId, Boards.Board> boardDatum in Boards.boardData)
				{
					Boards.Board value2 = boardDatum.Value;
					string dynamicIapId = value2.dynamicIapId;
					if (!string.IsNullOrEmpty(dynamicIapId) && dynamicIapId.ToLower() == purchaseId.ToLower() && IsUsingCustomIapIdForBoard(value2))
					{
						boardReturned = boardDatum.Key;
						return true;
					}
				}
			}
			boardReturned = Boards.BoardId.normal;
			return false;
		}

		public static string ConvertBoardTypeToDynamicPurchaseId(Boards.BoardId boardId)
		{
			Boards.Board board = Boards.boardData[boardId];
			string text = ((!IsUsingCustomIapIdForBoard(board)) ? ("com.kiloo.subwayboard." + boardId) : board.dynamicIapId);
			return text.ToLower();
		}

		private static bool IsUsingCustomIapIdForBoard(Boards.Board board)
		{
			bool result = false;
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				if (board.overrideDynamicIapIdiOS)
				{
					result = true;
				}
				break;
			case RuntimePlatform.Android:
				if (DeviceUtility.isKindleDevice)
				{
					if (board.overrideDynamicIapIdKindle)
					{
						result = true;
					}
				}
				else if (board.overrideDynamicIapIdAndroid)
				{
					result = true;
				}
				break;
			}
			return result;
		}

		public static void PrintDynamicIdsForBoardTypeForAllPlatforms(Boards.BoardId boardId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Boards.Board board = Boards.boardData[boardId];
			string text = Strings.Get(board.name);
			string dynamicIapId = board.dynamicIapId;
			string text2 = "com.kiloo.subwayboard." + boardId;
			stringBuilder.AppendLine(string.Format("iOS Id for " + text + ": {0}", (!board.overrideDynamicIapIdiOS) ? text2 : dynamicIapId));
			stringBuilder.AppendLine(string.Format("Android Id for " + text + ": {0}", (!board.overrideDynamicIapIdAndroid) ? text2.ToLower() : dynamicIapId.ToLower()));
			stringBuilder.AppendLine(string.Format("Kindle Id for " + text + ": {0}", (!board.overrideDynamicIapIdKindle) ? text2 : dynamicIapId));
		}

		public static string OutputAllInapps()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, InAppItemModel> inAppDatum in inAppData)
			{
				stringBuilder.AppendFormat("{0}\n", inAppDatum.Key);
			}
			return stringBuilder.ToString();
		}
	}
}
