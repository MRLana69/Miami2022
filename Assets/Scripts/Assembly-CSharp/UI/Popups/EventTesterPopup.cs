using System;
using System.Collections.Generic;
using System.Linq;
using Info;
using JetBrains.Annotations;
using SYBO.Subway.Info;
using UI.Screens;
using UnityEngine;

namespace UI.Popups
{
	public class EventTesterPopup : UIPopup
	{
		[SerializeField]
		private SelectPopupButton _eventTypePopupButton;

		[SerializeField]
		private SelectPopupButton _charPopupButton;

		[SerializeField]
		private SelectPopupButton _boardPopupButton;

		[SerializeField]
		private SelectPopupButton _hoursLeftPopupButton;

		[SerializeField]
		private SelectPopupButton _eventCategoryPopupButton;

		[SerializeField]
		private GameObject boardTypeButton;

		[SerializeField]
		private GameObject characterTypeButton;

		[SerializeField]
		private GameObject _eventCategoryButton;

		[SerializeField]
		private GameObject enableButton;

		[SerializeField]
		private GameObject disableButton;

		[SerializeField]
		private GameObject _updateButton;

		private string _hoursLeft;

		private string _charType;

		private string _boardType;

		private string _eventType;

		private string _eventCategory;

		protected override void Init()
		{
			EventDelegate.Add(_eventTypePopupButton.onChange, EventTypeSelected);
			_eventTypePopupButton.SetItems(GetEventTypes());
			List<string> items = Enum.GetNames(typeof(Characters.CharacterId)).ToList();
			EventDelegate.Add(_charPopupButton.onChange, CharSelected);
			_charPopupButton.SetItems(items);
			List<string> items2 = Boards.GetAllBoardNames().ToList();
			EventDelegate.Add(_boardPopupButton.onChange, BoardSelected);
			_boardPopupButton.SetItems(items2);
			List<string> items3 = Enum.GetNames(typeof(SSEventCategory)).ToList();
			EventDelegate.Add(_eventCategoryPopupButton.onChange, EventCategorySelected);
			_eventCategoryPopupButton.SetItems(items3);
			EventDelegate.Add(_hoursLeftPopupButton.onChange, NewTimeLimitSelected);
		}

		private List<string> GetEventTypes()
		{
			SSEventType[] array = Enum.GetValues(typeof(SSEventType)) as SSEventType[];
			List<string> list = new List<string>();
			if (array == null)
			{
				return list;
			}
			SSEventType[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				SSEventType sSEventType = array2[i];
				string text = sSEventType.ToString();
				switch (sSEventType)
				{
				case SSEventType.SpecialPowerBoard:
				case SSEventType.ExpiredLimitedCharacter:
				case SSEventType.ExpiredLimitedBoard:
					text += "(Wed)";
					break;
				case SSEventType.SuperMysteryBox:
					text += "(Fri)";
					break;
				case SSEventType.BiggerJackpot:
					text += "(Tue)";
					break;
				case SSEventType.MysteryPowerup:
					text += "(Mon)";
					break;
				case SSEventType.CharacterDay:
					text += "(Thu)";
					break;
				case SSEventType.CharacterName:
					text += "(Sat/Sun)";
					break;
				}
				list.Add(text);
			}
			return list;
		}

		public void EventTypeSelected()
		{
			characterTypeButton.SetActive(false);
			boardTypeButton.SetActive(false);
			_eventCategoryButton.SetActive(false);
			string eventType = _eventType;
			if (_eventTypePopupButton != null && !string.IsNullOrEmpty(_eventTypePopupButton.value))
			{
				_eventType = _eventTypePopupButton.value.Split('(')[0];
				switch (_eventType)
				{
				case "CharacterDay":
				case "ExpiredLimitedCharacter":
					characterTypeButton.SetActive(true);
					break;
				case "ExpiredLimitedBoard":
				case "SpecialPowerBoard":
					boardTypeButton.SetActive(true);
					break;
				case "CharacterName":
				case "BiggerJackpot":
					_eventCategoryButton.SetActive(true);
					break;
				}
				CheckDirty(eventType, _eventType);
			}
		}

		public void CharSelected()
		{
			string charType = _charType;
			_charType = _charPopupButton.value;
			CheckDirty(charType, _charType);
		}

		public void BoardSelected()
		{
			string boardType = _boardType;
			_boardType = _boardPopupButton.value;
			CheckDirty(boardType, _boardType);
		}

		public void EventCategorySelected()
		{
			string eventCategory = _eventCategory;
			_eventCategory = _eventCategoryPopupButton.value;
			CheckDirty(eventCategory, _eventCategory);
		}

		public void NewTimeLimitSelected()
		{
			string hoursLeft = _hoursLeft;
			_hoursLeft = _hoursLeftPopupButton.value;
			CheckDirty(hoursLeft, _hoursLeft);
		}

		private void CheckDirty(string originalValue, string newValue)
		{
			if (Globals.DEBUG_USE_EVENT_TESTER && originalValue != newValue)
			{
				_updateButton.SetActive(true);
				disableButton.SetActive(false);
			}
		}

		[UsedImplicitly]
		public void UpdateEvent()
		{
			DisableEvent();
			EnableEvent();
		}

		public void EnableEvent()
		{
			enableButton.SetActive(false);
			_updateButton.SetActive(false);
			disableButton.SetActive(true);
			Globals.DEBUG_USE_EVENT_TESTER = true;
			EventTesterManager.Instance.UpdateInfo(_eventType, _charType, _boardType, _eventCategory, _hoursLeft);
			BoardScreenManager.Instance.forceRefreshOnBoardScreen = true;
			CharacterScreen.forceRefreshOnCharacterScreen = true;
			OnlineSettings.instance.DownloadNow();
			UIScreenController.Instance.ClosePopup();
			UIScreenController.Instance.ClosePopup();
			UIScreenController.Instance.ClosePopup();
		}

		public void DisableEvent()
		{
			enableButton.SetActive(true);
			_updateButton.SetActive(false);
			disableButton.SetActive(false);
			Globals.DEBUG_USE_EVENT_TESTER = false;
			BoardScreenManager.Instance.forceRefreshOnBoardScreen = true;
			CharacterScreen.forceRefreshOnCharacterScreen = true;
			OnlineSettings.instance.DownloadNow();
			UIScreenController.Instance.ClosePopup();
			UIScreenController.Instance.ClosePopup();
			UIScreenController.Instance.ClosePopup();
		}
	}
}
