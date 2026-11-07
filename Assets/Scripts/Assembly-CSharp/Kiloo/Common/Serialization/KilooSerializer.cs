using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Kiloo.Common.Serialization.Parsing;
using Kiloo.Common.Utility;

namespace Kiloo.Common.Serialization
{
	public class KilooSerializer : IJsonSerializer, IBsonSerializer, ISerializer
	{
		public KilooSerializerFormat serializerFormat;

		public bool indent = true;

		public bool ignoreCase = true;

		public bool ignoreErrors;

		public KilooSerializerMapping mapping;

		public IFormatProvider formatProvider = CultureInfo.InvariantCulture;

		private static readonly Regex _timezoneRegex = new Regex("[+-](?:0?\\d|1[0-4])(:[0-5]\\d)?$");

		private const string VALID_CHARACTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";

		public void WriteJsonValue(object value, TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			KilooJson.Serialize(value, writer, mapping, indent);
		}

		public object ReadJsonValue(Type type, TextReader reader)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			object value = KilooJson.Deserialize(reader, ignoreCase);
			return ConvertValue(type, value, null);
		}

		public void WriteBsonObject(object obj, BinaryWriter writer)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteBsonObject(obj, mapping);
		}

		public object ReadBsonObject(Type type, BinaryReader reader)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			object value = reader.ReadBsonObject(ignoreCase);
			return ConvertValue(type, value, null);
		}

		public void WriteObject(object value, Stream outputStream)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (serializerFormat == KilooSerializerFormat.Bson)
			{
				BinaryWriter binaryWriter = new BinaryWriter(outputStream);
				WriteBsonObject(value, binaryWriter);
				binaryWriter.Flush();
				return;
			}
			if (serializerFormat == KilooSerializerFormat.Json)
			{
				StreamWriter streamWriter = new StreamWriter(outputStream);
				WriteJsonValue(value, streamWriter);
				streamWriter.Flush();
				return;
			}
			throw new NotSupportedException("Serializer format not supported: " + serializerFormat);
		}

		public object ReadObject(Type type, Stream inputStream, bool allowBuffering)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			if (inputStream.CanSeek)
			{
				if (CouldBeBson(inputStream))
				{
					return ReadBsonObject(type, new BinaryReader(inputStream));
				}
				return ReadJsonValue(type, (!allowBuffering) ? ((TextReader)new UnbufferedStreamReader(inputStream)) : ((TextReader)new StreamReader(inputStream)));
			}
			if (serializerFormat == KilooSerializerFormat.Bson)
			{
				return ReadBsonObject(type, new BinaryReader(inputStream));
			}
			if (serializerFormat == KilooSerializerFormat.Json)
			{
				return ReadJsonValue(type, (!allowBuffering) ? ((TextReader)new UnbufferedStreamReader(inputStream)) : ((TextReader)new StreamReader(inputStream)));
			}
			throw new NotSupportedException("Serializer format not supported: " + serializerFormat);
		}

		[Pure]
		public object ConvertValue(Type type, object value)
		{
			return ConvertValue(type, value, null);
		}

		[Pure]
		private object ConvertValue([NotNull] Type type, [CanBeNull] object value, [CanBeNull] string propertyName)
		{
			Exception innerException = null;
			try
			{
				if (value == null && (!type.IsValueType || Nullable.GetUnderlyingType(type) != null))
				{
					return null;
				}
				if (type.IsInstanceOfType(value))
				{
					return value;
				}
				if (type.IsAssignableFrom(typeof(string)))
				{
					if (value is IConvertible)
					{
						return Convert.ChangeType(value, typeof(string), formatProvider);
					}
					IFormattable formattable = value as IFormattable;
					if (formattable != null)
					{
						return formattable.ToString(null, formatProvider);
					}
				}
				string text = value as string;
				if (text != null)
				{
					if (type.IsAssignableFrom(typeof(Guid)))
					{
						return new Guid(text);
					}
					if (type.IsAssignableFrom(typeof(DateTimeOffset)))
					{
						return ParseDateTimeOffset(text);
					}
					if (type.IsAssignableFrom(typeof(DateTime)))
					{
						return DateTime.Parse(text, formatProvider, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
					}
					if (type.IsAssignableFrom(typeof(byte[])))
					{
						return Convert.FromBase64String(text);
					}
				}
				byte[] array = value as byte[];
				if (array != null && array.Length == 16 && type.IsAssignableFrom(typeof(Guid)))
				{
					return new Guid(array);
				}
				if (type.IsEnum)
				{
					return ConvertToEnum(type, value);
				}
				if (type.IsArray)
				{
					IList list = value as IList;
					if (list != null)
					{
						return ConvertToArray(type, list);
					}
				}
				if (typeof(IEnumerable).IsAssignableFrom(type))
				{
					IEnumerable enumerable = value as IEnumerable;
					if (enumerable != null)
					{
						return ConvertToCollection(type, enumerable);
					}
				}
				if (!type.IsPrimitive)
				{
					IDictionary dictionary = value as IDictionary;
					if (dictionary != null)
					{
						return ConvertToObject(type, dictionary);
					}
				}
				if (value is DateTime && type.IsAssignableFrom(typeof(DateTimeOffset)))
				{
					return new DateTimeOffset((DateTime)value);
				}
				if (value is IConvertible)
				{
					return Convert.ChangeType(value, type, formatProvider);
				}
			}
			catch (Exception ex)
			{
				innerException = ex;
			}
			if (ignoreErrors)
			{
				return GetDefault(type);
			}
			string text2 = ((propertyName == null) ? string.Empty : ("Error assigning " + propertyName + ". "));
			throw new InvalidCastException(text2 + "Could not convert " + GetDisplayValue(value) + " to " + type, innerException);
		}

		private DateTimeOffset ParseDateTimeOffset(string stringValue)
		{
			string text = stringValue.Trim();
			TimeSpan offset;
			if (text.EndsWith("GMT", StringComparison.OrdinalIgnoreCase))
			{
				text = text.Substring(0, text.Length - 3);
				offset = TimeSpan.Zero;
			}
			else if (text.EndsWith("Z", StringComparison.OrdinalIgnoreCase))
			{
				text = text.Substring(0, text.Length - 1);
				offset = TimeSpan.Zero;
			}
			else
			{
				Match match = _timezoneRegex.Match(text);
				if (match.Success)
				{
					string value = match.Value;
					offset = TimeSpan.Parse((value[0] != '+') ? value : value.Substring(1));
					text = text.Substring(0, text.Length - value.Length);
				}
				else
				{
					offset = TimeSpan.Zero;
				}
			}
			return new DateTimeOffset(DateTime.Parse(text, formatProvider, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal).Ticks, offset);
		}

		private static string GetDisplayValue(object value)
		{
			if (value == null)
			{
				return "null";
			}
			string text = value as string;
			if (text != null)
			{
				return "\"" + text + "\"";
			}
			return value.ToString();
		}

		[Pure]
		private static object GetDefault(Type type)
		{
			if (type.IsValueType)
			{
				return Activator.CreateInstance(type);
			}
			return null;
		}

		[Pure]
		private object ConvertToEnum(Type type, object value)
		{
			string text = value as string;
			if (text != null)
			{
				return Enum.Parse(type, text, true);
			}
			object obj = Enum.ToObject(type, value);
			if (!Enum.IsDefined(type, obj))
			{
				throw new Exception("Enum value not defined");
			}
			return obj;
		}

		[Pure]
		private Array ConvertToArray(Type type, [NotNull] IList jsonList)
		{
			int count = jsonList.Count;
			Type elementType = type.GetElementType();
			Array array = Array.CreateInstance(elementType, count);
			for (int i = 0; i < count; i++)
			{
				array.SetValue(ConvertValue(elementType, jsonList[i], null), i);
			}
			return array;
		}

		[Pure]
		private object ConvertToCollection(Type type, [NotNull] IEnumerable enumerable)
		{
			if (mapping != null)
			{
				type = mapping.GetConcreteType(type);
			}
			if (type.IsAbstract)
			{
				throw new NotSupportedException("No suitable constructor for " + type.FullName);
			}
			IDictionary dictionary = enumerable as IDictionary;
			object obj;
			object[] array2;
			if (dictionary != null)
			{
				MethodInfo[] methods = type.GetMethods();
				MethodInfo[] array = methods;
				foreach (MethodInfo methodInfo in array)
				{
					if (!(methodInfo.Name == "Add"))
					{
						continue;
					}
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (parameters.Length != 2)
					{
						continue;
					}
					obj = Activator.CreateInstance(type);
					array2 = new object[2];
					{
						foreach (DictionaryEntry item in dictionary)
						{
							array2[0] = ConvertValue(parameters[0].ParameterType, item.Key, null);
							array2[1] = ConvertValue(parameters[1].ParameterType, item.Value, null);
							methodInfo.Invoke(obj, array2);
						}
						return obj;
					}
				}
			}
			Type enumerableElementType = GetEnumerableElementType(type);
			MethodInfo method = type.GetMethod("Add", new Type[1] { enumerableElementType });
			if (method == null)
			{
				throw new NotSupportedException("No suitable collection initializer found in " + type.FullName);
			}
			obj = Activator.CreateInstance(type);
			array2 = new object[1];
			foreach (object item2 in enumerable)
			{
				array2[0] = ConvertValue(enumerableElementType, item2, null);
				method.Invoke(obj, array2);
			}
			return obj;
		}

		[Pure]
		private Type GetEnumerableElementType(Type enumerableType)
		{
			Type[] interfaces = enumerableType.GetInterfaces();
			foreach (Type type in interfaces)
			{
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				{
					return type.GetGenericArguments()[0];
				}
			}
			return typeof(object);
		}

		[Pure]
		private object ConvertToObject(Type type, [NotNull] IDictionary properties)
		{
			if (mapping != null)
			{
				type = mapping.GetConcreteType(type);
			}
			if (type.IsAbstract)
			{
				throw new NotSupportedException("No suitable constructor for " + type.FullName);
			}
			properties = SanitizeKeys(properties);
			object obj = null;
			if (type.GetConstructor(Type.EmptyTypes) != null)
			{
				obj = Activator.CreateInstance(type);
			}
			else
			{
				ConstructorInfo[] constructors = type.GetConstructors();
				ConstructorInfo[] array = constructors;
				foreach (ConstructorInfo constructor in array)
				{
					obj = TryConstruct(constructor, properties);
					if (obj != null)
					{
						break;
					}
				}
				if (obj == null)
				{
					if (type.IsValueType)
					{
						obj = Activator.CreateInstance(type);
					}
					if (obj == null)
					{
						throw new NotSupportedException("No suitable constructor for " + type.FullName);
					}
				}
			}
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			FieldInfo[] array2 = fields;
			foreach (FieldInfo fieldInfo in array2)
			{
				object value;
				if ((fieldInfo.Attributes & FieldAttributes.NotSerialized) == 0 && TryGetValueForMember(type, fieldInfo.Name, properties, out value, false))
				{
					fieldInfo.SetValue(obj, ConvertValue(fieldInfo.FieldType, value, fieldInfo.Name));
				}
			}
			PropertyInfo[] properties2 = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] array3 = properties2;
			foreach (PropertyInfo propertyInfo in array3)
			{
				object value2;
				if (propertyInfo.CanWrite && TryGetValueForMember(type, propertyInfo.Name, properties, out value2, true))
				{
					propertyInfo.SetValue(obj, ConvertValue(propertyInfo.PropertyType, value2, propertyInfo.Name), null);
				}
			}
			return obj;
		}

		private bool TryGetValueForMember(Type containingType, string memberName, [NotNull] IDictionary properties, out object value, bool useMappingOfBaseTypes)
		{
			if (mapping != null)
			{
				return mapping.TryGetValueForMember(containingType, memberName, properties, out value, useMappingOfBaseTypes);
			}
			value = properties[memberName];
			return value != null || properties.Contains(memberName);
		}

		private bool IsValidKey(string key)
		{
			int length = key.Length;
			for (int i = 0; i < length; i++)
			{
				if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_".IndexOf(key[i]) < 0)
				{
					return false;
				}
			}
			return true;
		}

		private bool AreKeysValid(IDictionary dictionary)
		{
			foreach (string key in dictionary.Keys)
			{
				if (!IsValidKey(key))
				{
					return false;
				}
			}
			return true;
		}

		private IDictionary SanitizeKeys(IDictionary dictionary)
		{
			if (AreKeysValid(dictionary))
			{
				return dictionary;
			}
			StringBuilder stringBuilder = new StringBuilder();
			Dictionary<string, object> dictionary2 = ((!ignoreCase) ? new Dictionary<string, object>(dictionary.Count) : new Dictionary<string, object>(dictionary.Count, StringComparer.OrdinalIgnoreCase));
			foreach (DictionaryEntry item in dictionary)
			{
				string text = (string)item.Key;
				object value = (dictionary2[text] = item.Value);
				if (IsValidKey(text))
				{
					continue;
				}
				stringBuilder.Length = 0;
				int length = text.Length;
				for (int i = 0; i < length; i++)
				{
					char value3 = text[i];
					if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_".IndexOf(value3) >= 0)
					{
						stringBuilder.Append(value3);
					}
				}
				text = stringBuilder.ToString();
				dictionary2[text] = value;
			}
			return dictionary2;
		}

		[Pure]
		private object TryConstruct([NotNull] ConstructorInfo constructor, [NotNull] IDictionary properties)
		{
			try
			{
				ParameterInfo[] parameters = constructor.GetParameters();
				object[] array = new object[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
				{
					ParameterInfo parameterInfo = parameters[i];
					array[i] = ConvertValue(parameterInfo.ParameterType, properties[parameterInfo.Name], null);
				}
				return Activator.CreateInstance(constructor.DeclaringType, array);
			}
			catch (Exception)
			{
			}
			return null;
		}

		private bool CouldBeBson(Stream inputStream)
		{
			if (!inputStream.CanSeek)
			{
				throw new ArgumentException("Stream must be seekable", "inputStream");
			}
			long position = inputStream.Position;
			long num = inputStream.Length - position;
			if (num >= 5)
			{
				try
				{
					BinaryReader binaryReader = new BinaryReader(inputStream);
					int num2 = binaryReader.ReadInt32();
					if (num2 >= 5 && num2 <= num)
					{
						inputStream.Seek(num2 - 5, SeekOrigin.Current);
						byte b = binaryReader.ReadByte();
						return b == 0;
					}
				}
				catch (Exception)
				{
				}
				finally
				{
					inputStream.Seek(position, SeekOrigin.Begin);
				}
			}
			return false;
		}
	}
}
