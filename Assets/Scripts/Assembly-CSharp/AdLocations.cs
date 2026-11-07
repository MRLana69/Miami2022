using System.Collections.Generic;
using Kiloo.Common.Ads;

public static class AdLocations
{
	public static readonly AdLocationInfo UNDETERMINED_LOCATION = NewAdLocation("undetermined", AdType.Interstitial);

	public static readonly AdLocationInfo HOME_INTERSTITIALS = NewAdLocation("home_interstitials", AdType.Interstitial);

	public static readonly AdLocationInfo HOME_VIDEOS = NewAdLocation("home_videos", AdType.Video);

	public static readonly AdLocationInfo GAME_OVER_INTERSTITIALS = NewAdLocation("game_over_interstitials", AdType.Interstitial);

	public static readonly AdLocationInfo EARN_COINS_VIDEOS = NewAdLocation("earn_coins_videos", AdType.Video);

	public static readonly AdLocationInfo DOUBLE_UP_VIDEOS = NewAdLocation("double_up_videos", AdType.Video);

	public static readonly AdLocationInfo WORDY_WEEKEND_VIDEOS = NewAdLocation("wordy_weekend_videos", AdType.Video);

	public static readonly AdLocationInfo BOARD_BUTTON_VIDEOS = NewAdLocation("board_button_videos", AdType.Video);

	public static readonly AdLocationInfo REVIVAL_VIDEOS = NewAdLocation("revival_videos", AdType.Video);

	public static readonly AdLocationInfo GAMEOVER_DOUBLE_COINS_VIDEOS = NewAdLocation("double_coins_videos", AdType.Video);

	public static readonly AdLocationInfo HOVERBOARD_POPUP_VIDEOS = NewAdLocation("hoverboard_popup_videos", AdType.Video);

	public static readonly AdLocationInfo HOVERBOARD_FREE_VIDEOS = NewAdLocation("hoverboard_free_videos", AdType.Video);

	public static readonly AdLocationInfo HOVERBOARD_RUN_VIDEOS = NewAdLocation("hoverboard_run_videos", AdType.Video);

	public static readonly AdLocationInfo SKIP_MISSION_PAUSE_VIDEOS = NewAdLocation("skip_mission_pause_videos", AdType.Video);

	public static readonly AdLocationInfo SKIP_MISSION_TOUR_VIDEOS = NewAdLocation("skip_mission_tour_videos", AdType.Video);

	public static readonly AdLocationInfo FREE_COINS_FREE_VIDEOS = NewAdLocation("free_coins_free_videos", AdType.Video);

	public static readonly AdLocationInfo FREE_COINS_POPUP_VIDEOS = NewAdLocation("free_coins_popup_videos", AdType.Video);

	public static readonly AdLocationInfo HEADSTART_FREE_VIDEOS = NewAdLocation("headstart_free_videos", AdType.Video);

	public static readonly AdLocationInfo HEADSTART_RUN_VIDEOS = NewAdLocation("headstart_run_videos", AdType.Video);

	public static readonly AdLocationInfo SCOREBOOSTER_FREE_VIDEOS = NewAdLocation("scorebooster_free_videos", AdType.Video);

	public static readonly AdLocationInfo SCOREBOOSTER_RUN_VIDEOS = NewAdLocation("scorebooster_run_videos", AdType.Video);

	public static readonly AdLocationInfo BIRTHDAY_BOARD_VIDEO = NewAdLocation("birthday_board_video", AdType.Video);

	public static readonly AdLocationInfo WELCOME_BACK_VIDEOS = NewAdLocation("welcome_back_videos", AdType.Video);

	public static ICollection<AdLocationInfo> adLocations;

	private static AdLocationInfo NewAdLocation(string id, AdType type)
	{
		adLocations = adLocations ?? new List<AdLocationInfo>();
		AdLocationInfo adLocationInfo = new AdLocationInfo(id, type);
		adLocations.Add(adLocationInfo);
		return adLocationInfo;
	}
}
