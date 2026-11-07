using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Info;
using Missions;
using SYBO.Subway.Info;

namespace UI.Screens.RewardManagement
{
	public class CelebrationReward
	{
		public CelebrationRewardOrigin CelebrationRewardOrigin;

		public CelebrationRewardType rewardType;

		public UnlockItemSource unlockItemSource;

		public int amount;

		public Characters.CharacterId characterId;

		public int characterOutfitIndex;

		public Boards.BoardId boardId;

		public Trophies.Trophy trophyType;

		public UpgradeType upgradeType;

		public long Uid;

		public AwardTierType awardTierType;

		public string awardTitle;

		public int rank = 1;

		public int score;

		public int weekNumber;

		public Hoverboard.BoardAbility boardPower;

		public int characterEventCompleted;

		public string description = string.Empty;

		public List<HoverboardThemes.ThemeIdentifier> bundledThemes = new List<HoverboardThemes.ThemeIdentifier>();

		public string kilooAnalyticsSourceId;

		public ObtainableItemSource syboAnalyticsSource;

		public long syboAnalyticsSourceId = -1L;

		public string syboAnalyticsSourceMetaOrNull;

		[Obsolete("This should only be used for PlayerInfo migration purposes")]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(CelebrationRewardOrigin).Append(",");
			stringBuilder.Append(rewardType).Append(",");
			stringBuilder.Append(amount).Append(",");
			stringBuilder.Append(characterId).Append(",");
			stringBuilder.Append(characterOutfitIndex).Append(",");
			stringBuilder.Append(boardId).Append(",");
			stringBuilder.Append(trophyType).Append(",");
			stringBuilder.Append(upgradeType).Append(",");
			stringBuilder.Append(Uid).Append(",");
			stringBuilder.Append(awardTierType).Append(",");
			stringBuilder.Append(awardTitle).Append(",");
			stringBuilder.Append(rank).Append(",");
			stringBuilder.Append(score).Append(",");
			stringBuilder.Append(weekNumber).Append(",");
			stringBuilder.Append(boardPower).Append(",");
			stringBuilder.Append(characterEventCompleted).Append(",");
			stringBuilder.Append(description).Append(",");
			stringBuilder.Append(syboAnalyticsSource).Append(",");
			stringBuilder.Append(syboAnalyticsSourceId).Append(",");
			stringBuilder.Append(syboAnalyticsSourceMetaOrNull).Append(",");
			stringBuilder.Append(kilooAnalyticsSourceId).Append(",");
			stringBuilder.Append(string.Join(";", bundledThemes.Select((HoverboardThemes.ThemeIdentifier x) => x.ToString()).ToArray()));
			return stringBuilder.ToString();
		}

		[Obsolete("This should only be used for PlayerInfo migration purposes")]
		public void PopulateFromString(string rewardAsString)
		{
			ResetToDefaultValues();
			string[] array = rewardAsString.Split(',');
			try
			{
				if (array.Length >= 9)
				{
					CelebrationRewardOrigin = (CelebrationRewardOrigin)Enum.Parse(typeof(CelebrationRewardOrigin), array[0]);
					rewardType = (CelebrationRewardType)Enum.Parse(typeof(CelebrationRewardType), array[1]);
					amount = int.Parse(array[2]);
					characterId = (Characters.CharacterId)Enum.Parse(typeof(Characters.CharacterId), array[3]);
					characterOutfitIndex = int.Parse(array[4]);
					boardId = (Boards.BoardId)Enum.Parse(typeof(Boards.BoardId), array[5]);
					trophyType = (Trophies.Trophy)Enum.Parse(typeof(Trophies.Trophy), array[6]);
					upgradeType = (UpgradeType)Enum.Parse(typeof(UpgradeType), array[7]);
					Uid = long.Parse(array[8]);
				}
				if (array.Length >= 11)
				{
					awardTierType = (AwardTierType)Enum.Parse(typeof(AwardTierType), array[9]);
					awardTitle = array[10];
				}
				if (array.Length >= 14)
				{
					rank = int.Parse(array[11]);
					score = int.Parse(array[12]);
					weekNumber = int.Parse(array[13]);
				}
				if (array.Length >= 15)
				{
					boardPower = (Hoverboard.BoardAbility)Enum.Parse(typeof(Hoverboard.BoardAbility), array[14]);
				}
				if (array.Length >= 17)
				{
					characterEventCompleted = int.Parse(array[15]);
					description = array[16];
				}
				if (array.Length >= 20)
				{
					syboAnalyticsSource = (ObtainableItemSource)Enum.Parse(typeof(ObtainableItemSource), array[17]);
					syboAnalyticsSourceId = long.Parse(array[18]);
					syboAnalyticsSourceMetaOrNull = array[19];
				}
				if (array.Length >= 21)
				{
					kilooAnalyticsSourceId = array[20];
				}
				if (array.Length < 22)
				{
					return;
				}
				bundledThemes = new List<HoverboardThemes.ThemeIdentifier>();
				string[] array2 = array[21].Split(';');
				string[] array3 = array2;
				foreach (string text in array3)
				{
					if (text.Length > 0)
					{
						HoverboardThemes.ThemeIdentifier item = (HoverboardThemes.ThemeIdentifier)Enum.Parse(typeof(HoverboardThemes.ThemeIdentifier), text);
						bundledThemes.Add(item);
					}
				}
			}
			catch (Exception ex)
			{
				ResetToDefaultValues();
				SDebug.LogError("The string could not be converted to a reward!" + ex.Message);
			}
		}

		private void ResetToDefaultValues()
		{
			CelebrationRewardOrigin = CelebrationRewardOrigin.Notset;
			rewardType = CelebrationRewardType._notset;
			unlockItemSource = UnlockItemSource.NotSet;
			amount = 0;
			characterId = Characters.CharacterId.slick;
			characterOutfitIndex = 0;
			boardId = Boards.BoardId.normal;
			trophyType = Trophies.Trophy.diamond;
			upgradeType = UpgradeType._notset;
			Uid = 0L;
			awardTierType = AwardTierType.None;
			awardTitle = string.Empty;
			rank = 1;
			score = 0;
			weekNumber = 1;
			boardPower = Hoverboard.BoardAbility.None;
		}
	}
}
