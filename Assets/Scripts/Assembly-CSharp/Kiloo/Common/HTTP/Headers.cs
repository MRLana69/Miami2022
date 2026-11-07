using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kiloo.Common.HTTP
{
	public class Headers : IEnumerable<KeyValuePair<string, string>>, IEnumerable
	{
		[CompilerGenerated]
		private sealed class _003CGetEnumerator_003Ec__Iterator0 : IEnumerator, IDisposable, IEnumerator<KeyValuePair<string, string>>
		{
			internal Dictionary<string, List<string>>.Enumerator _0024locvar0;

			internal KeyValuePair<string, List<string>> _003Cheader_003E__1;

			internal Headers _0024this;

			internal KeyValuePair<string, string> _0024current;

			internal bool _0024disposing;

			internal int _0024PC;

			KeyValuePair<string, string> IEnumerator<KeyValuePair<string, string>>.Current
			{
				[DebuggerHidden]
				get
				{
					return System_002ECollections_002EGeneric_002EIEnumerator_003CSystem_002ECollections_002EGeneric_002EKeyValuePair_003Cstring_002Cstring_003E_003E_002Eget_Current();
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return _0024current;
				}
			}

			[DebuggerHidden]
			public _003CGetEnumerator_003Ec__Iterator0()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)_0024PC;
				_0024PC = -1;
				bool flag = false;
				switch (num)
				{
				case 0u:
					_0024locvar0 = _0024this._headers.GetEnumerator();
					num = 4294967293u;
					goto case 1u;
				case 1u:
					try
					{
						switch (num)
						{
						default:
							if (_0024locvar0.MoveNext())
							{
								_003Cheader_003E__1 = _0024locvar0.Current;
								_0024current = new KeyValuePair<string, string>(_003Cheader_003E__1.Key, _0024this.Get(_003Cheader_003E__1.Key));
								if (!_0024disposing)
								{
									_0024PC = 1;
								}
								flag = true;
								goto IL_00d4;
							}
							break;
						}
					}
					finally
					{
						if (!flag)
						{
							((IDisposable)_0024locvar0/*cast due to constrained. prefix*/).Dispose();
						}
					}
					_0024PC = -1;
					goto default;
				default:
					{
						return false;
					}
					IL_00d4:
					return true;
				}
			}

			[DebuggerHidden]
			private KeyValuePair<string, string> System_002ECollections_002EGeneric_002EIEnumerator_003CSystem_002ECollections_002EGeneric_002EKeyValuePair_003Cstring_002Cstring_003E_003E_002Eget_Current()
			{
				return _0024current;
			}

			[DebuggerHidden]
			public void Dispose()
			{
				uint num = (uint)_0024PC;
				_0024disposing = true;
				_0024PC = -1;
				switch (num)
				{
				case 1u:
					try
					{
						break;
					}
					finally
					{
						((IDisposable)_0024locvar0/*cast due to constrained. prefix*/).Dispose();
					}
				case 0u:
					break;
				}
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}
		}

		private readonly Dictionary<string, List<string>> _headers = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);

		private string _contentType;

		private ContentType _parsedContentType;

		private static readonly string[] _niceNameExceptions = new string[6] { "ETag", "P3P", "Content-MD5", "WWW-Authenticate", "TE", "KAC-TOTP" };

		public string this[string name]
		{
			get
			{
				return Get(name, string.Empty);
			}
			set
			{
				name = NicifyName(name);
				string[] array = value.Split(',');
				List<string> list = new List<string>();
				string[] array2 = array;
				foreach (string text in array2)
				{
					list.Add(text.Trim());
				}
				_headers[name] = list;
			}
		}

		public int Count
		{
			get
			{
				int num = 0;
				foreach (KeyValuePair<string, List<string>> header in _headers)
				{
					num += header.Value.Count;
				}
				return num;
			}
		}

		public ContentType contentType
		{
			get
			{
				string text = this["Content-Type"];
				if (_contentType != text)
				{
					string[] array = text.Split(';');
					string text2 = array[0].ToLowerInvariant();
					int num = text2.IndexOf('/');
					string type;
					string subType;
					if (num > 0)
					{
						type = text2.Substring(0, num).Trim();
						subType = text2.Substring(num + 1).Trim();
					}
					else
					{
						type = array[0].Trim().ToLower();
						subType = string.Empty;
					}
					Dictionary<string, string> dictionary = null;
					for (int i = 1; i < array.Length; i++)
					{
						if (dictionary == null)
						{
							dictionary = new Dictionary<string, string>(array.Length, StringComparer.InvariantCultureIgnoreCase);
						}
						num = array[i].IndexOf('=');
						if (num >= 0)
						{
							dictionary[array[i].Substring(0, num).Trim()] = array[i].Substring(num + 1).Trim();
						}
						else
						{
							dictionary[array[i].Trim()] = string.Empty;
						}
					}
					_contentType = text;
					_parsedContentType = new ContentType(type, subType, dictionary);
				}
				return _parsedContentType;
			}
		}

		public bool Contains(string name)
		{
			return _headers.ContainsKey(name);
		}

		public void Clear()
		{
			_headers.Clear();
		}

		public void Remove(string name)
		{
			_headers.Remove(name);
		}

		public Headers Add(KeyValuePair<string, string> header)
		{
			return Add(header.Key, header.Value);
		}

		public Headers Add(string name, object value)
		{
			List<string> value2;
			if (!_headers.TryGetValue(name, out value2))
			{
				name = NicifyName(name);
				value2 = new List<string>();
				_headers[name] = value2;
			}
			string text = value as string;
			if (text == null)
			{
				text = ConvertValue(value);
			}
			string[] array = text.Split(',');
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				value2.Add(text2.Trim());
			}
			return this;
		}

		public Headers AddAll(IEnumerable<KeyValuePair<string, string>> headers)
		{
			foreach (KeyValuePair<string, string> header in headers)
			{
				Add(header.Key, header.Value);
			}
			return this;
		}

		public string Get(string name)
		{
			return Get(name, string.Empty);
		}

		public string Get(string name, string defaultValue)
		{
			List<string> value;
			if (_headers.TryGetValue(name, out value))
			{
				return string.Join(", ", value.ToArray());
			}
			return defaultValue;
		}

		public DateTime Get(string name, DateTime defaultValue, bool allowDeltaTimeSeconds = false)
		{
			DateTime? dateTime = Get(name, null, allowDeltaTimeSeconds);
			if (dateTime.HasValue)
			{
				return dateTime.Value;
			}
			return defaultValue;
		}

		public DateTime? Get(string name, DateTime? defaultValue, bool allowDeltaTimeSeconds = false)
		{
			string text = Get(name, (string)null);
			if (text != null)
			{
				DateTime result;
				if (DateTime.TryParseExact(text, DateTimeFormatInfo.InvariantInfo.RFC1123Pattern, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal, out result))
				{
					return result;
				}
				long result2;
				if (allowDeltaTimeSeconds && long.TryParse(text, out result2) && result2 >= 0)
				{
					return DateTime.UtcNow.AddSeconds(result2);
				}
			}
			return defaultValue;
		}

		public long Get(string name, long defaultValue)
		{
			string text = Get(name, (string)null);
			long result;
			if (text != null && long.TryParse(text, out result))
			{
				return result;
			}
			return defaultValue;
		}

		public IEnumerable<string> GetAll(string name)
		{
			List<string> value;
			if (_headers.TryGetValue(name, out value))
			{
				return _headers[name];
			}
			return new string[0];
		}

		public Headers Set(string name, object value)
		{
			this[name] = ConvertValue(value);
			return this;
		}

		public Headers SetAll(IEnumerable<KeyValuePair<string, string>> headers)
		{
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			foreach (KeyValuePair<string, string> header in headers)
			{
				List<string> value;
				if (!dictionary.TryGetValue(header.Key, out value))
				{
					value = new List<string>();
					dictionary[header.Key] = value;
				}
				value.Add(header.Value);
			}
			SetAll(dictionary);
			return this;
		}

		public Headers SetAll(string name, IEnumerable<string> values)
		{
			name = NicifyName(name);
			List<string> list = new List<string>();
			foreach (string value in values)
			{
				list.Add(value);
			}
			_headers[name] = list;
			return this;
		}

		public Headers SetAll(Dictionary<string, List<string>> headers)
		{
			foreach (KeyValuePair<string, List<string>> header in headers)
			{
				SetAll(header.Key, header.Value);
			}
			return this;
		}

		public void ReadFrom(BinaryReader reader)
		{
			_headers.Clear();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string name = reader.ReadString();
				int num2 = reader.ReadInt32();
				for (int j = 0; j < num2; j++)
				{
					Add(name, reader.ReadString());
				}
			}
		}

		public void WriteTo(BinaryWriter writer)
		{
			writer.Write(_headers.Count);
			foreach (KeyValuePair<string, List<string>> header in _headers)
			{
				writer.Write(header.Key);
				List<string> value = header.Value;
				writer.Write(value.Count);
				foreach (string item in value)
				{
					writer.Write(item);
				}
			}
		}

		[DebuggerHidden]
		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			//yield-return decompiler failed: Could not find currentField
			_003CGetEnumerator_003Ec__Iterator0 obj = new _003CGetEnumerator_003Ec__Iterator0();
			obj._0024this = this;
			return obj;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private static bool IsNameNice(string name)
		{
			int length = name.Length;
			for (int i = 0; i < length; i++)
			{
				if (i == 0 || name[i - 1] == '-')
				{
					if (char.ToUpperInvariant(name[i]) != name[i])
					{
						return false;
					}
				}
				else if (char.ToLowerInvariant(name[i]) != name[i])
				{
					return false;
				}
			}
			return true;
		}

		private static string NicifyName(string name)
		{
			for (int i = 0; i < _niceNameExceptions.Length; i++)
			{
				if (_niceNameExceptions[i].Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					return _niceNameExceptions[i];
				}
			}
			if (IsNameNice(name))
			{
				return name;
			}
			int length = name.Length;
			char[] array = new char[length];
			for (int j = 0; j < length; j++)
			{
				bool flag = j == 0 || name[j - 1] == '-';
				array[j] = ((!flag) ? char.ToLowerInvariant(name[j]) : char.ToUpperInvariant(name[j]));
			}
			return new string(array);
		}

		private string ConvertValue(object value)
		{
			if (value is DateTime)
			{
				return ((DateTime)value).ToUniversalTime().ToString(DateTimeFormatInfo.InvariantInfo.RFC1123Pattern);
			}
			return value.ToString();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			using (IEnumerator<KeyValuePair<string, string>> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, string> current = enumerator.Current;
					if (flag)
					{
						flag = false;
					}
					else
					{
						stringBuilder.Append('\n');
					}
					stringBuilder.Append(current.Key).Append(": ").Append(current.Value);
				}
			}
			return stringBuilder.ToString();
		}

		public Dictionary<string, List<string>> GetInternalDictionary()
		{
			return _headers;
		}
	}
}
