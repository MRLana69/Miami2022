using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace SYBO.Subway.Level.TrackEvents
{
	[RequireComponent(typeof(TrackObject))]
	public abstract class TrackEvent : MonoBehaviour
	{
		[Range(0f, 1f)]
		[SerializeField]
		private float _probability = 1f;

		protected Transform _transform;

		private TrackObject _trackObject;

		protected bool _launched;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action Launching__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action Stopped__BackingField;

		public event Action Launching
		{
			add
			{
				Action action = Launching__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref Launching__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = Launching__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref Launching__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action Stopped
		{
			add
			{
				Action action = Stopped__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref Stopped__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = Stopped__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref Stopped__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		protected virtual void Initialize()
		{
			_transform = base.transform;
			_trackObject = GetComponent<TrackObject>();
			_trackObject.OnActivate += TrackObject_OnActivate;
			_trackObject.OnDeactivate += TrackObject_OnDeactivate;
		}

		private void Start()
		{
			Initialize();
		}

		protected virtual void OnDestroy()
		{
			if ((bool)_trackObject)
			{
				_trackObject.OnActivate -= TrackObject_OnActivate;
				_trackObject.OnDeactivate -= TrackObject_OnDeactivate;
			}
		}

		protected abstract void Launch();

		protected void EvaluateAndLaunchEvent()
		{
			if (!_launched && !(_probability < UnityEngine.Random.value))
			{
				if (Launching__BackingField != null)
				{
					Launching__BackingField();
				}
				_launched = true;
				Launch();
			}
		}

		protected virtual void TrackObject_OnActivate()
		{
			EvaluateAndLaunchEvent();
		}

		protected virtual void TrackObject_OnDeactivate()
		{
			_launched = false;
			if (Stopped__BackingField != null)
			{
				Stopped__BackingField();
			}
		}
	}
}
