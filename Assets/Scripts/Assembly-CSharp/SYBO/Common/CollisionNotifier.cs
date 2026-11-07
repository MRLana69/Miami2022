using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace SYBO.Common
{
	[RequireComponent(typeof(Collider))]
	public class CollisionNotifier : MonoBehaviour
	{
		private bool _initialized;

		private Collider _collider;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<Collider> TriggerEnter__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<Collider> TriggerExit__BackingField;

		public List<Collider> CollisionsEnterThisFrame = new List<Collider>();

		public List<Collider> CollisionsExitThisFrame = new List<Collider>();

		public Collider Collider
		{
			get
			{
				Initialize();
				return _collider;
			}
		}

		public event Action<Collider> TriggerEnter
		{
			add
			{
				Action<Collider> action = TriggerEnter__BackingField;
				Action<Collider> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref TriggerEnter__BackingField, (Action<Collider>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<Collider> action = TriggerEnter__BackingField;
				Action<Collider> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref TriggerEnter__BackingField, (Action<Collider>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<Collider> TriggerExit
		{
			add
			{
				Action<Collider> action = TriggerExit__BackingField;
				Action<Collider> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref TriggerExit__BackingField, (Action<Collider>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<Collider> action = TriggerExit__BackingField;
				Action<Collider> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref TriggerExit__BackingField, (Action<Collider>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		private void Awake()
		{
			Initialize();
		}

		private void Initialize()
		{
			if (!_initialized)
			{
				_collider = GetComponent<Collider>();
				_initialized = true;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			CollisionsEnterThisFrame.Add(other);
			if (TriggerEnter__BackingField != null)
			{
				TriggerEnter__BackingField(other);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			CollisionsExitThisFrame.Add(other);
			if (TriggerExit__BackingField != null)
			{
				TriggerExit__BackingField(other);
			}
		}

		public void ClearFrame()
		{
			CollisionsEnterThisFrame.Clear();
			CollisionsExitThisFrame.Clear();
		}
	}
}
