using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace SYBO.Components.Utilities
{
	public static class ReflectionWrapper
	{
		public static IEnumerable<object> GetCustomAttributes(MemberInfo type, bool inherit)
		{
			return type.GetCustomAttributes(inherit);
		}

		public static bool HasCustomAttributes(MemberInfo type, bool inherit)
		{
			object[] customAttributes = type.GetCustomAttributes(inherit);
			return customAttributes != null && customAttributes.Length > 0;
		}

		public static bool HasCustomAttributes(Type type, bool inherit)
		{
			object[] customAttributes = type.GetCustomAttributes(inherit);
			return customAttributes != null && customAttributes.Length > 0;
		}

		public static Attribute GetCustomAttribute(MemberInfo type, Type attributeType)
		{
			return Attribute.GetCustomAttribute(type, attributeType);
		}

		public static Attribute GetCustomAttribute(Type type, Type attributeType)
		{
			return Attribute.GetCustomAttribute(type, attributeType);
		}

		public static MethodInfo GetMethod(Type type, string methodName, BindingFlags bindingFlags)
		{
			return type.GetMethod(methodName, bindingFlags);
		}

		public static ConstructorInfo GetConstructor(Type type)
		{
			return type.GetConstructor(Type.EmptyTypes);
		}

		public static ConstructorInfo GetConstructor(Type type, Type[] types)
		{
			return type.GetConstructor(types);
		}

		public static ConstructorInfo[] GetConstructors(Type type, BindingFlags bindingFlags)
		{
			return type.GetConstructors(bindingFlags);
		}

		public static object GetUninitializedObject(Type type)
		{
			return FormatterServices.GetUninitializedObject(type);
		}

		public static IEnumerable<Type> GetDerivedTypes(Type parentType)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				Type[] types = assembly.GetTypes();
				foreach (Type type in types)
				{
					if (DerivesFrom(type, parentType))
					{
						yield return type;
					}
				}
			}
		}

		public static bool DerivesFrom(Type type, Type parentType)
		{
			while (type != null)
			{
				if (type == parentType)
				{
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}

		public static FieldInfo[] GetFields(Type type, BindingFlags bindingFlags)
		{
			return type.GetFields(bindingFlags);
		}

		public static FieldInfo[] GetFieldsForTree(Type type, BindingFlags bindingFlags)
		{
			Dictionary<IntPtr, FieldInfo> dictionary = new Dictionary<IntPtr, FieldInfo>();
			GetFieldsForTreeIntoSet(type, bindingFlags, dictionary);
			FieldInfo[] array = new FieldInfo[dictionary.Count];
			dictionary.Values.CopyTo(array, 0);
			return array;
		}

		public static Type GetInterface(Type type, Type interfaceType)
		{
			if (IsGenericType(interfaceType))
			{
				Type[] genericArguments = GetGenericArguments(interfaceType);
				foreach (Type type2 in genericArguments)
				{
					if (type2.FullName != null)
					{
						throw new ArgumentException("Closed generic interface types are not supported by GetInterface.");
					}
				}
			}
			return type.GetInterface(interfaceType.FullName);
		}

		public static Type[] GetGenericArguments(Type type)
		{
			return type.GetGenericArguments();
		}

		public static bool IsPrimitive(Type type)
		{
			return type.IsPrimitive;
		}

		public static bool IsGenericType(Type type)
		{
			return type.IsGenericType;
		}

		public static bool IsAbstract(Type type)
		{
			return type.IsAbstract;
		}

		public static bool IsValueType(Type type)
		{
			return type.IsValueType;
		}

		public static bool IsEnum(Type type)
		{
			return type.IsEnum;
		}

		public static Assembly GetAssembly(Type type)
		{
			return type.Assembly;
		}

		public static Type[] GetTypes(Assembly assembly)
		{
			return assembly.GetTypes();
		}

		internal static Type GetBaseType(Type type)
		{
			return type.BaseType;
		}

		private static void GetFieldsForTreeIntoSet(Type type, BindingFlags bindingFlags, Dictionary<IntPtr, FieldInfo> resultSet)
		{
			Type baseType = GetBaseType(type);
			if (baseType != null)
			{
				GetFieldsForTreeIntoSet(baseType, bindingFlags, resultSet);
			}
			FieldInfo[] fields = GetFields(type, bindingFlags);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (!resultSet.ContainsKey(fieldInfo.FieldHandle.Value))
				{
					resultSet.Add(fieldInfo.FieldHandle.Value, fieldInfo);
				}
			}
		}
	}
}
