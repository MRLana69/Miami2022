using System;
using System.Collections.Generic;
using Info;

namespace SYBO.Subway.Info
{
	public class Characters
	{
		public enum CharacterId
		{
			slick = 0,
			tricky = 1,
			fresh = 2,
			spike = 3,
			yutani = 4,
			frank = 5,
			frizzy = 6,
			king = 7,
			lucy = 8,
			ninja = 9,
			tag = 10,
			tasha = 11,
			zoe = 12,
			brody = 13,
			princek = 14,
			zombiejake = 15,
			elftricky = 16,
			tony = 17,
			carmen = 18,
			roberto = 19,
			kim = 20,
			harumi = 21,
			nick = 22,
			coco = 23,
			monkeyking = 24,
			moscowCharacter01 = 25,
			halloweenCharacter01 = 26,
			londonCharacter01 = 27,
			mumbaiCharacter01 = 28,
			seoulCharacter01 = 29,
			mexicoCharacter01 = 30,
			vancouverCharacter01 = 31,
			saopauloCharacter01 = 32,
			losangelesCharacter01 = 33,
			cairoCharacter01 = 34,
			bangkokCharacter01 = 35,
			facebookRewardCharacter01 = 36,
			londonCharacter02 = 37,
			lasVegasCharacter01 = 38,
			hawaiiCharacter01 = 39,
			arabiaCharacter01 = 40,
			veniceCharacter01 = 41,
			greeceCharacter01 = 42,
			kenyaCharacter01 = 43,
			transylvaniaCharacter01 = 44,
			sanfranciscoCharacter01 = 45,
			pragueCharacter01 = 46,
			madagascarCharacter01 = 47,
			peruCharacter01 = 48,
			singaporeCharacter01 = 49,
			icelandCharacter01 = 50,
			havanaCharacter01 = 51,
			washingtonCharacter01 = 52,
			holidayCharacter2016 = 53,
			amsterdamCharacter01 = 54,
			monacoCharacter01 = 55,
			copenhagenCharacter01 = 56,
			marrakeshCharacter01 = 57,
			shanghaiCharacter01 = 58,
			barcelonaCharacter01 = 59,
			mexicoCharacter02 = 60,
			stPetersburgCharacter01 = 61,
			chicagoCharacter01 = 62,
			buenosAiresCharacter01 = 63,
			veniceBeachCharacter01 = 64,
			firstPurchaseBonusCharacter01 = 65,
			berlinCharacter01 = 66,
			hongKongCharacter01 = 67,
			newOrleansCharacter01 = 68,
			atlantaCharacter01 = 69,
			graffitiGirl = 70,
			zurichCharacter01 = 71,
			easterSpecialCharacter01 = 72,
			guangzhouCharacter01 = 73,
			dubaiCharacter01 = 104,
			macauCharacter01 = 105
		}

		public class Model
		{
			public string name;

			public string modelName;

			public int Price;

			public PurchaseData.UnlockType unlockType;

			public string tokenName;

			public string tokenSprite2dName;

			public MissionTarget missionTargetKey;

			public string characterLimitedDescription;

			public bool isNewInThisUpdate;

			public bool hasOnlineSettings;

			public DateTime defaultExpirationDate;

			public DateTime defaultStartDate;

			public bool overrideDynamicIapIdAndroid;

			public bool overrideDynamicIapIdiOS;

			public bool overrideDynamicIapIdKindle;

			public string dynamicIapId;

			public bool isPartOfBundle;
		}

		public const string ONLINE_SETTINGS_CHARACTER_PREFIX_EXPIRE = "character_expiretime_";

		public const string ONLINE_SETTINGS_CHARACTER_PREFIX_START = "character_starttime_";

