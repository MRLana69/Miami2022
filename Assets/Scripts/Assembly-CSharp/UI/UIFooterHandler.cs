using Data.Save;
using InApp;
using Kiloo;
using Kiloo.Common.Extensions;
using UnityEngine;

namespace UI
{
	public class UIFooterHandler : MonoBehaviour
	{
		[SerializeField]
		public UIButtonOverlayOff button0;

		[SerializeField]
		public UIButtonOverlayOff button1;

		[SerializeField]
		public UIButtonOverlayOff button2;

		[SerializeField]
		public UIButtonOverlayOff button3;

		public GameObject coinshopDiscount;

		public GameObject specialSaleCoinshopDiscount;

		public GameObject shopDailyGift;

		[SerializeField]
		private UISprite[] discountStickers;

		[SerializeField]
		private UILabel[] discountStickerLabels;

		[SerializeField]
		private UISprite specialSaleDiscountSticker;

		[SerializeField]
		private UILabel specialSaleDiscountStickerLabel;

		public void HandleStickers()
		{
			if (coinshopDiscount == null)
			{
				return;
			}
			UISprite[] array = discountStickers;
			foreach (UISprite uISprite in array)
			{
				uISprite.color = GlobalColorsGenerated.DISCOUNT_STICKER_COLOR;
			}
			UILabel[] array2 = discountStickerLabels;
			foreach (UILabel uILabel in array2)
			{
				uILabel.color = GlobalColorsGenerated.DISCOUNT_STICKER_LABEL_COLOR;
			}
			if (InAppManager.IsSpecialSaleActive)
			{
				if (InAppManager.IsWinterHolidaySpecialSaleActive)
				{
					specialSaleDiscountSticker.color = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_STICKER_COLOR;
					specialSaleDiscountStickerLabel.color = GlobalColors.SPECIAL_DISCOUNT_WINTER_HOLIDAY_STICKER_LABEL_COLOR;
				}
				else if (InAppManager.IsNewYearSpecialSaleActive)
				{
					specialSaleDiscountSticker.color = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_STICKER_COLOR;
					specialSaleDiscountStickerLabel.color = GlobalColors.SPECIAL_DISCOUNT_NEW_YEAR_STICKER_LABEL_COLOR;
				}
				else if (InAppManager.IsBlackFridaySpecialSaleActive)
				{
					specialSaleDiscountSticker.color = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_STICKER_COLOR;
					specialSaleDiscountStickerLabel.color = GlobalColors.SPECIAL_DISCOUNT_BLACK_FRIDAY_STICKER_LABEL_COLOR;
				}
			}
			string topScreenName = UIScreenController.Instance.GetTopScreenName();
			bool flag = topScreenName.IsOneOf("UpgradeScreen", "FreeStuffScreen") && UserSave.dailyGiftLastGiven.Date < CDateTime.Now.Date;
			bool flag2 = !flag && topScreenName.IsOneOf("UpgradeScreen", "FreeStuffScreen") && InAppManager.IsAnyDiscountAvailable;
			coinshopDiscount.SetActive(flag2 && !InAppManager.IsSpecialSaleActive);
			specialSaleCoinshopDiscount.SetActive(flag2 && InAppManager.IsSpecialSaleActive);
			shopDailyGift.SetActive(flag);
		}
	}
}
