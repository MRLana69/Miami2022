using System;
using System.Collections.Generic;

namespace SYBO.Components.Json
{
	public class JsonObjectStore
	{
		private class JsonObjectStoreTypeEntry
		{
			private Dictionary<string, object> _storedObjects = new Dictionary<string, object>();

			public Type StoreType;

			public object Get(string key)
			{
				object value;
				if (_storedObjects.TryGetValue(key, out value))
				{
					return value;
				}
				throw new JsonReferenceException(string.Format("JsonObjectStore doesn't contain the {0} with key {1}", StoreType.FullName, key));
			}

			public void Add(string key, object obj)
			{
				if (_storedObjects.ContainsKey(key))
				{
					throw new JsonReferenceException(string.Format("JsonObjectStore already contains the {0} with key {1}", StoreType.FullName, key));
				}
				_storedObjects.Add(key, obj);
			}

			public bool Contains(string key)
			{
				return _storedObjects.ContainsKey(key);
			}

			public JsonObjectStoreTypeEntry Duplicate()
			{
				JsonObjectStoreTypeEntry jsonObjectStoreTypeEntry = new JsonObjectStoreTypeEntry();
				jsonObjectStoreTypeEntry.StoreType = StoreType;
				foreach (KeyValuePair<string, object> storedObject in _storedObjects)
				{
					jsonObjectStoreTypeEntry._storedObjects.Add(storedObject.Key, storedObject.Value);
				}
				return jsonObjectStoreTypeEntry;
			}
		}

		private Dictionary<Type, JsonObjectStoreTypeEntry> _types = new Dictionary<Type, JsonObjectStoreTypeEntry>();

		public T Get<T>(object key)
		{
			JsonObjectStoreTypeEntry value;
			if (_types.TryGetValue(typeof(T), out value))
			{
				return (T)value.Get(key.ToString());
			}
			throw new JsonReferenceException(string.Format("JsonObjectStore doesn't contain any objects of type {0}", typeof(T).FullName));
		}

		public object Get(Type type, object key)
		{
			JsonObjectStoreTypeEntry value;
			if (_types.TryGetValue(type, out value))
			{
				return value.Get(key.ToString());
			}
			throw new JsonReferenceException(string.Format("JsonObjectStore doesn't contain any objects of type {0}", type.FullName));
		}

		public void Set<T>(object key, T obj)
		{
			JsonObjectStoreTypeEntry value;
			if (!_types.TryGetValue(typeof(T), out value))
			{
				value = new JsonObjectStoreTypeEntry();
				value.StoreType = typeof(T);
				_types.Add(typeof(T), value);
			}
			value.Add(key.ToString(), obj);
		}

		public void Set(Type type, object key, object obj)
		{
			JsonObjectStoreTypeEntry value;
			if (!_types.TryGetValue(type, out value))
			{
				value = new JsonObjectStoreTypeEntry();
				value.StoreType = type;
				_types.Add(type, value);
			}
			value.Add(key.ToString(), obj);
		}

		public bool HasObject(Type type, object key)
		{
			JsonObjectStoreTypeEntry value;
			if (!_types.TryGetValue(type, out value))
			{
				return false;
			}
			return value.Contains(key.ToString());
		}

		public JsonObjectStore Duplicate()
		{
			JsonObjectStore jsonObjectStore = new JsonObjectStore();
			foreach (KeyValuePair<Type, JsonObjectStoreTypeEntry> type in _types)
			{
				jsonObjectStore._types.Add(type.Key, type.Value.Duplicate());
			}
			return jsonObjectStore;
		}
	}
}
