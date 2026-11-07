using InApp;
using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public abstract class SubwayPurchaseEvent : SubwayBaseSessionEvent
	{
		[SerializeField]
		[SerializedAs("purchase_no")]
		public readonly long PurchaseId;

		[SerializeField]
		[SerializedSizeLimit(150, false)]
		[SerializedAs("menus_before")]
		public readonly string MenusBefore;

		[SerializeField]
		[SerializedAs("game_no")]
		public readonly int? GameNumberOrNull;

		[SerializeField]
		[SerializedSizeLimit(64, true)]
		[SerializedAs("item_id")]
		public readonly string ItemTitleId;

		[SerializeField]
		[SerializedAs("item_type")]
		public readonly int ItemType;

		public SubwayPurchaseEvent(string itemTitleId, InAppItemModel.ItemType itemType, long purchaseId, string menusBefore, int? gameNumberOrNull)
		{
			ItemTitleId = itemTitleId;
			ItemType = (int)itemType;
			PurchaseId = purchaseId;
			MenusBefore = menusBefore;
			GameNumberOrNull = gameNumberOrNull;
		}
	}
}
