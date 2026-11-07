using System;
using ABTest;
using JetBrains.Annotations;
using NaughtyAttributes;
using SYBO.Components.Utilities;
using SYBO.Subway.Inputs;
using UnityEngine;

namespace UI.MessageCenter
{
	[AddComponentMenu("GUI/Change Screen Button")]
	public class UIButtonChangeScreen : UIBasicButton
	{
		public enum ScreenChangeType
		{
			PushScreen = 0,
			SwitchScreen = 1,
			BackToPrevious = 2,
			QueuePopup = 3,
			ClosePopup = 4,
			PushPopup = 5
		}

		public ScreenChangeType screenChangeType;

		[ShowIf("ShowScreens")]
		[Dropdown("_screens")]
		[SerializeField]
		private string _screenName;

		[ShowIf("ShowPopups")]
		[Dropdown("_popups")]
		[SerializeField]
		private string _popupName;

		private string[] _popups = UIScreens.POPUPS;

		private string[] _screens = UIScreens.SCREENS;

		public string PopupName
		{
			get
			{
				return _popupName;
			}
			set
			{
				_popupName = value;
			}
		}

		[Button(null)]
		[UsedImplicitly]
		private void UpdateAndSortLists()
		{
			_screens = UIScreens.SCREENS;
			_popups = UIScreens.POPUPS;
			Array.Sort(_screens);
			Array.Sort(_popups);
		}

		public string GetScreenOrPopupName()
		{
			return ShowScreens() ? _screenName : ((!ShowPopups()) ? string.Empty : _popupName);
		}

		[UsedImplicitly]
		private bool ShowScreens()
		{
			return screenChangeType == ScreenChangeType.PushScreen || screenChangeType == ScreenChangeType.SwitchScreen;
		}

		[UsedImplicitly]
		private bool ShowPopups()
		{
			return screenChangeType == ScreenChangeType.QueuePopup || screenChangeType == ScreenChangeType.PushPopup;
		}

		protected override void Send()
		{
			string screenOrPopupName = GetScreenOrPopupName();
			ABTestManager.CountPages(screenOrPopupName);
			switch (screenOrPopupName)
			{
			case "StorePopup":
			{
				string topScreenName = UIScreenController.Instance.GetTopScreenName();
				if (!string.IsNullOrEmpty(topScreenName))
				{
					Flurry.LogEventWithAParameter("More coins button clicked", "Screen Name", topScreenName);
				}
				else
				{
					Flurry.LogEvent("More coins button clicked");
				}
				break;
			}
			case "PauseUI":
				SingletonMonoBehaviour<InputManager>.Instance.WasButtonClicked = true;
				break;
			case "IngameUI":
			{
				string currentPopupName = UIScreenController.Instance.GetCurrentPopupName();
				if (currentPopupName == "ConsumablesPopup")
				{
					SingletonMonoBehaviour<InputManager>.Instance.WasButtonClicked = true;
				}
				break;
			}
			}
			if (base.enabled && base.gameObject.activeInHierarchy)
			{
				if (string.IsNullOrEmpty(screenOrPopupName) && (screenChangeType == ScreenChangeType.PushScreen || screenChangeType == ScreenChangeType.SwitchScreen || screenChangeType == ScreenChangeType.QueuePopup))
				{
					SDebug.LogError(base.name + " tried to send an empty Change Screen message");
				}
				switch (screenChangeType)
				{
				case ScreenChangeType.PushScreen:
					UIScreenController.Instance.PushScreen(screenOrPopupName);
					break;
				case ScreenChangeType.SwitchScreen:
					UIScreenController.Instance.SwitchScreen(screenOrPopupName);
					break;
				case ScreenChangeType.BackToPrevious:
					UIScreenController.Instance.BackToPreviousScreen();
					break;
				case ScreenChangeType.QueuePopup:
					UIScreenController.Instance.QueuePopup(screenOrPopupName);
					break;
				case ScreenChangeType.PushPopup:
					UIScreenController.Instance.PushPopup(screenOrPopupName);
					break;
				case ScreenChangeType.ClosePopup:
					UIScreenController.Instance.ClosePopup();
					break;
				}
			}
		}
	}
}
