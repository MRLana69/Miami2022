using System;
using System.Collections.Generic;
using System.Linq;
using Data.Managers;
using Info;
using Missions;
using SYBO.Subway.Info;
using UI.Popups;
using UI.Screens;
using UI.Screens.RewardManagement;
using UnityEngine;

public class CelebrationTesterPopup : UIPopup
{
	[SerializeField]
	private SelectPopupButton _originPopupButton;

	[SerializeField]
	private SelectPopupButton _typePopupButton;

	[SerializeField]
	private SelectPopupButton _powerupPopupButton;

	[SerializeField]
	private SelectPopupButton _charPopupButton;

	[SerializeField]
	private SelectPopupButton _boardPopupButton;

	[SerializeField]
	private SelectPopupButton _trophyPopupButton;

	[SerializeField]
	private SelectPopupButton _awardPopupButton;

	[SerializeField]
	private SelectPopupButton _amountPopupButton;

	[SerializeField]
	private SelectPopupButton _topRunRankPopupButton;

	[SerializeField]
	private SelectPopupButton _themesPopupButton;

	[SerializeField]
	private UILabel _buttonLabel;

	[SerializeField]
	private GameObject _typeButton;

	[SerializeField]
	private GameObject _amountButton;

	[SerializeField]
	private GameObject _charButton;

	[SerializeField]
	private GameObject _boardsButton;

	[SerializeField]
	private GameObject _powerupButtons;

	[SerializeField]
	private GameObject _trrankButton;

	[SerializeField]
	private GameObject _awardButton;

	[SerializeField]
	private GameObject _trophyButton;

	[SerializeField]
	private GameObject _themesButton;

	[SerializeField]
	private UITable _buttonTable;

	private static CelebrationReward _currentCelebrationReward = new CelebrationReward();

	protected override float screenOffset
	{
		get
		{
			return -13f;
		}
	}

	private void Start()
	{
		EventDelegate.Add(_originPopupButton.onChange, OriginSelected);
		_originPopupButton.SetItems(Enum.GetNames(typeof(CelebrationRewardOrigin)).ToList());
		EventDelegate.Add(_typePopupButton.onChange, TypeSelected);
		_typePopupButton.SetItems(Enum.GetNames(typeof(CelebrationRewardType)).ToList());
		EventDelegate.Add(_powerupPopupButton.onChange, PowerupSelected);
		_powerupPopupButton.SetItems(Enum.GetNames(typeof(UpgradeType)).ToList());
		EventDelegate.Add(_charPopupButton.onChange, CharSelected);
		_charPopupButton.SetItems(Enum.GetNames(typeof(Characters.CharacterId)).ToList());
		EventDelegate.Add(_boardPopupButton.onChange, BoardSelected);
		_boardPopupButton.SetItems(Boards.GetAllBoardNames().ToList());
		EventDelegate.Add(_trophyPopupButton.onChange, OldTrophySelected);
		_trophyPopupButton.SetItems(Enum.GetNames(typeof(Trophies.Trophy)).ToList());
		EventDelegate.Add(_awardPopupButton.onChange, AwardSelected);
		_awardPopupButton.SetItems(Enum.GetNames(typeof(AwardTierType)).ToList());
		EventDelegate.Add(_amountPopupButton.onChange, NewAmountSelected);
		EventDelegate.Add(_topRunRankPopupButton.onChange, NewRankSelected);
		EventDelegate.Add(_themesPopupButton.onChange, NewThemesSelected);
	}

	public override void ClosePopup()
	{
		UIScreenController.Instance.ClosePopup();
	}

