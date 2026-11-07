using System;
using System.Collections.Generic;
using System.Reflection;
using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Components.Settings
{
	public static class SettingsHelper
	{
		public static FieldInfo[] GetAllSerializedFieldsStatic(Type settingsType)
		{
			FieldInfo[] fields = ReflectionWrapper.GetFields(settingsType, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			List<FieldInfo> list = new List<FieldInfo>(fields.Length);
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				if (ShouldSerializeField(fieldInfo))
				{
					list.Add(fieldInfo);
				}
			}
			return list.ToArray();
		}

		public static bool ShouldSerializeField(FieldInfo field)
		{
			bool result = field.IsPublic;
			foreach (object customAttribute in ReflectionWrapper.GetCustomAttributes(field, true))
			{
				if (customAttribute is DebugSettingAttribute && StaticSettings.Runtime.IsFinalBuild)
				{
					return false;
				}
				if (customAttribute is NonSerializedAttribute)
				{
					result = false;
				}
				if (customAttribute is SerializeField)
				{
					result = true;
				}
			}
			return result;
		}
	}
}
