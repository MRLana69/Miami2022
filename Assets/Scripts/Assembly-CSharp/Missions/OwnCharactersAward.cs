using System;
using Data.Managers;
using SYBO.Subway.Info;

namespace Missions
{
	public class OwnCharactersAward : Award
	{
		public OwnCharactersAward(AwardStaticData awardData, PersistentAwardProgress savedProgress, Action<Award> handler)
			: base(awardData, savedProgress, handler)
		{
		}

		private int GetNumberOfOwnedCharacters()
		{
			int num = 0;
			foreach (Characters.CharacterId item in Characters.characterOrder)
			{
				if (CharacterManager.IsCharacterOwned(item))
				{
					num++;
				}
			}
			return num;
		}

		protected override int GetProgressForGoal(AwardGoal goal)
		{
			return GetNumberOfOwnedCharacters();
		}

		public override bool IsGoalAccomplished(AwardGoal goal)
		{
			AwardSimpleGoal awardSimpleGoal = goal as AwardSimpleGoal;
			if (awardSimpleGoal != null && GetProgressForGoal(goal) >= awardSimpleGoal.goal)
			{
				return true;
			}
			return false;
		}

		protected override void ActivateCurrentAwardTier()
		{
			CharacterManager.characterUnlocked += OnCharacterUnlocked;
			CheckAndUpdateProgress();
		}

		protected override void DeactivateCurrentAwardTier()
		{
			CharacterManager.characterUnlocked -= OnCharacterUnlocked;
		}

		protected override void RecordNewOffsets()
		{
		}

		private void OnCharacterUnlocked(Characters.CharacterId id, bool unlockedByRestore)
		{
			CheckAndUpdateProgress();
		}
	}
}