	public void OriginSelected()
	{
		if (!string.IsNullOrEmpty(_originPopupButton.value))
		{
			string value = _originPopupButton.value;
			_currentCelebrationReward.CelebrationRewardOrigin = (CelebrationRewardOrigin)Enum.Parse(typeof(CelebrationRewardOrigin), value, true);
			_powerupButtons.SetActive(false);
			_typeButton.SetActive(false);
			_amountButton.SetActive(false);
			_boardsButton.SetActive(false);
			_charButton.SetActive(false);
			_trrankButton.SetActive(false);
			_awardButton.SetActive(false);
			_themesButton.SetActive(false);
			_trophyButton.SetActive(false);
			switch (_currentCelebrationReward.CelebrationRewardOrigin)
			{
			case CelebrationRewardOrigin.BoardUnlock:
				_currentCelebrationReward.rewardType = CelebrationRewardType.specialBoard;
				_boardsButton.SetActive(true);
				break;
			case CelebrationRewardOrigin.CharacterUnlock:
				_currentCelebrationReward.rewardType = CelebrationRewardType.character;
				_charButton.SetActive(true);
				break;
			case CelebrationRewardOrigin.TopRun:
				_currentCelebrationReward.rewardType = CelebrationRewardType.medal;
				_trrankButton.SetActive(true);
				break;
			case CelebrationRewardOrigin.Award:
				_awardButton.SetActive(true);
				break;
			case CelebrationRewardOrigin.NewHighScore:
				_currentCelebrationReward.rewardType = CelebrationRewardType.highscore;
				break;
			case CelebrationRewardOrigin.BundleUnlock:
				_currentCelebrationReward.rewardType = CelebrationRewardType.boardBundle;
				_currentCelebrationReward.boardId = Boards.BoardId.starterPackBoard01;
				_currentCelebrationReward.rewardType = CelebrationRewardType.welcomePackBoard;
				break;
			case CelebrationRewardOrigin.UnlockItem:
				_typeButton.SetActive(true);
				break;
			case CelebrationRewardOrigin.MysteryBox:
			case CelebrationRewardOrigin.SuperMysteryBox:
			case CelebrationRewardOrigin.WeeklyChallengeNormal:
			case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
			case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
			case CelebrationRewardOrigin.WeeklyChallengeHiddenPrize:
			case CelebrationRewardOrigin.MysteryBoxMini:
			case CelebrationRewardOrigin.CharacterEventSuperMysteryBox:
				_typeButton.SetActive(true);
				break;
			case CelebrationRewardOrigin.TokenMysteryBox:
				_currentCelebrationReward.rewardType = CelebrationRewardType.token;
				_amountButton.SetActive(true);
				_charButton.SetActive(true);
				break;
			case CelebrationRewardOrigin.BoardBundleUnlock:
				_currentCelebrationReward.rewardType = CelebrationRewardType.boardBundle;
				_boardsButton.SetActive(true);
				_themesButton.SetActive(true);
				break;
			}
			_buttonTable.Reposition();
		}
	}

	public void TypeSelected()
	{
		if (!string.IsNullOrEmpty(_typePopupButton.value))
		{
			string value = _typePopupButton.value;
			_currentCelebrationReward.rewardType = (CelebrationRewardType)Enum.Parse(typeof(CelebrationRewardType), value, true);
			_amountButton.SetActive(false);
			_charButton.SetActive(false);
			_boardsButton.SetActive(false);
			_trophyButton.SetActive(false);
			_powerupButtons.SetActive(false);
			switch (_currentCelebrationReward.rewardType)
			{
			case CelebrationRewardType.coins:
			case CelebrationRewardType.keys:
				_amountButton.SetActive(true);
				break;
			case CelebrationRewardType.powerup:
				_amountButton.SetActive(true);
				_powerupButtons.SetActive(true);
				break;
			case CelebrationRewardType.token:
				_charButton.SetActive(true);
				_amountButton.SetActive(true);
				break;
			case CelebrationRewardType.trophy:
				_trophyButton.SetActive(true);
				break;
			case CelebrationRewardType.character:
				_charButton.SetActive(true);
				break;
			case CelebrationRewardType.specialBoard:
			case CelebrationRewardType.boardBundle:
			case CelebrationRewardType.welcomePackBoard:
				_boardsButton.SetActive(true);
				break;
			}
			_buttonTable.Reposition();
		}
	}

	public void PowerupSelected()
	{
		string value = _powerupPopupButton.value;
		if (!string.IsNullOrEmpty(value))
		{
			_currentCelebrationReward.upgradeType = (UpgradeType)Enum.Parse(typeof(UpgradeType), value, true);
		}
		else
		{
			_currentCelebrationReward.upgradeType = UpgradeType._notset;
		}
	}

	public void CharSelected()
	{
		if (string.IsNullOrEmpty(_charPopupButton.value))
		{
			return;
		}
		string value = _charPopupButton.value;
		_currentCelebrationReward.characterId = (Characters.CharacterId)Enum.Parse(typeof(Characters.CharacterId), value, true);
		_currentCelebrationReward.characterOutfitIndex = 0;
		if (_currentCelebrationReward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterUnlock)
		{
			_themesButton.SetActive(true);
			_themesPopupButton.multiSelect = false;
			List<string> list = new List<string>();
			list.Add("None");
			List<string> list2 = list;
			if (Outfits.TryGetOutfitsForCharacter(_currentCelebrationReward.characterId) != null)
			{
				list2.AddRange((from x in Outfits.TryGetOutfitsForCharacter(_currentCelebrationReward.characterId)
					select x.title).ToList());
			}
			_themesPopupButton.SetItems(list2);
			_buttonTable.Reposition();
		}
		else
		{
			_currentCelebrationReward.characterOutfitIndex = 0;
			_themesButton.SetActive(false);
			_buttonTable.Reposition();
		}
	}

