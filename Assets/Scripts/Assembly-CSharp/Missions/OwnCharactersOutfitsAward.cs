using System;
using System.Collections.Generic;
using Data.Managers;
using Info;
using SYBO.Subway.Info;

namespace Missions
{
	public class OwnCharactersOutfitsAward : Award
	{
		public OwnCharactersOutfitsAward(AwardStaticData awardData, PersistentAwardProgress savedProgress, Action<Award> handler)
			: base(awardData, savedProgress, handler)
		{
		}

		private int GetNumberOfOwnedCharacterOutfits()
		{
			int num = 0;
			foreach (Characters.CharacterId item in Characters.characterOrder)
			{
				List<Outfit> list = Outfits.TryGetOutfitsForCharacter(item);
				if (list == null || list.Count <= 0)
				{
					continue;
				}
				for (int i = 1; i <= list.Count; i++)
				{
					if (CharacterManager.IsOutfitUnlocked(item, i))
					{
						num++;
					}
				}
			}
			return num;
		}

		protected override int GetProgressForGoal(AwardGoal goal)
		{
			return GetNumberOfOwnedCharacterOutfits();
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
			CharacterManager.outfitUnlocked += OnOutfitUnlocked;
			CheckAndUpdateProgress();
		}

		protected override void DeactivateCurrentAwardTier()
		{
			CharacterManager.outfitUnlocked -= OnOutfitUnlocked;
		}

		private void OnOutfitUnlocked(Characters.CharacterId id, int outfitIndex)
		{
			CheckAndUpdateProgress();
		}

		protected override void RecordNewOffsets()
		{
		}
	}
}
