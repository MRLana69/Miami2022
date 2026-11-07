using System;
using System.Collections.Generic;

namespace SYBO.Components.Json
{
	public class JsonCustomSerializerCollection
	{
		private Dictionary<Type, JsonCustomSerializer> _customSerializers = new Dictionary<Type, JsonCustomSerializer>();

		public int CustomSerializerCount
		{
			get
			{
				return _customSerializers.Count;
			}
		}

		public bool HasSerializer(Type type)
		{
			return _customSerializers.ContainsKey(type);
		}

		public JsonDict Serialize(Type type, object obj, JsonContext context)
		{
			return _customSerializers[type].Serialize(obj, context);
		}

		public object Deserialize(Type type, JsonDict json, JsonContext context)
		{
			return _customSerializers[type].Deserialize(json, context);
		}

		public JsonList SerializeList(Type type, object obj, JsonContext context)
		{
			return _customSerializers[type].SerializeList(obj, context);
		}

		public object DeserializeList(Type type, JsonList json, JsonContext context)
		{
			return _customSerializers[type].DeserializeList(json, context);
		}

		public void AddSerializer(Type type, JsonCustomSerializer serializer)
		{
			_customSerializers[type] = serializer;
		}

		public JsonCustomSerializerCollection Duplicate()
		{
			JsonCustomSerializerCollection jsonCustomSerializerCollection = new JsonCustomSerializerCollection();
			foreach (KeyValuePair<Type, JsonCustomSerializer> customSerializer in _customSerializers)
			{
				jsonCustomSerializerCollection._customSerializers.Add(customSerializer.Key, customSerializer.Value);
			}
			return jsonCustomSerializerCollection;
		}
	}
}
