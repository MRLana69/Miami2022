using System.Collections.Generic;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using UI.Generic.Layout;
using UI.Popups;
using UI.Popups.WelcomeBackRewardPopupInternal;
using UnityEngine;

public class WelcomeBackRewardPopup : UIPopup
{
	public enum PopupState
	{
		Default = 0,
		OnlyWelcomeBackBoxShown = 1,
		OnlyBonusOfferBoxShown = 2
	}

	private const int WELCOME_BACK_BOX_BOTTOM_PADDING = 4;

	private const int BONUS_OFFER_BOX_BOTTOM_PADDING = 2;

	private const int DEFAULT_BOTTOM_PADDING = 2;

	private const float DEFAULT_POPUP_Y_POSITION = -14.8f;

	private const float CENTER_POPUP_Y_POSITION = 0f;

	private const string AD_OPTIONS_FORMAT = "{0};{1}";

	private const string AD_OPTIONS_NOT_AVAILABLE_VALUE = "-1";

	[SerializeField]
	private Transform _content;

	[SerializeField]
	private VerticalLayout _contentLayout;

	[SerializeField]
	private WelcomeBackBox _welcomeBackBox;

	[SerializeField]
	private BonusOfferBox _bonusOfferBox;

	[SerializeField]
	private UIWidget _bottomPadding;

	[SerializeField]
	private UIWidget[] _widgetsToUpdateOnSizeChange;

	private PopupState _currentState;

	protected override void Init()
	{
		_welcomeBackBox.OnItemGiven += OnWelcomeBackItemGiven;
		_welcomeBackBox.OnButtonClicked += OnWelcomeBackButtonClicked;
		_bonusOfferBox.OnItemGiven += OnBonusOfferGiven;
		_bonusOfferBox.OnButtonClicked += OnBonusOfferButtonClicked;
	}

	private void OnDestroy()
	{
		_welcomeBackBox.OnItemGiven -= OnWelcomeBackItemGiven;
		_bonusOfferBox.OnItemGiven -= OnBonusOfferGiven;
		_bonusOfferBox.OnItemGiven -= OnBonusOfferGiven;
		_bonusOfferBox.OnButtonClicked -= OnBonusOfferButtonClicked;
	}

	protected override void Show()
	{
		if (Globals.DEBUG_WELCOME_BACK_REWARD_POPUP_ENABLED)
		{
			_currentState = Globals.DEBUG_WELCOME_BACK_REWARD_DEBUG_STATE;
		}
		SwitchToState(_currentState);
	}

	private void OnBonusOfferGiven()
	{
		CheckIfShouldClosePopup();
		SwitchToState(PopupState.OnlyWelcomeBackBoxShown);
	}

	private void OnWelcomeBackItemGiven()
	{
		CheckIfShouldClosePopup();
		SwitchToState(PopupState.OnlyBonusOfferBoxShown);
	}

	private void OnWelcomeBackButtonClicked(WelcomeBackRewardItemPayload buttonPayload)
	{
		LogCurrentStatus(_welcomeBackBox.GetWelcomeBackItemDescription());
	}

	private void OnBonusOfferButtonClicked(WelcomeBackRewardItemPayload buttonPayload)
	{
		if (buttonPayload.requiresWatchingAdForItem)
		{
			LogCurrentStatus(_bonusOfferBox.GetBonusOfferAdItemDescription());
		}
	}

	private void CheckIfShouldClosePopup()
	{
		if (_currentState == PopupState.OnlyWelcomeBackBoxShown || _currentState == PopupState.OnlyBonusOfferBoxShown)
		{
			ClosePopup();
		}
	}

	private void SwitchToState(PopupState state)
	{
		switch (state)
		{
		case PopupState.Default:
			_welcomeBackBox.Show();
			_bonusOfferBox.Show();
			_bottomPadding.height = 2;
			ResetToDefaultPosition();
			break;
		case PopupState.OnlyWelcomeBackBoxShown:
			_welcomeBackBox.Show();
			_bonusOfferBox.Hide();
			_bottomPadding.height = 4;
			CenterPopup();
			break;
		case PopupState.OnlyBonusOfferBoxShown:
			_bonusOfferBox.Show();
			_welcomeBackBox.Hide();
			_bottomPadding.height = 2;
			CenterPopup();
			break;
		}
		_currentState = state;
		UpdatePopupWidgets();
	}

	private void ResetToDefaultPosition()
	{
		_content.SetLocalPosY(-14.8f);
	}

	private void CenterPopup()
	{
		_content.SetLocalPosY(0f);
	}

	private void UpdatePopupWidgets()
	{
		_contentLayout.Align();
		UIWidget[] widgetsToUpdateOnSizeChange = _widgetsToUpdateOnSizeChange;
		foreach (UIWidget uIWidget in widgetsToUpdateOnSizeChange)
		{
			uIWidget.UpdateAnchors();
		}
	}

	[UsedImplicitly]
	public void OnCloseButtonClicked()
	{
		LogCurrentStatus("Closed");
		ClosePopup();
	}

	public override void ClosePopup()
	{
		base.ClosePopup();
		_currentState = PopupState.Default;
	}

	public void LogCurrentStatus(string result)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("HasAd", AdSystemManager.IsCached(AdLocations.WELCOME_BACK_VIDEOS).ToString());
		dictionary.Add("AdOptions", GetAdOptionsAvailable());
		dictionary.Add("Result", result);
		Dictionary<string, string> parameters = dictionary;
		Flurry.LogEventWithSeveralParameters("WelcomeBackPopUp", parameters);
	}

	private string GetAdOptionsAvailable()
	{
		string arg = _welcomeBackBox.GetWelcomeBackItemDescription();
		string arg2 = _bonusOfferBox.GetBonusOfferAdItemDescription();
		switch (_currentState)
		{
		case PopupState.OnlyWelcomeBackBoxShown:
			arg2 = "-1";
			break;
		case PopupState.OnlyBonusOfferBoxShown:
			arg = "-1";
			break;
		}
		return string.Format("{0};{1}", arg2, arg);
	}
}
