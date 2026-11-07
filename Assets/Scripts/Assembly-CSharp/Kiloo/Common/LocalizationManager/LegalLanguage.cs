using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kiloo.Common.Extensions;
using NaughtyAttributes;
using UnityEngine;

namespace Kiloo.Common.LocalizationManager
{
	[CreateAssetMenu(menuName = "Localization/Legal Language")]
	public class LegalLanguage : ScriptableObject
	{
		[Serializable]
		public class Term
		{
			public string key;

			public string value;
		}

		[ReadOnly]
		public string id;

		[SerializeField]
		private string _localeId;

		public List<string> affectedLocales;

		[SerializeField]
		private string _pasteLocaleList;

		[ResizableTextArea]
		[SerializeField]
		private string _pasteTermValues;

		public List<Term> terms;

		public string this[string key]
		{
			get
			{
				Term term = terms.FirstOrDefault((Term t) => t.key == key);
				if (term != null)
				{
					return term.value;
				}
				return string.Empty;
			}
		}

		public CultureInfo GetCulture()
		{
			return CultureInfo.GetCultureInfo(_localeId);
		}

		[Button(null)]
		private void ImportLocalePaste()
		{
			if (_pasteLocaleList.Length > 0)
			{
				string[] source = _pasteLocaleList.Split(' ');
				affectedLocales.AddRangeDistinct(source.Where((string l) => !l.IsNullOrEmpty()));
				_pasteLocaleList = string.Empty;
			}
		}

		[Button(null)]
		private void ImportValuePaste()
		{
			if (_pasteTermValues.Length > 0)
			{
				List<string> list = (from v in _pasteTermValues.Split('\n')
					where !v.IsNullOrWhiteSpace()
					select v).ToList();
				for (int num = 0; num < terms.Count; num++)
				{
					Term term = terms[num];
					term.value = list[num];
				}
				_pasteTermValues = string.Empty;
			}
		}

		private void OnValidate()
		{
			id = base.name;
		}
	}
}
