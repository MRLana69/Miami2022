using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Data.Save;
using Kiloo;
using Kiloo.Common.Extensions;
using Kiloo.Common.FlurryAnalytics;
using Kiloo.Common.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class Flurry
{
	public const string EVENT_UISCREEN_CHANGED_PREFIX = "UI Screen ";

	public const string EVENT_POPUPSCREEN_CHANGED_PREFIX = "POPUP Screen ";

	public const string EVENT_FIRST_GAMECENTER_LOGIN = "First GameCenter Login";

	public const string EVENT_FIRST_FACEBOOK_LOGIN = "First Facebook Login";

	public const string EVENT_SOCIAL_POKE = "Social friend poked";

	public const string EVENT_SOCIAL_BRAG = "Social bragged";

	public const string EVENT_SOCIAL_BRAGFACEBOOK = "Social bragged Facebook";

	public const string EVENT_SOCIAL_FACEBOOK_FRIENDS_RETRIEVED = "Social Facebook Friends Retrieved";

	public const string EVENT_SOCIAL_GC_FRIENDS_RETRIEVED = "Social GameCenter Friends Retrieved";

	public const string EVENT_SOCIAL_FRIENDS_CONSOLIDATED = "Social Friends Consolidated";

	public const string EVENT_FACEBOOK_STATUS = "Facebook status";

	private const string HAS_LOGGED_DAILY_EVENT_SOCIAL_PREFIX = "socflurdaily";

	public const string EVENT_MYSTERY_BOX_OPENED = "Mystery Box opened";

	public const string EVENT_MINIMYSTERY_BOX_OPENED = "Mini Mystery Box opened";

	public const string EVENT_SUPER_MYSTERY_BOX_OPENED = "Super mystery box opened";

	public const string EVENT_INAPPPURCHASE_COMPLETED = "InApp purchase completed";

	public const string EVENT_INAPPPURCHASE_COINPACK1 = "InApp Coin Pack 1 purchased";

	public const string EVENT_INAPPPURCHASE_COINPACK2 = "InApp Coin Pack 2 purchased";

	public const string EVENT_INAPPPURCHASE_COINPACK2_5 = "InApp Coin Pack 2.5 purchased";

	public const string EVENT_INAPPPURCHASE_COINPACK3 = "InApp Coin Pack 3 purchased";

	public const string EVENT_INAPPPURCHASE_COINPACK4 = "InApp Coin Pack 4 purchased";

	public const string EVENT_INAPPPURCHASE_COINPACK5 = "InApp Coin Pack 5 purchased";

	public const string EVENT_INAPPPURCHASE_KEYPACK1 = "InApp Key Pack 1 purchased";

	public const string EVENT_INAPPPURCHASE_KEYPACK1_5 = "InApp Key Pack 1.5 purchased";

	public const string EVENT_INAPPPURCHASE_KEYPACK2 = "InApp Key Pack 2 purchased";

	public const string EVENT_INAPPPURCHASE_KEYPACK3 = "InApp Key Pack 3 purchased";

	public const string EVENT_INAPPPURCHASE_KEYPACK4 = "InApp Key Pack 4 purchased";

	public const string EVENT_INAPPPURCHASE_DOUBLECOINS = "Double Coin purchased";

	public const string EVENT_INAPPPURCHASE_DOUBLECOINS_POPUP = "Double Coin purchased GameOver";

	public const string EVENT_INAPPPURCHASE_BUNDLE1 = "InApp Bundle 1 purchased";

	public const string EVENT_INAPPPURCHASE_BUNDLE2 = "InApp Bundle 2 purchased";

	public const string EVENT_INAPPPURCHASE_BUNDLE3 = "InApp Bundle 3 purchased";

	public const string EVENT_INAPPPURCHASE_BUNDLE4 = "InApp Bundle 4 purchased";

	public const string EVENT_INAPPPURCHASE_BUNDLE5 = "InApp Bundle 5 purchased";

	public const string EVENT_CHARACTER_UNLOCKED = "Character unlocked";

	public const string EVENT_DAYS_PASSED_SINCE_LAST_PLAY = "Days passed since last play";

	public const string EVENT_BUY_HOVERBOARD_FROM_POPUP = "Purchase hoverboard from popup";

	public const string EVENT_BUY_SCOREBOOSTER_FROM_POPUP = "Purchase scorebooster from popup";

	public const string EVENT_BUY_HEADSTART_FROM_POPUP = "Purchase headstart from popup";

	public const string EVENT_APPLICATION_AUTHENTICITY = "Application Authenticity";

	public const string EVENT_UNKNOWN_GPU = "Unknown GPU";

	public const string EVENT_DEVICE_PERFORMANCE = "Device performance";

	public const string EVENT_END_OF_RUN_AVG_FPS = "InRunAvgFPS";

	public const string EVENT_END_OF_RUN_POWERUPS_USED = "Powerups used in run";

	public const string EVENT_CHARACTERS_OWNED = "Characters owned";

	public const string EVENT_HOVERBOARDS_OWNED = "Hoverboards owned";

	public const string EVENT_CHARACTER_OUTFITS_OWNED = "Character outfits owned";

	public const string EVENT_HOVERBOARD_ABILITIES_OWNED = "Hoverboard abilities owned";

	public const string EVENT_ACTIVATED_HOVERBOARD = "Boost hoverboard activated";

	public const string EVENT_ACTIVATED_HEADSTART = "Boost headstart activated";

	public const string EVENT_ACTIVATED_SCOREBOOSTER = "Boost scorebooster activated";

	public const string EVENT_DAILY_BUNDLEIDENTIFIER = "The current bundle identifier";

	public const string EVENT_DEVICE_LOCALE = "Device locale";

	public const string EVENT_BOARD_BUNDLE_POPUP_SHOWN = "Board bundle popup shown";

	public const string EVENT_STORAGE_MAIN_FOLDER_SIZE = "Main Storage Sizes";

	public const string EVENT_STORAGE_PROVIDER_FOLDER_SIZE = "Provider Storage Sizes";

	public const string EVENT_GAMEOVER_DOUBLECOINS_PRESSED = "Double Coin button pressed";

	public const string EVENT_GAMEOVER_TAB_CHANGED = "Game Over Screen Tabs";

	public const string EVENT_GAMEOVER_SCREEN_BUTTONS = "Game Over Screen Buttons";

	public const string EVENT_FIRST_VALIDATED_PURCHASE = "First Validated Purchase";

	public const string EVENT_FIRST_PURCHASE_POPUP = "First purchase popup";

	public const string EVENT_PUSH_NOTIFICATION_OPENED = "PushNotificationOpen";

	public const string EVENT_WELCOME_PACK_PURCHASED = "welcome_pack_purchased";

	public const string EVENT_FRONT_SCREEN_PROMOTION_POPUP = "Front Screen Promotion Popup";

	public const string EVENT_MISSIONSET_COMPLETED = "Mission Set completed";

	public const string EVENT_DAILY_CHALLENGE_COMPLETED = "Daily Challenge completed";

	public const string EVENT_FILEUTIL_LOAD_CORRUPTED = "FileUtil load corrupted";

	public const string EVENT_FILEUTIL_SAVE_EXCEPTION = "FileUtil Save Exception";

	public const string EVENT_FILEUTIL_SAFE_SAVE_EXCEPTION = "FileUtil SafeSave Exception";

	public const string EVENT_FRIENDREWARD_COLLECTED = "Friend reward collect";

	public const string EVENT_EARNCOINS_ITEM_CLICKED = "Earn Coins item clicked";

	public const string EVENT_VIDEO_ADS_CLICKED = "Video Ads button clicked";

	public const string EVENT_QUICKVIDEO_ADS_CLICKED = "Quick Video Ads button clicked";

	public const string EVENT_EARNCOINS_SCREEN_OPENED = "Earn coins screen opened";

	public const string EVENT_OLD_DEVICE_TOKEN_UNREGISTERED = "Old Device Token Unregistered";

	public const string EVENT_CHARACTER_THEME_UNLOCKED = "Outfit bought";

	public const string EVENT_HOVERBOARD_BOUGHT = "Hoverboard bought";

	public const string EVENT_FACEBOOK_SHARE = "Share link on Facebook";

	public const string EVENT_BREADCRUMBS_BEFORE_FREE_HOVERBOARD = "Bread crumbs before purchasing the free hoverboard";

	public const string EVENT_MORE_COINS_CLICKED = "More coins button clicked";

	public const string EVENT_BACK_BTN_PRESSED = "Back button pressed";

	public const string EVENT_HOME_BUTTON_PRESSED = "Home button pressed";

	public const string EVENT_RATING_POPUP = "Rate the app popup";

	public const string EVENT_AMOUNT_OF_COINS = "Daily report of coins";

	public const string EVENT_AMOUNT_OF_KEYS = "Daily keys report";

	public const string EVENT_AMOUNT_OF_CONSUMABLE_HOVERBOARDS = "Daily report of consumable hoverboards";

	public const string EVENT_AMOUNT_OF_HEADSTARTS = "Daily report of headstarts owned";

	public const string EVENT_AMOUNT_OF_SCOREBOOSTERS = "Daily report of score boosters owned";

	public const string EVENT_DAILY_ONLINE_STATUS = "Daily Check of Online Status";

	public const string EVENT_DAILY_HIGHSCORE_REPORT = "Daily highscore report";

	public const string EVENT_NOTIFICATIONS_STATE = "State of notifications";

	public const string EVENT_REMINDERS_STATE = "State of reminders";

	public const string EVENT_SSEVENT_POPUP_SHOWN = "Event Popup was shown";

	public const string EVENT_HOVERBOARD_TRIED = "Event Popup was shown";

	public const string EVENT_ACTIVE_CHARACTER = "Currently active character";

	public const string EVENT_ACTIVE_BOARD = "Currently active board";

	public const string EVENT_SKIPPED_CHARACTER_FOR_CHARACTER_NAME_EVENT = "Skipped character for Wordy Weekend";

	public const string EVENT_UNLOCKED_CHARACTER_FOR_CHARACTER_NAME_EVENT_WITH_PARAMETERS = "Unlocked character for Wordy Weekend";

	public const string EVENT_ATTEMPT_TO_BUY_COINS_FOR_CHARACTER_NAME_EVENT = "Attempt to buy coins for Wordy Weekend";

	public const string EVENT_ATTEMPT_TO_BUY_KEYS_FOR_CHARACTER_NAME_EVENT = "Attempt to buy keys for the Wordy Weekend";

	public const string EVENT_BOUGHT_COINS_FOR_CHARACTER_NAME_EVENT = "Bought coins for Wordy Weekend";

	public const string EVENT_BOUGHT_KEYS_FOR_CHARACTER_NAME_EVENT = "Bought keys for Wordy Weekend";

	public const string EVENT_CANCELED_BUYING_COINS_FOR_CHARACTER_NAME_EVENT = "Canceled buying coins for Wordy Weekend";

	public const string EVENT_CANCELED_BUYING_KEYS_FOR_CHARACTER_NAME_EVENT = "Canceled buying keys for Wordy Weekend";

	public const string EVENT_UNABLE_TO_BUY_COINS_FOR_CHARACTER_NAME_EVENT = "Unable to buy coins for Wordy Weekend";

	public const string EVENT_UNABLE_TO_BUY_KEYS_FOR_CHARACTER_NAME_EVENT = "Unable to buy keys for Wordy Weekend";

	public const string EVENT_ATTEMPTED_TO_SHOW_DOUBLE_UP_BUTTON = "Attempted to show double up button";

	public const string EVENT_DOUBLE_UP_CHOICE_RESULT = "Double up choice";

	public const string EVENT_DOUBLE_UP_BUTTON_SHOWN = "Double up button shown";

	public const string EVENT_DOUBLE_UP_BUTTON_CLICKED = "Double up button clicked";

	public const string EVENT_LIMITED_CHARACTER_POPUP_CLOSED = "Limited character popup closed";

	public const string EVENT_LIMITED_BOARD_POPUP_CLOSED = "Limited board popup closed";

	public const string EVENT_FIRST_INSTALL_DATE = "First install date";

	public const string EVENT_CONSUMABLES_SOLD_DURING_RUN = "Consumables Sold During Run";

	public const string EVENT_ABTEST_DAILY_REPORT = "A/B test daily report";

	public const string EVENT_ABTEST_INAPP_OPENED = "A/B Product Click";

	public const string EVENT_ABTEST_SCREEN_OPENED = "A/B Screen Opened";

	public const string EVENT_ABTEST_AD_REMOVAL_POPUP_SHOWN = "A/B Ad Removal Popup Shown";

	public const string EVENT_RESTORED_VALIDATED_ITEM = "Restored item";

	public const string EVENT_EXIT_TUTORIAL_AREA = "Tutorial exit";

	public const string EVENT_FIRST_TIME_FRONT_SCREEN = "First front screen";

	public const string EVENT_TUTORIAL_HOVERBOARD = "Tutorial hoverboard activated";

	public const string EVENT_RUNS_FOR_MISSION = "Runs completed for mission";

	public const string EVENT_FRONT_SCREEN_FLOW = "Front screen flow";

	public const string EVENT_GAMEOVER_SCREEN_FLOW = "Game over screen flow";

	public const string EVENT_MANUALLY_CLOSED_AD_OVERLAY = "Manually closed ad overlay";

	public const string EVENT_SHOW_COINS_VIDEO_BUTTON = "Attempt to show not enough coin video button";

	public const string EVENT_UI_SCREEN_NOT_ENOUGH_CURRENCY_POPUP = "NotEnoughCurrencyPopup";

	public const string EVENT_ADSYSTEM_PROVIDER_TIME_LIMIT_REACHED = "Provider Time Limit Reached";

	public const string EVENT_ATTEMPT_TO_FILL = "Attempt to fill {0}";

	public const string EVENT_ADSYSTEM_AD_LOCATION_EVENT_NAME = "Ad Location: {0}";

	public const string EVENT_LOW_MEMORY_WARNING_RECEIVED = "Low Memory Warning Recieved";

	public const string EVENT_LOW_MEMORY_WARNING_SESSION_RESULT = "Low Memory Session Result";

	public const string EVENT_PROMO_CODE_USED = "Promo Code Used";

	public const string EVENT_PROMO_CODE_ERROR = "Promo code error";

	public const string EVENT_PROMO_CODE_CONFIRMATION = "Promo code confirmation";

	public const string EVENT_OFFERWALL_SHOWN = "OfferWall Shown";

	public const string EVENT_OFFERWALL_CLOSED = "OfferWall Closed";

	public const string EVENT_OFFERWALL_AVAILABLE = "OfferWall Available";

	public const string EVENT_OFFERWALL_PAYOUT = "OfferWall Payout";

	public const string EVENT_HOVERBOARD_THEME_BOUGHT = "Hoverboard Theme bought";

	public const string EVENT_HOVERBOARD_THEME_ACTIVATED = "Hoverboard Theme activated";

	public const string EVENT_HOVERBOARD_THEME_DEACTIVATED = "Hoverboard Theme deactivated";

	public const string EVENT_BOOST_SCOREBOOSTER_PURCHASED = "Boost Scorebooster purchased";

	public const string EVENT_BOOST_HEADSTART2000_PURCHASED = "Boost Headstart2000 purchased";

	public const string EVENT_BOOST_HOVERBOARD_PURCHASED = "Boost Hoverboard purchased";

	public const string EVENT_BOOST_COINMAGNET_PURCHASED = "Boost Coinmagnet purchased";

	public const string EVENT_BOOST_DOUBLEMULTIPLIER_PURCHASED = "Boost 2x multiplier purchased";

	public const string EVENT_BOOST_JETPACK_PURCHASED = "Boost jetpack purchased";

	public const string EVENT_BOOST_LETTERS_PURCHASED = "Boost letters purchased";

	public const string EVENT_BOOST_SUPERSNEAKERS_PURCHASED = "Boost supersneakers purchased";

	public const string EVENT_BOOST_MISSION_SKIP_PURCHASED = "Boost Mission Skip purchased";

	public const string EVENT_BOOST_MISSION_SKIP_INFO = "Boost Mission Skip info";

	public const string EVENT_BOOST_MYSTERYBOX_VIEW_PRIZES = "Mysterybox view prices";

	public const string EVENT_SUPER_MYSTERYBOX_VIEW_PRIZES = "Super Mystery Box prizes";

	public const string EVENT_MINI_MYSTERYBOX_VIEW_PRIZES = "Mini Mystery Box prizes";

	public const string EVENT_SAVE_ME_CLICKED = "Save Me clicked";

	public const string EVENT_SAVE_ME_SHOWN = "Save Me shown";

	public const string EVENT_RESUMED_AFTER_REVIVE_WITH_VIDEO = "Resumed run after revive with video";

	public const string EVENT_END_OF_RUN = "End of run";

	public const string EVENT_WEEKLY_CHALLENGE_COMPLETED = "Weekly Challenge Completed";

	public const string EVENT_WEEKLY_CHALLENGE_PERIOD_PROGRESS = "WeeklyHunt Progress";

	public const string EVENT_INTERSTITIAL_GAME_OVER_COUNTER = "Interstitial Game Over Counter";

	public const string EVENT_SHOP_MYSTERYBOX_PURCHASED = "Shop MysteryBox purchased";

	public const string EVENT_SHOP_DAILY_GIFT_COLLECTED = "Daily Gift Collected";

	public const string EVENT_AWARD_TIER_COMPLETED = "Award tier completed";

	public const string EVENT_AWARD_COMPLETED = "Award completed";

	public const string EVENT_AWARD_TIER_COMPLETED_TIME = "Award tier completed time";

	public const string EVENT_AWARD_TIER_COLLECTED_TIME = "Award tier collected time";

	public const string EVENT_AMOUNT_OF_DIAMOND_AWARDS_FINISHED = "Total amount of diamond awards finished";

	public const string EVENT_AMOUNT_OF_GOLD_AWARDS_FINISHED = "Total amount of gold awards finished";

	public const string EVENT_AMOUNT_OF_SILVER_AWARDS_FINISHED = "Total amount of silver awards finished";

	public const string EVENT_ASSET_BUNDLE_FAILED_TO_CACHE = "Asset Bundle Failed to be Cached";

	public const string EVENT_ASSET_BUNDLE_FAILED_LOAD_FROM_CACHE = "Asset Bundle Failed to be loaded from Cache";

	public const string EVENT_ASSET_BUNDLE_FAILED_DOWNLOAD = "Asset Bundle Failed to Download";

	public const string EVENT_ASSET_BUNDLE_AVAILABILITY_CHECK_FAILED = "Asset Bundles Availability Check Failed";

	public const string EVENT_ASSET_BUNDLES_ALL_DOWNLOADED = "All Asset Bundle Downloads completed";

	public const string EVENT_CHALLENGES_SCREEN_PAGE_SHOWN = "Challenges screen shown";

	public const string EVENT_MISSIONS_VIDEO_BUTTON_FILL = "Mission video fill attempt";

	public const string EVENT_MISSIONS_VIDEO_BUTTON_PAUSE_BUTTON_FILL = "Mission video game pause fill attempt";

	public const string EVENT_PLAYER_BIRTHDAY = "Player Birthday entered";

	public const string EVENT_CONSENT_GIVEN = "Consent given";

	public const string EVENT_CONSENT_WITHDRAWN = "Consent withdrawn";

	public const string EVENT_CONSENT_PRIVACY_POLICY_OPENED = "Consent Privacy Policy link opened";

	public const string EVENT_BIRTHDAY_BOARD_BOARD_SCREEN_SHOWED = "Birthday Board showed on Board screen";

	public const string EVENT_BIRTHDAY_BOARD_POPUP_SHOWED = "Birthday Board showed in popup";

	public const string EVENT_ONLINE_SETTINGS_PARSING_FAILED = "Error deserializing JSon OnlineSettings";

	public const string EVENT_MIGRATION_ERROR_PLAYERINFO = "PlayerInfo Migration error";

	public const string EVENT_MIGRATION_ERROR_PLAYERPREFS = "PlayerPrefs Migration error";

	public const string EVENT_MIGRATION_UNHANDLED_UPGRADES = "Migration unhandled upgrades";

	public const string EVENT_SAVE_DATA_LOAD_ERRORS = "SaveData load error";

	public const string EVENT_SAVE_DATA_SAVE_ERRORS = "SaveData save error";

	public const string EVENT_SOFT_CURRENCY_SPENT = "Soft currency spent";

	public const string EVENT_SOFT_CURRENCY_EARNED = "Soft currency earned";

	public const string EVENT_FRONT_SCREEN_BUTTON_WITH_FOOTER_CLICKED = "FrontScreenButtonWithFooterClicked";

	public const string EVENT_FOOTER_BUTTON_CLICKED = "FooterButtonClicked";

	public const string EVENT_CONSUMABLE_VIDEO_FILL_ATTEMPT = "Consumable video fill attempt";

	public const string EVENT_DOUBLE_COINS_VIDEO_ATTEMPT = "DoubleCoinsVideoAttempt";

	public const string EVENT_DOUBLE_COINS_VIDEO_RESULT = "DoubleCoinsVideoResult";

	public const string EVENT_TOKEN_BOX_OPENED = "Character Token Box Opened";

	public const string EVENT_TOKEN_OBTAINED = "Character Token Obtained";

	public const string EVENT_TOKEN_VIEW_PRIZES_POPUP = "Character token box view prices";

	public const string EVENT_TOKEN_GET_TOKEN_BOX_POPUP = "Character token pop-up";

	public const string EVENT_RESULT_OF_AD_SHOWN = "Result of ad shown";

	public const string EVENT_ATTEMPT_TO_SHOW_BUTTON = "Attempt to show button";

	public const string EVENT_VIDEO_AD_BUTTON_CLICK = "{0}_generic";

	public const string EVENT_ATTEMPT_TO_CACHE_RESULT = "AttemptToCacheResult";

	public const string EVENT_ATTEMPT_TO_SHOW_AD = "Attempt to show ad";

	public const string EVENT_WELCOME_BACK_POPUP = "WelcomeBackPopUp";

	public const string BILLBOARD_FILL_RATE = "Billboard fill rate";

	public const string EVENT_ARGKEY_HAS_FILL = "HasFill";

	public const string EVENT_ARGKEY_DOUBLE_COINS_VIDEO_DID_SHOW_BANNER = "WasBannerShown";

	public const string EVENT_ARGKEY_DOUBLE_COINS_VIDEO_CLICKED = "IsDoubleCoinsVideoClicked";

	public const string EVENT_ARGKEY_DOUBLE_COINS_VIDEO_PAYOUT_AMOUNT = "VideoPayoutAmount";

	public const string EVENT_ARGKEY_DOUBLE_COINS_VIDEO_CHARGE = "IsCharged";

	public const string EVENT_ARGKEY_DOUBLE_COINS_VIDEO_WAS_SHOWN = "ElementShown";

	public const string EVENT_LEADERBOARD_POPUP_CLOSED = "Highscore pop up";

	public const string EVENT_ARGKEY_LEADERBOARD_POPUP_FACEBOOK_STATUS = "FacebookStatus";

	public const string EVENT_ARGKEY_LEADERBOARD_POPUP_GAMECENTER_STATUS = "GamecenterStatus";

	public const string EVENT_ARGKEY_ID = "Id";

	public const string EVENT_ARGKEY_TIER = "Tier";

	public const string EVENT_ARGKEY_UI_SCREENNAME = "Screen Name";

	public const string EVENT_ARGKEY_MISSIONSET = "Mission Set";

	public const string EVENT_ARGKEY_MISSIONSET_AND_INDEX = "Mission Set and Index";

	public const string EVENT_ARGKEY_MISSION_SKIPPED_FROM = "Mission Skipped From";

	public const string EVENT_ARGKEY_MISSION_CAN_SKIP_FROM_PAUSE = "Can skip from pause";

	public const string EVENT_ARGKEY_MISSION_SKIP_WITH_VIDEO = "Video";

	public const string EVENT_ARGKEY_TOTAL = "Total";

	public const string EVENT_ARGKEY_POPUPRESULT = "Result";

	public const string EVENT_ARGKEY_FILENAME = "Filename";

	public const string EVENT_ARGKEY_VIDEOADRESULT = "Result";

	public const string EVENT_ARGKEY_ITEM_IAP = "Item tiggered iap";

	public const string EVENT_ARGKEYS_SOCIAL_FRIENDS_RETRIEVED = "Total friends;Installed friends";

	public const string EVENT_ARGKEYS_SOCIAL_FRIENDS_CONSOLIDATED = "Total;Facebook;GameCenter";

	public const string EVENT_ARGKEY_IS_FROM_DAILY_CHALLENGE = "SMB from daily challenge";

	public const string EVENT_ARGKEY_RATE_ASKAGAIN = "Should ask again";

	public const string EVENT_ARGKEY_AMOUNT = "Amount";

	public const string EVENT_ARGKEY_NOTIFICATIONS_ENABLED = "Enabled";

	public const string EVENT_ARGKEY_APPLICATION_AUTHENTICITY = "Type";

	public const string EVENT_ARGKEY_RESULT = "Result";

	public const string EVENT_ARGKEY_STATE = "State";

	public const string EVENT_ARGKEY_STATUS = "Status";

	public const string EVENT_ARGKEY_VALUE = "Value";

	public const string EVENT_ARGKEY_AWARD = "Award";

	public const string EVENT_ARGKEY_TIME = "Time";

	public const string EVENT_ARGKEY_CHARACTER_NAME_ACTION_PRICE = "Price";

	public const string EVENT_ARGKEY_CHARACTER_NAME_CHARACTER = "Character";

	public const string EVENT_ARGKEY_CHARACTER_NAME_SKIPPED_WITH_AD = "Watched ad";

	public const string EVENT_ARGKEY_GPU_NAME = "GPU name";

	public const string EVENT_ARGKEY_PERFOMRNACE_LEVEL = "Level";

	public const string EVENT_ARGKEY_FPS_RANGE = "FPS range {0}";

	public const string EVENT_ARGKEY_ITEMS_OWNED_REGULAR = "regular";

	public const string EVENT_ARGKEY_ITEMS_OWNED_LIMITED = "limited";

	public const string EVENT_ARGKEY_ITEMS_OWNED_TOKEN = "token";

	public const string EVENT_ARGKEY_ITEMS_OWNED_FACEBOOK = "facebook";

	public const string EVENT_ARGKEY_ITEMS_OWNED_TOTAL = "total";

	public const string EVENT_ARGKEY_ACTIVATED_BOOST_SUCCESSFULLY = "successfully";

	public const string EVENT_ARGKEY_AWARDS_DIAMOND_COMPLETED = "Completed Diamond Awards";

	public const string EVENT_ARGKEY_AWARDS_GOLD_COMPLETED = "Completed Gold Awards";

	public const string EVENT_ARGKEY_AWARDS_SILVER_COMPLETED = "Completed Silver Awards";

	public const string EVENT_ARGKEY_COUNTRY = "Country";

	public const string EVENT_ARGKEY_ONLINE_COUNTRY = "Online Country";

	public const string EVENT_ARGKEY_OFFLINE_COUNTRY = "Offline Country";

	public const string EVENT_ARGKEY_LIMITED_CHARACTER_POPUP_CHARACTER_BOUGHT = "Character {0} bought";

	public const string EVENT_ARGKEY_LIMITED_CHARACTER_POPUP_OUTFIT_BOUGHT = "{0}_{1} bought";

	public const string EVENT_ARGKEY_ASSET_BUNDLE_NAME = "Asset Bundle Name";

	public const string EVENT_ARGKEY_ASSET_BUNDLE_VERSION = "Asset Bundle Version";

	public const string EVENT_ARGKEY_ASSET_BUNDLE_DOWNLOAD_ERROR = "Asset Bundle Download Error";

	public const string EVENT_ARGKEY_ALL_ASSET_BUNDLES_DOWNLOADED = "All Bundles Downloaded?";

	public const string EVENT_ARGKEY_ALL_ASSET_BUNDLES_DOWNLOADED_COUNTRY = "All Bundles Downloaded {0} in Country";

	public const string EVENT_ARGKEY_LIMITED_BOARD_POPUP_BOARD_BOUGHT = "Board {0} bought";

	public const string EVENT_ARGKEY_LIMITED_BOARD_POPUP_OUTFIT_BOUGHT = "{0}_{1} bought";

	public const string EVENT_ARGKEY_DAILT_BUNDLEIDENTIFIER = "Daily bundle identifier";

	public const string EVENT_ARGKEY_UNIX_TIME_IN_MILLIS = "Unix time in millis";

	public const string EVENT_ARGKEY_NATIVE_DOWNLOAD_KEY = "ad_download";

	public const string EVENT_ARGKEY_NATIVE_TEXT_KEY_FORMAT = "text_{0}";

	public const string EVENT_ARGKEY_NATIVE_IMAGE_KEY_FORMAT = "image_{0}";

	public const string EVENT_ARGKEY_HEADSTART = "Headstart";

	public const string EVENT_ARGKEY_SCOREBOOST = "Scoreboost";

	public const string EVENT_ARGKEY_REVIVE = "Revive";

	public const string EVENT_ARGKEY_PLAYER_BIRTHDAY = "Player birthday";

	public const string EVENT_ARGKEY_ABTEST_IDENTIFIER = "Identifier";

	public const string EVENT_ARGKEY_ABTEST_PAYLOAD = "Payload";

	public const string EVENT_ARGKEY_ABTEST_TAG = "Tag";

	public const string EVENT_ARGKEY_DEVICE_LOCALE = "Locale";

	public const string EVENT_ARGKEY_INAPP_ID = "productID";

	public const string EVENT_ARGKEY_INAPP_SCREEN = "screen";

	public const string EVENT_ARGKEY_STORE_VISITS = "store_visits";

	public const string EVENT_ARGKEY_EARN_COINS_VISITS = "earn_coins_visits";

	public const string EVENT_ARGKEY_BOOST_VISITS = "boosts_visits";

	public const string EVENT_ARGKEY_QUICK_STORE_VISITS = "quick_store_visits";

	public const string EVENT_ARGKEY_SHOW_SOURCE = "show_source";

	public const string EVENT_ARGKEY_SHOW_OUTCOME = "show_outcome";

	public const string EVENT_ARGKEY_FACEBOOK_LOGIN_SOURCE = "Facebook Login Source";

	public const string EVENT_ARGKEY_FIRST_VALID_PURCHASE_PRODUCT_ID = "Product ID";

	public const string EVENT_ARGKEY_PROVIDER = "Provider";

	public const string EVENT_ARGKEY_LOCATION = "Location";

	public const string EVENT_ARGKEY_REWARD = "Reward";

	public const string EVENT_ARGKEY_BIRTHDAY_BOARD_RETRIES = "Number of retries";

	public const string EVENT_ARGKEY_BIRTHDAY_BOARD_DID_WATCH = "Did watch ad";

	public const string EVENT_ARGKEY_MESSAGE = "Message";

	public const string EVENT_ARGKEY_TYPE = "Type";

	public const string EVENT_ARGKEY_JOBID = "JobID";

	public const string EVENT_ARGKEY_VIDEO_NOT_SHOWN = "NotShown";

	public const string EVENT_ARGKEY_VIDEO_AVAILABLE = "Available";

	public const string EVENT_ARGKEY_VIDEO_NO_FILL = "NoFill";

	public const string EVENT_ARGKEY_VIDEO_FILL = "Fill";

	public const string EVENT_ARGKEY_AD_LOCATION_EVENT_OUTCOME = "Outcome";

	public const string EVENT_ARG_AD_LOCATION_EVENT_OUTCOME = "{0};{1}";

	public const string EVENT_ARGKEY_SUCCESS = "Success";

	public const string EVENT_ARGKEY_FAILED = "Failed";

	public const string EVENT_ARGKEY_TIMED_OUT = "Timedout";

	public const string EVENT_ARGKEY_NO_FILL_CACHE_TIMEOUT = "NoFillCacheTimeout";

	public const string EVENT_ARGKEY_FAILED_SHOWING_AD = "FailedShowingAd";

	public const string EVENT_ARGKEY_SHOWED_AD = "ShowedAd";

	public const string EVENT_ARGKEY_ALREADY_CACHED = "AlreadyCached";

	public const string EVENT_ARGKEY_NO_FILL_BUTTON_TIMEOUT = "NoFillButtonTimeout";

	public const string EVENT_ARGKEY_NO_FILL_END_OF_WATERFALL = "NoFillEndOfWaterfall";

	public const string EVENT_ARGKEY_NO_CACHED_AD = "No cached ad";

	public const string EVENT_ARGKEY_NO_CLICK_INFO = "NoClickInfo";

	public const string EVENT_ARGKEY_EARLY = "Early";

	public const string EVENT_ARGKEY_FINISHED = "Finished";

	public const string EVENT_ARGKEY_CLICK = "Click";

	public const string EVENT_ARGKEY_NOT_CURRENT = "NotCurrent";

	public const string EVENT_ARGKEY_VIDEO_STATUS = "Video status";

	public const string EVENT_ARGKEY_SAVE_ME_POPUP_COUNT = "Count";

	public const string EVENT_ARGKEY_KEYS_NO_VIDEO = "KeysNoVideo";

	public const string EVENT_ARGKEY_KEYS_VIDEO = "KeysVideo";

	public const string EVENT_ARGKEY_VIDEO = "Video";

	public const string EVENT_ARGKEY_ACTION = "Action";

	public const string EVENT_ARGKEY_NUMBER_OF_REVIVES = "Number of revives";

	public const string EVENT_ARGKEY_USED_VIDEO = "Used video";

	public const string EVENT_ARGKEY_NUMBER_OF_KEYS_SPENT = "Keys Spent";

	public const string EVENT_ARGKEY_REVIVE_VIDEO_NOT_AVAILABLE = "Video not available";

	public const string EVENT_ARGKEY_SOURCE_OF_PURCHASE = "source_of_purchase";

	public const string EVENT_ARGKEY_BOUGHT_IN_RUN = "Bought in run";

	public const string EVENT_ARGKEY_ONLINE_SETTINGS_PARSING_FAILED_EXCEPTION = "Exception thrown deserializing JSon OnlineSettings";

	public const string EVENT_ARGKEY_SHOW_COINS_VIDEO_RESULT = "Result";

	public const string EVENT_ARG_SHOW_COINS_VIDEO_FILL = "Fill;";

	public const string EVENT_ARG_SHOW_COINS_VIDEO_NO_FILL = "No_Fill;";

	public const string EVENT_ARG_VIDEO_COINS_AVAILABLE = "Video_Available";

	public const string EVENT_ARG_POPUP_VIDEO_COINS_CLICKED = "Button_Name";

	public const string EVENT_ARG_POPUP_VIDEO_COINS_REWARD = "Video_Reward";

	public const string EVENT_ARG_POPUP_VIDEO_COINS_COOLDOWN = "Cooldown_step";

	public const string EVENT_ARGKEY_MEMORY_WARNING_DEVICE_TYPE = "DeviceType";

	public const string EVENT_ARGKEY_MEMORY_WARNING_TIME = "TimeSinceStartup(m)";

	public const string EVENT_ARGKEY_MEMORY_WARNING_UI_AMOUNT = "Screen/Popups";

	public const string EVENT_ARGKEY_MEMORY_WARNING_RUNS = "Runs";

	public const string EVENT_ARGKEY_MEMORY_WARNING_RESULT = "WarningOccured";

	public const string EVENT_ARGKEY_GAMEOVER_TAB_CLICKED = "TabClicked";

	public const string EVENT_ARGKEY_GAMEOVER_BUTTON_CLICKED = "ButtonClicked";

	public const string EVENT_ARGKEY_HAS_AD = "HasAd";

	public const string EVENT_ARGKEY_AD_OPTIONS = "AdOptions";

	public const string EVENT_ARGKEY_NUMBER_OF_AD_REQUESTS = "Number of ad requests";

	public const string EVENT_ARGKEY_NUMBER_OF_REQUEST_SUCCESS = "Number of request success";

	public const string EVENT_ARGKEY_RUN_LENGTH = "Run length";

	public const string EVENT_ARGKEY_NUMBER_OF_ADS_SHOWN = "Number of ads shown";

	public const string EVENT_ARGKEY_RUN_DURATION = "Run duration";

	public const string EVENT_ARG_PROMO_CODE = "Promo_code";

	public const string EVENT_ARG_CODE_TYPE = "CodeType";

	public const string EVENT_ARG_CHARACTERS = "Characters";

	public const string EVENT_ARG_BOARDS = "Boards";

	public const string EVENT_ARG_COINS = "Coins";

	public const string EVENT_ARG_KEYS = "Keys";

	public const string EVENT_ARG_RESULT = "Result";

	public const string EVENT_ARG_CONFIRMED = "Confirmed";

	public const string EVENT_ARG_NOT_CONFIRMED = "NotConfirmed";

	public const string EVENT_ARGKEY_PROMO_CODE_ERROR_TYPE_CLIENT = "ClientError";

	public const string EVENT_ARGKEY_PROMO_CODE_ERROR_TYPE_SERVER = "ServerError";

	public const string EVENT_ARG_PROMO_CODE_NEED_UPDATE = "Need to update";

	public const string EVENT_ARG_PROMO_CODE_ALREADY_USED = "Code already used";

	public const string EVENT_ARG_PROMO_CODE_PAYLOAD_ERROR = "Error in payload";

	public const string EVENT_ARG_PROMO_CODE_GET_CODE_TIMEOUT = "Get payload timeout";

	public const string EVENT_ARG_PROMO_CODE_CONTENT_OWNED = "Content already owned";

	public const string EVENT_ARGKEY_POPUPRESULT_OK = "Ok";

	public const string EVENT_ARGKEY_POPUPRESULT_CANCEL = "Cancel";

	public const string EVENT_ARGKEY_ASKRATE = "Ask again";

	public const string EVENT_ARGKEY_DONTASKRATE = "Do not ask again";

	public const string EVENT_ARGKEY_PERFORMANCE_LEVEL_LOW = "Low";

	public const string EVENT_ARGKEY_PERFORMANCE_LEVEL_HIGH = "High";

	public const string EVENT_ARGKEY_GPU_NAME_WITH_VALUE = "GPU name {0}";

	public const string EVENT_ARG_YES = "yes";

	public const string EVENT_ARG_NO = "no";

	public const string EVENT_ARGKEY_DOUBLE_UP_BUTTON_CHANCE = "Chance";

	public const string EVENT_ARGKEY_DOUBLE_UP_BUTTON_FILL = "Fill";

	public const string EVENT_ARGKEY_DOUBLE_UP_BUTTON_NO_CHANCE = "No Chance";

	public const string EVENT_ARGKEY_DOUBLE_UP_BUTTON_NO_FILL = "No Fill";

	public const string EVENT_ARGKEY_DOUBLE_UP_OPTION_RESULT_DOUBLED = "Doubled";

	public const string EVENT_ARGKEY_DOUBLE_UP_OPTION_RESULT_NOT_DOUBLED = "Not Doubled";

	public const string EVENT_ARGKEY_DOUBLE_UP_OPTION_RESULT_TRIPLE = "Tripled";

	public const string EVENT_ARGKEY_DOUBLE_UP_OPTION_RESULT_NOT_TRIPLE = "Not Tripled";

	public const string EVENT_ARGKEY_DOUBLE_UP_OPTION_RESULT_QUADRUPLED = "Quadrupled";

	public const string EVENT_ARGKEY_DOUBLE_UP_OPTION_RESULT_NOT_QUADRUPLED = "Not Quadrupled";

	public const string EVENT_ARGKEY_NATIVE_DOWNLOAD_VALUE = "ad download failed";

	public const string EVENT_ARGKEY_NATIVE_TEXT_VALUE_FORMAT = "total: {0} after: {1} - country:{2}";

	public const string EVENT_ARGKEY_NATIVE_IMAGE_VALUE_FORMAT = "ratio: {0} - country:{1}";

	public const string EVENT_ARGKEY_NATIVE_IMAGE_NAME_COVER = "Cover";

	public const string EVENT_ARGKEY_NATIVE_IMAGE_NAME_ICON = "Icon";

	public const string EVENT_ARGKEY_NATIVE_TEXT_NAME_TITLE = "Title";

	public const string EVENT_ARGKEY_NATIVE_TEXT_NAME_BODY = "Body";

	public const string EVENT_ARGKEY_NATIVE_TEXT_NAME_CTA = "Call To Action";

	public const string EVENT_ARGKEY_PLAYER_BIRTHDAY_VALUE = "{0}-{1}";

	public const string EVENT_ARGKEY_CONSENT_WITHDRAWN_RESULT = "Result";

	public const string EVENT_ARGKEY_MISSION_RUNS = "Runs completed";

	public const string EVENT_ARGKEY_TUTORIAL_SCREEN_FLOW = "Screen order";

	public const string EVENT_ARGKEY_SKIPPED_MISSIONS = "Skipped missions";

	public const string EVENT_ARGKEY_MOVED_TO_STORE = "Moved to store";

	public const string EVENT_ARGKEY_CLOSED = "Closed";

	public const string EVENT_ARGKEY_PURCHASED = "Purchased";

	public const string EVENT_ARGKEY_BUTTON_NAME = "button_name";

	public const string EVENT_ARGKEY_BOUGHT_WITH_KEYS = "bought using keys";

	public const string EVENT_ARGKEY_NOT_ENOUGH_KEYS = "not enough keys";

	public const string EVENT_ARGKEY_BOUGHT_WITH_IAP = "bought using iap";

	public const string EVENT_ARGKEY_BOUGHT_WITH_IAP_FAIL = "bought using iap fail";

	public const string EVENT_ARGKEY_BOUGHT_WITH_COINS = "bought using coins";

	public const string EVENT_ARGKEY_NOT_ENOUGH_COINS = "not enough coins";

	public const string EVENT_ARGKEYS_NO_REWARD = "No Reward";

	public const string EVENT_ARGKEY_OFFERWALL_ORIGIN = "Origin";

	public const string EVENT_ARGKEY_OFFERWALL_RESULT = "Result";

	public const string EVENT_ARG_OFFERWALL_RESULT_SHOWN = "Shown";

	public const string EVENT_ARG_OFFERWALL_RESULT_NOT_SHOWN = "Not shown";

	public const string EVENT_ARGKEY_OFFERWALL_RESULT_FAILED = "Failed";

	public const string EVENT_ARGKEY_MIGRATION_ERROR_TYPE = "Exception Type";

	public const string EVENT_ARGKEY_MIGRATION_ERROR_LAST_KEY = "Last loaded key";

	public const string EVENT_ARGKEY_SAVE_DATA_ERROR_TYPE = "Exception Type";

	public const string EVENT_ARGKEY_MIGRATION_SOURCE_VERSION = "Migrated from";

	public const string EVENT_ARGKEY_MIGRATION_UNHANDLED_UPGRADES = "Unhandled powerup";

	public const string EVENT_ARGKEY_CURRENCY_SOURCE_OFFERWALL = "Offerwall";

	public const string EVENT_ARGKEY_CURRENCY_SOURCE_MYSTERYBOX = "MysteryBox";

	public const string EVENT_ARGKEY_CURRENCY_SOURCE_INRUN = "InRun";

	public const string EVENT_ARGKEY_CURRENCY_SOURCE_FACEBOOK_GIFT = "FacebookGift";

	public const string EVENT_ARGKEY_CURRENCY_SOURCE_REVIVE = "Revive";

	public const string EVENT_ARGKEY_CURRENCY_SOURCE_WEEKLY_HUNT = "WeeklyHunt";

	public const string EVENT_ARGKEY_CURRENCY_SOURCE_FRIEND_REWARD = "FriendReward";

	public const string EVENT_ARGKEY_CURRENCY_SOURCE_OPEN_URL = "OpenUrl";

	public const string EVENT_ARGKEY_CURRENCY_SOURCE_MISSION_SET = "MissionSet";

	public const string EVENT_ARGKEY_CURRENCY_SOURCE_PLAYER_INFO_MIGRATION = "PlayerInfoMigration";

	public const string EVENT_ARGKEY_CURRENCY_SOURCE_DOUBLECOINS_VIDEO = "DoubleCoinsVideo";

	public const string EVENT_ARGKEY_CURRENCY_SOURCE_SKIPMISSION = "SkipMission";

	public const string EVENT_ARGKEY_CURRENCY_SOURCE_CHARACTERNAMESKIP = "WordyWeekendSkip";

	public const string EVENT_ARGKEY_CURRENCY_ID = "currency_id";

	public const string EVENT_ARGKEY_ITEM_ID = "item_id";

	public const string EVENT_ARGKEY_SOURCE_ID = "source_id";

	public const string EVENT_ARGKEY_SECOND_ITEM_ID = "item_id_2";

	public const string EVENT_ARGKEY_TOKEN_SCREEN = "ScreenName";

	public const string EVENT_ARGKEY_TOKEN_INFO = "TokenInfo";

	public const string EVENT_ARGKEY_TOKEN_SOURCE = "Source";

	public const string EVENT_ARGKEY_TOKEN_BOXCOST = "BoxCost";

	public const string EVENT_CLOUDSAVE_UPDATE = "CloudSave Update";

	public const string EVENT_CLOUDSAVE_COMMIT = "CloudSave Commit";

	public const string EVENT_CLOUDSAVE_ENABLED = "CloudSave Enabled";

	public const string EVENT_CLOUDSAVE_DISCONNECTED = "CloudSave Disconnected";

	public const string EVENT_CLOUDSAVE_CONFLICT_RESOLVED = "CloudSave Conflict Resolved";

	public const string EVENT_CLOUDSAVE_STATUS_POPUP = "CloudSave Status Popup";

	public const string EVENT_CLOUDSAVE_INTRODUCTION_POPUP = "CloudSave Introduction Popup";

	public const string EVENT_CLOUDSAVE_REMINDER_POPUP = "CloudSave Reminder Popup";

	public const string EVENT_CLOUDSAVE_ADVANCED_COMPARISON_POPUP = "CloudSave Advanced Comparison Popup";

	public const string EVENT_CLOUDSAVE_CONFIRMATION_POPUP = "CloudSave Confirmation Popup";

	public const string EVENT_NATIVE_LOGIN = "Native Login";

	public const string EVENT_CLOUDSAVE_UPDATE_ERROR = "CloudSave Update error";

	public const string EVENT_CLOUDSAVE_UPDATE_METADATA_ERROR = "CloudSave UpdateMetadata error";

	public const string EVENT_CLOUDSAVE_ASYNC_COMMIT_ERROR = "CloudSave Commit Async error";

	public const string EVENT_CLOUDSAVE_COMMIT_ERROR = "CloudSave Commit error";

	public const string EVENT_PROGRESS_LOSS_PARTIAL = "Progress Loss Partial";

	public const string EVENT_PROGRESS_LOSS_FULL = "Progress Loss Full";

	public const string EVENT_PROGRESS_LOSS_OLD_CLOUD_STATE = "Cloud State Old";

	public const string EVENT_PROGRESS_LOSS_SAVE_FILE_CORRUPT = "Corrupted File";

	public const string EVENT_PROGRESS_LOSS_NEW_CLOUD_STATE = "Cloud State New";

	public const string EVENT_PROGRESS_LOSS_DID_ACCOUNT_SWITCH = "Did account switch";

	public const string EVENT_PROGRESS_LOSS_MIGRATION_DONE = "Migration done";

	public const string EVENT_PROGRESS_LOSS_APP_REFRESH = "App Refreshed";

	public const string EVENT_ARGKEY_COMPLETED_LOGIN = "CompletedLogin";

	public const string EVENT_ARGKEY_INTRODUCTION_CLOSE_CLICKED = "CloseClicked";

	public const string EVENT_ARGKEY_INTRODUCTION_SHOWN_COUNT = "ShownCount";

	public const string EVENT_ARGKEY_REMINDER_STATE_WHEN_SHOWN = "State";

	public const string EVENT_ARGKEY_KILOOTAG = "KilooTag";

	public const string EVENT_ARGKEY_SUCCESS_TIME = "SuccessTime";

	public const string EVENT_ARGKEY_FAIL_TIME = "FailTime";

	public const string EVENT_ARGKEY_UPDATE_CONFLICT_TIME = "ConflictTime";

	public const string EVENT_ARGKEY_COMMIT_STARTED = "Started";

	public const string EVENT_ARGKEY_COMMIT_ASYNC_STARTED = "AsyncStarted";

	public const string EVENT_ARGKEY_SUCCESS_ASYNC_TIME = "AsyncSuccessTime";

	public const string EVENT_ARGKEY_FAIL_ASYNC_TIME = "AsyncFailTime";

	public const string EVENT_ARG_BUTTON_CLICKED = "ButtonClicked";

	public const string EVENT_ARG_FROM_DISABLED_POPUP = "FromDisabledPopup";

	public const string EVENT_ARG_FROM_INTRODUCTION_POPUP = "FromIntroductionPopup";

	public const string EVENT_ARG_FROM_STATUS_POPUP = "FromStatusPopup";

	public const string EVENT_ARG_FROM_REMINDER_POPUP = "FromReminderPopup";

	public const string EVENT_ARG_FROM_SETTINGS_POPUP = "FromSettingsPopup";

	public const string EVENT_ARG_FROM_PROMOTION_POPUP = "FromPromotionPopup";

	public const string EVENT_ARGKEY_FROM_COMPARISON_POPUP = "FromComparisonPopup";

	public const string EVENT_ARGKEY_NATIVE_LOGGED_IN = "NativeLoggedIn";

	public const string EVENT_ARGKEY_NATIVE_LOGGED_OUT = "NativeLoggedOut";

	public const string EVENT_ARGKEY_SOURCE = "Source";

	public const string EVENT_ARGKEY_DISCONNECT = "Disconnect";

	public const string EVENT_ARGKEY_REMINDER_CLOSE = "Close";

	public const string EVENT_ARGKEY_REMINDER_FIX = "Fix";

	public const string EVENT_ARG_CONFIRMATION_KEEP_LOCAL = "KeepLocal";

	public const string EVENT_ARG_CONFIRMATION_KEEP_CLOUD = "KeepCloud";

	public const string EVENT_ARG_CONFIRMATION_LOAD_PROGRESS = "LoadProgress";

	public const string EVENT_ARGKEY_CONFIRMATION_STATE = "state";

	public const string EVENT_ARG_CONFIRMATION_GOOD = "Good";

	public const string EVENT_ARG_CONFIRMATION_BAD = "Bad";

	public const string EVENT_ARGKEY_CONSUMABLE_VIDEO_STATE = "video status";

	public const string EVENT_ARGKEY_CONSUMABLE_TYPE = "consumable";

	public const string EVENT_ARGKEY_CLOUDSAVE_ERROR_MSG = "error_message";

	public const string EVENT_ARG_CONSUMABLE_VIDEO_STATE_FILL = "Fill-{0}";

	public const string EVENT_ARG_CONSUMABLE_VIDEO_STATE_NOFILL = "NoFill-{0}";

	public const string EVENT_ARG_FROM_COMPARISON_RECOMMENDING_THEIRS = "FromComparisonRecommendingTheirs";

	public const string EVENT_ARG_FROM_COMPARISON_RECOMMENDING_MINE = "FromComparisonRecommendingMine";

	public const string EVENT_ARG_FROM_COMPARISON_RECOMMENDING_NONE = "FromComparisonRecommendingNone";

	public const string EVENT_ARG_AUTO_RESOLVED = "AutoResolved";

	public const string EVENT_ARG_USING_THEIRS = "UsingTheirs";

	public const string EVENT_ARG_USING_MINE = "UsingMine";

	public const string EVENT_FREESTUFF_QUICKVIDEO_BUTTON_SHOWN = "Free Stuff Quick Video Ads Button Shown";

	public const string EVENT_ARG_FREESTUFF_QUICKVIDEO_VALUE_FORMAT = "step{0}|sinceDailyKeys{1}|{2}";

	public const string EVENT_ARG_FREESTUFF_QUICKVIDEO_SINCE_DAILY_KEYS = "SinceDailyKeys";

	public const string EVENT_FREE_STUFF_VIDEO_BUTTON = "Free Stuff Video Button";

	public const string EVENT_ARG_FREE_STUFF_VIDEO_BUTTON_SHOWN = "Shown";

	public const string EVENT_ARG_FREE_STUFF_VIDEO_BUTTON_CLICKED = "Clicked";

	public const string EVENT_ARG_FREE_STUFF_VIDEO_BUTTON_NO_FILL = "NoFill";

	public const string EVENT_ARG_HOVERBOARD_NAME = "Hoverboards";

	public const string EVENT_ARG_COINS_NAME = "Coins";

	public const string EVENT_ARG_SCORE_BOOSTER_NAME = "Score Booster";

	public const string EVENT_ARG_HEADSTART_NAME = "Headstart";

	private const string PLAYER_PREFS_FLURRY_USER_ID_KEY = "FLURRY_USER_ID_KEY";

	private static readonly int[][] FRIEND_BRACKETS = new int[5][]
	{
		new int[3] { -1, 10, 1 },
		new int[3] { 11, 30, 5 },
		new int[3] { 31, 100, 10 },
		new int[3] { 101, 1000, 100 },
		new int[3] { 1001, 999999, 1000 }
	};

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<string> onCloudSaveSend__BackingField;

	private static char[] PARAMETER_SPLITTERS = new char[1] { ';' };

	private const string VALIDATION_SUFFIX = " VALIDATED";

	public static string flurryUserId
	{
		get
		{
			if (!PlayerPrefs.HasKey("FLURRY_USER_ID_KEY"))
			{
				PlayerPrefs.SetString("FLURRY_USER_ID_KEY", Guid.NewGuid().ToString());
				PlayerPrefs.Save();
			}
			return PlayerPrefs.GetString("FLURRY_USER_ID_KEY");
		}
		set
		{
			PlayerPrefs.SetString("FLURRY_USER_ID_KEY", value);
			PlayerPrefs.Save();
		}
	}

	public static bool wasInitialized
	{
		get
		{
			return FlurryAnalytics.wasInitialized;
		}
	}

	public static event Action<string> onCloudSaveSend
	{
		add
		{
			Action<string> action = onCloudSaveSend__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onCloudSaveSend__BackingField, (Action<string>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<string> action = onCloudSaveSend__BackingField;
			Action<string> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onCloudSaveSend__BackingField, (Action<string>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static void LogGameCenterLogin()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer && !AnalyticsSave.hasLoggedGameCenterLogin)
		{
			LogEvent("First GameCenter Login");
			AnalyticsSave.hasLoggedGameCenterLogin = true;
		}
	}

	public static void LogFacebookLogin(string facebookLoginSource)
	{
		if (!AnalyticsSave.hasLoggedFacebookLogin)
		{
			LogEventWithAParameter("First Facebook Login", "Facebook Login Source", facebookLoginSource);
			AnalyticsSave.hasLoggedFacebookLogin = true;
		}
	}

	public static void LogOkPressedOnPopup(string eventName)
	{
		if (eventName != string.Empty)
		{
			LogEventWithAParameter(eventName, "Result", "Ok");
		}
	}

	public static void LogClosePressedOnPopup(string eventName)
	{
		if (eventName != string.Empty)
		{
			LogEventWithAParameter(eventName, "Result", "Cancel");
		}
	}

	public static string ConvertFriendCountToBracket(int count)
	{
		for (int i = 0; i < FRIEND_BRACKETS.Length; i++)
		{
			int num = FRIEND_BRACKETS[i][0];
			int num2 = FRIEND_BRACKETS[i][1];
			int num3 = FRIEND_BRACKETS[i][2];
			if (count < num || count > num2)
			{
				continue;
			}
			while (num <= num2)
			{
				int num4 = num + num3;
				if (count >= num && count < num4)
				{
					if (num3 == 1)
					{
						return count.ToString();
					}
					return num + "-" + (num4 - 1);
				}
				num = num4;
			}
		}
		return count.ToString();
	}

	public static bool ShouldLogDailyEvent(string eventName)
	{
		int dayOfYear = CDateTime.Now.DayOfYear;
		string key = "socflurdaily" + eventName;
		int value;
		if (AnalyticsSave.hasLoggedFlurryDailyEventSocial.TryGetValue(key, out value) && dayOfYear == value)
		{
			return false;
		}
		AnalyticsSave.hasLoggedFlurryDailyEventSocial.AddOrSet(key, dayOfYear);
		return true;
	}

	public static void StartSession(string apiKey)
	{
		FlurryAnalytics.SetContinueSessionMillis(10000L);
		FlurryAnalytics.SetUserId(GetFlurryUserIdHash());
		FlurryAnalytics.StartSession(apiKey);
	}

	public static string GetFlurryUserIdHash()
	{
		return StringUtility.ToHexString(Checksum.ComputeSHA256(flurryUserId));
	}

	public static void SetUserId()
	{
		FlurryAnalytics.SetUserId(GetFlurryUserIdHash());
	}

	public static void EndSession()
	{
		FlurryAnalytics.EndSession();
	}

	public static void LogEvent(string eventName)
	{
		if (!Globals.DEBUG_USE_DEBUG_LOGS_INSTEAD_OF_FLURRY)
		{
			FlurryAnalytics.LogEvent(eventName);
		}
	}

	public static void LogTimedEvent(string eventName)
	{
		if (!Globals.DEBUG_USE_DEBUG_LOGS_INSTEAD_OF_FLURRY)
		{
			FlurryAnalytics.LogTimedEvent(eventName);
		}
	}

	public static void EndTimedEvent(string eventName)
	{
		if (!Globals.DEBUG_USE_DEBUG_LOGS_INSTEAD_OF_FLURRY)
		{
			FlurryAnalytics.EndTimedEvent(eventName);
		}
	}

	public static void HandleFlurryUserIDBackup()
	{
		if (!AnalyticsSave.flurryUserId.IsNullOrEmpty() && AnalyticsSave.flurryUserId != flurryUserId)
		{
			Debug.Log("SyboUserId: " + AnalyticsSave.syboUserId);
			flurryUserId = AnalyticsSave.flurryUserId;
			string argument = string.Format("(1)HandleSyboUserIDBackup: {0} ", SyboAnalyticsSave.localSyboAnalyticsUserId + "/" + SyboAnalyticsSave.syboAnalyticsUserId + "/" + AnalyticsSave.syboUserId);
			onCloudSaveSend__BackingField.SafeInvoke(argument);
			SetUserId();
		}
		else if (AnalyticsSave.flurryUserId.IsNullOrEmpty())
		{
			AnalyticsSave.flurryUserId = flurryUserId;
			string argument2 = string.Format("(2)HandleSyboUserIDBackup: {0} ", SyboAnalyticsSave.localSyboAnalyticsUserId + "/" + SyboAnalyticsSave.syboAnalyticsUserId + "/" + AnalyticsSave.syboUserId);
			onCloudSaveSend__BackingField.SafeInvoke(argument2);
		}
	}

	public static void HandleSyboUserIDBackup()
	{
		if (!SyboAnalyticsSave.localSyboAnalyticsUserId.IsNullOrEmpty() && SyboAnalyticsSave.localSyboAnalyticsUserId != SyboAnalyticsSave.syboAnalyticsUserId)
		{
			flurryUserId = AnalyticsSave.flurryUserId;
			SetUserId();
		}
		else if (AnalyticsSave.syboUserId.IsNullOrEmpty())
		{
			AnalyticsSave.syboUserId = flurryUserId;
		}
	}

	public static void LogEventWithAParameter(string eventName, string argKey, string argValue)
	{
		if (!Globals.DEBUG_USE_DEBUG_LOGS_INSTEAD_OF_FLURRY)
		{
			FlurryAnalytics.LogEventWithParams(eventName, new Dictionary<string, string> { { argKey, argValue } });
		}
	}

	public static void LogEventWithSeveralParameters(string eventName, Dictionary<string, string> parameters)
	{
		if (!Globals.DEBUG_USE_DEBUG_LOGS_INSTEAD_OF_FLURRY)
		{
			FlurryAnalytics.LogEventWithParams(eventName, parameters);
		}
	}

	public static void LogEventWithSeveralParameters(string eventName, string argKeys, string argValues)
	{
		if (!Globals.DEBUG_USE_DEBUG_LOGS_INSTEAD_OF_FLURRY)
		{
			FlurryAnalytics.LogEventWithParams(eventName, ToDictionary(argKeys, argValues));
		}
	}

	private static Dictionary<string, string> ToDictionary(string argKeys, string argValues)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string[] array = argKeys.Split(PARAMETER_SPLITTERS);
		string[] array2 = argValues.Split(PARAMETER_SPLITTERS);
		for (int i = 0; i < array.Length && i < array2.Length; i++)
		{
			dictionary.Add(array[i], array2[i]);
		}
		return dictionary;
	}

	public static void LogInAppPurchase(string productId, string itemRequested, bool validationCallback = false)
	{
		string text = ((!validationCallback) ? string.Empty : " VALIDATED");
		LogEventWithAParameter("InApp purchase completed" + text, "Id", productId);
		switch (productId)
		{
		case "com.kiloo.subways.coinstier1":
		case "com.kiloo.subways.coinstier1discount":
		case "com.kiloo.subwaysurfers.coinstier1specialdiscount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				LogEventWithAParameter("InApp Coin Pack 1 purchased" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
			}
			else
			{
				LogEventWithSeveralParameters("InApp Coin Pack 1 purchased" + text, "Mission Set;Item tiggered iap", PlayerSave.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.coinstier2":
		case "com.kiloo.subways.coinstier2_discount":
		case "com.kiloo.subwaysurfers.coinstier2specialdiscount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				LogEventWithAParameter("InApp Coin Pack 2 purchased" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
			}
			else
			{
				LogEventWithSeveralParameters("InApp Coin Pack 2 purchased" + text, "Mission Set;Item tiggered iap", PlayerSave.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.coinstier2.5":
		case "com.kiloo.subways.coinstier2.5discount":
		case "com.kiloo.subwaysurfers.coinstier2.5specialdiscount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				LogEventWithAParameter("InApp Coin Pack 2.5 purchased" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
			}
			else
			{
				LogEventWithSeveralParameters("InApp Coin Pack 2.5 purchased" + text, "Mission Set;Item tiggered iap", PlayerSave.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.coinstier3":
		case "com.kiloo.subways.coinstier3discount":
		case "com.kiloo.subwaysurfers.coinstier3specialdiscount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				LogEventWithAParameter("InApp Coin Pack 3 purchased" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
			}
			else
			{
				LogEventWithSeveralParameters("InApp Coin Pack 3 purchased" + text, "Mission Set;Item tiggered iap", PlayerSave.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.coinstier4":
		case "com.kiloo.subways.coinstier4discount":
		case "com.kiloo.subwaysurfers.coinstier4specialdiscount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				LogEventWithAParameter("InApp Coin Pack 4 purchased" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
			}
			else
			{
				LogEventWithSeveralParameters("InApp Coin Pack 4 purchased" + text, "Mission Set;Item tiggered iap", PlayerSave.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.coinstier5":
		case "com.kiloo.subways.coinstier5discount":
		case "com.kiloo.subwaysurfers.coinstier5specialdiscount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				LogEventWithAParameter("InApp Coin Pack 5 purchased" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
			}
			else
			{
				LogEventWithSeveralParameters("InApp Coin Pack 5 purchased" + text, "Mission Set;Item tiggered iap", PlayerSave.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.keystier1":
		case "com.kiloo.subways.keystier1discount":
		case "com.kiloo.subwaysurfers.keystier1specialdiscount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				LogEventWithAParameter("InApp Key Pack 1 purchased" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
			}
			else
			{
				LogEventWithSeveralParameters("InApp Key Pack 1 purchased" + text, "Mission Set;Item tiggered iap", PlayerSave.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.keystier1.5":
		case "com.kiloo.subways.keystier1.5discount":
		case "com.kiloo.subwaysurfers.keystier1.5specialdiscount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				LogEventWithAParameter("InApp Key Pack 1.5 purchased" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
			}
			else
			{
				LogEventWithSeveralParameters("InApp Key Pack 1.5 purchased" + text, "Mission Set;Item tiggered iap", PlayerSave.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.keystier2":
		case "com.kiloo.subways.keystier2discount":
		case "com.kiloo.subwaysurfers.keystier2specialdiscount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				LogEventWithAParameter("InApp Key Pack 2 purchased" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
			}
			else
			{
				LogEventWithSeveralParameters("InApp Key Pack 2 purchased" + text, "Mission Set;Item tiggered iap", PlayerSave.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.keystier3":
		case "com.kiloo.subways.keystier3discount":
		case "com.kiloo.subwaysurfers.keystier3specialdiscount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				LogEventWithAParameter("InApp Key Pack 3 purchased" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
			}
			else
			{
				LogEventWithSeveralParameters("InApp Key Pack 3 purchased" + text, "Mission Set;Item tiggered iap", PlayerSave.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.keystier4":
		case "com.kiloo.subways.keystier4discount":
		case "com.kiloo.subwaysurfers.keystier4specialdiscount":
			if (string.IsNullOrEmpty(itemRequested))
			{
				LogEventWithAParameter("InApp Key Pack 4 purchased" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
			}
			else
			{
				LogEventWithSeveralParameters("InApp Key Pack 4 purchased" + text, "Mission Set;Item tiggered iap", PlayerSave.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.doublecoins":
		case "com.kiloo.subways.doublecoinsdiscount":
			if (UIScreenController.IsInstanced)
			{
				if (string.IsNullOrEmpty(UIScreenController.Instance.GetCurrentPopupName()))
				{
					LogEventWithAParameter("Double Coin purchased" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
				}
				else
				{
					LogEventWithAParameter("Double Coin purchased GameOver" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
				}
			}
			else
			{
				LogEventWithAParameter("Double Coin purchased" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
			}
			break;
		case "com.kiloo.subways.bundletier1":
			if (string.IsNullOrEmpty(itemRequested))
			{
				LogEventWithAParameter("InApp Bundle 1 purchased" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
			}
			else
			{
				LogEventWithSeveralParameters("InApp Bundle 1 purchased" + text, "Mission Set;Item tiggered iap", PlayerSave.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.bundletier2":
			if (string.IsNullOrEmpty(itemRequested))
			{
				LogEventWithAParameter("InApp Bundle 2 purchased" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
			}
			else
			{
				LogEventWithSeveralParameters("InApp Bundle 2 purchased" + text, "Mission Set;Item tiggered iap", PlayerSave.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.bundletier3":
			if (string.IsNullOrEmpty(itemRequested))
			{
				LogEventWithAParameter("InApp Bundle 3 purchased" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
			}
			else
			{
				LogEventWithSeveralParameters("InApp Bundle 3 purchased" + text, "Mission Set;Item tiggered iap", PlayerSave.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.bundletier4":
			if (string.IsNullOrEmpty(itemRequested))
			{
				LogEventWithAParameter("InApp Bundle 4 purchased" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
			}
			else
			{
				LogEventWithSeveralParameters("InApp Bundle 4 purchased" + text, "Mission Set;Item tiggered iap", PlayerSave.currentMissionSet + ";" + itemRequested);
			}
			break;
		case "com.kiloo.subways.bundletier5":
			if (string.IsNullOrEmpty(itemRequested))
			{
				LogEventWithAParameter("InApp Bundle 5 purchased" + text, "Mission Set", PlayerSave.currentMissionSet.ToString());
			}
			else
			{
				LogEventWithSeveralParameters("InApp Bundle 5 purchased" + text, "Mission Set;Item tiggered iap", PlayerSave.currentMissionSet + ";" + itemRequested);
			}
			break;
		}
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogError(string msg, UnityEngine.Object context = null)
	{
		SDebug.LogError("[Flurry] " + msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	private static void LogWarning(string msg, UnityEngine.Object context = null)
	{
	}

	[Conditional("ENABLE_FLURRY_LOGS")]
	private static void Log(string msg, UnityEngine.Object context = null)
	{
	}
}
