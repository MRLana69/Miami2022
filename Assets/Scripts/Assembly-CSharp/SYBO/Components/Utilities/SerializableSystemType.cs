using System;
using UnityEngine;

namespace SYBO.Components.Utilities
{
	[Serializable]
	public class SerializableSystemType : ISerializationCallbackReceiver
	{
		[SerializeField]
		private string _assemblyQualifiedName;

		private Type _systemType;

		public Type SystemType
		{
			get
			{
				return _systemType;
			}
			set
			{
				_systemType = value;
			}
		}

		public SerializableSystemType(Type systemType)
		{
			_systemType = systemType;
		}

		public override bool Equals(object obj)
		{
			SerializableSystemType serializableSystemType = obj as SerializableSystemType;
			if ((object)serializableSystemType == null)
			{
				return false;
			}
			return Equals(serializableSystemType);
		}

		public bool Equals(SerializableSystemType otherSystemType)
		{
			return (otherSystemType.SystemType == null && SystemType == null) || otherSystemType.SystemType.Equals(SystemType);
		}

		public override int GetHashCode()
		{
			if (_systemType == null)
			{
				return base.GetHashCode();
			}
			return _systemType.GetHashCode();
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (_assemblyQualifiedName == null)
			{
				_systemType = null;
			}
			else
			{
				_systemType = Type.GetType(_assemblyQualifiedName);
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (_systemType == null)
			{
				_assemblyQualifiedName = null;
			}
			else
			{
				_assemblyQualifiedName = _systemType.AssemblyQualifiedName;
			}
		}

		public static bool operator ==(SerializableSystemType a, SerializableSystemType b)
		{
			if (object.ReferenceEquals(a, b))
			{
				return true;
			}
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			return a.Equals(b);
		}

		public static bool operator !=(SerializableSystemType a, SerializableSystemType b)
		{
			return !(a == b);
		}
	}
}
