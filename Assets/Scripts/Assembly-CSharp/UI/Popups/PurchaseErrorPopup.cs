using InApp;
using SYBO.Subway;
using UnityEngine;

namespace UI.Popups
{
	public class PurchaseErrorPopup : UIPopup
	{
		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _description;

		[SerializeField]
		private UILabel _okButtonLabel;

		private bool isInRun
		{
			get
			{
				return Game.Instance.IsInGame.Value && UIScreenController.Instance.GetTopScreenName() == "IngameUI";
			}
		}

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
			return !isInRun;
		}

		protected override void Show()
		{
			InAppManagerPopupData popupData = InAppManager.instance.GetPopupData();
			if (popupData != null)
			{
				_title.text = popupData.popupTitle;
				_title.color = ColorData.instance.midnight;
				_description.text = popupData.popupDescription;
				_description.color = ColorData.instance.peacock;
			}
			_okButtonLabel.text = Strings.Get("OK");
		}

		protected override void Hide()
		{
			if (Game.Instance.IsInGame.Value && SaveMeManager.isPurchaseRunningIngame)
			{
				InAppManager.instance.NativePurchaseErrorForReviveWithKeys();
			}
		}
	}
}
