using UnityEngine;
using Utilities;

namespace UI.Screens
{
	public class NotebookScreen : UIScreen
	{
		[SerializeField]
		private UIWidget _background;

		private bool _hasAdjustedSafeAreaOffsets;

		public override void AdjustToResolution()
		{
			CheckSafeArea();
		}

		private void CheckSafeArea()
		{
			if (SafeArea.IsRestricted && !_hasAdjustedSafeAreaOffsets)
			{
				_background.bottomAnchor.absolute += SafeArea.OFFSET_BOTTOM;
				_background.ResetAndUpdateAnchors();
				_hasAdjustedSafeAreaOffsets = true;
			}
		}

		public override void GainFocus()
		{
		}

		public override void LoseFocus()
		{
		}
	}
}