	public void BoardSelected()
	{
		if (string.IsNullOrEmpty(_boardPopupButton.value))
		{
			return;
		}
		string value = _boardPopupButton.value;
		_currentCelebrationReward.boardId = (Boards.BoardId)Enum.Parse(typeof(Boards.BoardId), value, true);
		if (HoverboardThemes.hoverboardCustomThemes.ContainsKey(_currentCelebrationReward.boardId))
		{
			if (_currentCelebrationReward.CelebrationRewardOrigin == CelebrationRewardOrigin.BoardBundleUnlock)
			{
				_themesButton.SetActive(true);
				_themesPopupButton.multiSelect = true;
				_themesPopupButton.SetItems(HoverboardThemes.hoverboardCustomThemes[_currentCelebrationReward.boardId].Select((HoverboardThemes.HoverboardTheme x) => x.identifier.ToString()).ToList());
				_buttonTable.Reposition();
			}
			else if (_currentCelebrationReward.CelebrationRewardOrigin == CelebrationRewardOrigin.BoardUnlock)
			{
				_themesButton.SetActive(true);
				_themesPopupButton.multiSelect = false;
				List<string> list = new List<string>();
				list.Add("None");
				List<string> list2 = list;
				list2.AddRange(HoverboardThemes.hoverboardCustomThemes[_currentCelebrationReward.boardId].Select((HoverboardThemes.HoverboardTheme x) => x.identifier.ToString()).ToList());
				_themesPopupButton.SetItems(list2);
				_buttonTable.Reposition();
			}
		}
		else
		{
			_currentCelebrationReward.boardPower = Hoverboard.BoardAbility.None;
			_themesButton.SetActive(false);
			_buttonTable.Reposition();
		}
	}

	public void OldTrophySelected()
	{
		if (!string.IsNullOrEmpty(_trophyPopupButton.value))
		{
			string value = _trophyPopupButton.value;
			_currentCelebrationReward.trophyType = (Trophies.Trophy)Enum.Parse(typeof(Trophies.Trophy), value, true);
		}
	}

	public void AwardSelected()
	{
		if (!string.IsNullOrEmpty(_awardPopupButton.value))
		{
			string value = _awardPopupButton.value;
			_currentCelebrationReward.awardTierType = (AwardTierType)Enum.Parse(typeof(AwardTierType), value, true);
		}
	}

	public void NewAmountSelected()
	{
		if (!string.IsNullOrEmpty(_amountPopupButton.value))
		{
			string value = _amountPopupButton.value;
			_currentCelebrationReward.amount = int.Parse(value);
		}
	}

	public void NewRankSelected()
	{
		if (!string.IsNullOrEmpty(_topRunRankPopupButton.value))
		{
			string value = _topRunRankPopupButton.value;
			_currentCelebrationReward.rank = int.Parse(value);
			_currentCelebrationReward.score = 123543510;
			_currentCelebrationReward.weekNumber = 12;
		}
	}

	public void NewThemesSelected()
	{
		if (_currentCelebrationReward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterUnlock)
		{
			if (_themesPopupButton.value == null || !(_themesPopupButton.value != "None"))
			{
				return;
			}
			if (_themesPopupButton.value != "None")
			{
				OutfitSet outfitSet = Outfits.instance.outfitSets.First((OutfitSet x) => x.characterId == _currentCelebrationReward.characterId);
				for (int num = 0; num < outfitSet.outfitList.Length; num++)
				{
					if (outfitSet.outfitList[num].title == _themesPopupButton.value)
					{
						_currentCelebrationReward.characterOutfitIndex = num + 1;
						break;
					}
				}
			}
			else
			{
				_currentCelebrationReward.characterOutfitIndex = 0;
			}
		}
		else if (_currentCelebrationReward.CelebrationRewardOrigin == CelebrationRewardOrigin.BoardUnlock)
		{
			if (_themesPopupButton.value != null)
			{
				if (_themesPopupButton.value != "None")
				{
					HoverboardThemes.ThemeIdentifier themeIdentifier = (HoverboardThemes.ThemeIdentifier)Enum.Parse(typeof(HoverboardThemes.ThemeIdentifier), _themesPopupButton.value, true);
					_currentCelebrationReward.boardPower = HoverboardThemes.TryGetCustomThemesForBoard(_currentCelebrationReward.boardId)[HoverboardThemes.GetIndexForTheme(_currentCelebrationReward.boardId, themeIdentifier) - 1].power;
				}
				else
				{
					_currentCelebrationReward.boardPower = Hoverboard.BoardAbility.None;
				}
			}
		}
		else
		{
			if (_currentCelebrationReward.CelebrationRewardOrigin != CelebrationRewardOrigin.BoardBundleUnlock)
			{
				return;
			}
			List<HoverboardThemes.ThemeIdentifier> list = new List<HoverboardThemes.ThemeIdentifier>();
			if (_themesPopupButton.multipleValues != null)
			{
				foreach (string value in _themesPopupButton.multipleValues.Values)
				{
					HoverboardThemes.ThemeIdentifier item = (HoverboardThemes.ThemeIdentifier)Enum.Parse(typeof(HoverboardThemes.ThemeIdentifier), value, true);
					list.Add(item);
				}
			}
			_currentCelebrationReward.bundledThemes = list;
		}
	}

