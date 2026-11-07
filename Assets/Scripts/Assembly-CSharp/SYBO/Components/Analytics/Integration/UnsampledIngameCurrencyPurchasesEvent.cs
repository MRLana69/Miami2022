using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class UnsampledIngameCurrencyPurchasesEvent : SubwayBaseSessionEvent
	{
		[SerializeField]
		[SerializedSizeLimit(64, true)]
		[SerializedAs("item_id")]
		public readonly string ItemTitleId;

		[SerializeField]
		[SerializedAs("amount")]
		public readonly int Amount;

		public override string EventName
		{
			get
			{
				return "unsampled_ingame_currency_purchases";
			}
		}

		public UnsampledIngameCurrencyPurchasesEvent(string itemTitleId, int amount)
		{
			ItemTitleId = itemTitleId;
			Amount = amount;
		}
	}
}
