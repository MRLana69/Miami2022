using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInAdsRemoved : UISlideIn
	{
		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _subtext;

		[SerializeField]
		private HorizontalLayout _layout;

		public override void SetupSlideIn(SlideIn slideIn)
		{
			base.gameObject.SetActive(true);
			_title.text = Strings.Get("SLIDE_IN_ADS_REMOVED_FIRST_LINE");
			_subtext.text = Strings.Get("SLIDE_IN_ADS_REMOVED_SECOND_LINE");
			_layout.Align(true);
			SlideIn();
		}
	}
}
