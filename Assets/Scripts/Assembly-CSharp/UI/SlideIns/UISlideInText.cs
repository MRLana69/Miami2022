using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInText : UISlideIn
	{
		[SerializeField]
		private UISprite _background;

		[SerializeField]
		private UILabel _line1;

		[SerializeField]
		private HorizontalLayout _horizontalLayout;

		public override void SetupSlideIn(SlideIn slideIn)
		{
			base.gameObject.SetActive(true);
			_line1.text = slideIn.stringValue;
			_horizontalLayout.Align();
			SlideIn();
		}

		private void CalculateBoxHeight()
		{
		}
	}
}
