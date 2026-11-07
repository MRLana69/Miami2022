using System.Collections.Generic;
using SYBO.Subway.Info;

public class CharacterNameEventData
{
	public Characters.CharacterId currentCharacter;

	public int collectedLetterIndex = -1;

	public int skippedCharacters;

	public bool wasLastCharacterCompleted;

	public int completedCharacters;

	public List<Characters.CharacterId> charactersList = new List<Characters.CharacterId>();

	public List<string> categoryWordsList = new List<string>();

	public long eventID;

	public SSEventCategory eventCategory = SSEventCategory.Cat1;

	public CharacterNameEventData(List<Characters.CharacterId> charactersList, List<string> categoryWordsList, SSEventCategory eventCategory, Characters.CharacterId currentCharacter, int collectedLetterIndex, int completedCharacters, int skippedCharacters, bool wasLastCharacterCompleted, long eventID)
	{
		this.charactersList = charactersList;
		this.categoryWordsList = categoryWordsList;
		this.eventCategory = eventCategory;
		this.currentCharacter = currentCharacter;
		this.collectedLetterIndex = collectedLetterIndex;
		this.completedCharacters = completedCharacters;
		this.skippedCharacters = skippedCharacters;
		this.wasLastCharacterCompleted = wasLastCharacterCompleted;
		this.eventID = eventID;
	}

	public CharacterNameEventData()
	{
	}
}
