using System;
using Data.Managers;
using Info;
using UnityEngine;

namespace SYBO.Subway.Promotion
{
	public class BoardPromotion : PromotionDefinition
	{
		[SerializeField]
		private Boards.BoardId _boardId = Boards.BoardId.hongKongBoard01;

		[Header("Background")]
		[SerializeField]
		private Color _backgroundColor;

		[SerializeField]
		private Color _flashColor;

		[SerializeField]
		private Color _beamColor;

		public Boards.BoardId BoardId
		{
			get
			{
				return _boardId;
			}
		}

		public Color BackgroundColor
		{
			get
			{
				return _backgroundColor;
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

		public override DateTime StartTime
		{
			get
			{
				return Boards.boardData[_boardId].defaultStartDate;
			}
		}

		public override DateTime ExpireTime
		{
			get
			{
				return Boards.boardData[_boardId].defaultExpirationTime;
			}
		}

		public override string TitleId
		{
			get
			{
				return Boards.boardData[BoardId].name;
			}
		}

		public override string SubtitleId
		{
			get
			{
				return Boards.boardData[BoardId].limitedDescription;
			}
		}

		public override string PopupId
		{
			get
			{
				return "BoardPromotionPopup";
			}
		}

		protected override bool IsActiveCheck
		{
			get
			{
				return !BoardManager.IsBoardOwned(_boardId);
			}
		}

		public override string GetPurchaseFailureFlurryEventValue()
		{
			string result = string.Empty;
			Boards.Board board = Boards.boardData[_boardId];
			switch (board.unlockType)
			{
			case PurchaseData.UnlockType.Coins:
				result = "not enough coins";
				break;
			case PurchaseData.UnlockType.Keys:
				result = "not enough keys";
				break;
			case PurchaseData.UnlockType.InApp:
				result = "bought using iap fail";
				break;
			}
			return result;
		}

		public override string GetPurchaseSuccessFlurryEventValue()
		{
			string result = string.Empty;
			Boards.Board board = Boards.boardData[_boardId];
			switch (board.unlockType)
			{
			case PurchaseData.UnlockType.Coins:
				result = "bought using coins";
				break;
			case PurchaseData.UnlockType.Keys:
				result = "bought using keys";
				break;
			case PurchaseData.UnlockType.InApp:
				result = "bought using iap";
				break;
			}
			return result;
		}

		public override bool ShouldTrackPopupCloseFlurryEvent()
		{
			return !BoardManager.IsBoardOwned(_boardId);
		}
	}
}
