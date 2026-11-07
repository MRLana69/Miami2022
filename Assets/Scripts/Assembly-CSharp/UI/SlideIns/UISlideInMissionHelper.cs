using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInMissionHelper : UISlideIn
	{
		[SerializeField]
		private UILabel _line1;

		[SerializeField]
		private UILabel _line2;

		[SerializeField]
		private Layout _layout;

		[SerializeField]
		private Layout _line2Layout;

		public override void SetupSlideIn(SlideIn slideIn)
		{
			base.gameObject.SetActive(true);
			_line1.text = slideIn.stringValue;
			_line2.text = Strings.Get("SLIDE_IN_MISSION_LINE_2");
			_layout.Align();
			_line2Layout.Align();
			SlideIn();
		}
	}
}
