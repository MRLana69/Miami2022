using System;

public class CharacterModifierCollectionChangedEventArgs : EventArgs
{
	private CharacterModifierCollectionChangeType _characterModifierCollectionChangeType;

	private CharacterModifier _characterModifier;

	public CharacterModifierCollectionChangeType CharacterModifierCollectionChangeType
	{
		get
		{
			return _characterModifierCollectionChangeType;
		}
	}

	public CharacterModifier CharacterModifier
	{
		get
		{
			return _characterModifier;
		}
	}

	public CharacterModifierCollectionChangedEventArgs(CharacterModifierCollectionChangeType characterModifierCollectionChangeType, CharacterModifier characterModifier)
	{
		_characterModifierCollectionChangeType = characterModifierCollectionChangeType;
		_characterModifier = characterModifier;
	}
}
