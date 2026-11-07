using System;
using System.Collections.Generic;
using Kiloo.Common.Extensions;

namespace SYBO.Common.StateSystem
{
	public class StateMachine
	{
		private class Trigger
		{
			public readonly int Id;

			public Action<Trigger> OnTriggered;

			public Trigger(int id)
			{
				Id = id;
			}

			public void Set()
			{
				OnTriggered.SafeInvoke(this);
			}
		}

		private class Transition
		{
			public readonly State From;

			public readonly State To;

			public readonly Trigger Trigger;

			public Transition(State from, State to, Trigger trigger)
			{
				From = from;
				To = to;
				Trigger = trigger;
			}
		}

		private State _currentState;

		private bool _initialStateEntered;

		private Dictionary<int, State> _states = new Dictionary<int, State>();

		private List<Transition> _transitions = new List<Transition>();

		private Dictionary<int, Trigger> _triggers = new Dictionary<int, Trigger>();

		public StateMachine(IScheduler scheduler)
		{
			if (scheduler != null)
			{
				scheduler.RegisterUpdateHandler(OnUpdate);
				scheduler.RegisterLateUpdateHandler(OnLateUpdate);
			}
		}

		public void EnterInitialState(int id)
		{
			if (!_initialStateEntered)
			{
				_currentState = GetState<State>(id);
				if (_currentState != null)
				{
					_currentState.Enter(_currentState);
					_initialStateEntered = true;
				}
			}
		}

		public void AddState(State state)
		{
			_states.Add(state.Id, state);
		}

		public T GetState<T>(int id) where T : State
		{
			if (_states.ContainsKey(id))
			{
				return _states[id] as T;
			}
			SDebug.LogErrorFormat("No state was found for id {0}", id);
			return (T)null;
		}

		public bool IsCurrentState<T>() where T : State
		{
			return _currentState is T;
		}

		public T TryGetCurrentState<T>() where T : State
		{
			return _currentState as T;
		}

		public void ActivateTrigger(int id)
		{
			if (_triggers.ContainsKey(id))
			{
				_triggers[id].Set();
				return;
			}
			SDebug.LogErrorFormat("No trigger was found for id {0}", id);
		}

		private Trigger GetOrAddTrigger(int id)
		{
			if (_triggers.ContainsKey(id))
			{
				return _triggers[id];
			}
			return new Trigger(id);
		}

		private void AddTransition(State from, State to, Trigger trigger)
		{
			_transitions.Add(new Transition(from, to, trigger));
			if (!_triggers.ContainsKey(trigger.Id))
			{
				_triggers.Add(trigger.Id, trigger);
				trigger.OnTriggered = (Action<Trigger>)Delegate.Combine(trigger.OnTriggered, new Action<Trigger>(Trigger_OnTriggered));
			}
		}

		public void AddTransition(int fromId, int toId, int triggerId)
		{
			AddTransition(GetState<State>(fromId), GetState<State>(toId), GetOrAddTrigger(triggerId));
		}

		private void Trigger_OnTriggered(Trigger trigger)
		{
			foreach (Transition transition in _transitions)
			{
				if (transition.From == _currentState && transition.Trigger == trigger)
				{
					_currentState.Leave(transition.To);
					_currentState = transition.To;
					_currentState.Enter(transition.From);
					break;
				}
			}
		}

		private void OnUpdate()
		{
			if (_initialStateEntered)
			{
				_currentState.OnUpdate();
			}
		}

		private void OnLateUpdate()
		{
			if (_initialStateEntered)
			{
				_currentState.OnLateUpdate();
			}
		}
	}
}
