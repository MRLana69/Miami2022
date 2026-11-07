using InApp;
using Info;
using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class SubwayItemAddedEvent : SubwayBaseSessionEvent
	{
		[SerializeField]
		[SerializedAs("source")]
		public readonly int Source;

		[SerializeField]
		[SerializedAs("source_id")]
		public readonly long SourceId;

		[SerializeField]
		[SerializedSizeLimit(64, false)]
		[SerializedAs("source_meta")]
		public readonly string SourceMetaOrNull;

		[SerializeField]
		[SerializedSizeLimit(64, true)]
		[SerializedAs("item_id")]
		public readonly string ItemTitleId;

		[SerializeField]
		[SerializedAs("item_type")]
		public readonly int ItemType;

		[SerializeField]
		[SerializedAs("item_balance_after")]
		public readonly long ItemBalanceAfter;

		[SerializeField]
		[SerializedAs("item_delta")]
		public readonly long ItemDelta;

		[SerializeField]
		[SerializedAs("instance_id")]
		public readonly long InstanceId;

		public override string EventName
		{
			get
			{
				return "item_added";
			}
		}

		public SubwayItemAddedEvent(ObtainableItemSource source, long sourceId, string sourceMetaOrNull, string itemTitleId, InAppItemModel.ItemType itemType, long itemBalanceAfter, long itemDelta)
		{
			Source = (int)source;
			SourceId = sourceId;
			SourceMetaOrNull = sourceMetaOrNull;
			ItemTitleId = itemTitleId;
			ItemType = (int)itemType;
			ItemBalanceAfter = itemBalanceAfter;
			ItemDelta = itemDelta;
			InstanceId = SubwayRandom.DateTimeTicksRandomOffset();
		}

		public SubwayItemAddedEvent(ObtainableItemSource source, long sourceId, string sourceMetaOrNull, string itemTitleId, InAppItemModel.ItemType itemType, long itemBalanceAfter, long itemDelta, long instanceId)
			: this(source, sourceId, sourceMetaOrNull, itemTitleId, itemType, itemBalanceAfter, itemDelta)
		{
			InstanceId = instanceId;
		}
	}
}
