#define ENABLE_ERROR_LOGS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Data.Save;
using Kiloo;
using Kiloo.GameCenter;
using SYBO.Common.Utilities;
using UnityEngine;

public class SocialManagerIOS : MonoBehaviour
{
	public enum GCState
	{
		LoggedOut = 0,
		Authenticating = 1,
		LoadingFriends = 2,
		LoggedIn = 3
	}

	private const string ONLINESETTINGS_FILTER_GC_FRIENDS_KEY = "social_filter_gc_friends";

	private const bool FILTER_GC_FRIENDS_USING_LEADERBOARD_DEFAULT = true;

	private const string GC_LEADERBOARD_ID = "com.kiloo.subwaysurfers.ScoreLeaderboard";

	private static SocialManagerIOS _instance;

	private GCState _gcState;

	private Dictionary<string, GameCenterProfile> _gcFriends;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<GameCenterProfile> playerPhotoDownloadedHandler__BackingField;

	private bool _pausePhotoDownloading;

	private int _photoDownloadContext;

	private List<string> _latestPreConsolidationGCIds;

	private List<string> _latestSuccesfullyConsolidationGCIds;

	private static bool _applicationIsQuiting;

	private int _loadFriendsContext;

	private GameCenterPlugin.ShowViewState _lastViewState;

	private string _lastLeaderboardCategory = "com.kiloo.subwaysurfers.ScoreLeaderboard";

	private GameCenterPlugin.LeaderboardTimeScope _lastTimeScope;

	private Action _invalidate;

	public GCState gameCenterState
	{
		get
		{
			return _gcState;
		}
		set
		{
			if (_gcState != value)
			{
				_gcState = value;
			}
		}
	}

	public bool pausePhotoDownloading
	{
		get
		{
			return _pausePhotoDownloading;
		}
		set
		{
			_pausePhotoDownloading = value;
		}
	}

	public static SocialManagerIOS instance
	{
		get
		{
			Init();
			return _instance;
		}
	}

	public static bool isInstanced
	{
		get
		{
			return _instance != null;
		}
	}