		public static readonly Dictionary<CharacterId, Model> characterData = new Dictionary<CharacterId, Model>
		{
			{
				CharacterId.slick,
				new Model
				{
					name = "CHARACTERS_JAKE",
					modelName = "slick",
					tokenName = "CHARACTER_TOKEN_NAME_JAKE",
					tokenSprite2dName = "trophy_jake_token",
					unlockType = PurchaseData.UnlockType.AlwaysUnlocked,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterId.tricky,
				new Model
				{
					name = "CHARACTERS_TRICKY",
					modelName = "tricky",
					unlockType = PurchaseData.UnlockType.Tokens,
					tokenName = "CHARACTER_TOKEN_NAME_TRICKY",
					tokenSprite2dName = "trophy_tricky_token",
					Price = 3,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterId.fresh,
				new Model
				{
					name = "CHARACTERS_FRESH",
					modelName = "fresh",
					tokenName = "CHARACTER_TOKEN_NAME_FRESH",
					tokenSprite2dName = "trophy_fresh_token",
					unlockType = PurchaseData.UnlockType.Tokens,
					Price = 50,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterId.spike,
				new Model
				{
					name = "CHARACTERS_SPIKE",
					modelName = "spike",
					unlockType = PurchaseData.UnlockType.Tokens,
					tokenName = "CHARACTER_TOKEN_NAME_SPIKE",
					tokenSprite2dName = "trophy_spike_token",
					Price = 200,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterId.yutani,
				new Model
				{
					name = "CHARACTERS_YUTANI",
					modelName = "yutani",
					unlockType = PurchaseData.UnlockType.Tokens,
					tokenName = "CHARACTER_TOKEN_NAME_YUTANI",
					tokenSprite2dName = "trophy_yutani_token",
					Price = 500,
					missionTargetKey = MissionTarget.HaveYutani,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterId.lucy,
				new Model
				{
					name = "CHARACTERS_LUCY",
					modelName = "lucy",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 7000,
					missionTargetKey = MissionTarget.HaveLucy,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterId.ninja,
				new Model
				{
					name = "CHARACTERS_NINJA",
					modelName = "ninja",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 20000,
					missionTargetKey = MissionTarget.HaveNinja,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterId.frank,
				new Model
				{
					name = "CHARACTERS_FRANK",
					modelName = "frank",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 40000,
					missionTargetKey = MissionTarget.HaveFrank,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterId.king,
				new Model
				{
					name = "CHARACTERS_KING",
					modelName = "king",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 80000,
					missionTargetKey = MissionTarget.HaveKing,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterId.frizzy,
				new Model
				{
					name = "CHARACTERS_FRIZZY",
					modelName = "frizzy",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 150000,
					missionTargetKey = MissionTarget.HaveFrizzy,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterId.tag,
				new Model
				{
					name = "CHARACTERS_TAGBOT",
					modelName = "tag",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 12000,
					missionTargetKey = MissionTarget.HaveTag,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterId.tasha,
				new Model
				{
					name = "CHARACTERS_TASHA",
					modelName = "tasha",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 30000,
					missionTargetKey = MissionTarget.HaveTasha,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterId.zoe,
				new Model
				{
					name = "CHARACTERS_ZOE",
					modelName = "zoe",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 120000,
					missionTargetKey = MissionTarget.HaveZoe,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterId.brody,
				new Model
				{
					name = "CHARACTERS_BRODY",
					modelName = "brody",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 350000,
					missionTargetKey = MissionTarget.HaveBrody,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterId.princek,
				new Model
				{
					name = "CHARACTERS_PRINCE_K",
					modelName = "princek",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 980000,
					missionTargetKey = MissionTarget.HavePrinceK,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterId.zombiejake,
				new Model
				{
					name = "CHARACTERS_ZOMBIE_JAKE",
					modelName = "zombiejake",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 9, 13),
					defaultExpirationDate = new DateTime(2018, 11, 1),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_HALLOWEEN"
				}
			},
			{
				CharacterId.elftricky,
				new Model
				{
					name = "CHARACTERS_ELF_TRICHY",
					modelName = "elftricky",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 11, 2),
					defaultExpirationDate = new DateTime(2018, 12, 12),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_HOLIDAY"
				}
			},
			{
				CharacterId.tony,
				new Model
				{
					name = "CHARACTERS_TONY",
					modelName = "tony",
					unlockType = PurchaseData.UnlockType.Keys,
					Price = 40,
					isNewInThisUpdate = true,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 5, 29),
					defaultExpirationDate = new DateTime(2019, 7, 18),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_NEW_YORK"
				}
			},
			{
				CharacterId.carmen,
				new Model
				{
					name = "CHARACTERS_CARMEN",
					modelName = "carmen",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 11, 27),
					defaultExpirationDate = new DateTime(2018, 12, 12),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_RIO"
				}
			},
			{
				CharacterId.roberto,
				new Model
				{
					name = "CHARACTERS_ROBERTO",
					modelName = "roberto",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2014, 3, 20),
					defaultExpirationDate = new DateTime(2014, 5, 8),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_ROME"
				}
			},
			{
				CharacterId.kim,
				new Model
				{
					name = "CHARACTERS_KIM",
					modelName = "kim",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2016, 3, 30),
					defaultExpirationDate = new DateTime(2016, 5, 12),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_SYDNEY"
				}
			},
			{
				CharacterId.harumi,
				new Model
				{
					name = "CHARACTERS_HARUMI",
					modelName = "harumi",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 1, 31),
					defaultExpirationDate = new DateTime(2018, 3, 22),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_TOKYO"
				}
			},
			{
				CharacterId.nick,
				new Model
				{
					name = "CHARACTERS_NICK",
					modelName = "nick",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = true,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 5, 29),
					defaultExpirationDate = new DateTime(2019, 7, 18),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_MIAMI"
				}
			},
			{
				CharacterId.coco,
				new Model
				{
					name = "CHARACTERS_COCO",
					modelName = "coco",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 1, 14),
					defaultExpirationDate = new DateTime(2019, 1, 31),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_PARIS"
				}
			},
			{
				CharacterId.monkeyking,
				new Model
				{
					name = "CHARACTERS_MONKEYKING",
					modelName = "monkeyking",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 2, 4),
					defaultExpirationDate = new DateTime(2019, 2, 5),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_BEIJING"
				}
			},
			{
				CharacterId.moscowCharacter01,
				new Model
				{
					name = "CHARACTERS_MOSCOWCHARACTER01",
					modelName = "moscowCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2013, 9, 4),
					defaultExpirationDate = new DateTime(2013, 10, 10),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_MOSCOW"
				}
			},
			{
				CharacterId.halloweenCharacter01,
				new Model
				{
					name = "CHARACTERS_HALLOWEENCHARACTER01",
					modelName = "halloweenCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 9, 13),
					defaultExpirationDate = new DateTime(2018, 11, 1),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_NEW_ORLEANS"
				}
			},
			{
				CharacterId.londonCharacter01,
				new Model
				{
					name = "CHARACTERS_LONDON_CHARACTER01",
					modelName = "londonCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 11, 2),
					defaultExpirationDate = new DateTime(2018, 12, 12),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_LONDON"
				}
			},
			{
				CharacterId.mumbaiCharacter01,
				new Model
				{
					name = "CHARACTERS_MUMBAI_CHARACTER01",
					modelName = "mumbaiCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 5, 28),
					defaultExpirationDate = new DateTime(2018, 7, 12),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_MUMBAI"
				}
			},
			{
				CharacterId.seoulCharacter01,
				new Model
				{
					name = "CHARACTERS_SEOUL_CHARACTER01",
					modelName = "seoulCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 4, 9),
					defaultExpirationDate = new DateTime(2019, 5, 7),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_SEOUL"
				}
			},
			{
				CharacterId.mexicoCharacter01,
				new Model
				{
					name = "CHARACTERS_MEXICO_CHARACTER01",
					modelName = "mexicoCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 1, 14),
					defaultExpirationDate = new DateTime(2019, 1, 31),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_MEXICO_CITY"
				}
			},
			{
				CharacterId.vancouverCharacter01,
				new Model
				{
					name = "CHARACTERS_VANCOUVER_CHARACTER01",
					modelName = "vancouverCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 12, 13),
					defaultExpirationDate = new DateTime(2019, 1, 3),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_VANCOUVER"
				}
			},
			{
				CharacterId.saopauloCharacter01,
				new Model
				{
					name = "CHARACTERS_SAOPAULO_CHARACTER01",
					modelName = "saopauloCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 12, 13),
					defaultExpirationDate = new DateTime(2019, 1, 3),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_SAOPAULO"
				}
			},
			{
				CharacterId.losangelesCharacter01,
				new Model
				{
					name = "CHARACTERS_LOSANGELES_CHARACTER01",
					modelName = "losangelesCharacter01",
					unlockType = PurchaseData.UnlockType.Keys,
					Price = 40,
					isNewInThisUpdate = true,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 5, 29),
					defaultExpirationDate = new DateTime(2019, 7, 18),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_LOSANGELES"
				}
			},
			{
				CharacterId.cairoCharacter01,
				new Model
				{
					name = "CHARACTERS_CAIRO_CHARACTER01",
					modelName = "cairoCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 1, 14),
					defaultExpirationDate = new DateTime(2019, 1, 31),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_CAIRO"
				}
			},
			{
				CharacterId.bangkokCharacter01,
				new Model
				{
					name = "CHARACTERS_BANGKOK_CHARACTER01",
					modelName = "bangkokCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 3, 22),
					defaultExpirationDate = new DateTime(2019, 4, 8),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_BANGKOK"
				}
			},
			{
				CharacterId.facebookRewardCharacter01,
				new Model
				{
					name = "CHARACTERS_FACEBOOK_REWARD_CHARACTER01",
					modelName = "facebookRewardCharacter01",
					unlockType = PurchaseData.UnlockType.Facebook,
					Price = 1,
					isNewInThisUpdate = false,
					hasOnlineSettings = false,
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_FACEBOOK"
				}
			},
			{
				CharacterId.londonCharacter02,
				new Model
				{
					name = "CHARACTERS_LONDON_CHARACTER02",
					modelName = "londonCharacter02",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 11, 2),
					defaultExpirationDate = new DateTime(2018, 12, 12),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_HOLIDAY"
				}
			},
			{
				CharacterId.lasVegasCharacter01,
				new Model
				{
					name = "CHARACTERS_LAS_VEGAS_CHARACTER01",
					modelName = "lasVegasCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 5, 29),
					defaultExpirationDate = new DateTime(2019, 7, 18),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_LAS_VEGAS"
				}
			},
			{
				CharacterId.hawaiiCharacter01,
				new Model
				{
					name = "CHARACTERS_HAWAII_CHARACTER01",
					modelName = "hawaiiCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 6, 29),
					defaultExpirationDate = new DateTime(2018, 7, 6),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_HAWAII"
				}
			},
			{
				CharacterId.arabiaCharacter01,
				new Model
				{
					name = "CHARACTERS_ARABIA_CHARACTER01",
					modelName = "arabiaCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2017, 1, 5),
					defaultExpirationDate = new DateTime(2017, 3, 2),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_ARABIA"
				}
			},
			{
				CharacterId.veniceCharacter01,
				new Model
				{
					name = "CHARACTERS_VENICE_CHARACTER01",
					modelName = "veniceCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 1, 14),
					defaultExpirationDate = new DateTime(2019, 1, 31),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_VENICE"
				}
			},
			{
				CharacterId.greeceCharacter01,
				new Model
				{
					name = "CHARACTERS_GREECE_CHARACTER01",
					modelName = "greeceCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 1, 14),
					defaultExpirationDate = new DateTime(2019, 1, 31),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_GREECE"
				}
			},
			{
				CharacterId.kenyaCharacter01,
				new Model
				{
					name = "CHARACTERS_KENYA_CHARACTER01",
					modelName = "kenyaCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2015, 9, 3),
					defaultExpirationDate = new DateTime(2015, 10, 15),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_KENYA"
				}
			},
			{
				CharacterId.transylvaniaCharacter01,
				new Model
				{
					name = "CHARACTERS_TRANSYLVANIA_CHARACTER01",
					modelName = "transylvaniaCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 9, 13),
					defaultExpirationDate = new DateTime(2018, 11, 1),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_TRANSYLVANIA"
				}
			},
			{
				CharacterId.sanfranciscoCharacter01,
				new Model
				{
					name = "CHARACTERS_SANFRANCISCO_CHARACTER01",
					modelName = "sanfranciscoCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = true,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 5, 29),
					defaultExpirationDate = new DateTime(2019, 7, 18),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_SAN_FRANCISCO"
				}
			},
			{
				CharacterId.pragueCharacter01,
				new Model
				{
					name = "CHARACTERS_PRAGUE_CHARACTER01",
					modelName = "pragueCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 1, 14),
					defaultExpirationDate = new DateTime(2019, 1, 31),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_PRAGUE"
				}
			},
			{
				CharacterId.madagascarCharacter01,
				new Model
				{
					name = "CHARACTERS_MADAGASCAR_CHARACTER01",
					modelName = "madagascarCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 3, 3),
					defaultExpirationDate = new DateTime(2019, 3, 21),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_MADAGASCAR"
				}
			},
			{
				CharacterId.peruCharacter01,
				new Model
				{
					name = "CHARACTERS_PERU_CHARACTER01",
					modelName = "peruCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2017, 5, 4),
					defaultExpirationDate = new DateTime(2017, 6, 22),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_PERU"
				}
			},
			{
				CharacterId.singaporeCharacter01,
				new Model
				{
					name = "CHARACTERS_SINGAPORE_CHARACTER01",
					modelName = "singaporeCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 1, 14),
					defaultExpirationDate = new DateTime(2019, 1, 31),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_SINGAPORE"
				}
			},
			{
				CharacterId.icelandCharacter01,
				new Model
				{
					name = "CHARACTERS_ICELAND_CHARACTER01",
					modelName = "icelandCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 2, 21),
					defaultExpirationDate = new DateTime(2018, 4, 12),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_ICELAND"
				}
			},
			{
				CharacterId.havanaCharacter01,
				new Model
				{
					name = "CHARACTERS_HAVANA_CHARACTER01",
					modelName = "havanaCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 6, 18),
					defaultExpirationDate = new DateTime(2018, 8, 9),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_HAVANA"
				}
			},
			{
				CharacterId.washingtonCharacter01,
				new Model
				{
					name = "CHARACTERS_WASHINGTON_CHARACTER01",
					modelName = "washingtonCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = true,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 5, 29),
					defaultExpirationDate = new DateTime(2019, 7, 18),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_WASHINGTON"
				}
			},
			{
				CharacterId.holidayCharacter2016,
				new Model
				{
					name = "CHARACTERS_WINTER_HOLIDAY_CHARACTER_2016",
					modelName = "holidayCharacter2016",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 11, 2),
					defaultExpirationDate = new DateTime(2018, 12, 12),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_WINTER_HOLIDAY"
				}
			},
			{
				CharacterId.amsterdamCharacter01,
				new Model
				{
					name = "CHARACTERS_AMSTERDAM_CHARACTER01",
					modelName = "amsterdamCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2016, 11, 7),
					defaultExpirationDate = new DateTime(2017, 1, 19),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_AMSTERDAM"
				}
			},
			{
				CharacterId.monacoCharacter01,
				new Model
				{
					name = "CHARACTERS_MONACO_CHARACTER01",
					modelName = "monacoCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2017, 4, 6),
					defaultExpirationDate = new DateTime(2018, 5, 31),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_MONACO"
				}
			},
			{
				CharacterId.copenhagenCharacter01,
				new Model
				{
					name = "CHARACTERS_COPENHAGEN_CHARACTER01",
					modelName = "copenhagenCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 4, 9),
					defaultExpirationDate = new DateTime(2019, 5, 7),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_COPENHAGEN"
				}
			},
			{
				CharacterId.marrakeshCharacter01,
				new Model
				{
					name = "CHARACTERS_MARRAKESH_CHARACTER01",
					modelName = "marrakeshCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 10, 8),
					defaultExpirationDate = new DateTime(2018, 11, 29),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_MARRAKESH"
				}
			},
			{
				CharacterId.shanghaiCharacter01,
				new Model
				{
					name = "CHARACTERS_SHANGHAI_CHARACTER01",
					modelName = "shanghaiCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 2, 4),
					defaultExpirationDate = new DateTime(2019, 2, 5),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_SHANGHAI"
				}
			},
			{
				CharacterId.barcelonaCharacter01,
				new Model
				{
					name = "CHARACTERS_BARCELONA_CHARACTER01",
					modelName = "barcelonaCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2017, 8, 31),
					defaultExpirationDate = new DateTime(2017, 9, 21),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_BARCELONA"
				}
			},
			{
				CharacterId.mexicoCharacter02,
				new Model
				{
					name = "CHARACTERS_MEXICO_CHARACTER02",
					modelName = "mexicoCharacter02",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 9, 13),
					defaultExpirationDate = new DateTime(2018, 11, 1),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_MEXICO"
				}
			},
			{
				CharacterId.stPetersburgCharacter01,
				new Model
				{
					name = "CHARACTERS_ST_PETERSBURG_CHARACTER01",
					modelName = "stPetersburgCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 11, 2),
					defaultExpirationDate = new DateTime(2018, 12, 12),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_ST_PETERSBURG"
				}
			},
			{
				CharacterId.chicagoCharacter01,
				new Model
				{
					name = "CHARACTERS_CHICAGO_CHARACTER01",
					modelName = "chicagoCharacter01",
					unlockType = PurchaseData.UnlockType.Keys,
					Price = 40,
					isNewInThisUpdate = true,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 5, 29),
					defaultExpirationDate = new DateTime(2019, 7, 18),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_CHICAGO"
				}
			},
			{
				CharacterId.buenosAiresCharacter01,
				new Model
				{
					name = "CHARACTERS_BUENOS_AIRES_CHARACTER01",
					modelName = "buenosAiresCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 3, 14),
					defaultExpirationDate = new DateTime(2018, 5, 3),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_BUENOS_AIRES"
				}
			},
			{
				CharacterId.veniceBeachCharacter01,
				new Model
				{
					name = "CHARACTERS_VENICE_BEACH_CHARACTER01",
					modelName = "veniceBeachCharacter01",
					unlockType = PurchaseData.UnlockType.InApp,
					Price = 95000,
					isNewInThisUpdate = true,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 5, 29),
					defaultExpirationDate = new DateTime(2019, 7, 18),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_VENICE_BEACH"
				}
			},
			{
				CharacterId.firstPurchaseBonusCharacter01,
				new Model
				{
					name = "CHARACTERS_FIRST_PURCHASE_BONUS_CHARACTER01",
					modelName = "firstPurchaseBonusCharacter01",
					unlockType = PurchaseData.UnlockType.RewardForInApp,
					isNewInThisUpdate = false,
					hasOnlineSettings = false,
					Price = 1,
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_FIRSTPURCHASEBONUS"
				}
			},
			{
				CharacterId.berlinCharacter01,
				new Model
				{
					name = "CHARACTERS_BERLIN_CHARACTER01",
					modelName = "berlinCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 2, 1),
					defaultExpirationDate = new DateTime(2019, 3, 3),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_BERLIN"
				}
			},
			{
				CharacterId.hongKongCharacter01,
				new Model
				{
					name = "CHARACTERS_HONG_KONG_CHARACTER01",
					modelName = "hongKongCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 2, 4),
					defaultExpirationDate = new DateTime(2019, 2, 5),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_HONG_KONG"
				}
			},
			{
				CharacterId.newOrleansCharacter01,
				new Model
				{
					name = "CHARACTERS_NEW_ORLEANS_CHARACTER01",
					modelName = "newOrleansCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2018, 9, 13),
					defaultExpirationDate = new DateTime(2018, 10, 7),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_HALLOWEEN"
				}
			},
			{
				CharacterId.atlantaCharacter01,
				new Model
				{
					name = "CHARACTERS_ATLANTA_CHARACTER01",
					modelName = "atlantaCharacter01",
					unlockType = PurchaseData.UnlockType.InApp,
					Price = 95000,
					isNewInThisUpdate = true,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 5, 29),
					defaultExpirationDate = new DateTime(2019, 7, 18),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_ATLANTA"
				}
			},
			{
				CharacterId.graffitiGirl,
				new Model
				{
					name = "CHARACTERS_GRAFFITI_GIRL",
					modelName = "graffitiGirl",
					unlockType = PurchaseData.UnlockType.Keys,
					Price = 100,
					isNewInThisUpdate = false,
					hasOnlineSettings = false
				}
			},
			{
				CharacterId.zurichCharacter01,
				new Model
				{
					name = "CHARACTERS_ZURICH_CHARACTER01",
					modelName = "zurichCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 3, 3),
					defaultExpirationDate = new DateTime(2019, 3, 21),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_ZURICH"
				}
			},
			{
				CharacterId.easterSpecialCharacter01,
				new Model
				{
					name = "CHARACTERS_EASTER_SPECIAL",
					modelName = "easterSpecialCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 3, 3),
					defaultExpirationDate = new DateTime(2019, 3, 21),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_EASTER"
				}
			},
			{
				CharacterId.guangzhouCharacter01,
				new Model
				{
					name = "CHARACTERS_GUANGZHOU_CHARACTER01",
					modelName = "guangzhouCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 4, 9),
					defaultExpirationDate = new DateTime(2019, 5, 7),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_EVENT"
				}
			},
			{
				CharacterId.dubaiCharacter01,
				new Model
				{
					name = "CHARACTERS_DUBAI_CHARACTER01",
					modelName = "dubaiCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 5, 8),
					defaultExpirationDate = new DateTime(2019, 5, 28),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_DUBAI"
				}
			},
			{
				CharacterId.macauCharacter01,
				new Model
				{
					name = "CHARACTERS_MACAU_CHARACTER01",
					modelName = "macauCharacter01",
					unlockType = PurchaseData.UnlockType.Coins,
					Price = 95000,
					isNewInThisUpdate = false,
					hasOnlineSettings = true,
					defaultStartDate = new DateTime(2019, 5, 8),
					defaultExpirationDate = new DateTime(2019, 5, 28),
					characterLimitedDescription = "UPDATE_SPECIAL_TAG_EVENT"
				}
			}
		};

		public static List<CharacterId> characterOrder = new List<CharacterId>
		{
			CharacterId.yutani,
			CharacterId.spike,
			CharacterId.fresh,
			CharacterId.tricky,
			CharacterId.slick,
			CharacterId.graffitiGirl,
			CharacterId.firstPurchaseBonusCharacter01,
			CharacterId.facebookRewardCharacter01,
			CharacterId.nick,
			CharacterId.veniceBeachCharacter01,
			CharacterId.tony,
			CharacterId.chicagoCharacter01,
			CharacterId.losangelesCharacter01,
			CharacterId.washingtonCharacter01,
			CharacterId.sanfranciscoCharacter01,
			CharacterId.atlantaCharacter01,
			CharacterId.lasVegasCharacter01,
			CharacterId.dubaiCharacter01,
			CharacterId.macauCharacter01,
			CharacterId.guangzhouCharacter01,
			CharacterId.seoulCharacter01,
			CharacterId.copenhagenCharacter01,
			CharacterId.bangkokCharacter01,
			CharacterId.zurichCharacter01,
			CharacterId.easterSpecialCharacter01,
			CharacterId.madagascarCharacter01,
			CharacterId.monkeyking,
			CharacterId.shanghaiCharacter01,
			CharacterId.hongKongCharacter01,
			CharacterId.vancouverCharacter01,
			CharacterId.saopauloCharacter01,
			CharacterId.carmen,
			CharacterId.londonCharacter01,
			CharacterId.elftricky,
			CharacterId.londonCharacter02,
			CharacterId.holidayCharacter2016,
			CharacterId.stPetersburgCharacter01,
			CharacterId.marrakeshCharacter01,
			CharacterId.newOrleansCharacter01,
			CharacterId.halloweenCharacter01,
			CharacterId.zombiejake,
			CharacterId.mexicoCharacter02,
			CharacterId.transylvaniaCharacter01,
			CharacterId.berlinCharacter01,
			CharacterId.havanaCharacter01,
			CharacterId.mumbaiCharacter01,
			CharacterId.hawaiiCharacter01,
			CharacterId.monacoCharacter01,
			CharacterId.buenosAiresCharacter01,
			CharacterId.icelandCharacter01,
			CharacterId.cairoCharacter01,
			CharacterId.harumi,
			CharacterId.coco,
			CharacterId.singaporeCharacter01,
			CharacterId.barcelonaCharacter01,
			CharacterId.peruCharacter01,
			CharacterId.arabiaCharacter01,
			CharacterId.amsterdamCharacter01,
			CharacterId.veniceCharacter01,
			CharacterId.kim,
			CharacterId.pragueCharacter01,
			CharacterId.kenyaCharacter01,
			CharacterId.greeceCharacter01,
			CharacterId.mexicoCharacter01,
			CharacterId.moscowCharacter01,
			CharacterId.roberto,
			CharacterId.frank,
			CharacterId.frizzy,
			CharacterId.king,
			CharacterId.lucy,
			CharacterId.ninja,
			CharacterId.tag,
			CharacterId.tasha,
			CharacterId.zoe,
			CharacterId.brody,
			CharacterId.princek
		};

		public static CharacterId? GetCharacterTypeForName(string name)
		{
			foreach (KeyValuePair<CharacterId, Model> characterDatum in characterData)
			{
				if (Strings.Get(characterDatum.Value.name) == name)
				{
					return characterDatum.Key;
				}
			}
			return null;
		}
	}
}
