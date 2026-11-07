using System;
using Data.Managers;
using Info;
using SYBO.Subway.Info;
using UnityEngine;

namespace SYBO.Subway.Promotion
{
	public class CharacterPromotion : PromotionDefinition
	{
		[Header("Character")]
		[SerializeField]
		private string _characterIdStr;

		[SerializeField]
		private AnimationClip[] _characterAnimations;

		[SerializeField]
		private bool _useSpecialAssets;

		[SerializeField]
		private int _themeIndex;

		[Header("Background")]
		[SerializeField]
		private Color _backgroundColorTop;

		[SerializeField]
		private Color _backgroundColoBottom;

		[SerializeField]
		private Color _flashColor;

		[SerializeField]
		private Color _beamColor;

		public SYBO.Subway.Info.Characters.CharacterId CharacterId
		{
			get
			{
				return (SYBO.Subway.Info.Characters.CharacterId)Enum.Parse(typeof(SYBO.Subway.Info.Characters.CharacterId), _characterIdStr);
			}
		}

		public AnimationClip[] CharacterAnimations
		{
			get
			{
				return _characterAnimations;
			}
		}

		public bool UseSpecialAssets
		{
			get
			{
				return _useSpecialAssets;
			}
		}

		public int ThemeIndex
		{
			get
			{
				return _themeIndex;
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

		public override string TitleId
		{
			get
			{
				return SYBO.Subway.Info.Characters.characterData[CharacterId].name;
			}
		}

		public override string SubtitleId
		{
			get
			{
				return SYBO.Subway.Info.Characters.characterData[CharacterId].characterLimitedDescription;
			}
		}

		public override string PopupId
		{
			get
			{
				return "CharacterPromotionPopup";
			}
		}

		public override DateTime StartTime
		{
			get
			{
				return SYBO.Subway.Info.Characters.characterData[CharacterId].defaultStartDate;
			}
		}

		public override DateTime ExpireTime
		{
			get
			{
				return SYBO.Subway.Info.Characters.characterData[CharacterId].defaultExpirationDate;
			}
		}

		protected override bool IsActiveCheck
		{
			get
			{
				return !CharacterManager.IsCharacterOwned(CharacterId);
			}
		}

		public override string GetPurchaseFailureFlurryEventValue()
		{
			string result = string.Empty;
			SYBO.Subway.Info.Characters.Model model = SYBO.Subway.Info.Characters.characterData[CharacterId];
			switch (model.unlockType)
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
			SYBO.Subway.Info.Characters.Model model = SYBO.Subway.Info.Characters.characterData[CharacterId];
			switch (model.unlockType)
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
			return !CharacterManager.IsCharacterOwned(CharacterId);
		}
	}
}
