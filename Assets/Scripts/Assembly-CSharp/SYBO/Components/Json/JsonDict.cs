using System;
using System.Collections.Generic;

namespace SYBO.Components.Json
{
	public class JsonDict : Dictionary<string, object>
	{
		public static JsonDict EmptyDict
		{
			get
			{
				return new JsonDict();
			}
		}

		public JsonDict()
		{
		}

		public JsonDict(int capacity)
			: base(capacity)
		{
		}

		public JsonDict GetDict(string name)
		{
			object value;
			if (TryGetValue(name, out value) && value is JsonDict)
			{
				return (JsonDict)value;
			}
			return EmptyDict;
		}

		public JsonDict GetOrCreateDict(string name)
		{
			object value;
			if (TryGetValue(name, out value) && value is JsonDict)
			{
				return (JsonDict)value;
			}
			JsonDict jsonDict = new JsonDict();
			Add(name, jsonDict);
			return jsonDict;
		}

		public JsonList GetList(string name)
		{
			object value;
			if (TryGetValue(name, out value) && value is JsonList)
			{
				return (JsonList)value;
			}
			return JsonList.EmptyList;
		}

		public JsonList GetOrCreateList(string name)
		{
			object value;
			if (TryGetValue(name, out value) && value is JsonList)
			{
				return (JsonList)value;
			}
			JsonList jsonList = new JsonList();
			Add(name, jsonList);
			return jsonList;
		}

		public int GetInt(string name, int defaultValue = 0)
		{
			object value;
			if (!TryGetValue(name, out value))
			{
				return defaultValue;
			}
			if (value is int)
			{
				return (int)value;
			}
			try
			{
				return Convert.ToInt32(value);
			}
			catch
			{
				return defaultValue;
			}
		}

		public long GetInt64(string name, long defaultValue = 0L)
		{
			object value;
			if (!TryGetValue(name, out value))
			{
				return defaultValue;
			}
			if (value is long)
			{
				return (long)value;
			}
			try
			{
				return Convert.ToInt64(value);
			}
			catch
			{
				return defaultValue;
			}
		}

		public float GetFloat(string name, float defaultValue = 0f)
		{
			object value;
			if (!TryGetValue(name, out value))
			{
				return defaultValue;
			}
			if (value is float)
			{
				return (float)value;
			}
			try
			{
				return Convert.ToSingle(value);
			}
			catch
			{
				return defaultValue;
			}
		}

		public double GetDouble(string name, double defaultValue = 0.0)
		{
			object value;
			if (!TryGetValue(name, out value))
			{
				return defaultValue;
			}
			if (value is double)
			{
				return (double)value;
			}
			try
			{
				return Convert.ToDouble(value);
			}
			catch
			{
				return defaultValue;
			}
		}

		public bool GetBool(string name, bool defaultValue = false)
		{
			object value;
			if (!TryGetValue(name, out value))
			{
				return defaultValue;
			}
			if (value is bool)
			{
				return (bool)value;
			}
			return defaultValue;
		}

		public string GetString(string name, string defaultValue = "")
		{
			object value;
			if (!TryGetValue(name, out value))
			{
				return defaultValue;
			}
			if (value == null)
			{
				return null;
			}
			return (value as string) ?? defaultValue;
		}

		public string Serialize()
		{
			return JsonApi.SerializeDom(this);
		}

		public static JsonDict Deserialize(string json)
		{
			return (JsonApi.DeserializeDom(json) as JsonDict) ?? EmptyDict;
		}
	}
}
