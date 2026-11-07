using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Kiloo.Common.Utility;
using Kiloo.Plugins;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.GameCenter
{
	public class GameCenterPlugin : MonoBehaviour
	{
		public struct Player
		{
			public string playerID;

			public string alias;

			public string displayName;

			public bool isFriend;
		}

		public struct LocalPlayer
		{
			public string playerID;

			public string alias;

			public string displayName;

			public bool underage;

			public bool authenticated;
		}

		public struct Score
		{
			public string playerID;

			public int rank;

			public long value;

			public string formattedValue;

			public DateTime date;

			public ulong context;
		}

		public class Leaderboard
		{
			public string category;

			public int rangeBegin;

			public int rangeCount;

			public LeaderboardPlayerScope playerScope;

			public LeaderboardTimeScope timeScope;

			public Score localPlayerScore;

			public Score[] scores;

			public string groupIdentifier;

			public uint maxRange;

			public string title;
		}

		public class Achievement
		{
			public string identifier;

			public float percentComplete;

			public DateTime lastReportedDate;

			public bool complete
			{
				get
				{
					return percentComplete >= 100f;
				}
			}
		}

		public class AchievementDescription
		{
			public string identifier;

			public string title;

			public string unachievedDescription;

			public string achievedDescription;

			public int maximumPoints;

			public bool replayable;

			public bool hidden;

			public string groupIdentifier;
		}

		public enum PhotoSize
		{
			Small = 0,
			Normal = 1
		}

		public enum ShowViewState
		{
			Default = -1,
			LeaderBoards = 0,
			Achievements = 1,
			Challenges = 2
		}

		public enum LeaderboardTimeScope
		{
			Today = 0,
			Week = 1,
			AllTime = 2
		}

		public enum LeaderboardPlayerScope
		{
			Global = 0,
			FriendsOnly = 1
		}

		public delegate void AuthenticateHandler(bool hasLoginView, int errorCode, string error);

		public delegate void GenerateIdentityVerificationSignatureCompletionHandler(string publicKeyUrl, byte[] signature, byte[] salt, ulong timestamp, string error);

		public delegate void LoadFriendsCompletionHandler(string[] friends, string error);

		public delegate void LoadPlayersCompletionHandler(Player[] players, string error);

		public delegate void LoadPhotoCompletionHandler(string playerID, Texture2D photo, string error);

		private delegate void InternalLoadPhotoCompletionHandler(int imgWidth, int imgHeight, string error);

		public delegate void ViewDismissedHandler(ShowViewState viewState, string leaderboardCategory, LeaderboardTimeScope leaderboardTimeScope);

		public delegate void RequestCompleteHandler(string error);

		public delegate void LoadLeaderboardCompletionHandler(Leaderboard leaderboard, string error);

		public delegate void LoadAchievementsCompletionHandler(Achievement[] achievements, string error);

		public delegate void LoadAchievementDescriptionsCompletionHandler(AchievementDescription[] achievements, string error);

		[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
		private class GenerateIdentityVerificationSignatureResult
		{
			public int context;

			public string publicKeyUrl;

			public string signature;

			public string salt;

			public ulong timestamp;

			public string error;
		}

		private const char SEPARATION_CHAR = '|';

		private const string ESCAPED_SEPARATION_CHAR = "&#124;";

		private bool _hasLoginView;

		private bool _initialAuthenticationComplete;

		private ViewDismissedHandler _viewDismissedHandler;

		private static GameCenterPlugin _instance;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static AuthenticateHandler authenticationEvent__BackingField;

		public static bool isSupported
		{
			get
			{
				return Application.platform == RuntimePlatform.IPhonePlayer;
			}
		}

		public static bool hasLoginView
		{
			get
			{
				return _instance != null && _instance._hasLoginView;
			}
		}

		public static bool initialAuthenticationComplete
		{
			get
			{
				return _instance != null && _instance._initialAuthenticationComplete;
			}
		}

		public static event AuthenticateHandler authenticationEvent
		{
			add
			{
				AuthenticateHandler authenticateHandler = authenticationEvent__BackingField;
				AuthenticateHandler authenticateHandler2;
				do
				{
					authenticateHandler2 = authenticateHandler;
					authenticateHandler = Interlocked.CompareExchange(ref authenticationEvent__BackingField, (AuthenticateHandler)Delegate.Combine(authenticateHandler2, value), authenticateHandler);
				}
				while ((object)authenticateHandler != authenticateHandler2);
			}
			remove
			{
				AuthenticateHandler authenticateHandler = authenticationEvent__BackingField;
				AuthenticateHandler authenticateHandler2;
				do
				{
					authenticateHandler2 = authenticateHandler;
					authenticateHandler = Interlocked.CompareExchange(ref authenticationEvent__BackingField, (AuthenticateHandler)Delegate.Remove(authenticateHandler2, value), authenticateHandler);
				}
				while ((object)authenticateHandler != authenticateHandler2);
			}
		}

		[DllImport("__Internal")]
		[Conditional("UNITY_IPHONE")]
		private static extern void _GameCenterInit(string gameObject);

		[DllImport("__Internal")]
		[Conditional("UNITY_IPHONE")]
		private static extern void _GameCenterShowLoginView();

		[DllImport("__Internal")]
		[Conditional("UNITY_IPHONE")]
		private static extern void _GameCenterGetLocalPlayer(ref LocalPlayer localPlayer);

		[DllImport("__Internal")]
		[Conditional("UNITY_IPHONE")]
		private static extern void _GameCenterGenerateIdentityVerificationSignature(int handlerContext);

		[DllImport("__Internal")]
		[Conditional("UNITY_IPHONE")]
		private static extern void _GameCenterLoadFriends(int handlerContext);

		[DllImport("__Internal")]
		[Conditional("UNITY_IPHONE")]
		private static extern void _GameCenterLoadPlayers(string playerIDs, int handlerContext);

		[DllImport("__Internal")]
		[Conditional("UNITY_IPHONE")]
		private static extern void _GameCenterLoadPhoto(string playerID, int size, int handlerContext);

		private static bool _GameCenterArePhotosSupported()
		{
			return false;
		}

		private static bool _GameCenterGetPhotoPixels(int context, int width, int height, IntPtr pixels)
		{
			return false;
		}

		[DllImport("__Internal")]
		[Conditional("UNITY_IPHONE")]
		private static extern void _GameCenterShowView(int viewState, string leaderboardCategory, int leaderboardTimeScope);

		[DllImport("__Internal")]
		[Conditional("UNITY_IPHONE")]
		private static extern void _GameCenterReportScore(string category, long scoreValue, ulong scoreContext, int handlerContext);

		[DllImport("__Internal")]
		[Conditional("UNITY_IPHONE")]
		private static extern void _GameCenterLoadLeaderboard(string category, int playerScope, int timeScope, int rangeBegin, int rangeCount, int handlerContext);

		[DllImport("__Internal")]
		[Conditional("UNITY_IPHONE")]
		private static extern void _GameCenterLoadLeaderboardForIDs(string category, string playerIDs, int timeScope, int handlerContext);

		[DllImport("__Internal")]
		[Conditional("UNITY_IPHONE")]
		private static extern void _GameCenterReportAchievement(string achievementID, float percentComplete, int showsCompletionBanner, int handlerContext);

		[DllImport("__Internal")]
		[Conditional("UNITY_IPHONE")]
		private static extern void _GameCenterReportAchievements(string interleavedAchievements, int showsCompletionBanner, int handlerContext);

		[DllImport("__Internal")]
		[Conditional("UNITY_IPHONE")]
		private static extern void _GameCenterLoadAchievements(int handlerContext);

		[DllImport("__Internal")]
		[Conditional("UNITY_IPHONE")]
		private static extern void _GameCenterLoadAchievementDescriptions(int handlerContext);

		[DllImport("__Internal")]
		[Conditional("UNITY_IPHONE")]
		private static extern void _GameCenterResetAchievements(int handlerContext);

		public static void Init()
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance == null)
				{
					_instance = PluginUtil.CreateBridgeObject<GameCenterPlugin>();
				}
			}
			else
			{
				LogError("Could not authenticate local player (not on iOS)");
			}
		}

		public static void ShowLoginView()
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					if (_instance._hasLoginView)
					{
						_instance._hasLoginView = false;
					}
				}
				else
				{
					LogError("ShowLoginView was called before Init");
				}
			}
			else
			{
				LogError("Could not show login view (not on iOS)");
			}
		}

		public static LocalPlayer GetLocalPlayer()
		{
			LocalPlayer result = default(LocalPlayer);
			if (Application.platform == RuntimePlatform.IPhonePlayer && result.displayName == null)
			{
				result.displayName = result.alias;
			}
			return result;
		}

		public static void GetLocalPlayer(ref LocalPlayer player)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (player.displayName == null)
				{
					player.displayName = player.alias;
				}
			}
			else
			{
				player = default(LocalPlayer);
			}
		}

		public static void GenerateIdentityVerificationSignature([CanBeNull] GenerateIdentityVerificationSignatureCompletionHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = Depository.CheckIn(completionHandler, _instance);
				}
				else
				{
					LogError("GenerateIdentityVerificationSignature was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler(null, null, null, 0uL, "Could not generate identity verification signature (not on iOS)");
			}
		}

		public static Coroutine GenerateIdentityVerificationSignature([NotNull] GameCenterGenerateIdentityVerificationSignatureRequest request)
		{
			return CoroutineUtil.StartCoroutine(GenerateIdentityVerificationSignatureCoroutine(request));
		}

		private static IEnumerator GenerateIdentityVerificationSignatureCoroutine([NotNull] GameCenterGenerateIdentityVerificationSignatureRequest request)
		{
			bool working = true;
			bool canceled = false;
			GenerateIdentityVerificationSignatureCompletionHandler onComplete = (string publicKeyUrl, byte[] signature, byte[] salt, ulong timestamp, string error) =>
			{
				if (!canceled)
				{
					request.publicKeyUrl = publicKeyUrl;
					request.signature = signature;
					request.salt = salt;
					request.timestamp = timestamp;
					request.error = error;
				}
				working = false;
			};
			request.isDone = false;
			GenerateIdentityVerificationSignature(onComplete);
			float startTime = Time.realtimeSinceStartup;
			while (working)
			{
				if (request.timeout > 0f && Time.realtimeSinceStartup - startTime > request.timeout)
				{
					canceled = true;
					request.didTimeout = true;
					break;
				}
				yield return null;
			}
			request.isDone = true;
		}

		public static void LoadFriends([CanBeNull] LoadFriendsCompletionHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = Depository.CheckIn(completionHandler, _instance);
				}
				else
				{
					LogError("LoadFriends was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler(null, "Could not load friends (not on iOS)");
			}
		}

		public static Coroutine LoadFriends([NotNull] GameCenterLoadFriendsRequest request)
		{
			return CoroutineUtil.StartCoroutine(LoadFriendsCoroutine(request));
		}

		private static IEnumerator LoadFriendsCoroutine([NotNull] GameCenterLoadFriendsRequest request)
		{
			bool working = true;
			bool canceled = false;
			LoadFriendsCompletionHandler onComplete = (string[] result, string error) =>
			{
				if (!canceled)
				{
					request.result = result;
					request.error = error;
				}
				working = false;
			};
			request.isDone = false;
			LoadFriends(onComplete);
			float startTime = Time.realtimeSinceStartup;
			while (working)
			{
				if (request.timeout > 0f && Time.realtimeSinceStartup - startTime > request.timeout)
				{
					canceled = true;
					request.didTimeout = true;
					break;
				}
				yield return null;
			}
			request.isDone = true;
		}

		public static void LoadPlayers([NotNull] string[] playerIDs, [CanBeNull] LoadPlayersCompletionHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = Depository.CheckIn(completionHandler, _instance);
				}
				else
				{
					LogError("LoadPlayers was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler(null, "Could not load players (not on iOS)");
			}
		}

		public static Coroutine LoadPlayers([NotNull] GameCenterLoadPlayersRequest request)
		{
			return CoroutineUtil.StartCoroutine(LoadPlayersCoroutine(request));
		}

		private static IEnumerator LoadPlayersCoroutine([NotNull] GameCenterLoadPlayersRequest request)
		{
			bool working = true;
			bool canceled = false;
			LoadPlayersCompletionHandler onComplete = (Player[] result, string error) =>
			{
				if (!canceled)
				{
					request.result = result;
					request.error = error;
				}
				working = false;
			};
			request.isDone = false;
			LoadPlayers(request.playerIDs, onComplete);
			float startTime = Time.realtimeSinceStartup;
			while (working)
			{
				if (request.timeout > 0f && Time.realtimeSinceStartup - startTime > request.timeout)
				{
					canceled = true;
					request.didTimeout = true;
					break;
				}
				yield return null;
			}
			request.isDone = true;
		}

		public static bool ArePhotosSupported()
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					return _GameCenterArePhotosSupported();
				}
				LogError("ArePhotosSupported was called before Init");
			}
			return false;
		}

		public static void LoadPhoto([NotNull] string playerID, PhotoSize size, int textureWidth, int textureHeight, [CanBeNull] LoadPhotoCompletionHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int context = 0;
					context = Depository.CheckIn((InternalLoadPhotoCompletionHandler)((int imgWidth, int imgHeight, string error) =>
					{
						Texture2D texture2D = null;
						if (imgWidth != 0 && imgHeight != 0)
						{
							Color32[] array = new Color32[textureWidth * textureHeight];
							GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
							bool flag = _GameCenterGetPhotoPixels(context, textureWidth, textureHeight, gCHandle.AddrOfPinnedObject());
							gCHandle.Free();
							if (flag)
							{
								texture2D = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
								texture2D.wrapMode = TextureWrapMode.Clamp;
								texture2D.SetPixels32(array);
								texture2D.Apply();
							}
						}
						if (completionHandler != null)
						{
							completionHandler(playerID, texture2D, error);
						}
					}), _instance);
				}
				else
				{
					LogError("LoadPhoto was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler(playerID, null, "Could not load photo (not on iOS)");
			}
		}

		public static Coroutine LoadPhoto([NotNull] GameCenterLoadPhotoRequest request)
		{
			return CoroutineUtil.StartCoroutine(LoadPhotoCoroutine(request));
		}

		private static IEnumerator LoadPhotoCoroutine([NotNull] GameCenterLoadPhotoRequest request)
		{
			bool working = true;
			bool canceled = false;
			LoadPhotoCompletionHandler onComplete = (string id, Texture2D result, string error) =>
			{
				if (!canceled)
				{
					request.result = result;
					request.error = error;
				}
				working = false;
			};
			request.isDone = false;
			LoadPhoto(request.playerID, request.size, request.textureWidth, request.textureHeight, onComplete);
			float startTime = Time.realtimeSinceStartup;
			while (working)
			{
				if (request.timeout > 0f && Time.realtimeSinceStartup - startTime > request.timeout)
				{
					canceled = true;
					request.didTimeout = true;
					break;
				}
				yield return null;
			}
			request.isDone = true;
		}

		public static void ShowView(ShowViewState viewState, [CanBeNull] string leaderboardCategory = null, LeaderboardTimeScope leaderboardTimeScope = LeaderboardTimeScope.AllTime, [CanBeNull] ViewDismissedHandler viewDismissedHandler = null)
		{
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				return;
			}
			if (_instance != null)
			{
				if (_instance._viewDismissedHandler != null)
				{
					LogWarning("ShowView called while a view is already shown. Only one dismissed handler can be set at a time.");
				}
				_instance._viewDismissedHandler = viewDismissedHandler;
			}
			else
			{
				LogError("ShowView was called before Init");
			}
		}

		public static void ReportScore(string category, long scoreValue, ulong scoreContext, [CanBeNull] RequestCompleteHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = Depository.CheckIn(completionHandler, _instance);
				}
				else
				{
					LogError("ReportScore was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler("Could not report score (not on iOS)");
			}
		}

		public static Coroutine ReportScore([NotNull] GameCenterReportScoreRequest request)
		{
			return CoroutineUtil.StartCoroutine(RequestCoroutine(request, (RequestCompleteHandler completeHandler) =>
			{
				ReportScore(request.category, request.scoreValue, request.scoreContext, completeHandler);
			}));
		}

		public static void LoadLeaderboard([CanBeNull] string category, LeaderboardPlayerScope playerScope, LeaderboardTimeScope timeScope, int rangeBegin, int rangeCount, [CanBeNull] LoadLeaderboardCompletionHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = Depository.CheckIn((LoadLeaderboardCompletionHandler)((Leaderboard leaderboard, string error) =>
					{
						if (completionHandler != null)
						{
							if (leaderboard != null)
							{
								leaderboard.category = category;
								leaderboard.playerScope = playerScope;
								leaderboard.timeScope = timeScope;
								leaderboard.rangeBegin = rangeBegin;
								leaderboard.rangeCount = rangeCount;
							}
							completionHandler(leaderboard, error);
						}
					}), _instance);
				}
				else
				{
					LogError("LoadLeaderboard was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler(null, "Could not load leaderboard (not on iOS)");
			}
		}

		public static Coroutine LoadLeaderboard([NotNull] GameCenterLoadLeaderboardRequest request)
		{
			return CoroutineUtil.StartCoroutine(LoadLeaderboardCoroutine(request));
		}

		private static IEnumerator LoadLeaderboardCoroutine([NotNull] GameCenterLoadLeaderboardRequest request)
		{
			bool working = true;
			bool canceled = false;
			LoadLeaderboardCompletionHandler onComplete = (Leaderboard result, string error) =>
			{
				if (!canceled)
				{
					request.result = result;
					request.error = error;
				}
				working = false;
			};
			request.isDone = false;
			if (request.playerIDs != null)
			{
				LoadLeaderboardForIDs(request.category, request.playerIDs, request.timeScope, onComplete);
			}
			else
			{
				LoadLeaderboard(request.category, request.playerScope, request.timeScope, request.rangeBegin, request.rangeCount, onComplete);
			}
			float startTime = Time.realtimeSinceStartup;
			while (working)
			{
				if (request.timeout > 0f && Time.realtimeSinceStartup - startTime > request.timeout)
				{
					canceled = true;
					request.didTimeout = true;
					break;
				}
				yield return null;
			}
			request.isDone = true;
		}

		public static void LoadLeaderboardForIDs([CanBeNull] string category, [NotNull] string[] playerIDs, LeaderboardTimeScope timeScope, [CanBeNull] LoadLeaderboardCompletionHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = Depository.CheckIn((LoadLeaderboardCompletionHandler)((Leaderboard leaderboard, string error) =>
					{
						if (completionHandler != null)
						{
							if (leaderboard != null)
							{
								leaderboard.category = category;
								leaderboard.timeScope = timeScope;
							}
							completionHandler(leaderboard, error);
						}
					}), _instance);
				}
				else
				{
					LogError("LoadLeaderboardForIDs was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler(null, "Could not load leaderboard for ids (not on iOS)");
			}
		}

		public static void ReportAchievement([NotNull] string identifier, float percentComplete, bool showsCompletionBanner, [CanBeNull] RequestCompleteHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = Depository.CheckIn(completionHandler, _instance);
				}
				else
				{
					LogError("ReportAchievement was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler("Could not report achievement (not on iOS)");
			}
		}

		public static void ReportAchievement([NotNull] Achievement achievement, bool showsCompletionBanner, [CanBeNull] RequestCompleteHandler completionHandler)
		{
			ReportAchievement(achievement.identifier, achievement.percentComplete, showsCompletionBanner, completionHandler);
		}

		public static void ReportAchievements([NotNull] Achievement[] achievements, bool showsCompletionBanner, [CanBeNull] RequestCompleteHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = Depository.CheckIn(completionHandler, _instance);
					StringBuilder stringBuilder = new StringBuilder();
					bool flag = false;
					foreach (Achievement achievement in achievements)
					{
						if (flag)
						{
							stringBuilder.Append('|');
						}
						else
						{
							flag = true;
						}
						stringBuilder.Append(achievement.identifier).Append('|').Append(achievement.percentComplete);
					}
				}
				else
				{
					LogError("ReportAchievements was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler("Could not report achievements (not on iOS)");
			}
		}

		public static Coroutine ReportAchievements([NotNull] GameCenterReportAchievementsRequest request)
		{
			return CoroutineUtil.StartCoroutine(RequestCoroutine(request, (RequestCompleteHandler completeHandler) =>
			{
				ReportAchievements(request.achievements, request.showsCompletionBanner, completeHandler);
			}));
		}

		public static void LoadAchievements([CanBeNull] LoadAchievementsCompletionHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = Depository.CheckIn(completionHandler, _instance);
				}
				else
				{
					LogError("LoadAchievements was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler(null, "Could not load achievements (not on iOS)");
			}
		}

		public static Coroutine LoadAchievements(GameCenterLoadAchievementsRequest request)
		{
			return CoroutineUtil.StartCoroutine(LoadAchievementsCoroutine(request));
		}

		private static IEnumerator LoadAchievementsCoroutine([NotNull] GameCenterLoadAchievementsRequest request)
		{
			bool working = true;
			bool canceled = false;
			LoadAchievementsCompletionHandler onComplete = (Achievement[] result, string error) =>
			{
				if (!canceled)
				{
					request.result = result;
					request.error = error;
				}
				working = false;
			};
			request.isDone = false;
			LoadAchievements(onComplete);
			float startTime = Time.realtimeSinceStartup;
			while (working)
			{
				if (request.timeout > 0f && Time.realtimeSinceStartup - startTime > request.timeout)
				{
					canceled = true;
					request.didTimeout = true;
					break;
				}
				yield return null;
			}
			request.isDone = true;
		}

		public static void LoadAchievementDescriptions([CanBeNull] LoadAchievementDescriptionsCompletionHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = Depository.CheckIn(completionHandler, _instance);
				}
				else
				{
					LogError("LoadAchievementDescriptions was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler(null, "Could not load achievement descriptions (not on iOS)");
			}
		}

		public static Coroutine LoadAchievementDescriptions([NotNull] GameCenterLoadAchievementDescriptionsRequest request)
		{
			return CoroutineUtil.StartCoroutine(LoadAchievementDescriptionsCoroutine(request));
		}

		private static IEnumerator LoadAchievementDescriptionsCoroutine([NotNull] GameCenterLoadAchievementDescriptionsRequest request)
		{
			bool working = true;
			bool canceled = false;
			LoadAchievementDescriptionsCompletionHandler onComplete = (AchievementDescription[] result, string error) =>
			{
				if (!canceled)
				{
					request.result = result;
					request.error = error;
				}
				working = false;
			};
			request.isDone = false;
			LoadAchievementDescriptions(onComplete);
			float startTime = Time.realtimeSinceStartup;
			while (working)
			{
				if (request.timeout > 0f && Time.realtimeSinceStartup - startTime > request.timeout)
				{
					canceled = true;
					request.didTimeout = true;
					break;
				}
				yield return null;
			}
			request.isDone = true;
		}

		public static void ResetAchievements([CanBeNull] RequestCompleteHandler completionHandler)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (_instance != null)
				{
					int num = Depository.CheckIn(completionHandler, _instance);
				}
				else
				{
					LogError("ResetAchievements was called before Init");
				}
			}
			else if (completionHandler != null)
			{
				completionHandler("Could not reset achievements (not on iOS)");
			}
		}

		public static Coroutine ResetAchievements([NotNull] GameCenterRequest request)
		{
			return CoroutineUtil.StartCoroutine(RequestCoroutine(request, ResetAchievements));
		}

		private static IEnumerator RequestCoroutine([NotNull] GameCenterRequest request, [NotNull] Action<RequestCompleteHandler> requestFunction)
		{
			bool working = true;
			bool canceled = false;
			RequestCompleteHandler onComplete = (string error) =>
			{
				if (!canceled)
				{
					request.error = error;
				}
				working = false;
			};
			request.isDone = false;
			requestFunction(onComplete);
			float startTime = Time.realtimeSinceStartup;
			while (working)
			{
				if (request.timeout > 0f && Time.realtimeSinceStartup - startTime > request.timeout)
				{
					canceled = true;
					request.didTimeout = true;
					break;
				}
				yield return null;
			}
			request.isDone = true;
		}

		[UsedImplicitly]
		private void OnGameCenterAuthenticationEvent(string param)
		{
			_initialAuthenticationComplete = true;
			AuthenticateHandler authenticateHandler = authenticationEvent__BackingField;
			if (authenticateHandler != null)
			{
				string[] array = param.Split('|');
				if (array.Length != 3)
				{
					LogError("OnGameCenterAuthenticationEvent: Unexpected number of parameters :" + array.Length);
				}
				_hasLoginView = array[0] == "true";
				int errorCode = ((array[1].Length > 0) ? int.Parse(array[1]) : 0);
				string error = ((array[2].Length <= 0) ? null : UnescapeResultString(array[2]));
				authenticateHandler(_hasLoginView, errorCode, error);
			}
		}

		[UsedImplicitly]
		private void OnGameCenterGenerateIdentityVerificationSignatureComplete(string json)
		{
			GenerateIdentityVerificationSignatureResult generateIdentityVerificationSignatureResult;
			try
			{
				generateIdentityVerificationSignatureResult = JsonUtility.FromJson<GenerateIdentityVerificationSignatureResult>(json);
			}
			catch (Exception)
			{
				LogError("OnGameCenterGenerateIdentityVerificationSignatureComplete: Could not parse result");
				return;
			}
			GenerateIdentityVerificationSignatureCompletionHandler generateIdentityVerificationSignatureCompletionHandler = Depository.CheckOut(generateIdentityVerificationSignatureResult.context) as GenerateIdentityVerificationSignatureCompletionHandler;
			if (generateIdentityVerificationSignatureCompletionHandler != null)
			{
				byte[] signature = ((generateIdentityVerificationSignatureResult.signature == null) ? null : Convert.FromBase64String(generateIdentityVerificationSignatureResult.signature));
				byte[] salt = ((generateIdentityVerificationSignatureResult.salt == null) ? null : Convert.FromBase64String(generateIdentityVerificationSignatureResult.salt));
				generateIdentityVerificationSignatureCompletionHandler(generateIdentityVerificationSignatureResult.publicKeyUrl, signature, salt, generateIdentityVerificationSignatureResult.timestamp, generateIdentityVerificationSignatureResult.error);
			}
			else
			{
				LogError("OnGameCenterGenerateIdentityVerificationSignatureComplete: Handler not registered");
			}
		}

		[UsedImplicitly]
		private void OnGameCenterLoadFriendsComplete(string param)
		{
			string[] array = param.Split('|');
			int num = array.Length;
			if (num < 2)
			{
				LogError("OnGameCenterLoadFriendsComplete: Unexpected number of parameters :" + array.Length);
				return;
			}
			int ticket = int.Parse(array[0]);
			LoadFriendsCompletionHandler loadFriendsCompletionHandler = Depository.CheckOut(ticket) as LoadFriendsCompletionHandler;
			if (loadFriendsCompletionHandler != null)
			{
				string error = ((array[1].Length <= 0) ? null : UnescapeResultString(array[1]));
				string[] array2 = null;
				if (num > 2)
				{
					array2 = new string[num - 2];
					int num2 = 2;
					int num3 = 0;
					while (num2 < num)
					{
						array2[num3] = array[num2];
						num2++;
						num3++;
					}
				}
				loadFriendsCompletionHandler(array2, error);
			}
			else
			{
				LogError("OnGameCenterLoadFriendsComplete: Handler not registered");
			}
		}

		[UsedImplicitly]
		private void OnGameCenterLoadPlayersComplete(string param)
		{
			string[] array = param.Split('|');
			int num = array.Length;
			if (num < 2 || (num - 2) % 4 != 0)
			{
				LogError("OnGameCenterLoadPlayersComplete: Unexpected number of parameters :" + array.Length);
				return;
			}
			int ticket = int.Parse(array[0]);
			LoadPlayersCompletionHandler loadPlayersCompletionHandler = Depository.CheckOut(ticket) as LoadPlayersCompletionHandler;
			if (loadPlayersCompletionHandler != null)
			{
				string error = ((array[1].Length <= 0) ? null : UnescapeResultString(array[1]));
				Player[] array2 = null;
				if (num > 2)
				{
					int num2 = (num - 2) / 4;
					array2 = new Player[num2];
					int num3 = 2;
					for (int i = 0; i < num2; i++)
					{
						array2[i].playerID = array[num3];
						array2[i].alias = UnescapeResultString(array[num3 + 1]);
						array2[i].displayName = UnescapeResultString(array[num3 + 2]);
						array2[i].isFriend = array[num3 + 3] == "true";
						if (array2[i].displayName.Length == 0)
						{
							array2[i].displayName = array2[i].alias;
						}
						num3 += 4;
					}
				}
				loadPlayersCompletionHandler(array2, error);
			}
			else
			{
				LogError("OnGameCenterLoadPlayersComplete: Handler not registered");
			}
		}

		[UsedImplicitly]
		private void OnGameCenterLoadPhotoComplete(string param)
		{
			string[] array = param.Split('|');
			int num = array.Length;
			if (num != 4)
			{
				LogError("OnGameCenterLoadPhotoComplete: Unexpected number of parameters :" + array.Length);
				return;
			}
			int num2 = int.Parse(array[0]);
			InternalLoadPhotoCompletionHandler internalLoadPhotoCompletionHandler = Depository.CheckOut(num2) as InternalLoadPhotoCompletionHandler;
			if (internalLoadPhotoCompletionHandler != null)
			{
				string error = ((array[1].Length <= 0) ? null : UnescapeResultString(array[1]));
				int imgWidth = int.Parse(array[2]);
				int imgHeight = int.Parse(array[3]);
				internalLoadPhotoCompletionHandler(imgWidth, imgHeight, error);
			}
			else
			{
				LogError("OnGameCenterLoadPhotoComplete: Handler not registered: " + num2);
			}
		}

		[UsedImplicitly]
		private void OnGameCenterRequestComplete(string param)
		{
			string[] array = param.Split('|');
			int num = array.Length;
			if (num != 2)
			{
				LogError("OnGameCenterRequestComplete: Unexpected number of parameters :" + array.Length);
				return;
			}
			int num2 = int.Parse(array[0]);
			RequestCompleteHandler requestCompleteHandler = Depository.CheckOut(num2) as RequestCompleteHandler;
			if (requestCompleteHandler != null)
			{
				string error = ((array[1].Length <= 0) ? null : UnescapeResultString(array[1]));
				requestCompleteHandler(error);
			}
			else
			{
				LogError("OnGameCenterRequestComplete: Handler not registered: " + num2);
			}
		}

		[UsedImplicitly]
		private void OnGameCenterLoadLeaderboardComplete(string param)
		{
			string[] array = param.Split('|');
			int num = array.Length;
			if (num != 2 && (num < 11 || (num - 5) % 6 != 0))
			{
				LogError("OnGameCenterLoadLeaderboardComplete: Unexpected number of parameters :" + array.Length);
				return;
			}
			int ticket = int.Parse(array[0]);
			LoadLeaderboardCompletionHandler loadLeaderboardCompletionHandler = Depository.CheckOut(ticket) as LoadLeaderboardCompletionHandler;
			if (loadLeaderboardCompletionHandler != null)
			{
				string error = ((array[1].Length <= 0) ? null : UnescapeResultString(array[1]));
				Leaderboard leaderboard = null;
				if (num > 2)
				{
					Leaderboard leaderboard2 = new Leaderboard();
					leaderboard2.title = array[2];
					leaderboard2.groupIdentifier = ((array[3].Length != 0) ? array[3] : null);
					leaderboard2.maxRange = uint.Parse(array[4]);
					leaderboard = leaderboard2;
					int num2 = (num - 5) / 6;
					int num3 = 5;
					leaderboard.localPlayerScore.playerID = array[num3];
					leaderboard.localPlayerScore.rank = int.Parse(array[num3 + 1]);
					leaderboard.localPlayerScore.value = long.Parse(array[num3 + 2]);
					leaderboard.localPlayerScore.formattedValue = UnescapeResultString(array[num3 + 3]);
					leaderboard.localPlayerScore.date = TimeUtility.FromUnixTime(int.Parse(array[num3 + 4]));
					leaderboard.localPlayerScore.context = ulong.Parse(array[num3 + 5]);
					num3 += 6;
					num2--;
					Score[] array2 = new Score[num2];
					for (int i = 0; i < num2; i++)
					{
						array2[i].playerID = array[num3];
						array2[i].rank = int.Parse(array[num3 + 1]);
						array2[i].value = long.Parse(array[num3 + 2]);
						array2[i].formattedValue = UnescapeResultString(array[num3 + 3]);
						array2[i].date = TimeUtility.FromUnixTime(int.Parse(array[num3 + 4]));
						array2[i].context = ulong.Parse(array[num3 + 5]);
						num3 += 6;
					}
					leaderboard.scores = array2;
				}
				loadLeaderboardCompletionHandler(leaderboard, error);
			}
			else
			{
				LogError("OnGameCenterLoadLeaderboardComplete: Handler not registered");
			}
		}

		[UsedImplicitly]
		private void OnGameCenterLoadAchievementsComplete(string param)
		{
			string[] array = param.Split('|');
			int num = array.Length;
			if (num < 2 || (num - 2) % 3 != 0)
			{
				LogError("OnGameCenterLoadAchievementsComplete: Unexpected number of parameters :" + array.Length);
				return;
			}
			int ticket = int.Parse(array[0]);
			LoadAchievementsCompletionHandler loadAchievementsCompletionHandler = Depository.CheckOut(ticket) as LoadAchievementsCompletionHandler;
			if (loadAchievementsCompletionHandler != null)
			{
				string error = ((array[1].Length <= 0) ? null : UnescapeResultString(array[1]));
				Achievement[] array2 = null;
				if (num > 2)
				{
					int num2 = (num - 2) / 3;
					array2 = new Achievement[num2];
					int num3 = 2;
					for (int i = 0; i < num2; i++)
					{
						array2[i] = new Achievement
						{
							identifier = array[num3],
							percentComplete = float.Parse(array[num3 + 1]),
							lastReportedDate = TimeUtility.FromUnixTime(int.Parse(array[num3 + 2]))
						};
						num3 += 3;
					}
				}
				loadAchievementsCompletionHandler(array2, error);
			}
			else
			{
				LogError("OnGameCenterLoadAchievementsComplete: Handler not registered");
			}
		}

		[UsedImplicitly]
		private void OnGameCenterLoadAchievementDescriptionsComplete(string param)
		{
			string[] array = param.Split('|');
			int num = array.Length;
			if (num < 2 || (num - 2) % 8 != 0)
			{
				LogError("OnGameCenterLoadAchievementDescriptionsComplete: Unexpected number of parameters :" + array.Length);
				return;
			}
			int ticket = int.Parse(array[0]);
			LoadAchievementDescriptionsCompletionHandler loadAchievementDescriptionsCompletionHandler = Depository.CheckOut(ticket) as LoadAchievementDescriptionsCompletionHandler;
			if (loadAchievementDescriptionsCompletionHandler != null)
			{
				string error = ((array[1].Length <= 0) ? null : UnescapeResultString(array[1]));
				AchievementDescription[] array2 = null;
				if (num > 2)
				{
					int num2 = (num - 2) / 8;
					array2 = new AchievementDescription[num2];
					int num3 = 2;
					for (int i = 0; i < num2; i++)
					{
						array2[i] = new AchievementDescription
						{
							identifier = array[num3],
							title = UnescapeResultString(array[num3 + 1]),
							unachievedDescription = UnescapeResultString(array[num3 + 2]),
							achievedDescription = UnescapeResultString(array[num3 + 3]),
							maximumPoints = int.Parse(array[num3 + 4]),
							replayable = (array[num3 + 5] == "true"),
							hidden = (array[num3 + 6] == "true"),
							groupIdentifier = ((array[num3 + 7].Length != 0) ? array[num3 + 7] : null)
						};
						num3 += 8;
					}
				}
				loadAchievementDescriptionsCompletionHandler(array2, error);
			}
			else
			{
				LogError("OnGameCenterLoadAchievementDescriptionsComplete: Handler not registered");
			}
		}

		[UsedImplicitly]
		private void OnGameCenterViewDismissed(string param)
		{
			string[] array = param.Split('|');
			int num = array.Length;
			if (num != 3)
			{
				LogError("OnGameCenterViewDismissed: Unexpected number of parameters :" + array.Length);
				return;
			}
			if (_viewDismissedHandler != null)
			{
				ShowViewState viewState = (ShowViewState)int.Parse(array[0]);
				string leaderboardCategory = ((array[1].Length <= 0) ? null : array[1]);
				LeaderboardTimeScope leaderboardTimeScope = (LeaderboardTimeScope)int.Parse(array[2]);
				_viewDismissedHandler(viewState, leaderboardCategory, leaderboardTimeScope);
			}
			_viewDismissedHandler = null;
		}

		private static string UnescapeResultString(string s)
		{
			if (s != null && s.Contains("&#124;"))
			{
				return s.Replace("&#124;", '|'.ToString(CultureInfo.InvariantCulture));
			}
			return s;
		}

		private static void LogError(string msg)
		{
			Debug.LogError("[GameCenterPlugin] " + msg);
		}

		private static void LogWarning(string msg)
		{
			Debug.LogWarning("[GameCenterPlugin] " + msg);
		}

		[Conditional("GAMECENTER_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[GameCenterPlugin] " + msg);
		}
	}
}
