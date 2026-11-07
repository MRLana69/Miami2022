using System;
using System.Diagnostics;
using Kiloo.Common.Utility;

namespace Kiloo.Common.Timer
{
	public class Timer : BaseTimer
	{
		private double _currentInterval;

		private string _name;

		public Timer(float timerInterval, bool oneShot = false)
			: base(timerInterval, oneShot)
		{
		}

		public Timer()
		{
		}

		public void SetName(string name)
		{
			_name = name;
		}

		public override void Start()
		{
			if (!_running)
			{
				_running = true;
				_currentInterval = base.interval;
				StaticUpdater.instance.onUpdate += Update;
			}
		}

		public override void Stop()
		{
			if (_running)
			{
				_running = false;
				StaticUpdater.instance.onUpdate -= Update;
			}
		}

		private void Update(float deltaTime)
		{
			if (!_running)
			{
				return;
			}
			_currentInterval -= deltaTime;
			if (_currentInterval < 0.0)
			{
				TriggerIntervalElapsed();
				if (singleShot)
				{
					Stop();
				}
				else
				{
					_currentInterval = base.interval;
				}
			}
		}

		public static void ExecuteWithDelay(float delay, Action action)
		{
			Timer timer = new Timer(delay, true);
			timer.intervalElapsed += action;
			timer.Start();
		}

		public static void ExecuteNextFrame(Action action)
		{
			Timer timer = new Timer(0f, true);
			timer.intervalElapsed += action;
			timer.Start();
		}

		[Conditional("TIMER_LOGS")]
		private static void Log(string message)
		{
		}
	}
}
