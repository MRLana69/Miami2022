using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using SYBO.Components.Utilities;

namespace SYBO.Components.Json.Internal
{
	public class JsonParser
	{
		protected enum Token
		{
			None = 0,
			CurlyOpen = 1,
			CurlyClose = 2,
			SquaredOpen = 3,
			SquaredClose = 4,
			Colon = 5,
			Comma = 6,
			String = 7,
			Number = 8,
			True = 9,
			False = 10,
			Null = 11
		}

		public static int BuilderCapacity = 2000;

		protected static int _lastErrorIndex = -1;

		protected static string _lastDecode = string.Empty;

		private static int _indentation;

		private static List<string> _indentations;

		private static StringBuilder _stringParsingBuffer;

		private static HashSet<char> _possibleNumberChars = new HashSet<char>
		{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
			'+', '-', '.', 'a', 'A', 'b', 'B', 'c', 'C', 'd',
			'D', 'e', 'E', 'f', 'F', 'x', 'X'
		};

		public static T Deserialize<T>(string json, JsonContext context = null)
		{
			object obj = DeserializeDom(json);
			if (obj == null)
			{
				return default(T);
			}
			if (obj is T)
			{
				return (T)obj;
			}
			return JsonReflectionParser.Deserialize<T>(obj, context);
		}

		public static string Serialize<T>(T obj, JsonContext context = null)
		{
			object json = JsonReflectionParser.Serialize(obj, context);
			if (context == null)
			{
				context = new JsonContext();
			}
			return SerializeDom(json, context);
		}

		public static object DeserializeDom(string json)
		{
			_lastDecode = json;
			if (json != null)
			{
				int index = 0;
				bool success = true;
				object result = ParseValue(json, ref index, ref success);
				if (success)
				{
					_lastErrorIndex = -1;
				}
				else
				{
					_lastErrorIndex = index;
				}
				return result;
			}
			return null;
		}

		public static string SerializeDom(object json, JsonContext context)
		{
			_indentation = 0;
			StringBuilder stringBuilder = new StringBuilder(BuilderCapacity);
			return (!SerializeValue(json, stringBuilder, context)) ? null : stringBuilder.ToString();
		}

		public static bool LastDecodeSuccessful()
		{
			return _lastErrorIndex == -1;
		}

		public static int GetLastErrorIndex()
		{
			return _lastErrorIndex;
		}

		public static string GetLastErrorSnippet()
		{
			if (_lastErrorIndex == -1)
			{
				return string.Empty;
			}
			int num = _lastErrorIndex - 5;
			int num2 = _lastErrorIndex + 15;
			if (num < 0)
			{
				num = 0;
			}
			if (num2 >= _lastDecode.Length)
			{
				num2 = _lastDecode.Length - 1;
			}
			return _lastDecode.Substring(num, num2 - num + 1);
		}

		protected static JsonDict ParseObject(string json, ref int index)
		{
			JsonDict jsonDict = new JsonDict();
			NextToken(json, ref index);
			bool flag = false;
			while (!flag)
			{
				switch (LookAhead(json, index))
				{
				case Token.None:
					return null;
				case Token.Comma:
					NextToken(json, ref index);
					continue;
				case Token.CurlyClose:
					NextToken(json, ref index);
					return jsonDict;
				}
				string text = ParseString(json, ref index);
				if (text == null)
				{
					return null;
				}
				Token token = NextToken(json, ref index);
				if (token != Token.Colon)
				{
					return null;
				}
				bool success = true;
				object value = ParseValue(json, ref index, ref success);
				if (!success)
				{
					return null;
				}
				jsonDict[text] = value;
			}
			return jsonDict;
		}

		protected static JsonList ParseArray(string json, ref int index)
		{
			JsonList jsonList = new JsonList();
			NextToken(json, ref index);
			bool flag = false;
			while (!flag)
			{
				switch (LookAhead(json, index))
				{
				case Token.None:
					return null;
				case Token.Comma:
					NextToken(json, ref index);
					continue;
				case Token.SquaredClose:
					break;
				default:
				{
					bool success = true;
					object item = ParseValue(json, ref index, ref success);
					if (!success)
					{
						return null;
					}
					jsonList.Add(item);
					continue;
				}
				}
				NextToken(json, ref index);
				break;
			}
			return jsonList;
		}

		protected static object ParseValue(string json, ref int index, ref bool success)
		{
			switch (LookAhead(json, index))
			{
			case Token.String:
				return ParseString(json, ref index);
			case Token.Number:
				return ParseNumber(json, ref index);
			case Token.CurlyOpen:
				return ParseObject(json, ref index);
			case Token.SquaredOpen:
				return ParseArray(json, ref index);
			case Token.True:
				NextToken(json, ref index);
				return bool.Parse("TRUE");
			case Token.False:
				NextToken(json, ref index);
				return bool.Parse("FALSE");
			case Token.Null:
				NextToken(json, ref index);
				return null;
			default:
				success = false;
				return null;
			}
		}

