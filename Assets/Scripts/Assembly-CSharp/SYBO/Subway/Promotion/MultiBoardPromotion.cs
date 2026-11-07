using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SYBO.Common;
using UI.Screens.StoreScreenInternal;
using UnityEngine;

namespace SYBO.Subway.Promotion
{
	public class MultiBoardPromotion : PromotionDefinition
	{
		[Header("Titles")]
		[SerializeField]
		private string _titleId;

		[SerializeField]
		private string _subtitleId;

		[Header("Dates")]
		[SerializeField]
		private SerializableDate _serializedStartDate = SerializableDate.DefaultDate;

		[SerializeField]
		private SerializableDate _serializedExpireDate = SerializableDate.DefaultDate;

		[Header("Board Entries")]
		[SerializeField]
		private MultiBoardBundlePromotionEntry _entry1 = new MultiBoardBundlePromotionEntry();

		[SerializeField]
		private MultiBoardBundlePromotionEntry _entry2 = new MultiBoardBundlePromotionEntry();

		[SerializeField]
		private MultiBoardBundlePromotionEntry _entry3 = new MultiBoardBundlePromotionEntry();

		[Header("Shop Board Entries")]
		[SerializeField]
		private List<StoreEventContainerPayload> _shopEntries;

		private DateTime _startDate = DateTime.MinValue;

		private DateTime _expireDate = DateTime.MinValue;

		public override string PopupId
		{
			get
			{
				return "MultiBoardPromotionPopup";
			}
		}

		public override string TitleId
		{
			get
			{
				return _titleId;
			}
		}

		public override string SubtitleId
		{
			get
			{
				return _subtitleId;
			}
		}

		public override DateTime StartTime
		{
			get
			{
				if (_startDate == DateTime.MinValue)
				{
					_startDate = new DateTime(_serializedStartDate.Year, _serializedStartDate.Month, _serializedStartDate.Day);
				}
				return _startDate;
			}
		}

		public override DateTime ExpireTime
		{
			get
			{
				if (_expireDate == DateTime.MinValue)
				{
					_expireDate = new DateTime(_serializedExpireDate.Year, _serializedExpireDate.Month, _serializedExpireDate.Day);
				}
				return _expireDate;
			}
		}

		public List<StoreEventContainerPayload> ShopEntries
		{
			get
			{
				return _shopEntries;
			}
		}

		public ReadOnlyCollection<MultiBoardBundlePromotionEntry> AllEntries
		{
			get
			{
				List<MultiBoardBundlePromotionEntry> list = new List<MultiBoardBundlePromotionEntry>();
				list.Add(_entry1);
				list.Add(_entry2);
				list.Add(_entry3);
				return list.AsReadOnly();
			}
		}

		public override string GetPurchaseFailureFlurryEventValue()
		{
			return null;
		}

		public override string GetPurchaseSuccessFlurryEventValue()
		{
			return null;
		}

		public override bool ShouldTrackPopupCloseFlurryEvent()
		{
			return true;
		}
	}
}
