using System.Collections.Generic;
using Data.Managers;
using Info;
using SYBO.Subway.Info;
using UI.Screens;
using UnityEngine;

public class CharacterOwnageDebugCellHelper : MonoBehaviour
{
	[SerializeField]
	private UILabel _title;

	[SerializeField]
	private UILabel _description;

	[SerializeField]
	private GameObject _offIcon;

	[SerializeField]
	private GameObject _onIcon;

	private bool _currentIconState;

	private KeyValuePair<Characters.CharacterId, Characters.Model> _character;

	public void Init(KeyValuePair<Characters.CharacterId, Characters.Model> character)
	{
		_character = character;
		_title.text = Strings.Get(character.Value.name);
		string text = Strings.Get(character.Value.characterLimitedDescription);
		_description.text = (string.IsNullOrEmpty(text) ? "Default" : text);
		_currentIconState = CharacterManager.IsCharacterOwned(character.Key);
		if (character.Value.unlockType != PurchaseData.UnlockType.AlwaysUnlocked)
		{
			_offIcon.SetActive(!_currentIconState);
			_onIcon.SetActive(_currentIconState);
		}
		else
		{
			_offIcon.SetActive(false);
			_onIcon.SetActive(false);
		}
	}

	public void ToggleActive()
	{
		_currentIconState = !_currentIconState;
		_offIcon.SetActive(!_currentIconState);
		_onIcon.SetActive(_currentIconState);
		CharacterManager.CollectToken(_character.Key, _character.Value.Price * (_currentIconState ? 1 : (-1)));
		CharacterScreen.forceRefreshOnCharacterScreen = true;
	}
}
