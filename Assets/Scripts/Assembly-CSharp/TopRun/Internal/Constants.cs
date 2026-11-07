namespace TopRun.Internal
{
	public static class Constants
	{
		public const string API_GET_TOURNAMENT_BY_ID = "/tournaments/gettournamentbyid";

		public const string API_FIND_MATCHED_PLAYERS = "/tournaments/findmatchedplayers";

		public const string API_FIND_MATCHED_PLAYERS_BY_CURRENT_SCORE = "/tournaments/findmatchedplayersbycurrentscore";

		public const string API_GET_LATEST_TOURNAMENT_ID_BY_COUNTRY = "/tournaments/getlatesttournamentidbycountry";

		public const string API_CREATE_PLAYER = "/players/createplayer";

		public const string API_GET_PLAYERS_BY_FACEBOOK_IDS = "/facebook/getplayersbyfacebookids";

		public const string API_GET_PREVIOUS_TOURNAMENT_RESULTS_FOR_PLAYERS = "/tournaments/getprevioustournamentresultsforplayers";

		public const string API_FACEBOOK_LOGIN = "/facebook/login";

		public const string API_FACEBOOK_REGISTER = "/facebook/register";

		public const string API_GET_COUNTRY_CHANGE_DATE = "/players/getcountrychangedate";

		public const string API_SUBMIT_SCORE = "/scores/submitscore";

		public const int DEFAULT_STRANGER_AMOUNT = 10;

		public const int DEFAULT_FRIENDS_REFRESH_INTERVAL = 1800;

		public const int DEFAULT_TOURNAMENT_DATA_REFRESH_INTERVAL = 300;

		public const int DEFAULT_TOURNAMENT_DATA_REFRESH_INTERVAL_WHEN_RESULT_IS_DUE = 30;

		public const string DEFAULT_BASE_URL = "https://subwaysurfers.kiloo-games.com/api/v3";

		public const double DEFAULT_REQUEST_TIMEOUT_PROFILE_IMAGE = 20.0;

		public const double DEFAULT_REQUEST_TIMEOUT = 10.0;

		public const double DEFAULT_REQUEST_MIN_BACK_OFF_TIME = 1.0;

		public const double DEFAULT_REQUEST_MAX_BACK_OFF_TIME = 120.0;

		public const int DEFAULT_STRANGER_AMOUNT_RETRY_INTERVAL = 3600;

		public const int DEFAULT_CHALLENGERS_AMOUNT = 0;

		public const int DEFAULT_CHALLENGERS_PLACEMENT_REQUIREMENT = 4;

		public const double DEFAULT_CHALLENGERS_PERIOD_NORMALIZED_START = 0.5;

		public const double DEFAULT_CHALLENGERS_PERIOD_NORMALIZED_END = 2925.0 / 4095.0;

		public const int DEFAULT_FACEBOOK_FRIEND_PUBLIC_KEY_EXPIRY_SECONDS_MIN = 86400;

		public const int DEFAULT_FACEBOOK_FRIEND_PUBLIC_KEY_EXPIRY_SECONDS_MAX = 604800;

		public const int DEFAULT_MAX_FACEBOOK_FRIEND_PUBLIC_KEY_REQUESTS = 20;

		public const string ONLINESETTINGS_KEY_STRANGER_AMOUNT = "toprun_stranger_amount";

		public const string ONLINESETTINGS_KEY_FRIENDS_REFRESH_INTERVAL = "toprun_friends_refresh_interval";

		public const string ONLINESETTINGS_KEY_TOURNAMENT_DATA_REFRESH_INTERVAL = "toprun_refresh_interval";

		public const string ONLINESETTINGS_KEY_TOURNAMENT_DATA_REFRESH_INTERVAL_WHEN_RESULT_IS_DUE = "toprun_refresh_interval_when_result_is_due";

		public const string ONLINESETTINGS_KEY_GRACE_PERIOD = "toprun_grace_period";

		public const string ONLINESETTINGS_KEY_BASE_URL = "toprun_base_url";

		public const string ONLINESETTINGS_KEY_REQUEST_TIMEOUT = "toprun_request_timeout";

		public const string ONLINESETTINGS_KEY_FACEBOOK_PROFILE_IMAGE_TIMEOUT = "toprun_request_image_timeout";

		public const string ONLINESETTINGS_KEY_REQUEST_MIN_BACK_OFF_TIME = "toprun_request_min_back_off_time";

		public const string ONLINESETTINGS_KEY_REQUEST_MAX_BACK_OFF_TIME = "toprun_request_max_back_off_time";

		public const string ONLINESETTINGS_KEY_CHALLENGERS_AMOUNT = "toprun_challengers_amount";

		public const string ONLINESETTINGS_KEY_CHALLENGERS_PLACEMENT_REQUIREMENT = "toprun_challengers_placement_requirement";

		public const string ONLINESETTINGS_KEY_CHALLENGERS_PERIOD_NORMALIZED_START = "toprun_challengers_period_normalized_start";

		public const string ONLINESETTINGS_KEY_CHALLENGERS_PERIOD_NORMALIZED_END = "toprun_challengers_period_normalized_end";

		public const string ONLINESETTINGS_KEY_STRANGER_AMOUNT_RETRY_INTERVAL = "toprun_stranger_amount_retry_interval";

		public const string ONLINESETTINGS_KEY_SCOREMATCH_AMOUNT_RETRY_INTERVAL = "toprun_scorematch_amount_retry_interval";

		public const string ONLINESETTINGS_KEY_FACEBOOK_FRIEND_PUBLIC_KEY_EXPIRY_MIN = "toprun_facebook_friend_public_key_expiry_min";

		public const string ONLINESETTINGS_KEY_FACEBOOK_FRIEND_PUBLIC_KEY_EXPIRY_MAX = "toprun_facebook_friend_public_key_expiry_max";

		public const string ONLINESETTINGS_KEY_MAX_FACEBOOK_FRIEND_PUBLIC_KEY_REQUESTS = "toprun_max_facebook_public_key_requests";
	}
}
