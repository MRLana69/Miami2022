using Data.Save;
using Info;
using Missions;
using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInMissionSetHelper : UISlideIn
	{
		[SerializeField]
		private UILabel line1;

		[SerializeField]
		private UILabel lineReward;

		[SerializeField]
		private UISprite superMysteryBox;

		[SerializeField]
		private UISprite mysteryBox;

		[SerializeField]
		private UISprite coinIcon;

		[SerializeField]
		private UILabel coinLabel;

		[SerializeField]
		private Layout _layout;

		[SerializeField]
		private Layout _coinLayout;

		private int _missionsInStoryLineSet;

		private void Awake()
		{
			_missionsInStoryLineSet = MissionsManager.Instance.missionSetStoryCount + 1;
		}

		public override void SetupSlideIn(SlideIn slideIn)
		{
			base.gameObject.SetActive(true);
			lineReward.enabled = false;
			superMysteryBox.enabled = false;
			mysteryBox.enabled = false;
			EnableCoinLabel(slideIn.type == SlideInData.SlideInType.LettersCompleteCoins);
			line1.text = Strings.Get("DAILY_CHALLENGE_COMPLETE");
			if (slideIn.type == SlideInData.SlideInType.MissionSet)
			{
				if (PlayerSave.missionSetCompletedCount > _missionsInStoryLineSet)
				{
					superMysteryBox.enabled = true;
				}
				else
				{
					lineReward.enabled = true;
					lineReward.text = "x" + ScoreMultiplierInfo.rawMultiplier;
				}
				line1.text = Strings.Get("MISSION_SET_COMPLETE");
			}
			else if (slideIn.type == SlideInData.SlideInType.LettersCompleteMysteryBox)
			{
				superMysteryBox.enabled = slideIn.mysteryBoxType == MysteryBox.Type.Super;
				mysteryBox.enabled = slideIn.mysteryBoxType != MysteryBox.Type.Super;
			}
			_layout.Align();
			SlideIn();
		}

		public void NewCoinAmount(int coinAmount)
		{
			NewCoinAmount(coinAmount.ToString());
		}

		private void NewCoinAmount(string coinAmount)
		{
			base.gameObject.SetActive(true);
			coinLabel.text = coinAmount;
			coinIcon.MakePixelPerfect();
			base.gameObject.SetActive(false);
		}

		private void EnableCoinLabel(bool enable)
		{
			coinIcon.gameObject.SetActive(enable);
			coinLabel.gameObject.SetActive(enable);
			if (enable)
			{
				_coinLayout.Align();
			}
		}
	}
}
