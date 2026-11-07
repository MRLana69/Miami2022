using System;
using System.Collections.Generic;
using System.Linq;
using Info;
using UnityEngine;

public class HoverboardThemes
{
	public class HoverboardTheme
	{
		public int price;

		public PurchaseData.UnlockType unlockType;

		public Color32 buttonBgColor;

		public Color32 buttonIconColor;

		public bool isNewInThisUpdate;

		public string title;

		public UIOrder uiOrder;

		public Hoverboard.BoardAbility power;

		public ThemeIdentifier identifier;

		public string iconName;

		public string GetAnalyticsReadablePowerName()
		{
			switch (power)
			{
			case Hoverboard.BoardAbility.Jumper:
				return "Super jump";
			case Hoverboard.BoardAbility.Lowrider:
				return "Stay low";
			case Hoverboard.BoardAbility.HighSpeed:
			case Hoverboard.BoardAbility.Speed:
				return "Speed up";
			case Hoverboard.BoardAbility.Teleport:
				return "Zap sideways";
			case Hoverboard.BoardAbility.Glider:
				return "Smooth Drift";
			case Hoverboard.BoardAbility.Vanity:
				return "Trail";
			default:
				return power.ToString();
			}
		}
	}

	public enum UIOrder
	{
		First = 0,
		Second = 1
	}

	public enum ThemeIdentifier
	{
		none = 0,
		heroboardTheme01 = 1,
		heroboardTheme02 = 2,
		starboardTheme01 = 3,
		starboardTheme02 = 4,
		greatWhiteTheme01 = 5,
		greatWhiteTheme02 = 6,
		surfboardTheme01 = 7,
		surfboardTheme02 = 8,
		monsterTheme1 = 9,
		monsterTheme2 = 10,
		snowboardTheme1 = 11,
		snowboardTheme2 = 12,
		miamiTheme1 = 13,
		miamiTheme2 = 14,
		skullfireTheme1 = 15,
		romeTheme01 = 16,
		skullfireTheme02 = 17,
		lumberjackTheme01 = 18,
		lumberjackTheme02 = 19,
		romeTheme02 = 20,
		londonBoard01Theme01 = 21,
		londonBoard01Theme02 = 22,
		chicky01Theme01 = 23,
		chicky01Theme02 = 24,
		veniceBoard02Theme01 = 25,
		transylvaniaBoard01Theme01 = 26,
		sanfranciscoBoard01Theme01 = 27,
		arabiaBoard01Theme01 = 28,
		bangkokBoard01Theme01 = 29,
		cairoBoard02Theme01 = 30,
		cairoBoard02Theme02 = 31,
		starterPackBoard01Theme01 = 32,
		starterPackBoard01Theme02 = 33,
		mumbaiBoard02Theme01 = 34,
		mumbaiBoard02Theme02 = 35,
		hongKongBoard01Theme01 = 36,
		marrakeshBoard02Theme01 = 37,
		marrakeshBoard02Theme02 = 38,
		rioBoard03Theme01 = 39,
		rioBoard03Theme02 = 40,
		julyBoardTheme01 = 41,
		julyBoardTheme02 = 42
	}

