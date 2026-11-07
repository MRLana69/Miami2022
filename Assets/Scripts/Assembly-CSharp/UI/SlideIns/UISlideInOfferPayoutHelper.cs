using Kiloo.Common.Extensions;
using UI.Generic;
using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInOfferPayoutHelper : UISlideIn
	{
		[SerializeField]
		private UILabel _line1;

		[SerializeField]
		private UILabel _price;

		[SerializeField]
		private Layout _layout;

		[SerializeField]
		private Layout _line2Layout;

		[SerializeField]
		private UISprite _icon;

		[SerializeField]
		private FitSpriteToWidget _iconFit;

		public override void SetupSlideIn(SlideIn slideIn)
		{
			base.gameObject.SetActive(true);
			_line1.effectColor = ColorData.instance.shadow;
			_price.effectColor = ColorData.instance.shadow;
			_line1.text = ((!slideIn.purchase) ? Strings.Get("SLIDE_IN_PAYOUT_RECEIVED") : Strings.Get("SLIDE_IN_PAYOUT_PURCHASE"));
			_price.text = slideIn.stringValue;
			_icon.spriteName = ((!slideIn.iconName.IsNullOrEmpty()) ? slideIn.iconName : "icon_coin_large");
			_iconFit.FitNow();
			_layout.Align();
			_line2Layout.Align();
			SlideIn();
		}
	}
}
