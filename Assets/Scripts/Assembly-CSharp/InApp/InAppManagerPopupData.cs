using System.Collections.Generic;

namespace InApp
{
	public class InAppManagerPopupData
	{
		public string popupTitle;

		public string popupDescription;

		public string popupName;

		public bool canMakePurchase;

		public bool isCoinsPack;

		public bool isErrorMessage;

		public int currencyMissing;

		public List<InAppItemModel> bestOffers;
	}
}
