using UI.Generic.Layout;
using UnityEngine;

namespace UI.SlideIns
{
	public class UISlideInLettersHelper : UISlideIn
	{
		[SerializeField]
		private Layout _layout;

		public override void SetupSlideIn(SlideIn slideIn)
		{
			base.gameObject.SetActive(true);
			_layout.Align();
			SlideIn();
		}
	}
}
