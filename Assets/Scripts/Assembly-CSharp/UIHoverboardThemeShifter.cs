using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Data.Managers;
using Data.Save;
using Info;
using Kiloo.Common.Extensions;
using UI.Screens.BoardScreenInternal;
using UnityEngine;

public class UIHoverboardThemeShifter : MonoBehaviour
{
	private const int MAX_NUMBER_OF_CUSTOM_THEMES = 3;

	[SerializeField]
	private GameObject _defaultHoverboardThemeButton;

	[SerializeField]
	private GameObject _customHoverboardThemeButton;

	[SerializeField]
	private Transform _selectedMarker;

	private Vector3 _distanceBetweenButtons = Vector3.zero;

	private List<HoverboardThemeButton> _customButtons = new List<HoverboardThemeButton>();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<int> OnClickedTheme__BackingField;

	private Boards.BoardId _cachedBoard;

	private int _cachedThemeIndex;

	private List<HoverboardThemes.HoverboardTheme> _currentBoardThemes = new List<HoverboardThemes.HoverboardTheme>();

	private HoverboardThemes.HoverboardTheme[] _sortedThemes;

	private BoardManager.ScreenContext _context;

	public event Action<int> OnClickedTheme
	{
		add
		{
			Action<int> action = OnClickedTheme__BackingField;
			Action<int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnClickedTheme__BackingField, (Action<int>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<int> action = OnClickedTheme__BackingField;
			Action<int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnClickedTheme__BackingField, (Action<int>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	private void Awake()
	{
		Vector3 localPosition = _defaultHoverboardThemeButton.transform.localPosition;
		Vector3 localPosition2 = _customHoverboardThemeButton.transform.localPosition;
		_distanceBetweenButtons = localPosition2 - localPosition;
		_customButtons.Add(_customHoverboardThemeButton.GetComponent<HoverboardThemeButton>());
		_selectedMarker.GetComponent<UISprite>().color = GlobalColors.CHARACTERS_CUSTOMIZATION_BASE_SKIN_ICON;
		_defaultHoverboardThemeButton.GetComponent<HoverboardThemeButton>().OnPress += ClickedTheme;
		_customButtons[0].OnPress += ClickedTheme;
		_selectedMarker.transform.position = _defaultHoverboardThemeButton.transform.position;
	}

	public void Init(BoardManager.ScreenContext context)
	{
		_context = context;
	}

	private HoverboardThemes.HoverboardTheme[] SortThemes(List<HoverboardThemes.HoverboardTheme> themes)
	{
		HoverboardThemes.HoverboardTheme[] array = new HoverboardThemes.HoverboardTheme[themes.Count];
		List<HoverboardThemes.HoverboardTheme> list = new List<HoverboardThemes.HoverboardTheme>();
		list.AddRange(themes);
		int num = int.MaxValue;
		HoverboardThemes.HoverboardTheme hoverboardTheme = themes[0];
		for (int i = 0; i < themes.Count; i++)
		{
			foreach (HoverboardThemes.HoverboardTheme item in list)
			{
				if ((int)item.uiOrder < num)
				{
					num = (int)item.uiOrder;
					hoverboardTheme = item;
				}
			}
			array[i] = hoverboardTheme;
			list.Remove(hoverboardTheme);
			num = int.MaxValue;
		}
		return array;
	}

	public void MakeCustomThemesAvailable(List<HoverboardThemes.HoverboardTheme> themes)
	{
		base.gameObject.SetActive(true);
		_currentBoardThemes = themes;
		if (themes.Count > 0)
		{
			_sortedThemes = SortThemes(themes);
		}
		foreach (HoverboardThemeButton customButton in _customButtons)
		{
			customButton.gameObject.SetActive(false);
		}
		int num = 0;
		if (themes != null)
		{
			num = ((themes.Count <= 3) ? themes.Count : 3);
		}
		_defaultHoverboardThemeButton.GetComponent<HoverboardThemeButton>().InitButton(string.Empty, GlobalColors.HOVERBOARD_CUSTOMIZATION_BASE_SKIN_FILL, GlobalColors.HOVERBOARD_CUSTOMIZATION_BASE_SKIN_ICON, 0);
		for (int i = 0; i < num; i++)
		{
			if (_customButtons.Count > i)
			{
				HoverboardThemeButton hoverboardThemeButton = _customButtons[i];
				if (_sortedThemes != null && _sortedThemes.Length > i)
				{
					HoverboardThemes.HoverboardTheme hoverboardTheme = _sortedThemes[i];
					SetActiveBubbles(hoverboardThemeButton, hoverboardTheme, BoardManager.GetCurrentlyShownBoard(_context), themes.IndexOf(hoverboardTheme) + 1, _sortedThemes.Length);
					hoverboardThemeButton.InitButton(hoverboardTheme.iconName, hoverboardTheme.buttonBgColor, hoverboardTheme.buttonIconColor, themes.IndexOf(hoverboardTheme) + 1);
				}
				hoverboardThemeButton.gameObject.SetActive(true);
				continue;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(_customHoverboardThemeButton);
			HoverboardThemeButton component = gameObject.GetComponent<HoverboardThemeButton>();
			if (component != null)
			{
				component.transform.parent = base.gameObject.transform;
				component.transform.localPosition = _customHoverboardThemeButton.transform.localPosition + _distanceBetweenButtons * i;
				component.transform.localScale = Vector3.one;
				component.name = i + 1 + "BoardThemeBtn";
				if (_sortedThemes != null && _sortedThemes.Length > i)
				{
					HoverboardThemes.HoverboardTheme hoverboardTheme2 = _sortedThemes[i];
					SetActiveBubbles(component, hoverboardTheme2, BoardManager.GetCurrentlyShownBoard(_context), themes.IndexOf(hoverboardTheme2) + 1, _sortedThemes.Length);
					component.InitButton(hoverboardTheme2.iconName, hoverboardTheme2.buttonBgColor, hoverboardTheme2.buttonIconColor, themes.IndexOf(hoverboardTheme2) + 1);
				}
				_customButtons.Add(component);
				component.OnPress += ClickedTheme;
			}
		}
		UpdateThemeStates();
	}

	private void OnDisable()
	{
		foreach (HoverboardThemeButton customButton in _customButtons)
		{
			customButton.gameObject.SetActive(false);
		}
	}

	private void SetActiveBubbles(HoverboardThemeButton btn, HoverboardThemes.HoverboardTheme ht, Boards.BoardId boardId, int index, int themeCount)
	{
		bool enableGiftBuble;
		if (index != 0)
		{
			enableGiftBuble = (ht.unlockType == PurchaseData.UnlockType.Coins || ht.unlockType == PurchaseData.UnlockType.Keys) && ht.price == 0 && !BoardManager.IsThemeOwned(boardId, ht.identifier) && (_cachedThemeIndex == 0 || _cachedThemeIndex != btn.themeIndex);
		}
		else
		{
			Boards.Board board = Boards.boardData[boardId];
			enableGiftBuble = (board.unlockType == PurchaseData.UnlockType.Coins || board.unlockType == PurchaseData.UnlockType.Keys) && board.price == 0 && !BoardManager.IsBoardOwned(boardId);
		}
		bool enableNewBuble = !BoardManager.HasAllThemesBeenSeen(boardId) && index == themeCount && !UserSave.isFreshInstall && ht.isNewInThisUpdate;
		btn.SetActiveBubbles(enableGiftBuble, enableNewBuble);
	}

	private void UpdateSelector(int index)
	{
		if (index == 0)
		{
			_selectedMarker.transform.position = _defaultHoverboardThemeButton.transform.position;
		}
		else
		{
			_selectedMarker.transform.position = _customButtons[_currentBoardThemes.IndexOf(_sortedThemes[index - 1])].transform.position;
		}
	}

	public void DisableThemeShifter()
	{
		base.gameObject.SetActive(false);
	}

	public void UpdateThemeStates()
	{
		for (int i = 0; i < _customButtons.Count; i++)
		{
			HoverboardThemeButton.ThemeState currentState = ((!BoardManager.IsThemeOwned(_cachedBoard, HoverboardThemes.GetThemeIdentifier(_cachedBoard, i))) ? ((!BoardManager.IsBoardOwned(_cachedBoard)) ? ((!Boards.boardData[_cachedBoard].isPartOfBundle) ? HoverboardThemeButton.ThemeState.NotPurchaseable : HoverboardThemeButton.ThemeState.BundleState) : HoverboardThemeButton.ThemeState.NotOwned) : ((!BoardManager.IsThemeActive(_cachedBoard, HoverboardThemes.GetThemeIdentifier(_cachedBoard, i))) ? HoverboardThemeButton.ThemeState.OwnedAndInactive : HoverboardThemeButton.ThemeState.OwnedAndActive));
			_customButtons[i].SetCurrentState(currentState);
		}
	}

	public void ClickedTheme(int index)
	{
		_cachedBoard = BoardManager.GetCurrentlyShownBoard(_context);
		_cachedThemeIndex = index;
		UpdateSelector(index);
		if (index != 0)
		{
			BoardManager.SetThemeSeen(_cachedBoard, HoverboardThemes.GetThemeIdentifier(_cachedBoard, index - 1));
		}
		OnClickedTheme__BackingField.SafeInvoke(index);
	}

	public void ResetThemeSelection()
	{
		_cachedBoard = BoardManager.GetCurrentlyShownBoard(_context);
		_cachedThemeIndex = 0;
		UpdateSelector(0);
		UpdateThemeStates();
	}
}
