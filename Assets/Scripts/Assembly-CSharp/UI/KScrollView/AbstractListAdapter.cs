using System;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;

namespace UI.KScrollView
{
	public abstract class AbstractListAdapter
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<int> onNotifyDataSetChanged__BackingField;

		public event Action<int> onNotifyDataSetChanged
		{
			add
			{
				Action<int> action = onNotifyDataSetChanged__BackingField;
				Action<int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNotifyDataSetChanged__BackingField, (Action<int>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<int> action = onNotifyDataSetChanged__BackingField;
				Action<int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onNotifyDataSetChanged__BackingField, (Action<int>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public abstract int GetDataEntryCount();

		public abstract Container GetContainer(Container container, int dataIndex, bool isMeasurePass = false);

		public virtual int GetModelType(int index)
		{
			return 0;
		}

		public void NotifyDataSetChanged(int selection = 0)
		{
			onNotifyDataSetChanged__BackingField.SafeInvoke(selection);
		}
	}
}
