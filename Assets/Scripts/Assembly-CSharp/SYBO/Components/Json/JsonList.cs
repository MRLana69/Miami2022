using System;
using System.Collections.Generic;

namespace SYBO.Components.Json
{
	public class JsonList : List<object>
	{
		public static JsonList EmptyList
		{
			get
			{
				return new JsonList();
			}
		}

		public JsonList()
		{
		}

		public JsonList(int capacity)
			: base(capacity)
		{
		}

		public JsonDict GetDict(int index)
		{
			return (base[index] as JsonDict) ?? JsonDict.EmptyDict;
		}

		public JsonList GetList(int index)
		{
			return (base[index] as JsonList) ?? EmptyList;
		}

		public int GetInt(int index, int defaultValue = 0)
		{
			object obj = base[index];
			if (obj is int)
			{
				return (int)obj;
			}
			try
			{
				return Convert.ToInt32(obj);
			}
			catch
			{
				return defaultValue;
			}
		}

		public long GetInt64(int index, long defaultValue = 0L)
		{
			object obj = base[index];
			if (obj is long)
			{
				return (long)obj;
			}
			try
			{
				return Convert.ToInt64(obj);
			}
			catch
			{
				return defaultValue;
			}
		}

		public float GetFloat(int index, float defaultValue = 0f)
		{
			object obj = base[index];
			if (obj is float)
			{
				return (float)obj;
			}
			try
			{
				return Convert.ToSingle(obj);
			}
			catch
			{
				return defaultValue;
			}
		}

		public double GetDouble(int index, double defaultValue = 0.0)
		{
			object obj = base[index];
			if (obj is double)
			{
				return (double)obj;
			}
			try
			{
				return Convert.ToDouble(obj);
			}
			catch
			{
				return defaultValue;
			}
		}

		public bool GetBool(int index, bool defaultValue = false)
		{
			object obj = base[index];
			if (obj is bool)
			{
				return (bool)obj;
			}
			return defaultValue;
		}

		public string GetString(int index, string defaultValue = "")
		{
			return (base[index] as string) ?? defaultValue;
		}

		public static JsonList ParseString(string json)
		{
			return JsonApi.DeserializeDom(json) as JsonList;
		}
	}
}
