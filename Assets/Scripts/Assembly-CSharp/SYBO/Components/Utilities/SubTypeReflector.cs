using System;
using System.Collections.Generic;
using System.Reflection;

namespace SYBO.Components.Utilities
{
	public static class SubTypeReflector
	{
		public static List<Type> GetSubTypes<T>() where T : class
		{
			return GetSubTypes(typeof(T));
		}

		public static List<Type> GetSubTypes(Type typeT)
		{
			if (!typeT.IsClass)
			{
				throw new ArgumentException("Given type must be a class.", "typeT");
			}
			List<Type> list = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				if (IsUnwantedAssembly(assembly))
				{
					continue;
				}
				Type[] types = assembly.GetTypes();
				foreach (Type type in types)
				{
					if (IsCorrectType(type, typeT))
					{
						list.Add(type);
					}
				}
			}
			return list;
		}

		private static bool IsUnwantedAssembly(Assembly assembly)
		{
			string fullName = assembly.FullName;
			if (fullName.StartsWith("Mono.Cecil"))
			{
				return true;
			}
			if (fullName.StartsWith("UnityScript"))
			{
				return true;
			}
			if (fullName.StartsWith("Boo.Lan"))
			{
				return true;
			}
			if (fullName.StartsWith("System"))
			{
				return true;
			}
			if (fullName.StartsWith("I18N"))
			{
				return true;
			}
			if (fullName.StartsWith("UnityEngine"))
			{
				return true;
			}
			if (fullName.StartsWith("UnityEditor"))
			{
				return true;
			}
			if (fullName.StartsWith("mscorlib"))
			{
				return true;
			}
			return false;
		}

		private static bool IsCorrectType(Type type, Type typeT)
		{
			if (!type.IsClass)
			{
				return false;
			}
			if (type.IsAbstract)
			{
				return false;
			}
			if (typeT.IsGenericTypeDefinition)
			{
				if (!IsSubclassOfGenericType(type, typeT))
				{
					return false;
				}
			}
			else if (!type.IsSubclassOf(typeT))
			{
				return false;
			}
			return true;
		}

		private static bool IsSubclassOfGenericType(Type type, Type genericType)
		{
			if (type == genericType)
			{
				return false;
			}
			Type typeFromHandle = typeof(object);
			while (type != null && type != typeFromHandle)
			{
				Type type2 = ((!type.IsGenericType) ? type : type.GetGenericTypeDefinition());
				if (genericType == type2)
				{
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
	}
}