	public void ShowDebugCelebration()
	{
		RewardDebugCelebrationReward();
		UIScreenController.Instance.PushCelebrationPopup();
	}

	public void ToggleDebugCelebration()
	{
		Globals.DEBUG_USE_CELEBRATION_TESTER = !Globals.DEBUG_USE_CELEBRATION_TESTER;
		_buttonLabel.text = ((!Globals.DEBUG_USE_CELEBRATION_TESTER) ? "Enable" : "Disable");
	}

	public static void RewardDebugCelebrationReward()
	{
		if (_currentCelebrationReward.rewardType == CelebrationRewardType.specialBoard || _currentCelebrationReward.rewardType == CelebrationRewardType.boardBundle)
		{
			HoverboardThemes.HoverboardTheme hoverboardTheme = null;
			if (_currentCelebrationReward.boardPower != Hoverboard.BoardAbility.None)
			{
				List<HoverboardThemes.HoverboardTheme> source = HoverboardThemes.hoverboardCustomThemes[_currentCelebrationReward.boardId];
				hoverboardTheme = source.First((HoverboardThemes.HoverboardTheme x) => x.power == _currentCelebrationReward.boardPower);
			}
			if (_currentCelebrationReward.boardPower == Hoverboard.BoardAbility.None && _currentCelebrationReward.bundledThemes.Count == 0)
			{
				BoardManager.UnlockBoard(_currentCelebrationReward.boardId);
				UIModelController.Instance.SelectCurrentlyShownBoard(_currentCelebrationReward.boardId);
			}
			else if (hoverboardTheme != null)
			{
				if (!BoardManager.IsBoardOwned(_currentCelebrationReward.boardId))
				{
					BoardManager.UnlockBoard(_currentCelebrationReward.boardId);
					UIModelController.Instance.SelectCurrentlyShownBoard(_currentCelebrationReward.boardId);
				}
				BoardManager.SetThemeAcquired(_currentCelebrationReward.boardId, hoverboardTheme.identifier);
				BoardManager.SetThemeActive(_currentCelebrationReward.boardId, hoverboardTheme.identifier, true);
			}
			else if (_currentCelebrationReward.bundledThemes.Count > 0)
			{
				if (!BoardManager.IsBoardOwned(_currentCelebrationReward.boardId))
				{
					BoardManager.UnlockBoard(_currentCelebrationReward.boardId);
					UIModelController.Instance.SelectCurrentlyShownBoard(_currentCelebrationReward.boardId);
				}
				foreach (HoverboardThemes.ThemeIdentifier bundledTheme in _currentCelebrationReward.bundledThemes)
				{
					BoardManager.SetThemeAcquired(_currentCelebrationReward.boardId, bundledTheme);
					BoardManager.SetThemeActive(_currentCelebrationReward.boardId, bundledTheme, true);
				}
			}
			BoardManager.ResetDisplayedCustomSets(_currentCelebrationReward.boardId);
			BoardScreenManager.Instance.forceRefreshOnBoardScreen = true;
		}
		else
		{
			if (_currentCelebrationReward.rewardType == CelebrationRewardType.character)
			{
				if (_currentCelebrationReward.characterOutfitIndex == 0)
				{
					CharacterManager.UnlockCharacter(_currentCelebrationReward.characterId);
				}
				else
				{
					CharacterManager.UnlockOutfit(_currentCelebrationReward.characterId, _currentCelebrationReward.characterOutfitIndex);
				}
				CharacterScreen.forceRefreshOnCharacterScreen = true;
				return;
			}
			if (_currentCelebrationReward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterEventSuperMysteryBox)
			{
				_currentCelebrationReward.characterEventCompleted = 1;
				_currentCelebrationReward.description = "JAKE";
			}
		}
		RewardManager.AddRewardToUnlock(_currentCelebrationReward);
	}
}
