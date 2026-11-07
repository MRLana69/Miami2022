using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kiloo.Plugins
{
	public static class PluginUtil
	{
		private struct Handler
		{
			public object context;

			public Delegate handler;
		}

		private static int _nextTicket = 1;

		private static readonly Dictionary<int, Handler> _sharedHandlerStore = new Dictionary<int, Handler>();

		public static int GetNextTicket()
		{
			return _nextTicket++;
		}

		public static int CheckInHandler(Delegate handler, object context)
		{
			if ((object)handler != null)
			{
				lock (_sharedHandlerStore)
				{
					int nextTicket = GetNextTicket();
					_sharedHandlerStore.Add(nextTicket, new Handler
					{
						context = context,
						handler = handler
					});
					return nextTicket;
				}
			}
			return 0;
		}

		public static Delegate CheckOutHandler(int ticket)
		{
			if (ticket != 0)
			{
				lock (_sharedHandlerStore)
				{
					Handler value;
					if (_sharedHandlerStore.TryGetValue(ticket, out value))
					{
						_sharedHandlerStore.Remove(ticket);
						return value.handler;
					}
				}
			}
			return null;
		}

		public static IEnumerable<int> RemoveHandlers(object context)
		{
			lock (_sharedHandlerStore)
			{
				List<int> list = new List<int>();
				foreach (KeyValuePair<int, Handler> item in _sharedHandlerStore)
				{
					if (item.Value.context.Equals(context))
					{
						list.Add(item.Key);
					}
				}
				foreach (int item2 in list)
				{
					_sharedHandlerStore.Remove(item2);
				}
				return list;
			}
		}

		public static void RemoveAllHandlers()
		{
			lock (_sharedHandlerStore)
			{
				_sharedHandlerStore.Clear();
			}
		}

		public static T CreateBridgeObject<T>(string name = null, bool dontDestroyOnLoad = true) where T : MonoBehaviour
		{
			if (name == null)
			{
				string name2 = typeof(T).Name;
				string text = name2;
				int num = 0;
				while ((bool)GameObject.Find(text))
				{
					text = string.Format("{0}{1:00}", name2, ++num);
				}
				name = text;
			}
			GameObject gameObject = new GameObject(name);
			gameObject.hideFlags = HideFlags.DontSave;
			T result = gameObject.AddComponent<T>();
			if (dontDestroyOnLoad)
			{
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			return result;
		}
	}
}
