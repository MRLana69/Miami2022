using System;
using Info;
using UnityEngine;

namespace SYBO.Subway.Promotion
{
	[Serializable]
	public class MultiBoardBundlePromotionEntry
	{
		[SerializeField]
		private Boards.BoardId _boardId = Boards.BoardId.hongKongBoard01;

		[SerializeField]
		private Color _backgroundColorTop = Color.white;

		[SerializeField]
		private Color _backgroundColorBottom = Color.white;

		[SerializeField]
		private Color _backdropColor = Color.white;

		[SerializeField]
		private Color _strokeColor = Color.white;

		[SerializeField]
		private Color _beamColor = Color.white;

		[SerializeField]
		private Color _flashColor = Color.white;

		[SerializeField]
		private Color _titleColor = Color.white;

		[SerializeField]
		private Vector3 _boardPosition = new Vector3(0f, 0f, -35f);

		[SerializeField]
		private Vector3 _boardRotation = new Vector3(10f, 115f, -80f);

		[SerializeField]
		private float _boardScale = 13f;

		[SerializeField]
		private AnimationClip _animationClip;

		public Boards.BoardId BoardId
		{
			get
			{
				return _boardId;
			}
		}

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

		public Color BackdropColor
		{
			get
			{
				return _backdropColor;
			}
		}

		public Color StrokeColor
		{
			get
			{
				return _strokeColor;
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

		public Vector3 BoardPosition
		{
			get
			{
				return _boardPosition;
			}
		}

		public Vector3 BoardRotation
		{
			get
			{
				return _boardRotation;
			}
		}

		public Vector3 BoardScale
		{
			get
			{
				return Vector3.one * _boardScale;
			}
		}

		public AnimationClip AnimationClip
		{
			get
			{
				return _animationClip;
			}
		}

		public Color TitleColor
		{
			get
			{
				return _titleColor;
			}
		}
	}
}
