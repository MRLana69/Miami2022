using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace SYBO.Subway.Pickups.Pickers
{
	public class PickerCollection
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<PickerRemoveRequestEventArgs> OnPickerRemoveRequest__BackingField;

		private List<IPicker> _iPickerList;

		private PickupManager _pickupManager;

		public event Action<PickerRemoveRequestEventArgs> OnPickerRemoveRequest
		{
			add
			{
				Action<PickerRemoveRequestEventArgs> action = OnPickerRemoveRequest__BackingField;
				Action<PickerRemoveRequestEventArgs> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPickerRemoveRequest__BackingField, (Action<PickerRemoveRequestEventArgs>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<PickerRemoveRequestEventArgs> action = OnPickerRemoveRequest__BackingField;
				Action<PickerRemoveRequestEventArgs> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnPickerRemoveRequest__BackingField, (Action<PickerRemoveRequestEventArgs>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public PickerCollection(PickupManager pickupManager)
		{
			_pickupManager = pickupManager;
			_iPickerList = new List<IPicker>();
		}

		public void Add(IPicker iPicker)
		{
			if (!_iPickerList.Contains(iPicker))
			{
				_iPickerList.Add(iPicker);
				iPicker.OnRemoveRequest += IPicker_OnRemoveRequest;
				iPicker.Add(_pickupManager);
			}
		}

		public bool HasPickerOfType<T>()
		{
			for (int i = 0; i < _iPickerList.Count; i++)
			{
				if (_iPickerList[i].GetType() == typeof(T))
				{
					return true;
				}
			}
			return false;
		}

		public IPicker GetItemAtIndex(int i)
		{
			return _iPickerList[i];
		}

		public void Remove(IPicker iPicker)
		{
			if (_iPickerList.Remove(iPicker))
			{
				iPicker.OnRemoveRequest -= IPicker_OnRemoveRequest;
			}
		}

		public void UpdatePickers()
		{
			_iPickerList.Sort((IPicker a, IPicker b) => b.ExecutionOrder.CompareTo(a.ExecutionOrder));
			int num = _iPickerList.Count - 1;
			while (num >= 0 && _iPickerList[num].UpdatePicker())
			{
				num--;
			}
		}

		private void IPicker_OnRemoveRequest(PickerRemoveRequestEventArgs pickerRemoveRequestEvent)
		{
			if (OnPickerRemoveRequest__BackingField != null)
			{
				OnPickerRemoveRequest__BackingField(pickerRemoveRequestEvent);
			}
		}
	}
}
