using System;
using System.Collections.Generic;
using UnityEngine;

namespace SYBO.Components.Utilities
{
	[Serializable]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		private readonly string ErrorMsgFormat = "There are {0} keys and {1} values after deserialization. Make surethat both key and value types are serializable.";

		[SerializeField]
		private List<TKey> _keys = new List<TKey>();

		[SerializeField]
		private List<TValue> _values = new List<TValue>();

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			_keys.Clear();
			_values.Clear();
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<TKey, TValue> current = enumerator.Current;
					_keys.Add(current.Key);
					_values.Add(current.Value);
				}
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			Clear();
			if (_keys.Count != _values.Count)
			{
				throw new Exception(string.Format(ErrorMsgFormat, _keys.Count, _values.Count));
			}
			for (int i = 0; i < _keys.Count; i++)
			{
				Add(_keys[i], _values[i]);
			}
		}
	}
}
