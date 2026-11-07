using System;
using Data.Managers;
using SYBO.Common;
using UnityEngine;

namespace SYBO.Subway.Promotion
{
	public class DuoPromotion : PromotionDefinition
	{
		[Header("Titles")]
		[SerializeField]
		private string _titleId;

		[SerializeField]
		private string _subtitleId;

		[SerializeField]
		private string _bannerTextId;

		[Header("Dates")]
		[SerializeField]
		private SerializableDate _serializedStartDate = SerializableDate.DefaultDate;

		[SerializeField]
		private SerializableDate _serializedExpireDate = SerializableDate.DefaultDate;

		[Header("Background")]
		[SerializeField]
		private Color _backgroundColorTop;

		[SerializeField]
		private Color _backgroundColoBottom;

		[SerializeField]
		private Color _flashColor;

		[SerializeField]
		private Color _beamColor;

		[Header("Inapp product key")]
		[SerializeField]
		private string _inAppDataKey;

		[Header("Characters/Boards")]
		[SerializeField]
		private DuoPromotionEntry _entry1;

		[SerializeField]
		private DuoPromotionEntry _entry2;

		private DateTime _startDate = DateTime.MinValue;

		private DateTime _expireDate = DateTime.MinValue;

		public override string PopupId
		{
			get
			{
				return "DuoPromotionPopup";
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

		public string BanneTextid
		{
			get
			{
				return _bannerTextId;
			}
		}

		public string InAppDataKey
		{
			get
			{
				return "com.kiloo.subways." + _inAppDataKey;
			}
		}

		public Color BackgroundColorTop
		{
			get
			{
				return _backgroundColorTop;
			}
		}

		public Color BackgroundColorBottom
		{
			get
			{
				return _backgroundColoBottom;
			}
		}

		public Color BeamColor
		{
			get
			{
				return _beamColor;
			}
		}

		public Color FlashColor
		{
			get
			{
				return _flashColor;
			}
		}

		public DuoPromotionEntry Entry1
		{
			get
			{
				return _entry1;
			}
		}

		public DuoPromotionEntry Entry2
		{
			get
			{
				return _entry2;
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

		protected override bool IsActiveCheck
		{
			get
			{
				return !CharacterManager.IsCharacterOwned(_entry1.CharacterId) || !CharacterManager.IsCharacterOwned(_entry2.CharacterId) || !BoardManager.IsBoardOwned(_entry1.BoardId) || !BoardManager.IsBoardOwned(_entry2.BoardId);
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
