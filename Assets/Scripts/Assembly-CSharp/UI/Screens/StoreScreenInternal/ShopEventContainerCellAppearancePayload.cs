using System;
using UnityEngine;

namespace UI.Screens.StoreScreenInternal
{
	[Serializable]
	public class ShopEventContainerCellAppearancePayload
	{
		public Color fillTopColor = Color.black;

		public Color fillBottomColor = Color.white;

		public Color titleColor = Color.black;

		public Color titleEffectColor = Color.white;

		public Color subtitleColor = Color.black;

		public Color subtitleEffectColor = Color.white;

		public Color boardThemeColor = Color.black;

		public Color boardThemeEffectColor = Color.white;

		public Color bannerColorTop = Color.white;

		public Color bannerColorBottom = Color.white;

		public Color bannerTextColor = Color.white;

		public bool canBeWide = true;

		public StoreEventContainerSmall.EventCellButtonAction buttonAction;

		public string popupID;

		public int cellPriority;
	}
}
