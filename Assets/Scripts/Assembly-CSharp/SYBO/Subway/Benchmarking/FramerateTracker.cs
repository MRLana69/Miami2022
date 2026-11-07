using System;
using System.Diagnostics;
using SYBO.Common.Benchmarking.Utility;
using UnityEngine;

namespace SYBO.Subway.Benchmarking
{
	public class FramerateTracker
	{
		private const float MillisecondsToSeconds = 0.001f;

		private Stopwatch _stopWatch = new Stopwatch();

		private float _framesRendered;

		private GameObject _updater;

		private FirstMonobehaviour _first;

		private LastMonobehaviour _last;

		private float _totalScriptTime;

		private float _totalRenderTime;

		private float _totalPhysicsTime;

		private float _scriptStartTime;

		private float _renderStartTime;

		private float _physicsStartTime;

		private bool _paused;

		private bool _skipFrame;

		private bool _maxFrametimeSamplingEnabled;

		private float _maxFrameTimeSampleInterval;

		private long _lastFrameStartTicks;

		private float[] _samples;

		private int _currentSampleIndex;

		private float _currentSampleInterval;

		private float _currentSampleMaxTime;

		public long ElapsedMilliseconds
		{
			get
			{
				return _stopWatch.ElapsedMilliseconds;
			}
		}

		public bool IsTracking
		{
			get
			{
				return _stopWatch.IsRunning;
			}
		}

		public float MaxFrameTime
		{
			get
			{
				if (!_maxFrametimeSamplingEnabled)
				{
					return 0f;
				}
				return Mathf.Max(Mathf.Max(_samples), _currentSampleMaxTime);
			}
		}

		public FramerateTracker(float maxFrameTimeSampleRange, int maxFrametimeSampleCount)
		{
			_maxFrametimeSamplingEnabled = true;
			_samples = new float[maxFrametimeSampleCount];
			_maxFrameTimeSampleInterval = maxFrameTimeSampleRange / (float)maxFrametimeSampleCount;
		}

		public void StartTracking()
		{
			_updater = new GameObject("Updater");
			_first = _updater.AddComponent<FirstMonobehaviour>();
			_last = _updater.AddComponent<LastMonobehaviour>();
			FirstMonobehaviour first = _first;
			first.OnUpdate = (Action)Delegate.Combine(first.OnUpdate, new Action(FirstMonobehaviour_OnUpdate));
			FirstMonobehaviour first2 = _first;
			first2.OnFixedUpdate = (Action)Delegate.Combine(first2.OnFixedUpdate, new Action(FirstMonobehaviour_OnFixedUpdate));
			LastMonobehaviour last = _last;
			last.OnLateUpdate = (Action)Delegate.Combine(last.OnLateUpdate, new Action(LastMonobehaviour_OnLateUpdate));
			FirstMonobehaviour first3 = _first;
			first3.OnOnGUI = (Action)Delegate.Combine(first3.OnOnGUI, new Action(FirstMonobehaviour_OnOnGUI));
			LastMonobehaviour last2 = _last;
			last2.OnYieldFixedUpdate = (Action)Delegate.Combine(last2.OnYieldFixedUpdate, new Action(LastMonobehaviour_OnYieldFixedUpdate));
			FirstMonobehaviour first4 = _first;
			first4.OnOnApplicationPause = (Action<bool>)Delegate.Combine(first4.OnOnApplicationPause, new Action<bool>(FirstMonobehaviour_OnOnApplicationPause));
			_paused = false;
			_stopWatch.Start();
		}

		public void EndTracking()
		{
			_stopWatch.Stop();
			_stopWatch.Reset();
			ResetFrameTimes();
			UnityEngine.Object.Destroy(_updater);
		}

		public void Reset()
		{
			_stopWatch.Stop();
			_stopWatch.Reset();
			ResetFrameTimes();
			_paused = false;
			_stopWatch.Start();
		}

		private void ResetFrameTimes()
		{
			_framesRendered = 0f;
			_totalScriptTime = 0f;
			_totalRenderTime = 0f;
			_totalPhysicsTime = 0f;
			_scriptStartTime = 0f;
			_renderStartTime = 0f;
			_physicsStartTime = 0f;
		}

		public void TogglePause(bool paused)
		{
			_paused = paused;
			_skipFrame = true;
			_scriptStartTime = 0f;
			_renderStartTime = 0f;
			_physicsStartTime = 0f;
			if (_paused)
			{
				_stopWatch.Stop();
			}
			else
			{
				_stopWatch.Start();
			}
		}

		public float GetFramerate()
		{
			return _framesRendered / ((float)_stopWatch.ElapsedMilliseconds * 0.001f);
		}

		public float GetPotentialFramerate()
		{
			long elapsedMilliseconds = _stopWatch.ElapsedMilliseconds;
			float num = _totalScriptTime + _totalRenderTime + _totalPhysicsTime;
			return _framesRendered / ((float)elapsedMilliseconds * 0.001f) / num * (float)elapsedMilliseconds;
		}

		private void FirstMonobehaviour_OnOnApplicationPause(bool paused)
		{
			TogglePause(paused);
		}

		private void FirstMonobehaviour_OnFixedUpdate()
		{
			if (!_paused)
			{
				_physicsStartTime = _stopWatch.ElapsedMilliseconds;
			}
		}

		private void LastMonobehaviour_OnYieldFixedUpdate()
		{
			if (!_paused && _physicsStartTime > 0f)
			{
				_totalPhysicsTime += (float)_stopWatch.ElapsedMilliseconds - _physicsStartTime;
				_physicsStartTime = 0f;
			}
		}

		private void FirstMonobehaviour_OnOnGUI()
		{
			if (!_paused && _renderStartTime > 0f)
			{
				_totalRenderTime += (float)_stopWatch.ElapsedMilliseconds - _renderStartTime;
				_renderStartTime = 0f;
			}
		}

		private void LastMonobehaviour_OnLateUpdate()
		{
			if (!_paused)
			{
				if (_scriptStartTime > 0f)
				{
					_totalScriptTime += (float)_stopWatch.ElapsedMilliseconds - _scriptStartTime;
					_scriptStartTime = 0f;
				}
				_renderStartTime = _stopWatch.ElapsedMilliseconds;
			}
		}

		private void FirstMonobehaviour_OnUpdate()
		{
			long elapsedMilliseconds = _stopWatch.ElapsedMilliseconds;
			if (_paused)
			{
				_lastFrameStartTicks = elapsedMilliseconds;
				return;
			}
			if (_skipFrame)
			{
				_skipFrame = false;
				_lastFrameStartTicks = elapsedMilliseconds;
				return;
			}
			if (_maxFrametimeSamplingEnabled)
			{
				_scriptStartTime = elapsedMilliseconds;
				float num = (float)(elapsedMilliseconds - _lastFrameStartTicks) * 0.001f;
				_currentSampleMaxTime = Mathf.Max(_currentSampleMaxTime, num);
				_currentSampleInterval += num;
				if (_currentSampleInterval >= _maxFrameTimeSampleInterval)
				{
					_samples[_currentSampleIndex++] = _currentSampleMaxTime;
					_currentSampleMaxTime = 0f;
					if (_currentSampleIndex >= _samples.Length)
					{
						_currentSampleIndex = 0;
					}
					_currentSampleInterval -= _maxFrameTimeSampleInterval;
				}
			}
			_framesRendered++;
			_lastFrameStartTicks = elapsedMilliseconds;
		}
	}
}
