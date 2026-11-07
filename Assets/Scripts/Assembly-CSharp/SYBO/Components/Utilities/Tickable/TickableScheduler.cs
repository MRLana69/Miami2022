using System;
using System.Collections.Generic;
using SYBO.Components.Utilities.Data;
using UnityEngine;

namespace SYBO.Components.Utilities.Tickable
{
	[Serializable]
	[ExecuteInEditMode]
	public class TickableScheduler : SingletonMonoBehaviour<TickableScheduler>
	{
		public const float TargetEditTimeFPS = 60f;

		[HideInInspector]
		public TickableEvent OnRegisteredTickablesChanged = new TickableEvent();

		private List<ITickable> _registeredTickables;

		private bool _isApplicationPaused;

		private bool _isApplicationFocused;

		private DelayedTickTimer _editTimeDelayedTickTimer;

		public bool IsPaused
		{
			get
			{
				return _isApplicationPaused;
			}
			set
			{
				_isApplicationPaused = value;
			}
		}

		public int RegisteredTickablesCount
		{
			get
			{
				return _registeredTickables.Count;
			}
		}

		protected void OnApplicationFocus(bool hasFocus)
		{
			_isApplicationFocused = !hasFocus;
		}

		protected void OnApplicationPause(bool isPaused)
		{
			_isApplicationPaused = isPaused;
		}

		protected void OnEnable()
		{
			Initialize();
		}

		public void Initialize()
		{
			UnregisterAllTickables();
		}

		private void OnDisable()
		{
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			UnregisterAllTickables();
		}

		protected void Update()
		{
			Tick(Time.deltaTime);
		}

		public void RegisterTickable(ITickable tickable)
		{
			_registeredTickables.Add(tickable);
			OnRegisteredTickablesChanged.Invoke(tickable);
		}

		public void Tick(float deltaTime)
		{
			if (_registeredTickables.Count > 0)
			{
			}
			for (int num = _registeredTickables.Count - 1; num >= 0; num--)
			{
				if (_registeredTickables.Count > num)
				{
					if (Application.isPlaying)
					{
						_registeredTickables[num].OnTick(_isApplicationFocused, _isApplicationPaused, deltaTime);
					}
					else
					{
						_registeredTickables[num].OnTick(_isApplicationFocused, false, deltaTime);
					}
				}
			}
		}

		public bool HasRegisteredTickable(ITickable tickable)
		{
			return _registeredTickables.Contains(tickable);
		}

		public void UnregisterTickable(ITickable tickable)
		{
			if (HasRegisteredTickable(tickable))
			{
				_registeredTickables.Remove(tickable);
			}
			OnRegisteredTickablesChanged.Invoke(tickable);
		}

		private void UnregisterAllTickables()
		{
			if (_registeredTickables == null)
			{
				_registeredTickables = new List<ITickable>();
			}
			SingletonMonoBehaviour.LogFormat(1, "{0}.UnregisterAllTickables() BEFORE Count={1}.", GetType(), _registeredTickables.Count);
			for (int num = _registeredTickables.Count - 1; num >= 0; num--)
			{
				if (HasRegisteredTickable(_registeredTickables[num]))
				{
					_registeredTickables.Remove(_registeredTickables[num]);
				}
				else
				{
					SingletonMonoBehaviour.LogErrorFormat("{0}.UnregisterTickable() Fail. This not previously registered, tickable={1}.", GetType(), _registeredTickables[num]);
				}
			}
			SingletonMonoBehaviour.LogFormat(1, "{0}.UnregisterAllTickables() AFTER Count={1}.", GetType(), _registeredTickables.Count);
			OnRegisteredTickablesChanged.Invoke(null);
		}
	}
}
