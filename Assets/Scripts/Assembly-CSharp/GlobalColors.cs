using System.Collections.Generic;
using UnityEngine;

public class GlobalColors
{
	public const int COLOR_BUTTON_PRESSED_OFFSET = 30;

	public static readonly Color32 AWARDS_NEW_STICKER_COLOR = new Color32(byte.MaxValue, 120, 38, byte.MaxValue);

	public static readonly Color32 AWARDS_COLLECTIBLE_STICKER_COLOR = new Color32(byte.MaxValue, 231, 63, byte.MaxValue);

	public static readonly Color32 TOP_RUN_STICKER_COLOR = new Color32(245, 85, 3, byte.MaxValue);

	public static readonly Color32 GIFT_COLOR = new Color32(248, 151, 0, byte.MaxValue);

	public static readonly Color32 GIFT_LABEL_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 FOOTER_ME_COLOR = new Color32(1, 101, 1, byte.MaxValue);

	private static readonly Color32 FOOTER_ME_COLOR_LIGHT = new Color32(118, 152, 69, byte.MaxValue);

	private static readonly Color32 FOOTER_ME_COLOR_SELECTED = new Color32(1, 48, 1, byte.MaxValue);

	public static readonly Color32 FOOTER_SHOP_COLOR = new Color32(38, 110, 171, byte.MaxValue);

	private static readonly Color32 FOOTER_SHOP_COLOR_LIGHT = new Color32(99, 155, 194, byte.MaxValue);

	private static readonly Color32 FOOTER_SHOP_COLOR_SELECTED = new Color32(15, 49, 79, byte.MaxValue);

	public static readonly Color32 FOOTER_SOCIAL_COLOR = new Color32(80, 43, 139, byte.MaxValue);

	private static readonly Color32 FOOTER_SOCIAL_COLOR_LIGHT = new Color32(138, 108, 202, byte.MaxValue);

	private static readonly Color32 FOOTER_SOCIAL_COLOR_SELECTED = new Color32(36, 18, 65, byte.MaxValue);

	public static readonly Color32 FOOTER_BACK_COLOR = new Color32(56, 56, 56, byte.MaxValue);

	private static readonly Color32 FOOTER_BACK_COLOR_LIGHT = new Color32(130, 130, 130, byte.MaxValue);

	public static readonly Color32 FOOTER_BIG_PLAY_RESUME_COLOR = new Color32(70, 157, 42, byte.MaxValue);

	public static readonly Color32 FOOTER_BIG_WAIT_COLOR = new Color32(118, 118, 118, byte.MaxValue);

