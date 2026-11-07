using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInUnlock : UISlideIn
	{
		[SerializeField]
		private UILabel _line1;

		[SerializeField]
		private UILabel _line2;

		[SerializeField]
		private Layout _layout;

		public override void SetupSlideIn(SlideIn slideIn)
		{
			base.gameObject.SetActive(true);
			_line1.text = Strings.Get("SLIDE_IN_UNLOCKED");
			_line2.text = slideIn.stringValue;
			_layout.Align();
			SlideIn();
		}
	}
}
