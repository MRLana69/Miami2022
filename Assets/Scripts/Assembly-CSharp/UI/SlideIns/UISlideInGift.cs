using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInGift : UISlideIn
	{
		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _gift;

		[SerializeField]
		private HorizontalLayout _layout;

		public override void SetupSlideIn(SlideIn slideIn)
		{
			_title.text = Strings.Get("SLIDE_IN_GIFT_TITLE");
			_gift.text = slideIn.stringValue;
			base.gameObject.SetActive(true);
			_layout.Align(true);
			SlideIn();
		}
	}
}
