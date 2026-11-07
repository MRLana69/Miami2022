using System;
using System.Collections.Generic;

namespace Info
{
	public static class Boards
	{
		public enum BoardId
		{
			normal = 0,
			bouncer = 1,
			lowrider = 2,
			snowboard = 3,
			surfboard = 4,
			theoriginal = 5,
			starboard = 6,
			miami = 7,
			monster = 8,
			skullfire = 9,
			liberty = 10,
			speedboard = 11,
			toucan = 12,
			treehugger = 13,
			chick = 14,
			rome = 15,
			soccer = 16,
			theOutback = 17,
			greatWhiteWakeboard = 18,
			cherry = 19,
			kitty = 20,
			hotrod = 21,
			flamingo = 22,
			rose = 23,
			panda = 24,
			dragon = 25,
			moscowBoard01 = 26,
			halloweenBoard01 = 27,
			londonBoard01 = 28,
			londonBoard02 = 29,
			mumbaiBoard01 = 30,
			teleboard = 31,
			gliderboard = 32,
			seoulBoard01 = 33,
			mexicoBoard01 = 34,
			vancouverBoard01 = 35,
			saopauloBoard01 = 36,
			losangelesBoard01 = 37,
			cairoBoard01 = 38,
			bangkokBoard01 = 39,
			londonBoard03 = 40,
			lasVegasBoard01 = 41,
			hawaiiBoard01 = 42,
			arabiaBoard01 = 43,
			veniceBoard01 = 44,
			rioBoard01 = 45,
			sydneyBoard01 = 46,
			greeceBoard01 = 47,
			kenyaBoard01 = 48,
			transylvaniaBoard01 = 49,
			northpoleBoard01 = 50,
			sanfranciscoBoard01 = 51,
			pragueBoard01 = 52,
			madagascarBoard01 = 53,
			sydneyBoard02 = 54,
			peruBoard01 = 55,
			singaporeBoard01 = 56,
			veniceBoard02 = 57,
			rioBoard02 = 58,
			icelandBoard01 = 59,
			havanaBoard01 = 60,
			halloweenBoard02 = 61,
			washingtonBoard01 = 62,
			northpoleBoard02 = 63,
			amsterdamBoard01 = 64,
			arabiaBoard02 = 65,
			monacoBoard01 = 66,
			hawaiiBoard02 = 67,
			copenhagenBoard01 = 68,
			birthdayBoard01 = 69,
			marrakeshBoard01 = 70,
			shanghaiBoard01 = 71,
			barcelonaBoard01 = 72,
			mexicoBoard02 = 73,
			stPetersburgBoard01 = 74,
			cairoBoard02 = 75,
			chicagoBoard01 = 76,
			parisBoard01 = 77,
			buenosAiresBoard01 = 78,
			veniceBeachBoard01 = 79,
			mumbaiBoard02 = 80,
			starterPackBoard01 = 81,
			newYorkBoard01 = 82,
			berlinBoard01 = 83,
			hongKongBoard01 = 84,
			marrakeshBoard02 = 95,
			rioBoard03 = 96,
			atlantaBoard01 = 98,
			harbinBoard01 = 99,
			sanfranciscoBoard02 = 100,
			zurichBoard01 = 101,
			dubaiBoard01 = 104,
			macauBoard01 = 105,
			julyBoard = 106
		}

		public class Board
		{
			public string name;

			public string boardModelName;

			public int price;

			public PurchaseData.UnlockType unlockType;

			public string tokenSprite2dName;

			public string description;

			public string limitedDescription;

			public bool isNewInThisUpdate;

			public bool hasOnlineTimeLimit;

			public DateTime defaultExpirationTime;

			public DateTime defaultStartDate;

			public bool overrideDynamicIapIdAndroid;

			public bool overrideDynamicIapIdiOS;

			public bool overrideDynamicIapIdKindle;

			public string dynamicIapId;

			public bool isPartOfBundle;
		}

		public const string ONLINE_SETTINGS_BOARD_PREFIX_EXPIRE = "hoverboard_expiretime_";

		public const string ONLINE_SETTINGS_BOARD_PREFIX_START = "hoverboard_starttime_";

