using UnityEngine;

namespace UI.KScrollView
{
	public class KUIScrollView : UIScrollView
	{
		public ScrollView scrollView;

		public override Bounds bounds
		{
			get
			{
				return (!(scrollView == null)) ? scrollView.listViewBounds : base.bounds;
			}
		}

		protected override bool shouldMove
		{
			get
			{
				if (!disableDragIfFits)
				{
					return true;
				}
				if (mPanel == null)
				{
					mPanel = GetComponent<UIPanel>();
				}
				Vector4 baseClipRegion = mPanel.baseClipRegion;
				Bounds bounds = this.bounds;
				if (base.canMoveHorizontally && bounds.size.x > baseClipRegion.z)
				{
					return true;
				}
				if (base.canMoveVertically && bounds.size.y > baseClipRegion.w)
				{
					return true;
				}
				return false;
			}
		}
	}
}
