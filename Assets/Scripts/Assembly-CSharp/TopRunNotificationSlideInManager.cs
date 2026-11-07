using System;
using JetBrains.Annotations;
using Kiloo;
using SYBO.Subway;
using TopRun;
using UI.SlideIns;
using UnityEngine;

public class TopRunNotificationSlideInManager : MonoBehaviour
{
	private int[] _warningTimesInMinutes = new int[2] { 10, 1 };

	private bool[] _areWarningTimesGameOnly = new bool[2] { false, true };

	private bool _isCurrentWarningTimerGameOnly;

	private const string GAME_OBJECT_NAME = "TopRunNotificationManager";

	private static TopRunNotificationSlideInManager _instance;

	private float timeUntilSlideInInSeconds;

	private int timeToShowOnLabel;

	private float timeLeftForCurrentWarning = 10000000f;

	private bool _doUseTimer;

	private DateTime _originalTourEndDate = DateTime.MinValue;

	private static bool _applicationIsQuitting;

	public static TopRunNotificationSlideInManager instance
	{
		get
		{
			Init();
			return _instance;
		}
	}

	public static void Init()
	{
		if (_instance == null && !_applicationIsQuitting)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "TopRunNotificationManager";
			GameObject gameObject2 = gameObject;
			UnityEngine.Object.DontDestroyOnLoad(gameObject2);
			_instance = gameObject2.AddComponent<TopRunNotificationSlideInManager>();
			_instance.CalculateNextWarningTime();
		}
	}

	private void Update()
	{
		if (!_doUseTimer)
		{
			return;
		}
		timeLeftForCurrentWarning -= RealTimeTracker.deltaTime;
		if (!(timeLeftForCurrentWarning < 0f))
		{
			return;
		}
		if (_isCurrentWarningTimerGameOnly)
		{
			if (Game.Instance != null && Game.Instance.IsInGame.Value && UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.TopRunTimer,
					value = timeToShowOnLabel
				});
			}
		}
		else if (UIScreenController.IsInstanced)
		{
			UIScreenController.Instance.QueueSlideIn(new SlideIn
			{
				type = SlideInData.SlideInType.TopRunTimer,
				value = timeToShowOnLabel
			});
		}
		_doUseTimer = false;
		CalculateNextWarningTime();
	}

	private void InitiateWarningTimes()
	{
		IManager manager = TopRunRoot.manager;
		if (manager.tournament != null && _originalTourEndDate != manager.tournament.officialEndTimeUTC)
		{
			_originalTourEndDate = manager.tournament.officialEndTimeUTC;
			CalculateNextWarningTime();
		}
	}

	private void CalculateNextWarningTime()
	{
		_doUseTimer = false;
		IManager manager = TopRunRoot.manager;
		if (manager.tournament == null)
		{
			return;
		}
		for (int i = 0; i < _warningTimesInMinutes.Length; i++)
		{
			if (manager.tournament.officialEndTimeUTC > CDateTime.UtcNow.AddMinutes(_warningTimesInMinutes[i]))
			{
				_isCurrentWarningTimerGameOnly = _areWarningTimesGameOnly[i];
				timeUntilSlideInInSeconds = (float)(manager.tournament.officialEndTimeUTC.Subtract(new TimeSpan(0, 0, _warningTimesInMinutes[i], 0)) - CDateTime.UtcNow).TotalSeconds;
				timeToShowOnLabel = _warningTimesInMinutes[i];
				timeLeftForCurrentWarning = timeUntilSlideInInSeconds;
				_doUseTimer = true;
				break;
			}
		}
	}

	private void OnEnable()
	{
		TopRunEventsHandler.instance.OnTopRunChangedEvent += TournamentChanged;
	}

	private void OnDisable()
	{
		TopRunEventsHandler.instance.OnTopRunChangedEvent -= TournamentChanged;
	}

	private void TournamentChanged(object sender, EventArgs eventArg)
	{
		InitiateWarningTimes();
	}

	[UsedImplicitly]
	private void OnApplicationQuit()
	{
		_applicationIsQuitting = true;
		_instance = null;
	}
}
