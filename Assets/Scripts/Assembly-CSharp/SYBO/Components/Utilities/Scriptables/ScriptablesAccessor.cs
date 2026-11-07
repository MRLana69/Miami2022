using System;
using System.Reflection;
using SYBO.Components.Logging;
using UnityEngine;

namespace SYBO.Components.Utilities.Scriptables
{
	public static class ScriptablesAccessor
	{
		public static ScriptablesAccessorEvent OnUpdatedFields = new ScriptablesAccessorEvent();

		public const BindingFlags PublicStaticFields = BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public;

		public static void UpdateFields(Type partialClass)
		{
			FieldInfo[] fields = partialClass.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
			TypeListAttribute typeListAttribute = Attribute.GetCustomAttribute(partialClass, typeof(TypeListAttribute)) as TypeListAttribute;
			if (typeListAttribute == null)
			{
				return;
			}
			Type[] types = typeListAttribute.Types;
			foreach (Type type in types)
			{
				FieldInfo[] fields2 = Array.FindAll(fields, (FieldInfo x) => type.IsAssignableFrom(x.FieldType));
				PopulateFieldsOfType(type, fields2, (UnityEngine.Object feature, FieldInfo field) => feature.name == field.Name);
				OnUpdatedFields.Invoke(type);
			}
		}

		public static bool CanUpdateFields(Type partialClass)
		{
			TypeListAttribute typeListAttribute = Attribute.GetCustomAttribute(partialClass, typeof(TypeListAttribute)) as TypeListAttribute;
			if (typeListAttribute != null)
			{
				return true;
			}
			return false;
		}

		private static void PopulateFieldsOfType(Type type, FieldInfo[] fields, Func<UnityEngine.Object, FieldInfo, bool> comparator)
		{
			if (fields == null || fields.Length <= 0)
			{
				return;
			}
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(type);
			foreach (FieldInfo field in fields)
			{
				UnityEngine.Object asset = Array.Find(array, (UnityEngine.Object x) => comparator(x, field));
				PopulateFieldOfType(type, field, asset);
			}
		}

		private static void PopulateFieldOfType(Type type, FieldInfo field, UnityEngine.Object asset)
		{
			field.SetValue(null, null);
			if (asset != null)
			{
				if (field.FieldType.IsAssignableFrom(asset.GetType()))
				{
					field.SetValue(null, asset);
					return;
				}
				Log.Utilities.LogErrorFormat(0, "Cannot assign asset with name '{0}' of type '{1}' to field '{2}' with type '{3}'", asset.name, asset.GetType(), field.Name, field.FieldType);
			}
		}
	}
}
