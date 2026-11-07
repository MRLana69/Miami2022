using System.Collections.Generic;
using Missions;

namespace Info
{
	public static class Trophies
	{
		public enum Trophy
		{
			diamond = 0,
			goldbar = 1,
			goldChainClock = 2,
			goldChainDollar = 3,
			goldSkull = 4,
			headphones = 5,
			lpBlack = 6,
			tapeBlack = 7
		}

		public const int TROPHY_COUNT = 8;

		public static readonly Dictionary<Trophy, TrophyData> TROPHY_DATA = new Dictionary<Trophy, TrophyData>
		{
			{
				Trophy.diamond,
				new TrophyData
				{
					name = "TROPHY_GEMSTONE",
					obtainSource = MysteryBox.Type.Super
				}
			},
			{
				Trophy.goldbar,
				new TrophyData
				{
					name = "TROPHY_GOLD_BAR",
					obtainSource = MysteryBox.Type.Super
				}
			},
			{
				Trophy.goldChainClock,
				new TrophyData
				{
					name = "TROPHY_CLASSY_CLOCK",
					obtainSource = MysteryBox.Type.Normal
				}
			},
			{
				Trophy.goldChainDollar,
				new TrophyData
				{
					name = "TROPHY_DOLLAR_CHAIN",
					obtainSource = MysteryBox.Type.Super
				}
			},
			{
				Trophy.goldSkull,
				new TrophyData
				{
					name = "TROPHY_PAPERWEIGHT",
					obtainSource = MysteryBox.Type.Super
				}
			},
			{
				Trophy.headphones,
				new TrophyData
				{
					name = "TROPHY_HEADPHONES",
					obtainSource = MysteryBox.Type.Normal
				}
			},
			{
				Trophy.lpBlack,
				new TrophyData
				{
					name = "TROPHY_PLATINUM_RECORD",
					obtainSource = MysteryBox.Type.Normal
				}
			},
			{
				Trophy.tapeBlack,
				new TrophyData
				{
					name = "TROPHY_CASSETTE",
					obtainSource = MysteryBox.Type.Normal
				}
			}
		};

		private static Stat StatForTrophyType(Trophy trophy)
		{
			switch (trophy)
			{
			case Trophy.diamond:
				return Stat.Diamond;
			case Trophy.goldbar:
				return Stat.Goldbar;
			case Trophy.goldChainClock:
				return Stat.GoldChainClock;
			case Trophy.goldChainDollar:
				return Stat.GoldChainDollar;
			case Trophy.goldSkull:
				return Stat.GoldSkull;
			case Trophy.headphones:
				return Stat.Headphones;
			case Trophy.lpBlack:
				return Stat.LpBlack;
			case Trophy.tapeBlack:
				return Stat.TapeBlack;
			default:
				return Stat.NotSet;
			}
		}

		public static void OldTrophySpawned(Trophy trophy)
		{
			Stat stat = StatForTrophyType(trophy);
			stat.IncrementValue();
		}

		public static bool ShouldSpawnTrophy(Trophy trophy)
		{
			Stat stat = StatForTrophyType(trophy);
			return AwardsManager.instance.ShouldSpawnOldTrophyForStat(stat);
		}
	}
}