	private static event Action<GameCenterProfile> playerPhotoDownloadedHandler
	{
		add
		{
			Action<GameCenterProfile> action = playerPhotoDownloadedHandler__BackingField;
			Action<GameCenterProfile> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref playerPhotoDownloadedHandler__BackingField, (Action<GameCenterProfile>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<GameCenterProfile> action = playerPhotoDownloadedHandler__BackingField;
			Action<GameCenterProfile> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref playerPhotoDownloadedHandler__BackingField, (Action<GameCenterProfile>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private void OnApplicationQuit()
	{
		_applicationIsQuiting = true;
	}

	private static void Init()
	{
		if (_instance == null && !_applicationIsQuiting)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "SocialManagerIOS";
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<SocialManagerIOS>();
			HierarchyUtility.ReparentByHierarchyPathType(gameObject.transform, HierarchyPathType.Dynamic);
			HierarchyUtility.SetAsLastSibling(gameObject.transform);
			HierarchyUtility.ReparentByHierarchyPathType(gameObject.transform, HierarchyPathType.DynamicManagers);
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	[Conditional("UNITY_IPHONE")]
	public void LoadFriends()
	{
		if (GameCenterPlugin.GetLocalPlayer().authenticated)
		{
			int context = ++_loadFriendsContext;
			gameCenterState = GCState.LoadingFriends;
			GameCenterPlugin.LoadLeaderboard("com.kiloo.subwaysurfers.ScoreLeaderboard", GameCenterPlugin.LeaderboardPlayerScope.FriendsOnly, GameCenterPlugin.LeaderboardTimeScope.AllTime, 1, 100, (GameCenterPlugin.Leaderboard leaderboard, string leaderboardError) =>
			{
				if (context == _loadFriendsContext)
				{
					if (leaderboard != null)
					{
						Dictionary<string, GameCenterProfile> dictionary = new Dictionary<string, GameCenterProfile>(leaderboard.scores.Length);
						GameCenterPlugin.Score localPlayerScore = leaderboard.localPlayerScore;
						long value = localPlayerScore.value;
						if (value < PlayerSave.highScore)
						{
						}
						int num = 0;
						GameCenterPlugin.Score[] scores = leaderboard.scores;
						for (int i = 0; i < scores.Length; i++)
						{
							GameCenterPlugin.Score score = scores[i];
							if (score.playerID != localPlayerScore.playerID)
							{
								GameCenterProfile value2;
								if (_gcFriends == null || !_gcFriends.TryGetValue(score.playerID, out value2))
								{
									value2 = new GameCenterProfile
									{
										id = score.playerID
									};
								}
								dictionary[score.playerID] = value2;
								num++;
							}
						}
						_gcFriends = dictionary;
						Dictionary<string, GameCenterProfile>.KeyCollection keys = _gcFriends.Keys;
						string[] array = new string[keys.Count];
						int num2 = 0;
						foreach (string item in keys)
						{
							array[num2++] = item;
						}
						GameCenterPlugin.LoadPlayers(array, (GameCenterPlugin.Player[] players, string playersError) =>
						{
							if (context == _loadFriendsContext)
							{
								if (players != null)
								{
									for (int j = 0; j < players.Length; j++)
									{
										GameCenterPlugin.Player player = players[j];
										GameCenterProfile value3;
										if (_gcFriends != null && _gcFriends.TryGetValue(player.playerID, out value3))
										{
											value3.alias = player.alias;
											value3.displayName = player.displayName;
										}
									}
								}
								StartCoroutine(RefreshFriendImages(_gcFriends));
							}
							else
							{
								LogWarning("SocialManagerIOS.LoadFriends: Loading players complete but was started in another context.");
							}
						});
					}
					else
					{
						_gcFriends = null;
					}
				}
				else
				{
					LogWarning("SocialManagerIOS.LoadFriends: Loading leaderboard complete but was started in another context.");
				}
			});
		}
		else
		{
			LogError("SocialManagerIOS.LoadFriends: Not authenticated");
		}
	}

	private IEnumerator RefreshFriendImages(Dictionary<string, GameCenterProfile> gcProfiles)
	{
		int photoDownloadContext = ++_photoDownloadContext;
		List<GameCenterProfile> profiles = new List<GameCenterProfile>(gcProfiles.Count);
		foreach (GameCenterProfile value in gcProfiles.Values)
		{
			if (value.image == null)
			{
				profiles.Add(value);
			}
		}
		foreach (GameCenterProfile profile in profiles)
		{
			yield return StartCoroutine(DownloadPictureForProfile(profile, SocialManager.SocialPictureSize.Normal, (GameCenterProfile prof, Texture2D picture) =>
			{
				profile.image = picture;
			}));
			while (pausePhotoDownloading && photoDownloadContext == _photoDownloadContext)
			{
				yield return null;
			}
			if (photoDownloadContext != _photoDownloadContext)
			{
				break;
			}
		}
	}

	public IEnumerator DownloadPictureForProfile(GameCenterProfile profile, SocialManager.SocialPictureSize pictureSize, Action<GameCenterProfile, Texture2D> handler)
	{
		DateTime lastRefresh = profile.refreshedTime;
		if (!(profile.image == null) && !((CDateTime.Now - lastRefresh).TotalSeconds > 3600.0))
		{
			yield break;
		}
		int textureDimension = ((pictureSize != SocialManager.SocialPictureSize.Large) ? 50 : 250);
		GameCenterPlugin.LoadPhoto(profile.id, GameCenterPlugin.PhotoSize.Small, textureDimension, textureDimension, (string playerID, Texture2D photo, string error) =>
		{
			profile.refreshedTime = CDateTime.Now;
			if (error == null && handler != null)
			{
				handler(profile, photo);
			}
		});
		while (profile.refreshedTime == lastRefresh)
		{
			yield return null;
		}
	}

	public void AbortAsynchronousProcesses()
	{
		_photoDownloadContext++;
		_loadFriendsContext++;
	}

	[Conditional("UNITY_IPHONE")]
	private void FlurryLogGCFriendsRetrieved(int total, int withGameInstalled)
	{
		if (Flurry.ShouldLogDailyEvent("Social GameCenter Friends Retrieved"))
		{
			string text = Flurry.ConvertFriendCountToBracket(total);
			string text2 = Flurry.ConvertFriendCountToBracket(withGameInstalled);
			Flurry.LogEventWithSeveralParameters("Social GameCenter Friends Retrieved", "Total friends;Installed friends", text + ";" + text2);
		}
	}

	[Conditional("UNITY_IPHONE")]
	private void PerformLoadFriendsDoneActions()
	{
		gameCenterState = GCState.LoggedIn;
		Action invalidate = _invalidate;
		if (invalidate != null)
		{
			invalidate();
		}
	}

	public bool IsDoneAuthenticating()
	{
		return gameCenterState == GCState.LoggedIn || gameCenterState == GCState.LoggedOut;
	}

	[Conditional("UNITY_IPHONE")]
	public void ReportScore(int newScore)
	{
		if (!GameCenterPlugin.GetLocalPlayer().authenticated)
		{
			return;
		}
		GameCenterPlugin.ReportScore("com.kiloo.subwaysurfers.ScoreLeaderboard", newScore, 0uL, (string error) =>
		{
			if (error != null)
			{
				LogError("SocialManagerIOS.ReportToGameCenter: Error reporting score to GameCenter");
			}
		});
	}

	[Conditional("UNITY_IPHONE")]
	public void ShowLeaderBoard()
	{
		GameCenterPlugin.ShowView(_lastViewState, _lastLeaderboardCategory, _lastTimeScope, (GameCenterPlugin.ShowViewState viewState, string leaderboardCategory, GameCenterPlugin.LeaderboardTimeScope leaderboardTimescope) =>
		{
			_lastViewState = viewState;
			_lastLeaderboardCategory = leaderboardCategory;
			_lastTimeScope = leaderboardTimescope;
		});
	}

	public string GetGCListAsStringAndSaveAsPreConsolidated()
	{
		if (_latestPreConsolidationGCIds == null)
		{
			_latestPreConsolidationGCIds = new List<string>();
		}
		else
		{
			_latestPreConsolidationGCIds.Clear();
		}
		string result;
		if (_gcFriends != null)
		{
			Dictionary<string, GameCenterProfile>.KeyCollection keys = _gcFriends.Keys;
			string[] array = new string[keys.Count];
			int num = 0;
			foreach (string item in keys)
			{
				array[num++] = item;
			}
			result = string.Join(";", array);
			_latestPreConsolidationGCIds.AddRange(array);
		}
		else
		{
			result = string.Empty;
		}
		return result;
	}

	public void HandleConsolidatedFriendsSuccess()
	{
		_latestSuccesfullyConsolidationGCIds = _latestPreConsolidationGCIds;
		_latestPreConsolidationGCIds = null;
	}

	public bool HasUnconsolidatedGCFriends()
	{
		if (_gcFriends != null)
		{
			if (_latestSuccesfullyConsolidationGCIds == null)
			{
				return true;
			}
			foreach (string key in _gcFriends.Keys)
			{
				if (!_latestSuccesfullyConsolidationGCIds.Contains(key))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void ClearFriends()
	{
		_latestSuccesfullyConsolidationGCIds = null;
		_latestPreConsolidationGCIds = null;
		_gcFriends = null;
	}

	public bool GCFriendsContainKey(string key)
	{
		return _gcFriends != null && _gcFriends.ContainsKey(key);
	}

	public GameCenterProfile TryGetUserProfileFromKey(string key)
	{
		GameCenterProfile value;
		if (_gcFriends != null && _gcFriends.TryGetValue(key, out value))
		{
			return value;
		}
		return null;
	}

	public void AddInvalidateHandler(Action handler)
	{
		_invalidate = (Action)Delegate.Combine(_invalidate, handler);
	}

	[Conditional("UNITY_IPHONE")]
	public static void AddPlayerPhotoDownloadedHandler(Action<GameCenterProfile> handler)
	{
		playerPhotoDownloadedHandler += handler;
	}

	[Conditional("UNITY_IPHONE")]
	public static void RemovePlayerPhotoDownloadedHandler(Action<GameCenterProfile> handler)
	{
		playerPhotoDownloadedHandler -= handler;
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogError(string msg, UnityEngine.Object context = null)
	{
		SDebug.LogError(msg, context);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogWarning(string msg, UnityEngine.Object context = null)
	{
	}

	[Conditional("SOCIAL_DEBUG_LOGS")]
	public static void Log(string msg, UnityEngine.Object context = null)
	{
	}
}
