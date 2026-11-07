using System.Collections.Generic;
using JetBrains.Annotations;

namespace Kiloo.Common.Utility
{
	public static class Depository
	{
		private struct Deposit
		{
			public object context;

			public object deposit;
		}

		private static int _nextTicket = 1;

		private static readonly Dictionary<int, Deposit> _sharedStore = new Dictionary<int, Deposit>();

		public static int GetNextTicket()
		{
			return _nextTicket++;
		}

		public static int CheckIn([CanBeNull] object deposit, [CanBeNull] object context)
		{
			if (deposit != null)
			{
				lock (_sharedStore)
				{
					int nextTicket = GetNextTicket();
					_sharedStore.Add(nextTicket, new Deposit
					{
						context = context,
						deposit = deposit
					});
					return nextTicket;
				}
			}
			return 0;
		}

		public static object CheckOut(int ticket)
		{
			if (ticket != 0)
			{
				lock (_sharedStore)
				{
					Deposit value;
					if (_sharedStore.TryGetValue(ticket, out value))
					{
						_sharedStore.Remove(ticket);
						return value.deposit;
					}
				}
			}
			return null;
		}

		public static IDictionary<int, object> RemoveByContext([CanBeNull] object context)
		{
			lock (_sharedStore)
			{
				Dictionary<int, object> dictionary = new Dictionary<int, object>();
				foreach (KeyValuePair<int, Deposit> item in _sharedStore)
				{
					if (item.Value.context.Equals(context))
					{
						dictionary[item.Key] = item.Value.deposit;
					}
				}
				foreach (KeyValuePair<int, object> item2 in dictionary)
				{
					_sharedStore.Remove(item2.Key);
				}
				return dictionary;
			}
		}

		public static void RemoveAll()
		{
			lock (_sharedStore)
			{
				_sharedStore.Clear();
			}
		}
	}
}
