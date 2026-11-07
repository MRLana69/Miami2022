using System;
using System.Collections.Generic;
using SYBO.Components.Json.Internal;
using UnityEngine;

namespace SYBO.Components.Json
{
	public class JsonContext
	{
		public class SubtreeFileReference
		{
			public string Filename;

			public object SubObject;

			public JsonFieldInfo Field;
		}

		private class SerializeByKeyReferenceData
		{
			public JsonFieldInfo field;

			public object json;

			public SerializeByKeyReferenceData next;
		}

		private class DeserializeByKeyReferenceData
		{
			public object obj;

			public JsonFieldInfo field;

			public object json;

			public DeserializeByKeyReferenceData next;
		}

		private class SerializationCallback
		{
			public ISerializationCallbackReceiver serializedInterface;

			public SerializationCallback next;
		}

		public const int MaximumNestingDepth = 100;

		public JsonObjectStore ObjectStore = new JsonObjectStore();

		public JsonCustomSerializerCollection CustomSerializers = new JsonCustomSerializerCollection();

		public bool TolerateBlindReferencesOnSerialize;

		public bool RemoveWhitespace;

		public bool IncludeStatics;

		public Predicate<JsonFieldInfo> ShouldFilterField;

		public List<SubtreeFileReference> JsonFileReferences = new List<SubtreeFileReference>();

		private int nestedDeserializeCounter;

		private int nestedSerializeCounter;

		private DeserializeByKeyReferenceData _referenceDataToBeResolved;

		private SerializeByKeyReferenceData _referenceDataToBeValidated;

		private SerializationCallback _lastSerializationCallback;

		private SerializationCallback _firstSerializationCallback;

		public object Serialize<T>(T data)
		{
			BeginSerialize();
			object result = JsonReflectionParser.Serialize(data, this);
			EndSerialize();
			return result;
		}

		public T Deserialize<T>(object json)
		{
			BeginDeserialize();
			T result = JsonReflectionParser.Deserialize<T>(json, this);
			EndDeserialize();
			return result;
		}

		public void BeginDeserialize()
		{
			if (nestedSerializeCounter != 0)
			{
				throw new JsonSerializationException("Call to BeginDeserialize() while already doing a Serialization Batch Job.");
			}
			nestedDeserializeCounter++;
			if (nestedDeserializeCounter > 100)
			{
				throw new JsonSerializationException("Nesting depth must be less than " + 100);
			}
		}

		public void EndDeserialize()
		{
			if (nestedDeserializeCounter == 0)
			{
				throw new JsonSerializationException("Call to EndDeserialize() but not doing a Deserialization Batch Job.");
			}
			nestedDeserializeCounter--;
			if (nestedDeserializeCounter == 0)
			{
				ResolveReferences();
				InvokeSerializationEvents();
			}
		}

		public void BeginSerialize()
		{
			if (nestedDeserializeCounter != 0)
			{
				throw new JsonSerializationException("Call to BeginSerialize() while already doing a Deserialization Batch Job.");
			}
			nestedSerializeCounter++;
			if (nestedSerializeCounter > 100)
			{
				throw new JsonSerializationException("Nesting depth must be less than " + 100);
			}
		}

		public void EndSerialize()
		{
			if (nestedSerializeCounter == 0)
			{
				throw new JsonSerializationException("Call to EndSerialize() but not doing a Serialization Batch Job.");
			}
			nestedSerializeCounter--;
			if (nestedSerializeCounter == 0)
			{
				ValidateReferences();
			}
		}

		public void RegisterSerializeByKeyReference(JsonFieldInfo field, object json)
		{
			if (!TolerateBlindReferencesOnSerialize)
			{
				_referenceDataToBeValidated = new SerializeByKeyReferenceData
				{
					field = field,
					json = json,
					next = _referenceDataToBeValidated
				};
			}
		}

		public void RegisterDeserializeByKeyReference(object obj, JsonFieldInfo field, object json)
		{
			if (json != null)
			{
				_referenceDataToBeResolved = new DeserializeByKeyReferenceData
				{
					obj = obj,
					field = field,
					json = json,
					next = _referenceDataToBeResolved
				};
			}
		}

