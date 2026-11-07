using Kiloo.Common.Extensions;
using UI.Generic.Layout;
using UnityEngine;

namespace UI.Popups
{
	public class PrizeMBPopup : UIPopup
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

		[SerializeField]
		private UILabel _trophiesLabel;

		[SerializeField]
		private UILabel _amountOfCoinsLabel;

		[Header("Special")]
		[SerializeField]
		private UILabel _specialLabel;

		[SerializeField]
		private UILabel _headstartsLabel;

		[SerializeField]
		private UILabel _upToBoardsLabel;

		[SerializeField]
		private UILabel _upToCoinsLabel;

		[Header("Common")]
		[SerializeField]
		private UILabel _commonLabel;

		[Header("Jackpot")]
		[SerializeField]
		private GameObject _defaultJackpotElements;

		[SerializeField]
		private GameObject _megaJackpotElements;

		[Header("Default Jackpot")]
		[SerializeField]
		private UILabel _jackpotLabel;

		[SerializeField]
		private UILabel _coinsTitleLabel;

		[SerializeField]
		private Layout _defaultJackpotLayout;

		[Header("Mega Jackpot")]
		[SerializeField]
		private UILabel _newJackpotTitleLabel;

		[SerializeField]
		private UILabel _oldJackpotLabel;

		[SerializeField]
		private UILabel _newJackpotLabel;

		[SerializeField]
		private Layout _megaJackpotLayout;

		[SerializeField]
		private UISprite _lineThrough;

		[SerializeField]
		private UISprite _lineThroughShadow;

		private const int LINETHROUGH_WIDTH_PADDING = 4;

		private const float LINETHROUGH_Y_OFFSET = -1f;

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

		public override string GetFlurryName()
		{
			return "PrizeMBPopup_Keys";
		}

		protected override void Init()
		{
			_titleLabel.text = Strings.Get("PRIZE_POPUP_TITLE");
			_titleLabel.fontSize = 23;
			_titleLabel.color = ColorData.instance.midnight;
			_subtitleLabel.text = Strings.Get("PRIZE_POPUP_SUBTITLE_MB");
			_subtitleLabel.fontSize = 17;
			_subtitleLabel.color = ColorData.instance.peacock;
			_subtitleLayout.Align();
			_rareLabel.text = Strings.Get("PRIZE_POPUP_CATEGORY_RARE");
			_keyLabel.text = string.Format(Strings.Get("PRIZE_POPUP_REWARD_KEY_SINGULAR"));
			_scoreBoosterLabel.text = Strings.Get("PRIZE_POPUP_REWARD_SCORE_BOOSTER_PLURAL");
			_trophiesLabel.text = Strings.Get("PRIZE_POPUP_REWARD_TROPHIES");
			_amountOfCoinsLabel.text = string.Format(Strings.Get("PRIZE_POPUP_REWARD_COINS"), Utility.NumberToStringFormat(5000));
			_specialLabel.text = Strings.Get("PRIZE_POPUP_CATEGORY_SPECIAL");
			_headstartsLabel.text = Strings.Get("PRIZE_POPUP_REWARD_HEADSTARTS");
			_upToBoardsLabel.text = string.Format(Strings.Get("PRIZE_POPUP_REWARD_UP_TO_BOARDS"), Utility.NumberToStringFormat(10));
			_upToCoinsLabel.text = string.Format(Strings.Get("PRIZE_POPUP_REWARD_UP_TO_COINS"), Utility.NumberToStringFormat(2500));
			_commonLabel.text = Strings.Get("PRIZE_POPUP_CATEGORY_COMMON");
		}

		protected override void Show()
		{
			SSEventsManagerController.OnSSEventStartedHandler += SetJackpotLabels;
			SSEventsManagerController.OnSSEventFinishedHandler += SetJackpotLabels;
			UIScreenController.OnApplicationResumed += SetJackpotLabels;
			SetJackpotLabels();
		}

		protected override void Hide()
		{
			SSEventsManagerController.OnSSEventStartedHandler -= SetJackpotLabels;
			SSEventsManagerController.OnSSEventFinishedHandler -= SetJackpotLabels;
			UIScreenController.OnApplicationResumed -= SetJackpotLabels;
		}

		public override bool ShouldShowTopBar()
		{
			return !UIScreenController.Instance.IsPopupInQueue("CelebrationPopup");
		}

		private void SetJackpotLabels()
		{
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			if (activeEventManager != null && activeEventManager.GetEventType() == SSEventType.BiggerJackpot)
			{
				_defaultJackpotElements.SetActive(false);
				_megaJackpotElements.SetActive(true);
				_newJackpotTitleLabel.text = Strings.Get("PRIZE_POPUP_CATEGORY_JACKPOT");
				string text = string.Format(Strings.Get("PRIZE_POPUP_REWARD_JACKPOT_COINS"), Utility.NumberToStringFormat(100000));
				string text2 = string.Format(Strings.Get("PRIZE_POPUP_REWARD_JACKPOT_COINS"), Utility.NumberToStringFormat(MysteryBox.GetMegaJackpotAmount(MysteryBox.Type.Normal)));
				_oldJackpotLabel.text = text;
				_newJackpotLabel.text = text2;
				_megaJackpotLayout.Align();
				int width = _oldJackpotLabel.printedSize.x.CeilToInt() + 4;
				_lineThrough.width = width;
				_lineThroughShadow.width = width;
				_lineThrough.transform.SetLocalPosY(_oldJackpotLabel.transform.localPosition.y);
				_lineThroughShadow.transform.SetLocalPosY(_oldJackpotLabel.transform.localPosition.y + -1f);
			}
			else
			{
				_defaultJackpotElements.SetActive(true);
				_megaJackpotElements.SetActive(false);
				string text3 = string.Format(Strings.Get("PRIZE_POPUP_REWARD_JACKPOT_COINS"), Utility.NumberToStringFormat(100000));
				_coinsTitleLabel.text = text3;
				_jackpotLabel.text = Strings.Get("PRIZE_POPUP_CATEGORY_JACKPOT");
				_defaultJackpotLayout.Align();
			}
		}
	}
}
