using System;
using System.Reflection;
using SYBO.Components.Logging;

namespace SYBO.Components.Json
{
	public class JsonFieldInfo
	{
		public FieldInfo Field;

		public Type ValueType;

		public bool SerializeByKey;

		public bool EnumAsString;

		public string SerializedName;

		public int SizeLimit;

		public bool FailOnSizeLimit;

		public string SerializedFileName;

		public object GetValue(object obj)
		{
			return Field.GetValue(obj);
		}

		public void SetValue(object obj, object value)
		{
			Field.SetValue(obj, value);
		}

		public object ReadValue(JsonDict json)
		{
			object value;
			json.TryGetValue(SerializedName, out value);
			return value;
		}

		public object GetKey(object obj)
		{
			return JsonClassInfo.GetJsonTypeReflection(ValueType).GetKey(obj);
		}

		public void WriteValue(object value, JsonDict json)
		{
			json[SerializedName] = value;
		}

		public string ToLimitedString(object value)
		{
			string text = value.ToString();
			if (SizeLimit >= 0 && text != null && text.Length > SizeLimit)
			{
				if (FailOnSizeLimit)
				{
					throw new SizeLimitException(string.Format("JsonField {0} Failed on Size Limit\nSize: {1}\nLimit: {2}\nFull string: {3}", SerializedName, text.Length, SizeLimit, text));
				}
				Log.Json.LogErrorFormat("Json - Truncating field value due to limit ({0})\n{1}", SizeLimit, text);
				return text.Substring(0, SizeLimit);
			}
			return text;
		}
	}
}