	public static readonly Dictionary<Boards.BoardId, List<HoverboardTheme>> hoverboardCustomThemes = new Dictionary<Boards.BoardId, List<HoverboardTheme>>
	{
		{
			Boards.BoardId.theoriginal,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 40,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_HERO_TELEPORT_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_HERO_TELEPORT_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_ZAP_SIDEWAYS_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.heroboardTheme01,
					power = Hoverboard.BoardAbility.Teleport,
					iconName = "icon_upgrade_zap_sideways"
				},
				new HoverboardTheme
				{
					price = 55,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_HERO_SUPER_JUMP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_HERO_SUPER_JUMP_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SUPER_JUMP_DESCRIPTION",
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.heroboardTheme02,
					power = Hoverboard.BoardAbility.Jumper,
					iconName = "icon_upgrade_super_jump"
				}
			}
		},
		{
			Boards.BoardId.starboard,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 0,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_STARBOARD_TRAIL_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_STARBOARD_TRAIL_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_STAR_TRAIL_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.starboardTheme01,
					power = Hoverboard.BoardAbility.Vanity,
					iconName = "icon_upgrade_star_trail"
				},
				new HoverboardTheme
				{
					price = 45,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_STARBOARD_DOUBLE_JUMP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_STARBOARD_DOUBLE_JUMP_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_DOUBLE_JUMP_DESCRIPTION",
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.heroboardTheme02,
					power = Hoverboard.BoardAbility.DoubleJump,
					iconName = "icon_upgrade_double_jump"
				}
			}
		},
		{
			Boards.BoardId.greatWhiteWakeboard,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 50,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_GREAT_WHITE_SPEEDUP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_GREAT_WHITE_SPEEDUP_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SPEED_UP_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.greatWhiteTheme01,
					power = Hoverboard.BoardAbility.HighSpeed,
					iconName = "icon_upgrade_speed_up"
				},
				new HoverboardTheme
				{
					price = 60,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_GREAT_WHITE_GLIDER_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_GREAT_WHITE_GLIDER_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SMOOTH_DRIFT_DESCRIPTION",
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.greatWhiteTheme02,
					power = Hoverboard.BoardAbility.Glider,
					iconName = "icon_upgrade_smooth_drift"
				}
			}
		},
		{
			Boards.BoardId.surfboard,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 55,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_SURF_BOARD_LOWRIDER_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_SURF_BOARD_LOWRIDER_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_STAY_LOW_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.surfboardTheme01,
					power = Hoverboard.BoardAbility.Lowrider,
					iconName = "icon_upgrade_stay_low"
				},
				new HoverboardTheme
				{
					price = 60,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_SURF_BOARD_GLIDER_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_SURF_BOARD_GLIDER_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SMOOTH_DRIFT_DESCRIPTION",
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.surfboardTheme02,
					power = Hoverboard.BoardAbility.Glider,
					iconName = "icon_upgrade_smooth_drift"
				}
			}
		},
		{
			Boards.BoardId.monster,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 50,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_MONSTER_SPEEDUP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_MONSTER_SPEEDUP_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SPEED_UP_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.monsterTheme1,
					power = Hoverboard.BoardAbility.HighSpeed,
					iconName = "icon_upgrade_speed_up"
				},
				new HoverboardTheme
				{
					price = 55,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_MONSTER_JUMPER_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_MONSTER_JUMPER_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SUPER_JUMP_DESCRIPTION",
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.monsterTheme2,
					power = Hoverboard.BoardAbility.Jumper,
					iconName = "icon_upgrade_super_jump"
				}
			}
		},
		{
			Boards.BoardId.snowboard,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 45,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_FREESTYLER_DOUBLE_JUMP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_FREESTYLER_DOUBLE_JUMP_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_DOUBLE_JUMP_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.snowboardTheme1,
					power = Hoverboard.BoardAbility.DoubleJump,
					iconName = "icon_upgrade_double_jump"
				},
				new HoverboardTheme
				{
					price = 55,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_FREESTYLER_SUPER_JUMP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_FREESTYLER_SUPER_JUMP_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SUPER_JUMP_DESCRIPTION",
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.snowboardTheme2,
					power = Hoverboard.BoardAbility.Jumper,
					iconName = "icon_upgrade_super_jump"
				}
			}
		},
		{
			Boards.BoardId.miami,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 30,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_MIAMI_PINK_TRAIL_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_MIAMI_PINK_TRAIL_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_PINK_TRAIL_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.miamiTheme1,
					power = Hoverboard.BoardAbility.Vanity,
					iconName = "icon_upgrade_pink_trail"
				},
				new HoverboardTheme
				{
					price = 60,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_MIAMI_SMOOTHDRIFT_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_MIAMI_SMOOTHDRIFT_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SMOOTH_DRIFT_DESCRIPTION",
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.miamiTheme2,
					power = Hoverboard.BoardAbility.Glider,
					iconName = "icon_upgrade_smooth_drift"
				}
			}
		},
		{
			Boards.BoardId.skullfire,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 45,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_SKULLFIRE_DOUBLE_JUMP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_SKULLFIRE_DOUBLE_JUMP_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_DOUBLE_JUMP_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.skullfireTheme1,
					power = Hoverboard.BoardAbility.DoubleJump,
					iconName = "icon_upgrade_double_jump"
				},
				new HoverboardTheme
				{
					price = 50,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_SKULLFIRE_SPEEDUP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_SKULLFIRE_SPEEDUP_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SPEED_UP_DESCRIPTION",
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.skullfireTheme02,
					power = Hoverboard.BoardAbility.HighSpeed,
					iconName = "icon_upgrade_speed_up"
				}
			}
		},
		{
			Boards.BoardId.rome,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 40,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_ROME_ZAPSIDEWAYS_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_ROME_ZAPSIDEWAYS_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_ZAP_SIDEWAYS_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.romeTheme01,
					power = Hoverboard.BoardAbility.Teleport,
					iconName = "icon_upgrade_zap_sideways"
				},
				new HoverboardTheme
				{
					price = 50,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_ROME_SPEEDUP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_ROME_SPEEDUP_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SPEED_UP_DESCRIPTION",
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.romeTheme02,
					power = Hoverboard.BoardAbility.HighSpeed,
					iconName = "icon_upgrade_speed_up"
				}
			}
		},
		{
			Boards.BoardId.treehugger,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 45,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_LUMBERJACK_DOUBLE_JUMP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_LUMBERJACK_DOUBLE_JUMP_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_DOUBLE_JUMP_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.lumberjackTheme01,
					power = Hoverboard.BoardAbility.DoubleJump,
					iconName = "icon_upgrade_double_jump"
				},
				new HoverboardTheme
				{
					price = 55,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_LUMBERJACK_LOWRIDER_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_LUMBERJACK_LOWRIDER_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_STAY_LOW_DESCRIPTION",
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.lumberjackTheme02,
					power = Hoverboard.BoardAbility.Lowrider,
					iconName = "icon_upgrade_stay_low"
				}
			}
		},
		{
			Boards.BoardId.londonBoard01,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 30,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_LONDONBOARD01_TRAIL_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_LONDONBOARD01_TRAIL_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SNOW_TRAIL_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.londonBoard01Theme01,
					power = Hoverboard.BoardAbility.Vanity,
					iconName = "icon_upgrade_snow_trail"
				},
				new HoverboardTheme
				{
					price = 55,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_LONDONBOARD01_SUPER_JUMP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_LONDONBOARD01_SUPER_JUMP_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SUPER_JUMP_DESCRIPTION",
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.londonBoard01Theme02,
					power = Hoverboard.BoardAbility.Jumper,
					iconName = "icon_upgrade_super_jump"
				}
			}
		},
		{
			Boards.BoardId.chick,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 30,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_CHICK_TRAIL_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_CHICK_TRAIL_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_EGG_TRAIL_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.chicky01Theme01,
					power = Hoverboard.BoardAbility.Vanity,
					iconName = "icon_feather"
				},
				new HoverboardTheme
				{
					price = 50,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_CHICK_SPEEDUP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_CHICK_SPEEDUP_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SPEED_UP_DESCRIPTION",
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.chicky01Theme02,
					power = Hoverboard.BoardAbility.HighSpeed,
					iconName = "icon_upgrade_speed_up"
				}
			}
		},
		{
			Boards.BoardId.veniceBoard02,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 60,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_VENICEBOARD02_GLIDER_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_VENICEBOARD02_GLIDER_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SMOOTH_DRIFT_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.veniceBoard02Theme01,
					power = Hoverboard.BoardAbility.Glider,
					iconName = "icon_upgrade_smooth_drift"
				}
			}
		},
		{
			Boards.BoardId.transylvaniaBoard01,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 40,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_ROME_ZAPSIDEWAYS_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_ROME_ZAPSIDEWAYS_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_ZAP_SIDEWAYS_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.transylvaniaBoard01Theme01,
					power = Hoverboard.BoardAbility.Teleport,
					iconName = "icon_upgrade_zap_sideways"
				}
			}
		},
		{
			Boards.BoardId.sanfranciscoBoard01,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 30,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_SANFRANCISCOBOARD01_TRAIL_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_SANFRANCISCOBOARD01_TRAIL_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_FLOWER_TRAIL_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.sanfranciscoBoard01Theme01,
					power = Hoverboard.BoardAbility.Vanity,
					iconName = "icon_upgrade_flower"
				}
			}
		},
		{
			Boards.BoardId.arabiaBoard01,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 30,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_ARABIABOARD01_TRAIL_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_ARABIABOARD01_TRAIL_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SMOKE_TRAIL_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.arabiaBoard01Theme01,
					power = Hoverboard.BoardAbility.Vanity,
					iconName = "icon_smoke"
				}
			}
		},
		{
			Boards.BoardId.bangkokBoard01,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 30,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_BANGKOKBOARD01_TRAIL_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_BANGKOKBOARD01_TRAIL_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_BUBBLE_TRAIL_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.bangkokBoard01Theme01,
					power = Hoverboard.BoardAbility.Vanity,
					iconName = "icon_upgrade_pink_trail"
				}
			}
		},
		{
			Boards.BoardId.cairoBoard02,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 60,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_CAIROBOARD02_GLIDER_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_CAIROBOARD02_GLIDER_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SMOOTH_DRIFT_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.cairoBoard02Theme02,
					power = Hoverboard.BoardAbility.Glider,
					iconName = "icon_upgrade_smooth_drift"
				},
				new HoverboardTheme
				{
					price = 50,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_CAIROBOARD02_SPEEDUP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_CAIROBOARD02_SPEEDUP_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SPEED_UP_DESCRIPTION",
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.cairoBoard02Theme01,
					power = Hoverboard.BoardAbility.HighSpeed,
					iconName = "icon_upgrade_speed_up"
				}
			}
		},
		{
			Boards.BoardId.mumbaiBoard02,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 50,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_MUMBAIBOARD02_SPEEDUP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_MUMBAIBOARD02_SPEEDUP_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SPEED_UP_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.mumbaiBoard02Theme01,
					power = Hoverboard.BoardAbility.Speed,
					iconName = "icon_upgrade_speed_up"
				},
				new HoverboardTheme
				{
					price = 40,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_MUMBAIBOARD02_ZAPSIDEWAYS_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_MUMBAIBOARD02_ZAPSIDEWAYS_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_COLOR_SPLASH_DESCRIPTION",
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.mumbaiBoard02Theme02,
					power = Hoverboard.BoardAbility.Teleport,
					iconName = "icon_upgrade_color_splash"
				}
			}
		},
		{
			Boards.BoardId.starterPackBoard01,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 30,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_STARTER_PACK_BOARD_01_MUSIC_TRAIL_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_STARTER_PACK_BOARD_01_MUSIC_TRAIL_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_MUSIC_TRAIL_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.starterPackBoard01Theme01,
					power = Hoverboard.BoardAbility.Vanity,
					iconName = "icon_upgrade_sound_trail"
				},
				new HoverboardTheme
				{
					price = 55,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_HERO_SUPER_JUMP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_HERO_SUPER_JUMP_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SUPER_JUMP_DESCRIPTION",
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.starterPackBoard01Theme02,
					power = Hoverboard.BoardAbility.Jumper,
					iconName = "icon_upgrade_super_jump"
				}
			}
		},
		{
			Boards.BoardId.hongKongBoard01,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 60,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_HONGKONG_GLIDE_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_HONGKONG_GLIDE_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SMOOTH_DRIFT_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.hongKongBoard01Theme01,
					power = Hoverboard.BoardAbility.Glider,
					iconName = "icon_upgrade_smooth_drift"
				}
			}
		},
		{
			Boards.BoardId.marrakeshBoard02,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 60,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_MARRAKESHBOARD02_GLIDER_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_MARRAKESHBOARD02_GLIDER_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SMOOTH_DRIFT_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.marrakeshBoard02Theme01,
					power = Hoverboard.BoardAbility.Glider,
					iconName = "icon_upgrade_smooth_drift"
				},
				new HoverboardTheme
				{
					price = 55,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_MARRAKESHBOARD02_SUPER_JUMP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_MARRAKESHBOARD02_SUPER_JUMP_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SUPER_JUMP_DESCRIPTION",
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.marrakeshBoard02Theme02,
					power = Hoverboard.BoardAbility.Jumper,
					iconName = "icon_upgrade_super_jump"
				}
			}
		},
		{
			Boards.BoardId.rioBoard03,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 30,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_RIOBOARD03_SPARK_TRAIL_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_RIOBOARD03_SPARK_TRAIL_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SPARK_TRAIL_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.rioBoard03Theme01,
					power = Hoverboard.BoardAbility.Vanity,
					iconName = "icon_upgrade_spark_trail"
				},
				new HoverboardTheme
				{
					price = 50,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_RIOBOARD03_SUPER_SPEED_UP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_RIOBOARD03_SUPER_SPEED_UP_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SPEED_UP_DESCRIPTION",
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.rioBoard03Theme02,
					power = Hoverboard.BoardAbility.HighSpeed,
					iconName = "icon_upgrade_speed_up"
				}
			}
		},
		{
			Boards.BoardId.julyBoard,
			new List<HoverboardTheme>
			{
				new HoverboardTheme
				{
					price = 80,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_RIOBOARD03_SPARK_TRAIL_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_RIOBOARD03_SPARK_TRAIL_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SPARK_TRAIL_DESCRIPTION",
					uiOrder = UIOrder.First,
					identifier = ThemeIdentifier.julyBoardTheme01,
					power = Hoverboard.BoardAbility.Vanity,
					iconName = "icon_upgrade_spark_trail"
				},
				new HoverboardTheme
				{
					price = 100,
					unlockType = PurchaseData.UnlockType.Keys,
					buttonBgColor = GlobalColors.HOVERBOARD_THEME_RIOBOARD03_SUPER_SPEED_UP_FILL,
					buttonIconColor = GlobalColors.HOVERBOARD_THEME_RIOBOARD03_SUPER_SPEED_UP_ICON,
					isNewInThisUpdate = false,
					title = "HOVERBOARD_ABILITY_SPEED_UP_DESCRIPTION",
					uiOrder = UIOrder.Second,
					identifier = ThemeIdentifier.julyBoardTheme02,
					power = Hoverboard.BoardAbility.HighSpeed,
					iconName = "icon_upgrade_speed_up"
				}
			}
		}
	};

	public static HoverboardTheme GetThemeAbility(Boards.BoardId board, Hoverboard.BoardAbility ability)
	{
		if (hoverboardCustomThemes.ContainsKey(board))
		{
			foreach (HoverboardTheme item in hoverboardCustomThemes[board])
			{
				if (item.power == ability)
				{
					return item;
				}
			}
		}
		return null;
	}

	public static string GetBoardPowerName(Boards.BoardId board, Hoverboard.BoardAbility power)
	{
		if (hoverboardCustomThemes.ContainsKey(board))
		{
			foreach (HoverboardTheme item in hoverboardCustomThemes[board])
			{
				if (item.power == power)
				{
					return Strings.Get(item.title);
				}
			}
			SDebug.LogError("No board theme of type " + board.ToString() + " not found. Returning empty string.");
			return string.Empty;
		}
		SDebug.LogError("No board themes exist for board type: " + board.ToString() + ". Returning empty string.");
		return string.Empty;
	}

	public static int GetIndexForTheme(Boards.BoardId boardId, ThemeIdentifier themeIdentifier)
	{
		foreach (HoverboardTheme item in hoverboardCustomThemes[boardId])
		{
			if (item.identifier == themeIdentifier)
			{
				return (int)(item.uiOrder + 1);
			}
		}
		return 0;
	}

	public static int GetThemeIndexForPower(Boards.BoardId boardId, Hoverboard.BoardAbility power)
	{
		List<HoverboardTheme> list = hoverboardCustomThemes[boardId];
		int num = 0;
		foreach (HoverboardTheme item in list)
		{
			if (item.power == power)
			{
				return num;
			}
			num++;
		}
		SDebug.LogErrorFormat("Unable to find valid theme index for the board {0} with power {1}", boardId, power);
		return -1;
	}

	public static ThemeIdentifier GetThemeIdentifier(Boards.BoardId bId, int tIndex)
	{
		if (tIndex < 0)
		{
			return ThemeIdentifier.none;
		}
		if (hoverboardCustomThemes.ContainsKey(bId))
		{
			foreach (HoverboardTheme item in hoverboardCustomThemes[bId])
			{
				if (tIndex == (int)item.uiOrder)
				{
					return item.identifier;
				}
			}
			return ThemeIdentifier.none;
		}
		return ThemeIdentifier.none;
	}

	public static List<HoverboardTheme> TryGetCustomThemesForBoard(Boards.BoardId boardId)
	{
		List<HoverboardTheme> value;
		hoverboardCustomThemes.TryGetValue(boardId, out value);
		return value;
	}

	public static void VerifyData()
	{
		foreach (KeyValuePair<Boards.BoardId, List<HoverboardTheme>> hoverboardCustomTheme in hoverboardCustomThemes)
		{
			ValidateUIOrder(hoverboardCustomTheme);
		}
	}

	private static void ValidateUIOrder(KeyValuePair<Boards.BoardId, List<HoverboardTheme>> themeSet)
	{
		List<UIOrder> list = Enum.GetValues(typeof(UIOrder)).Cast<UIOrder>().ToList();
		foreach (HoverboardTheme item in themeSet.Value)
		{
			if (list.Contains(item.uiOrder))
			{
				list.Remove(item.uiOrder);
			}
			else
			{
				SDebug.LogError(string.Concat("UIOrder is repeated in themes for board: ", themeSet.Key, ". Please make sure no UIOrder enum member is repeated within the same theme set for a specific board."));
			}
		}
	}
}
