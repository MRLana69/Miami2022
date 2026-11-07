using InApp;
using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class OfferInitiatedEvent : SubwayBaseSessionEvent
	{
		[SerializeField]
		[SerializedAs("offer_id")]
		public readonly long OfferId;

		[SerializeField]
		[SerializedAs("offer_type")]
		[SerializedSizeLimit(15, false)]
		public readonly string OfferType;

		[SerializeField]
		[SerializedAs("offer_title")]
		[SerializedSizeLimit(80, false)]
		public readonly string OfferTitle;

		[SerializeField]
		[SerializedAs("item_amount")]
		public readonly int OfferItemAmount;

		[SerializeField]
		[SerializedSizeLimit(64, true)]
		[SerializedAs("item_id")]
		public readonly string ItemTitleId;

		[SerializeField]
		[SerializedAs("item_type")]
		public readonly int ItemType;

		public override string EventName
		{
			get
			{
				return "offer_initiated";
			}
		}

		public OfferInitiatedEvent(long offerId, string offerType, string offerTitle, int offerItemAmount, string itemTitleId, InAppItemModel.ItemType itemType)
		{
			OfferId = offerId;
			OfferType = offerType;
			OfferTitle = offerTitle;
			OfferItemAmount = offerItemAmount;
			ItemTitleId = itemTitleId;
			ItemType = (int)itemType;
		}
	}
}
