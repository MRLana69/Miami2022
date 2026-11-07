using System;
using System.Collections.Generic;
using System.Reflection;
using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Components.Json
{
	public class JsonClassInfo
	{
		public readonly Type type;

		public readonly JsonFieldInfo[] CachedSerializedFields;

		public readonly JsonFieldInfo[] CachedSerializedStaticFields;

		private FieldInfo _keyField;

		private FieldInfo _jsonContextField;

		[ThreadStatic]
		private static Dictionary<Type, JsonClassInfo> _cachedClassInfo = new Dictionary<Type, JsonClassInfo>();

		public bool TypeHasKey
		{
			get
			{
				return _keyField != null;
			}
		}

		private JsonClassInfo(Type reflectedType)
		{
			type = reflectedType;
			CachedSerializedFields = GetJsonFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			CachedSerializedStaticFields = GetJsonFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public string GetKey(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			Type type = obj.GetType();
			if (type != this.type && !type.IsSubclassOf(this.type))
			{
				throw new JsonReflectionException(string.Format("Object must be descendant of the type associated with the JsonClassInfo instance. ({0})", this.type));
			}
			object obj2 = ((_keyField == null) ? null : _keyField.GetValue(obj));
			return (obj2 == null) ? null : obj2.ToString();
		}

		public object Construct(JsonContext context)
		{
			if (ReflectionWrapper.IsAbstract(type))
			{
				return null;
			}
			ConstructorInfo constructor = ReflectionWrapper.GetConstructor(type);
			object obj = ((constructor == null) ? ReflectionWrapper.GetUninitializedObject(type) : constructor.Invoke(null));
			if (_jsonContextField != null)
			{
				_jsonContextField.SetValue(obj, context);
			}
			return obj;
		}

		private JsonFieldInfo[] GetJsonFields(BindingFlags fieldFlags)
		{
			FieldInfo[] fieldsForTree = ReflectionWrapper.GetFieldsForTree(type, fieldFlags);
			List<JsonFieldInfo> list = new List<JsonFieldInfo>(fieldsForTree.Length);
			FieldInfo[] array = fieldsForTree;
			foreach (FieldInfo fieldInfo in array)
			{
				if (fieldInfo.FieldType == typeof(JsonContext))
				{
					_jsonContextField = fieldInfo;
				}
				bool serializeAsKey;
				bool isKeyField;
				bool enumAsString;
				string serializedName;
				int sizeLimit;
				bool failOnSizeLimit;
				if (ShouldSerializeField(fieldInfo, out serializeAsKey, out isKeyField, out enumAsString, out serializedName, out sizeLimit, out failOnSizeLimit))
				{
					JsonFieldInfo jsonFieldInfo = new JsonFieldInfo();
					jsonFieldInfo.Field = fieldInfo;
					jsonFieldInfo.ValueType = LookupValueType(fieldInfo);
					jsonFieldInfo.SerializeByKey = serializeAsKey;
					jsonFieldInfo.EnumAsString = enumAsString;
					jsonFieldInfo.SerializedName = serializedName;
					jsonFieldInfo.SizeLimit = sizeLimit;
					jsonFieldInfo.FailOnSizeLimit = failOnSizeLimit;
					if (isKeyField)
					{
						_keyField = fieldInfo;
					}
					jsonFieldInfo.SerializeByKey = serializeAsKey;
					list.Add(jsonFieldInfo);
				}
			}
			return list.ToArray();
		}

		public static void DiscardClassInfoCache()
		{
			_cachedClassInfo.Clear();
		}

		public static bool IsClassInfoCached(Type reflectedType)
		{
			return _cachedClassInfo.ContainsKey(reflectedType);
		}

		public static JsonClassInfo GetJsonTypeReflection(Type reflectedType)
		{
			JsonClassInfo value;
			if (!_cachedClassInfo.TryGetValue(reflectedType, out value))
			{
				value = GetNonCachedTypeReflection(reflectedType);
				_cachedClassInfo.Add(reflectedType, value);
			}
			return value;
		}

		public static JsonClassInfo GetNonCachedTypeReflection(Type reflectedType)
		{
			if (reflectedType == null)
			{
				return null;
			}
			return new JsonClassInfo(reflectedType);
		}

		public static string GetSerializedName(FieldInfo field)
		{
			if (field == null)
			{
				return null;
			}
			SerializedAsAttribute serializedAsAttribute = ReflectionWrapper.GetCustomAttribute(field, typeof(SerializedAsAttribute)) as SerializedAsAttribute;
			if (serializedAsAttribute != null)
			{
				return serializedAsAttribute.SerializedAsName;
			}
			return field.Name;
		}

		private static Type LookupValueType(FieldInfo field)
		{
			Type fieldType = field.FieldType;
			Type type;
			if ((type = ReflectionWrapper.GetInterface(fieldType, typeof(IList<>))) != null)
			{
				return ReflectionWrapper.GetGenericArguments(type)[0];
			}
			if ((type = ReflectionWrapper.GetInterface(fieldType, typeof(IDictionary<, >))) != null)
			{
				return ReflectionWrapper.GetGenericArguments(type)[1];
			}
			return fieldType;
		}

		private static bool ShouldSerializeField(FieldInfo field, out bool serializeAsKey, out bool isKeyField, out bool enumAsString, out string serializedName, out int sizeLimit, out bool failOnSizeLimit)
		{
			bool result = field.IsPublic;
			serializeAsKey = false;
			isKeyField = false;
			enumAsString = false;
			serializedName = GetSerializedName(field);
			sizeLimit = -1;
			failOnSizeLimit = false;
			foreach (object customAttribute in ReflectionWrapper.GetCustomAttributes(field, true))
			{
				if (customAttribute is SerializedSizeLimitAttribute)
				{
					sizeLimit = (customAttribute as SerializedSizeLimitAttribute).Size;
					failOnSizeLimit = (customAttribute as SerializedSizeLimitAttribute).FailOnLimit;
				}
				if (customAttribute is EnumAsStringAttribute)
				{
					enumAsString = true;
				}
				if (customAttribute is SerializeField)
				{
					result = true;
				}
				if (customAttribute is NonSerializedAttribute)
				{
					result = false;
				}
				if (customAttribute is JsonKeyAttribute)
				{
					isKeyField = true;
				}
				if (customAttribute is JsonSerializeKeyOnlyAttribute)
				{
					serializeAsKey = true;
				}
			}
			return result;
		}
	}
}
