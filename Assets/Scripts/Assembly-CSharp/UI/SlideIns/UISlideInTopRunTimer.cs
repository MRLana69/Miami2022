using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInTopRunTimer : UISlideIn
	{
		[SerializeField]
		private UILabel _topRunLabel;

		[SerializeField]
		private UILabel _timerLabel;

		[SerializeField]
		private Layout _layout;

		public override void SetupSlideIn(SlideIn slideIn)
		{
			int value = slideIn.value;
			base.gameObject.SetActive(true);
			if (value > 1)
			{
				_timerLabel.text = string.Format(Strings.Get("SLIDE_IN_TOPRUN_MINUTES_LEFT"), value);
			}
			else if (value == 1)
			{
				_timerLabel.text = string.Format(Strings.Get("SLIDE_IN_TOPRUN_MINUTE_LEFT"), value);
			}
			else
			{
				_timerLabel.text = Strings.Get("SLIDE_IN_TOPRUN_ENDING_SOON");
			}
			_topRunLabel.text = Strings.Get("TOPRUN_TITLE");
			_layout.Align();
			SlideIn();
		}
	}
}
