using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kiloo.Common.LocalizationManager
{
	public class LocaleData : ScriptableObject
	{
		[Serializable]
		private struct SerializableLocale
		{
			public string nativeName;

			public bool isEnabled;

			public string language;

			public string region;

			public string[] extraRegions;
		}

		private const string RESOURCE_PATH = "Kiloo/LocalizationManager/LocaleData";

		private static LocaleData _instance;

		public Locale[] supportedLocales = new Locale[0];

		public Dictionary<string, string> nativeNames = new Dictionary<string, string>();

		[SerializeField]
		private List<SerializableLocale> _locales;

		public static LocaleData instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load<LocaleData>("Kiloo/LocalizationManager/LocaleData");
					_instance.Load();
				}
				return _instance;
			}
		}

		public Locale defaultLocale
		{
			get
			{
				return supportedLocales[0];
			}
		}

		private void Load()
		{
			List<SerializableLocale> list = _locales.FindAll((SerializableLocale x) => x.isEnabled);
			supportedLocales = new Locale[list.Count];
			for (int num = 0; num < list.Count; num++)
			{
				SerializableLocale serializableLocale = list[num];
				Locale locale = Locale.Create(serializableLocale.language, serializableLocale.region, serializableLocale.extraRegions, serializableLocale.nativeName);
				supportedLocales[num] = locale;
				nativeNames[locale.combined] = serializableLocale.nativeName;
			}
		}
	}
}
