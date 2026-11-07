using SYBO.Subway;
using UnityEngine;

public class InRunAverageFPSCounter : MonoBehaviour
{
	private float _runStartedTime;

	private float _runEndedTime;

	private float _runPausedTime;

	private float _runResumedTime;

	private float _appMinimizedTime;

	private float _appResumedTime;

	private float _amountOfTimePaused;

	private int _frameCount;

	private bool _isInitialized;

	private bool _isInRun;

	private bool _isPaused;

	private bool _isSubscribedToOnPause;

	private const int MAX_NUMBER_OF_INTERVALS = 10;

	private string _gpuName = string.Empty;

	public static int runAverageFPS { get; private set; }

	public static int runLength { get; private set; }

	public void Initialize()
	{
		if (!_isInitialized)
		{
			Game.OnGameStarted += RunStarted;
			Game.OnMenuScreenShown += RunEnded;
			Game.OnGameAwake += SubscribeToGameInstanceEvents;
			_gpuName = SystemInfo.graphicsDeviceName;
			_isInitialized = true;
		}
	}

	private void SubscribeToGameInstanceEvents()
	{
		if (!_isSubscribedToOnPause)
		{
			Game.Instance.OnPauseChange += PauseChange;
			_isSubscribedToOnPause = true;
		}
	}

	private void Update()
	{
		if (_isInRun && !_isPaused)
		{
			_frameCount++;
		}
	}

	private void RunStarted()
	{
		_isInRun = true;
		_amountOfTimePaused = 0f;
		_frameCount = 0;
		runAverageFPS = 0;
		runLength = 0;
		SubscribeToGameInstanceEvents();
		_runStartedTime = Time.realtimeSinceStartup;
	}

	private void RunEnded()
	{
		_isInRun = false;
		_runEndedTime = Time.realtimeSinceStartup;
		runLength = Mathf.CeilToInt(_runEndedTime - (_runStartedTime + _amountOfTimePaused));
		runAverageFPS = _frameCount / runLength;
		if (runAverageFPS > 0)
		{
			Flurry.LogEventWithAParameter("InRunAvgFPS", string.Format("FPS range {0}", RoundUpToTargetValue(runAverageFPS, 6)), string.Format("GPU name {0}", _gpuName));
		}
	}

	private string RoundUpToTargetValue(int total, int factor)
	{
		string result = string.Empty;
		for (int i = 1; i <= 10; i++)
		{
			if (total <= i * factor || (i == 10 && total > i * factor))
			{
				int num = i * factor;
				int num2 = num - factor;
				result = num2 + " - " + num;
				break;
			}
		}
		return result;
	}

	private void PauseChange(bool pause)
	{
		if (_isInRun)
		{
			_isPaused = pause;
			if (pause)
			{
				_runPausedTime = Time.realtimeSinceStartup;
				return;
			}
			_runResumedTime = Time.realtimeSinceStartup;
			_amountOfTimePaused += _runResumedTime - _runPausedTime;
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (_isInRun && !_isPaused)
		{
			_isPaused = paused;
			if (paused)
			{
				_appMinimizedTime = Time.realtimeSinceStartup;
				return;
			}
			_appResumedTime = Time.realtimeSinceStartup;
			_amountOfTimePaused += _appResumedTime - _appMinimizedTime;
		}
	}
}
