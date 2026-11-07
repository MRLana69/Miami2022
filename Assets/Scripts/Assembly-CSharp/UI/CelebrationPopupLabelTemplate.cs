using System.Collections.Generic;
using Data.Managers;
using Data.Save;
using Info;
using Kiloo.Common.Extensions;
using Missions;
using SYBO.Subway.Info;
using UI.Generic.Layout;
using UI.Popups.BoardBundlePopupInternal;
using UI.Screens.RewardManagement;
using UnityEngine;
using Utilities;

namespace UI
{
	public class CelebrationPopupLabelTemplate : MonoBehaviour
	{
		public UILabel bigLabel;

		public UILabel subLabel;

		[SerializeField]
		private GameObject normalStateGO;

		[SerializeField]
		private GameObject megaJackpotStateGO;

		[SerializeField]
		private GameObject awardStateGO;

		[SerializeField]
		private GameObject topRunStateGO;

		[SerializeField]
		private GameObject characterEventStateGO;

		[SerializeField]
		private GameObject doubleUpEventStateGO;

		[SerializeField]
		private GameObject boardThemesObjct;

		[SerializeField]
		private UILabel jackpotBigLabel;

		[SerializeField]
		private UILabel jackpotSubLabel;

		[SerializeField]
		private UILabel awardTopDescriptionLabel;

		[SerializeField]
		private UILabel awardBigLabel;

		[SerializeField]
		private UILabel awardBottomDescriptionLabel;

		[SerializeField]
		private UILabel doubleUpTitleLabel;

		[SerializeField]
		private UILabel doubleUpSubLabel;

		public UILabel doubleUpItemLabel;

		[SerializeField]
		private UILabel doubleUpDescribtionLabel;

		[SerializeField]
		private UILabel topRunTopDescriptionLabel;

		[SerializeField]
		private UILabel topRunBigLabel;

		[SerializeField]
		private UILabel topRunScore;

		[SerializeField]
		private UILabel topRunWeekNr;

		[SerializeField]
		private UILabel topRunCountry;

		[SerializeField]
		private UISprite topRunCountryFlag;

		[SerializeField]
		private Layout _topRunCountryLayout;

		[SerializeField]
		private UILabel topRunCheckMarkText;

		[SerializeField]
		private UILabel characterEventWordNumber;

		[SerializeField]
		private UILabel characterEventWordNumberShadow;

		[SerializeField]
		private UILabel characterEventWord;

		[SerializeField]
		private HorizontalLayout themeLayout;

		[SerializeField]
		private BundleIcon bundleIcon1;

		[SerializeField]
		private BundleIcon bundleIcon2;

		public float Alpha
		{
			get
			{
				return bigLabel.alpha;
			}
			set
			{
				bigLabel.alpha = Mathf.Clamp01(value);
			}
		}

		private void OnEnable()
		{
			normalStateGO.SetActive(false);
			megaJackpotStateGO.SetActive(false);
			awardStateGO.SetActive(false);
			topRunStateGO.SetActive(false);
			characterEventStateGO.SetActive(false);
			doubleUpEventStateGO.SetActive(false);
			boardThemesObjct.SetActive(false);
		}

		public void Init(int backgroundDepth)
		{
			UIPanel component = base.gameObject.GetComponent<UIPanel>();
			component.depth = backgroundDepth + 1;
			if (SafeArea.IsRestricted)
			{
				doubleUpTitleLabel.topAnchor.absolute -= SafeArea.OFFSET_TOP + SafeArea.OFFSET_BOTTOM;
				doubleUpTitleLabel.bottomAnchor.absolute -= SafeArea.OFFSET_TOP + SafeArea.OFFSET_BOTTOM;
			}
		}

		public void SetupPowerup(UpgradeType upgrade, int amount, bool doubled = false)
		{
			normalStateGO.SetActive(true);
			Alpha = ((!doubled) ? 0f : 1f);
			bigLabel.text = _GetPowerupLabel(upgrade, amount);
			SetMaxLineCountNicely(bigLabel, 1);
			subLabel.text = Strings.Get(Upgrades.upgrades[upgrade].mysteryBoxDescription);
			SetMaxLineCountNicely(subLabel, 1);
		}

