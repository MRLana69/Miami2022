using UnityEngine;

namespace UI.ShopInternal
{
	public class SegmentDivider : MonoBehaviour
	{
		public class SegmentDividerPayload
		{
			public string headerText;

			public Color backgroundColor;

			public Color textColor;

			public Color textShadowColor;
		}

		[SerializeField]
		private UISprite _background;

		[SerializeField]
		private UISprite _backgroundShadow;

		[SerializeField]
		private UILabel _headerLabel;

		public void Setup(SegmentDividerPayload payload)
		{
			_headerLabel.text = payload.headerText;
			_headerLabel.color = payload.textColor;
			_headerLabel.effectColor = payload.textShadowColor;
			_background.color = payload.backgroundColor;
			_backgroundShadow.color = ColorData.instance.shadow;
		}
	}
}