	public static readonly Color32 EVENTS_POPUP_SPLASH_COLOR = new Color32(0, 162, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 EVENTS_POPUP_DESCRIPTION_BOX_JACKPOT_EVENT_COLOR = new Color32(1, 143, 254, byte.MaxValue);

	public static readonly Color32 EVENTS_POPUP_DESCRIPTION_BOX_NORMAL_COLOR = new Color32(254, 157, 1, byte.MaxValue);

	public static readonly Color32 EVENTS_POPUP_GLOW_DEFAULT_COLOR = new Color32(221, 209, 76, 182);

	public static readonly Color32 EVENTS_POPUP_GLOW_JACKPOT_COLOR = new Color32(170, 207, 229, 200);

	public static readonly Color32 EVENTS_POPUP_CATEGORY_DEFAULT_TITLE_LINE_1_COLOR = new Color32(7, 41, 74, byte.MaxValue);

	public static readonly Color32 EVENTS_POPUP_CATEGORY_DEFAULT_TITLE_LINE_2_COLOR = new Color32(0, 162, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 EVENTS_POPUP_CATEGORY_DEFAULT_TIMER_COLOR = new Color32(0, 162, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 EVENTS_POPUP_CATEGORY_DEFAULT_TIMER_CONTENT_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 EVENTS_POPUP_CATEGORY_VALENTINE_SPLASH_COLOR = new Color32(131, 196, 250, byte.MaxValue);

	public static readonly Color32 EVENTS_POPUP_CATEGORY_VALENTINE_TITLE_LINE_1_COLOR = new Color32(241, 74, 139, byte.MaxValue);

	public static readonly Color32 EVENTS_POPUP_CATEGORY_VALENTINE_TITLE_LINE_2_COLOR = new Color32(252, 125, 187, byte.MaxValue);

	public static readonly Color32 EVENTS_POPUP_CATEGORY_VALENTINE_TIMER_COLOR = new Color32(252, 125, 187, byte.MaxValue);

	public static readonly Color32 INACTIVE_HEADSTART_BUTTON_BACKGROUND_COLOR = new Color32(0, 0, 0, 128);

	public static readonly Color32 INACTIVE_SCOREBOOSTER_BUTTON_BACKGROUND_COLOR = new Color32(0, 0, 0, 128);

	public static readonly Color32 ACTIVE_HEADSTART_BUTTON_BACKGROUND_COLOR = new Color32(253, 225, 15, 229);

	public static readonly Color32 ACTIVE_SCOREBOOSTER_BUTTON_BACKGROUND_COLOR = new Color32(0, 132, byte.MaxValue, 229);

	public static readonly Color32 ACTIVE_HEADSTART_GEAR_BACKGROUND_COLOR = new Color32(254, 235, 0, byte.MaxValue);

	public static readonly Color32 ACTIVE_HEADSTART_GEAR_COUNT_COLOR = new Color32(153, 21, 7, byte.MaxValue);

	public static readonly Color32 ACTIVE_SCOREBOOSTER_GEAR_BACKGROUND_COLOR = new Color32(96, 214, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 ACTIVE_SCOREBOOSTER_GEAR_COUNT_COLOR = new Color32(26, 62, 162, byte.MaxValue);

	public static readonly Color32 INACTIVE_GEAR_BACKGROUND_COLOR = new Color32(0, 0, 0, 102);

	public static readonly Color32 INACTIVE_GEAR_COUNT_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 INITIAL_GEAR_GLOW_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);

	public static readonly Color32 FINAL_GEAR_GLOW_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 INITIAL_HEADSTART_BUTTON_GLOW_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);

	public static readonly Color32 FINAL_HEADSTART_BUTTON_GLOW_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 160);

	public static readonly Color32 PRIMARY_ACTION_COLOR = new Color32(65, 151, 42, byte.MaxValue);

	private static readonly Color32 PRIMARY_ACTION_COLOR_LIGHT = new Color32(153, 206, 60, byte.MaxValue);

	private static readonly Color32 PRIMARY_ACTION_COLOR_SELECTED = new Color32(54, 137, 190, byte.MaxValue);

	public static readonly Color32 PRIMARY_ACTION_COLOR_UNAVAILABLE = new Color32(118, 118, 118, byte.MaxValue);

	public static readonly Color32 COLOR_WHITE = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 COLOR_PRESSED = new Color32(128, 128, 128, byte.MaxValue);

	public static readonly Color32 SECONDARY_ACTION_COLOR = new Color32(244, 76, 0, byte.MaxValue);

	private static readonly Color32 SECONDARY_ACTION_COLOR_LIGHT = new Color32(byte.MaxValue, 199, 46, byte.MaxValue);

	public static readonly Color32 TERTIARY_ACTION_COLOR = new Color32(54, 137, 190, byte.MaxValue);

	private static readonly Color32 TERTIARY_ACTION_COLOR_LIGHT = new Color32(183, 227, 245, 112);

	private static readonly Color32 TERTIARY_ACTION_COLOR_SELECTED = new Color32(54, 137, 190, byte.MaxValue);

	public static readonly Color32 TERTIARY_ACTION_COLOR_UNAVAILABLE = new Color32(118, 118, 118, byte.MaxValue);

	public static readonly Color32 EXIT_COLOR = new Color32(168, 3, 3, byte.MaxValue);

	private static readonly Color32 EXIT_COLOR_LIGHT = new Color32(byte.MaxValue, 81, 81, byte.MaxValue);

	public static readonly Color32 SEASONAL_COLOR_FILL_NORMAL = new Color32(91, 170, 28, byte.MaxValue);

	public static readonly Color32 SEASONAL_COLOR_FILL_LIGHT = new Color32(148, 227, 85, 112);

	public static readonly Color32 FACEBOOK_COLOR = new Color32(59, 91, 153, byte.MaxValue);

	private static readonly Color32 FACEBOOK_COLOR_LIGHT = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 60);

