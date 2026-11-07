using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;

namespace Kiloo.Common.Serialization.Parsing
{
	public static class ConversionHelper
	{
		[CompilerGenerated]
		private sealed class _003CValueAsKeyValuePairs_003Ec__Iterator0 : IEnumerable, IEnumerable<KeyValuePair<string, object>>, IEnumerator, IDisposable, IEnumerator<KeyValuePair<string, object>>
		{
			internal object obj;

			internal IEnumerable<KeyValuePair<string, object>> _003Ckvps_003E__0;

			internal IEnumerator<KeyValuePair<string, object>> _0024locvar0;

			internal KeyValuePair<string, object> _003Ckvp_003E__1;

			internal IDictionary _003Cdict_003E__0;

			internal IDictionaryEnumerator _0024locvar1;

			internal DictionaryEntry _003Centry_003E__2;

			internal IDisposable _0024locvar2;

			internal Type _003Ctype_003E__0;

			internal FieldInfo[] _003Cfields_003E__0;

			internal FieldInfo[] _0024locvar3;

			internal int _0024locvar4;

			internal FieldInfo _003Cfield_003E__3;

			internal bool _003CshouldSerialize_003E__4;

			internal object _003Cvalue_003E__5;

			internal KilooSerializerMapping mapping;

			internal string _003Ckey_003E__6;

			internal PropertyInfo[] _003Cprops_003E__0;

			internal PropertyInfo[] _0024locvar5;

			internal int _0024locvar6;

			internal PropertyInfo _003Cprop_003E__7;

			internal ParameterInfo[] _003CindexParameters_003E__8;

			internal object _003Cvalue_003E__9;

			internal string _003Ckey_003E__10;

			internal KeyValuePair<string, object> _0024current;

			internal bool _0024disposing;

			internal int _0024PC;

			KeyValuePair<string, object> IEnumerator<KeyValuePair<string, object>>.Current
			{
				[DebuggerHidden]
				get
				{
					return System_002ECollections_002EGeneric_002EIEnumerator_003CSystem_002ECollections_002EGeneric_002EKeyValuePair_003Cstring_002Cobject_003E_003E_002Eget_Current();
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
			public _003CValueAsKeyValuePairs_003Ec__Iterator0()
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
					if (obj == null)
					{
						throw new ArgumentNullException("obj");
					}
					_003Ckvps_003E__0 = obj as IEnumerable<KeyValuePair<string, object>>;
					if (_003Ckvps_003E__0 != null)
					{
						_0024locvar0 = _003Ckvps_003E__0.GetEnumerator();
						num = 4294967293u;
						goto case 1u;
					}
					_003Cdict_003E__0 = obj as IDictionary;
					if (_003Cdict_003E__0 != null)
					{
						_0024locvar1 = _003Cdict_003E__0.GetEnumerator();
						num = 4294967293u;
						goto case 2u;
					}
					_003Ctype_003E__0 = obj.GetType();
					_003Cfields_003E__0 = _003Ctype_003E__0.GetFields(BindingFlags.Instance | BindingFlags.Public);
					_0024locvar3 = _003Cfields_003E__0;
					_0024locvar4 = 0;
					goto IL_02fd;
				case 1u:
					try
					{
						switch (num)
						{
						default:
							if (_0024locvar0.MoveNext())
							{
								_003Ckvp_003E__1 = _0024locvar0.Current;
								_0024current = _003Ckvp_003E__1;
								if (!_0024disposing)
								{
									_0024PC = 1;
								}
								flag = true;
								goto end_IL_0011;
							}
							break;
						}
					}
					finally
					{
						if (!flag && _0024locvar0 != null)
						{
							_0024locvar0.Dispose();
						}
					}
					goto default;
				case 2u:
					try
					{
						switch (num)
						{
						default:
							if (_0024locvar1.MoveNext())
							{
								_003Centry_003E__2 = (DictionaryEntry)_0024locvar1.Current;
								_0024current = new KeyValuePair<string, object>(ValueAsString(_003Centry_003E__2.Key, CultureInfo.InvariantCulture) ?? _003Centry_003E__2.Key.ToString(), _003Centry_003E__2.Value);
								if (!_0024disposing)
								{
									_0024PC = 2;
								}
								flag = true;
								goto end_IL_0011;
							}
							break;
						}
					}
					finally
					{
						if (!flag && (_0024locvar2 = _0024locvar1 as IDisposable) != null)
						{
							_0024locvar2.Dispose();
						}
					}
					goto default;
				case 3u:
					_0024locvar4++;
					goto IL_02fd;
				case 4u:
					_0024locvar6++;
					goto IL_0426;
				default:
					{
						return false;
					}
					IL_02fd:
					if (_0024locvar4 < _0024locvar3.Length)
					{
						_003Cfield_003E__3 = _0024locvar3[_0024locvar4];
						_003CshouldSerialize_003E__4 = (_003Cfield_003E__3.Attributes & FieldAttributes.NotSerialized) == 0;
						if (_003CshouldSerialize_003E__4)
						{
							_003Cvalue_003E__5 = _003Cfield_003E__3.GetValue(obj);
							if (_003Cvalue_003E__5 != GetDefault(_003Cfield_003E__3.FieldType))
							{
								_003Ckey_003E__6 = ((mapping == null) ? _003Cfield_003E__3.Name : mapping.GetSerializedNameForMember(_003Ctype_003E__0, _003Cfield_003E__3.Name, false));
								_0024current = new KeyValuePair<string, object>(_003Ckey_003E__6, _003Cvalue_003E__5);
								if (!_0024disposing)
								{
									_0024PC = 3;
								}
								break;
							}
						}
						goto case 3u;
					}
					_003Cprops_003E__0 = _003Ctype_003E__0.GetProperties(BindingFlags.Instance | BindingFlags.Public);
					_0024locvar5 = _003Cprops_003E__0;
					_0024locvar6 = 0;
					goto IL_0426;
					IL_0426:
					if (_0024locvar6 < _0024locvar5.Length)
					{
						_003Cprop_003E__7 = _0024locvar5[_0024locvar6];
						if (_003Cprop_003E__7.CanRead)
						{
							_003CindexParameters_003E__8 = _003Cprop_003E__7.GetIndexParameters();
							if (_003CindexParameters_003E__8.Length == 0)
							{
								_003Cvalue_003E__9 = _003Cprop_003E__7.GetValue(obj, null);
								if (_003Cvalue_003E__9 != GetDefault(_003Cprop_003E__7.PropertyType))
								{
									_003Ckey_003E__10 = ((mapping == null) ? _003Cprop_003E__7.Name : mapping.GetSerializedNameForMember(_003Ctype_003E__0, _003Cprop_003E__7.Name, true));
									_0024current = new KeyValuePair<string, object>(_003Ckey_003E__10, _003Cvalue_003E__9);
									if (!_0024disposing)
									{
										_0024PC = 4;
									}
									break;
								}
							}
						}
						goto case 4u;
					}
					_0024PC = -1;
					goto default;
					end_IL_0011:
					break;
				}
				return true;
			}

