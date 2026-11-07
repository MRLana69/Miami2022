using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Kiloo.Common.HTTP;
using Kiloo.Common.Plugins.DeviceUtility;
using Kiloo.Common.Utility;
using SYBO.Subway;
using SYBO.Subway.StateSystem;
using SYBO.Subway.Themes;
using UnityEngine;

public class FPSLog
{
	private static FPSLog _instance;

	private bool _logging;

	private int _frameCount;

	private int _fps;

	private double _timeElapsed;

	private double _totalTimeElapsed;

	private double _timeSinceLog;

	private double _timeToLog = 60.0;

	private StringBuilder _fpsLog = new StringBuilder();

	private const int MAX_LOG_SIZE = 1000;

	private bool _loggingFrameTimes;

	private List<float> _frameTimes = new List<float>();

	private StringBuilder _frameTimeLog = new StringBuilder();

	private bool previousColliderState;

	private int originalScreenTimeout;

	private IEnumerator _updateCoroutine;

	private const string GOOGLE_SCRIPT_URL = "https://script.google.com/macros/s/AKfycbz2lBaGXHu05CgN1mXAr217sresz20BwK_3YwuHO2sHx_eJu5g/exec";

	private const string GOOGLE_SHEET_ID = "1d4AZJWVKm1c438UX21RZIMHGpzjRd013Bmk10_n5tmU";

	private string _groupingUsed = "FPS_Data";

	private const string GROUPING_DEFAULT_NAME = "FPS_Data";

	private const string GROUPING_UNCAPPED_NAME = "FPS_Data_Uncapped";

	private const string GROUPING_FRAMETIME_DEFAULT_NAME = "FrameTime_Data";

	private const string GROUPING_FRAMETIME_UNCAPPED_NAME = "FrameTime_Data_Uncapped";

	private const string UPLOAD_SUCCESS_RESPONSE = "Upload Successful";

	private const string CONNECTION_TEST_SUCCESS_RESPONSE = "Connection Established";

	private const string SPREADSHEET_ID_PARAMETER_KEY = "ssid";

	private const string WORKSHEET_NAME_PARAMETER_KEY = "sheet";

	private const string ACTION_PARAMETER_KEY = "action";

	private const string GROUPBY_PARAMETER_KEY = "groupby";

	private const string ACTION_CONNECT_TEST_PARAMETER_VALUE = "ConnectionTest";

	private const string ACTION_UPLOAD_PARAMETER_VALUE = "POST";

	private const string DATA_DEFAULT_PARAMETER_KEY = "val";

	private const string DATA_TARGETFRAMERATE_PARAMETER_KEY = "targetframerate";

	private bool _skipEntry;

	private bool _retryUploadIfFailed;

	public const int DEFAULT_AUTORUN_LENGTH = 600;

	private int _autoRunLength = 600;

	private bool _useCustomFramerate;

	private int _targetFramerate = 60;

	public const int DEFAULT_PERFORMANCE_DEVICE_FRAMERATE = 60;

	public const int LOW_PERFORMANCE_DEVICE_FRAMERATE = 20;

	public const int HIGH_PERFORMANCE_DEVICE_FRAMERATE = 40;

	public const int HIGH_PERFORMANCE_IOS_DEVICE_FRAMERATE = 30;

	public const int DEFAULT_LOGGING_FREQUENCY = 60;

	public bool RunTestologyCappedTest = true;

	public bool RunTestologyUnCappedTest = true;

	private string _lastCappedRunResult = "Not Run";

	private string _lastUncappedRunResult = "Not Run";

