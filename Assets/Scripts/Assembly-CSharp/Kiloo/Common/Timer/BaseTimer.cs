using System;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;

namespace Kiloo.Common.Timer
{
	public abstract class BaseTimer
	{
		protected readonly bool singleShot;

		protected bool _running;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action intervalElapsed__BackingField;

		public float interval { protected get; set; }

		public bool isRunning
		{
			get
			{
				return _running;
			}
		}

		public event Action intervalElapsed
		{
			add
			{
				Action action = intervalElapsed__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref intervalElapsed__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = intervalElapsed__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref intervalElapsed__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		protected BaseTimer(float timerInterval, bool oneShot = false)
		{
			interval = timerInterval;
			singleShot = oneShot;
		}

		public BaseTimer()
		{
		}

		public abstract void Start();

		public abstract void Stop();

		protected void TriggerIntervalElapsed()
		{
			intervalElapsed__BackingField.SafeInvoke();
		}
	}
}
