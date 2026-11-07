using System;
using UnityEngine;

namespace SYBO.Subway.Promotion
{
	[Serializable]
	public class PromotionPresentation
	{
		[Header("Background")]
		[SerializeField]
		private Color _backgroundColorTop;

		[SerializeField]
		private Color _backgroundColorBottom;

		[SerializeField]
		private Color _flashColor;

		[SerializeField]
		private Color _beamColor;

		public Color BackgroundColorTop
		{
			get
			{
				return _backgroundColorTop;
			}
		}

		public Color BackgroundColorBottom
		{
			get
			{
				return _backgroundColorBottom;
			}
		}

		public Color BeamColor
		{
			get
			{
				return _beamColor;
			}
		}

		public Color FlashColor
		{
			get
			{
				return _flashColor;
			}
		}
	}
}
