using SYBO.Subway.Info;
using UnityEngine;

namespace UI.Popups
{
	public class FirstPurchaseBonusPopup : UIPopup
	{
		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _subTitle;

		[SerializeField]
		private UILabel _getNowButtonLabel;

		private UIModelController _uiModelController;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		private void Awake()
		{
			_uiModelController = UIModelController.Instance;
		}

		protected override void Show()
		{
			SetupTexts();
			_uiModelController.ShowMenuModel(Characters.CharacterId.firstPurchaseBonusCharacter01, 0, UIModelController.ModelScreen.FirstPurchaseBonusBigCharacter);
		}

		protected override void Hide()
		{
			_uiModelController.DeactivateModelScreen(UIModelController.ModelScreen.FirstPurchaseBonusBigCharacter);
		}

		private void SetupTexts()
		{
			_title.text = Strings.Get("FIRST_PURCHASE_POPUP_HEADLINE");
			_title.color = ColorData.instance.midnight;
			_subTitle.text = Strings.Get("FIRST_PURCHASE_STORE_TAB_SUB_TITLE");
			_subTitle.color = ColorData.instance.peacock;
			_getNowButtonLabel.text = Strings.Get("FIRST_PURCHASE_POPUP_BUTTON");
		}

		public void OnGetNowButtonClick()
		{
			UIScreenController instance = UIScreenController.Instance;
			instance.ignoreLastPopupClosed = true;
			instance.ClosePopup();
			instance.PushPopup("StorePopup");
			Flurry.LogEventWithAParameter("First purchase popup", "Result", "Moved to store");
		}

		public void OnClosePopup()
		{
			ClosePopup();
			Flurry.LogEventWithAParameter("First purchase popup", "Result", "Closed");
		}
	}
}
