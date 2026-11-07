using UI.Generic.Layout;
using UnityEngine;

namespace UI.Popups
{
	public class PrizeMMBPopup : UIPopup
	{
		[Header("Popup")]
		[SerializeField]
		private UILabel _titleLabel;

		[SerializeField]
		private UILabel _subtitleLabel;

		[SerializeField]
		private Layout _subtitleLayout;

		[Header("Rare")]
		[SerializeField]
		private UILabel _rareLabel;

		[SerializeField]
		private UILabel _keyLabel;

		[SerializeField]
		private UILabel _scoreBoosterLabel;

		[Header("Common")]
		[SerializeField]
		private UILabel _commonLabel;

		[SerializeField]
		private UILabel _boardsLabel;

		[SerializeField]
		private UILabel _coinsLabel;

		protected override float screenOffset
		{
			get
			{
				if (!ShouldShowTopBar())
				{
					return 0f;
				}
				return -13f;
			}
		}

		public override bool ShouldShowTopBar()
		{
			return !UIScreenController.Instance.IsPopupInQueue("CelebrationPopup");
		}

		protected override void Init()
		{
			_titleLabel.text = Strings.Get("PRIZE_POPUP_TITLE");
			_titleLabel.fontSize = 23;
			_titleLabel.color = ColorData.instance.midnight;
			_subtitleLabel.text = Strings.Get("PRIZE_POPUP_SUBTITLE_MMB");
			_subtitleLabel.fontSize = 17;
			_subtitleLabel.color = ColorData.instance.peacock;
			_subtitleLayout.Align();
			_rareLabel.text = Strings.Get("PRIZE_POPUP_CATEGORY_RARE");
			_keyLabel.text = string.Format(Strings.Get("PRIZE_POPUP_REWARD_UP_TO_KEYS"), Utility.NumberToStringFormat(2));
			_scoreBoosterLabel.text = Strings.Get("PRIZE_POPUP_REWARD_SCORE_BOOSTER_SINGULAR");
			_commonLabel.text = Strings.Get("PRIZE_POPUP_CATEGORY_COMMON");
			_boardsLabel.text = string.Format(Strings.Get("PRIZE_POPUP_REWARD_UP_TO_BOARDS"), Utility.NumberToStringFormat(3));
			_coinsLabel.text = string.Format(Strings.Get("PRIZE_POPUP_REWARD_UP_TO_COINS"), Utility.NumberToStringFormat(200));
		}
	}
}