		public void SetupCoins()
		{
			normalStateGO.SetActive(true);
			Alpha = 0f;
			bigLabel.text = string.Empty;
			SetMaxLineCountNicely(bigLabel, 1);
			subLabel.text = string.Empty;
		}

		public void UpdateCoins(int amount, bool isMegaJackpot = false, bool doubled = false)
		{
			if (doubled)
			{
				megaJackpotStateGO.SetActive(false);
				doubleUpItemLabel.text = _GetCoinsLabel(amount);
				subLabel.text = string.Empty;
			}
			else if (isMegaJackpot)
			{
				normalStateGO.SetActive(false);
				megaJackpotStateGO.SetActive(true);
				jackpotBigLabel.text = Strings.Get("CELEBRATION_LABEL_TEMPLATE_MEGA_JACKPOT");
				jackpotSubLabel.text = _GetCoinsLabel(amount);
			}
			else
			{
				normalStateGO.SetActive(true);
				bigLabel.text = _GetCoinsLabel(amount);
				subLabel.text = string.Empty;
			}
		}

		public void SetupKeys(int amount, bool doubled = false)
		{
			normalStateGO.SetActive(true);
			Alpha = ((!doubled) ? 0f : 1f);
			bigLabel.text = _GetKeysLabel(amount);
			SetMaxLineCountNicely(bigLabel, 1);
			subLabel.text = string.Empty;
		}

		public void SetupToken(Characters.CharacterId characterId, int amount, bool doubled = false)
		{
			normalStateGO.SetActive(true);
			Alpha = ((!doubled) ? 0f : 1f);
			bigLabel.text = string.Format("{0}x {1}", amount, Strings.Get(Characters.characterData[characterId].tokenName));
			SetMaxLineCountNicely(bigLabel, 1);
			Characters.Model model = Characters.characterData[characterId];
			if (model.unlockType == PurchaseData.UnlockType.AlwaysUnlocked || InventorySave.unlockedCharacters.Contains(characterId))
			{
				List<Outfit> list = Outfits.TryGetOutfitsForCharacter(characterId);
				Outfit outfit = null;
				int nextTokenUnlockableOutfitIndex = Outfits.GetNextTokenUnlockableOutfitIndex(characterId);
				if (nextTokenUnlockableOutfitIndex >= 0)
				{
					outfit = list[nextTokenUnlockableOutfitIndex];
				}
				if (outfit != null)
				{
					int num = outfit.price - CharacterManager.GetCollectedTokens(characterId) - amount;
					if (num > 0)
					{
						subLabel.text = string.Format(Strings.Get("TOKEN_COLLECT_MORE_TO_UNLOCK"), num, Strings.Get(Characters.characterData[characterId].name), Strings.Get(outfit.title));
					}
					else
					{
						subLabel.text = string.Format(Strings.Get("TOKEN_UNLOCKED_OUTFIT"), Strings.Get(Characters.characterData[characterId].name), Strings.Get(outfit.title));
					}
				}
			}
			else
			{
				int num2 = model.Price - CharacterManager.GetCollectedTokens(characterId) - amount;
				if (num2 > 0)
				{
					subLabel.text = string.Format(Strings.Get("CELEBRATION_POPUP_COLLECT_MORE_TOKENS"), num2, Strings.Get(model.name));
				}
				else
				{
					subLabel.text = string.Format(Strings.Get("CELEBRATION_POPUP_YOU_UNLOCKED_TROPHY"), Strings.Get(model.name));
				}
			}
			SetMaxLineCountNicely(subLabel, 1);
		}

		public void SetupTrophy(Trophies.Trophy trophy)
		{
			normalStateGO.SetActive(true);
			Alpha = 0f;
			bigLabel.text = _GetTrophyLabel(trophy);
			SetMaxLineCountNicely(bigLabel, 1);
			subLabel.text = Strings.Get("CELEBRATION_POPUP_TROPHY_SUBLABEL_TEXT");
			SetMaxLineCountNicely(subLabel, 0);
		}

		public void SetupMedal()
		{
			normalStateGO.SetActive(true);
			Alpha = 0f;
			bigLabel.text = Strings.Get("CELEBRATION_POPUP_MEDAL_BIG_LABEL");
			SetMaxLineCountNicely(bigLabel, 1);
			subLabel.text = string.Empty;
		}

