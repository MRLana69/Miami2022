using System;
using System.Diagnostics;
using Kiloo.Common.Utility;

namespace Kiloo.Common.Timer
{
	public class RealTimer : BaseTimer
	{
		private DateTime _startStamp = DateTime.MinValue;

		public bool multiTrigger;

		public RealTimer(float timerInterval, bool oneShot = false)
			: base(timerInterval, oneShot)
		{
		}

		public RealTimer()
		{
		}

		public override void Start()
		{
			if (!_running)
			{
				_running = true;
				_startStamp = CDateTime.UtcNow;
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
			bool flag = false;
			while ((GetCurrentTime() - _startStamp).TotalSeconds >= (double)base.interval)
			{
				if (!flag || multiTrigger)
				{
					TriggerIntervalElapsed();
					flag = true;
				}
				if (singleShot)
				{
					Stop();
					break;
				}
				_startStamp = _startStamp.AddSeconds(base.interval);
			}
		}

		protected virtual DateTime GetCurrentTime()
		{
			return CDateTime.UtcNow;
		}

		[Conditional("TIMER_LOGS")]
		private static void Log(string message)
		{
		}
	}
}
