using System.Collections.Generic;
using UI.Generic.Layout;
using UI.RenderTexture;
using UnityEngine;

namespace UI.Popups
{
	public class TokenMysteryBoxPrizesPopup : UIPopup
	{
		[Header("Labels")]
		[SerializeField]
		private UILabel _titleLabel;

		[SerializeField]
		private UILabel _subTitleLabel;

		[SerializeField]
		private UILabel _descriptionLabel;

		[Header("Sprites")]
		[SerializeField]
		private UISprite _gradientBackground;

		[SerializeField]
		private UISprite _bottomBackground;

		[SerializeField]
		private List<UISprite> _stripes;

		[Header("other refs")]
		[SerializeField]
		private DefaultRenderTextureSetup _starParticleRenderTextureSetup;

		[SerializeField]
		private HorizontalLayout _subtitleLayout;

		protected override float screenOffset
		{
			get
			{
				if (!ShouldShowTopBar())
				{
					return 0f;
				}
				return -13f;
			}
		}

		public override bool ShouldShowTopBar()
		{
			return !UIScreenController.Instance.IsPopupInQueue("CelebrationPopup");
		}

		protected override void Init()
		{
			SetupTexts();
			SetupColors();
			_starParticleRenderTextureSetup.Setup(new CameraSettings
			{
				backgroundColor = new Color(1f, 1f, 1f, 0f)
			});
		}

		public override void GainFocus()
		{
		}

		public override void LoseFocus()
		{
		}

		private void SetupColors()
		{
			_titleLabel.color = ColorData.instance.midnight;
			_titleLabel.fontSize = 23;
			_subTitleLabel.color = ColorData.instance.peacock;
			_subTitleLabel.fontSize = 17;
			_descriptionLabel.effectStyle = UILabel.Effect.Shadow;
			_descriptionLabel.effectDistance = new Vector2(1f, 1f);
			_descriptionLabel.color = ColorData.instance.pit;
			_descriptionLabel.effectColor = ColorData.instance.turquoise;
			_gradientBackground.applyGradient = true;
			_gradientBackground.gradientTop = ColorData.instance.teal;
			_gradientBackground.gradientBottom = ColorData.instance.turquoise;
			for (int i = 0; i < _stripes.Count; i++)
			{
				_stripes[0].color = ColorData.instance.stripeColor;
			}
			_bottomBackground.color = ColorData.instance.push;
		}

		private void SetupTexts()
		{
			_titleLabel.text = Strings.Get("PRIZE_POPUP_TITLE");
			_subTitleLabel.text = Strings.Get("TOKEN_MYSTERY_BOX_PRIZES_POPUP_SUBTITLE");
			_subtitleLayout.Align();
			_descriptionLabel.text = Strings.Get("TOKEN_MYSTERY_BOX_DESCRIPTION");
		}
	}
}