		public void SetupCharacter(string charName)
		{
			normalStateGO.SetActive(true);
			Alpha = 0f;
			bigLabel.text = string.Format(Strings.Get("CELEBRATION_POPUP_UNLOCK"), charName);
			SetMaxLineCountNicely(bigLabel, 0);
			subLabel.text = string.Empty;
		}

		public void SetupSpecialBoard(string boardName)
		{
			normalStateGO.SetActive(true);
			Alpha = 0f;
			bigLabel.text = string.Format(Strings.Get("CELEBRATION_POPUP_UNLOCK"), boardName);
			SetMaxLineCountNicely(bigLabel, 0);
			subLabel.text = string.Empty;
		}

		public void SetupBoardBundle(Boards.BoardId boardID, HoverboardThemes.HoverboardTheme theme1, HoverboardThemes.HoverboardTheme theme2)
		{
			SetupSpecialBoard(string.Format(Strings.Get("BOARD_DISPLAY_NAME_WITH_SUFFIX"), Strings.Get(Boards.boardData[boardID].name)));
			boardThemesObjct.SetActive(true);
			bundleIcon1.EnableOrDisableTheme(false);
			if (theme1 != null)
			{
				bundleIcon1.SetupTheme(theme1);
			}
			bundleIcon2.EnableOrDisableTheme(false);
			if (theme2 != null)
			{
				bundleIcon2.SetupTheme(theme2);
			}
			themeLayout.Align();
		}

		public void SetupEventSpecialBoard(string boardName)
		{
			normalStateGO.SetActive(true);
			Alpha = 0f;
			bigLabel.text = string.Format(Strings.Get("CELEBRATION_POPUP_EVENT_BOARD_TRYOUT"), boardName);
			SetMaxLineCountNicely(bigLabel, 0);
			subLabel.text = string.Empty;
		}

		public void SetupAward(string awardTitle, int rewardAmount, AwardTierType type)
		{
			Alpha = 0f;
			string text = string.Empty;
			switch (type)
			{
			case AwardTierType.Bronze:
				text = Strings.Get("AWARD_CELEBRATION_BRONZE");
				break;
			case AwardTierType.Silver:
				text = Strings.Get("AWARD_CELEBRATION_SILVER");
				break;
			case AwardTierType.Gold:
				text = Strings.Get("AWARD_CELEBRATION_GOLD");
				break;
			case AwardTierType.Diamond:
				text = Strings.Get("AWARD_CELEBRATION_DIAMOND");
				break;
			}
			normalStateGO.SetActive(false);
			awardStateGO.SetActive(true);
			topRunStateGO.SetActive(false);
			doubleUpEventStateGO.SetActive(false);
			awardTopDescriptionLabel.text = awardTitle;
			awardBigLabel.text = text;
			awardBottomDescriptionLabel.text = string.Format(Strings.Get("CELEBRATION_POPUP_AWARD_KEYS"), rewardAmount);
		}

		public void SetupTopRun(int score, int rank, int weekNumber)
		{
			Alpha = 0f;
			normalStateGO.SetActive(false);
			doubleUpEventStateGO.SetActive(false);
			topRunStateGO.SetActive(true);
			topRunTopDescriptionLabel.text = Strings.Get("TOPRUN_CELEBRATION_TITLE");
			topRunScore.text = score + string.Empty;
			topRunWeekNr.text = string.Format(Strings.Get("TOPRUN_WEEK_TITLE"), weekNumber);
			topRunCountryFlag.transform.gameObject.SetActive(true);
			topRunCountry.transform.gameObject.SetActive(true);
			topRunCountryFlag.spriteName = CountryManager.instance.GetCurrentTournamentCountry().flag;
			topRunCountry.text = Strings.Get(CountryManager.instance.GetCurrentTournamentCountry().displayNameID).ToUpper();
			_topRunCountryLayout.Align();
			topRunCheckMarkText.transform.gameObject.SetActive(false);
			switch (rank)
			{
			case 1:
				topRunBigLabel.text = Strings.Get("TOPRUN_RANK_1_TITLE");
				topRunBigLabel.fontSize = 40;
				return;
			case 2:
				topRunBigLabel.text = Strings.Get("TOPRUN_RANK_2_TITLE");
				topRunBigLabel.fontSize = 40;
				return;
			case 3:
				topRunBigLabel.text = Strings.Get("TOPRUN_RANK_3_TITLE");
				topRunBigLabel.fontSize = 40;
				return;
			case 4:
				topRunBigLabel.text = Strings.Get("TOPRUN_RANK_4_TITLE");
				topRunBigLabel.fontSize = 40;
				return;
			}
			topRunCheckMarkText.transform.gameObject.SetActive(true);
			topRunCheckMarkText.text = Strings.Get("TOPRUN_RANK_CHECKMARK_SUBTEXT");
			topRunBigLabel.text = Strings.Get("TOPRUN_RANK_CHECKMARK");
			topRunCountryFlag.transform.gameObject.SetActive(false);
			topRunCountry.transform.gameObject.SetActive(false);
			topRunBigLabel.fontSize = 30;
		}

