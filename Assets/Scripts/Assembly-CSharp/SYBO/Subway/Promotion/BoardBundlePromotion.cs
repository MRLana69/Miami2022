using System;
using System.Globalization;
using Data.Managers;
using Info;
using UnityEngine;

namespace SYBO.Subway.Promotion
{
	public class BoardBundlePromotion : PromotionDefinition
	{
		private const string ONLINE_SETTINGS_KEY_ENABLE_BOARD_BUNDLE_SALE = "active_board_bundle_sale";

		private const string BOARD_BUNDLE_SALE_DISABLED_VALUE = "off";

		private static readonly CultureInfo ONLINESETTINGS_DATETIME_CULTUREINFO = new CultureInfo("da-DK");

		[SerializeField]
		private Boards.BoardId _boardId = Boards.BoardId.rioBoard03;

		[SerializeField]
		private Hoverboard.BoardAbility _boardAbility;

		[SerializeField]
		private string _bundleTier01 = "rioboard03.bundletier1";

		[SerializeField]
		private string _bundleTier02 = "rioboard03.bundletier2";

		[Header("Background")]
		[SerializeField]
		private Color _backgroundColorTop = Color.white;

		[SerializeField]
		private Color _backgroundColorBottom = Color.white;

		[SerializeField]
		private Color _flashColor = Color.white;

		[SerializeField]
		private Color _beamColor = Color.white;

		[Header("Bundle Background Presentation")]
		[SerializeField]
		private PromotionPresentation _boardBundlePresentation;

		[SerializeField]
		private PromotionPresentation _boardBundleTier01Presentation;

		[SerializeField]
		private PromotionPresentation _boardBundleTier02Presentation;

		[Header("Model")]
		[SerializeField]
		private Vector3 _modelPosition = new Vector3(-2f, -65f, 920f);

		[SerializeField]
		private Vector3 _modelRotation;

		[Header("Best Label")]
		[SerializeField]
		private Color _bestSplatBgColor;

		[SerializeField]
		private Color _bestSplatLabelColor;

		public Boards.BoardId BoardId
		{
			get
			{
				return _boardId;
			}
		}

		public string BundleTier01
		{
			get
			{
				return _bundleTier01;
			}
		}

		public string BundleTier02
		{
			get
			{
				return _bundleTier02;
			}
		}

		public PromotionPresentation BoardBundlePresentation
		{
			get
			{
				return _boardBundlePresentation;
			}
		}

		public PromotionPresentation BoardBundleTier01Presentation
		{
			get
			{
				return _boardBundleTier01Presentation;
			}
		}

		public PromotionPresentation BoardBundleTier02Presentation
		{
			get
			{
				return _boardBundleTier02Presentation;
			}
		}

		public Color BestSplatBgColor
		{
			get
			{
				return _bestSplatBgColor;
			}
		}

		public Color BestSplatLabelColor
		{
			get
			{
				return _bestSplatLabelColor;
			}
		}

		public override DateTime StartTime
		{
			get
			{
				return BoardStartAvailabilityDate(_boardId);
			}
		}

		public Hoverboard.BoardAbility BoardAbility
		{
			get
			{
				return _boardAbility;
			}
		}

		public Vector3 ModelPosition
		{
			get
			{
				return _modelPosition;
			}
		}

		public Vector3 ModelRotation
		{
			get
			{
				return _modelRotation;
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
				return _backgroundColorBottom;
			}
		}

		public Color FlashColor
		{
			get
			{
				return _flashColor;
			}
		}

		public Color BeamColor
		{
			get
			{
				return _beamColor;
			}
		}

		public override DateTime ExpireTime
		{
			get
			{
				return BoardExpirationDate(_boardId);
			}
		}

		protected override bool IsActiveCheck
		{
			get
			{
				return !BoardManager.IsBoardOwned(_boardId) && !BoardManager.IsBoardDisabled(_boardId) && !IsOnlineSettingsDisabled();
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
				return "BoardBundlePromoPopup";
			}
		}

		private DateTime BoardStartAvailabilityDate(Boards.BoardId boardId)
		{
			string key = "hoverboard_starttime_" + boardId;
			DateTime result = DateTime.MinValue;
			string valueString;
			if (!OnlineSettings.instance.HasValue(key))
			{
				result = ((!(Boards.boardData[boardId].defaultStartDate != DateTime.MinValue)) ? DateTime.MinValue : Boards.boardData[boardId].defaultStartDate);
			}
			else if (OnlineSettings.instance.TryGetValue(key, out valueString) && valueString != "disabled")
			{
				try
				{
					result = DateTime.Parse(valueString, ONLINESETTINGS_DATETIME_CULTUREINFO);
				}
				catch (FormatException)
				{
				}
			}
			return result;
		}

		public DateTime BoardExpirationDate(Boards.BoardId boardId)
		{
			string key = "hoverboard_expiretime_" + boardId;
			DateTime result = DateTime.MinValue;
			string valueString;
			if (!OnlineSettings.instance.HasValue(key))
			{
				result = DefaultExpirationDateForHoverboard(boardId);
			}
			else if (OnlineSettings.instance.TryGetValue(key, out valueString) && valueString != "disabled")
			{
				try
				{
					result = DateTime.Parse(valueString, ONLINESETTINGS_DATETIME_CULTUREINFO);
				}
				catch (FormatException)
				{
					result = DefaultExpirationDateForHoverboard(boardId);
				}
			}
			return result;
		}

		private DateTime DefaultExpirationDateForHoverboard(Boards.BoardId bId)
		{
			if (Boards.boardData[bId].defaultExpirationTime != DateTime.MinValue)
			{
				return Boards.boardData[bId].defaultExpirationTime;
			}
			return DateTime.MaxValue;
		}

		private bool IsOnlineSettingsDisabled()
		{
			string valueString;
			if (OnlineSettings.instance.TryGetValue("active_board_bundle_sale", out valueString) && valueString == "off")
			{
				return true;
			}
			return false;
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