		protected static string ParseString(string json, ref int index)
		{
			EatWhitespace(json, ref index);
			if (_stringParsingBuffer == null)
			{
				_stringParsingBuffer = new StringBuilder(1024);
			}
			_stringParsingBuffer.Length = 0;
			char c = json[index++];
			bool flag = false;
			while (!flag && index != json.Length)
			{
				c = json[index++];
				switch (c)
				{
				case '"':
					flag = true;
					break;
				case '\\':
					if (index != json.Length)
					{
						switch (json[index++])
						{
						case '"':
							_stringParsingBuffer.Append('"');
							continue;
						case '\\':
							_stringParsingBuffer.Append('\\');
							continue;
						case '/':
							_stringParsingBuffer.Append('/');
							continue;
						case 'b':
							_stringParsingBuffer.Append('\b');
							continue;
						case 'f':
							_stringParsingBuffer.Append('\f');
							continue;
						case 'n':
							_stringParsingBuffer.Append('\n');
							continue;
						case 'r':
							_stringParsingBuffer.Append('\r');
							continue;
						case 't':
							_stringParsingBuffer.Append('\t');
							continue;
						case 'u':
							break;
						default:
							continue;
						}
						int num = json.Length - index;
						if (num >= 4)
						{
							uint utf = uint.Parse(json.Substring(index, 4), NumberStyles.HexNumber);
							_stringParsingBuffer.Append(char.ConvertFromUtf32((int)utf));
							index += 4;
							continue;
						}
					}
					break;
				default:
					_stringParsingBuffer.Append(c);
					continue;
				}
				break;
			}
			if (!flag)
			{
				return null;
			}
			return _stringParsingBuffer.ToString();
		}

		protected static object ParseNumber(string json, ref int index)
		{
			EatWhitespace(json, ref index);
			int lastIndexOfNumber = GetLastIndexOfNumber(json, index);
			int num = lastIndexOfNumber - index + 1;
			if (num > 2 && json[index] == '0' && (json[index + 1] == 'x' || json[index + 1] == 'X'))
			{
				string hexString = json.Substring(index + 2, num - 2);
				index = lastIndexOfNumber + 1;
				return ParseHex(hexString);
			}
			string text = json.Substring(index, num);
			index = lastIndexOfNumber + 1;
			if (text.Contains("."))
			{
				return double.Parse(text, CultureInfo.InvariantCulture);
			}
			return long.Parse(text);
		}

		private static object ParseHex(string hexString)
		{
			if (hexString.Length > 8)
			{
				return long.Parse(hexString, NumberStyles.HexNumber);
			}
			return int.Parse(hexString, NumberStyles.HexNumber);
		}

		protected static int GetLastIndexOfNumber(string json, int index)
		{
			int i;
			for (i = index; i < json.Length && _possibleNumberChars.Contains(json[i]); i++)
			{
			}
			return i - 1;
		}

		protected static void EatWhitespace(string json, ref int index)
		{
			while (index < json.Length && " \t\n\r".IndexOf(json[index]) != -1)
			{
				index++;
			}
		}

		protected static Token LookAhead(string json, int index)
		{
			int index2 = index;
			return NextToken(json, ref index2);
		}

