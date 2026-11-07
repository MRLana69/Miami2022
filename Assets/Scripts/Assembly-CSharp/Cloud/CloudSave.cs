using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AppRefresh;
using Authentication;
using Cloud.Internal;
using Data.Managers;
using Data.Save;
using Info;
using Kiloo;
using Kiloo.Common.Backend;
using Kiloo.Common.Backend.Authentication;
using Kiloo.Common.Backend.Cloud;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Serialization;
using Kiloo.Common.Utility;
using Missions;
using SYBO.Subway;
using UI.Screens;
using UI.SlideIns;
using UnityEngine;

namespace Cloud
{
	public static class CloudSave
	{
		private const string DEFAULT_CLOUD_BASE_URL = "https://cloudsave2-subwaysurfers.kiloo-games.com/";

		private const string ONLINESETTINGS_CLOUD_URL_KEY = "cloudsave_url";

		private const int DEFAULT_DATA_VERSION = 0;

		private const bool USE_SIMULATED_SERVER_IN_EDITOR = true;

		private const string CLOUD_SAVE_DISABLED_KEY = "cloudsave_disabled";

		private const string CLOUD_SAVE_APP_RESUME_TIME_INTERVAL_KEY = "cloudsave_app_resume_interval";

		private const int CLOUD_SAVE_APP_RESUME_INTERVAL_DEFAULT = 3600;

		private const bool CLOUD_SAVE_DISABLED_DEFAULT = false;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action stateChanged__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onCloudSaveDisabled__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onCloudSaveUpdateStart__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onCloudSaveRestored__BackingField;

		private static int _dataVersion;

		private static CloudSaveState _state;

		private static CloudSaveErrorState _errorState;

		private static ICloudServer _cloudServer;

		private static CloudWorkingCopy _workingCopy;

		private static CloudSaveConflictResolver _conflictResolver;

		private static KilooSerializer _serializer;

		private static IInstallationInfo _installationInfo;

		private static SafeFileSystem _userFileSystem;

		private static SafeFileSystem _cloudFileSystem;

		private static bool _mayQueueCelebration;

		public static CloudSaveState state
		{
			get
			{
				return _state;
			}
			private set
			{
				if (_state != value)
				{
					_state = value;
					stateChanged__BackingField.SafeInvoke();
				}
			}
		}

		public static CloudSaveErrorState errorState
		{
			get
			{
				return _errorState;
			}
			private set
			{
				_errorState = value;
			}
		}

		public static CloudSaveConflictResolver conflictResolver
		{
			get
			{
				return _conflictResolver;
			}
		}

		private static bool workingCopyCreated
		{
			get
			{
				return _workingCopy != null;
			}
		}

		public static bool useSimulatedServer
		{
			get
			{
				return Application.isEditor ? true : false;
			}
		}

		public static string userName
		{
			get
			{
				return Auth.displayName.IsNullOrEmpty() ? Strings.Get("PLAYER_GENERIC_NAME") : Auth.displayName;
			}
		}

		public static bool isAvailable { get; private set; }

		public static int appResumeTimeInterval
		{
			get
			{
				return OnlineSettings.instance.GetValue("cloudsave_app_resume_interval", 3600);
			}
		}

		public static bool initializedAtStartup { get; private set; }

		public static bool loginRoutineInProgress { get; private set; }

		public static bool appResumeRoutineInProgress { get; private set; }

		public static bool operationInProgress
		{
			get
			{
				return !initializedAtStartup || loginRoutineInProgress || appResumeRoutineInProgress;
			}
		}