			[DebuggerHidden]
			private KeyValuePair<string, object> System_002ECollections_002EGeneric_002EIEnumerator_003CSystem_002ECollections_002EGeneric_002EKeyValuePair_003Cstring_002Cobject_003E_003E_002Eget_Current()
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
						if (_0024locvar0 != null)
						{
							_0024locvar0.Dispose();
						}
					}
				case 2u:
					try
					{
						break;
					}
					finally
					{
						if ((_0024locvar2 = _0024locvar1 as IDisposable) != null)
						{
							_0024locvar2.Dispose();
						}
					}
				case 0u:
				case 3u:
				case 4u:
					break;
				}
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}

			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable<KeyValuePair<string, object>>)this).GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref _0024PC, 0, -2) == -2)
				{
					return this;
				}
				return new _003CValueAsKeyValuePairs_003Ec__Iterator0
				{
					obj = obj,
					mapping = mapping
				};
			}
		}

		[Pure]
		public static string ValueAsString(object value, IFormatProvider formatProvider)
		{
			string text = value as string;
			if (text != null)
			{
				return text;
			}
			if (value is DateTimeOffset)
			{
				return ((DateTimeOffset)value).ToString("O", DateTimeFormatInfo.InvariantInfo);
			}
			if (value is DateTime)
			{
				return ((DateTime)value).ToUniversalTime().ToString("O", DateTimeFormatInfo.InvariantInfo);
			}
			byte[] array = value as byte[];
			if (array != null)
			{
				return Convert.ToBase64String(array);
			}
			if (value is IConvertible)
			{
				try
				{
					return (string)Convert.ChangeType(value, typeof(string), formatProvider);
				}
				catch (Exception)
				{
				}
			}
			IFormattable formattable = value as IFormattable;
			if (formattable != null)
			{
				return formattable.ToString(null, formatProvider);
			}
			IEnumerable<char> enumerable = value as IEnumerable<char>;
			if (enumerable != null)
			{
				return new string((value as char[]) ?? enumerable.ToArray());
			}
			return null;
		}

		[Pure]
		public static IDictionary ValueAsDictionary(object obj, KilooSerializerMapping mapping)
		{
			if (obj == null)
			{
				return null;
			}
			return ValueAsKeyValuePairs(obj, mapping).ToDictionary((KeyValuePair<string, object> pair) => pair.Key, (KeyValuePair<string, object> pair) => pair.Value);
		}

		[DebuggerHidden]
		[Pure]
		public static IEnumerable<KeyValuePair<string, object>> ValueAsKeyValuePairs([NotNull] object obj, KilooSerializerMapping mapping)
		{
			//yield-return decompiler failed: Could not find currentField
			_003CValueAsKeyValuePairs_003Ec__Iterator0 obj2 = new _003CValueAsKeyValuePairs_003Ec__Iterator0();
			obj2.obj = obj;
			obj2.mapping = mapping;
			obj2._0024PC = -2;
			return obj2;
		}

		private static object GetDefault(Type type)
		{
			if (type.IsValueType)
			{
				return Activator.CreateInstance(type);
			}
			return null;
		}
	}
}
