using System;
using System.Collections;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kiloo.Common.Utility
{
	public class Wait
	{
		public class Config
		{
			public struct TimeConfig
			{
				private readonly Config _config;

				public TimeConfig(Config wait)
				{
					_config = wait;
				}

				public Config ScaledSeconds()
				{
					_config._timeOption = TimeOption.ScaledTime;
					return _config;
				}

				public Config UnscaledSeconds()
				{
					_config._timeOption = TimeOption.UnscaledTime;
					return _config;
				}
			}

			private float _time;

			private TimeOption _timeOption;

			private Func<bool> _whileCondition;

			private Func<bool> _pauseWhileCondition;

			private Action _thenInvokeAction;

			private Action<Config> _thenInvokeActionWithConfig;

			public bool didTimeout { get; private set; }

			public bool isDone { get; private set; }

			public TimeConfig For(float time)
			{
				_time = time;
				return new TimeConfig(this);
			}

			public Config While(Func<bool> condition)
			{
				_whileCondition = condition;
				return this;
			}

			public Config PauseWhile(Func<bool> condition)
			{
				_pauseWhileCondition = condition;
				return this;
			}

			public Config ThenInvoke(Action action)
			{
				_thenInvokeAction = action;
				return this;
			}

			public Config ThenInvoke(Action<Config> action)
			{
				_thenInvokeActionWithConfig = action;
				return this;
			}

			public Coroutine StartCoroutine()
			{
				return CoroutineUtil.StartCoroutine(WaitCoroutine());
			}

			public Coroutine StartCoroutineOn([NotNull] MonoBehaviour proxy)
			{
				return proxy.StartCoroutine(WaitCoroutine());
			}

			private IEnumerator WaitCoroutine()
			{
				float startTime = ((_timeOption != TimeOption.UnscaledTime) ? Time.time : Time.realtimeSinceStartup);
				float currentTime = startTime;
				double pausedTimeTotal = 0.0;
				double pausedTime = 0.0;
				float pausedStartTime = ((_pauseWhileCondition == null || !_pauseWhileCondition()) ? 0f : startTime);
				while (_whileCondition == null || _whileCondition())
				{
					yield return null;
					if (!(_time > 0f))
					{
						continue;
					}
					if (_pauseWhileCondition != null)
					{
						float num = currentTime;
						currentTime = ((_timeOption != TimeOption.UnscaledTime) ? Time.time : Time.realtimeSinceStartup);
						if (_pauseWhileCondition())
						{
							if (pausedStartTime <= 0f)
							{
								pausedStartTime = num;
							}
							pausedTime = currentTime - pausedStartTime;
						}
						else if (pausedStartTime > 0f)
						{
							pausedTimeTotal += pausedTime;
							pausedTime = 0.0;
							pausedStartTime = 0f;
						}
					}
					else
					{
						currentTime = ((_timeOption != TimeOption.UnscaledTime) ? Time.time : Time.realtimeSinceStartup);
					}
					double num2 = (double)(currentTime - startTime) - (pausedTimeTotal + pausedTime);
					if (num2 > (double)_time)
					{
						didTimeout = true;
						break;
					}
				}
				isDone = true;
				if (_thenInvokeAction != null)
				{
					_thenInvokeAction();
				}
				else if (_thenInvokeActionWithConfig != null)
				{
					_thenInvokeActionWithConfig(this);
				}
			}
		}

		public static Config.TimeConfig For(float time)
		{
			return new Config().For(time);
		}

		public static Config While(Func<bool> condition)
		{
			return new Config().While(condition);
		}

		[Conditional("WAIT_DEBUG_LOGS")]
		private static void Log(string msg)
		{
			Debug.Log("[Wait] " + msg);
		}
	}
}
