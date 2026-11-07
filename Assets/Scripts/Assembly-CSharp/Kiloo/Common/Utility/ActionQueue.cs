using System;
using System.Collections.Generic;
using Kiloo.Common.Extensions;

namespace Kiloo.Common.Utility
{
	public class ActionQueue
	{
		private readonly LinkedList<Action<Action>> actionQueue = new LinkedList<Action<Action>>();

		private bool _running;

		public int Count
		{
			get
			{
				return actionQueue.Count;
			}
		}

		public void AddAction(Action<Action> action)
		{
			actionQueue.AddLast(action);
		}

		public void AddAllActionsFromQueue(ActionQueue queue)
		{
			foreach (Action<Action> item in queue.actionQueue)
			{
				AddAction(item);
			}
		}

		public void Clear()
		{
			actionQueue.Clear();
			_running = false;
		}

		public void AddAndRunSequentially(Action<Action> action)
		{
			AddAction(action);
			RunSequentially();
		}

		public void RunSequentially(Action callback = null)
		{
			if (_running)
			{
				return;
			}
			_running = true;
			Action recursiveAction = null;
			recursiveAction = () =>
			{
				LinkedListNode<Action<Action>> element = actionQueue.First;
				if (element != null)
				{
					Action<Action> value = element.Value;
					actionQueue.RemoveFirst();
					value.SafeInvoke(recursiveAction);
				}
				else
				{
					_running = false;
					callback.SafeInvoke();
				}
			};
			recursiveAction();
		}

		public void RunParallel(Action callback = null)
		{
			if (_running)
			{
				return;
			}
			_running = true;
			int size = actionQueue.Count;
			if (size > 0)
			{
				int index = 0;
				Action argument = () =>
				{
					index++;
					if (index >= size)
					{
						_running = false;
						callback.SafeInvoke();
					}
				};
				foreach (Action<Action> item in actionQueue)
				{
					item.SafeInvoke(argument);
				}
				actionQueue.Clear();
			}
			else
			{
				_running = false;
				callback.SafeInvoke();
			}
		}
	}
}