		public static event Action stateChanged
		{
			add
			{
				Action action = stateChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref stateChanged__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = stateChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref stateChanged__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onCloudSaveDisabled
		{
			add
			{
				Action action = onCloudSaveDisabled__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onCloudSaveDisabled__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onCloudSaveDisabled__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onCloudSaveDisabled__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onCloudSaveUpdateStart
		{
			add
			{
				Action action = onCloudSaveUpdateStart__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onCloudSaveUpdateStart__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onCloudSaveUpdateStart__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onCloudSaveUpdateStart__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onCloudSaveRestored
		{
			add
			{
				Action action = onCloudSaveRestored__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onCloudSaveRestored__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onCloudSaveRestored__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onCloudSaveRestored__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		static CloudSave()
		{
			loginRoutineInProgress = false;
			appResumeRoutineInProgress = false;
			initializedAtStartup = false;
			isAvailable = !DeviceUtility.isKindleDevice && !OnlineSettings.instance.GetValue("cloudsave_disabled", false);
		}

		public static IEnumerator StartUpRoutine()
		{
			if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WebGLPlayer)
			{
				initializedAtStartup = true;
				yield break;
			}
			yield return Auth.Init(false);
			Init();
			if (!PreferencesSave.cloudSaveOptedOut)
			{
				yield return Peek();
				yield return UpdateAsync();
			}
			if (AppRefreshHandler.pendingAppRefresh)
			{
				yield return AppRefreshHandler.StartRefresh(RefreshType.UI | RefreshType.Data);
			}
			initializedAtStartup = true;
		}

		public static IEnumerator LoginRoutine(Action onUpdateFinished = null, bool skipAuthenticate = false)
		{
			loginRoutineInProgress = true;
			if (!skipAuthenticate)
			{
				yield return Auth.Init(true);
			}
			EnableAndInit();
			yield return UpdateAsync(onUpdateFinished);
			if (AppRefreshHandler.pendingAppRefresh)
			{
				yield return AppRefreshHandler.StartRefresh(RefreshType.UI | RefreshType.Data);
			}
			loginRoutineInProgress = false;
		}

		private static void CreateWorkingCopy()
		{
			Auth.CreateKilooAuth();
			if (isAvailable)
			{
				if (_serializer == null)
				{
					_serializer = new KilooSerializer();
				}
				if (_cloudServer == null)
				{
					string value = OnlineSettings.instance.GetValue("cloudsave_url", "https://cloudsave2-subwaysurfers.kiloo-games.com/");
					_cloudServer = CreateServer(value, Auth.kilooAuth, _serializer);
				}
				if (_userFileSystem == null)
				{
					_userFileSystem = new SafeFileSystem(DeviceUtility.GetUserDataPath());
				}
				if (_installationInfo == null)
				{
					_installationInfo = new InstallationInfo(new BsonFileStorage(_userFileSystem, _serializer), SystemInfo.deviceUniqueIdentifier);
				}
				if (_cloudFileSystem == null)
				{
					_cloudFileSystem = new SafeFileSystem(GetSaveDir());
				}
				_dataVersion = GetDataVersion();
				if (_workingCopy == null)
				{
					_workingCopy = new CloudWorkingCopy(_dataVersion, _cloudServer, _cloudFileSystem, _serializer, _installationInfo.id);
					_workingCopy.itemUpdated += OnItemUpdated;
				}
				if (!Auth.kilooTag.IsNullOrEmpty())
				{
					_workingCopy.SetUserData(GetCloudFilePath(), MakeUserData());
				}
			}
		}

		public static void OnSaveGameCorrupted(DataCorruptionType corruptionType)
		{
			CreateWorkingCopy();
			if (corruptionType == DataCorruptionType.CloudDataCorruption && _workingCopy.IsFileAdded(GetCloudFilePath()))
			{
				_workingCopy.ResetFile(GetCloudFilePath());
			}
		}

		private static void Init()
		{
			if (isAvailable)
			{
				if (PreferencesSave.cloudSaveOptedOut)
				{
					UserSave.cloudSaveEverEnabled = true;
					state = CloudSaveState.OptedOut;
				}
				else if (!Auth.isInitialized)
				{
					state = CloudSaveState.Error;
					errorState = CloudSaveErrorState.Unauthenticated;
				}
				else if (UserSave.cloudSaveEnabled || !UserSave.hasCloudSaveSuccessfullyPeeked)
				{
					CreateWorkingCopy();
					RefreshState(true);
				}
			}
		}

		private static int GetDataVersion()
		{
			string[] array = DeviceUtility.GetBundleVersion().Split('.');
			if (array.Length != 3)
			{
				LogError("Bundle version was split into {0}. This should NOT happen, expected 3.".Populate(array.Length));
				return 0;
			}
			int[] array2 = Array.ConvertAll(array, int.Parse);
			int[] array3 = array2;
			foreach (int num in array3)
			{
				if (num >= 1000)
				{
					LogError("Individual components of version code is higher larger than maximum allowed.");
					return 0;
				}
			}
			int num2 = 0;
			num2 += array2[0] * 1000000;
			num2 += array2[1] * 1000;
			return num2 + array2[2];
		}

		private static ICloudServer CreateServer(string url, IKilooAuth kilooAuth, IBsonSerializer serializer)
		{
			if (useSimulatedServer)
			{
				return SimulatedCloudServer.instance;
			}
			return new CloudServer2(new Uri(url), kilooAuth, serializer);
		}

		public static void Commit()
		{
			DateTime utcNow = CDateTime.UtcNow;
			if (!PrepareCommit())
			{
				return;
			}
			Flurry.LogEventWithAParameter("CloudSave Commit", "Started", string.Empty);
			bool flag = true;
			try
			{
				_workingCopy.Commit();
			}
			catch (Exception ex)
			{
				flag = false;
				Flurry.LogEventWithAParameter("CloudSave Commit error", "error_message", ex.Message);
			}
			if (flag)
			{
				if (UserSave.cloudSaveEnabled)
				{
					SetStateActiveAndQueueCelebrationIfNeeded();
				}
			}
			else
			{
				OnProccessUnsuccessful();
			}
			Flurry.LogEventWithAParameter("CloudSave Commit", (!flag) ? "FailTime" : "SuccessTime", PutInSecondBucket(CDateTime.UtcNow - utcNow));
		}

		private static IEnumerator CommitAsync()
		{
			DateTime startTime = CDateTime.UtcNow;
			if (_workingCopy.operationInProcess)
			{
				yield break;
			}
			while (AppRefreshHandler.refreshInProgress)
			{
				yield return null;
			}
			if (!PrepareCommit())
			{
				yield break;
			}
			Flurry.LogEventWithAParameter("CloudSave Commit", "AsyncStarted", string.Empty);
			yield return _workingCopy.CommitAsync((bool success, string errorMessage) =>
			{
				if (success)
				{
					if (UserSave.cloudSaveEnabled)
					{
						SetStateActiveAndQueueCelebrationIfNeeded();
					}
					UserSave.latestSuccessfulCloudSaveSyncTime = CDateTime.UtcNow;
				}
				else
				{
					OnProccessUnsuccessful();
					Flurry.LogEventWithAParameter("CloudSave Commit Async error", "error_message", errorMessage);
				}
				Flurry.LogEventWithAParameter("CloudSave Commit", (!success) ? "AsyncFailTime" : "AsyncSuccessTime", PutInSecondBucket(CDateTime.UtcNow - startTime));
			});
		}

		private static bool PrepareCommit()
		{
			if (!workingCopyCreated)
			{
				return false;
			}
			if (_workingCopy.operationInProcess)
			{
				return false;
			}
			switch (state)
			{
			case CloudSaveState.Inactive:
				return false;
			case CloudSaveState.Conflict:
				return false;
			case CloudSaveState.Error:
				return false;
			default:
				if (Auth.kilooTag.IsNullOrEmpty())
				{
					return false;
				}
				_workingCopy.SetUserDataIfChanged(GetCloudFilePath(), MakeUserData());
				_workingCopy.ScanForLocalModifications();
				return _workingCopy.isModified;
			}
		}

		private static Coroutine Peek()
		{
			return CoroutineUtil.StartCoroutine(Peek_Routine());
		}

		public static IEnumerator Peek_Routine()
		{
			if ((Globals.DEBUG && !Globals.DEBUG_ONLINE_SAVE_PEEK_ENABLED) || !workingCopyCreated)
			{
				yield break;
			}
			if (state == CloudSaveState.Active)
			{
				UserSave.hasCloudSaveSuccessfullyPeeked = true;
			}
			else
			{
				if (UserSave.hasCloudSaveSuccessfullyPeeked)
				{
					yield break;
				}
				yield return _workingCopy.UpdateMetadataAsync((bool success, string errorMessage) =>
				{
					if (success)
					{
						UserSave.hasCloudSaveSuccessfullyPeeked = true;
						if (_workingCopy.isOutOfDate)
						{
							state = CloudSaveState.Active;
							UserSave.cloudSaveEnabled = true;
							UserSave.cloudSaveEverEnabled = true;
							UserSave.hasCloudSaveCelebrationBeenShown = true;
							UserSave.cloudSaveEnabledByPeekRoutine = true;
						}
					}
					else
					{
						Flurry.LogEventWithAParameter("CloudSave UpdateMetadata error", "error_message", errorMessage);
					}
				});
			}
		}

		public static void StartAppResume()
		{
			if (UserSave.cloudSaveEnabled && initializedAtStartup && !(UIScreenController.Instance == null) && !UIScreenController.Instance.IsAnyOverlayActive() && !appResumeRoutineInProgress && appResumeTimeInterval >= 0)
			{
				CoroutineUtil.StartCoroutine(AppResumeRoutine());
			}
		}

		private static IEnumerator AppResumeRoutine()
		{
			appResumeRoutineInProgress = true;
			while (!CanStartAppResumeRoutine())
			{
				if (UIScreenController.Instance.IsAnyOverlayActive())
				{
					appResumeRoutineInProgress = false;
					yield break;
				}
				if (!UserSave.cloudSaveEnabled)
				{
					appResumeRoutineInProgress = false;
					yield break;
				}
				yield return null;
			}
			UIScreenController.Instance.ShowCloudSaveOverlay(Strings.Get("CLOUDSAVE_OVERLAY_TEXT"));
			if (state == CloudSaveState.Conflict)
			{
				_workingCopy.Abort();
			}
			yield return Auth.Init(false);
			Init();
			if (!PreferencesSave.cloudSaveOptedOut)
			{
				yield return UpdateAsync();
			}
			if (AppRefreshHandler.pendingAppRefresh)
			{
				yield return AppRefreshHandler.StartRefresh(RefreshType.UI | RefreshType.Data);
			}
			UIScreenController.Instance.HideCloudSaveOverlay();
			appResumeRoutineInProgress = false;
			UIScreen frontScreenInstance = UIScreenController.Instance.GetScreenFromCache("FrontScreen");
			if (frontScreenInstance != null && frontScreenInstance is FrontScreen)
			{
				(frontScreenInstance as FrontScreen).HandleConflictsAfterAppResume();
			}
		}

		private static bool CanStartAppResumeRoutine()
		{
			if (UIScreenController.Instance == null)
			{
				return false;
			}
			if (!UserSave.cloudSaveEnabled)
			{
				return false;
			}
			bool flag = UIScreenController.Instance.GetTopScreenName().IsOneOf("FrontScreen", "GameOverScreen");
			flag &= !UIScreenController.Instance.isShowingPopup;
			bool flag2 = Game.IsInstantiated && Game.Instance.IsInGame.Value;
			bool flag3 = UIScreenController.Instance.IsAnyOverlayActive();
			bool flag4 = (CDateTime.UtcNow - UserSave.latestSuccessfulCloudSaveSyncTime).TotalSeconds > (double)appResumeTimeInterval;
			return !flag2 && !flag3 && flag && flag4;
		}

		public static Coroutine UpdateAsync(Action onFinished = null)
		{
			onCloudSaveUpdateStart__BackingField.SafeInvoke();
			return CoroutineUtil.StartCoroutine(Update_Routine(onFinished));
		}

		private static IEnumerator Update_Routine(Action onFinished = null)
		{
			DateTime startTime = CDateTime.UtcNow;
			if (!workingCopyCreated || state == CloudSaveState.Inactive || state == CloudSaveState.OptedOut || _workingCopy.operationInProcess)
			{
				yield break;
			}
			SaveManager.Save();
			_conflictResolver = null;
			bool wait = true;
			bool hadConflict = false;
			_workingCopy.UpdateAsync((ICloudConflict[] conflicts) =>
			{
				ICloudConflict conflict = conflicts[0];
				if (conflicts.Length > 1)
				{
					LogError("Multiple conflicts. This should NOT happen.");
					for (int i = 0; i < conflicts.Length; i++)
					{
						ICloudConflict cloudConflict = conflicts[i];
						if (cloudConflict.name.Equals("cloud"))
						{
							cloudConflict.ResolveUsingMine();
						}
						else
						{
							conflict = cloudConflict;
						}
						LogError("Conflict {0}: {1}".Populate(i, cloudConflict.name));
					}
				}
				_conflictResolver = new CloudSaveConflictResolver(conflict, IsLocalProfileAllowedToAutoResolve());
				if (_conflictResolver.resolution == CloudConflictResolution.NotResolved)
				{
					state = CloudSaveState.Conflict;
				}
				wait = false;
				Flurry.LogEventWithAParameter("CloudSave Update", "ConflictTime", PutInSecondBucket(CDateTime.UtcNow - startTime));
				hadConflict = true;
				if (state == CloudSaveState.Conflict)
				{
					CoroutineUtil.StartCoroutine(StartRefreshIfRequired(onFinished));
				}
			}, (bool updateSuccess, string errorMessage) =>
			{
				_conflictResolver = null;
				wait = false;
				if (updateSuccess)
				{
					SetStateActiveAndQueueCelebrationIfNeeded();
					onCloudSaveRestored__BackingField.SafeInvoke();
				}
				else
				{
					OnProccessUnsuccessful();
					Flurry.LogEventWithAParameter("CloudSave Update error", "error_message", errorMessage);
				}
				if (!hadConflict)
				{
					Flurry.LogEventWithAParameter("CloudSave Update", (!updateSuccess) ? "FailTime" : "SuccessTime", PutInSecondBucket(CDateTime.UtcNow - startTime));
					hadConflict = false;
				}
				CoroutineUtil.StartCoroutine(StartRefreshIfRequired(() =>
				{
					if (updateSuccess)
					{
						CoroutineUtil.StartCoroutine(CommitAsync());
					}
					onFinished.SafeInvoke();
				}));
			});
			while (wait)
			{
				yield return null;
			}
		}

		private static IEnumerator StartRefreshIfRequired(Action onFinished = null)
		{
			if (AppRefreshHandler.pendingAppRefresh)
			{
				yield return AppRefreshHandler.StartRefresh(RefreshType.UI | RefreshType.Data);
			}
			onFinished.SafeInvoke();
		}

		private static bool IsLocalProfileAllowedToAutoResolve()
		{
			if (UserSave.cloudSaveEnabledByPeekRoutine)
			{
				ICloudUserData cloudUserData = MakeUserData();
				ICloudUserData cloudUserData2 = MakeNewUserData();
				foreach (string key in cloudUserData.ToDictionary().Keys)
				{
					if ((Application.platform != RuntimePlatform.IPhonePlayer || !(key == "highscore")) && cloudUserData[key] != cloudUserData2[key])
					{
						return false;
					}
				}
			}
			return true;
		}

		private static string PutInSecondBucket(TimeSpan span)
		{
			if (span.TotalSeconds >= 10.0)
			{
				return ">10";
			}
			int num = (int)span.TotalSeconds;
			return "{0}-{1}".Populate(num, num + 1);
		}

		private static void OnProccessUnsuccessful()
		{
			if (_workingCopy != null)
			{
				if (Application.internetReachability == NetworkReachability.NotReachable)
				{
					state = CloudSaveState.Error;
					errorState = CloudSaveErrorState.Offline;
				}
				else if (!Auth.isAuthorized && !useSimulatedServer)
				{
					state = CloudSaveState.Error;
					errorState = CloudSaveErrorState.Unauthenticated;
				}
				else if (_workingCopy.isIncompatible)
				{
					state = CloudSaveState.Error;
					errorState = CloudSaveErrorState.GameOutdated;
				}
				else if (_workingCopy.isAccountMismatch)
				{
					HandleAccountMismatch();
				}
				else if (!_workingCopy.operationInProcess && !_workingCopy.isOutOfDate)
				{
					state = CloudSaveState.Error;
					errorState = CloudSaveErrorState.Offline;
				}
			}
		}

		private static void EnableAndInit()
		{
			UserSave.cloudSaveEnabled = true;
			UserSave.cloudSaveEverEnabled = true;
			UserSave.hasCloudSaveSuccessfullyPeeked = true;
			_mayQueueCelebration = true;
			Init();
		}

		public static void Disable(bool optedOut = false)
		{
			UserSave.cloudSaveEnabled = false;
			if (optedOut)
			{
				PreferencesSave.cloudSaveOptedOut = true;
				UIScreenController.Instance.QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.CloudSave,
					cloudSaveState = CloudSaveState.OptedOut
				});
				Auth.Reset();
				Auth.SignOut();
			}
			if (workingCopyCreated)
			{
				_workingCopy.Abort();
				_workingCopy.itemUpdated -= OnItemUpdated;
				_workingCopy = null;
				_cloudServer = null;
			}
			RefreshState();
			onCloudSaveDisabled__BackingField.SafeInvoke();
		}

		private static void RefreshState(bool calledFromInit = false)
		{
			if (PreferencesSave.cloudSaveOptedOut)
			{
				state = CloudSaveState.OptedOut;
			}
			else if (!UserSave.cloudSaveEnabled || !workingCopyCreated)
			{
				state = CloudSaveState.Inactive;
			}
			else if (!Auth.isAuthorized && !useSimulatedServer)
			{
				state = CloudSaveState.Error;
				errorState = CloudSaveErrorState.Unauthenticated;
			}
			else if (_workingCopy.isIncompatible)
			{
				state = CloudSaveState.Error;
				errorState = CloudSaveErrorState.GameOutdated;
			}
			else if (_workingCopy.isAccountMismatch)
			{
				HandleAccountMismatch();
			}
			else if (calledFromInit)
			{
				if (UserSave.cloudSaveEnabled)
				{
					state = CloudSaveState.Active;
					UserSave.cloudSaveEverEnabled = true;
				}
			}
			else
			{
				SetStateActiveAndQueueCelebrationIfNeeded();
			}
		}

		private static string GetSaveDir()
		{
			return SaveManager.GetCloudFolderFullPath();
		}

		private static string GetCloudFilePath()
		{
			return SaveManager.GetCloudFileFullPath(string.Empty);
		}

		private static void OnItemUpdated(object sender, CloudItemUpdatedArgs cloudItemUpdatedArgs)
		{
			AppRefreshHandler.QueueAppRefresh();
		}

		public static ICloudUserData MakeUserData()
		{
			CloudUserData cloudUserData = new CloudUserData();
			cloudUserData.Add("multiplier", ScoreMultiplierInfo.rawMultiplier.ToString());
			cloudUserData.Add("highscore", PlayerSave.highScore.ToString());
			cloudUserData.Add("keys", InventorySave.keys.ToString());
			cloudUserData.Add("upgrades", Upgrades.CountUpgradeTiers().ToString());
			cloudUserData.Add("awards", string.Format("{0:0}", AwardsManager.instance.GetAwardsPercentageComplete()));
			cloudUserData.Add("characters", InventorySave.unlockedCharacters.Count.ToString());
			cloudUserData.Add("board", InventorySave.unlockedBoards.Count.ToString());
			cloudUserData.Add("outfits", InventorySave.GetOutfitCount().ToString());
			cloudUserData.Add("boardThemes", BoardManager.CountBoardThemesUnlocked().ToString());
			return cloudUserData;
		}

		public static ICloudUserData MakeNewUserData()
		{
			CloudUserData cloudUserData = new CloudUserData();
			cloudUserData.Add("multiplier", "1");
			cloudUserData.Add("highscore", "0");
			cloudUserData.Add("keys", 5.ToString());
			cloudUserData.Add("upgrades", "0");
			cloudUserData.Add("awards", "0");
			cloudUserData.Add("characters", "1");
			cloudUserData.Add("board", "0");
			cloudUserData.Add("outfits", "0");
			cloudUserData.Add("boardThemes", "0");
			return cloudUserData;
		}

		public static ICloudUserData DEBUG_MakeUserData()
		{
			return MakeUserData();
		}

		private static void HandleAccountMismatch()
		{
			if (SocialManager.instance.facebookIsLoggedIn)
			{
				SocialManager.instance.FacebookLogout();
			}
			_workingCopy.Clear(true);
			_workingCopy.itemUpdated -= OnItemUpdated;
			_workingCopy = null;
			Init();
		}

		private static void SetUserDataIfChanged(this CloudWorkingCopy workingCopy, string file, ICloudUserData userData)
		{
			ICloudItem item = workingCopy.GetItem(file);
			if (item != null && item.userData != null && userData != null)
			{
				string[] array = item.userData.ToDictionary().Values.ToArray();
				string[] array2 = userData.ToDictionary().Values.ToArray();
				if (array.Length == array2.Length)
				{
					bool flag = true;
					for (int i = 0; i < array.Length; i++)
					{
						flag &= array[i] == array2[i];
					}
					if (flag)
					{
						return;
					}
				}
			}
			workingCopy.SetUserData(file, userData);
		}

		private static void SetStateActiveAndQueueCelebrationIfNeeded()
		{
			if (_mayQueueCelebration && !UserSave.hasCloudSaveCelebrationBeenShown)
			{
				UserSave.hasCloudSaveCelebrationBeenShown = true;
				UIScreenController.Instance.PushPopup("CloudSaveCelebrationPopup");
				_mayQueueCelebration = false;
			}
			state = CloudSaveState.Active;
		}

		[Conditional("CLOUDSAVE_LOGS")]
		private static void Log(string msg, UnityEngine.Object obj = null)
		{
		}

		private static void LogError(string msg, UnityEngine.Object obj = null)
		{
			SDebug.LogError("[CloudSave] " + msg, obj);
		}
	}
}