		public void RegisterSerializationCallBack(ISerializationCallbackReceiver serializedInterface)
		{
			SerializationCallback serializationCallback = new SerializationCallback();
			serializationCallback.serializedInterface = serializedInterface;
			SerializationCallback serializationCallback2 = serializationCallback;
			if (_firstSerializationCallback == null)
			{
				_firstSerializationCallback = serializationCallback2;
				_lastSerializationCallback = serializationCallback2;
			}
			else
			{
				_lastSerializationCallback.next = serializationCallback2;
				_lastSerializationCallback = serializationCallback2;
			}
		}

		public JsonContext Duplicate()
		{
			JsonContext jsonContext = new JsonContext();
			jsonContext.CustomSerializers = CustomSerializers.Duplicate();
			jsonContext.ObjectStore = ObjectStore.Duplicate();
			jsonContext.TolerateBlindReferencesOnSerialize = TolerateBlindReferencesOnSerialize;
			return jsonContext;
		}

		public Dictionary<Type, HashSet<string>> CollectUnknownReferences()
		{
			Dictionary<Type, HashSet<string>> dictionary = new Dictionary<Type, HashSet<string>>();
			for (DeserializeByKeyReferenceData deserializeByKeyReferenceData = _referenceDataToBeResolved; deserializeByKeyReferenceData != null; deserializeByKeyReferenceData = deserializeByKeyReferenceData.next)
			{
				JsonReflectionParser.CollectReferenceUnknownKeys(deserializeByKeyReferenceData.field.Field.FieldType, deserializeByKeyReferenceData.json, this, dictionary);
			}
			return dictionary;
		}

		public void ResolveKnownReferences()
		{
			DeserializeByKeyReferenceData deserializeByKeyReferenceData = null;
			DeserializeByKeyReferenceData deserializeByKeyReferenceData2 = null;
			while (_referenceDataToBeResolved != null)
			{
				try
				{
					object value = JsonReflectionParser.ReadReference(_referenceDataToBeResolved.field.Field.FieldType, _referenceDataToBeResolved.json, this);
					_referenceDataToBeResolved.field.Field.SetValue(_referenceDataToBeResolved.obj, value);
					_referenceDataToBeResolved = _referenceDataToBeResolved.next;
				}
				catch (JsonReferenceException)
				{
					if (deserializeByKeyReferenceData == null)
					{
						deserializeByKeyReferenceData = _referenceDataToBeResolved;
						deserializeByKeyReferenceData2 = _referenceDataToBeResolved;
					}
					else
					{
						deserializeByKeyReferenceData2.next = _referenceDataToBeResolved;
						deserializeByKeyReferenceData2 = _referenceDataToBeResolved;
					}
					_referenceDataToBeResolved = _referenceDataToBeResolved.next;
					deserializeByKeyReferenceData2.next = null;
				}
			}
			_referenceDataToBeResolved = deserializeByKeyReferenceData;
		}

		private void ResolveReferences()
		{
			while (_referenceDataToBeResolved != null)
			{
				object value = JsonReflectionParser.ReadReference(_referenceDataToBeResolved.field.Field.FieldType, _referenceDataToBeResolved.json, this);
				_referenceDataToBeResolved.field.Field.SetValue(_referenceDataToBeResolved.obj, value);
				_referenceDataToBeResolved = _referenceDataToBeResolved.next;
			}
		}

		private void InvokeSerializationEvents()
		{
			while (_firstSerializationCallback != null)
			{
				_firstSerializationCallback.serializedInterface.OnAfterDeserialize();
				_firstSerializationCallback = _firstSerializationCallback.next;
			}
			_lastSerializationCallback = null;
		}

		private void ValidateReferences()
		{
			if (TolerateBlindReferencesOnSerialize)
			{
				_referenceDataToBeValidated = null;
			}
			while (_referenceDataToBeValidated != null)
			{
				if (!ObjectStore.HasObject(_referenceDataToBeValidated.field.ValueType, _referenceDataToBeValidated.json))
				{
					throw new JsonReferenceException(string.Format("Json references object {0} with key {1} which doesn't exist.", _referenceDataToBeValidated.field.ValueType.Name, _referenceDataToBeValidated.json.ToString()));
				}
				_referenceDataToBeValidated = _referenceDataToBeValidated.next;
			}
		}
	}
}
