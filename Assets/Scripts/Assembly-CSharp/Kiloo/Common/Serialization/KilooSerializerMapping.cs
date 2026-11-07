using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace Kiloo.Common.Serialization
{
	public class KilooSerializerMapping
	{
		private struct Member
		{
			[UsedImplicitly]
			public readonly Type type;

			[UsedImplicitly]
			public readonly string name;

			public Member(Type type, string name)
			{
				this.type = type;
				this.name = name;
			}
		}

		public class MapFrom<TFrom>
		{
			private readonly KilooSerializerMapping _mapping;

			public MapFrom(KilooSerializerMapping mapping)
			{
				_mapping = mapping;
			}

			public KilooSerializerMapping To<TTo>() where TTo : TFrom
			{
				return _mapping.Map(typeof(TFrom), typeof(TTo));
			}
		}

		private Dictionary<Member, List<string>> _deserializeMemberMapping;

		private Dictionary<Member, string> _serializeMemberMapping;

		private Dictionary<Type, Type> _bindings;

		public KilooSerializerMapping Map<TSubject>(Expression<Func<TSubject, object>> memberExpression, string serializedPropertyName, bool useWhenSerializing = false)
		{
			MemberExpression memberExpression2 = memberExpression.Body as MemberExpression;
			if (memberExpression2 == null)
			{
				throw new ArgumentException("Parameter must be a member expression", "memberExpression");
			}
			return Map(typeof(TSubject), memberExpression2.Member.Name, serializedPropertyName, useWhenSerializing);
		}

		public KilooSerializerMapping Map([NotNull] Type containingType, [NotNull] string memberName, [NotNull] string serializedPropertyName, bool useWhenSerializing = false)
		{
			if (containingType == null)
			{
				throw new ArgumentNullException("containingType");
			}
			if (string.IsNullOrEmpty(memberName))
			{
				throw new ArgumentException("Value cannot be null or empty.", "memberName");
			}
			MemberInfo[] member = containingType.GetMember(memberName, MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public);
			if (member.Length == 0)
			{
				throw new ArgumentException("Value does not match a public instance field or property in type.", "memberName");
			}
			Map(new Member(containingType, memberName), serializedPropertyName, useWhenSerializing);
			return this;
		}

		private void Map(Member member, [NotNull] string serializedPropertyName, bool useWhenSerializing)
		{
			if (string.IsNullOrEmpty(serializedPropertyName))
			{
				throw new ArgumentException("Value cannot be null or empty.", "serializedPropertyName");
			}
			if (_deserializeMemberMapping == null)
			{
				_deserializeMemberMapping = new Dictionary<Member, List<string>>();
			}
			List<string> value;
			if (!_deserializeMemberMapping.TryGetValue(member, out value))
			{
				value = new List<string>();
				_deserializeMemberMapping[member] = value;
			}
			value.Add(serializedPropertyName);
			if (useWhenSerializing)
			{
				if (_serializeMemberMapping == null)
				{
					_serializeMemberMapping = new Dictionary<Member, string>();
				}
				_serializeMemberMapping.Add(member, serializedPropertyName);
			}
		}

		public string GetSerializedNameForMember(Type containingType, string memberName, bool useMappingOfBaseTypes)
		{
			if (_serializeMemberMapping != null)
			{
				string value;
				if (_serializeMemberMapping.TryGetValue(new Member(containingType, memberName), out value))
				{
					return value;
				}
				if (useMappingOfBaseTypes)
				{
					Type baseType = containingType.BaseType;
					while (baseType != null && baseType != typeof(object))
					{
						if (_serializeMemberMapping.TryGetValue(new Member(baseType, memberName), out value))
						{
							return value;
						}
						baseType = baseType.BaseType;
					}
					Type[] interfaces = containingType.GetInterfaces();
					for (int i = 0; i < interfaces.Length; i++)
					{
						if (_serializeMemberMapping.TryGetValue(new Member(interfaces[i], memberName), out value))
						{
							return value;
						}
					}
				}
			}
			return memberName;
		}

		private bool TryGetMappedValueForMember(Type containingType, string memberName, IDictionary properties, out object value)
		{
			List<string> value2;
			if (_deserializeMemberMapping != null && _deserializeMemberMapping.TryGetValue(new Member(containingType, memberName), out value2))
			{
				foreach (string item in value2)
				{
					value = properties[item];
					if (value != null || properties.Contains(item))
					{
						return true;
					}
				}
			}
			value = null;
			return false;
		}

		public bool TryGetValueForMember(Type containingType, string memberName, IDictionary properties, out object value, bool useMappingOfBaseTypes)
		{
			if (_deserializeMemberMapping != null)
			{
				if (TryGetMappedValueForMember(containingType, memberName, properties, out value))
				{
					return true;
				}
				if (useMappingOfBaseTypes)
				{
					Type baseType = containingType.BaseType;
					while (baseType != null && baseType != typeof(object))
					{
						if (TryGetMappedValueForMember(baseType, memberName, properties, out value))
						{
							return true;
						}
						baseType = baseType.BaseType;
					}
					Type[] interfaces = containingType.GetInterfaces();
					for (int i = 0; i < interfaces.Length; i++)
					{
						if (TryGetMappedValueForMember(interfaces[i], memberName, properties, out value))
						{
							return true;
						}
					}
				}
			}
			value = properties[memberName];
			if (value != null || properties.Contains(memberName))
			{
				return true;
			}
			return false;
		}

		public Type GetConcreteType(Type abstractType)
		{
			Type value;
			if (_bindings != null && _bindings.TryGetValue(abstractType, out value))
			{
				return value;
			}
			return abstractType;
		}

		public KilooSerializerMapping Map(Type fromAbstractType, Type toConcreteType)
		{
			if (!fromAbstractType.IsAssignableFrom(toConcreteType))
			{
				throw new InvalidCastException(string.Concat("Cannot assign a ", toConcreteType, " to a ", fromAbstractType));
			}
			if (_bindings == null)
			{
				_bindings = new Dictionary<Type, Type>();
			}
			_bindings.Add(fromAbstractType, toConcreteType);
			return this;
		}

		public MapFrom<TFrom> Map<TFrom>()
		{
			return new MapFrom<TFrom>(this);
		}
	}
}
