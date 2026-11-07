using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInErrorMessage : UISlideIn
	{
		[SerializeField]
		private UILabel _messageLabel;

		[SerializeField]
		private Layout _layout;

		public override void SetupSlideIn(SlideIn slideIn)
		{
			base.gameObject.SetActive(true);
			_messageLabel.text = slideIn.stringValue;
			_layout.Align();
			SlideIn();
		}
	}
}
