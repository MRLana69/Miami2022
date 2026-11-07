using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInItemHunt : UISlideIn
	{
		[SerializeField]
		private UILabel _textLabel;

		[SerializeField]
		private Layout _layout;

		public override void SetupSlideIn(SlideIn slideIn)
		{
			base.gameObject.SetActive(true);
			_textLabel.text = Strings.Get("WEEKLY_HUNT_SLIDEIN");
			_layout.Align();
			SlideIn();
		}
	}
}
