using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Data.Managers;
using Data.Save;
using Info;
using Kiloo.Common.Extensions;
using SYBO.Subway.Info;
using UI.Screens.CharacterScreenInternal;
using UnityEngine;

public class UICharacterThemeShifter : MonoBehaviour
{
	private const int MAX_NUMBER_OF_CUSTOM_THEMES = 3;

	[SerializeField]
	private GameObject _defaultCharacterThemeButton;

	[SerializeField]
	private GameObject _customCharacterThemeButton;

	[SerializeField]
	private Transform _selectedMarker;

	private Vector3 _distanceBetweenButtons = Vector3.zero;

	private List<CharacterThemeButton> _customButtons = new List<CharacterThemeButton>();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<int> OnThemeButtonPressed__BackingField;

	private int _activeTheme;

	private Characters.CharacterId _cachedCharacter;

	private List<Outfit> _currentCharacterThemes = new List<Outfit>();

	private Outfit[] _sortedOutfits;

	public event Action<int> OnThemeButtonPressed
	{
		add
		{
			Action<int> action = OnThemeButtonPressed__BackingField;
			Action<int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnThemeButtonPressed__BackingField, (Action<int>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<int> action = OnThemeButtonPressed__BackingField;
			Action<int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnThemeButtonPressed__BackingField, (Action<int>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public void InitValues(Characters.CharacterId charId, int index)
	{
		_cachedCharacter = charId;
		_activeTheme = index;
	}

	private void Awake()
	{
		Vector3 localPosition = _defaultCharacterThemeButton.transform.localPosition;
		Vector3 localPosition2 = _customCharacterThemeButton.transform.localPosition;
		_distanceBetweenButtons = localPosition2 - localPosition;
		_customButtons.Add(_customCharacterThemeButton.GetComponent<CharacterThemeButton>());
		_selectedMarker.GetComponent<UISprite>().color = GlobalColors.CHARACTERS_CUSTOMIZATION_BASE_SKIN_ICON;
		_defaultCharacterThemeButton.GetComponent<CharacterThemeButton>().OnPress += ThemeButtonPressed;
		_customButtons[0].OnPress += ThemeButtonPressed;
		_selectedMarker.transform.position = _defaultCharacterThemeButton.transform.position;
	}

	private Outfit[] SortThemes(List<Outfit> themes)
	{
		Outfit[] array = new Outfit[themes.Count];
		List<Outfit> list = new List<Outfit>();
		list.AddRange(themes);
		int num = int.MaxValue;
		Outfit outfit = themes[0];
		for (int i = 0; i < themes.Count; i++)
		{
			foreach (Outfit item in list)
			{
				if (item.uiPriority < num)
				{
					num = item.uiPriority;
					outfit = item;
				}
			}
			array[i] = outfit;
			list.Remove(outfit);
			num = int.MaxValue;
		}
		return array;
	}

	public void MakeCustomThemesAvailable(List<Outfit> themes)
	{
		_currentCharacterThemes = themes;
		if (themes.Count > 0)
		{
			_sortedOutfits = SortThemes(themes);
		}
		foreach (CharacterThemeButton customButton in _customButtons)
		{
			customButton.gameObject.SetActive(false);
		}
		int num = 0;
		if (themes != null)
		{
			num = ((themes.Count <= 3) ? themes.Count : 3);
		}
		_defaultCharacterThemeButton.GetComponent<CharacterThemeButton>().SetOutfitColors(GlobalColors.CHARACTERS_CUSTOMIZATION_BASE_SKIN_FILL, GlobalColors.CHARACTERS_CUSTOMIZATION_BASE_SKIN_ICON, 0);
		for (int i = 0; i < num; i++)
		{
			CharacterThemeButton characterThemeButton = null;
			if (_customButtons.Count > i)
			{
				characterThemeButton = _customButtons[i];
				characterThemeButton.gameObject.SetActive(true);
			}
			else
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(_customCharacterThemeButton);
				characterThemeButton = gameObject.GetComponent<CharacterThemeButton>();
				if (characterThemeButton != null)
				{
					characterThemeButton.transform.parent = base.gameObject.transform;
					characterThemeButton.transform.localPosition = _customCharacterThemeButton.transform.localPosition + _distanceBetweenButtons * i;
					characterThemeButton.transform.localScale = Vector3.one;
					characterThemeButton.name = i + 1 + "CharThemeBtn";
					_customButtons.Add(characterThemeButton);
					characterThemeButton.OnPress += ThemeButtonPressed;
				}
			}
			if (characterThemeButton != null && _sortedOutfits != null && _sortedOutfits.Length > i)
			{
				Outfit outfit = _sortedOutfits[i];
				int num2 = themes.IndexOf(outfit) + 1;
				Characters.CharacterId currentlyShownModel = UIModelController.Instance.currentlyShownModel;
				int outfitRequirementIndex = Outfits.GetOutfitRequirementIndex(currentlyShownModel, num2);
				if (outfitRequirementIndex == -1)
				{
					characterThemeButton.SetLockIconEnabled(!CharacterManager.IsCharacterOwned(currentlyShownModel));
				}
				else
				{
					characterThemeButton.SetLockIconEnabled(!CharacterManager.IsOutfitUnlocked(currentlyShownModel, num2) && !CharacterManager.IsOutfitUnlocked(currentlyShownModel, outfitRequirementIndex + 1));
				}
				SetActiveBubbles(characterThemeButton, outfit, currentlyShownModel, num2);
				characterThemeButton.SetOutfitColors(outfit.buttonBgColor, outfit.buttonIconColor, num2);
			}
		}
	}

	private void OnDisable()
	{
		foreach (CharacterThemeButton customButton in _customButtons)
		{
			customButton.gameObject.SetActive(false);
		}
	}

	private void SetActiveBubbles(CharacterThemeButton btn, Outfit ot, Characters.CharacterId id, int index)
	{
		bool flag = !CharacterStickerManager.HasThemeBeenSeenForCharacter(id, index) && !UserSave.isFreshInstall && ot.isNewInThisUpdate && ot.price != 0;
		bool flag2 = false;
		bool enableLimitedGiftBubble = false;
		if (!flag)
		{
			flag2 = !CharacterStickerManager.IsThemeUnlockedForCharacter(id, index) && ot.price == 0 && _activeTheme != btn.themeIndex;
			if (flag2 && Characters.characterData[id].hasOnlineSettings)
			{
				flag2 = false;
				enableLimitedGiftBubble = true;
			}
		}
		btn.SetActiveBubbles(flag2, flag, enableLimitedGiftBubble);
	}

	private void SelectButton(int index)
	{
		bool flag = index == 0;
		if (_selectedMarker != null)
		{
			if (flag)
			{
				_selectedMarker.transform.position = _defaultCharacterThemeButton.transform.position;
			}
			else
			{
				_selectedMarker.transform.position = _customButtons[_currentCharacterThemes.IndexOf(_sortedOutfits[index - 1])].transform.position;
			}
		}
	}

	private void ThemeButtonPressed(int index)
	{
		UpdateUIForCharacter(UIModelController.Instance.currentlyShownModel, index);
		OnThemeButtonPressed__BackingField.SafeInvoke(index);
	}

	public void UpdateUIForCharacter(Characters.CharacterId charId, int outfitIndex)
	{
		if (_activeTheme != outfitIndex || _cachedCharacter != charId)
		{
			_cachedCharacter = charId;
			_activeTheme = outfitIndex;
			UserSave.OutfitSeen(_cachedCharacter, outfitIndex);
			SelectButton(outfitIndex);
		}
	}
}
