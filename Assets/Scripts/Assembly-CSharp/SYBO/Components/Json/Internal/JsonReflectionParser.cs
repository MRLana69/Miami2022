using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using SYBO.Components.Logging;
using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Components.Json.Internal
{
	public static class JsonReflectionParser
	{
		public const string DateTimeFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFF'Z'";

		public const string DateTimeFormatString2 = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";

		public static T Deserialize<T>(object json, JsonContext context = null)
		{
			return (T)Deserialize(typeof(T), json, context);
		}

		public static void DeserializeOnto<T>(T obj, JsonDict json, JsonContext context = null)
		{
			if (context == null)
			{
				context = new JsonContext();
			}
			ReflectionDeserializeOnto(typeof(T), obj, json, context);
		}

		public static object Deserialize(Type type, object json, JsonContext context = null)
		{
			if (context == null)
			{
				context = new JsonContext();
			}
			context.BeginDeserialize();
			object result = RecursiveReadValue(type, json, context);
			context.EndDeserialize();
			return result;
		}

		public static object Serialize<T>(T obj, JsonContext context = null)
		{
			return Serialize(typeof(T), obj, context);
		}

		public static object Serialize(Type type, object obj, JsonContext context = null)
		{
			if (context == null)
			{
				context = new JsonContext();
			}
			context.BeginSerialize();
			object result = RecursiveWriteValue(obj, type, null, context);
			context.EndSerialize();
			return result;
		}

		public static void CollectReferenceUnknownKeys(Type type, object value, JsonContext context, Dictionary<Type, HashSet<string>> output)
		{
			if (value == null)
			{
				return;
			}
			Type type2;
			if ((type2 = ReflectionWrapper.GetInterface(type, typeof(IList<>))) != null)
			{
				Type type3 = ReflectionWrapper.GetGenericArguments(type2)[0];
				JsonList jsonList = value as JsonList;
				if (jsonList == null)
				{
					string[] array = value.ToString().Split(',');
					for (int i = 0; i < array.Length; i++)
					{
						CollectReferenceUnknownKeys(type3, array[i].Trim(), context, output);
					}
				}
				else
				{
					for (int j = 0; j < jsonList.Count; j++)
					{
						CollectReferenceUnknownKeys(type3, jsonList[j], context, output);
					}
				}
			}
			else
			{
				if (ReflectionWrapper.IsGenericType(type) && type.GetGenericTypeDefinition() == typeof(Dictionary<, >))
				{
					JsonDict jsonDict = value as JsonDict;
					if (jsonDict == null)
					{
						return;
					}
					Type type4 = ReflectionWrapper.GetGenericArguments(type)[0];
					Type type5 = ReflectionWrapper.GetGenericArguments(type)[1];
					{
						foreach (KeyValuePair<string, object> item in jsonDict)
						{
							JsonClassInfo jsonTypeReflection = JsonClassInfo.GetJsonTypeReflection(type4);
							if (jsonTypeReflection.TypeHasKey)
							{
								CollectReferenceUnknownKeys(type4, item.Key, context, output);
							}
							CollectReferenceUnknownKeys(type5, item.Value, context, output);
						}
						return;
					}
				}
				if (ReflectionWrapper.IsValueType(type))
				{
					return;
				}
				string text = value.ToString();
				if (!context.ObjectStore.HasObject(type, text))
				{
					HashSet<string> value2;
					if (!output.TryGetValue(type, out value2))
					{
						value2 = new HashSet<string>();
						output.Add(type, value2);
					}
					value2.Add(text);
				}
			}
		}

		public static object ReadReference(Type type, object value, JsonContext context)
		{
			if (value == null)
			{
				return null;
			}
			Type type2;
			if ((type2 = ReflectionWrapper.GetInterface(type, typeof(IList<>))) != null)
			{
				JsonList jsonList = value as JsonList;
				if (jsonList == null)
				{
					jsonList = new JsonList();
					string[] array = value.ToString().Split(',');
					for (int i = 0; i < array.Length; i++)
					{
						jsonList.Add(array[i].Trim());
					}
				}
				Type type3 = ReflectionWrapper.GetGenericArguments(type2)[0];
				ConstructorInfo constructor = ReflectionWrapper.GetConstructor(type, new Type[1] { typeof(int) });
				IList list = ((constructor == null) ? (ReflectionWrapper.GetConstructor(type).Invoke(null) as IList) : (constructor.Invoke(new object[1] { jsonList.Count }) as IList));
				for (int j = 0; j < jsonList.Count; j++)
				{
					object value2 = ReadReference(type3, jsonList[j], context);
					if (list.Count > j)
					{
						list[j] = value2;
					}
					else
					{
						list.Add(value2);
					}
				}
				return list;
			}
			if (ReflectionWrapper.IsGenericType(type) && type.GetGenericTypeDefinition() == typeof(Dictionary<, >))
			{
				JsonDict jsonDict = value as JsonDict;
				if (jsonDict == null)
				{
					throw new FormatException(string.Format("Cannot translate value {0} to a Dictionary", value));
				}
				ConstructorInfo constructor2 = ReflectionWrapper.GetConstructor(type);
				IDictionary dictionary = constructor2.Invoke(null) as IDictionary;
				Type type4 = ReflectionWrapper.GetGenericArguments(type)[0];
				Type type5 = ReflectionWrapper.GetGenericArguments(type)[1];
				{
					foreach (KeyValuePair<string, object> item in jsonDict)
					{
						dictionary[DictionaryResolveKey(item.Key, type4, context)] = ReadReference(type5, item.Value, context);
					}
					return dictionary;
				}
			}
			if (!ReflectionWrapper.IsValueType(type))
			{
				return context.ObjectStore.Get(type, value);
			}
			Log.Json.LogErrorFormat("Unsupported data type in Settings: {0}", type.FullName);
			return null;
		}

		public static object ReflectionDeserialize(Type objType, JsonDict json, JsonContext context)
		{
			if (context == null)
			{
				context = new JsonContext();
			}
			JsonClassInfo jsonTypeReflection = JsonClassInfo.GetJsonTypeReflection(objType);
			object obj = jsonTypeReflection.Construct(context);
			ReflectionDeserializeOnto(objType, obj, json, context);
			return obj;
		}

		public static void ReflectionDeserializeOnto(Type objType, object obj, JsonDict json, JsonContext context)
		{
			JsonClassInfo jsonTypeReflection = JsonClassInfo.GetJsonTypeReflection(objType);
			DeserializeFieldsOnto(jsonTypeReflection.CachedSerializedFields, obj, json, context);
			if (context.IncludeStatics)
			{
				DeserializeFieldsOnto(jsonTypeReflection.CachedSerializedStaticFields, obj, json, context);
			}
			if (jsonTypeReflection.TypeHasKey)
			{
				context.ObjectStore.Set(objType, jsonTypeReflection.GetKey(obj), obj);
			}
		}

		private static void DeserializeFieldsOnto(JsonFieldInfo[] fields, object obj, JsonDict json, JsonContext context)
		{
			foreach (JsonFieldInfo jsonFieldInfo in fields)
			{
				if (context.ShouldFilterField == null || !context.ShouldFilterField(jsonFieldInfo))
				{
					if (NeedsFileReference(jsonFieldInfo))
					{
						CreateFileReference(obj, jsonFieldInfo, context);
					}
					else
					{
						DeserializeFieldOnto(jsonFieldInfo, obj, json, context);
					}
				}
			}
		}

		public static void DeserializeFieldOnto(JsonFieldInfo jsonField, object obj, JsonDict json, JsonContext context)
		{
			object obj2 = jsonField.ReadValue(json);
			if (obj2 != null)
			{
				if (jsonField.SerializeByKey)
				{
					context.RegisterDeserializeByKeyReference(obj, jsonField, obj2);
				}
				else
				{
					RecursiveReadField(obj, jsonField.Field, obj2, context);
				}
			}
		}

		public static JsonDict ReflectionSerialize(Type type, object obj, JsonContext context)
		{
			JsonClassInfo jsonTypeReflection = JsonClassInfo.GetJsonTypeReflection(type);
			JsonDict jsonDict = new JsonDict(jsonTypeReflection.CachedSerializedFields.Length);
			if (jsonTypeReflection.TypeHasKey)
			{
				context.ObjectStore.Set(type, jsonTypeReflection.GetKey(obj), obj);
			}
			SerializeFieldsFrom(jsonTypeReflection.CachedSerializedFields, obj, jsonDict, context);
			if (context.IncludeStatics)
			{
				SerializeFieldsFrom(jsonTypeReflection.CachedSerializedStaticFields, obj, jsonDict, context);
			}
			return jsonDict;
		}

		public static void PrepareReflectionForSerialization(Type[] serializationObjectTypes)
		{
			HashSet<Type> preparedClasses = new HashSet<Type>();
			for (int i = 0; i < serializationObjectTypes.Length; i++)
			{
				PrepareReflectionRecursively(serializationObjectTypes[i], preparedClasses);
			}
		}

		private static void PrepareReflectionRecursively(Type type, HashSet<Type> preparedClasses)
		{
			if (!preparedClasses.Contains(type))
			{
				preparedClasses.Add(type);
				JsonClassInfo jsonTypeReflection = JsonClassInfo.GetJsonTypeReflection(type);
				for (int i = 0; i < jsonTypeReflection.CachedSerializedFields.Length; i++)
				{
					PrepareReflectionRecursively(jsonTypeReflection.CachedSerializedFields[i].ValueType, preparedClasses);
				}
			}
		}

		private static void SerializeFieldsFrom(JsonFieldInfo[] fields, object obj, JsonDict json, JsonContext context)
		{
			foreach (JsonFieldInfo jsonFieldInfo in fields)
			{
				if (context.ShouldFilterField == null || !context.ShouldFilterField(jsonFieldInfo))
				{
					if (NeedsFileReference(jsonFieldInfo))
					{
						CreateFileReference(obj, jsonFieldInfo, context);
						continue;
					}
					object value = RecursiveWriteValue(jsonFieldInfo.GetValue(obj), jsonFieldInfo.Field.FieldType, jsonFieldInfo, context);
					jsonFieldInfo.WriteValue(value, json);
				}
			}
		}

		private static bool NeedsFileReference(JsonFieldInfo field)
		{
			return ReflectionWrapper.GetCustomAttribute(field.Field, typeof(SerializeFileAttribute)) != null;
		}

		private static void CreateFileReference(object obj, JsonFieldInfo field, JsonContext context)
		{
			SerializeFileAttribute serializeFileAttribute = ReflectionWrapper.GetCustomAttribute(field.Field, typeof(SerializeFileAttribute)) as SerializeFileAttribute;
			if (serializeFileAttribute != null)
			{
				field.SerializedFileName = serializeFileAttribute.SerializedFilename;
				JsonContext.SubtreeFileReference subtreeFileReference = new JsonContext.SubtreeFileReference();
				subtreeFileReference.Filename = serializeFileAttribute.SerializedFilename;
				subtreeFileReference.SubObject = obj;
				subtreeFileReference.Field = field;
				context.JsonFileReferences.Add(subtreeFileReference);
			}
		}

		private static object RecursiveWriteValue(object value, Type ft, JsonFieldInfo field, JsonContext context)
		{
			if (value == null)
			{
				return null;
			}
			if (context.CustomSerializers.HasSerializer(ft) && (field == null || !field.SerializeByKey))
			{
				if (ReflectionWrapper.GetInterface(ft, typeof(IList<>)) != null)
				{
					context.BeginSerialize();
					JsonList result = context.CustomSerializers.SerializeList(ft, value, context);
					context.EndSerialize();
					return result;
				}
				context.BeginSerialize();
				JsonDict result2 = context.CustomSerializers.Serialize(ft, value, context);
				context.EndSerialize();
				return result2;
			}
			if (ft == typeof(string))
			{
				return field.ToLimitedString(value);
			}
			if (ft == typeof(IntPtr) || ft == typeof(UIntPtr) || ft == typeof(IntPtr?) || ft == typeof(UIntPtr?))
			{
				throw new JsonSerializationException("IntPtr types are not supported.");
			}
			if (ft == typeof(ulong) || ft == typeof(ulong?))
			{
				throw new JsonSerializationException("ulongs are not supported.");
			}
			if (ReflectionWrapper.IsPrimitive(ft) || (ReflectionWrapper.IsGenericType(ft) && ft.GetGenericTypeDefinition() == typeof(Nullable<>) && ReflectionWrapper.IsPrimitive(Nullable.GetUnderlyingType(ft))) || ft == typeof(JsonDict) || ft == typeof(JsonList))
			{
				return value;
			}
			if (ft == typeof(DateTime))
			{
				return field.ToLimitedString(((DateTime)value).ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFF'Z'"));
			}
			if (ft == typeof(Guid))
			{
				return field.ToLimitedString((Guid)value);
			}
			Type type;
			if ((type = ReflectionWrapper.GetInterface(ft, typeof(IList<>))) != null)
			{
				Type ft2 = ReflectionWrapper.GetGenericArguments(type)[0];
				IList list = value as IList;
				JsonList jsonList = new JsonList(list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					jsonList.Add(RecursiveWriteValue(list[i], ft2, field, context));
				}
				return jsonList;
			}
			if ((type = ReflectionWrapper.GetInterface(ft, typeof(IDictionary<, >))) != null)
			{
				Type type2 = ReflectionWrapper.GetGenericArguments(type)[0];
				Type ft3 = ReflectionWrapper.GetGenericArguments(type)[1];
				IDictionary dictionary = value as IDictionary;
				JsonDict jsonDict = new JsonDict(dictionary.Count);
				{
					foreach (DictionaryEntry item in dictionary)
					{
						jsonDict[GetKey(item.Key, type2)] = RecursiveWriteValue(item.Value, ft3, field, context);
					}
					return jsonDict;
				}
			}
			if (ReflectionWrapper.IsEnum(ft))
			{
				if (field != null && field.EnumAsString)
				{
					return field.ToLimitedString(value);
				}
				return (int)value;
			}
			if (field != null && field.SerializeByKey)
			{
				object key = field.GetKey(value);
				context.RegisterSerializeByKeyReference(field, key);
				return key;
			}
			return RecursiveWriteObject(value, context);
		}

		private static string GetKey(object obj, Type type)
		{
			JsonClassInfo jsonTypeReflection = JsonClassInfo.GetJsonTypeReflection(type);
			if (jsonTypeReflection.TypeHasKey)
			{
				return jsonTypeReflection.GetKey(obj);
			}
			return obj.ToString();
		}

		private static JsonDict RecursiveWriteObject(object obj, JsonContext context)
		{
			context.BeginSerialize();
			Type type = obj.GetType();
			if (context.CustomSerializers.HasSerializer(type))
			{
				JsonDict result = context.CustomSerializers.Serialize(type, obj, context);
				context.EndSerialize();
				return result;
			}
			JsonDict result2 = ReflectionSerialize(type, obj, context);
			context.EndSerialize();
			return result2;
		}

		private static object RecursiveReadValue(Type type, object value, JsonContext context)
		{
			if (value == null)
			{
				return null;
			}
			if (context.CustomSerializers.HasSerializer(type))
			{
				if (ReflectionWrapper.GetInterface(type, typeof(IList<>)) != null)
				{
					context.BeginDeserialize();
					object result = context.CustomSerializers.DeserializeList(type, value as JsonList, context);
					context.EndDeserialize();
					return result;
				}
				context.BeginDeserialize();
				object result2 = context.CustomSerializers.Deserialize(type, value as JsonDict, context);
				context.EndDeserialize();
				return result2;
			}
			if (type == typeof(int) || type == typeof(int?))
			{
				return ConvertOrDefault<int>(value);
			}
			if (type == typeof(short) || type == typeof(short?))
			{
				return ConvertOrDefault<short>(value);
			}
			if (type == typeof(uint) || type == typeof(uint?))
			{
				return ConvertOrDefault<uint>(value);
			}
			if (type == typeof(ushort) || type == typeof(ushort?))
			{
				return ConvertOrDefault<ushort>(value);
			}
			if (type == typeof(sbyte) || type == typeof(sbyte?))
			{
				return ConvertOrDefault<sbyte>(value);
			}
			if (type == typeof(byte) || type == typeof(byte?))
			{
				return ConvertOrDefault<byte>(value);
			}
			if (type == typeof(char) || type == typeof(char?))
			{
				return ConvertOrDefault<char>(value);
			}
			if (type == typeof(long) || type == typeof(long?))
			{
				return ConvertOrDefault<long>(value);
			}
			if (type == typeof(float) || type == typeof(float?))
			{
				return ConvertOrDefault<float>(value);
			}
			if (type == typeof(double) || type == typeof(double?))
			{
				return ConvertOrDefault<double>(value);
			}
			if (type == typeof(bool) || type == typeof(bool?))
			{
				if (value is bool)
				{
					return value;
				}
				throw new FormatException(string.Format("Cannot translate value {0} to a bool", value));
			}
			if (type == typeof(string))
			{
				if (value is string)
				{
					return value;
				}
				return value.ToString();
			}
			if (type == typeof(JsonList))
			{
				if (value is JsonList)
				{
					return value;
				}
				throw new FormatException(string.Format("Cannot translate value {0} to a JsonList", value));
			}
			if (type == typeof(JsonDict))
			{
				if (value is JsonDict)
				{
					return value;
				}
				throw new FormatException(string.Format("Cannot translate value {0} to a JsonDict", value));
			}
			Type type2;
			if ((type2 = ReflectionWrapper.GetInterface(type, typeof(IList<>))) != null)
			{
				JsonList jsonList = value as JsonList;
				if (jsonList == null)
				{
					jsonList = new JsonList();
					string[] array = value.ToString().Split(',');
					for (int i = 0; i < array.Length; i++)
					{
						jsonList.Add(array[i].Trim());
					}
				}
				Type type3 = ReflectionWrapper.GetGenericArguments(type2)[0];
				ConstructorInfo constructor = ReflectionWrapper.GetConstructor(type, new Type[1] { typeof(int) });
				IList list = ((constructor == null) ? (ReflectionWrapper.GetConstructor(type).Invoke(null) as IList) : (constructor.Invoke(new object[1] { jsonList.Count }) as IList));
				for (int j = 0; j < jsonList.Count; j++)
				{
					object value2 = RecursiveReadValue(type3, jsonList[j], context);
					if (list.Count > j)
					{
						list[j] = value2;
					}
					else
					{
						list.Add(value2);
					}
				}
				return list;
			}
			if (ReflectionWrapper.IsGenericType(type) && type.GetGenericTypeDefinition() == typeof(Dictionary<, >))
			{
				JsonDict jsonDict = value as JsonDict;
				if (jsonDict == null)
				{
					throw new FormatException(string.Format("Cannot translate value {0} to a Dictionary", value));
				}
				ConstructorInfo constructor2 = ReflectionWrapper.GetConstructor(type);
				IDictionary dictionary = constructor2.Invoke(null) as IDictionary;
				Type type4 = ReflectionWrapper.GetGenericArguments(type)[0];
				Type type5 = ReflectionWrapper.GetGenericArguments(type)[1];
				{
					foreach (KeyValuePair<string, object> item in jsonDict)
					{
						dictionary[DictionaryResolveKey(item.Key, type4, context)] = RecursiveReadValue(type5, item.Value, context);
					}
					return dictionary;
				}
			}
			if (type == typeof(DateTime))
			{
				string text = value as string;
				if (text == null)
				{
					throw new FormatException(string.Format("Cannot translate value {0} to a DateTime", value));
				}
				DateTime result3;
				if (!DateTime.TryParseExact(text, "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFF'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result3) && !DateTime.TryParseExact(text, "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result3))
				{
					Debug.LogFormat("{0}", text);
					Debug.LogFormat("{0}", "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFF'Z'");
					Debug.LogFormat("{0}", "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
				}
				return result3;
			}
			if (type == typeof(Guid))
			{
				string text2 = value as string;
				if (text2 == null)
				{
					throw new FormatException(string.Format("Cannot translate value {0} to a DateTime", value));
				}
				return new Guid(text2);
			}
			if (ReflectionWrapper.IsEnum(type))
			{
				if (value is string)
				{
					return Enum.Parse(type, (string)value, true);
				}
				if (value is int)
				{
					return value;
				}
				return Convert.ToInt32(value);
			}
			JsonDict jsonDict2 = value as JsonDict;
			if (jsonDict2 == null)
			{
				throw new FormatException(string.Format("Cannot translate value {0} to a {1}", value, type));
			}
			return RecursiveReadObject(type, jsonDict2, context);
		}

		private static object ConvertOrDefault<T>(object value)
		{
			if (value is T)
			{
				return value;
			}
			try
			{
				return Convert.ChangeType(value, typeof(T));
			}
			catch (InvalidCastException)
			{
				return default(T);
			}
		}

		private static object DictionaryResolveKey(string key, Type type, JsonContext context)
		{
			JsonClassInfo jsonTypeReflection = JsonClassInfo.GetJsonTypeReflection(type);
			if (jsonTypeReflection.TypeHasKey)
			{
				return context.ObjectStore.Get(type, key);
			}
			if (ReflectionWrapper.IsEnum(type))
			{
				try
				{
					return Enum.Parse(type, key);
				}
				catch (ArgumentException)
				{
					return Enum.ToObject(type, int.Parse(key));
				}
			}
			return Convert.ChangeType(key, type);
		}

		private static void RecursiveReadField(object obj, FieldInfo field, object jsonValue, JsonContext context)
		{
			field.SetValue(obj, RecursiveReadValue(field.FieldType, jsonValue, context));
		}

		private static object RecursiveReadObject(Type objType, JsonDict json, JsonContext context)
		{
			object obj = ((!context.CustomSerializers.HasSerializer(objType)) ? ReflectionDeserialize(objType, json, context) : context.CustomSerializers.Deserialize(objType, json, context));
			ISerializationCallbackReceiver serializationCallbackReceiver = obj as ISerializationCallbackReceiver;
			if (serializationCallbackReceiver != null)
			{
				context.RegisterSerializationCallBack(serializationCallbackReceiver);
			}
			return obj;
		}
	}
}
