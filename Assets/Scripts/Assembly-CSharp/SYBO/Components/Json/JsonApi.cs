using System;
using SYBO.Components.Json.Internal;

namespace SYBO.Components.Json
{
	public class JsonApi
	{
		public static int BuilderCapacity
		{
			get
			{
				return JsonParser.BuilderCapacity;
			}
			set
			{
				JsonParser.BuilderCapacity = value;
			}
		}

		public static T Deserialize<T>(string json, JsonContext context = null)
		{
			return JsonParser.Deserialize<T>(json, context);
		}

		public static string Serialize<T>(T obj, JsonContext context = null)
		{
			return JsonParser.Serialize(obj, context);
		}

		public static object DeserializeDom(string json)
		{
			return JsonParser.DeserializeDom(json);
		}

		public static string SerializeDom(object json, JsonContext context = null)
		{
			if (context == null)
			{
				context = new JsonContext();
			}
			return JsonParser.SerializeDom(json, context);
		}

		public static T ReflectionDeserializeDom<T>(object jsonobj, JsonContext context = null)
		{
			return JsonReflectionParser.Deserialize<T>(jsonobj, context);
		}

		public static object ReflectionDeserializeDom(Type type, object jsonobj, JsonContext context = null)
		{
			return JsonReflectionParser.Deserialize(type, jsonobj, context);
		}

		public static object ReflectionSerializeDom<T>(T data, JsonContext context = null)
		{
			return JsonReflectionParser.Serialize(data, context);
		}

		public static object ReflectionSerializeDom(Type type, object data, JsonContext context = null)
		{
			return JsonReflectionParser.Serialize(type, data, context);
		}

		public static object ReflectionDeserializeDomAsType(Type objType, JsonDict json, JsonContext context = null)
		{
			return JsonReflectionParser.ReflectionDeserialize(objType, json, context);
		}

		public static JsonDict ReflectionSerializeDomAsType(Type objType, object obj, JsonContext context = null)
		{
			return JsonReflectionParser.ReflectionSerialize(objType, obj, context);
		}

		public static bool LastDecodeSuccessful()
		{
			return JsonParser.LastDecodeSuccessful();
		}

		public static int GetLastErrorIndex()
		{
			return JsonParser.GetLastErrorIndex();
		}

		public static string GetLastErrorSnippet()
		{
			return JsonParser.GetLastErrorSnippet();
		}

		public static void PrepareReflectionForSerialization(params Type[] serializationObjectTypes)
		{
			JsonReflectionParser.PrepareReflectionForSerialization(serializationObjectTypes);
		}
	}
}
