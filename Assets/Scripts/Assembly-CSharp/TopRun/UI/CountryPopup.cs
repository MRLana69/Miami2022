using System;
using System.Collections;
using System.Collections.Generic;
using Kiloo;
using Kiloo.Common.Utility;
using UI;
using UI.Generic.Layout;
using UI.KScrollView;
using UI.Popups;
using UnityEngine;
using UnityEngine.Serialization;

namespace TopRun.UI
{
	public class CountryPopup : UIPopup
	{
		private class CountryListAdapter : ListAdapter<Country>
		{
			private GameObject _countryCell;

			private CountryPopup _countryPopup;

			public CountryListAdapter(List<Country> countryList, GameObject cell, CountryPopup popup)
				: base(countryList)
			{
				_countryCell = cell;
				_countryPopup = popup;
			}

			public override Container GetContainer(Container container, int dataIndex, bool isMeasurePass = false)
			{
				Container container2 = container;
				if (container2 == null)
				{
					container2 = new Container(_countryCell);
					CountryCellController component = container2.prefabInstance.GetComponent<CountryCellController>();
					container2.controlComponent = component;
					UIEventListener uIEventListener = UIEventListener.Get(container2.prefabInstance);
					uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(_countryPopup.OnClickContainer));
					uIEventListener.parameter = container2;
				}
				CountryCellController countryCellController = (CountryCellController)container2.controlComponent;
				countryCellController.countryName.text = Strings.Get(GetDataItem(dataIndex).displayNameID);
				countryCellController.countryFlag.spriteName = GetDataItem(dataIndex).flag;
				countryCellController.countryLocale = GetDataItem(dataIndex).countryCode;
				IManager manager = TopRunRoot.manager;
				string text = GetDataItem(dataIndex).countryCode.ToLower();
				if (manager != null && text == manager.country.ToLower())
				{
					if (manager.tournament == null || text == manager.tournament.country.ToLower())
					{
						countryCellController.statusSprite.spriteName = "icon_character_selected";
						countryCellController.statusSprite.enabled = true;
						countryCellController.countryName.color = GlobalColors.COUNTRY_POPUP_SELECTED_CELL_TEXT_COLOUR;
					}
					else
					{
						countryCellController.statusSprite.spriteName = "icon_timer";
						countryCellController.statusSprite.enabled = true;
						countryCellController.countryName.color = GlobalColors.COUNTRY_POPUP_SELECTED_CELL_TEXT_COLOUR;
					}
				}
				else if (manager != null && manager.tournament != null && text == manager.tournament.country.ToLower())
				{
					countryCellController.statusSprite.spriteName = "icon_character_owned";
					countryCellController.statusSprite.enabled = true;
					countryCellController.countryName.color = GlobalColors.COUNTRY_POPUP_NORMAL_CELL_TEXT_COLOUR;
				}
				else
				{
					countryCellController.statusSprite.enabled = false;
					countryCellController.countryName.color = GlobalColors.COUNTRY_POPUP_NORMAL_CELL_TEXT_COLOUR;
				}
				return container2;
			}
		}

		[FormerlySerializedAs("countryListView")]
		[SerializeField]
		private ScrollView _countryListView;

		[FormerlySerializedAs("countryCell")]
		[SerializeField]
		private GameObject _countryCell;

		[FormerlySerializedAs("popupTitle")]
		[SerializeField]
		private UILabel _popupTitle;

		[FormerlySerializedAs("currentCountryFlag")]
		[SerializeField]
		private UISprite _currentCountryFlag;

		[FormerlySerializedAs("currentCountryName")]
		[SerializeField]
		private UILabel _currentCountryName;

		[FormerlySerializedAs("infoText")]
		[SerializeField]
		private UILabel _infoText;

		[FormerlySerializedAs("currentCountryLayout")]
		[SerializeField]
		private HorizontalLayout _currentCountryLayout;

		private CountryListAdapter _adapter;

		private bool _firstShow = true;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			_adapter = new CountryListAdapter(CountryManager.instance.GetListOfCountries(), _countryCell, this);
			_countryListView.SetAdapter(_adapter);
			_popupTitle.text = Strings.Get("SETTINGS_POPUP_COUNTRY_TITLE");
			_popupTitle.fontSize = 23;
			_infoText.text = Strings.Get("SETTINGS_POPUP_COUNTRY_DESCRIPTION");
			ColorData instance = ColorData.instance;
			_popupTitle.color = instance.midnight;
			_currentCountryName.color = instance.peacock;
			_infoText.color = instance.peacock;
		}

		protected override void Show()
		{
			UpdatePopupUI();
			if (_firstShow)
			{
				CoroutineUtil.StartCoroutine(DelayedSelect());
				_firstShow = false;
			}
			else
			{
				SelectCurrentCountry();
			}
		}

		private IEnumerator DelayedSelect()
		{
			yield return null;
			SelectCurrentCountry();
		}

		private void SelectCurrentCountry()
		{
			_countryListView.SetSelection(CountryManager.instance.GetCurrentCountryIndex());
		}

		protected override void Hide()
		{
			IManager manager = TopRunRoot.manager;
			if (manager != null)
			{
				manager.tournamentChanged += TournamentChanged;
				manager.RefreshTournament();
			}
		}

		private void TournamentChanged(object sender, TournamentChangedEventArgs eventArg)
		{
			IManager manager = TopRunRoot.manager;
			if (manager != null && eventArg.tournamentWasUpdated)
			{
				if (manager.tournament != null && manager.country != manager.tournament.country && manager.nextTournamentFetchTime < CDateTime.UtcNow && (manager.score == 0 || manager.tournament.hasResult))
				{
					manager.FetchNextTournament();
				}
				manager.tournamentChanged -= TournamentChanged;
			}
		}

		private void OnClickContainer(GameObject go)
		{
			Container container = (Container)go.GetComponent<UIEventListener>().parameter;
			CountryCellController countryCellController = (CountryCellController)container.controlComponent;
			TopRunRoot.manager.country = countryCellController.countryLocale.ToUpper();
			UpdatePopupUI();
			_countryListView.SetSelection(container.positionIndex, ContainerPivot.None, container.top - _countryListView.rootPanel.transform.localPosition.y);
		}

		private void UpdatePopupUI()
		{
			_currentCountryFlag.spriteName = CountryManager.instance.GetCurrentTournamentCountry().flag;
			_currentCountryName.text = Strings.Get(CountryManager.instance.GetCurrentTournamentCountry().displayNameID).ToUpper();
			_currentCountryLayout.Align();
		}
	}
}
