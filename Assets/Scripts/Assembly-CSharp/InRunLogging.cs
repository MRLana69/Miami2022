using System.Collections.Generic;
using Kiloo.Analytics;
using SYBO.Subway;

public static class InRunLogging
{
	private static int _headstartUsedInRun;

	private static int _scoreboostersUsedInRun;

	private static bool _isFirstShow = true;

	private static bool _isInitialized;

	public static void Init()
	{
		if (!_isInitialized)
		{
			Game.OnGameStarted += OnGameStarted;
			Game.OnMenuScreenShown += OnMenuScreenShown;
			SlideinPowerupHelper.OnHeadstartActivated += OnHeadstartActivated;
			SlideinPowerupHelper.OnScoreBoostActivated += OnScoreBoostActivated;
			_isInitialized = true;
		}
	}

	private static void OnScoreBoostActivated(int tier)
	{
		_scoreboostersUsedInRun = tier;
	}

	private static void OnHeadstartActivated(int tier)
	{
		_headstartUsedInRun = tier;
	}

	private static void OnGameStarted()
	{
		_headstartUsedInRun = 0;
		_scoreboostersUsedInRun = 0;
	}

	private static void OnMenuScreenShown()
	{
		if (_isFirstShow)
		{
			_isFirstShow = false;
			return;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Headstart", _headstartUsedInRun.ToString());
		dictionary.Add("Scoreboost", _scoreboostersUsedInRun.ToString());
		Dictionary<string, string> parameters = dictionary;
		Flurry.LogEventWithSeveralParameters("Powerups used in run", parameters);
		Manager.LogEvent(EventType.RunEnded);
	}
}
