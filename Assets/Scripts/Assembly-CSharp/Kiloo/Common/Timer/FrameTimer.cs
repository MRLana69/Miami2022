using System;
using Kiloo.Common.Utility;

namespace Kiloo.Common.Timer
{
	public class FrameTimer : BaseTimer
	{
		private int _elapsedFrames;

		private int _framesToDelay;

		private FrameTimer(int framesToDelay, bool oneShot = false)
			: base(0f, oneShot)
		{
			base.interval = framesToDelay;
			_framesToDelay = framesToDelay;
		}

		public static void ExecuteWithDelay(int framesToDelay, Action action)
		{
			FrameTimer frameTimer = new FrameTimer(framesToDelay, true);
			frameTimer.intervalElapsed += action;
			frameTimer._framesToDelay = framesToDelay;
			frameTimer.Start();
		}

		public override void Start()
		{
			if (!_running)
			{
				_running = true;
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
			_elapsedFrames++;
			if (_elapsedFrames >= _framesToDelay)
			{
				TriggerIntervalElapsed();
				if (singleShot)
				{
					Stop();
				}
				else
				{
					_elapsedFrames = 0;
				}
			}
		}
	}
}
