using System;
using System.Collections.Generic;
using System.Reflection;
using SYBO.Components.Settings;
using SYBO.Components.Singletons;
using SYBO.Components.Utilities;
using UnityEngine.Events;

namespace SYBO.Components.Logging
{
	[Serializable]
	public class LogManager : Singleton<LogManager>
	{
		private object _lockObject = new object();

		private List<LogCategory> _categories = new List<LogCategory>(20);

		private LogCategorySettings[] _categorySettings = new LogCategorySettings[0];

		private LogSettings _settings;

		public LogSettings Settings
		{
			get
			{
				return _settings;
			}
			set
			{
				lock (_lockObject)
				{
					if (_settings != value)
					{
						if (_settings != null && _settings.Changed != null)
						{
							_settings.Changed.RemoveListener(OnLogSettingsChanged);
						}
						_settings = value;
						if (_categories.Count == 0)
						{
							ReflectionFindCategories();
						}
						else
						{
							UpdateCategorySettings();
						}
					}
				}
			}
		}

		public IEnumerable<LogCategory> Categories
		{
			get
			{
				return _categories;
			}
		}

		public string UserData { get; internal set; }

		public string CurrentScene { get; internal set; }

		private LogManager()
		{
		}

		public override void Initialize()
		{
			if (Settings == null)
			{
				Settings = StaticSettings.Log;
				if (Settings.Changed == null)
				{
					Settings.Changed = new UnityEvent();
				}
				else
				{
					Settings.Changed.RemoveListener(OnLogSettingsChanged);
				}
				Settings.Changed.AddListener(OnLogSettingsChanged);
			}
			else
			{
				ReflectionFindCategories();
			}
			if (LogCategory.LogEngine == null)
			{
				LogCategory.LogEngine = new LogPlatformUnityLean();
			}
		}

		private void OnLogSettingsChanged()
		{
			UpdateCategorySettings();
			SettingsEngine.Save();
		}

		private void ReflectionFindCategories()
		{
			_categories.Clear();
			Type[] types = ReflectionWrapper.GetTypes(ReflectionWrapper.GetAssembly(typeof(LogCategory)));
			foreach (Type type in types)
			{
				if (ReflectionWrapper.GetBaseType(type) == typeof(LogCategory))
				{
					ConstructorInfo constructor = ReflectionWrapper.GetConstructor(type, new Type[1] { typeof(int) });
					LogCategory item = (LogCategory)constructor.Invoke(new object[1] { _categories.Count });
					_categories.Add(item);
				}
			}
			UpdateCategorySettings();
		}

		private void UpdateCategorySettings()
		{
			_categorySettings = new LogCategorySettings[_categories.Count];
			bool flag = Settings.Categories != null && _categories.Count == Settings.Categories.Count;
			if (flag)
			{
				for (int i = 0; i < _categories.Count; i++)
				{
					LogCategorySettings value;
					if (!Settings.Categories.TryGetValue(_categories[i].CategoryText, out value))
					{
						flag = false;
						break;
					}
					_categories[i].ApplySetting(value);
					_categorySettings[i] = value;
				}
			}
			if (flag)
			{
				return;
			}
			Dictionary<string, LogCategorySettings> dictionary = new Dictionary<string, LogCategorySettings>();
			Dictionary<string, LogCategorySettings> dictionary2 = new Dictionary<string, LogCategorySettings>();
			foreach (LogCategorySettings value3 in Settings.Categories.Values)
			{
				dictionary[value3.categoryClassName] = value3;
				dictionary[value3.categoryText] = value3;
			}
			bool flag2 = false;
			for (int j = 0; j < _categories.Count; j++)
			{
				LogCategorySettings value2;
				if (!dictionary.TryGetValue(_categories[j].CategoryText, out value2) && !dictionary.TryGetValue(_categories[j].GetType().Name, out value2))
				{
					value2 = new LogCategorySettings();
					flag2 = true;
				}
				if (_categories[j].ApplySetting(value2))
				{
					flag2 = true;
				}
				_categorySettings[j] = value2;
				dictionary2[value2.categoryText] = value2;
			}
			Settings.Categories = dictionary2;
			if (flag2)
			{
				if (Settings.Changed != null)
				{
					Settings.Changed.Invoke();
				}
				else
				{
					OnLogSettingsChanged();
				}
			}
		}

		public void NotifyEntry(LogEntry entry)
		{
		}

		public string GetCategoryText(int _filterCategory)
		{
			return _categories[_filterCategory].CategoryText;
		}

		public int GetCategoriesCount()
		{
			return _categories.Count;
		}

		[Obsolete("The LogLongString has been moved from the LogManager to the relevant LogCategory.", false)]
		public static void LogLongString(LogCategory category, int level, string data)
		{
		}
	}
}
