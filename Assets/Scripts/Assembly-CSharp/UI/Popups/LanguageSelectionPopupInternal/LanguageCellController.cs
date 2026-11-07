using Data.Save;
using Kiloo.Common.LocalizationManager;
using UnityEngine;

namespace UI.Popups.LanguageSelectionPopupInternal
{
	public class LanguageCellController : MonoBehaviour
	{
		private const string SPRITENAME_SELECTED = "icon_character_selected";

		private readonly Color32 _normalTextColor = GlobalColors.COUNTRY_POPUP_NORMAL_CELL_TEXT_COLOUR;

		private readonly Color32 _selectedTextColor = GlobalColors.COUNTRY_POPUP_SELECTED_CELL_TEXT_COLOUR;

		[SerializeField]
		private UILabel _languageLabel;

		[SerializeField]
		private UISprite _statusSprite;

		public Locale locale { get; private set; }

		public void UpdateContent(Locale newLocale)
		{
			locale = newLocale;
			if (locale == null)
			{
				_languageLabel.text = string.Empty;
				_statusSprite.enabled = false;
				return;
			}
			string text = null;
			Color32 color = _normalTextColor;
			string language = PreferencesSave.language;
			if (Strings.language.locale == locale && (language == locale.combined || string.IsNullOrEmpty(language)))
			{
				text = "icon_character_selected";
				color = _selectedTextColor;
			}
			_languageLabel.text = locale.nativeName;
			_languageLabel.color = color;
			_statusSprite.spriteName = text;
			_statusSprite.enabled = !string.IsNullOrEmpty(text);
		}
	}
}
