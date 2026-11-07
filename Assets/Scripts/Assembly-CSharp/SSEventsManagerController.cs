using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using AppRefresh;
using Data.Managers;
using Data.Save;
using Info;
using Kiloo;
using Missions;
using SYBO.Common.Utilities;
using SYBO.Subway.Info;
using UnityEngine;

public class SSEventsManagerController
{
	public static bool HasLoggedRouteEventWarning;

	public const string ONLINE_SETTING_EVENT_ID = "event_id";

	public const string ONLINE_SETTING_EVENT_START = "event_start";

	public const string ONLINE_SETTING_EVENT_END = "event_end";

	public const string ONLINE_SETTING_EVENT_VALUE = "event_value";

	public const string ONLINE_SETTING_EVENT_TYPE = "event_type";

	public const string ONLINE_SETTING_TITLE_FIRST = "event_popup_title_first";

	public const string ONLINE_SETTING_TITLE_SECOND = "event_popup_title_second";

	public const string ONLINE_SETTING_DESCRIPTION_FIRST = "event_popup_description_first";

	public const string ONLINE_SETTING_DESCRIPTION_SECOND = "event_popup_description_second";

	public const string ONLINE_SETTING_EVENT_CATEGORY = "event_category";

	public const string ONLINE_SETTING_EVENT_RIBBON_TEXT = "event_ribbon_text";

	public const string ONLINE_SETTING_EVENT_MEGA_JACKPOT_MB_VALUE = "event_jackpot_mysterybox_value";

	public const string ONLINE_SETTING_EVENT_MEGA_JACKPOT_SMB_VALUE = "event_jackpot_supermysterybox_value";

	public const string ONLINE_SETTING_EVENT_THEME = "event_theme";

	private IEventManager _currentEventManager;

	private static GameObject _eventTimerGameObject;

	private static MonoBehaviour _eventTimerMono;

	private bool _isInitialized;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnSSEventStartedHandler__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnSSEventFinishedHandler__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action OnSSEventChangedHandler__BackingField;

	private static SSEventsManagerController _instance;

