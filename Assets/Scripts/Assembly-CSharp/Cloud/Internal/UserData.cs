using System.Collections.Generic;
using Kiloo.Common.Backend.Cloud;

namespace Cloud.Internal
{
	public class UserData
	{
		public const string MULTIPLIER_KEY = "multiplier";

		public const string HIGHSCORE_KEY = "highscore";

		public const string KEYS_KEY = "keys";

		public const string UPGRADES_KEY = "upgrades";

		public const string AWARDS_KEY = "awards";

		public const string CHARACTERS_KEY = "characters";

		public const string BOARD_KEY = "board";

		public const string OUTFITS_KEY = "outfits";

		public const string BOARDTHEMES_KEY = "boardThemes";

		public int multiplier;

		public int highScore;

		public int keys;

		public int upgradeTiers;

		public int characters;

		public int outfits;

		public int boards;

		public int boardThemes;

		public float awardsPercentage;

		public UserData(ICloudUserData data)
		{
			if (data != null)
			{
				LoadUserData(data.ToDictionary());
			}
		}

		public bool StrictlyBetterThan(UserData other)
		{
			bool flag = true;
			flag &= multiplier >= other.multiplier;
			flag &= highScore >= other.highScore;
			flag &= keys >= other.keys;
			flag &= upgradeTiers >= other.upgradeTiers;
			flag &= characters >= other.characters;
			flag &= outfits >= other.outfits;
			flag &= boards >= other.boards;
			return flag & (boardThemes >= other.boardThemes);
		}

		private void LoadUserData(Dictionary<string, string> data)
		{
			multiplier = int.Parse(GetEntry(data, "multiplier"));
			highScore = int.Parse(GetEntry(data, "highscore"));
			keys = int.Parse(GetEntry(data, "keys"));
			upgradeTiers = int.Parse(GetEntry(data, "upgrades"));
			awardsPercentage = float.Parse(GetEntry(data, "awards"));
			characters = int.Parse(GetEntry(data, "characters"));
			boards = int.Parse(GetEntry(data, "board"));
			outfits = int.Parse(GetEntry(data, "outfits"));
			boardThemes = int.Parse(GetEntry(data, "boardThemes"));
		}

		public string AsString()
		{
			return string.Format("multiplier: {0} | highscore: {1} | keys: {2} | upgradeTiers: {3} | awardsPercentage: {4} | characters: {5} | boards: {6} | outfits: {7} | boardThemes: {8}", multiplier, highScore, keys, upgradeTiers, awardsPercentage, characters, boards, outfits, boardThemes);
		}

		private static string GetEntry(Dictionary<string, string> dict, string key)
		{
			return (!dict.ContainsKey(key)) ? "0" : dict[key];
		}
	}
}
