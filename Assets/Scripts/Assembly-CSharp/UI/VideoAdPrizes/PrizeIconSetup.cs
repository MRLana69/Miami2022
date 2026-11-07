using System.Collections.Generic;
using UnityEngine;

namespace UI.VideoAdPrizes
{
	public abstract class PrizeIconSetup
	{
		private const string ICON_NAME_MYSTERY_BOX = "icon_MysteryBox_big";

		private const string ICON_NAME_MINI_MYSTERY_BOX = "icon_big_mini_mysteryBox";

		private const string ICON_NAME_SUPER_MYSTERY_BOX = "icon_bigSuperMysteryBox";

		private const string ICON_NAME_COINS = "Icon_coin_pouch_larger";

		private const string ICON_NAME_KEY = "icon_key_cloud";

		private const string ICON_NAME_KEY_OFFERSCREEN = "icon_keyPack_01";

		[SerializeField]
		protected VideoAdPrizeType _prizeType;

		[SerializeField]
		protected Vector3 _iconPosition;

		[SerializeField]
		protected Vector3 _iconRotation;

		[SerializeField]
		protected int _iconHeight = 52;

		[SerializeField]
		protected Vector3 _starPosition;

		public VideoAdPrizeType prizeType
		{
			get
			{
				return _prizeType;
			}
		}

		public Vector3 starPosition
		{
			get
			{
				return _starPosition;
			}
		}

		public abstract UISprite iconSprite { get; set; }

		public void Setup(bool isOfferScreen = false)
		{
			SetupIconSprite(isOfferScreen);
			OnSetup();
		}

		public void Deactivate()
		{
			DeactivateIconSprite();
			OnDeactivate();
		}

		private void SetupIconSprite(bool isOfferScreen = false)
		{
			if (iconSprite == null)
			{
				return;
			}
			iconSprite.enabled = true;
			switch (_prizeType)
			{
			case VideoAdPrizeType.SuperMysteryBox:
				iconSprite.spriteName = "icon_bigSuperMysteryBox";
				break;
			case VideoAdPrizeType.MysteryBox:
				iconSprite.spriteName = "icon_MysteryBox_big";
				break;
			case VideoAdPrizeType.MiniMysteryBox:
				iconSprite.spriteName = "icon_big_mini_mysteryBox";
				break;
			case VideoAdPrizeType.Keys:
			case VideoAdPrizeType.DailyVideoPrize:
				if (isOfferScreen)
				{
					iconSprite.spriteName = "icon_keyPack_01";
				}
				else
				{
					iconSprite.spriteName = "icon_key_cloud";
				}
				break;
			case VideoAdPrizeType.Coins:
				iconSprite.spriteName = "Icon_coin_pouch_larger";
				break;
			default:
				Debug.LogError("PrizeIconSetup: Could not associate VideoAdPrizeType with icon name.");
				break;
			}
			iconSprite.transform.localPosition = _iconPosition;
			iconSprite.transform.localEulerAngles = _iconRotation;
			iconSprite.MakePixelPerfect();
			iconSprite.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnHeight;
			iconSprite.height = _iconHeight;
		}

		private void DeactivateIconSprite()
		{
			if (iconSprite != null)
			{
				iconSprite.enabled = false;
				iconSprite.keepAspectRatio = UIWidget.AspectRatioSource.Free;
			}
		}

		protected virtual void OnSetup()
		{
		}

		protected virtual void OnDeactivate()
		{
		}

		public static List<T> GetPrizeIconSetupsWithTypeAndDeactivateTheRest<T>(IEnumerable<T> prizeIconSetups, VideoAdPrizeType prizeType) where T : PrizeIconSetup
		{
			List<T> list = new List<T>();
			foreach (T prizeIconSetup in prizeIconSetups)
			{
				T current = prizeIconSetup;
				if (current.prizeType == prizeType)
				{
					list.Add(current);
				}
				else
				{
					current.Deactivate();
				}
			}
			return list;
		}
	}
}