		public void SetupMultiply(CelebrationReward reward)
		{
			Alpha = 0f;
			normalStateGO.SetActive(false);
			megaJackpotStateGO.SetActive(false);
			topRunStateGO.SetActive(false);
			doubleUpEventStateGO.SetActive(true);
			doubleUpDescribtionLabel.gameObject.SetActive(false);
			doubleUpTitleLabel.fontSize = 30;
			MysteryBox.Multipliers multiplierForReward = MysteryBox.GetMultiplierForReward(reward);
			switch (multiplierForReward)
			{
			case MysteryBox.Multipliers.x2:
				doubleUpTitleLabel.text = Strings.Get("DOUBLE_UP_CELEBRATION_SCREEN_TITLE");
				break;
			default:
				doubleUpTitleLabel.text = string.Empty;
				break;
			}
			doubleUpSubLabel.text = Strings.Get("DOUBLE_UP_CELEBRATION_SCREEN_SUBTITLE");
			if (reward.rewardType == CelebrationRewardType.coins)
			{
				doubleUpItemLabel.text = _GetCoinsLabel(reward.amount);
			}
			else if (reward.rewardType == CelebrationRewardType.token)
			{
				doubleUpDescribtionLabel.gameObject.SetActive(true);
				int num = reward.amount * (int)multiplierForReward;
				doubleUpItemLabel.text = string.Format("{0}x {1}", num, Strings.Get(Characters.characterData[reward.characterId].tokenName));
				Characters.Model model = Characters.characterData[reward.characterId];
				if (model.unlockType == PurchaseData.UnlockType.AlwaysUnlocked || InventorySave.unlockedCharacters.Contains(reward.characterId))
				{
					List<Outfit> list = Outfits.TryGetOutfitsForCharacter(reward.characterId);
					Outfit outfit = null;
					int nextTokenUnlockableOutfitIndex = Outfits.GetNextTokenUnlockableOutfitIndex(reward.characterId);
					if (nextTokenUnlockableOutfitIndex >= 0)
					{
						outfit = list[nextTokenUnlockableOutfitIndex];
					}
					if (outfit != null)
					{
						int num2 = outfit.price - CharacterManager.GetCollectedTokens(reward.characterId) - (num - reward.amount);
						if (num2 > 0)
						{
							doubleUpDescribtionLabel.text = string.Format(Strings.Get("TOKEN_COLLECT_MORE_TO_UNLOCK"), num2, Strings.Get(Characters.characterData[reward.characterId].name), Strings.Get(outfit.title));
						}
						else
						{
							doubleUpDescribtionLabel.text = string.Format(Strings.Get("TOKEN_UNLOCKED_OUTFIT"), Strings.Get(Characters.characterData[reward.characterId].name), Strings.Get(outfit.title));
						}
					}
				}
				else
				{
					int num3 = model.Price - CharacterManager.GetCollectedTokens(reward.characterId) - (num - reward.amount);
					if (num3 > 0)
					{
						doubleUpDescribtionLabel.text = string.Format(Strings.Get("CELEBRATION_POPUP_COLLECT_MORE_TOKENS"), num3, Strings.Get(model.name));
					}
					else
					{
						doubleUpDescribtionLabel.text = string.Format(Strings.Get("CELEBRATION_POPUP_YOU_UNLOCKED_TROPHY"), Strings.Get(model.name));
					}
				}
			}
			else if (reward.rewardType == CelebrationRewardType.keys)
			{
				doubleUpItemLabel.text = _GetKeysLabel(reward.amount * (int)multiplierForReward);
			}
			else if (reward.rewardType == CelebrationRewardType.powerup)
			{
				doubleUpItemLabel.text = _GetPowerupLabel(reward.upgradeType, reward.amount * (int)multiplierForReward);
				doubleUpDescribtionLabel.gameObject.SetActive(true);
				doubleUpDescribtionLabel.text = Strings.Get(Upgrades.upgrades[reward.upgradeType].mysteryBoxDescription);
			}
		}

