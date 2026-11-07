using InApp;
using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class SubwayRealCurrencyPurchaseEvent : SubwayPurchaseEvent
	{
		[SerializeField]
		[SerializedSizeLimit(10, false)]
		[SerializedAs("vendor")]
		public readonly string Vendor;

		[SerializeField]
		[SerializedAs("price")]
		public readonly float LocalCurrencyPrice;

		public override string EventName
		{
			get
			{
				return "real_currency_purchase";
			}
		}

		public SubwayRealCurrencyPurchaseEvent(string itemTitleId, InAppItemModel.ItemType itemType, long purchaseId, string menusBefore, int? gameNumberOrNull, string vendor, float localCurrencyPrice)
			: base(itemTitleId, itemType, purchaseId, menusBefore, gameNumberOrNull)
		{
			Vendor = vendor;
			LocalCurrencyPrice = localCurrencyPrice;
		}
	}
}