	public static readonly Color32 SOCIAL_GREY = new Color32(119, 118, 118, byte.MaxValue);

	private static readonly Color32 SOCIAL_GRAY_OVERLAY = new Color32(160, 160, 160, byte.MaxValue);

	public static readonly Color32 WEEKLYHUNT_CELEBRATION_TEXT_COLOR_STRIPED_BACKGROUND = new Color32(0, 0, 0, byte.MaxValue);

	public static readonly Color32 WEEKLYHUNT_STRIPE_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 50);

	public static readonly Color32 WEEKLYHUNT_CELEBRATION_TEXT_COLOR = new Color32(0, 0, 0, byte.MaxValue);

	public static readonly Color32 WEEKLYHUNT_CELEBRATION_TEXT_SHADOW_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 CHARACTERS_CUSTOMIZATION_BASE_SKIN_FILL = new Color32(54, 137, 190, byte.MaxValue);

	public static readonly Color32 CHARACTERS_CUSTOMIZATION_BASE_SKIN_ICON = new Color32(14, 54, 80, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_CUSTOMIZATION_BASE_SKIN_FILL = new Color32(54, 137, 190, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_CUSTOMIZATION_BASE_SKIN_ICON = new Color32(14, 54, 80, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_HERO_SUPER_JUMP_FILL = new Color32(209, 91, 0, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_HERO_SUPER_JUMP_ICON = new Color32(211, 249, 59, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_HERO_TELEPORT_FILL = new Color32(0, 93, 114, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_HERO_TELEPORT_ICON = new Color32(38, 237, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_STARBOARD_TRAIL_FILL = new Color32(81, 103, 9, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_STARBOARD_TRAIL_ICON = new Color32(byte.MaxValue, 248, 46, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_STARBOARD_DOUBLE_JUMP_FILL = new Color32(133, 24, 151, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_STARBOARD_DOUBLE_JUMP_ICON = new Color32(byte.MaxValue, 233, 52, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_GREAT_WHITE_SPEEDUP_FILL = new Color32(177, 27, 27, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_GREAT_WHITE_SPEEDUP_ICON = new Color32(byte.MaxValue, 239, 57, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_GREAT_WHITE_GLIDER_FILL = new Color32(25, 123, 73, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_GREAT_WHITE_GLIDER_ICON = new Color32(0, byte.MaxValue, 138, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_SURF_BOARD_LOWRIDER_FILL = new Color32(50, 102, 179, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_SURF_BOARD_LOWRIDER_ICON = new Color32(191, byte.MaxValue, 81, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_SURF_BOARD_GLIDER_FILL = new Color32(25, 123, 73, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_SURF_BOARD_GLIDER_ICON = new Color32(0, byte.MaxValue, 138, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_MONSTER_SPEEDUP_FILL = new Color32(177, 27, 27, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_MONSTER_SPEEDUP_ICON = new Color32(byte.MaxValue, 239, 57, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_MONSTER_JUMPER_FILL = new Color32(209, 91, 0, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_MONSTER_JUMPER_ICON = new Color32(211, 249, 59, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_FREESTYLER_DOUBLE_JUMP_FILL = new Color32(133, 24, 151, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_FREESTYLER_DOUBLE_JUMP_ICON = new Color32(byte.MaxValue, 233, 52, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_FREESTYLER_SUPER_JUMP_FILL = new Color32(209, 91, 0, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_FREESTYLER_SUPER_JUMP_ICON = new Color32(211, 249, 59, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_MIAMI_PINK_TRAIL_FILL = new Color32(232, 74, 149, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_MIAMI_PINK_TRAIL_ICON = new Color32(byte.MaxValue, 185, 217, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_MIAMI_SMOOTHDRIFT_FILL = new Color32(25, 123, 73, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_MIAMI_SMOOTHDRIFT_ICON = new Color32(0, byte.MaxValue, 138, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_SKULLFIRE_DOUBLE_JUMP_FILL = new Color32(133, 24, 151, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_SKULLFIRE_DOUBLE_JUMP_ICON = new Color32(byte.MaxValue, 233, 52, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_SKULLFIRE_SPEEDUP_FILL = new Color32(177, 27, 27, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_SKULLFIRE_SPEEDUP_ICON = new Color32(byte.MaxValue, 239, 57, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_ROME_ZAPSIDEWAYS_FILL = new Color32(0, 93, 114, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_ROME_ZAPSIDEWAYS_ICON = new Color32(38, 237, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_ROME_SPEEDUP_FILL = new Color32(177, 27, 27, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_ROME_SPEEDUP_ICON = new Color32(byte.MaxValue, 239, 57, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_LUMBERJACK_DOUBLE_JUMP_FILL = new Color32(133, 24, 151, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_LUMBERJACK_DOUBLE_JUMP_ICON = new Color32(byte.MaxValue, 233, 52, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_LUMBERJACK_LOWRIDER_FILL = new Color32(50, 102, 179, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_LUMBERJACK_LOWRIDER_ICON = new Color32(191, byte.MaxValue, 81, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_LONDONBOARD01_TRAIL_FILL = new Color32(31, 57, 144, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_LONDONBOARD01_TRAIL_ICON = new Color32(163, 220, 246, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_LONDONBOARD01_SUPER_JUMP_FILL = new Color32(209, 91, 0, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_LONDONBOARD01_SUPER_JUMP_ICON = new Color32(211, 249, 59, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_CHICK_TRAIL_FILL = new Color32(251, 226, 13, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_CHICK_TRAIL_ICON = new Color32(201, 77, 0, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_CHICK_SPEEDUP_FILL = new Color32(177, 27, 27, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_CHICK_SPEEDUP_ICON = new Color32(byte.MaxValue, 239, 57, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_VENICEBOARD02_GLIDER_FILL = new Color32(25, 123, 73, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_VENICEBOARD02_GLIDER_ICON = new Color32(0, byte.MaxValue, 138, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_SANFRANCISCOBOARD01_TRAIL_FILL = new Color32(237, 97, 163, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_SANFRANCISCOBOARD01_TRAIL_ICON = new Color32(248, 182, 212, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_HONGKONG_GLIDE_FILL = new Color32(25, 123, 73, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_HONGKONG_GLIDE_ICON = new Color32(0, byte.MaxValue, 138, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_BANGKOKBOARD01_TRAIL_FILL = new Color32(237, 97, 163, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_BANGKOKBOARD01_TRAIL_ICON = new Color32(248, 182, 212, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_ARABIABOARD01_TRAIL_FILL = new Color32(156, 131, 97, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_ARABIABOARD01_TRAIL_ICON = new Color32(212, 197, 174, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_CAIROBOARD02_SPEEDUP_FILL = new Color32(177, 27, 27, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_CAIROBOARD02_SPEEDUP_ICON = new Color32(byte.MaxValue, 239, 57, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_CAIROBOARD02_GLIDER_FILL = new Color32(25, 123, 73, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_CAIROBOARD02_GLIDER_ICON = new Color32(0, byte.MaxValue, 138, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_MUMBAIBOARD02_SPEEDUP_FILL = new Color32(177, 27, 27, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_MUMBAIBOARD02_SPEEDUP_ICON = new Color32(byte.MaxValue, 239, 57, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_MUMBAIBOARD02_ZAPSIDEWAYS_FILL = new Color32(93, 0, 180, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_MUMBAIBOARD02_ZAPSIDEWAYS_ICON = new Color32(254, 147, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_STARTER_PACK_BOARD_01_MUSIC_TRAIL_ICON = new Color32(168, byte.MaxValue, 0, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_STARTER_PACK_BOARD_01_MUSIC_TRAIL_FILL = new Color32(14, 167, 50, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_MARRAKESHBOARD02_GLIDER_FILL = new Color32(25, 123, 73, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_MARRAKESHBOARD02_GLIDER_ICON = new Color32(0, byte.MaxValue, 138, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_MARRAKESHBOARD02_SUPER_JUMP_FILL = new Color32(209, 91, 0, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_MARRAKESHBOARD02_SUPER_JUMP_ICON = new Color32(211, 249, 59, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_RIOBOARD03_SPARK_TRAIL_FILL = new Color32(byte.MaxValue, 127, 0, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_RIOBOARD03_SPARK_TRAIL_ICON = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_RIOBOARD03_SUPER_SPEED_UP_FILL = new Color32(177, 27, 27, byte.MaxValue);

	public static readonly Color32 HOVERBOARD_THEME_RIOBOARD03_SUPER_SPEED_UP_ICON = new Color32(byte.MaxValue, 239, 57, byte.MaxValue);

	public static readonly Color32 EVENTS_POPUP_STRIPES_JACKPOT = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 75);

	public static readonly Color32 EVENTS_POPUP_STRIPES_NORMAL = new Color32(byte.MaxValue, 242, 66, 75);

	public static readonly Color32 COINS_ORIGINAL_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 COINS_LIGHT_PURPLE = new Color32(208, 166, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 COINS_DARK_PURPLE = new Color32(142, 31, 186, byte.MaxValue);

	public static readonly Color32 MULTIPLIER_LIGHT_PURPLE = new Color32(208, 166, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 MULTIPLIER_DARK_PURPLE = new Color32(142, 31, 186, byte.MaxValue);

	public static readonly Color32 COUNTRY_POPUP_NORMAL_CELL_TEXT_COLOUR = new Color32(11, 43, 83, byte.MaxValue);

	public static readonly Color32 COUNTRY_POPUP_SELECTED_CELL_TEXT_COLOUR = new Color32(3, 116, 165, byte.MaxValue);

	public static readonly Color32 TOPRUN_ME_PLAYER_BACKGROUND_COLOUR = new Color32(11, 43, 83, 204);

	public static readonly Color32 TOPRUN_NORMAL_PLAYER_TEXT_COLOUR = new Color32(11, 43, 83, byte.MaxValue);

	public static readonly Color32 TOPRUN_POPUP_TEXT_BACKGROUND_PURPLE = new Color32(111, 87, 150, byte.MaxValue);

	public static readonly Color32 TOPRUN_POPUP_DIAMOND_BACKGROUND = new Color32(152, 125, 196, byte.MaxValue);

	public static readonly Color32 TOPRUN_POPUP_GOLD_BACKGROUND = new Color32(byte.MaxValue, 185, 29, byte.MaxValue);

	public static readonly Color32 TOPRUN_POPUP_SILVER_BACKGROUND = new Color32(85, 169, 233, byte.MaxValue);

	public static readonly Color32 TOPRUN_POPUP_BRONZE_BACKGROUND = new Color32(248, 142, 86, byte.MaxValue);

	public static readonly Color32 TOPRUN_RANK_HEADER_00_BACKGROUND_COLOUR = new Color32(112, 84, 156, byte.MaxValue);

	public static readonly Color32 TOPRUN_RANK_HEADER_00_TITLE_COLOUR = new Color32(243, 235, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 TOPRUN_RANK_HEADER_00_TITLE_SHADOW_COLOUR = new Color32(32, 0, 64, byte.MaxValue);

	public static readonly Color32 TOPRUN_PLAYER_BACKGOURD_00_DARK_COLOUR = new Color32(130, 83, 168, 127);

	public static readonly Color32 TOPRUN_PLAYER_BACKGOURD_00_LIGHT_COLOUR = new Color32(203, 159, 221, 127);

	public static readonly Color32 TOPRUN_RANK_HEADER_10_BACKGROUND_COLOUR = new Color32(238, 181, 72, byte.MaxValue);

	public static readonly Color32 TOPRUN_RANK_HEADER_10_TITLE_COLOUR = new Color32(byte.MaxValue, 249, 229, byte.MaxValue);

	public static readonly Color32 TOPRUN_RANK_HEADER_10_TITLE_SHADOW_COLOUR = new Color32(189, 124, 0, byte.MaxValue);

	public static readonly Color32 TOPRUN_PLAYER_BACKGOURD_10_DARK_COLOUR = new Color32(byte.MaxValue, 190, 16, 127);

	public static readonly Color32 TOPRUN_PLAYER_BACKGOURD_10_LIGHT_COLOUR = new Color32(byte.MaxValue, 232, 152, 127);

	public static readonly Color32 TOPRUN_RANK_HEADER_20_BACKGROUND_COLOUR = new Color32(60, 124, 173, byte.MaxValue);

	public static readonly Color32 TOPRUN_RANK_HEADER_20_TITLE_COLOUR = new Color32(243, 253, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 TOPRUN_RANK_HEADER_20_TITLE_SHADOW_COLOUR = new Color32(2, 77, 135, byte.MaxValue);

	public static readonly Color32 TOPRUN_PLAYER_BACKGOURD_20_DARK_COLOUR = new Color32(49, 166, byte.MaxValue, 127);

	public static readonly Color32 TOPRUN_PLAYER_BACKGOURD_20_LIGHT_COLOUR = new Color32(138, 196, 233, 127);

	public static readonly Color32 TOPRUN_RANK_HEADER_30_BACKGROUND_COLOUR = new Color32(236, 145, 96, byte.MaxValue);

	public static readonly Color32 TOPRUN_RANK_HEADER_30_TITLE_COLOUR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 TOPRUN_RANK_HEADER_30_TITLE_SHADOW_COLOUR = new Color32(185, 87, 34, byte.MaxValue);

	public static readonly Color32 TOPRUN_PLAYER_BACKGOURD_30_DARK_COLOUR = new Color32(252, 163, 117, 127);

	public static readonly Color32 TOPRUN_PLAYER_BACKGOURD_30_LIGHT_COLOUR = new Color32(byte.MaxValue, 226, 211, 127);

	public static readonly Color32 TOPRUN_RANK_HEADER_40_BACKGROUND_COLOUR = new Color32(112, 132, 145, byte.MaxValue);

	public static readonly Color32 TOPRUN_RANK_HEADER_40_TITLE_COLOUR = new Color32(244, 244, 244, byte.MaxValue);

	public static readonly Color32 TOPRUN_RANK_HEADER_40_TITLE_SHADOW_COLOUR = new Color32(62, 91, 110, byte.MaxValue);

	public static readonly Color32 TOPRUN_PLAYER_BACKGOURD_40_DARK_COLOUR = new Color32(109, 153, 184, 127);

	public static readonly Color32 TOPRUN_PLAYER_BACKGOURD_40_LIGHT_COLOUR = new Color32(170, 201, 223, 127);

	public static readonly Color32 CHARACTERWORDEVENT_CELEBRATION_BLUE = new Color32(9, 42, 83, byte.MaxValue);

	public static readonly Color32 CHARACTERWORDEVENT_CELEBRATION_WHITE = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 CHARACTERWORDEVENT_CELEBRATION_BLUESHADOW = new Color32(30, 84, 127, byte.MaxValue);

	public static readonly Color32 CHARACTERWORDEVENT_CELEBRATION_WHITESHADOW = new Color32(158, 213, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 CHARACTERWORDEVENT_POPUP_NAME_ACTIVE = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 CHARACTERWORDEVENT_POPUP_NAME_INACTIVE = new Color32(181, 107, 18, byte.MaxValue);

	public static readonly Color32 CHARACTERWORDEVENT_POPUP_NAME_SHADOW_ACTIVE = new Color32(143, 69, 0, byte.MaxValue);

	public static readonly Color32 CHARACTERWORDEVENT_POPUP_NAME_SHADOW_INACTIVE = new Color32(254, 230, 140, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_TITLE_COLOR = new Color32(230, 229, 162, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_TITLE_COLOR_SHADOW = new Color32(117, 23, 11, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_DESCRIPTION1_ACTIVE_COLOR = new Color32(171, 171, 171, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_DESCRIPTION1_ACTIVE_SHADOW_COLOR = new Color32(0, 0, 0, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_DESCRIPTION1_INACTIVE_COLOR = new Color32(117, 21, 11, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_DESCRIPTION1_INACTIVE_SHADOW_COLOR = new Color32(0, 0, 0, 0);

	public static readonly Color32 SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_PRICE_COLOR = new Color32(230, 229, 162, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_PRICE_SHADOW_COLOR = new Color32(0, 64, 48, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_LIMITED_TIME_COLOR = new Color32(156, 232, 186, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_LIMITED_TIME_SHADOW_COLOR = new Color32(0, 64, 48, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_DESCRIPTION2_COLOR = new Color32(254, 214, 32, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_DESCRIPTION2_SHADOW_COLOR = new Color32(117, 23, 11, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_LINE_THROUGH_COLOR = new Color32(254, 214, 32, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_WINTER_HOLIDAY_BUTTON_LINE_THROUGH_SHADOW_COLOR = new Color32(117, 23, 11, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_WINTER_HOLIDAY_STICKER_LABEL_COLOR = new Color32(230, 229, 162, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_WINTER_HOLIDAY_STICKER_COLOR = new Color32(171, 45, 45, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_TITLE_COLOR = new Color32(246, 216, 77, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_TITLE_COLOR_SHADOW = new Color32(10, 21, 134, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_DESCRIPTION1_ACTIVE_COLOR = new Color32(182, 226, 254, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_DESCRIPTION1_ACTIVE_SHADOW_COLOR = new Color32(0, 0, 0, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_DESCRIPTION1_INACTIVE_COLOR = new Color32(11, 14, 56, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_DESCRIPTION1_INACTIVE_SHADOW_COLOR = new Color32(0, 0, 0, 0);

	public static readonly Color32 SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_PRICE_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_PRICE_SHADOW_COLOR = new Color32(108, 6, 23, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_LIMITED_TIME_COLOR = new Color32(108, 6, 23, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_LIMITED_TIME_SHADOW_COLOR = new Color32(byte.MaxValue, 159, 127, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_DESCRIPTION2_COLOR = new Color32(182, 226, 254, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_DESCRIPTION2_SHADOW_COLOR = new Color32(10, 21, 134, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_LINE_THROUGH_COLOR = new Color32(182, 226, 254, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_NEW_YEAR_BUTTON_LINE_THROUGH_SHADOW_COLOR = new Color32(10, 21, 134, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_NEW_YEAR_STICKER_COLOR = new Color32(87, 97, 235, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_NEW_YEAR_STICKER_LABEL_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_TITLE_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_TITLE_COLOR_SHADOW = new Color32(0, 0, 0, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_DESCRIPTION1_ACTIVE_COLOR = new Color32(166, 166, 166, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_DESCRIPTION1_ACTIVE_SHADOW_COLOR = new Color32(0, 0, 0, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_DESCRIPTION1_INACTIVE_COLOR = new Color32(166, 166, 166, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_DESCRIPTION1_INACTIVE_SHADOW_COLOR = new Color32(0, 0, 0, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_PRICE_COLOR = new Color32(0, 0, 0, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_PRICE_SHADOW_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_LIMITED_TIME_COLOR = new Color32(130, 29, 2, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_LIMITED_TIME_SHADOW_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_DESCRIPTION2_COLOR = new Color32(byte.MaxValue, 221, 61, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_DESCRIPTION2_SHADOW_COLOR = new Color32(0, 0, 0, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_LINE_THROUGH_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_BLACK_FRIDAY_BUTTON_LINE_THROUGH_SHADOW_COLOR = new Color32(50, 50, 50, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_BLACK_FRIDAY_STICKER_COLOR = new Color32(0, 0, 0, byte.MaxValue);

	public static readonly Color32 SPECIAL_DISCOUNT_BLACK_FRIDAY_STICKER_LABEL_COLOR = new Color32(byte.MaxValue, 221, 61, byte.MaxValue);

	public static readonly Color32 FIRST_PURCHASE_BONUS_BREADCRUMP_BUBBLE_COLOR = new Color32(36, 134, 187, byte.MaxValue);

	public static readonly Dictionary<UIButtonOverlayOff.ButtonType, ButtonColorScheme> buttonColorData = new Dictionary<UIButtonOverlayOff.ButtonType, ButtonColorScheme>
	{
		{
			UIButtonOverlayOff.ButtonType.Custom,
			new ButtonColorScheme
			{
				light = null,
				selected = null,
				unavailable = null,
				original = null
			}
		},
		{
			UIButtonOverlayOff.ButtonType.Facebook,
			new ButtonColorScheme
			{
				light = FACEBOOK_COLOR_LIGHT,
				selected = null,
				unavailable = null,
				original = FACEBOOK_COLOR
			}
		},
		{
			UIButtonOverlayOff.ButtonType.Footer_back,
			new ButtonColorScheme
			{
				light = FOOTER_BACK_COLOR_LIGHT,
				selected = null,
				unavailable = null,
				original = FOOTER_BACK_COLOR
			}
		},
		{
			UIButtonOverlayOff.ButtonType.Footer_friends,
			new ButtonColorScheme
			{
				light = FOOTER_SOCIAL_COLOR_LIGHT,
				selected = FOOTER_SOCIAL_COLOR_SELECTED,
				unavailable = null,
				original = FOOTER_SOCIAL_COLOR
			}
		},
		{
			UIButtonOverlayOff.ButtonType.Footer_me,
			new ButtonColorScheme
			{
				light = FOOTER_ME_COLOR_LIGHT,
				selected = FOOTER_ME_COLOR_SELECTED,
				unavailable = null,
				original = FOOTER_ME_COLOR
			}
		},
		{
			UIButtonOverlayOff.ButtonType.Footer_shop,
			new ButtonColorScheme
			{
				light = FOOTER_SHOP_COLOR_LIGHT,
				selected = FOOTER_SHOP_COLOR_SELECTED,
				unavailable = null,
				original = FOOTER_SHOP_COLOR
			}
		},
		{
			UIButtonOverlayOff.ButtonType.Primary,
			new ButtonColorScheme
			{
				light = PRIMARY_ACTION_COLOR_LIGHT,
				selected = PRIMARY_ACTION_COLOR_SELECTED,
				unavailable = PRIMARY_ACTION_COLOR_UNAVAILABLE,
				original = PRIMARY_ACTION_COLOR
			}
		},
		{
			UIButtonOverlayOff.ButtonType.Seasonal,
			new ButtonColorScheme
			{
				light = SEASONAL_COLOR_FILL_LIGHT,
				selected = null,
				unavailable = null,
				original = SEASONAL_COLOR_FILL_NORMAL
			}
		},
		{
			UIButtonOverlayOff.ButtonType.Secondary,
			new ButtonColorScheme
			{
				light = SECONDARY_ACTION_COLOR_LIGHT,
				selected = null,
				unavailable = null,
				original = SECONDARY_ACTION_COLOR
			}
		},
		{
			UIButtonOverlayOff.ButtonType.SocialGrey,
			new ButtonColorScheme
			{
				light = SOCIAL_GRAY_OVERLAY,
				selected = null,
				unavailable = null,
				original = SOCIAL_GREY
			}
		},
		{
			UIButtonOverlayOff.ButtonType.Tertiary,
			new ButtonColorScheme
			{
				light = TERTIARY_ACTION_COLOR_LIGHT,
				selected = TERTIARY_ACTION_COLOR_SELECTED,
				unavailable = TERTIARY_ACTION_COLOR_UNAVAILABLE,
				original = TERTIARY_ACTION_COLOR
			}
		},
		{
			UIButtonOverlayOff.ButtonType.Exit,
			new ButtonColorScheme
			{
				light = EXIT_COLOR_LIGHT,
				selected = null,
				unavailable = null,
				original = EXIT_COLOR
			}
		}
	};

	public static Color ApplyButtonPressColorOffset(Color32 col)
	{
		col.r = (byte)Mathf.Clamp(col.r - 30, 0, 255);
		col.g = (byte)Mathf.Clamp(col.g - 30, 0, 255);
		col.b = (byte)Mathf.Clamp(col.b - 30, 0, 255);
		return col;
	}

	public static ButtonColorScheme GetButtonColorScheme(UIButtonOverlayOff.ButtonType buttonType, GameObject gameObject)
	{
		if (buttonColorData.ContainsKey(buttonType))
		{
			return buttonColorData[buttonType];
		}
		SDebug.LogError("No color scheme for button type: " + buttonType.ToString() + ". GameObject: " + gameObject.name);
		return buttonColorData[UIButtonOverlayOff.ButtonType.Custom];
	}
}
