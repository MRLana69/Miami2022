using System.Collections.Generic;
using InApp;

namespace UI.DebugPopup
{
	public class InAppManagerDebugPopupDataEntries
	{
		private static Dictionary<InAppManagerPopupType, InAppManagerPopupData> _appManagerPopupDatas = new Dictionary<InAppManagerPopupType, InAppManagerPopupData>
		{
			{
				InAppManagerPopupType.CoinsWithInternet,
				new InAppManagerPopupData
				{
					popupName = "NotEnoughCurrencyPopup",
					bestOffers = new List<InAppItemModel> { InAppData.inAppData["com.kiloo.subways.coinstier1"] },
					canMakePurchase = true,
					currencyMissing = 75322,
					isCoinsPack = true,
					popupDescription = "75322 Coins needed",
					popupTitle = "Not enough coins?"
				}
			},
			{
				InAppManagerPopupType.CoinsWithNoInternet,
				new InAppManagerPopupData
				{
					popupName = "NotEnoughCurrencyPopup",
					canMakePurchase = false,
					currencyMissing = 75322,
					isCoinsPack = true,
					popupDescription = "75322 Coins needed",
					popupTitle = "Not enough coins?"
				}
			},
			{
				InAppManagerPopupType.KeysWithInternet,
				new InAppManagerPopupData
				{
					popupName = "NotEnoughCurrencyPopup",
					bestOffers = new List<InAppItemModel> { InAppData.inAppData["com.kiloo.subways.keystier1"] },
					canMakePurchase = true,
					currencyMissing = 23,
					isCoinsPack = false,
					popupDescription = "23 Keys needed",
					popupTitle = "Not enough keys?"
				}
			},
			{
				InAppManagerPopupType.KeysWithNoInternet,
				new InAppManagerPopupData
				{
					popupName = "NotEnoughCurrencyPopup",
					canMakePurchase = false,
					currencyMissing = 23,
					isCoinsPack = false,
					popupDescription = "23 Keys needed",
					popupTitle = "Not enough keys?"
				}
			},
			{
				InAppManagerPopupType.PurchaseFailed,
				new InAppManagerPopupData
				{
					popupName = "PurchaseErrorPopup",
					popupTitle = Strings.Get("PURCHASE_FAILED_TITLE"),
					popupDescription = Strings.Get("PURCHASE_FAILED_BODY"),
					canMakePurchase = false,
					isCoinsPack = false,
					isErrorMessage = true
				}
			},
			{
				InAppManagerPopupType.PurchaseItemUnavailable,
				new InAppManagerPopupData
				{
					popupName = "PurchaseErrorPopup",
					popupTitle = Strings.Get("PURCHASE_FAILED_TITLE"),
					popupDescription = Strings.Get("PURCHASE_FAILED_BODY"),
					canMakePurchase = false,
					isCoinsPack = false,
					isErrorMessage = true
				}
			}
		};

		public static InAppManagerPopupData GetPopupData(InAppManagerPopupType inAppManagerPopupType)
		{
			return _appManagerPopupDatas[inAppManagerPopupType];
		}
	}
}