	public static FPSLog instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new FPSLog();
			}
			return _instance;
		}
	}

	public int autoRunLength
	{
		get
		{
			return _autoRunLength;
		}
	}

	public int targetFramerate
	{
		get
		{
			return _targetFramerate;
		}
	}

	public bool useCustomFramerate
	{
		get
		{
			return _useCustomFramerate;
		}
	}

	private string SetLastRunResult
	{
		set
		{
			if (_targetFramerate == 60)
			{
				_lastUncappedRunResult = value;
			}
			else
			{
				_lastCappedRunResult = value;
			}
		}
	}

	public bool IsLoggingFPS { get; private set; }

	public bool IsUploadingFPS { get; private set; }

	public void SetFPSUpload(bool value)
	{
		IsUploadingFPS = value;
	}

	public void SetFPSFramerate(int value = 0)
	{
		switch (value)
		{
		case 0:
			_useCustomFramerate = false;
			_targetFramerate = 60;
			break;
		case -1:
			_useCustomFramerate = false;
			switch (DeviceInfo.Instance.performanceLevel)
			{
			case DeviceInfo.PerformanceLevel.Low:
				_targetFramerate = 20;
				break;
			case DeviceInfo.PerformanceLevel.High:
				if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					_targetFramerate = 30;
				}
				else
				{
					_targetFramerate = 40;
				}
				break;
			default:
				_targetFramerate = 60;
				break;
			}
			break;
		default:
			_useCustomFramerate = true;
			_targetFramerate = value;
			break;
		}
		Application.targetFrameRate = _targetFramerate;
	}

	public void SetFPSAutoLength(int value = 0)
	{
		if (value == 0)
		{
			_autoRunLength = 600;
		}
		else
		{
			_autoRunLength = value;
		}
	}

	public void StartAutoFPSTest(bool runPreset = false, bool runFrameTimes = false)
	{
		CoroutineUtil.StartCoroutine(FPSTestRoutine(runPreset, runFrameTimes));
	}

	private IEnumerator FPSTestRoutine(bool runPreset = false, bool runFrameTimes = false)
	{
		if (runPreset)
		{
			_loggingFrameTimes = runFrameTimes;
			yield return AutoFPSTest();
			Game.OnMenuScreenShown += ResetAutoFPS;
		}
		else
		{
			if (RunTestologyCappedTest)
			{
				_loggingFrameTimes = runFrameTimes;
				SetFPSFramerate(-1);
				SetFPSUpload(true);
				_groupingUsed = "FPS_Data";
				yield return AutoFPSTest();
			}
			if (RunTestologyUnCappedTest)
			{
				_loggingFrameTimes = runFrameTimes;
				SetFPSFramerate(60);
				SetFPSUpload(true);
				_groupingUsed = "FPS_Data_Uncapped";
				yield return AutoFPSTest();
			}
			string testResult = "FPS tests have been executed\n\nTest results:\n  Capped Test: " + _lastCappedRunResult + "\n  UncappedTest: " + _lastUncappedRunResult;
			DeviceUtility.ShowNativePopup("FPS Results", testResult, "Ok");
		}
		ResetAutoFPS();
		yield return null;
	}

	private void ResetAutoFPS()
	{
		Physics.IgnoreLayerCollision(Layers.Instance.Character, Layers.Instance.Hit, previousColliderState);
		_groupingUsed = "FPS_Data";
		_lastCappedRunResult = "Not Run";
		_lastUncappedRunResult = "Not Run";
		SetFPSLogging(false);
		SetFPSUpload(false);
		SetFPSFramerate();
		_loggingFrameTimes = false;
		Game.OnMenuScreenShown -= ResetAutoFPS;
	}

	public void SetFPSLogging(bool shouldEnable, int frequency = 60)
	{
		if (shouldEnable)
		{
			originalScreenTimeout = Screen.sleepTimeout;
			Screen.sleepTimeout = -1;
			IsLoggingFPS = true;
			_timeToLog = frequency;
			_updateCoroutine = Update();
			CoroutineUtil.StartCoroutine(_updateCoroutine);
			if (!useCustomFramerate)
			{
				SetFPSFramerate(-1);
			}
		}
		else
		{
			Screen.sleepTimeout = originalScreenTimeout;
			IsLoggingFPS = false;
			CoroutineUtil.StopCoroutine(_updateCoroutine);
			Application.targetFrameRate = 60;
		}
	}

	private IEnumerator AutoFPSTest()
	{
		previousColliderState = Globals.DEBUG_CHARACTER_MOVE_THROUGH_COLLIDERS;
		Physics.IgnoreLayerCollision(Layers.Instance.Character, Layers.Instance.Hit, true);
		SetFPSLogging(true, _autoRunLength);
		SetFPSUpload(true);
		RunStarted();
		Game.Instance.StartNewGame();
		UIScreenController.Instance.PushScreen("IngameUI");
		while (UIScreenController.Instance.isShowingPopup)
		{
			UIScreenController.Instance.ClosePopup();
		}
		yield return CoroutineUtil.StartCoroutine(CoroutineUtil.WaitForRealSeconds(_autoRunLength));
		GameStats.Instance.ResetGame();
		GameStateMachine.Instance.ActivateTrigger(107);
		RunEnded();
		yield return new WaitForSeconds(10f);
		while (UIScreenController.Instance.isShowingPopup)
		{
			UIScreenController.Instance.ClosePopup();
		}
		UIScreenController.Instance.PushScreen("FrontScreen");
		yield return new WaitForSeconds(0.2f);
		UIScreenController.Instance.ClosePopup();
	}

	private void RunStarted()
	{
		_fpsLog = new StringBuilder();
		_frameTimeLog = new StringBuilder();
		_frameCount = 0;
		_fps = 0;
		_timeElapsed = 0.0;
		_totalTimeElapsed = 0.0;
		_timeSinceLog = 0.0;
		_logging = true;
		_skipEntry = true;
	}

	private void RunEnded()
	{
		_logging = false;
		Log();
		if (IsUploadingFPS)
		{
			CoroutineUtil.StartCoroutine(SendFPSAndFrameTime2Google(_fpsLog.ToString().TrimEnd(';'), _frameTimeLog.ToString().TrimEnd(';')));
		}
		SetFPSUpload(false);
		CoroutineUtil.StopCoroutine(_updateCoroutine);
	}

	private IEnumerator SendFPSAndFrameTime2Google(string fps, string frametimes)
	{
		_retryUploadIfFailed = true;
		yield return Send2GoogleSheet(fps);
		if (_loggingFrameTimes)
		{
			if (_groupingUsed == "FPS_Data")
			{
				_groupingUsed = "FrameTime_Data";
			}
			else
			{
				_groupingUsed = "FrameTime_Data_Uncapped";
			}
			_retryUploadIfFailed = true;
			yield return Send2GoogleSheet(frametimes);
		}
		yield return null;
	}

	private void PauseChanged(bool isPaused)
	{
		_logging = !isPaused;
	}

	private IEnumerator Update()
	{
		while (true)
		{
			if (_logging)
			{
				_frameCount++;
				_timeElapsed += Time.unscaledDeltaTime;
				_totalTimeElapsed += Time.unscaledDeltaTime;
				_timeSinceLog += Time.unscaledDeltaTime;
				_frameTimes.Add(Time.unscaledDeltaTime);
				if (_timeElapsed > 1.0)
				{
					_fps = (int)((double)_frameCount / _timeElapsed);
					_frameCount = 0;
					_timeElapsed = 0.0;
					if (_skipEntry)
					{
						_skipEntry = false;
					}
					else
					{
						_fpsLog.Append(_fps);
						_fpsLog.Append(';');
						_frameTimeLog.Append(Mathf.Max(_frameTimes.ToArray()).ToString("0.00"));
						_frameTimeLog.Append(';');
					}
					_frameTimes.Clear();
					if (_timeSinceLog > _timeToLog)
					{
						Log();
					}
				}
			}
			yield return null;
		}
	}

	private void Log()
	{
		_timeSinceLog = 0.0;
		int num = Mathf.CeilToInt(_fpsLog.ToString().Length / 1000);
		for (int i = 0; i <= num; i++)
		{
			int num2 = i * 1000;
			int num3 = (i + 1) * 1000;
			if (num3 < _fpsLog.Length)
			{
			}
		}
	}

	public bool TestGoogleConnection()
	{
		string absoluteUri = WebClient.AddQueryParametersToUri(new Uri("https://script.google.com/macros/s/AKfycbz2lBaGXHu05CgN1mXAr217sresz20BwK_3YwuHO2sHx_eJu5g/exec"), new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("ssid", "1d4AZJWVKm1c438UX21RZIMHGpzjRd013Bmk10_n5tmU"),
			new KeyValuePair<string, string>("sheet", _groupingUsed),
			new KeyValuePair<string, string>("action", "ConnectionTest")
		}).AbsoluteUri;
		KeyValuePair<bool, string> keyValuePair = SendRequest(absoluteUri, 5f);
		if (keyValuePair.Key && keyValuePair.Value == "Connection Established")
		{
			return true;
		}
		return false;
	}

	public KeyValuePair<bool, string> SendRequest(string url, float timeout)
	{
		WWW wWW = new WWW(url);
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		bool flag = false;
		while (!wWW.isDone && !flag)
		{
			if ((float)stopwatch.ElapsedMilliseconds > timeout * 1000f)
			{
				flag = true;
			}
		}
		stopwatch.Stop();
		if (wWW.isDone)
		{
		}
		return new KeyValuePair<bool, string>(wWW.isDone, wWW.text);
	}

	private IEnumerator Send2GoogleSheet(string dataPoints)
	{
		if (dataPoints.Length <= 60)
		{
			yield break;
		}
		float[] values = dataPoints.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(float.Parse)
			.ToArray();
		string url = WebClient.AddQueryParametersToUri(new Uri("https://script.google.com/macros/s/AKfycbz2lBaGXHu05CgN1mXAr217sresz20BwK_3YwuHO2sHx_eJu5g/exec"), new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("ssid", "1d4AZJWVKm1c438UX21RZIMHGpzjRd013Bmk10_n5tmU"),
			new KeyValuePair<string, string>("sheet", ThemeManager.Instance.Theme.name),
			new KeyValuePair<string, string>("action", "POST"),
			new KeyValuePair<string, string>("groupby", _groupingUsed),
			new KeyValuePair<string, string>("targetframerate", Application.targetFrameRate.ToString()),
			new KeyValuePair<string, string>("val", DateTime.Now.ToString(CultureInfo.InvariantCulture)),
			new KeyValuePair<string, string>("val", SystemInfo.deviceModel),
			new KeyValuePair<string, string>("val", SystemInfo.deviceName),
			new KeyValuePair<string, string>("val", DeviceInfo.Instance.performanceLevel.ToString()),
			new KeyValuePair<string, string>("val", Application.targetFrameRate.ToString()),
			new KeyValuePair<string, string>("val", SystemInfo.operatingSystem),
			new KeyValuePair<string, string>("val", string.Format("{0:c}", TimeSpan.FromSeconds(_totalTimeElapsed))),
			new KeyValuePair<string, string>("val", DeviceUtility.GetBundleVersion()),
			new KeyValuePair<string, string>("val", dataPoints),
			new KeyValuePair<string, string>("val", Mathf.Min(values).ToString()),
			new KeyValuePair<string, string>("val", (values.ToList().Sum() / (float)values.Length).ToString("0.00")),
			new KeyValuePair<string, string>("val", Mathf.Max(values).ToString())
		}).AbsoluteUri;
		KeyValuePair<bool, string> requestResult = SendRequest(url, 5f);
		if (requestResult.Key && requestResult.Value == "Upload Successful")
		{
			UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get("UPLOAD_SUCCESS_MESSAGE"));
			SetLastRunResult = "Success";
			yield break;
		}
		UIScreenController.Instance.QueueErrorMessageSlidein(Strings.Get("UPLOAD_FAILED_MESSAGE"));
		if (_retryUploadIfFailed)
		{
			SetLastRunResult = "Failed";
			_retryUploadIfFailed = false;
			yield return Send2GoogleSheet(dataPoints);
		}
		else
		{
			SetLastRunResult = "Failed";
		}
	}
}
