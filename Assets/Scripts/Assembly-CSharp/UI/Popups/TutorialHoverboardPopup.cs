using Data.Save;
using Info;
using JetBrains.Annotations;
using SYBO.Subway.Info;
using UI.Generic.Layout;
using UI.SlideIns;
using UnityEngine;

namespace UI.Popups
{
	public class TutorialHoverboardPopup : CharacterPopup
	{
		[SerializeField]
		private UILabel _calloutLabel;

		[SerializeField]
		private UILabel _subtitleLabel;

		[SerializeField]
		private Layout _labelLayout;

		[SerializeField]
		private UILabel _buttonLabel;

		[SerializeField]
		private GameObject _centerAnchor;

		[SerializeField]
		private GameObject _tutorialMultiLinesPrefab;

		private TutorialMultilineLabelsHelper _tutorialLinesHelper;

		private string _hoverboardScreenType = string.Empty;

		private const int NUMBER_OF_FREE_HOVERBOARDS = 3;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			_tutorialLinesHelper = TutorialMultilineLabelsHelper.AddToGameObject(_centerAnchor, _tutorialMultiLinesPrefab);
		}

		protected override void Show()
		{
			SetCharacter(Characters.CharacterId.slick, new Vector2(42f, -6f), null, 19f, false, null, false);
			_buttonLabel.text = Strings.Get("POPUP_TUTORIAL_HOVERBOARD_BUTTON");
			string line;
			string line2;
			if (Achievements.Instance.GetNumberOfHoverboardsUsed() == 0)
			{
				_calloutLabel.text = Strings.Get("POPUP_TUTORIAL_HOVERBOARD_NEVER_USED_CALLOUT");
				_subtitleLabel.text = Strings.Get("POPUP_TUTORIAL_HOVERBOARD_NEVER_USED_SUBTITLE");
				line = "POPUP_TUTORIAL_HOVERBOARD_NEVER_USED_LINE2_BIG";
				line2 = "POPUP_TUTORIAL_HOVERBOARD_NEVER_USED_LINE2_SMALL";
				_hoverboardScreenType = "GuidelineHoverboardNeverUsed";
			}
			else
			{
				_calloutLabel.text = Strings.Get("POPUP_TUTORIAL_HOVERBOARD_NEVER_BOUGHT_CALLOUT");
				_subtitleLabel.text = Strings.Get("POPUP_TUTORIAL_HOVERBOARD_NEVER_BOUGHT_SUBTITLE");
				line = "POPUP_TUTORIAL_HOVERBOARD_NEVER_BOUGHT_LINE2_BIG";
				line2 = "POPUP_TUTORIAL_HOVERBOARD_NEVER_BOUGHT_LINE2_SMALL";
				_hoverboardScreenType = "GuidelineHoverboardNeverBought";
			}
			_labelLayout.Align();
			_tutorialLinesHelper.SetText("POPUP_TUTORIAL_HOVERBOARD_LINE1_BIG", "POPUP_TUTORIAL_HOVERBOARD_LINE1_SMALL", line, line2, "POPUP_TUTORIAL_HOVERBOARD_LINE3_BIG", "POPUP_TUTORIAL_HOVERBOARD_LINE3_SMALL");
		}

		protected override void Hide()
		{
			HideCharacter();
			InventorySave.IncreaseAmount(UpgradeType.hoverboard, 3);
			UIScreenController.Instance.QueueSlideIn(new SlideIn
			{
				type = SlideInData.SlideInType.VideoPayout,
				consumableReward = SellableConsumable.hoverboard,
				value = 3
			});
			UIScreenController.Instance.QueuePopup("ConsumablesPopup");
		}

		[UsedImplicitly]
		public void OkClicked()
		{
			Flurry.LogOkPressedOnPopup("POPUP Screen " + _hoverboardScreenType);
			UIScreenController.Instance.CloseActivePopups(screenName);
		}

		[UsedImplicitly]
		public void CloseClicked()
		{
			Flurry.LogClosePressedOnPopup("POPUP Screen " + _hoverboardScreenType);
			UIScreenController.Instance.CloseActivePopups(screenName);
		}
	}
}