		public void SetupCharacterWordEvent(int wordNumber, string word)
		{
			Alpha = 0f;
			normalStateGO.SetActive(false);
			doubleUpEventStateGO.SetActive(false);
			characterEventStateGO.SetActive(true);
			string text = GlobalColors.CHARACTERWORDEVENT_CELEBRATION_BLUE.ToUIHex();
			string text2 = GlobalColors.CHARACTERWORDEVENT_CELEBRATION_WHITE.ToUIHex();
			string text3 = GlobalColors.CHARACTERWORDEVENT_CELEBRATION_BLUESHADOW.ToUIHex();
			string text4 = GlobalColors.CHARACTERWORDEVENT_CELEBRATION_WHITESHADOW.ToUIHex();
			string eventLabelPrefix = GetEventLabelPrefix();
			string format = Strings.Get("CELEBRATION_LABEL_TEMPLATE_CHARACTEREVENT_COMPLETED");
			characterEventWordNumber.text = string.Format(format, text, eventLabelPrefix, text2, wordNumber, text);
			characterEventWordNumberShadow.text = string.Format(format, text4, eventLabelPrefix, text3, wordNumber, text4);
			string text5 = string.Empty;
			foreach (char c in word)
			{
				text5 = text5 + c + " ";
			}
			string text6 = string.Empty;
			if (text5.Length > 0)
			{
				text6 = text5.Substring(0, text5.Length - 1);
			}
			characterEventWord.text = text6;
		}

		private string GetEventLabelPrefix()
		{
			CharacterNameEventManager characterNameEventManager = SSEventsManagerController.instance.GetActiveEventManager() as CharacterNameEventManager;
			if (characterNameEventManager != null)
			{
				switch (characterNameEventManager.GetEventCategory())
				{
				case SSEventCategory.Cat1:
					return Strings.Get("CELEBRATION_LABEL_TEMPLATE_CHARACTEREVENT_PART1_CATEGORY1");
				case SSEventCategory.Cat2:
					return Strings.Get("CELEBRATION_LABEL_TEMPLATE_CHARACTEREVENT_PART1_CATEGORY2");
				case SSEventCategory.Cat3:
					return Strings.Get("CELEBRATION_LABEL_TEMPLATE_CHARACTEREVENT_PART1_CATEGORY3");
				default:
					return Strings.Get("CELEBRATION_LABEL_TEMPLATE_CHARACTEREVENT_PART1");
				}
			}
			return Strings.Get("CELEBRATION_LABEL_TEMPLATE_CHARACTEREVENT_PART1");
		}

		private string _GetPowerupLabel(UpgradeType type, int amount)
		{
			string empty = string.Empty;
			empty = empty + amount + "x ";
			return empty + Strings.Get(Upgrades.upgrades[type].GetName());
		}

		private string _GetTrophyLabel(Trophies.Trophy trophy)
		{
			return Strings.Get(Trophies.TROPHY_DATA[trophy].name);
		}

		private string _GetCoinsLabel(int amount)
		{
			return string.Format(Strings.Get("CELEBRATION_POPUP_COINS_AMOUNT"), string.Format("{0:#,###0}", amount));
		}

		private string _GetKeysLabel(int amount)
		{
			string format = ((amount <= 1) ? Strings.Get("CELEBRATION_POPUP_KEY_AMOUNT") : Strings.Get("CELEBRATION_POPUP_KEYS_AMOUNT"));
			return string.Format(format, amount);
		}

		private void SetMaxLineCountNicely(UILabel label, int maxLineCount)
		{
			Vector2 localSize = label.localSize;
			int absolute = label.leftAnchor.absolute;
			int absolute2 = label.rightAnchor.absolute;
			label.maxLineCount = maxLineCount;
			label.width = (int)localSize.x;
			label.height = (int)localSize.y;
			label.leftAnchor.absolute = absolute;
			label.rightAnchor.absolute = absolute2;
		}
	}
}
