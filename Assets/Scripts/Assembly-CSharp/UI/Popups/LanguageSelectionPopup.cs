using System;
using System.Collections.Generic;
using System.Linq;
using Data.Save;
using Kiloo.Common.LocalizationManager;
using UI.KScrollView;
using UI.Popups.LanguageSelectionPopupInternal;
using UnityEngine;

namespace UI.Popups
{
	public class LanguageSelectionPopup : UIPopup
	{
		private class LanguageListAdapter : ListAdapter<Locale>
		{
			private GameObject _languageCell;

			private LanguageSelectionPopup _languagePopup;

			public LanguageListAdapter(List<Locale> list, GameObject cell, LanguageSelectionPopup popup)
				: base(list)
			{
				_languageCell = cell;
				_languagePopup = popup;
			}

			public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass = false)
			{
				Container container2 = container;
				if (container2 == null)
				{
					container2 = new Container(_languageCell);
					LanguageCellController component = container2.prefabInstance.GetComponent<LanguageCellController>();
					container2.controlComponent = component;
					UIEventListener uIEventListener = UIEventListener.Get(container2.prefabInstance);
					uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(_languagePopup.OnClickContainer));
					uIEventListener.parameter = container2;
				}
				LanguageCellController languageCellController = (LanguageCellController)container2.controlComponent;
				languageCellController.UpdateContent(GetDataItem(dataIndex));
				return container2;
			}
		}

		private LanguageListAdapter _adapter;

		[SerializeField]
		private GameObject _languageCell;

		[SerializeField]
		private ScrollView _languageListView;

		[SerializeField]
		private UILabel _popupSubtitle;

		[SerializeField]
		private UILabel _popupTitle;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			List<Locale> list = LocaleData.instance.supportedLocales.ToList();
			while (list.Count < 7)
			{
				list.Add(null);
			}
			_adapter = new LanguageListAdapter(list, _languageCell, this);
			_languageListView.SetAdapter(_adapter);
		}

		protected override void Show()
		{
			UpdateLabels();
		}

		private void UpdateLabels()
		{
			_popupTitle.text = Strings.Get("SETTINGS_POPUP_LANGUAGE_TITLE");
			_popupSubtitle.text = Strings.language.locale.nativeName;
			_popupTitle.color = ColorData.instance.midnight;
			_popupSubtitle.color = ColorData.instance.peacock;
		}

		private void OnClickContainer(GameObject go)
		{
			Container container = (Container)go.GetComponent<UIEventListener>().parameter;
			LanguageCellController languageCellController = (LanguageCellController)container.controlComponent;
			Locale locale = languageCellController.locale;
			if (!(locale == null))
			{
				string combined = locale.combined;
				if (Strings.language.locale.combined != combined)
				{
					ConfirmationPopup.requestedLocale = locale;
					ConfirmationPopup.variant = ConfirmationPopupVariant.LanguageSelection;
					UIScreenController.Instance.PushPopup("ConfirmationPopup");
				}
				else
				{
					PreferencesSave.language = combined;
				}
			}
		}
	}
}