	public static SSEventsManagerController instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new SSEventsManagerController();
			}
			return _instance;
		}
	}

	public static event Action OnSSEventStartedHandler
	{
		add
		{
			Action action = OnSSEventStartedHandler__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnSSEventStartedHandler__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnSSEventStartedHandler__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnSSEventStartedHandler__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action OnSSEventFinishedHandler
	{
		add
		{
			Action action = OnSSEventFinishedHandler__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnSSEventFinishedHandler__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnSSEventFinishedHandler__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnSSEventFinishedHandler__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action OnSSEventChangedHandler
	{
		add
		{
			Action action = OnSSEventChangedHandler__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnSSEventChangedHandler__BackingField, (Action)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = OnSSEventChangedHandler__BackingField;
			Action action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnSSEventChangedHandler__BackingField, (Action)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private SSEventsManagerController()
	{
		if (!_isInitialized)
		{
			Initialize();
		}
	}

	public void Initialize()
	{
		if (!_isInitialized)
		{
			OnlineSettings.instance.AddOnFinishedDownloadingOnlineSettingsHandler(StartEventTimer);
			UIScreenController.OnApplicationResumed += StartEventTimer;
			AgeRestriction.restrictionChanged += StartEventTimer;
			AppRefreshHandler.hasRefreshed += OnHasRefreshed;
			StartEventTimer();
			_isInitialized = true;
		}
	}

	private void OnHasRefreshed(RefreshType refreshType)
	{
		if (refreshType.HasFlag(RefreshType.Data))
		{
			_currentEventManager = null;
			StartEventTimer();
		}
	}

	public IEventManager GetActiveEventManager(SSEventType desiredEventType = SSEventType.None)
	{
		IEventManager result = null;
		if (_currentEventManager != null)
		{
			if (desiredEventType != SSEventType.None)
			{
				if (_currentEventManager.GetEventType() == desiredEventType && _currentEventManager.IsEventActive())
				{
					result = _currentEventManager;
				}
			}
			else if (_currentEventManager.IsEventActive())
			{
				result = _currentEventManager;
			}
		}
		return result;
	}

	public T GetTypeOfActiveEventManager<T>()
	{
		T result = default(T);
		if (_currentEventManager != null && _currentEventManager.IsEventActive())
		{
			try
			{
				result = (T)_currentEventManager;
			}
			catch (Exception ex)
			{
				SDebug.LogError(string.Concat("The type of specified manager could not be found: ", typeof(T), ", stack trace: ", ex.StackTrace));
			}
		}
		return result;
	}

	public void RestoreBoardAfterEventIfNeeded()
	{
		if (!EventsSave.shouldRestoreItemAfterEvent || (TimeLeftInSeconds().CompareTo(0L) > 0 && EventsSave.lastEventID == GetCurrentEventID()))
		{
			return;
		}
		Boards.BoardId currentBoardId = PlayerSave.currentBoardId;
		Boards.BoardId previousBoardId = PlayerSave.previousBoardId;
		if (!string.IsNullOrEmpty(EventsSave.lastEventItemID))
		{
			Boards.BoardId boardId = (Boards.BoardId)Enum.Parse(typeof(Boards.BoardId), EventsSave.lastEventItemID);
			if (currentBoardId == boardId)
			{
				BoardManager.LockBoard(currentBoardId);
				UIModelController.Instance.SelectCurrentlyShownBoard(previousBoardId);
			}
		}
		EventsSave.shouldRestoreItemAfterEvent = false;
	}

	public long TimeLeftInSeconds()
	{
		double unixCurrentTimestamp = GetUnixCurrentTimestamp();
		if (_currentEventManager != null && _currentEventManager.IsEventActive())
		{
			long num = EndTimestampForCurrentEventType();
			if (num > 0)
			{
				return num - (long)unixCurrentTimestamp;
			}
		}
		return 0L;
	}

	public void CurrentEventHasBeenTriedNow()
	{
		if (_currentEventManager != null)
		{
			EventsSave.lastTimeAnEventHasBeenTried = EndTimestampForCurrentEventType();
		}
	}

	public bool HasTheCurrentEventBeenTried()
	{
		long lastTimeAnEventHasBeenTried = EventsSave.lastTimeAnEventHasBeenTried;
		if (_currentEventManager != null)
		{
			return lastTimeAnEventHasBeenTried == EndTimestampForCurrentEventType();
		}
		return false;
	}

	public bool DoesCharacterMatchCurrentEvent(Characters.CharacterId characterId)
	{
		bool result = false;
		IEventManager activeEventManager = GetActiveEventManager();
		if (activeEventManager != null)
		{
			SSEventType eventType = activeEventManager.GetEventType();
			if (eventType == SSEventType.CharacterDay || eventType == SSEventType.ExpiredLimitedCharacter)
			{
				bool found;
				Characters.CharacterId? characterId2 = activeEventManager.GetEventValue<Characters.CharacterId>(out found);
				if (found && characterId2 == characterId)
				{
					result = true;
				}
			}
		}
		return result;
	}

	private void StartEventTimer()
	{
		long num = EndTimestampForCurrentEventType();
		long num2 = StartTimestampForCurrentEventType();
		long num3 = (long)GetUnixCurrentTimestamp();
		long timeLeft = 0L;
		if (_eventTimerGameObject != null)
		{
			UnityEngine.Object.Destroy(_eventTimerGameObject);
		}
		_eventTimerGameObject = new GameObject("EventTimers");
		if (Application.isPlaying)
		{
			UnityEngine.Object.DontDestroyOnLoad(_eventTimerGameObject);
		}
		_eventTimerMono = _eventTimerGameObject.AddComponent<NewMonoBehaviour>();
		HierarchyUtility.ReparentByHierarchyPathType(_eventTimerGameObject.transform, HierarchyPathType.DynamicManagers);
		if (EventsSave.lastTimeAnEventWasStarted.CompareTo(num2) < 0)
		{
			if (num3.CompareTo(num2) < 0)
			{
				timeLeft = num2 - num3;
			}
			_eventTimerMono.StartCoroutine(TimerToEventStart(timeLeft));
		}
		if (num.CompareTo(0L) > 0)
		{
			timeLeft = num - num3;
			if (timeLeft.CompareTo(0L) > 0)
			{
				_eventTimerMono.StartCoroutine(TimerToEventFinish(timeLeft));
			}
		}
		SetCurrentEventManager();
	}

	private IEnumerator TimerToEventStart(long timeLeft)
	{
		float startPoint = 0f;
		while (startPoint < (float)timeLeft)
		{
			startPoint += RealTimeTracker.deltaTime;
			yield return null;
		}
		EventsSave.lastTimeAnEventWasStarted = (long)GetUnixCurrentTimestamp();
		SetCurrentEventManager();
		if (OnSSEventStartedHandler__BackingField != null)
		{
			OnSSEventStartedHandler__BackingField();
		}
	}

	private IEnumerator TimerToEventFinish(long timeLeft)
	{
		float startPoint = 0f;
		while (startPoint < (float)timeLeft)
		{
			startPoint += RealTimeTracker.deltaTime;
			yield return null;
		}
		SetCurrentEventManager();
		if (OnSSEventFinishedHandler__BackingField != null)
		{
			OnSSEventFinishedHandler__BackingField();
		}
	}

	private long GetCurrentEventID()
	{
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			return EventTesterManager.Instance.GetEventID();
		}
		string valueString = string.Empty;
		if (OnlineSettings.instance.TryGetValue("event_id", out valueString))
		{
			long result = 0L;
			if (long.TryParse(valueString, out result))
			{
				return result;
			}
		}
		return 0L;
	}

	private long EndTimestampForCurrentEventType()
	{
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			return EventTesterManager.Instance.GetEventEndTime();
		}
		string valueString = string.Empty;
		if (OnlineSettings.instance.TryGetValue("event_end", out valueString))
		{
			long result = 0L;
			if (long.TryParse(valueString, out result))
			{
				return result;
			}
		}
		return 0L;
	}

	private long StartTimestampForCurrentEventType()
	{
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			return EventTesterManager.Instance.GetEventStartTime();
		}
		string valueString = string.Empty;
		if (OnlineSettings.instance.TryGetValue("event_start", out valueString))
		{
			long result = 0L;
			if (long.TryParse(valueString, out result))
			{
				return result;
			}
		}
		return 0L;
	}

	private double GetUnixCurrentTimestamp()
	{
		return (CDateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
	}

	private bool IsAnyEventEnabled()
	{
		long num = EndTimestampForCurrentEventType();
		if (num > 0)
		{
			long num2 = StartTimestampForCurrentEventType();
			double unixCurrentTimestamp = GetUnixCurrentTimestamp();
			if (num2 > 0)
			{
				return (double)num2 < unixCurrentTimestamp && unixCurrentTimestamp < (double)num;
			}
			return unixCurrentTimestamp < (double)num;
		}
		return false;
	}

	private SSEventType GetEnabledEventType()
	{
		if (IsAnyEventEnabled())
		{
			if (Globals.DEBUG_USE_EVENT_TESTER)
			{
				return EventTesterManager.Instance.GetEventType();
			}
			string valueString = string.Empty;
			if (OnlineSettings.instance.TryGetValue("event_type", out valueString))
			{
				try
				{
					return (SSEventType)Enum.Parse(typeof(SSEventType), valueString, true);
				}
				catch (ArgumentException)
				{
				}
			}
		}
		return SSEventType.None;
	}

	private void SetCurrentEventManager()
	{
		if (_currentEventManager != null)
		{
			_currentEventManager.SaveProgress();
		}
		SSEventType enabledEventType = GetEnabledEventType();
		long currentEventID = GetCurrentEventID();
		if (_currentEventManager == null || _currentEventManager.GetEventID() != currentEventID || enabledEventType != _currentEventManager.GetEventType())
		{
			if (enabledEventType != SSEventType.CharacterName)
			{
				AwardsData.ResetWordyWeekendChallengeOneWeekend();
			}
			if (AgeRestriction.waitForAgeRestriction)
			{
				return;
			}
			switch (enabledEventType)
			{
			case SSEventType.BiggerJackpot:
				_currentEventManager = BiggerJackpotEventManager.Create(enabledEventType, currentEventID);
				break;
			case SSEventType.CharacterDay:
				_currentEventManager = CharacterDayEventManager.Create(enabledEventType, currentEventID);
				break;
			case SSEventType.CharacterName:
				_currentEventManager = CharacterNameEventManager.Create(enabledEventType, currentEventID);
				break;
			case SSEventType.ExpiredLimitedBoard:
				_currentEventManager = ExpiredLimitedBoardEventManager.Create(enabledEventType, currentEventID);
				break;
			case SSEventType.ExpiredLimitedCharacter:
				_currentEventManager = ExpiredLimitedCharacterEventManager.Create(enabledEventType, currentEventID);
				break;
			case SSEventType.MysteryPowerup:
				_currentEventManager = MysteryPowerupEventManager.Create(enabledEventType, currentEventID);
				break;
			case SSEventType.SpecialPowerBoard:
				_currentEventManager = SpecialPowerBoardEventManager.Create(enabledEventType, currentEventID);
				break;
			case SSEventType.SuperMysteryBox:
				_currentEventManager = SuperMysteryBoxEventManager.Create(enabledEventType, currentEventID);
				break;
			default:
				_currentEventManager = null;
				break;
			}
		}
		if (OnSSEventChangedHandler__BackingField != null)
		{
			OnSSEventChangedHandler__BackingField();
		}
	}
}