		protected static Token NextToken(string json, ref int index)
		{
			EatWhitespace(json, ref index);
			if (index == json.Length)
			{
				return Token.None;
			}
			char c = json[index];
			index++;
			switch (c)
			{
			case '{':
				return Token.CurlyOpen;
			case '}':
				return Token.CurlyClose;
			case '[':
				return Token.SquaredOpen;
			case ']':
				return Token.SquaredClose;
			case ',':
				return Token.Comma;
			case '"':
				return Token.String;
			case '-':
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				return Token.Number;
			case ':':
				return Token.Colon;
			default:
			{
				index--;
				int num = json.Length - index;
				if (num >= 5 && json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e')
				{
					index += 5;
					return Token.False;
				}
				if (num >= 4 && json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e')
				{
					index += 4;
					return Token.True;
				}
				if (num >= 4 && json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l')
				{
					index += 4;
					return Token.Null;
				}
				return Token.None;
			}
			}
		}

		protected static bool SerializeObject(IDictionary dict, StringBuilder builder, JsonContext context)
		{
			builder.Append("{");
			_indentation++;
			IDictionaryEnumerator dictionaryEnumerator = SetupDictionaryEnumerator(dict);
			bool flag = true;
			while (dictionaryEnumerator.MoveNext())
			{
				string aString = dictionaryEnumerator.Key.ToString();
				object value = dictionaryEnumerator.Value;
				if (!flag)
				{
					builder.Append(",");
				}
				AppendIndentation(builder, context);
				SerializeString(aString, builder);
				builder.Append(":");
				if (!SerializeValue(value, builder, context))
				{
					_indentation--;
					return false;
				}
				flag = false;
			}
			_indentation--;
			AppendIndentation(builder, context);
			builder.Append("}");
			return true;
		}

		private static IDictionaryEnumerator SetupDictionaryEnumerator(IDictionary dict)
		{
			return dict.GetEnumerator();
		}

		protected static string GetIndentationString()
		{
			if (_indentations == null)
			{
				_indentations = new List<string>();
				_indentations.Add("\n");
			}
			while (_indentation >= _indentations.Count)
			{
				_indentations.Add(_indentations[_indentations.Count - 1] + "\t");
			}
			return _indentations[_indentation];
		}

		protected static bool SerializeArray(Array anArray, StringBuilder builder, JsonContext context)
		{
			builder.Append("[");
			if (anArray.Length > 0)
			{
				_indentation++;
				bool flag = true;
				for (int i = 0; i < anArray.Length; i++)
				{
					object value = anArray.GetValue(i);
					if (!flag)
					{
						builder.Append(",");
					}
					AppendIndentation(builder, context);
					if (!SerializeValue(value, builder, context))
					{
						return false;
					}
					flag = false;
				}
				_indentation--;
				AppendIndentation(builder, context);
			}
			builder.Append("]");
			return true;
		}

		protected static bool SerializeArray(IList anArray, StringBuilder builder, JsonContext context)
		{
			builder.Append("[");
			if (anArray.Count > 0)
			{
				_indentation++;
				bool flag = true;
				for (int i = 0; i < anArray.Count; i++)
				{
					object value = anArray[i];
					if (!flag)
					{
						builder.Append(",");
					}
					AppendIndentation(builder, context);
					if (!SerializeValue(value, builder, context))
					{
						return false;
					}
					flag = false;
				}
				_indentation--;
				AppendIndentation(builder, context);
			}
			builder.Append("]");
			return true;
		}

		protected static bool SerializeValue(object value, StringBuilder builder, JsonContext context)
		{
			Type type = ((value != null) ? value.GetType() : null);
			if (value == null)
			{
				builder.Append("null");
			}
			else if (type.IsArray)
			{
				SerializeArray((Array)value, builder, context);
			}
			else if (value is string)
			{
				SerializeString((string)value, builder);
			}
			else if (value is char)
			{
				SerializeString(Convert.ToString((char)value), builder);
			}
			else if (value is decimal)
			{
				SerializeString(Convert.ToString((decimal)value), builder);
			}
			else if (value is IDictionary)
			{
				SerializeObject((IDictionary)value, builder, context);
			}
			else if (value is IList)
			{
				SerializeArray((IList)value, builder, context);
			}
			else if (value is bool && (bool)value)
			{
				builder.Append("true");
			}
			else if (value is bool && !(bool)value)
			{
				builder.Append("false");
			}
			else if (type == typeof(sbyte) || type == typeof(byte) || type == typeof(short) || type == typeof(int) || type == typeof(long))
			{
				builder.Append(value);
			}
			else
			{
				if (!ReflectionWrapper.IsPrimitive(type))
				{
					return false;
				}
				SerializeNumber(Convert.ToDouble(value), builder);
			}
			return true;
		}

		protected static void SerializeString(string aString, StringBuilder builder)
		{
			builder.Append("\"");
			foreach (char c in aString)
			{
				switch (c)
				{
				case '"':
					builder.Append("\\\"");
					continue;
				case '\\':
					builder.Append("\\\\");
					continue;
				case '\b':
					builder.Append("\\b");
					continue;
				case '\f':
					builder.Append("\\f");
					continue;
				case '\n':
					builder.Append("\\n");
					continue;
				case '\r':
					builder.Append("\\r");
					continue;
				case '\t':
					builder.Append("\\t");
					continue;
				}
				int num = Convert.ToInt32(c);
				if (num >= 32 && num <= 126)
				{
					builder.Append(c);
				}
				else
				{
					builder.Append("\\u" + Convert.ToString(num, 16).PadLeft(4, '0'));
				}
			}
			builder.Append("\"");
		}

		protected static void SerializeNumber(double number, StringBuilder builder)
		{
			if (double.IsInfinity(number))
			{
				throw new JsonSerializationException("Cannot serialize infinity to JSON.");
			}
			if (double.IsNaN(number))
			{
				throw new JsonSerializationException("Cannot serialize NaN to JSON.");
			}
			builder.Append(number.ToString("R", CultureInfo.InvariantCulture));
		}

		protected static void AppendIndentation(StringBuilder builder, JsonContext context)
		{
			if (!context.RemoveWhitespace)
			{
				builder.Append(GetIndentationString());
			}
		}
	}
}
