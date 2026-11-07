using System.Reflection;
using UnityEngine;

namespace SYBO.Components.Settings
{
	public static class SettingsEngine
	{
		private static readonly string _rootResourceName = "SyboSettings";

		private static readonly string _resourceLoadPath = "Settings/{0}";

		private static object _loadLock = new object();

		private static bool _isLoaded = false;

		private static ISettingsProvider _settingsProvider = null;

		public static ISettingsProvider GetSettingsProvider()
		{
			return _settingsProvider;
		}

		public static void Reload()
		{
			_isLoaded = false;
			Load();
		}

		public static void Load()
		{
			lock (_loadLock)
			{
				if (!_isLoaded)
				{
					_isLoaded = true;
					_settingsProvider = new DefaultSettingsProvider(new CachingRootFileJsonSettingsParser(ReadFileText, typeof(StaticSettings), _rootResourceName));
					FieldInfo[] allSerializedFieldsStatic = SettingsHelper.GetAllSerializedFieldsStatic(typeof(StaticSettings));
					FieldInfo[] array = allSerializedFieldsStatic;
					foreach (FieldInfo fieldInfo in array)
					{
						fieldInfo.SetValue(null, _settingsProvider.ReflectionGetSettings(fieldInfo.FieldType));
					}
				}
			}
		}

		public static void Save()
		{
			Debug.LogWarningFormat("SettingsEngine.Save() is available only #if UNITY_EDITOR.");
		}

		public static void Backup()
		{
		}

		public static void Restore()
		{
		}

		private static string ReadFileText(string filename)
		{
			TextAsset textAsset = Resources.Load<TextAsset>(string.Format(_resourceLoadPath, filename));
			if (textAsset != null)
			{
				return textAsset.text;
			}
			return null;
		}
	}
}
