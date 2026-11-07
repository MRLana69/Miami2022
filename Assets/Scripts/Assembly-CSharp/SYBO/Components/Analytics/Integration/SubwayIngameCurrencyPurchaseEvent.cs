using InApp;
using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class SubwayIngameCurrencyPurchaseEvent : SubwayPurchaseEvent
	{
		[SerializeField]
		[SerializedSizeLimit(5, false)]
		[SerializedAs("currency_type")]
		public readonly string CurrencyType;

		[SerializeField]
		[SerializedAs("currency_delta")]
		public readonly long CurrencyDelta;

		[SerializeField]
		[SerializedAs("currency_balance_after")]
		public readonly long CurrencyBalanceAfter;

		public override string EventName
		{
			get
			{
				return "ingame_currency_purchase";
			}
		}

		public SubwayIngameCurrencyPurchaseEvent(string itemTitleId, InAppItemModel.ItemType itemType, long purchaseId, string menusBefore, int? gameNumberOrNull, InGameCurrencyType currencyType, long currencyDelta, long currencyBalanceAfter)
			: base(itemTitleId, itemType, purchaseId, menusBefore, gameNumberOrNull)
		{
			CurrencyType = currencyType.ToString().ToLowerInvariant();
			CurrencyDelta = currencyDelta;
			CurrencyBalanceAfter = currencyBalanceAfter;
		}

		public override string ToString()
		{
			return string.Format("\n Purchase by user {0}", base.UserId);
		}
	}
}
