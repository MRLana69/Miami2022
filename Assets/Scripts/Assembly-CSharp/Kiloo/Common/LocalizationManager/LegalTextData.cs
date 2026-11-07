using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AppRefresh;
using Data.Save;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using Kiloo.Common.Plugins.DeviceUtility;
using NaughtyAttributes;
using UnityEngine;

namespace Kiloo.Common.LocalizationManager
{
	public class LegalTextData : ScriptableObject
	{
		private const string RESOURCE_PATH = "Kiloo/LocalizationManager/LegalTextData";

		private static LegalTextData _instance;

		private LegalLanguage _currentLanguage;

		[InfoBox("Remember to generate LegalKeys (Kiloo/Localization/Generate Legal Keys", InfoBoxType.Normal, null)]
		[SerializeField]
		public TermKeys termKeys;

		[SerializeField]
		private LegalLanguage _fallbackLanguage;

		[ReorderableList]
		public List<LegalLanguage> legalLanguages;

		public static LegalTextData instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load<LegalTextData>("Kiloo/LocalizationManager/LegalTextData");
					_instance.Load();
				}
				return _instance;
			}
		}

		private void Load()
		{
			AppRefreshHandler.hasRefreshed += (RefreshType type) =>
			{
				DetectLocale();
			};
			DetectLocale();
		}

		public string GetLegalText(string key)
		{
			return _currentLanguage[key];
		}

		public CultureInfo GetLegalCulture()
		{
			try
			{
				return _currentLanguage.GetCulture();
			}
			catch (ArgumentException)
			{
				return _fallbackLanguage.GetCulture();
			}
		}

		private void DetectLocale()
		{
			string language = PreferencesSave.language;
			string input = ((!string.IsNullOrEmpty(language)) ? language : DeviceUtility.GetLocale());
			Locale locale;
			if (!Locale.TryParse(input, out locale))
			{
				locale = Locale.Parse("en-XX");
			}
			SetLanguage(locale);
		}

		public void SetLocale(string localeString)
		{
			Locale language = Locale.Parse(localeString);
			SetLanguage(language);
		}

		private void SetLanguage(Locale locale)
		{
			LegalLanguage legalLanguage = legalLanguages.Find((LegalLanguage l) => l.affectedLocales.Contains(locale.combined));
			if (!legalLanguage)
			{
				legalLanguage = _fallbackLanguage;
			}
			_currentLanguage = legalLanguage;
		}

		[Button(null)]
		[UsedImplicitly]
		private void SyncLanguageKeys()
		{
			foreach (LegalLanguage legalLanguage in legalLanguages)
			{
				SyncLanguage(legalLanguage);
			}
			SyncLanguage(_fallbackLanguage);
		}

		private void SyncLanguage(LegalLanguage lang)
		{
			List<LegalLanguage.Term> list = new List<LegalLanguage.Term>(termKeys.keys.Count);
			foreach (string key in termKeys.keys)
			{
				LegalLanguage.Term term = new LegalLanguage.Term
				{
					key = key
				};
				LegalLanguage.Term term2 = lang.terms.FirstOrDefault((LegalLanguage.Term t) => t.key == term.key);
				if (term2 != null)
				{
					term.value = term2.value;
				}
				list.Add(term);
			}
			lang.terms = list;
		}

		[Button(null)]
		[UsedImplicitly]
		private void FillEmptyTermsWithFallback()
		{
			foreach (LegalLanguage legalLanguage in legalLanguages)
			{
				FillMissingLanguageWithFallback(legalLanguage);
			}
		}

		private void FillMissingLanguageWithFallback(LegalLanguage lang)
		{
			foreach (LegalLanguage.Term term in lang.terms)
			{
				if (term.value.IsNullOrEmpty())
				{
					term.value = _fallbackLanguage[term.key];
				}
			}
		}
	}
}
