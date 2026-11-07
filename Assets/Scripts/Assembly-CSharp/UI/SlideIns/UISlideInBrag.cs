using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInBrag : UISlideIn
	{
		[SerializeField]
		private UILabel _text;

		[SerializeField]
		private Layout _layout;

		public override void SetupSlideIn(SlideIn slideIn)
		{
			base.gameObject.SetActive(true);
			_text.text = Strings.Get("SLIDE_IN_BRAG");
			_layout.Align();
			SlideIn();
		}
	}
}
