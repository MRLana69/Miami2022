using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using AppRefresh;
using Data.Save;
using Info;
using JetBrains.Annotations;
using Kiloo;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Utility;
using Kiloo.GameCenter;
using Kiloo.Plugins.Facebook;
using Missions;
using SYBO.Common.Utilities;
using TopRun;
using UI.Screens;
using UI.SlideIns;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SocialManager : MonoBehaviour
{
	public enum SocialPictureSize
	{
		Normal = 0,
		Large = 1
	}

	public enum FacebookState
	{
		None = 0,
		Error = 1,
		LoggingIn = 2,
		LoggedIn = 3,
		Opening = 4
	}

	private enum FacebookPermissionCheckState
	{
		Waiting = 0,
		Ok = 1,
		Missing = 2,
		LoggedOut = 3,
		Error = 4
	}

	private class FacebookPermissionCheck
	{
		public FacebookPermissionCheckState state;
	}

	public enum WWWRequestResult
	{
		Success = 0,
		Error = 1
	}

	public delegate void WWWComplete(WWWRequestResult result, string output, object cookie);

	private const byte VERSION = 2;

	private const string UNREGISTER_DEVICE_URL = "/unregisterdevice.php?android";

	private const string REGISTER_DEVICE_URL = "/register2.php?android";

	private const string REPORT_SCORE_URL = "/report2.php?android";

	private const string CONSOLIDATE_FRIENDS_URL = "/friends2.php?android";

	private const string UPDATE_FRIEND_SCORES_URL = "/scores2.php?android";

	private const string POKE_URL = "/poke.php?android";

	private const string BRAG_URL = "/brag.php?android";

	private const string ANNOUNCE_URL = "/announce.php?android";

	private const float FACEBOOK_LOGIN_TIMEOUT = 10f;

	public const string NOTIFICATION_SOUND_NAME = "cpush.wav";

	private const string FACEBOOK_APPID = "254616967963463";

	public const string SECRET = "resxrctrv7tgv7gb8h9h9u0909kllfmolkjnhghgjjkhjghg";

	private const string ONLINESETTINGS_CONSOLIDATE_MIN_SECONDS_KEY = "social_consolidate_min_seconds";

	private const int CONSOLIDATE_MIN_SECONDS_DEFAULT = 3600;

	public const float USER_DATA_REFRESH_ON_AUTHENTICATE_AGE = 600f;

	public const float FRIEND_DATA_REFRESH_ON_RESUME_AGE = 3600f;

	private float _lastConsolidateFriendCompleteTime;

	private List<string> _latestPreConsolidationFacebookIds;

	private List<string> _latestSuccesfullyConsolidatedFacebookIds;

	private const string ONLINESETTINGS_REPORT_GAMES_COUNT_KEY = "social_report_games_count";

	private const int REPORT_GAMES_COUNT_DEFAULT = 5;

	private const string ONLINESETTINGS_REPORT_MIN_SECONDS_KEY = "social_report_min_seconds";

	private const int REPOST_MIN_SECONDS_DEFAULT = 1200;

	private const string LAST_REGISTER_TIMESTAMPTICKS_PLAYERPREFSKEY = "socmanlastregtime";

	private const string LAST_REGISTER_DATA_PLAYERPREFSKEY = "socmanlastregdata";

	private const string ONLINESETTINGS_REGISTER_MIN_SECONDS_KEY = "social_register_min_seconds";

	private const int REGISTER_MIN_SECONDS_DEFAULT = 3600;

	private const string ONLINESETTINGS_FACEBOOK_FETCH_MAX_USERS_KEY = "social_facebook_fetch_max_users";

	private const int FACEBOOK_FETCH_MAX_USERS_DEFAULT = 5000;

	private const string ONLINESETTINGS_FACEBOOK_KEEP_MAX_USERS_KEY = "social_facebook_keep_max_users";

	private const int FACEBOOK_KEEP_MAX_USERS_DEFAULT = 200;

	private string _tempRegisterUserData;

	private bool _appHasFocus = true;

	private const string ONLINESETTINGS_FRIENDSCORES_MIN_SECONDS_KEY = "social_friendscores_min_seconds";

	private const int FRIENDSCORES_MIN_SECONDS_DEFAULT = 120;

	private static SocialManager _instance;

	private int _userid;

	private string _lastFacebookLoginLocation = "unset";

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<FacebookProfile> facebookPictureDownloadedHandler__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<bool> onRegisterUserCompleted__BackingField;

	private bool _registerUserCompleted;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<bool> friendsConsolidatedHandler__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action friendsChangedHandler__BackingField;

	private FacebookProfile _fbProfile;

	private GameCenterProfile _gcProfile;

	private Action _localUserUpdatedHandler;

	private List<Friend> _friends;

	private IDictionary<string, FacebookProfile> _fbProfiles;

	private bool _fbReady;

	private DateTime _lastFriendScoreUpdateTimestamp = new DateTime(0L);

	private bool _didAddAuthenticationHandlerToGameCenter;

	private Action _onFacebookStateChanged;

	private FacebookState _fbState;

	private bool _didAddChangeHandlerToFacebookSession;

	private readonly string[] requiredPermissions = new string[2] { "public_profile", "user_friends" };

	private int _downloadFacebookPicturesContext;

	public const int FACEBOOK_CONSOLIDATED = 1;

	public const int GAMECENTER_CONSOLIDATED = 2;

	private int _consolidationState;

	private bool _consolidatedFriendsCompleted;

	private static bool _applicationIsQuiting;

	private IDictionary<string, GameCenterPlugin.Achievement> _achievements;

	private int _initAchievementsContext;

	private bool _pausePhotoDownloading;

	public int localUserId
	{
		get
		{
			return _userid;
		}
	}

	public bool isRunningFacebookLogin { get; private set; }

	public bool isRegisterUserCompleted
	{
		get
		{
			return _registerUserCompleted;
		}
	}

	public FacebookProfile facebookProfile
	{
		get
		{
			return _fbProfile;
		}
	}

	public GameCenterProfile gamecenterProfile
	{
		get
		{
			return _gcProfile;
		}
	}

	public Texture2D localUserImage
	{
		get
		{
			if (facebookProfile != null)
			{
				return facebookProfile.image;
			}
			if (gamecenterProfile != null)
			{
				return gamecenterProfile.image;
			}
			return null;
		}
	}

	public string localUserRealName
	{
		get
		{
			if (facebookProfile != null)
			{
				return facebookProfile.name;
			}
			return Strings.Get("PLAYER_GENERIC_NAME");
		}
	}

	public string localDisplayName
	{
		get
		{
			if (facebookProfile != null)
			{
				return facebookProfile.name;
			}
			if (gamecenterProfile != null)
			{
				return gamecenterProfile.displayName;
			}
			return Strings.Get("PLAYER_SELF_REFERENCE");
		}
	}

	public bool HasFacebookProfile
	{
		get
		{
			return facebookProfile != null;
		}
	}

	public static SocialManager instance
	{
		get
		{
			Init();
			return _instance;
		}
	}

	public bool gamecenterIsLoggedIn
	{
		get
		{
			return SocialManagerIOS.instance.gameCenterState != SocialManagerIOS.GCState.Authenticating && _gcProfile != null;
		}
	}

	public bool gamecenterIsLoggedOut
	{
		get
		{
			return SocialManagerIOS.instance.gameCenterState == SocialManagerIOS.GCState.LoggedOut;
		}
	}

	public FacebookState facebookState
	{
		get
		{
			return _fbState;
		}
		private set
		{
			if (_fbState != value)
			{
				_fbState = value;
				Action onFacebookStateChanged = _onFacebookStateChanged;
				if (onFacebookStateChanged != null)
				{
					onFacebookStateChanged();
				}
			}
		}
	}

	public bool facebookIsLoggedIn
	{
		get
		{
			if (Globals.DEBUG_USE_DEBUG_LOGIN_STATE)
			{
				return true;
			}
			return FacebookSession.HasOpenInstance();
		}
	}

	public static bool facebookDialogsSupported
	{
		get
		{
			return FacebookSession.dialogsSupported;
		}
	}

	public int consolidationState
	{
		get
		{
			return _consolidationState;
		}
	}

	public bool consolidatedFriendsCompleted
	{
		get
		{
			return _consolidatedFriendsCompleted;
		}
	}

	public bool requestInvolvingDevicetokenInProgress { get; private set; }

	public bool canUnregisterDevice { get; private set; }

	public bool pausePhotoDownloading
	{
		get
		{
			return _pausePhotoDownloading;
		}
		set
		{
			if (SocialManagerIOS.isInstanced)
			{
				SocialManagerIOS.instance.pausePhotoDownloading = value;
			}
			_pausePhotoDownloading = value;
		}
	}

	public static event Action<FacebookProfile> facebookPictureDownloadedHandler
	{
		add
		{
			Action<FacebookProfile> action = facebookPictureDownloadedHandler__BackingField;
			Action<FacebookProfile> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref facebookPictureDownloadedHandler__BackingField, (Action<FacebookProfile>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<FacebookProfile> action = facebookPictureDownloadedHandler__BackingField;
			Action<FacebookProfile> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref facebookPictureDownloadedHandler__BackingField, (Action<FacebookProfile>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action<bool> onRegisterUserCompleted
	{
		add
		{
			Action<bool> action = onRegisterUserCompleted__BackingField;
			Action<bool> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onRegisterUserCompleted__BackingField, (Action<bool>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<bool> action = onRegisterUserCompleted__BackingField;
			Action<bool> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref onRegisterUserCompleted__BackingField, (Action<bool>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public event Action<bool> friendsConsolidatedHandler
	{
		add
		{
			Action<bool> action = friendsConsolidatedHandler__BackingField;
			Action<bool> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref friendsConsolidatedHandler__BackingField, (Action<bool>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<bool> action = friendsConsolidatedHandler__BackingField;
			Action<bool> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref friendsConsolidatedHandler__BackingField, (Action<bool>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action friendsChangedHandler
	{
		add
		{
			Action action = friendsChangedHandler__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref friendsChangedHandler__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = friendsChangedHandler__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref friendsChangedHandler__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	[Conditional("UNITY_IPHONE")]
	public void AddGameCenterSmallSizePictureDownloadedHandler(Action<GameCenterProfile> handler)
	{
	}

	[Conditional("UNITY_IPHONE")]
	public void RemoveGameCenterSmallSizePictureDownloadedHandler(Action<GameCenterProfile> handler)
	{
	}

	private void FriendConsolidationDidFinish(bool success)
	{
		friendsConsolidatedHandler__BackingField.SafeInvoke(success);
		FriendsDidChange();
	}

	private void FriendsDidChange()
	{
		Action action = friendsChangedHandler__BackingField;
		if (action != null)
		{
			action();
		}
	}

	public void AddLocalUserUpdatedHandler(Action handler)
	{
		_localUserUpdatedHandler = (Action)Delegate.Combine(_localUserUpdatedHandler, handler);
	}

	public void RemoveLocalUserUpdatedHandler(Action handler)
	{
		_localUserUpdatedHandler = (Action)Delegate.Remove(_localUserUpdatedHandler, handler);
	}

	public Friend FindFriend(int userid)
	{
		return _friends.Find((Friend f) => userid == f.userid);
	}

	public Friend[] FriendsSortedByScore()
	{
		if (_friends != null)
		{
			Friend[] array = _friends.ToArray();
			Array.Sort(array, (Friend x, Friend y) => y.score - x.score);
			return array;
		}
		return new Friend[0];
	}

	public Friend[] FriendsSortedByCash()
	{
		if (_friends != null)
		{
			Friend[] array = _friends.ToArray();
			Array.Sort(array, (Friend x, Friend y) => y.gamesToCashIn - x.gamesToCashIn);
			return array;
		}
		return new Friend[0];
	}

	public void DownloadLargePictureForUser(Friend friend, Action<Friend, Texture2D> handler)
	{
		if (handler == null)
		{
			return;
		}
		int relation = friend.relation;
		if ((relation & 1) == 1)
		{
			StartCoroutine(DownloadPictureForProfile(friend.fbProfile, SocialPictureSize.Large, (FacebookProfile profile, Texture2D texture) =>
			{
				handler(friend, texture);
			}));
		}
		else if ((relation & 2) == 2)
		{
			StartCoroutine(SocialManagerIOS.instance.DownloadPictureForProfile(friend.gcProfile, SocialPictureSize.Large, (GameCenterProfile Profiler, Texture2D texture) =>
			{
				handler(friend, texture);
			}));
		}
	}

	public static void Init()
	{
		if (!(_instance == null) || _applicationIsQuiting)
		{
			return;
		}
		GameObject gameObject = new GameObject("SocialManager");
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<SocialManager>();
		HierarchyUtility.ReparentByHierarchyPathType(gameObject.transform, HierarchyPathType.Dynamic);
		HierarchyUtility.SetAsLastSibling(gameObject.transform);
		AgeRestriction.restrictionChanged += () =>
		{
			if (AgeRestriction.isRestricted)
			{
				instance.FacebookLogout();
				IManager manager = TopRunRoot.manager;
				if (manager != null)
				{
					manager.RefreshTournament();
				}
			}
		};
		AppRefreshHandler.hasRefreshed += (RefreshType refreshType) =>
		{
			if (refreshType.HasFlag(RefreshType.Data) && FacebookRewardsInfo.CanPayoutReward())
			{
				instance.FacebookLogout();
			}
			if (refreshType.HasFlag(RefreshType.Data) || refreshType.HasFlag(RefreshType.UI))
			{
				instance._consolidatedFriendsCompleted = false;
				instance.Invalidate();
			}
		};
	}

	[Conditional("UNITY_IPHONE")]
	private void ClearGameCenter()
	{
		_gcProfile = null;
		if (_friends != null)
		{
			_friends.RemoveAll((Friend item) => item.gcProfile != null && item.fbProfile == null);
			_friends.ForEach((Friend item) =>
			{
				item.gcProfile = null;
			});
		}
		SocialManagerIOS.instance.ClearFriends();
		SocialManagerIOS.instance.AbortAsynchronousProcesses();
		_consolidationState &= -3;
	}

	private void OnGameCenterAuthentication(bool hasLoginView, int errorCode, string error)
	{
		GameCenterPlugin.LocalPlayer localPlayer = GameCenterPlugin.GetLocalPlayer();
		if (localPlayer.authenticated)
		{
			if (_gcProfile != null && !(_gcProfile.id != localPlayer.playerID) && (CDateTime.Now - _gcProfile.refreshedTime).TotalSeconds > 600.0)
			{
				_gcProfile = null;
			}
			if (_gcProfile == null)
			{
				_gcProfile = new GameCenterProfile
				{
					alias = localPlayer.alias,
					displayName = localPlayer.displayName,
					id = localPlayer.playerID,
					refreshedTime = CDateTime.Now
				};
				GameCenterPlugin.LoadPhoto(localPlayer.playerID, GameCenterPlugin.PhotoSize.Small, 50, 50, (string playerID, Texture2D photo, string photoError) =>
				{
					if (_gcProfile != null)
					{
						_gcProfile.image = photo;
					}
				});
				InitAchievements();
			}
			Flurry.LogGameCenterLogin();
		}
		else if (!gamecenterIsLoggedOut)
		{
			SocialManagerIOS.instance.gameCenterState = SocialManagerIOS.GCState.LoggedOut;
			Invalidate();
			FriendsDidChange();
		}
	}

	[Conditional("UNITY_IPHONE")]
	public void InitGameCenter()
	{
		SocialManagerIOS.instance.gameCenterState = SocialManagerIOS.GCState.Authenticating;
		if (!_didAddAuthenticationHandlerToGameCenter)
		{
			GameCenterPlugin.authenticationEvent += OnGameCenterAuthentication;
			_didAddAuthenticationHandlerToGameCenter = true;
		}
		if (GameCenterPlugin.initialAuthenticationComplete)
		{
			OnGameCenterAuthentication(GameCenterPlugin.hasLoginView, 0, null);
		}
		else
		{
			GameCenterPlugin.Init();
		}
	}

	private void OnGameCenterPhotoDownloaded(GameCenterProfile gcProfile)
	{
		if (gcProfile == _gcProfile)
		{
			if (_localUserUpdatedHandler != null)
			{
				_localUserUpdatedHandler();
			}
		}
		else
		{
			if (_friends == null)
			{
				return;
			}
			Friend friend = null;
			foreach (Friend friend2 in _friends)
			{
				if (friend2.gcProfile == gcProfile)
				{
					friend = friend2;
				}
			}
			if (friend != null)
			{
				friend.InvokeUpdatedHandler();
			}
		}
	}

	private void OnFacebookPhotoDownloaded(FacebookProfile fbProfile)
	{
		if (fbProfile == _fbProfile)
		{
			if (_localUserUpdatedHandler != null)
			{
				_localUserUpdatedHandler();
			}
		}
		else
		{
			if (_friends == null)
			{
				return;
			}
			Friend friend = null;
			foreach (Friend friend2 in _friends)
			{
				if (friend2.fbProfile == fbProfile)
				{
					friend = friend2;
				}
			}
			if (friend != null)
			{
				friend.InvokeUpdatedHandler();
			}
		}
	}

	public void AddFacebookStateChangedHandler(Action handler)
	{
		_onFacebookStateChanged = (Action)Delegate.Combine(_onFacebookStateChanged, handler);
	}

	public void RemoveFacebookStateChangedHandler(Action handler)
	{
		_onFacebookStateChanged = (Action)Delegate.Remove(_onFacebookStateChanged, handler);
	}

	public bool FacebookLogin(Action<bool> onComplete)
	{
		if (AgeRestriction.isRestricted)
		{
			return false;
		}
		if (!isRunningFacebookLogin)
		{
			_fbReady = false;
			isRunningFacebookLogin = true;
			if (UIScreenController.Instance != null)
			{
				UIScreenController uIScreenController = UIScreenController.Instance;
				string text = ((!uIScreenController.isShowingPopup) ? string.Empty : (uIScreenController.GetCurrentPopupName() + " on "));
				if (!string.IsNullOrEmpty(uIScreenController.GetTopScreenFlurryName()))
				{
					_lastFacebookLoginLocation = text + uIScreenController.GetTopScreenFlurryName();
				}
				else
				{
					_lastFacebookLoginLocation = text + uIScreenController.GetTopScreenName();
				}
			}
			StartCoroutine(FacebookLoginCoroutine(onComplete));
			return true;
		}
		LogWarning("SocialManager.FacebookLogin called, but we are already logging in", this);
		return false;
	}

	private void HandleLoggingIntoFacebookComplete(bool success, Action<bool> onComplete)
	{
		Invalidate();
		isRunningFacebookLogin = false;
		onComplete.SafeInvoke(success);
	}

	public void FacebookLogout()
	{
		FacebookSession facebookSession = FacebookSession.instance;
		if (facebookSession != null)
		{
			facebookSession.Close(true);
		}
		if (_friends != null)
		{
			_friends.RemoveAll((Friend item) => item.gcProfile == null && item.fbProfile != null);
			_friends.ForEach((Friend item) =>
			{
				item.fbProfile = null;
			});
		}
		_latestSuccesfullyConsolidatedFacebookIds = null;
		_fbProfiles = null;
		_fbProfile = null;
		FriendsDidChange();
		_consolidationState &= -2;
		CharacterScreenManager.Instance.OnFacebookLogout();
	}

	public static bool IsOAuthException(FacebookError error, object obj)
	{
		if (error != null)
		{
			if (error.category == FacebookError.Category.AUTHENTICATION_REOPEN_SESSION || error.category == FacebookError.Category.PERMISSION)
			{
				return true;
			}
			if (error.errorType == "OAuthException")
			{
				return true;
			}
		}
		return false;
	}

	private void OnFacebookSessionStatusChanged(FacebookSession.State status, FacebookError error)
	{
		if (status == FacebookSession.State.OPENING)
		{
			return;
		}
		if (FacebookSession.IsOpened(status))
		{
			if (facebookState == FacebookState.LoggingIn)
			{
				Flurry.LogFacebookLogin(_lastFacebookLoginLocation);
			}
			facebookState = FacebookState.LoggedIn;
		}
		else if (status == FacebookSession.State.CLOSED_LOGIN_FAILED)
		{
			if (facebookState == FacebookState.LoggingIn && error != null && error.shouldNotifyUser && !string.IsNullOrEmpty(error.userActionMessage))
			{
				DeviceUtility.ShowNativePopup(Strings.Get("FACEBOOK"), error.userActionMessage, Strings.Get("OK"));
			}
			facebookState = FacebookState.Error;
		}
		else if (error != null)
		{
			facebookState = FacebookState.Error;
		}
		else
		{
			facebookState = FacebookState.None;
		}
	}

	public bool HasFacebookAccessToken()
	{
		if (AgeRestriction.isRestricted)
		{
			return false;
		}
		FacebookSession facebookSession = FacebookSession.instance;
		if (facebookSession != null)
		{
			FacebookSession.State state = facebookSession.GetState();
			return state == FacebookSession.State.CREATED_TOKEN_LOADED || FacebookSession.IsOpened(state);
		}
		return false;
	}

	private FacebookSession CreateFacebookSessionIfNeeded()
	{
		if (!_didAddChangeHandlerToFacebookSession)
		{
			FacebookSession.statusChangedEvent += OnFacebookSessionStatusChanged;
			_didAddChangeHandlerToFacebookSession = true;
		}
		if (!FacebookSession.HasOpenInstance())
		{
			FacebookSession.appId = "254616967963463";
			string permissions = string.Join(",", requiredPermissions);
			FacebookSession.Create(permissions, FacebookDefaultAudience.FRIENDS);
		}
		return FacebookSession.instance;
	}

	private Coroutine FacebookCheckPermissionsAsync(FacebookPermissionCheck check)
	{
		return StartCoroutine(FacebookCheckPermissionsCoroutine(check));
	}

	private IEnumerator FacebookCheckPermissionsCoroutine(FacebookPermissionCheck check)
	{
		FacebookSession fbSession = FacebookSession.instance;
		check.state = FacebookPermissionCheckState.Waiting;
		fbSession.GraphRequest("me/permissions", new Dictionary<string, object>(), HttpMethod.GET, (IDictionary<string, object> result, FacebookError error, bool isFromCache) =>
		{
			try
			{
				if (check.state != FacebookPermissionCheckState.Waiting)
				{
					return;
				}
				if (IsOAuthException(error, result))
				{
					FacebookLogout();
					check.state = FacebookPermissionCheckState.LoggedOut;
				}
				else if (result != null)
				{
					object value;
					result.TryGetValue("data", out value);
					IList<object> list = value as IList<object>;
					if (list != null)
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string>();
						foreach (object item in list)
						{
							IDictionary dictionary2 = item as IDictionary;
							if (dictionary2 != null)
							{
								string text = dictionary2["permission"] as string;
								string text2 = dictionary2["status"] as string;
								if (text != null && text2 != null)
								{
									dictionary[text] = text2;
								}
							}
						}
						bool flag = true;
						string[] array = requiredPermissions;
						foreach (string text3 in array)
						{
							string value2;
							dictionary.TryGetValue(text3, out value2);
							if (value2 != "granted")
							{
								LogError("Required Facebook permission not granted: \"" + text3 + "\"");
								flag = false;
							}
						}
						check.state = (flag ? FacebookPermissionCheckState.Ok : FacebookPermissionCheckState.Missing);
					}
					else
					{
						LogError("Failed to get list of Facebook permissions");
					}
				}
			}
			catch (Exception)
			{
				LogError("Exception while checking permissions");
			}
			if (check.state == FacebookPermissionCheckState.Waiting)
			{
				check.state = FacebookPermissionCheckState.Error;
			}
		});
		float lastRealTime;
		float timeLeftBeforeGivingUp;
		ResetTimeLeft(out lastRealTime, out timeLeftBeforeGivingUp);
		while (check.state == FacebookPermissionCheckState.Waiting)
		{
			UpdateTimeLeft(ref lastRealTime, ref timeLeftBeforeGivingUp);
			if (timeLeftBeforeGivingUp <= 0f)
			{
				LogError("Timeout while checking permissions");
				check.state = FacebookPermissionCheckState.Error;
			}
			else
			{
				yield return null;
			}
		}
	}

	private void ResetTimeLeft(out float lastRealTime, out float timeLeft)
	{
		timeLeft = 10f;
		lastRealTime = Time.realtimeSinceStartup;
	}

	private void UpdateTimeLeft(ref float lastRealTime, ref float timeLeft)
	{
		if (facebookState != FacebookState.Opening)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float num = realtimeSinceStartup - lastRealTime;
			lastRealTime = realtimeSinceStartup;
			if (num < 0.5f && _appHasFocus)
			{
				timeLeft -= num;
			}
		}
	}

	private IEnumerator FacebookLoginCoroutine(Action<bool> onComplete)
	{
		FacebookSession fbSession = CreateFacebookSessionIfNeeded();
		if (fbSession == null)
		{
			HandleLoggingIntoFacebookComplete(false, onComplete);
			yield break;
		}
		bool asynchronousRequestsCompleted = false;
		float lastRealTime;
		float timeLeftBeforeGivingUp;
		if (!FacebookSession.HasOpenInstance())
		{
			facebookState = FacebookState.LoggingIn;
			fbSession.OpenForRead();
			ResetTimeLeft(out lastRealTime, out timeLeftBeforeGivingUp);
			while (facebookState != FacebookState.LoggedIn)
			{
				if (facebookState == FacebookState.Error)
				{
					HandleLoggingIntoFacebookComplete(false, onComplete);
					yield break;
				}
				UpdateTimeLeft(ref lastRealTime, ref timeLeftBeforeGivingUp);
				if (timeLeftBeforeGivingUp <= 0f)
				{
					fbSession.Cancel();
					facebookState = FacebookState.Error;
					HandleLoggingIntoFacebookComplete(false, onComplete);
					LogWarning("SocialManager WARNING: Facebook login timed out", this);
					yield break;
				}
				yield return null;
			}
		}
		FacebookPermissionCheck permissionsCheck = new FacebookPermissionCheck();
		yield return FacebookCheckPermissionsAsync(permissionsCheck);
		if (permissionsCheck.state == FacebookPermissionCheckState.Missing)
		{
			bool statusDidChange = false;
			FacebookSession.StatusChangedHandler statusChangedHandler = null;
			statusChangedHandler = (FacebookSession.State status, FacebookError error) =>
			{
				FacebookSession.statusChangedEvent -= statusChangedHandler;
				statusDidChange = true;
			};
			FacebookSession.statusChangedEvent += statusChangedHandler;
			fbSession.RequestNewReadPermissions(string.Join(",", requiredPermissions));
			ResetTimeLeft(out lastRealTime, out timeLeftBeforeGivingUp);
			while (!statusDidChange)
			{
				UpdateTimeLeft(ref lastRealTime, ref timeLeftBeforeGivingUp);
				if (timeLeftBeforeGivingUp <= 0f)
				{
					LogError("Timed out waiting for FacebookSession.RequestNewReadPermissions", this);
					FacebookLogout();
					facebookState = FacebookState.Error;
					HandleLoggingIntoFacebookComplete(false, onComplete);
					yield break;
				}
				yield return null;
			}
			yield return FacebookCheckPermissionsAsync(permissionsCheck);
		}
		if (permissionsCheck.state != FacebookPermissionCheckState.Ok)
		{
			if (permissionsCheck.state == FacebookPermissionCheckState.Missing)
			{
				LogError("Facebook permissions are NOT ok, logging out");
				FacebookLogout();
			}
			else
			{
				LogError("Error checking Facebook permissions");
			}
			facebookState = FacebookState.Error;
			HandleLoggingIntoFacebookComplete(false, onComplete);
			yield break;
		}
		ResetTimeLeft(out lastRealTime, out timeLeftBeforeGivingUp);
		fbSession.GraphRequest("me", new Dictionary<string, object> { { "fields", "id,name,first_name" } }, HttpMethod.GET, (IDictionary<string, object> result, FacebookError error, bool isFromCache) =>
		{
			if (!(timeLeftBeforeGivingUp <= 0f))
			{
				bool flag = false;
				if (IsOAuthException(error, result))
				{
					FacebookLogout();
				}
				else if (result != null)
				{
					_fbProfile = new FacebookProfile();
					object value;
					result.TryGetValue("id", out value);
					_fbProfile.id = value as string;
					result.TryGetValue("first_name", out value);
					_fbProfile.name = value as string;
					result.TryGetValue("name", out value);
					_fbProfile.fullName = value as string;
					flag = _fbProfile.id != null;
				}
				if (flag)
				{
					StartCoroutine(DownloadPictureForProfile(_fbProfile, SocialPictureSize.Normal, OnDownloadedFacebookPicture));
					int value2 = OnlineSettings.instance.GetValue("social_facebook_fetch_max_users", 5000);
					fbSession.GraphRequest("me/friends", new Dictionary<string, object>
					{
						{ "fields", "id,name,first_name,installed" },
						{
							"limit",
							value2.ToString(CultureInfo.InvariantCulture)
						}
					}, HttpMethod.GET, (IDictionary<string, object> friendsResult, FacebookError friendsError, bool friendsIsFromCache) =>
					{
						if (timeLeftBeforeGivingUp <= 0f)
						{
							return;
						}
						asynchronousRequestsCompleted = true;
						try
						{
							bool flag2 = false;
							if (IsOAuthException(friendsError, friendsResult))
							{
								FacebookLogout();
							}
							else
							{
								if (friendsError != null)
								{
								}
								if (friendsResult != null)
								{
									object value3;
									friendsResult.TryGetValue("data", out value3);
									IList<object> list = value3 as IList<object>;
									if (list != null)
									{
										int value4 = OnlineSettings.instance.GetValue("social_facebook_keep_max_users", 200);
										List<FacebookProfile> list2 = new List<FacebookProfile>();
										foreach (object item in list)
										{
											IDictionary<string, object> dictionary = item as IDictionary<string, object>;
											if (dictionary != null && dictionary.ContainsKey("id"))
											{
												dictionary.TryGetValue("id", out value3);
												string text = value3 as string;
												if (text != null && dictionary.ContainsKey("installed"))
												{
													FacebookProfile value5;
													if (_fbProfiles == null || !_fbProfiles.TryGetValue(text, out value5))
													{
														value5 = new FacebookProfile
														{
															id = text
														};
													}
													dictionary.TryGetValue("first_name", out value3);
													value5.name = value3 as string;
													dictionary.TryGetValue("name", out value3);
													value5.fullName = value3 as string;
													list2.Add(value5);
												}
											}
											else
											{
												LogError("FacebookLogin: Unexpected format of FaceBook Friend", this);
											}
										}
										int count = list2.Count;
										if (count > value4)
										{
											list2.Sort((FacebookProfile p1, FacebookProfile p2) => string.Compare(p1.id, p2.id, Utils.stringComparison));
											list2.RemoveRange(value4, count - value4);
										}
										Dictionary<string, FacebookProfile> dictionary2 = new Dictionary<string, FacebookProfile>();
										foreach (FacebookProfile item2 in list2)
										{
											dictionary2[item2.id] = item2;
										}
										_fbProfiles = dictionary2;
										if (Flurry.ShouldLogDailyEvent("Social Facebook Friends Retrieved"))
										{
											string text2 = Flurry.ConvertFriendCountToBracket(list.Count);
											string text3 = Flurry.ConvertFriendCountToBracket(count);
											Flurry.LogEventWithSeveralParameters("Social Facebook Friends Retrieved", "Total friends;Installed friends", text2 + ";" + text3);
										}
										flag2 = true;
									}
									else
									{
										LogError("FacebookLogin: Error: Could not retreive friends", this);
									}
								}
							}
							if (flag2)
							{
								_fbReady = true;
								HandleLoggingIntoFacebookComplete(true, onComplete);
								StartCoroutine(DownloadFacebookPictures(_fbProfiles));
							}
							else
							{
								_fbProfiles = null;
								_fbReady = true;
								HandleLoggingIntoFacebookComplete(false, onComplete);
							}
						}
						catch (Exception ex)
						{
							LogWarning("Error: " + ex);
							HandleLoggingIntoFacebookComplete(false, onComplete);
						}
					});
				}
				else
				{
					asynchronousRequestsCompleted = true;
					LogError("FacebookLogin: Error getting user info, error: " + error, this);
					HandleLoggingIntoFacebookComplete(false, onComplete);
				}
			}
		});
		while (!asynchronousRequestsCompleted && timeLeftBeforeGivingUp > 0f)
		{
			UpdateTimeLeft(ref lastRealTime, ref timeLeftBeforeGivingUp);
			if (timeLeftBeforeGivingUp <= 0f)
			{
				facebookState = FacebookState.Error;
				HandleLoggingIntoFacebookComplete(false, onComplete);
				LogWarning("SocialManager WARNING: Facebook requests to get me and friends info timed out", this);
				break;
			}
			yield return null;
		}
	}

	private bool HasUnconsolidatedFacebookFriends()
	{
		if (_fbProfiles != null)
		{
			if (_latestSuccesfullyConsolidatedFacebookIds == null)
			{
				return true;
			}
			foreach (string key in _fbProfiles.Keys)
			{
				if (!_latestSuccesfullyConsolidatedFacebookIds.Contains(key))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void RecommendAppFacebook()
	{
		if (FacebookSession.HasOpenInstance())
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("link", "http://redirect.kiloo.com/subwayapp.php");
			dictionary.Add("name", "Subway Surfers");
			dictionary.Add("caption", string.Empty);
			dictionary.Add("description", Strings.Get("FACEBOOK_RECOMMEND_CAPTION"));
			Dictionary<string, object> parameters = dictionary;
			FacebookSession.instance.ShowDialog("feed", parameters, (IDictionary<string, object> result, FacebookError error) =>
			{
				if (result != null && result.ContainsKey("post_id"))
				{
					Flurry.LogEvent("Share link on Facebook");
				}
				else if (error != null)
				{
					DeviceUtility.ShowNativePopup(Strings.Get("FACEBOOK"), Strings.Get("FACEBOOK_DIALOG_ERROR_MESSAGE"), Strings.Get("OK"));
				}
			});
		}
		else
		{
			LogError("Not logged in to facebook");
		}
	}

	public void BragFacebook(List<Friend> friends)
	{
		if (FacebookSession.HasOpenInstance())
		{
			List<Friend> list = null;
			if (friends != null)
			{
				list = new List<Friend>(friends.Count);
				foreach (Friend friend in friends)
				{
					if (friend.fbProfile != null && friend.score < PlayerSave.highScore)
					{
						list.Add(friend);
					}
				}
				list.Sort((Friend x, Friend y) => y.score - x.score);
			}
			int highScore = PlayerSave.highScore;
			string deviceTypeString = GetDeviceTypeString();
			string value = ((list == null || list.Count == 0) ? string.Format(Strings.Get("FACEBOOK_BRAG_MESSAGE_BEAT_NONE"), highScore, deviceTypeString) : ((list.Count == 1) ? string.Format(Strings.Get("FACEBOOK_BRAG_MESSAGE_BEAT_ONE"), highScore, deviceTypeString, list[0].fbProfile.fullName) : ((list.Count == 2) ? string.Format(Strings.Get("FACEBOOK_BRAG_MESSAGE_BEAT_TWO"), highScore, deviceTypeString, list[0].fbProfile.fullName, list[1].fbProfile.fullName) : ((list.Count != 3) ? string.Format(Strings.Get("FACEBOOK_BRAG_MESSAGE_BEAT_MANY"), highScore, deviceTypeString, list[0].fbProfile.fullName, list[1].fbProfile.fullName, list.Count - 2) : string.Format(Strings.Get("FACEBOOK_BRAG_MESSAGE_BEAT_THREE"), highScore, deviceTypeString, list[0].fbProfile.fullName, list[1].fbProfile.fullName, list[2].fbProfile.fullName)))));
			FacebookSession.instance.ShowDialog("feed", new Dictionary<string, object>
			{
				{ "link", "http://redirect.kiloo.com/subwayapp.php" },
				{ "name", "New Subway Surfers High Score" },
				{ "caption", "Download Subway Surfers now" },
				{ "description", value }
			}, (IDictionary<string, object> result, FacebookError error) =>
			{
				if (result != null && result.ContainsKey("post_id"))
				{
					Flurry.LogEvent("Social bragged Facebook");
					if (UIScreenController.IsInstanced)
					{
						UIScreenController.Instance.QueueSlideIn(new SlideIn
						{
							type = SlideInData.SlideInType.Brag
						});
					}
				}
				else if (error != null)
				{
					DeviceUtility.ShowNativePopup(Strings.Get("FACEBOOK"), Strings.Get("FACEBOOK_DIALOG_ERROR_MESSAGE"), Strings.Get("OK"));
				}
			});
		}
		else
		{
			LogError("Not logged in to facebook");
		}
	}

	private IEnumerator DownloadPictureForProfile(FacebookProfile profile, SocialPictureSize pictureSize, Action<FacebookProfile, Texture2D> handler)
	{
		if (profile == null)
		{
			LogError("facebook profile was null in DownloadFacebookPictures!");
			yield break;
		}
		string url = ((pictureSize != SocialPictureSize.Normal) ? ("http://graph.facebook.com/" + profile.id + "/picture?width=300&height=300") : ("http://graph.facebook.com/" + profile.id + "/picture?width=64&height=64"));
		WWW www = new WWW(url);
		yield return www;
		if (www.error != null)
		{
		}
		if (handler != null)
		{
			handler(profile, www.texture);
		}
	}

	private void OnDownloadedFacebookPicture(FacebookProfile profile, Texture2D picture)
	{
		if (!(picture == null) && (picture.width != 8 || picture.height != 8))
		{
			picture.wrapMode = TextureWrapMode.Clamp;
			profile.image = picture;
			if (facebookPictureDownloadedHandler__BackingField != null)
			{
				facebookPictureDownloadedHandler__BackingField(profile);
			}
		}
	}

	private IEnumerator DownloadFacebookPictures(IDictionary<string, FacebookProfile> fbProfiles)
	{
		int downloadFacebookPicturesContext = ++_downloadFacebookPicturesContext;
		List<FacebookProfile> profiles = new List<FacebookProfile>(fbProfiles.Count);
		foreach (FacebookProfile value in fbProfiles.Values)
		{
			if (value.image == null)
			{
				profiles.Add(value);
			}
		}
		foreach (FacebookProfile profile in profiles)
		{
			if (profile != null)
			{
				yield return StartCoroutine(DownloadPictureForProfile(profile, SocialPictureSize.Normal, OnDownloadedFacebookPicture));
				while (pausePhotoDownloading && downloadFacebookPicturesContext == _downloadFacebookPicturesContext)
				{
					yield return null;
				}
				if (downloadFacebookPicturesContext != _downloadFacebookPicturesContext)
				{
					break;
				}
			}
		}
	}

	public void Invalidate()
	{
		if ((Application.platform == RuntimePlatform.IPhonePlayer && !SocialManagerIOS.instance.IsDoneAuthenticating()) || RemoteNotificationControl.GetDeviceToken() == null || (HasFacebookAccessToken() && !_fbReady))
		{
			return;
		}
		if (ShouldRegisterUser())
		{
			RegisterUser((bool success) =>
			{
			});
		}
		if (ShouldConsolidateFriends())
		{
			ConsolidateFriends((bool success) =>
			{
				if (success)
				{
					if (FacebookSession.HasOpenInstance())
					{
						_consolidationState |= 1;
					}
					if (SocialManagerIOS.instance.gameCenterState == SocialManagerIOS.GCState.LoggedIn)
					{
						_consolidationState |= 2;
					}
				}
				_consolidatedFriendsCompleted = true;
				_lastConsolidateFriendCompleteTime = Time.realtimeSinceStartup;
			});
		}
		else
		{
			friendsConsolidatedHandler__BackingField.SafeInvoke(true);
		}
	}

	private bool ShouldRegisterUser()
	{
		if (PlayerPrefs.HasKey("socmanlastregtime"))
		{
			string text = PlayerPrefs.GetString("socmanlastregtime");
			long result;
			if (long.TryParse(text, out result))
			{
				DateTime dateTime = new DateTime(result);
				int value = OnlineSettings.instance.GetValue("social_register_min_seconds", 3600);
				if (CDateTime.Now >= dateTime + new TimeSpan(0, 0, value))
				{
					return true;
				}
				string fbid;
				string gcid;
				string deviceToken;
				string bundleVersion;
				string highestScore;
				string registerUserData;
				string timezone;
				string locale;
				FetchRegisterUserParams(out fbid, out gcid, out deviceToken, out bundleVersion, out highestScore, out registerUserData, out timezone, out locale);
				if ((!string.IsNullOrEmpty(fbid) || !string.IsNullOrEmpty(gcid)) && _userid == 0)
				{
					return true;
				}
				string text2 = PlayerPrefs.GetString("socmanlastregdata", string.Empty);
				if (text2 != registerUserData)
				{
					return true;
				}
				return false;
			}
			LogError("SocialManager.ShouldRegisterUser() Invalid ticks in playerprefs: " + text);
			return true;
		}
		return true;
	}

	private bool ShouldConsolidateFriends()
	{
		if (!consolidatedFriendsCompleted)
		{
			return true;
		}
		if (HasUnconsolidatedFacebookFriends())
		{
			return true;
		}
		float num = Time.realtimeSinceStartup - _lastConsolidateFriendCompleteTime;
		int value = OnlineSettings.instance.GetValue("social_consolidate_min_seconds", 3600);
		if (num >= (float)value)
		{
			return true;
		}
		return false;
	}

	public void CollectFriendReward(Friend friend)
	{
		friend.status.gamesCashedIn = friend.games;
	}

	public int CashIn(Friend friend, int max)
	{
		int num = friend.games - friend.status.gamesCashedIn;
		if (num > 0)
		{
			friend.status.gamesCashedIn = friend.games;
			return Mathf.Max(num, max);
		}
		return 0;
	}

	public int CashInAll(int maxPerFriend)
	{
		if (_friends == null)
		{
			return 0;
		}
		int num = 0;
		foreach (Friend friend in _friends)
		{
			num += CashIn(friend, maxPerFriend);
		}
		return num;
	}

	private static string GetSaveDataPath()
	{
		return DeviceUtility.GetUserDataPath() + "/socialdata";
	}

	private void OnRemoteNotificationsEnabled(string deviceToken, string oldDeviceToken)
	{
		if (Application.platform != RuntimePlatform.IPhonePlayer && !string.IsNullOrEmpty(oldDeviceToken))
		{
			Flurry.LogEvent("Old Device Token Unregistered");
			FireAndForgetUnregisterDevice(oldDeviceToken);
		}
		Invalidate();
	}

	[UsedImplicitly]
	private void Awake()
	{
		if (_instance != null)
		{
			LogError("SocialManager ERROR: Instance already set - has the script been added directly to a GameObject?", this);
			UnityEngine.Object.Destroy(this);
			return;
		}
		_instance = this;
		CreateFacebookSessionIfNeeded();
		if (PreferencesSave.remoteNotificationsEnabled)
		{
			RemoteNotificationControl.Enable(OnRemoteNotificationsEnabled, "cpush.wav");
		}
		facebookPictureDownloadedHandler += OnFacebookPhotoDownloaded;
		AttemptRelogin();
		Invalidate();
	}

	private void AttemptRelogin()
	{
		if (HasFacebookAccessToken())
		{
			FacebookLogin(null);
		}
	}

	[UsedImplicitly]
	private void OnApplicationFocus(bool focus)
	{
		_appHasFocus = focus;
	}

	[UsedImplicitly]
	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			if (HasFacebookAccessToken())
			{
				FacebookLogin(null);
			}
			if (GameCenterPlugin.GetLocalPlayer().authenticated)
			{
			}
			if (PreferencesSave.remoteNotificationsEnabled && string.IsNullOrEmpty(RemoteNotificationControl.GetDeviceToken()))
			{
				RemoteNotificationControl.Enable(OnRemoteNotificationsEnabled, "cpush.wav");
			}
		}
	}

	[UsedImplicitly]
	private void OnApplicationQuit()
	{
		_applicationIsQuiting = true;
	}

	private void InitAchievements()
	{
		if (!GameCenterPlugin.GetLocalPlayer().authenticated)
		{
			return;
		}
		int context = ++_initAchievementsContext;
		GameCenterPlugin.LoadAchievements((GameCenterPlugin.Achievement[] achievements, string error) =>
		{
			if (context == _initAchievementsContext && achievements != null)
			{
				if (_achievements == null)
				{
					_achievements = new Dictionary<string, GameCenterPlugin.Achievement>();
				}
				foreach (GameCenterPlugin.Achievement achievement in achievements)
				{
					_achievements[achievement.identifier] = achievement;
				}
			}
		});
	}

	public void ReportAchievement(string achievementId, float percentCompleted)
	{
		if (percentCompleted < 0f || percentCompleted > 100f)
		{
			LogError("SocialManager.ReportAchievement: percentCompleted has invalid value: " + percentCompleted + " (must be >= 0  and <= 100)");
		}
		else
		{
			if (!GameCenterPlugin.GetLocalPlayer().authenticated)
			{
				return;
			}
			bool flag = true;
			GameCenterPlugin.Achievement value;
			if (_achievements != null && _achievements.TryGetValue(achievementId, out value))
			{
				flag = percentCompleted > value.percentComplete;
			}
			if (!flag)
			{
				return;
			}
			GameCenterPlugin.ReportAchievement(achievementId, percentCompleted, false, (string error) =>
			{
				if (error == null && _achievements != null)
				{
					GameCenterPlugin.Achievement value2;
					if (!_achievements.TryGetValue(achievementId, out value2))
					{
						value2 = new GameCenterPlugin.Achievement
						{
							identifier = achievementId
						};
						_achievements[achievementId] = value2;
					}
					value2.percentComplete = percentCompleted;
					value2.lastReportedDate = CDateTime.Now;
				}
			});
		}
	}

	private static string GetRandomIdentifier()
	{
		return Guid.NewGuid().ToString();
	}

	public static string GetChecksum(string data)
	{
		return StringUtility.ToHexString(Checksum.ComputeSHA1(data + "resxrctrv7tgv7gb8h9h9u0909kllfmolkjnhghgjjkhjghg"), true);
	}

	private static string GetChecksum(params string[] data)
	{
		return GetChecksum(string.Join(null, data));
	}

	public static IEnumerator WWWRequestCoroutine(WWWComplete onWWWComplete, string relativeUrl, object cookie, params string[] postItems)
	{
		string url = "http://hoodrunner.kiloo.com" + relativeUrl;
		string identifier = GetRandomIdentifier();
		StringBuilder checksumSB = new StringBuilder();
		for (int i = 0; i < postItems.Length; i += 2)
		{
			if (postItems[i] == null)
			{
				LogError("WWWRequestCoroutine: Post item key " + i / 2 + " is null, excluding item");
				continue;
			}
			if (postItems[i + 1] == null)
			{
				LogError("WWWRequestCoroutine: Value for Post item " + postItems[i] + " is null, using empty string");
				postItems[i + 1] = string.Empty;
			}
			checksumSB.Append(postItems[i + 1]);
		}
		string checksum = GetChecksum(identifier + checksumSB);
		WWWForm postData = new WWWForm();
		postData.AddField("identifier", identifier);
		postData.AddField("checksum", checksum);
		StringBuilder sb = new StringBuilder();
		sb.Append("WWWRequest(").Append(url).Append(")\n");
		for (int j = 0; j < postItems.Length; j += 2)
		{
			sb.Append("Adding post data: \"").Append(postItems[j]).Append("\" = \"")
				.Append(postItems[j + 1])
				.Append("\"\n");
			if (postItems[j] != null)
			{
				postData.AddField(postItems[j], postItems[j + 1]);
			}
		}
		WWW www = new WWW(url, postData);
		yield return www;
		if (www.error != null)
		{
			sb.Append("Error: \"").Append(www.error).Append("\"\n");
		}
		else if (www.text != null)
		{
			sb.Append("Text: \"").Append(www.text).Append("\"\n");
		}
		if (onWWWComplete == null)
		{
			yield break;
		}
		if (www.error != null)
		{
			onWWWComplete(WWWRequestResult.Error, null, cookie);
			yield break;
		}
		string text = null;
		if (www.text != null)
		{
			int num = www.text.IndexOf("<result>", StringComparison.OrdinalIgnoreCase);
			if (num >= 0)
			{
				num += 8;
				int num2 = www.text.IndexOf("</result>", num, StringComparison.OrdinalIgnoreCase);
				if (num2 > num)
				{
					text = www.text.Substring(num, num2 - num);
				}
				else if (num2 == num)
				{
					text = string.Empty;
				}
			}
		}
		onWWWComplete((text == null) ? WWWRequestResult.Error : WWWRequestResult.Success, text, cookie);
	}

	private void FireAndForgetUnregisterDevice(string token)
	{
		if (!string.IsNullOrEmpty(token))
		{
			StartCoroutine(WWWRequestCoroutine(null, "/unregisterdevice.php?android", null, "devicetoken", token));
		}
	}

	private void UnregisterDevice(Action<bool> completionHandler)
	{
		Debug.Log("Unregistering for push notification");
		if (canUnregisterDevice)
		{
			if (PreferencesSave.remoteNotificationsEnabled)
			{
				string deviceToken = RemoteNotificationControl.GetDeviceToken();
				if (!string.IsNullOrEmpty(deviceToken))
				{
					canUnregisterDevice = false;
					requestInvolvingDevicetokenInProgress = true;
					StartCoroutine(WWWRequestCoroutine(WWWUnregisterDeviceCompleted, "/unregisterdevice.php?android", completionHandler, "devicetoken", deviceToken));
					return;
				}
			}
			else
			{
				LogError("SocialManager.UnregisterDevice: Already unregistered");
			}
		}
		else
		{
			LogError("SocialManager.UnregisterDevice: Can't unregister before register.");
		}
		if (completionHandler != null)
		{
			completionHandler(false);
		}
	}

	private void WWWUnregisterDeviceCompleted(WWWRequestResult result, string output, object cookie)
	{
		bool obj = false;
		if (result == WWWRequestResult.Success)
		{
			RemoteNotificationControl.Disable();
			obj = true;
			PlayerPrefs.SetString("socmanlastregdata", string.Empty);
		}
		if (cookie != null)
		{
			((Action<bool>)cookie)(obj);
		}
	}

	private void RegisterUser(Action<bool> registerUserCompleted)
	{
		string fbid;
		string gcid;
		string deviceToken;
		string bundleVersion;
		string highestScore;
		string timezone;
		string locale;
		FetchRegisterUserParams(out fbid, out gcid, out deviceToken, out bundleVersion, out highestScore, out _tempRegisterUserData, out timezone, out locale);
		if (string.IsNullOrEmpty(fbid) && string.IsNullOrEmpty(gcid) && string.IsNullOrEmpty(deviceToken))
		{
			if (registerUserCompleted != null)
			{
				registerUserCompleted(false);
			}
			return;
		}
		StartCoroutine(WWWRequestCoroutine(WWWRegisterUserCompleted, "/register2.php?android", registerUserCompleted, "version", bundleVersion, "gcid", gcid, "fbid", fbid, "devicetoken", deviceToken, "score", highestScore, "meters", "0", "games", "0", "rank", GetCurrentRank().ToString(), "timezone", timezone, "locale", locale));
	}

	private int GetCurrentRank()
	{
		return PlayerSave.currentMissionSet / MissionsManager.Instance.GetNumberOfBasicMission();
	}

	private void FetchRegisterUserParams(out string fbid, out string gcid, out string deviceToken, out string bundleVersion, out string highestScore, out string registerUserData, out string timezone, out string locale)
	{
		fbid = ((_fbProfile != null) ? _fbProfile.id : string.Empty);
		bundleVersion = DeviceUtility.GetBundleVersion();
		highestScore = PlayerSave.highScore.ToString();
		timezone = CDateTime.Now.GetCurrentTimeZoneUTCOffset().TotalHours.ToString(CultureInfo.InvariantCulture);
		locale = PreferencesSave.language;
		if (string.IsNullOrEmpty(locale))
		{
			locale = Strings.language.locale.combined;
		}
		deviceToken = RemoteNotificationControl.GetDeviceToken();
		gcid = string.Empty;
		registerUserData = fbid + gcid + deviceToken + bundleVersion + highestScore + locale;
	}

	private void WWWRegisterUserCompleted(WWWRequestResult result, string output, object cookie)
	{
		bool flag = false;
		if (result == WWWRequestResult.Success)
		{
			flag = true;
			canUnregisterDevice = true;
			requestInvolvingDevicetokenInProgress = false;
			IDictionary<string, string> dictionary = StringUtility.ParseProperties(output);
			if (dictionary.ContainsKey("userid"))
			{
				string text = dictionary["userid"];
				string text2 = dictionary["score"];
				string text3 = dictionary["meters"];
				string text4 = dictionary["games"];
				string text5 = dictionary["rank"];
				string a = dictionary["checksum"];
				string checksum = GetChecksum(text, text2, text3, text4, text5);
				if (string.Equals(a, checksum, Utils.stringComparisonIgnoreCase))
				{
					try
					{
						int userid = int.Parse(text);
						int num = int.Parse(text2);
						_userid = userid;
						PlayerSave.bragScoreThreshold = num;
						PlayerSave.SetHighScore(num);
					}
					catch (Exception)
					{
						LogError("Error parsing output data from register user");
						flag = false;
					}
				}
				else
				{
					LogError("Output data from register user corrupted or tampered with");
					flag = false;
				}
			}
		}
		if (flag && !string.IsNullOrEmpty(_tempRegisterUserData))
		{
			string value = CDateTime.Now.Ticks.ToString();
			PlayerPrefs.SetString("socmanlastregtime", value);
			PlayerPrefs.SetString("socmanlastregdata", _tempRegisterUserData);
			_tempRegisterUserData = null;
		}
		_registerUserCompleted = true;
		if (cookie != null)
		{
			((Action<bool>)cookie)(flag);
		}
		Action<bool> action = onRegisterUserCompleted__BackingField;
		if (action != null)
		{
			action(flag);
		}
	}

	private string GetFBListAsStringAndSaveAsPreConsolidated()
	{
		if (_fbProfiles != null)
		{
			if (_latestPreConsolidationFacebookIds == null)
			{
				_latestPreConsolidationFacebookIds = new List<string>();
			}
			else
			{
				_latestPreConsolidationFacebookIds.Clear();
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string key in _fbProfiles.Keys)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(';');
				}
				stringBuilder.Append(key);
				_latestPreConsolidationFacebookIds.Add(key);
			}
			return stringBuilder.ToString();
		}
		return string.Empty;
	}

	private void ConsolidateFriends(Action<bool> consolidateFriendsCompleted)
	{
		string fBListAsStringAndSaveAsPreConsolidated = GetFBListAsStringAndSaveAsPreConsolidated();
		string empty = string.Empty;
		if (string.IsNullOrEmpty(fBListAsStringAndSaveAsPreConsolidated) && string.IsNullOrEmpty(empty))
		{
			if (consolidateFriendsCompleted != null)
			{
				consolidateFriendsCompleted(true);
			}
			FriendConsolidationDidFinish(true);
		}
		else
		{
			StartCoroutine(WWWRequestCoroutine(WWWConsolidateFriendsCompleted, "/friends2.php?android", consolidateFriendsCompleted, "fblist", fBListAsStringAndSaveAsPreConsolidated, "gclist", empty));
		}
	}

	private void WWWConsolidateFriendsCompleted(WWWRequestResult result, string output, object cookie)
	{
		bool flag = false;
		if (result == WWWRequestResult.Success)
		{
			IDictionary<string, string> dictionary = StringUtility.ParseProperties(output);
			if (dictionary.ContainsKey("friendslist"))
			{
				string text = dictionary["friendslist"];
				string a = dictionary["checksum"];
				string checksum = GetChecksum(text);
				if (string.Equals(a, checksum, Utils.stringComparisonIgnoreCase))
				{
					if (string.IsNullOrEmpty(text))
					{
						_friends = null;
					}
					else
					{
						string[][] array = StringUtility.ParseSets(text);
						_friends = new List<Friend>(array.Length);
						string[][] array2 = array;
						foreach (string[] array3 in array2)
						{
							if (array3.Length >= 6 && (array3[1].Length > 0 || array3[2].Length > 0))
							{
								try
								{
									Friend friend = new Friend();
									friend.userid = int.Parse(array3[0]);
									string text2 = array3[2];
									if (text2.Length > 0)
									{
										FacebookProfile value = null;
										if (_fbProfiles != null)
										{
											_fbProfiles.TryGetValue(text2, out value);
										}
										friend.fbProfile = value;
									}
									friend.score = int.Parse(array3[3]);
									friend.meters = int.Parse(array3[4]);
									friend.games = int.Parse(array3[5]);
									friend.rank = ((array3.Length >= 7) ? int.Parse(array3[6]) : 0);
									Friend.Status status = null;
									if (SocialSave.friendStatus == null)
									{
										SocialSave.friendStatus = new Dictionary<string, Friend.Status>();
									}
									if (friend.fbProfile != null && SocialSave.friendStatus.ContainsKey(friend.fbProfile.id))
									{
										status = SocialSave.friendStatus[friend.fbProfile.id];
									}
									else if (friend.gcProfile != null && SocialSave.friendStatus.ContainsKey(friend.gcProfile.id))
									{
										status = SocialSave.friendStatus[friend.gcProfile.id];
									}
									else
									{
										Friend.Status status2 = new Friend.Status();
										status2.gamesCashedIn = friend.games;
										status = status2;
										string key = ((friend.fbProfile == null) ? friend.gcProfile.id : friend.fbProfile.id);
										SocialSave.friendStatus[key] = status;
									}
									friend.status = status;
									_friends.Add(friend);
								}
								catch (Exception ex)
								{
									LogError("Friend parse error " + ex.ToString());
								}
							}
							else
							{
								LogError("Malformed friend: (" + string.Join(";", array3) + ")");
							}
						}
					}
					if (Flurry.ShouldLogDailyEvent("Social Friends Consolidated"))
					{
						int count = 0;
						int num = 0;
						int num2 = 0;
						if (_friends != null)
						{
							count = _friends.Count;
							foreach (Friend friend2 in _friends)
							{
								if (friend2.fbProfile != null)
								{
									num++;
								}
								if (friend2.gcProfile != null)
								{
									num2++;
								}
							}
						}
						string text3 = Flurry.ConvertFriendCountToBracket(count);
						string text4 = Flurry.ConvertFriendCountToBracket(num);
						string text5 = Flurry.ConvertFriendCountToBracket(num2);
						Flurry.LogEventWithSeveralParameters("Social Friends Consolidated", "Total;Facebook;GameCenter", text3 + ";" + text4 + ";" + text5);
					}
					if (_latestPreConsolidationFacebookIds != null)
					{
						_latestSuccesfullyConsolidatedFacebookIds = new List<string>(_latestPreConsolidationFacebookIds);
					}
					flag = true;
				}
				else
				{
					LogError("SocialManager: Consolidated friend data corrupted");
				}
			}
		}
		if (cookie != null)
		{
			((Action<bool>)cookie)(flag);
		}
		FriendConsolidationDidFinish(flag);
	}

	public void ReportScore(int newScore, bool isNewHighscore)
	{
		if (_userid > 0)
		{
			int num = PlayerSave.unreportedGames++;
			if (num == 1)
			{
				PlayerSave.unreportedGamesTimeStamp = CDateTime.UtcNow;
			}
			bool flag = false;
			if (isNewHighscore)
			{
				flag = true;
			}
			else if (num >= OnlineSettings.instance.GetValue("social_report_games_count", 5))
			{
				flag = true;
			}
			else if (num > 1 && CDateTime.UtcNow > PlayerSave.unreportedGamesTimeStamp + GetForceReportScoreTimeSpan())
			{
				flag = true;
			}
			if (flag)
			{
				StartCoroutine(WWWRequestCoroutine(WWWReportScoreCompleted, "/report2.php?android", null, "userid", _userid.ToString(), "score", newScore.ToString(), "games", num.ToString()));
			}
		}
	}

	private void WWWReportScoreCompleted(WWWRequestResult result, string output, object cookie)
	{
		if (result == WWWRequestResult.Success)
		{
			PlayerSave.unreportedGames = 0;
		}
	}

	private TimeSpan GetForceReportScoreTimeSpan()
	{
		int num = OnlineSettings.instance.GetValue("social_report_min_seconds", 1200);
		if (num < 0)
		{
			num = 1200;
		}
		return new TimeSpan(0, 0, num);
	}

	public void UpdateFriendScores(Action<bool> updateFriendsScoresCompleted)
	{
		if (_friends == null || _friends.Count == 0)
		{
			return;
		}
		DateTime now = CDateTime.Now;
		double totalSeconds = (now - _lastFriendScoreUpdateTimestamp).TotalSeconds;
		if (totalSeconds < (double)OnlineSettings.instance.GetValue("social_friendscores_min_seconds", 120))
		{
			return;
		}
		_lastFriendScoreUpdateTimestamp = now;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (Friend friend in _friends)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(';');
			}
			stringBuilder.Append(friend.userid);
		}
		string text = stringBuilder.ToString();
		StartCoroutine(WWWRequestCoroutine(WWWUpdateFriendScoresCompleted, "/scores2.php?android", updateFriendsScoresCompleted, "idlist", text));
	}

	private void WWWUpdateFriendScoresCompleted(WWWRequestResult result, string output, object cookie)
	{
		bool obj = false;
		if (result == WWWRequestResult.Success)
		{
			IDictionary<string, string> dictionary = StringUtility.ParseProperties(output);
			if (dictionary.ContainsKey("scores"))
			{
				string text = dictionary["scores"];
				string a = dictionary["checksum"];
				string checksum = GetChecksum(text);
				if (string.Equals(a, checksum, Utils.stringComparisonIgnoreCase))
				{
					try
					{
						string[][] array = StringUtility.ParseSets(text);
						string[][] array2 = array;
						foreach (string[] array3 in array2)
						{
							if (array3.Length >= 4)
							{
								int userid = int.Parse(array3[0]);
								Friend friend = _friends.Find((Friend f) => f.userid == userid);
								if (friend != null)
								{
									friend.score = int.Parse(array3[1]);
									friend.meters = int.Parse(array3[2]);
									friend.games = int.Parse(array3[3]);
									friend.rank = ((array3.Length >= 5) ? int.Parse(array3[4]) : 0);
								}
								else
								{
									LogWarning("UpdateFriendScores: Unexpected friend user id");
								}
								continue;
							}
							LogError("UpdateFriendScores: Malformed score (" + string.Join(";", array3) + ")");
							throw new Exception();
						}
						obj = true;
					}
					catch (Exception)
					{
						LogError("UpdateFriendScores: Error parsing output data");
					}
				}
				else
				{
					LogError("UpdateFriendScores: Output data corrupt");
				}
			}
		}
		if (cookie != null)
		{
			((Action<bool>)cookie)(obj);
		}
	}

	public string GetNameForRelation(Friend friend)
	{
		if (friend.fbProfile != null)
		{
			return (facebookProfile == null) ? null : facebookProfile.fullName;
		}
		return (gamecenterProfile == null) ? null : gamecenterProfile.alias;
	}

	public void Poke(Friend friend)
	{
		MissionsManager.Instance.PlayerDidThis(MissionTarget.PokeFriend);
		string nameForRelation = GetNameForRelation(friend);
		friend.status.lastPokeTime = CDateTime.UtcNow;
		Flurry.LogEvent("Social friend poked");
		StartCoroutine(WWWRequestCoroutine(null, "/poke.php?android", null, "friend", friend.userid.ToString(), "name", nameForRelation));
	}

	public void SetPokeFirstTime(Friend friend)
	{
		friend.status.lastPokeTime = CDateTime.UtcNow;
	}

	public void BragNotify(int oldScore, List<Friend> friends)
	{
		if (friends == null)
		{
			return;
		}
		int count = friends.Count;
		StringBuilder stringBuilder = new StringBuilder(count * 8);
		StringBuilder stringBuilder2 = new StringBuilder(count * 2);
		foreach (Friend friend in friends)
		{
			int relation = friend.relation;
			int userid = friend.userid;
			if (relation != 0 && userid != 0)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(';');
					stringBuilder2.Append(';');
				}
				stringBuilder.Append(userid);
				stringBuilder2.Append(relation);
			}
		}
		if (stringBuilder.Length > 0)
		{
			string text = ((_fbProfile == null) ? string.Empty : _fbProfile.fullName);
			GameCenterPlugin.LocalPlayer localPlayer = GameCenterPlugin.GetLocalPlayer();
			string text2 = ((!localPlayer.authenticated) ? string.Empty : localPlayer.alias);
			StartCoroutine(WWWRequestCoroutine(null, "/brag.php?android", null, "oldscore", oldScore.ToString(), "newscore", PlayerSave.highScore.ToString(), "useridlist", stringBuilder.ToString(), "relationlist", stringBuilder2.ToString(), "fbname", text, "gcname", text2));
			Flurry.LogEvent("Social bragged");
		}
	}

	public void AnnounceToFriends(int relationsMask)
	{
		if (_friends == null)
		{
			return;
		}
		int count = _friends.Count;
		StringBuilder stringBuilder = new StringBuilder(count * 8);
		StringBuilder stringBuilder2 = new StringBuilder(count * 2);
		foreach (Friend friend in _friends)
		{
			int num = friend.relation & relationsMask;
			int userid = friend.userid;
			if (num != 0 && userid != 0)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(';');
					stringBuilder2.Append(';');
				}
				stringBuilder.Append(userid);
				stringBuilder2.Append(num);
			}
		}
		if (stringBuilder.Length > 0)
		{
			string text;
			string text2;
			if (_fbProfile != null && (relationsMask & 1) != 0)
			{
				text = _fbProfile.fullName;
				text2 = _fbProfile.id;
			}
			else
			{
				text = string.Empty;
				text2 = string.Empty;
			}
			GameCenterPlugin.LocalPlayer localPlayer = GameCenterPlugin.GetLocalPlayer();
			string text3;
			string text4;
			if (localPlayer.authenticated && (relationsMask & 2) != 0)
			{
				text3 = localPlayer.alias;
				text4 = localPlayer.playerID;
			}
			else
			{
				text3 = string.Empty;
				text4 = string.Empty;
			}
			StartCoroutine(WWWRequestCoroutine(null, "/announce.php?android", null, "useridlist", stringBuilder.ToString(), "relationlist", stringBuilder2.ToString(), "fbname", text, "gcname", text3, "fbid", text2, "gcid", text4));
		}
	}

	private static string GetDeviceTypeString()
	{
		return "Android Device";
	}

	public void UpdateRemoteNotificationControl(bool newValue)
	{
		if (PreferencesSave.remoteNotificationsEnabled && !newValue)
		{
			if (canUnregisterDevice)
			{
				UnregisterDevice((bool success) =>
				{
					requestInvolvingDevicetokenInProgress = false;
				});
			}
		}
		else
		{
			requestInvolvingDevicetokenInProgress = true;
			RemoteNotificationControl.Enable(OnRemoteNotificationsEnabled, "cpush.wav");
		}
	}

	public bool AreFacebookFriendsConsolidated()
	{
		return (consolidationState & 1) == 1;
	}

	public static void LogError(string msg, UnityEngine.Object context = null)
	{
		SDebug.LogError(msg, context);
	}

	public static void LogWarning(string msg, UnityEngine.Object context = null)
	{
	}

	[Conditional("SOCIAL_DEBUG_LOGS")]
	public static void Log(string msg, UnityEngine.Object context = null)
	{
	}
}
