using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Kiloo.Common.Serialization.Parsing
{
	public static class KilooJson
	{
		private sealed class Parser
		{
			private enum TOKEN
			{
				NONE = 0,
				CURLY_OPEN = 1,
				CURLY_CLOSE = 2,
				SQUARED_OPEN = 3,
				SQUARED_CLOSE = 4,
				COLON = 5,
				COMMA = 6,
				STRING = 7,
				NUMBER = 8,
				TRUE = 9,
				FALSE = 10,
				NULL = 11
			}

			private const string WHITE_SPACE = " \t\n\r";

			private const string WORD_BREAK = " \t\n\r{}[],:\"";

			private readonly TextReader _json;

			private readonly bool _ignoreCase;

			private char PeekChar
			{
				get
				{
					return Convert.ToChar(_json.Peek());
				}
			}

			private char NextChar
			{
				get
				{
					return Convert.ToChar(_json.Read());
				}
			}

			private string NextWord
			{
				get
				{
					StringBuilder stringBuilder = new StringBuilder();
					while (" \t\n\r{}[],:\"".IndexOf(PeekChar) == -1)
					{
						stringBuilder.Append(NextChar);
						if (_json.Peek() == -1)
						{
							break;
						}
					}
					return stringBuilder.ToString();
				}
			}

			private TOKEN NextToken
			{
				get
				{
					if (_json.Peek() == -1)
					{
						return TOKEN.NONE;
					}
					EatWhitespace();
					switch (PeekChar)
					{
					case '{':
						return TOKEN.CURLY_OPEN;
					case '}':
						_json.Read();
						return TOKEN.CURLY_CLOSE;
					case '[':
						return TOKEN.SQUARED_OPEN;
					case ']':
						_json.Read();
						return TOKEN.SQUARED_CLOSE;
					case ',':
						_json.Read();
						return TOKEN.COMMA;
					case '"':
						return TOKEN.STRING;
					case ':':
						return TOKEN.COLON;
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
						return TOKEN.NUMBER;
					default:
						switch (NextWord)
						{
						case "false":
							return TOKEN.FALSE;
						case "true":
							return TOKEN.TRUE;
						case "null":
							return TOKEN.NULL;
						default:
							return TOKEN.NONE;
						}
					}
				}
			}

			private Parser(TextReader reader, bool ignoreCase)
			{
				_json = reader;
				_ignoreCase = ignoreCase;
			}

			public static object Parse(TextReader reader, bool ignoreCase)
			{
				Parser parser = new Parser(reader, ignoreCase);
				return parser.ParseValue();
			}

			private Dictionary<string, object> ParseObject()
			{
				Dictionary<string, object> dictionary = ((!_ignoreCase) ? new Dictionary<string, object>() : new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));
				_json.Read();
				while (true)
				{
					switch (NextToken)
					{
					case TOKEN.COMMA:
						continue;
					case TOKEN.NONE:
						return null;
					case TOKEN.CURLY_CLOSE:
						return dictionary;
					}
					string text = ParseString();
					if (text == null)
					{
						return null;
					}
					if (NextToken != TOKEN.COLON)
					{
						return null;
					}
					_json.Read();
					dictionary[text] = ParseValue();
				}
			}

			private List<object> ParseArray()
			{
				List<object> list = new List<object>();
				_json.Read();
				bool flag = true;
				while (flag)
				{
					TOKEN nextToken = NextToken;
					switch (nextToken)
					{
					case TOKEN.NONE:
						return null;
					case TOKEN.SQUARED_CLOSE:
						flag = false;
						break;
					default:
					{
						object item = ParseByToken(nextToken);
						list.Add(item);
						break;
					}
					case TOKEN.COMMA:
						break;
					}
				}
				return list;
			}

			private object ParseValue()
			{
				TOKEN nextToken = NextToken;
				return ParseByToken(nextToken);
			}

			private object ParseByToken(TOKEN token)
			{
				switch (token)
				{
				case TOKEN.STRING:
					return ParseString();
				case TOKEN.NUMBER:
					return ParseNumber();
				case TOKEN.CURLY_OPEN:
					return ParseObject();
				case TOKEN.SQUARED_OPEN:
					return ParseArray();
				case TOKEN.TRUE:
					return true;
				case TOKEN.FALSE:
					return false;
				case TOKEN.NULL:
					return null;
				default:
					return null;
				}
			}

			private string ParseString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				_json.Read();
				bool flag = true;
				while (flag)
				{
					if (_json.Peek() == -1)
					{
						flag = false;
						break;
					}
					char nextChar = NextChar;
					switch (nextChar)
					{
					case '"':
						flag = false;
						break;
					case '\\':
						if (_json.Peek() == -1)
						{
							flag = false;
							break;
						}
						nextChar = NextChar;
						switch (nextChar)
						{
						case '"':
						case '/':
						case '\\':
							stringBuilder.Append(nextChar);
							break;
						case 'b':
							stringBuilder.Append('\b');
							break;
						case 'f':
							stringBuilder.Append('\f');
							break;
						case 'n':
							stringBuilder.Append('\n');
							break;
						case 'r':
							stringBuilder.Append('\r');
							break;
						case 't':
							stringBuilder.Append('\t');
							break;
						case 'u':
						{
							StringBuilder stringBuilder2 = new StringBuilder();
							for (int i = 0; i < 4; i++)
							{
								stringBuilder2.Append(NextChar);
							}
							stringBuilder.Append((char)Convert.ToInt32(stringBuilder2.ToString(), 16));
							break;
						}
						}
						break;
					default:
						stringBuilder.Append(nextChar);
						break;
					}
				}
				return stringBuilder.ToString();
			}

			private object ParseNumber()
			{
				string nextWord = NextWord;
				if (nextWord.IndexOf('.') == -1)
				{
					long result;
					long.TryParse(nextWord, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result);
					return result;
				}
				double result2;
				double.TryParse(nextWord, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out result2);
				return result2;
			}

			private void EatWhitespace()
			{
				while (" \t\n\r".IndexOf(PeekChar) != -1)
				{
					_json.Read();
					if (_json.Peek() == -1)
					{
						break;
					}
				}
			}
		}

		private sealed class Serializer
		{
			private readonly TextWriter _writer;

			private readonly bool _indent;

			private readonly KilooSerializerMapping _mapping;

			private int currentIndentation;

			private const char INDENT_CHAR = ' ';

			private const string NEWLINE = "\n";

			private const int INDENT_STEP = 2;

			private Serializer(TextWriter writer, bool indent, KilooSerializerMapping mapping)
			{
				_writer = writer;
				_indent = indent;
				_mapping = mapping;
			}

			public static void Serialize(object obj, TextWriter writer, bool indent, KilooSerializerMapping mapping)
			{
				Serializer serializer = new Serializer(writer, indent, mapping);
				serializer.SerializeValue(obj);
			}

			private void SerializeValue(object value)
			{
				if (value == null)
				{
					_writer.Write("null");
					return;
				}
				if (value is byte[])
				{
					string str = ConversionHelper.ValueAsString(value, CultureInfo.InvariantCulture);
					SerializeString(str);
					return;
				}
				if (value is bool)
				{
					_writer.Write((!(bool)value) ? "false" : "true");
					return;
				}
				string text = value as string;
				if (text != null)
				{
					SerializeString(text);
					return;
				}
				if (value is char)
				{
					SerializeString(((char)value/*cast due to constrained. prefix*/).ToString());
					return;
				}
				if (value is float || value is int || value is uint || value is long || value is double || value is sbyte || value is byte || value is short || value is ushort || value is ulong || value is decimal)
				{
					text = ((IConvertible)value).ToString(CultureInfo.InvariantCulture);
					_writer.Write(text);
					return;
				}
				text = ConversionHelper.ValueAsString(value, CultureInfo.InvariantCulture);
				if (text != null)
				{
					SerializeString(text);
					return;
				}
				if (value is IDictionary)
				{
					SerializeObject(ConversionHelper.ValueAsKeyValuePairs(value, _mapping));
					return;
				}
				IEnumerable enumerable = value as IEnumerable;
				if (enumerable != null)
				{
					SerializeArray(enumerable);
				}
				else
				{
					SerializeObject(ConversionHelper.ValueAsKeyValuePairs(value, _mapping));
				}
			}

			private void SerializeObject(IEnumerable<KeyValuePair<string, object>> properties)
			{
				bool flag = false;
				_writer.Write('{');
				currentIndentation += 2;
				foreach (KeyValuePair<string, object> property in properties)
				{
					if (flag)
					{
						_writer.Write(',');
					}
					if (_indent)
					{
						_writer.Write("\n");
						for (int i = 0; i < currentIndentation; i++)
						{
							_writer.Write(' ');
						}
					}
					string str = ConversionHelper.ValueAsString(property.Key, CultureInfo.InvariantCulture) ?? property.Key.ToString();
					SerializeString(str);
					_writer.Write(':');
					if (_indent)
					{
						_writer.Write(' ');
					}
					SerializeValue(property.Value);
					flag = true;
				}
				currentIndentation -= 2;
				if (flag && _indent)
				{
					_writer.Write("\n");
					for (int j = 0; j < currentIndentation; j++)
					{
						_writer.Write(' ');
					}
				}
				_writer.Write('}');
			}

			private void SerializeArray(IEnumerable anArray)
			{
				_writer.Write('[');
				currentIndentation += 2;
				bool flag = true;
				foreach (object item in anArray)
				{
					if (!flag)
					{
						_writer.Write(',');
					}
					if (_indent)
					{
						_writer.Write("\n");
						for (int i = 0; i < currentIndentation; i++)
						{
							_writer.Write(' ');
						}
					}
					SerializeValue(item);
					flag = false;
				}
				currentIndentation -= 2;
				if (_indent && !flag)
				{
					_writer.Write("\n");
					for (int j = 0; j < currentIndentation; j++)
					{
						_writer.Write(' ');
					}
				}
				_writer.Write(']');
			}

			private void SerializeString(string str)
			{
				_writer.Write('"');
				char[] array = str.ToCharArray();
				char[] array2 = array;
				foreach (char c in array2)
				{
					switch (c)
					{
					case '"':
						_writer.Write("\\\"");
						continue;
					case '\\':
						_writer.Write("\\\\");
						continue;
					case '\b':
						_writer.Write("\\b");
						continue;
					case '\f':
						_writer.Write("\\f");
						continue;
					case '\n':
						_writer.Write("\\n");
						continue;
					case '\r':
						_writer.Write("\\r");
						continue;
					case '\t':
						_writer.Write("\\t");
						continue;
					}
					int num = Convert.ToInt32(c);
					if (num >= 32 && num <= 126)
					{
						_writer.Write(c);
					}
					else
					{
						_writer.Write("\\u" + Convert.ToString(num, 16).PadLeft(4, '0'));
					}
				}
				_writer.Write('"');
			}
		}

		public static object Deserialize([NotNull] TextReader reader, bool ignoreCase = false)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			return Parser.Parse(reader, ignoreCase);
		}

		public static object Deserialize(string json, bool ignoreCase = false)
		{
			if (json == null)
			{
				return null;
			}
			using (StringReader reader = new StringReader(json))
			{
				return Parser.Parse(reader, ignoreCase);
			}
		}

		public static void Serialize(object obj, TextWriter writer, KilooSerializerMapping mapping, bool indent = false)
		{
			Serializer.Serialize(obj, writer, indent, mapping);
		}

		public static string Serialize(object obj, KilooSerializerMapping mapping, bool indent = false)
		{
			using (StringWriter stringWriter = new StringWriter())
			{
				Serializer.Serialize(obj, stringWriter, indent, mapping);
				return stringWriter.ToString();
			}
		}
	}
}
