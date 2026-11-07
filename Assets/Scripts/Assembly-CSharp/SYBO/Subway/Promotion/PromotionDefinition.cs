using System;
using Kiloo;
using Kiloo.Common.Utility;
using UI.ModelCamera;
using UI.Screens.StoreScreenInternal;
using UnityEngine;

namespace SYBO.Subway.Promotion
{
	public abstract class PromotionDefinition : ScriptableObject
	{
		[SerializeField]
		private bool _isEnabled;

		[SerializeField]
		private string _promotionId = "promotion_id";

		[Header("Popup")]
		[SerializeField]
		private Texture2D _stickerTexture;

		[SerializeField]
		private Color _titleTextColor;

		[SerializeField]
		private Color _subtitleTextColor;

		[SerializeField]
		private Color _timerBgColor = GlobalColorsGenerated.WEEKLYHUNT_TIMER_BG;

		[SerializeField]
		private Color _timerTextColor = GlobalColorsGenerated.WEEKLYHUNT_TIMER_TEXT;

		[Header("Sticker")]
		[SerializeField]
		private Vector2 _stickerPosition = new Vector2(92f, -3f);

		[SerializeField]
		private Vector2 _stickerSize = Vector2.one;

		[Header("StoreCell")]
		[SerializeField]
		private ShopEventContainerCellAppearancePayload _cellAppearancePayload;

		[SerializeField]
		private AnimationClip _cellModelAnimationClip;

		[Header("Small Cell Settings")]
		[SerializeField]
		private Vector3 _cellModelPosition = ModelScreenSetups.GetScreenSetup(UIModelController.ModelScreen.ShopScreenPromotion).localPosition;

		[SerializeField]
		private Vector3 _cellModelRotation = ModelScreenSetups.GetScreenSetup(UIModelController.ModelScreen.ShopScreenPromotion).localRotation;

		[SerializeField]
		private Vector3 _cellModelScale = Vector3.one * ModelScreenSetups.GetScreenSetup(UIModelController.ModelScreen.ShopScreenPromotion).localScaleOnAllAxes;

		[Header("Wide Cell Settings")]
		public bool useWideCellSettings;

		public Vector3 cellModelSecondaryPosition = ModelScreenSetups.GetScreenSetup(UIModelController.ModelScreen.ShopScreenPromotion).localPosition;

		public Vector3 cellModelSecondaryRotation = ModelScreenSetups.GetScreenSetup(UIModelController.ModelScreen.ShopScreenPromotion).localRotation;

		public Vector3 cellModelSecondaryScale = Vector3.one * ModelScreenSetups.GetScreenSetup(UIModelController.ModelScreen.ShopScreenPromotion).localScaleOnAllAxes;

		[Header("Front Screen")]
		[SerializeField]
		private GameObject _buttonPrefab;

		public bool IsActive
		{
			get
			{
				long num = CDateTime.Now.ToUnixTime();
				bool flag = num < StartTimeUnix || num >= ExpireTimeUnix;
				return _isEnabled && !flag && IsActiveCheck;
			}
		}

		protected virtual bool IsActiveCheck
		{
			get
			{
				return true;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return _isEnabled;
			}
			set
			{
				_isEnabled = value;
			}
		}

		public abstract string PopupId { get; }

		public abstract string TitleId { get; }

		public abstract string SubtitleId { get; }

		public abstract DateTime StartTime { get; }

		public abstract DateTime ExpireTime { get; }

		public string PromotionId
		{
			get
			{
				return _promotionId;
			}
		}

		public Texture2D StickerTexture
		{
			get
			{
				return _stickerTexture;
			}
		}

		public Color TitleTextColor
		{
			get
			{
				return _titleTextColor;
			}
		}

		public Color SubtitleTextColor
		{
			get
			{
				return _subtitleTextColor;
			}
		}

		public Color TimerBgColor
		{
			get
			{
				return _timerBgColor;
			}
		}

		public Color TimerTextColor
		{
			get
			{
				return _timerTextColor;
			}
		}

		public ShopEventContainerCellAppearancePayload CellAppearancePayload
		{
			get
			{
				return _cellAppearancePayload;
			}
		}

		public AnimationClip CellModelAnimationClip
		{
			get
			{
				return _cellModelAnimationClip;
			}
		}

		public Vector3 CellModelPosition
		{
			get
			{
				return _cellModelPosition;
			}
		}

		public Vector3 CellModelRotation
		{
			get
			{
				return _cellModelRotation;
			}
		}

		public Vector3 CellModelScale
		{
			get
			{
				return _cellModelScale;
			}
		}

		public GameObject ButtonPrefab
		{
			get
			{
				return _buttonPrefab;
			}
		}

		public long StartTimeUnix
		{
			get
			{
				return StartTime.ToUnixTime();
			}
		}

		public long ExpireTimeUnix
		{
			get
			{
				return ExpireTime.ToUnixTime();
			}
		}

		public TimeSpan TimeLeft
		{
			get
			{
				long expireTimeUnix = ExpireTimeUnix;
				long num = CDateTime.Now.ToUnixTime();
				return TimeSpan.FromSeconds(expireTimeUnix - num);
			}
		}

		public Vector2 StickerPosition
		{
			get
			{
				return _stickerPosition;
			}
		}

		public Vector2 StickerSize
		{
			get
			{
				return _stickerSize;
			}
		}

		public abstract string GetPurchaseSuccessFlurryEventValue();

		public abstract string GetPurchaseFailureFlurryEventValue();

		public abstract bool ShouldTrackPopupCloseFlurryEvent();
	}
}
