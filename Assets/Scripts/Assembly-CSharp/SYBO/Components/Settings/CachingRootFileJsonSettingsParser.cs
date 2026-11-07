using System;
using System.Collections.Generic;
using System.Reflection;
using SYBO.Components.Json;
using SYBO.Components.Utilities;

namespace SYBO.Components.Settings
{
	public class CachingRootFileJsonSettingsParser : ISettingsParser
	{
		public delegate string FileTextByPathGetter(string path);

		private FileTextByPathGetter _getFileText;

		private Dictionary<Type, object> _cachedSettings = new Dictionary<Type, object>();

		public CachingRootFileJsonSettingsParser(FileTextByPathGetter getFileText, Type rootSettingsType, string rootSettingsPath)
		{
			_getFileText = getFileText;
			LoadRootSettings(rootSettingsType, rootSettingsPath);
		}

		object ISettingsParser.ReflectionReadSettings(Type settingsType)
		{
			object value;
			if (!_cachedSettings.TryGetValue(settingsType, out value))
			{
				value = JsonClassInfo.GetJsonTypeReflection(settingsType).Construct(new JsonContext());
				_cachedSettings[settingsType] = value;
			}
			return value;
		}

		private string GetFileText(string path)
		{
			return _getFileText(path);
		}

		private void LoadRootSettings(Type rootSettingsType, string rootSettingsPath)
		{
			JsonFileSerializer jsonFileSerializer = new JsonFileSerializer(GetFileText, (string f, string t) =>
			{
			});
			JsonDict jsonDict = JsonDict.Deserialize(GetFileText(rootSettingsPath));
			if (jsonDict == null)
			{
				return;
			}
			FieldInfo[] allSerializedFieldsStatic = SettingsHelper.GetAllSerializedFieldsStatic(rootSettingsType);
			foreach (FieldInfo fieldInfo in allSerializedFieldsStatic)
			{
				SerializeFileAttribute serializeFileAttribute = ReflectionWrapper.GetCustomAttribute(fieldInfo, typeof(SerializeFileAttribute)) as SerializeFileAttribute;
				if (serializeFileAttribute != null)
				{
					_cachedSettings[fieldInfo.FieldType] = jsonFileSerializer.DeserializeFromMultipleFiles(fieldInfo.FieldType, serializeFileAttribute.SerializedFilename);
				}
				else
				{
					_cachedSettings[fieldInfo.FieldType] = JsonApi.ReflectionDeserializeDomAsType(fieldInfo.FieldType, jsonDict.GetDict(JsonClassInfo.GetSerializedName(fieldInfo)));
				}
			}
		}
	}
}
