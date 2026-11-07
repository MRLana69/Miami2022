using Kiloo.Common.Extensions;
using UI.Footer;
using UnityEngine;
using Utilities;

namespace UI.Pages
{
	public abstract class Page : MonoBehaviour
	{
		[SerializeField]
		protected UIWidget topContainer;

		public abstract void Init(UIWidget pageGuideWidget);

		public abstract void Show();

		public abstract void Hide();

		public virtual void GainFocus()
		{
		}

		public virtual void LoseFocus()
		{
		}

		public abstract FooterButton.FooterPayload GetButtonPayload();

		protected void TopAlign(UIWidget pageGuideWidget)
		{
			UIWidget component = GetComponent<UIWidget>();
			component.height = pageGuideWidget.height;
			float num = pageGuideWidget.worldCorners[1].y - component.worldCorners[1].y;
			Vector3 position = component.transform.position;
			position.y += num;
			component.transform.position = position;
			if (SafeArea.IsRestricted)
			{
				component.height -= SafeArea.OFFSET_TOP + SafeArea.OFFSET_BOTTOM;
				component.transform.OffsetLocalPosY(SafeArea.OFFSET_BOTTOM);
			}
		}
	}
}
