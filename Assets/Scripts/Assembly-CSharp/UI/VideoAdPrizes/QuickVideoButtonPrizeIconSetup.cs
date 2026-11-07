using System;
using UnityEngine;

namespace UI.VideoAdPrizes
{
	[Serializable]
	public class QuickVideoButtonPrizeIconSetup : PrizeIconSetup
	{
		[SerializeField]
		private Texture2D _iconMask;

		[SerializeField]
		protected UISprite _iconSprite;

		[SerializeField]
		private UIPanel _maskPanel;

		[SerializeField]
		private Transform _unrotateGlint;

		public Texture2D iconMask
		{
			get
			{
				return _iconMask;
			}
			set
			{
				_iconMask = value;
			}
		}

		public UIPanel maskPanel
		{
			get
			{
				return _maskPanel;
			}
			set
			{
				_maskPanel = value;
			}
		}

		public Transform unrotateGlint
		{
			get
			{
				return _unrotateGlint;
			}
			set
			{
				_unrotateGlint = value;
			}
		}

		public override UISprite iconSprite
		{
			get
			{
				return _iconSprite;
			}
			set
			{
				_iconSprite = value;
			}
		}

		protected override void OnSetup()
		{
			Enable();
			SetupMask();
			_unrotateGlint.localEulerAngles = -_iconRotation;
		}

		private void Enable()
		{
			_maskPanel.enabled = true;
		}

		protected override void OnDeactivate()
		{
			_maskPanel.enabled = false;
		}

		private void SetupMask()
		{
			_maskPanel.clipTexture = _iconMask;
			_maskPanel.transform.localPosition = _iconPosition;
			_maskPanel.transform.localEulerAngles = _iconRotation;
			_maskPanel.SetRect(_maskPanel.transform.position.x, _maskPanel.transform.position.y, _iconSprite.width, _iconSprite.height);
		}
	}
}
