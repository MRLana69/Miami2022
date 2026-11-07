using System;
using System.Collections.Generic;
using System.Diagnostics;
using AppRefresh;
using Info;
using LoadLevel;
using SYBO.Common.Automation;
using SYBO.Components.Utilities;
using SYBO.Subway;
using UnityEngine;

public class MemoryWarningHandler : MonoBehaviour
{
	private bool _warningOccuredInSession;

	private bool _hasLoggedOneMemoryWarningInSession;

	private int _numberOfRunsSinceStartup;

	private static readonly int[] BUCKETS_TIME_SINCE_STARTUP = new int[10] { 1, 2, 4, 7, 10, 15, 20, 30, 60, 100 };

	private static readonly int[] BUCKETS_UI_AMOUNT = new int[9] { 3, 6, 9, 12, 15, 18, 24, 30, 50 };

	private static readonly int[] BUCKETS_RUNS = new int[10] { 1, 2, 4, 7, 10, 15, 20, 30, 60, 100 };

	private const string MEMORY_CLEANUP_ENABLED_KEY = "memory_cleanup_enabled";

	private const bool MEMORY_CLEANUP_ENABLED_DEFAULT_VALUE = true;

	private bool _memoryCleanUpEnabled;

	private DateTime _lastMemoryCleanUpTime = DateTime.MinValue;

	private const string MEMORY_CLEANUP_COOLDOWN_KEY = "memory_cleanup_cooldown";

	private const int MEMORY_CLEANUP_COOLDOWN_DEFAULT_VALUE = 10;

	private TimeSpan _memoryCleanUpCooldown = new TimeSpan(0, 0, 0, 10);

	private bool _frontScreenHasShownOnce;

	private string _activeScreenDuringCleanup = string.Empty;

	public static MemoryWarningHandler instance;

	public string activeScreenDuringCleanup
	{
		get
		{
			return _activeScreenDuringCleanup;
		}
	}

	private void Start()
	{
		Application.lowMemory += OnLowMemoryWarningReceived;
		Game.OnGameStarted += IncrementRuns;
		AppRefreshHandler.hasRefreshed += OnAppRefresh;
		UIScreenController.OnChangedScreen += OnChangedScreen;
		OnlineSettings.instance.AddOnFinishedDownloadingOnlineSettingsHandler(UpdateOnlineSettingsValues);
	}

	private void Destroy()
	{
		Application.lowMemory -= OnLowMemoryWarningReceived;
		Game.OnGameStarted -= IncrementRuns;
		AppRefreshHandler.hasRefreshed -= OnAppRefresh;
		UIScreenController.OnChangedScreen -= OnChangedScreen;
		OnlineSettings.instance.RemoveOnFinishedDownloadingOnlineSettingsHandler(UpdateOnlineSettingsValues);
	}

	private void OnLowMemoryWarningReceived()
	{
		_warningOccuredInSession = true;
		if (UIScreenController.Instance != null && !_hasLoggedOneMemoryWarningInSession)
		{
			Flurry.LogEventWithSeveralParameters("Low Memory Warning Recieved", new Dictionary<string, string>
			{
				{
					"DeviceType",
					SystemInfo.deviceModel
				},
				{
					"TimeSinceStartup(m)",
					Bucket.BucketThis(Time.realtimeSinceStartup / 60f, BUCKETS_TIME_SINCE_STARTUP)
				},
				{
					"Screen/Popups",
					Bucket.BucketThis(UIScreenController.Instance.GetAmountOfCachedScreens(), BUCKETS_UI_AMOUNT)
				},
				{
					"Runs",
					Bucket.BucketThis(_numberOfRunsSinceStartup, BUCKETS_RUNS)
				}
			});
			_hasLoggedOneMemoryWarningInSession = true;
		}
		if (HasMemoryCleanupCooldownExpired() && IsMemoryCleanupCurrentlyAllowed())
		{
			_lastMemoryCleanUpTime = DateTime.UtcNow;
			_activeScreenDuringCleanup = UIScreenController.Instance.GetTopScreenName();
			AppRefreshHandler.StartRefresh(RefreshType.MemoryWarning);
		}
	}

	private void LogEndOfSessionMemoryWarningStatus()
	{
		if (LoadLevelCtrl.startupFinished)
		{
			Flurry.LogEventWithAParameter("Low Memory Session Result", "WarningOccured", _warningOccuredInSession.ToString());
		}
	}

	private void OnApplicationPause(bool isPaused)
	{
		if (isPaused)
		{
			LogEndOfSessionMemoryWarningStatus();
			_warningOccuredInSession = false;
		}
		else
		{
			_warningOccuredInSession = false;
			_hasLoggedOneMemoryWarningInSession = false;
		}
	}

	private void IncrementRuns()
	{
		_numberOfRunsSinceStartup++;
	}

	private void UpdateOnlineSettingsValues()
	{
		_memoryCleanUpEnabled = OnlineSettings.instance.GetValue("memory_cleanup_enabled", true);
		_memoryCleanUpCooldown = new TimeSpan(0, 0, 0, OnlineSettings.instance.GetValue("memory_cleanup_cooldown", 10));
	}

	private void OnChangedScreen(string screenName)
	{
		if (screenName == "FrontScreen" && !_frontScreenHasShownOnce)
		{
			_frontScreenHasShownOnce = true;
		}
	}

	private void OnAppRefresh(RefreshType refreshType)
	{
		if (refreshType == RefreshType.MemoryWarning)
		{
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}
	}

	private bool HasMemoryCleanupCooldownExpired()
	{
		return DateTime.UtcNow > _lastMemoryCleanUpTime + _memoryCleanUpCooldown;
	}

	private bool IsMemoryCleanupCurrentlyAllowed()
	{
		return _memoryCleanUpEnabled && _frontScreenHasShownOnce && LoadLevelCtrl.startupFinished && Game.IsInstantiated && !Game.Instance.IsInGame.Value && UIScreenController.IsInstanced && !string.IsNullOrEmpty(UIScreenController.Instance.GetTopScreenName()) && UIScreenController.Instance.GetTopScreenName() != "GameOverScreen" && !AgeRestriction.waitForAgeRestriction && (!SingletonMonoBehaviour<AutomationController>.IsInstantiated() || !SingletonMonoBehaviour<AutomationController>.Instance.IsRunning);
	}

	public static void StartTracking()
	{
		if (instance == null)
		{
			GameObject gameObject = new GameObject("MemoryWarningHandler");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			instance = gameObject.AddComponent<MemoryWarningHandler>();
		}
	}

	[Conditional("MEMORY_WARNING_LOGGING")]
	private static void Log(string msg)
	{
	}
}
