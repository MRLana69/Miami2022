using UI.Generic;
using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInVideoPayoutHelper : UISlideIn
	{
		[SerializeField]
		private UILabel _line1;

		[SerializeField]
		private UILabel _prizeCount;

		[SerializeField]
		private Layout _layout;

		[SerializeField]
		private Layout _line2Layout;

		[SerializeField]
		private UISprite _rewardSprite;

		[SerializeField]
		private FitSpriteToWidget _spriteFitter;

		public override void SetupSlideIn(SlideIn slideIn)
		{
			base.gameObject.SetActive(true);
			_line1.text = Strings.Get("SLIDE_IN_PAYOUT_RECEIVED");
			_prizeCount.text = slideIn.value.ToString();
			if (slideIn.consumableReward != null)
			{
				_rewardSprite.spriteName = slideIn.consumableReward.spriteName;
			}
			_spriteFitter.FitNow();
			_layout.Align();
			_line2Layout.Align();
			SlideIn();
		}
	}
}