		public static readonly Dictionary<BoardId, Board> boardData = new Dictionary<BoardId, Board>
		{
			{
				BoardId.normal,
				new Board
				{
					name = "HOVERBOARD_TITLE",
					boardModelName = "Hoverboard",
					price = 0,
					unlockType = PurchaseData.UnlockType.AlwaysUnlocked,
					description = string.Empty,
					limitedDescription = string.Empty,
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = false
				}
			},
			{
				BoardId.bouncer,
				new Board
				{
					name = "HOVERBOARDS_BOUNCER",
					boardModelName = "Jumpboard",
					price = 280000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = "HOVERBOARD_ABILITY_SUPER_JUMP_DESCRIPTION",
					limitedDescription = string.Empty,
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = false
				}
			},
			{
				BoardId.lowrider,
				new Board
				{
					name = "HOVERBOARDS_LOWRIDER",
					boardModelName = "Lowrider",
					price = 320000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = "HOVERBOARD_ABILITY_STAY_LOW_DESCRIPTION",
					limitedDescription = string.Empty,
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = false
				}
			},
			{
				BoardId.snowboard,
				new Board
				{
					name = "HOVERBOARDS_FREESTYLER",
					boardModelName = "Snowboard",
					price = 45000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = string.Empty,
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = false
				}
			},
			{
				BoardId.surfboard,
				new Board
				{
					name = "HOVERBOARDS_BIG_KAHUNA",
					boardModelName = "Surfboard",
					price = 65000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = string.Empty,
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = false
				}
			},
			{
				BoardId.theoriginal,
				new Board
				{
					name = "HOVERBOARDS_SUPERHERO",
					boardModelName = "Bamboard",
					price = 8000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = string.Empty,
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = false
				}
			},
			{
				BoardId.starboard,
				new Board
				{
					name = "HOVERBOARDS_STARBOARD",
					boardModelName = "Starboard",
					price = 0,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = string.Empty,
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = false
				}
			},
			{
				BoardId.miami,
				new Board
				{
					name = "HOVERBOARDS_MIAMI",
					boardModelName = "Miami",
					price = 12000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = string.Empty,
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = false
				}
			},
			{
				BoardId.monster,
				new Board
				{
					name = "HOVERBOARDS_MONSTER",
					boardModelName = "Monster",
					price = 30000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = string.Empty,
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = false
				}
			},
			{
				BoardId.skullfire,
				new Board
				{
					name = "HOVERBOARDS_SKULL_FIRE",
					boardModelName = "SkullFire",
					price = 75000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = string.Empty,
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = false
				}
			},
			{
				BoardId.liberty,
				new Board
				{
					name = "HOVERBOARDS_LIBERTY",
					boardModelName = "Liberty",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_NEW_YORK",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 7, 5),
					defaultExpirationTime = new DateTime(2018, 8, 30)
				}
			},
			{
				BoardId.speedboard,
				new Board
				{
					name = "HOVERBOARDS_DAREDEVIL",
					boardModelName = "Daredevil",
					price = 85000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = "HOVERBOARD_ABILITY_SPEED_UP_DESCRIPTION",
					limitedDescription = string.Empty,
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = false
				}
			},
			{
				BoardId.toucan,
				new Board
				{
					name = "HOVERBOARDS_TOUCAN",
					boardModelName = "RioBoard",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_RIO",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2013, 1, 28),
					defaultExpirationTime = new DateTime(2013, 2, 28)
				}
			},
			{
				BoardId.treehugger,
				new Board
				{
					name = "HOVERBOARDS_LUMBERJACK",
					boardModelName = "Treehugger",
					price = 4000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = string.Empty,
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = false
				}
			},
			{
				BoardId.chick,
				new Board
				{
					name = "HOVERBOARDS_CHICKY",
					boardModelName = "Chick",
					tokenSprite2dName = "icon_hunt_item_big",
					price = 50000,
					unlockType = PurchaseData.UnlockType.WeeklyHunt,
					description = string.Empty,
					limitedDescription = string.Empty,
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 4, 18),
					defaultExpirationTime = new DateTime(2019, 3, 21)
				}
			},
			{
				BoardId.rome,
				new Board
				{
					name = "HOVERBOARDS_SCOOT",
					boardModelName = "Rome",
					price = 35000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = string.Empty,
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = false
				}
			},
			{
				BoardId.soccer,
				new Board
				{
					name = "HOVERBOARDS_KICK_OFF",
					boardModelName = "Soccer",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_ROME",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2014, 3, 20),
					defaultExpirationTime = new DateTime(2014, 5, 8)
				}
			},
			{
				BoardId.theOutback,
				new Board
				{
					name = "HOVERBOARDS_OUTBACK",
					boardModelName = "TheOutback",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_SYDNEY",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2013, 3, 20),
					defaultExpirationTime = new DateTime(2013, 5, 2)
				}
			},
			{
				BoardId.greatWhiteWakeboard,
				new Board
				{
					name = "HOVERBOARDS_GREAT_WHITE",
					boardModelName = "GreatWhiteWakeboard",
					price = 20000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = string.Empty,
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = false
				}
			},
			{
				BoardId.cherry,
				new Board
				{
					name = "HOVERBOARDS_CHERRY",
					boardModelName = "Cherry",
					tokenSprite2dName = "icon_doll_medium",
					price = 60,
					unlockType = PurchaseData.UnlockType.WeeklyHunt,
					description = string.Empty,
					limitedDescription = "HUNT_SPECIAL_TAG",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2013, 5, 23),
					defaultExpirationTime = new DateTime(2013, 5, 30)
				}
			},
			{
				BoardId.kitty,
				new Board
				{
					name = "HOVERBOARDS_KITTY",
					boardModelName = "Fortune",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_TOKYO",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 1, 31),
					defaultExpirationTime = new DateTime(2018, 3, 22)
				}
			},
			{
				BoardId.hotrod,
				new Board
				{
					name = "HOVERBOARDS_HOTROD",
					boardModelName = "HotRod",
					price = 280000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = "HOVERBOARD_ABILITY_SPEED_UP_DESCRIPTION",
					limitedDescription = string.Empty,
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = false
				}
			},
			{
				BoardId.flamingo,
				new Board
				{
					name = "HOVERBOARDS_FLAMINGO",
					boardModelName = "Flamingo",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_MIAMI",
					isNewInThisUpdate = true,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 5, 29),
					defaultExpirationTime = new DateTime(2019, 7, 18)
				}
			},
			{
				BoardId.rose,
				new Board
				{
					name = "HOVERBOARDS_ROSE",
					boardModelName = "Rose",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_PARIS",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 1, 14),
					defaultExpirationTime = new DateTime(2019, 1, 30)
				}
			},
			{
				BoardId.panda,
				new Board
				{
					name = "HOVERBOARDS_PANDA",
					boardModelName = "Panda",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_BEIJING",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2014, 7, 8),
					defaultExpirationTime = new DateTime(2014, 9, 18)
				}
			},
			{
				BoardId.dragon,
				new Board
				{
					name = "HOVERBOARDS_DRAGON",
					boardModelName = "Dragon",
					tokenSprite2dName = "icon_hunt_item_big",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "SPECIAL_TAG_MOON_FESTIVAL",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 2, 4),
					defaultExpirationTime = new DateTime(2019, 1, 3)
				}
			},
			{
				BoardId.moscowBoard01,
				new Board
				{
					name = "HOVERBOARDS_MOSCOWBOARD01",
					boardModelName = "Teddy",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_MOSCOW",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2013, 9, 4),
					defaultExpirationTime = new DateTime(2013, 10, 10)
				}
			},
			{
				BoardId.halloweenBoard01,
				new Board
				{
					name = "HOVERBOARDS_HALLOWEENBOARD01",
					boardModelName = "Pumpkin",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_NEW_ORLEANS",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 9, 13),
					defaultExpirationTime = new DateTime(2018, 11, 1),
					overrideDynamicIapIdiOS = true,
					dynamicIapId = "com.kiloo.subwayboard.halloweenBoard01v2"
				}
			},
			{
				BoardId.londonBoard01,
				new Board
				{
					name = "HOVERBOARDS_LONDONBOARD01",
					boardModelName = "londonBoard01",
					tokenSprite2dName = "icon_doll_medium",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_LONDON",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 11, 2),
					defaultExpirationTime = new DateTime(2018, 12, 12)
				}
			},
			{
				BoardId.londonBoard02,
				new Board
				{
					name = "HOVERBOARDS_LONDONBOARD02",
					boardModelName = "londonBoard02",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_LONDON",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 11, 2),
					defaultExpirationTime = new DateTime(2018, 12, 12)
				}
			},
			{
				BoardId.mumbaiBoard01,
				new Board
				{
					name = "HOVERBOARDS_MUMBAIBOARD01",
					boardModelName = "mumbaiBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_MUMBAI",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 5, 28),
					defaultExpirationTime = new DateTime(2018, 7, 12)
				}
			},
			{
				BoardId.teleboard,
				new Board
				{
					name = "HOVERBOARDS_TELEBOARD",
					boardModelName = "teleboard",
					price = 220000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = "HOVERBOARD_ABILITY_ZAP_SIDEWAYS_DESCRIPTION",
					limitedDescription = string.Empty,
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = false
				}
			},
			{
				BoardId.seoulBoard01,
				new Board
				{
					name = "HOVERBOARDS_SEOULBOARD01",
					boardModelName = "seoulBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_SEOUL",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 4, 9),
					defaultExpirationTime = new DateTime(2019, 5, 7)
				}
			},
			{
				BoardId.mexicoBoard01,
				new Board
				{
					name = "HOVERBOARDS_MEXICOBOARD01",
					boardModelName = "mexicoBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_MEXICO_CITY",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2014, 3, 20),
					defaultExpirationTime = new DateTime(2014, 4, 16)
				}
			},
			{
				BoardId.vancouverBoard01,
				new Board
				{
					name = "HOVERBOARDS_VANCOUVERBOARD01",
					boardModelName = "vancouverBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_VANCOUVER",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2014, 4, 1),
					defaultExpirationTime = new DateTime(2014, 5, 28)
				}
			},
			{
				BoardId.gliderboard,
				new Board
				{
					name = "HOVERBOARDS_GLIDER",
					boardModelName = "gliderboard",
					price = 360000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = "HOVERBOARD_ABILITY_SMOOTH_DRIFT_DESCRIPTION",
					limitedDescription = string.Empty,
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = false
				}
			},
			{
				BoardId.saopauloBoard01,
				new Board
				{
					name = "HOVERBOARDS_SAOPAULOBOARD01",
					boardModelName = "saopauloBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_SAOPAULO",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2014, 5, 2),
					defaultExpirationTime = new DateTime(2014, 7, 10)
				}
			},
			{
				BoardId.losangelesBoard01,
				new Board
				{
					name = "HOVERBOARDS_LOSANGELESBOARD01",
					boardModelName = "losangelesBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.InApp,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_LOSANGELES",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 5, 8),
					defaultExpirationTime = new DateTime(2019, 5, 28)
				}
			},
			{
				BoardId.cairoBoard01,
				new Board
				{
					name = "HOVERBOARDS_CAIROBOARD01",
					boardModelName = "cairoBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_CAIRO",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 3, 22),
					defaultExpirationTime = new DateTime(2019, 4, 8)
				}
			},
			{
				BoardId.bangkokBoard01,
				new Board
				{
					name = "HOVERBOARDS_BANGKOKBOARD01",
					boardModelName = "bangkokBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_BANGKOK",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 3, 22),
					defaultExpirationTime = new DateTime(2019, 4, 8)
				}
			},
			{
				BoardId.londonBoard03,
				new Board
				{
					name = "HOVERBOARDS_LONDONBOARD03",
					boardModelName = "londonBoard03",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_LONDON",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 11, 2),
					defaultExpirationTime = new DateTime(2018, 12, 12)
				}
			},
			{
				BoardId.lasVegasBoard01,
				new Board
				{
					name = "HOVERBOARDS_LAS_VEGAS_BOARD01",
					boardModelName = "lasVegasBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_LAS_VEGAS",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 5, 8),
					defaultExpirationTime = new DateTime(2019, 5, 28)
				}
			},
			{
				BoardId.hawaiiBoard01,
				new Board
				{
					name = "HOVERBOARDS_HAWAII_BOARD01",
					boardModelName = "hawaiiBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_HAWAII",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2015, 11, 26),
					defaultExpirationTime = new DateTime(2016, 1, 21)
				}
			},
			{
				BoardId.arabiaBoard01,
				new Board
				{
					name = "HOVERBOARDS_ARABIA_BOARD01",
					boardModelName = "arabiaBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_ARABIA",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2017, 1, 16),
					defaultExpirationTime = new DateTime(2017, 3, 2)
				}
			},
			{
				BoardId.veniceBoard01,
				new Board
				{
					name = "HOVERBOARDS_VENICE_BOARD01",
					boardModelName = "veniceBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_VENICE",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 1, 14),
					defaultExpirationTime = new DateTime(2019, 1, 30)
				}
			},
			{
				BoardId.rioBoard01,
				new Board
				{
					name = "HOVERBOARDS_RIO_BOARD01",
					boardModelName = "rioBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_RIO",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 11, 27),
					defaultExpirationTime = new DateTime(2018, 12, 12)
				}
			},
			{
				BoardId.sydneyBoard01,
				new Board
				{
					name = "HOVERBOARDS_SYDNEY_BOARD01",
					boardModelName = "sydneyBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_SYDNEY",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2015, 6, 24),
					defaultExpirationTime = new DateTime(2015, 8, 13)
				}
			},
			{
				BoardId.greeceBoard01,
				new Board
				{
					name = "HOVERBOARDS_GREECE_BOARD01",
					boardModelName = "greeceBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_GREECE",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2015, 7, 15),
					defaultExpirationTime = new DateTime(2015, 9, 3)
				}
			},
			{
				BoardId.kenyaBoard01,
				new Board
				{
					name = "HOVERBOARDS_KENYA_BOARD01",
					boardModelName = "kenyaBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_KENYA",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2015, 9, 3),
					defaultExpirationTime = new DateTime(2015, 10, 15)
				}
			},
			{
				BoardId.transylvaniaBoard01,
				new Board
				{
					name = "HOVERBOARDS_TRANSYLVANIA_BOARD01",
					boardModelName = "transylvaniaBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_TRANSYLVANIA",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 9, 13),
					defaultExpirationTime = new DateTime(2018, 11, 1)
				}
			},
			{
				BoardId.northpoleBoard01,
				new Board
				{
					name = "HOVERBOARDS_NORTHPOLE_BOARD01",
					boardModelName = "northpoleBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_HOLIDAY",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 11, 2),
					defaultExpirationTime = new DateTime(2018, 12, 12)
				}
			},
			{
				BoardId.sanfranciscoBoard01,
				new Board
				{
					name = "HOVERBOARDS_SANFRANCISCO_BOARD01",
					boardModelName = "sanfranciscoBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_SAN_FRANCISCO",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 1, 31),
					defaultExpirationTime = new DateTime(2019, 2, 7)
				}
			},
			{
				BoardId.pragueBoard01,
				new Board
				{
					name = "HOVERBOARDS_PRAGUE_BOARD01",
					boardModelName = "pragueBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_PRAGUE",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2016, 1, 22),
					defaultExpirationTime = new DateTime(2016, 3, 31)
				}
			},
			{
				BoardId.sydneyBoard02,
				new Board
				{
					name = "HOVERBOARDS_SYDNEY_BOARD02",
					boardModelName = "sydneyBoard02",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_SYDNEY",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 3, 22),
					defaultExpirationTime = new DateTime(2019, 4, 8)
				}
			},
			{
				BoardId.madagascarBoard01,
				new Board
				{
					name = "HOVERBOARDS_MADAGASCAR_BOARD01",
					boardModelName = "madagascarBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_MADAGASCAR",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 1, 31),
					defaultExpirationTime = new DateTime(2019, 2, 7)
				}
			},
			{
				BoardId.peruBoard01,
				new Board
				{
					name = "HOVERBOARDS_PERU_BOARD01",
					boardModelName = "peruBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_PERU",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2017, 5, 4),
					defaultExpirationTime = new DateTime(2017, 6, 22)
				}
			},
			{
				BoardId.singaporeBoard01,
				new Board
				{
					name = "HOVERBOARDS_SINGAPORE_BOARD01",
					boardModelName = "singaporeBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_SINGAPORE",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2017, 9, 4),
					defaultExpirationTime = new DateTime(2017, 10, 12)
				}
			},
			{
				BoardId.veniceBoard02,
				new Board
				{
					name = "HOVERBOARDS_VENICE_BOARD02",
					boardModelName = "veniceBoard02",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_VENICE",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 1, 14),
					defaultExpirationTime = new DateTime(2019, 1, 30)
				}
			},
			{
				BoardId.rioBoard02,
				new Board
				{
					name = "HOVERBOARDS_RIO_BOARD02",
					boardModelName = "rioBoard02",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_RIO",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 12, 13),
					defaultExpirationTime = new DateTime(2019, 1, 3)
				}
			},
			{
				BoardId.icelandBoard01,
				new Board
				{
					name = "HOVERBOARDS_ICELAND_BOARD01",
					boardModelName = "icelandBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_ICELAND",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 2, 21),
					defaultExpirationTime = new DateTime(2018, 4, 12)
				}
			},
			{
				BoardId.havanaBoard01,
				new Board
				{
					name = "HOVERBOARDS_HAVANA_BOARD01",
					boardModelName = "havanaBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_HAVANA",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 1, 31),
					defaultExpirationTime = new DateTime(2019, 2, 7)
				}
			},
			{
				BoardId.halloweenBoard02,
				new Board
				{
					name = "HOVERBOARDS_HALLOWEEN_BOARD02",
					boardModelName = "halloweenBoard02",
					unlockType = PurchaseData.UnlockType.Coins,
					price = 50000,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_HALLOWEEN",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 9, 13),
					defaultExpirationTime = new DateTime(2018, 11, 1)
				}
			},
			{
				BoardId.washingtonBoard01,
				new Board
				{
					name = "HOVERBOARDS_WASHINGTON_BOARD01",
					boardModelName = "washingtonBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_WASHINGTON",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultExpirationTime = new DateTime(2017, 11, 30),
					defaultStartDate = new DateTime(2017, 10, 8)
				}
			},
			{
				BoardId.northpoleBoard02,
				new Board
				{
					name = "HOVERBOARDS_NORTHPOLE_BOARD02",
					boardModelName = "northpoleBoard02",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_WINTER_HOLIDAY",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 11, 2),
					defaultExpirationTime = new DateTime(2018, 12, 12)
				}
			},
			{
				BoardId.amsterdamBoard01,
				new Board
				{
					name = "HOVERBOARDS_AMSTERDAM_BOARD01",
					boardModelName = "amsterdamBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_AMSTERDAM",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 3, 3),
					defaultExpirationTime = new DateTime(2019, 3, 21)
				}
			},
			{
				BoardId.arabiaBoard02,
				new Board
				{
					name = "HOVERBOARDS_ARABIA_BOARD02",
					boardModelName = "arabiaBoard02",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_ARABIA",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2017, 1, 5),
					defaultExpirationTime = new DateTime(2017, 3, 2)
				}
			},
			{
				BoardId.monacoBoard01,
				new Board
				{
					name = "HOVERBOARDS_MONACO_BOARD01",
					boardModelName = "monacoBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = "HOVERBOARD_ABILITY_SPEED_UP_DESCRIPTION",
					limitedDescription = "UPDATE_SPECIAL_TAG_MONACO",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 5, 8),
					defaultExpirationTime = new DateTime(2019, 5, 28)
				}
			},
			{
				BoardId.copenhagenBoard01,
				new Board
				{
					name = "HOVERBOARDS_COPENHAGEN_BOARD01",
					boardModelName = "copenhagenBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_COPENHAGEN",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2017, 4, 6),
					defaultExpirationTime = new DateTime(2017, 6, 1)
				}
			},
			{
				BoardId.hawaiiBoard02,
				new Board
				{
					name = "HOVERBOARDS_HAWAII_BOARD02",
					boardModelName = "hawaiiBoard02",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_HAWAII",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2017, 3, 29),
					defaultExpirationTime = new DateTime(2017, 5, 11)
				}
			},
			{
				BoardId.birthdayBoard01,
				new Board
				{
					name = "HOVERBOARDS_BIRTHDAY_BOARD01",
					boardModelName = "birthdayBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.WatchVideo,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_ANNIVERSARY",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 4, 9),
					defaultExpirationTime = new DateTime(2019, 5, 7)
				}
			},
			{
				BoardId.marrakeshBoard01,
				new Board
				{
					name = "HOVERBOARDS_MARRAKESH_BOARD01",
					boardModelName = "marrakeshBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_MARRAKESH",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 10, 8),
					defaultExpirationTime = new DateTime(2018, 11, 29)
				}
			},
			{
				BoardId.shanghaiBoard01,
				new Board
				{
					name = "HOVERBOARDS_SHANGHAI_BOARD01",
					boardModelName = "shanghaiBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_SHANGHAI",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 2, 4),
					defaultExpirationTime = new DateTime(2019, 1, 3)
				}
			},
			{
				BoardId.barcelonaBoard01,
				new Board
				{
					name = "HOVERBOARDS_BARCELONA_BOARD01",
					boardModelName = "barcelonaBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_BARCELONA",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2017, 8, 31),
					defaultExpirationTime = new DateTime(2017, 9, 21)
				}
			},
			{
				BoardId.mexicoBoard02,
				new Board
				{
					name = "HOVERBOARDS_MEXICO_BOARD02",
					boardModelName = "mexicoBoard02",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_MEXICO",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 9, 13),
					defaultExpirationTime = new DateTime(2018, 11, 1)
				}
			},
			{
				BoardId.stPetersburgBoard01,
				new Board
				{
					name = "HOVERBOARDS_ST_PETERSBURG_BOARD01",
					boardModelName = "stPetersburgBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_ST_PETERSBURG",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 11, 2),
					defaultExpirationTime = new DateTime(2018, 12, 12)
				}
			},
			{
				BoardId.cairoBoard02,
				new Board
				{
					name = "HOVERBOARDS_CAIRO_BOARD02",
					boardModelName = "cairoBoard02",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_CAIRO",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2017, 11, 22),
					defaultExpirationTime = new DateTime(2018, 1, 18)
				}
			},
			{
				BoardId.chicagoBoard01,
				new Board
				{
					name = "HOVERBOARDS_CHICAGO_BOARD01",
					boardModelName = "chicagoBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_CHICAGO",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 12, 13),
					defaultExpirationTime = new DateTime(2019, 1, 3)
				}
			},
			{
				BoardId.parisBoard01,
				new Board
				{
					name = "HOVERBOARDS_PARIS_BOARD01",
					boardModelName = "parisBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "SPECIAL_TAG_VALENTINES",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 1, 14),
					defaultExpirationTime = new DateTime(2019, 1, 30)
				}
			},
			{
				BoardId.buenosAiresBoard01,
				new Board
				{
					name = "HOVERBOARDS_BUENOS_AIRES_BOARD01",
					boardModelName = "buenosAiresBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_BUENOS_AIRES",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 3, 14),
					defaultExpirationTime = new DateTime(2018, 5, 3)
				}
			},
			{
				BoardId.veniceBeachBoard01,
				new Board
				{
					name = "HOVERBOARDS_VENICE_BEACH_BOARD01",
					boardModelName = "veniceBeachBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_VENICE_BEACH",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 5, 1),
					defaultExpirationTime = new DateTime(2018, 6, 21)
				}
			},
			{
				BoardId.starterPackBoard01,
				new Board
				{
					name = "HOVERBOARDS_STARTER_PACK_BOARD01",
					boardModelName = "starterPackBoard01",
					price = 0,
					unlockType = PurchaseData.UnlockType.Redirect,
					description = string.Empty,
					limitedDescription = "SPECIAL_TAG_STARTER_BOARD",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = false
				}
			},
			{
				BoardId.mumbaiBoard02,
				new Board
				{
					name = "HOVERBOARDS_MUMBAI_BOARD02",
					boardModelName = "mumbaiBoard02",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_COLOR_BLAST",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 5, 28),
					defaultExpirationTime = new DateTime(2018, 7, 12)
				}
			},
			{
				BoardId.newYorkBoard01,
				new Board
				{
					name = "HOVERBOARDS_NEWYORK_BOARD01",
					boardModelName = "newYorkBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_NEW_YORK",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 7, 5),
					defaultExpirationTime = new DateTime(2018, 8, 30)
				}
			},
			{
				BoardId.berlinBoard01,
				new Board
				{
					name = "HOVERBOARDS_BERLIN_BOARD01",
					boardModelName = "berlinBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_BERLIN",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 8, 8),
					defaultExpirationTime = new DateTime(2018, 9, 20)
				}
			},
			{
				BoardId.hongKongBoard01,
				new Board
				{
					name = "HOVERBOARDS_HONG_KONG_BOARD01",
					boardModelName = "hongKongBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_HONG_KONG",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 8, 8),
					defaultExpirationTime = new DateTime(2018, 10, 11)
				}
			},
			{
				BoardId.marrakeshBoard02,
				new Board
				{
					name = "HOVERBOARDS_MARRAKESH_BOARD02",
					boardModelName = "marrakeshBoard02",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_MARRAKESH",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 10, 8),
					defaultExpirationTime = new DateTime(2018, 11, 29)
				}
			},
			{
				BoardId.rioBoard03,
				new Board
				{
					name = "HOVERBOARDS_RIO_BOARD03",
					boardModelName = "rioBoard03",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_NEW_YEAR",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 11, 27),
					defaultExpirationTime = new DateTime(2018, 12, 12)
				}
			},
			{
				BoardId.atlantaBoard01,
				new Board
				{
					name = "HOVERBOARDS_ATLANTA_BOARD01",
					boardModelName = "atlantaBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_ATLANTA",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2018, 12, 13),
					defaultExpirationTime = new DateTime(2019, 1, 3)
				}
			},
			{
				BoardId.harbinBoard01,
				new Board
				{
					name = "HOVERBOARDS_HARBIN_BOARD01",
					boardModelName = "harbinBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_HARBIN",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 2, 4),
					defaultExpirationTime = new DateTime(2019, 1, 3)
				}
			},
			{
				BoardId.sanfranciscoBoard02,
				new Board
				{
					name = "HOVERBOARDS_SANFRANCISCO_BOARD02",
					boardModelName = "sanfranciscoBoard02",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_GAME_EXPO",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 1, 31),
					defaultExpirationTime = new DateTime(2019, 2, 7)
				}
			},
			{
				BoardId.zurichBoard01,
				new Board
				{
					name = "HOVERBOARDS_ZURICH_BOARD01",
					boardModelName = "zurichBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_ZURICH",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 3, 3),
					defaultExpirationTime = new DateTime(2019, 3, 21)
				}
			},
			{
				BoardId.dubaiBoard01,
				new Board
				{
					name = "HOVERBOARDS_DUBAI_BOARD01",
					boardModelName = "dubaiBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_DUBAI",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 5, 8),
					defaultExpirationTime = new DateTime(2019, 5, 28)
				}
			},
			{
				BoardId.macauBoard01,
				new Board
				{
					name = "HOVERBOARDS_MACAU_BOARD01",
					boardModelName = "macauBoard01",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_EVENT",
					isNewInThisUpdate = false,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 5, 8),
					defaultExpirationTime = new DateTime(2019, 5, 28)
				}
			},
			{
				BoardId.julyBoard,
				new Board
				{
					name = "HOVERBOARDS_JULY_BOARD",
					boardModelName = "julyBoard",
					price = 50000,
					unlockType = PurchaseData.UnlockType.Coins,
					description = string.Empty,
					limitedDescription = "UPDATE_SPECIAL_TAG_JULY",
					isNewInThisUpdate = true,
					hasOnlineTimeLimit = true,
					defaultStartDate = new DateTime(2019, 5, 29),
					defaultExpirationTime = new DateTime(2019, 7, 18),
					isPartOfBundle = true
				}
			}
		};

		public static List<BoardId> boardOrder = new List<BoardId>
		{
			BoardId.snowboard,
			BoardId.bouncer,
			BoardId.hotrod,
			BoardId.teleboard,
			BoardId.skullfire,
			BoardId.starterPackBoard01,
			BoardId.normal,
			BoardId.julyBoard,
			BoardId.flamingo,
			BoardId.washingtonBoard01,
			BoardId.dubaiBoard01,
			BoardId.macauBoard01,
			BoardId.monacoBoard01,
			BoardId.lasVegasBoard01,
			BoardId.losangelesBoard01,
			BoardId.seoulBoard01,
			BoardId.birthdayBoard01,
			BoardId.bangkokBoard01,
			BoardId.cairoBoard01,
			BoardId.sydneyBoard02,
			BoardId.chick,
			BoardId.zurichBoard01,
			BoardId.amsterdamBoard01,
			BoardId.sanfranciscoBoard02,
			BoardId.sanfranciscoBoard01,
			BoardId.madagascarBoard01,
			BoardId.havanaBoard01,
			BoardId.parisBoard01,
			BoardId.veniceBoard01,
			BoardId.veniceBoard02,
			BoardId.atlantaBoard01,
			BoardId.rose,
			BoardId.harbinBoard01,
			BoardId.shanghaiBoard01,
			BoardId.dragon,
			BoardId.chicagoBoard01,
			BoardId.rioBoard02,
			BoardId.rioBoard03,
			BoardId.rioBoard01,
			BoardId.northpoleBoard01,
			BoardId.londonBoard01,
			BoardId.northpoleBoard02,
			BoardId.londonBoard03,
			BoardId.londonBoard02,
			BoardId.stPetersburgBoard01,
			BoardId.marrakeshBoard02,
			BoardId.marrakeshBoard01,
			BoardId.halloweenBoard02,
			BoardId.mexicoBoard02,
			BoardId.transylvaniaBoard01,
			BoardId.halloweenBoard01,
			BoardId.hongKongBoard01,
			BoardId.berlinBoard01,
			BoardId.newYorkBoard01,
			BoardId.liberty,
			BoardId.mumbaiBoard02,
			BoardId.mumbaiBoard01,
			BoardId.veniceBeachBoard01,
			BoardId.buenosAiresBoard01,
			BoardId.icelandBoard01,
			BoardId.kitty,
			BoardId.cairoBoard02,
			BoardId.singaporeBoard01,
			BoardId.barcelonaBoard01,
			BoardId.peruBoard01,
			BoardId.copenhagenBoard01,
			BoardId.hawaiiBoard02,
			BoardId.arabiaBoard02,
			BoardId.arabiaBoard01,
			BoardId.pragueBoard01,
			BoardId.hawaiiBoard01,
			BoardId.kenyaBoard01,
			BoardId.greeceBoard01,
			BoardId.sydneyBoard01,
			BoardId.saopauloBoard01,
			BoardId.vancouverBoard01,
			BoardId.mexicoBoard01,
			BoardId.moscowBoard01,
			BoardId.panda,
			BoardId.toucan,
			BoardId.soccer,
			BoardId.cherry,
			BoardId.theOutback,
			BoardId.lowrider,
			BoardId.starboard,
			BoardId.speedboard,
			BoardId.greatWhiteWakeboard,
			BoardId.gliderboard,
			BoardId.rome,
			BoardId.treehugger,
			BoardId.theoriginal,
			BoardId.surfboard,
			BoardId.miami,
			BoardId.monster
		};

		public static string[] GetAllBoardNames()
		{
			return Enum.GetNames(typeof(BoardId));
		}
	}
}
