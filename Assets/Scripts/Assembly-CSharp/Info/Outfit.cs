using System;
using UnityEngine;

namespace Info
{
	[Serializable]
	public class Outfit
	{
		public Color32 buttonBgColor;

		public Color32 buttonIconColor;

		public PurchaseData.UnlockType unlockType;

		public int uiPriority;

		public bool isNewInThisUpdate;

		public int price;

		public string title;

		public Outfit Clone()
		{
			Outfit outfit = new Outfit();
			outfit.buttonBgColor = buttonBgColor;
			outfit.buttonIconColor = buttonIconColor;
			outfit.unlockType = unlockType;
			outfit.isNewInThisUpdate = isNewInThisUpdate;
			outfit.uiPriority = uiPriority;
			outfit.price = price;
			outfit.title = title;
			return outfit;
		}

		public void Copy(Outfit copyFrom)
		{
			buttonBgColor = copyFrom.buttonBgColor;
			buttonIconColor = copyFrom.buttonIconColor;
			unlockType = copyFrom.unlockType;
			isNewInThisUpdate = copyFrom.isNewInThisUpdate;
			uiPriority = copyFrom.uiPriority;
			price = copyFrom.price;
			title = copyFrom.title;
		}
	}
}
