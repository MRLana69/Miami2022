using System.Collections.Generic;
using Info;
using UI.Screens.RewardManagement;

namespace UI.Helpers
{
	public static class RewardToIcon
	{
		public class IconData
		{
			public string SpriteName;

			public UpgradeType UpgradeType;
		}

		private static Dictionary<CelebrationRewardType, string> _rewardData;

		private static IconData[] _upgradeIcons;

		static RewardToIcon()
		{
			_rewardData = new Dictionary<CelebrationRewardType, string>
			{
				{
					CelebrationRewardType.coins,
					"icon_coin_large"
				},
				{
					CelebrationRewardType.keys,
					"icon_key"
				},
				{
					CelebrationRewardType.powerup,
					"btn_close"
				}
			};
			_upgradeIcons = new IconData[3]
			{
				new IconData
				{
					UpgradeType = UpgradeType.headstart2000,
					SpriteName = "icon_upgrades_headstart2000"
				},
				new IconData
				{
					UpgradeType = UpgradeType.hoverboard,
					SpriteName = "icon_hoverboard_single_tiny"
				},
				new IconData
				{
					UpgradeType = UpgradeType.scorebooster,
					SpriteName = "icon_score_multiplier"
				}
			};
		}

		public static string GetSpriteNameFromType(CelebrationRewardType rewardType, UpgradeType upgradeType = UpgradeType._notset)
		{
			if (rewardType == CelebrationRewardType.powerup)
			{
				if (upgradeType == UpgradeType._notset)
				{
					SDebug.LogError("Upgrade type not set for reward type: " + rewardType);
				}
				for (int i = 0; i < _upgradeIcons.Length; i++)
				{
					if (_upgradeIcons[i].UpgradeType == upgradeType)
					{
						return _upgradeIcons[i].SpriteName;
					}
				}
			}
			string value;
			if (_rewardData.TryGetValue(rewardType, out value))
			{
				return value;
			}
			return "btn_close";
		}
	}
}
