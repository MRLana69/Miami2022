using System;

namespace SYBO.Components.Utilities.Data
{
	public class DelayedTickTimer
	{
		private const float UnsetFloat = -1f;

		private float _targetFPS = 1f;

		private long _ticksLast;

		private long _ticksCurrent;

		private float _delayTarget;

		private float _deltaTime;

		public float TargetFPS
		{
			get
			{
				return _targetFPS;
			}
			set
			{
				_targetFPS = value;
			}
		}

		public float DeltaTime
		{
			get
			{
				return _deltaTime;
			}
			set
			{
				_deltaTime = value;
			}
		}

		public DelayedTickTimer(float targetFPS)
		{
			_targetFPS = targetFPS;
			_ticksLast = DateTime.Now.Ticks;
		}

		public bool Tick()
		{
			_ticksCurrent = DateTime.Now.Ticks;
			_delayTarget = 1f / _targetFPS;
			_ticksCurrent = DateTime.Now.Ticks;
			_deltaTime = (float)TimeSpan.FromTicks(_ticksCurrent - _ticksLast).TotalSeconds;
			if (_deltaTime >= _delayTarget)
			{
				_ticksLast = DateTime.Now.Ticks;
				return true;
			}
			return false;
		}
	}
}
