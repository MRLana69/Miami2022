using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

namespace UI.Popups
{
	public class ConnectGameCenterPopup : UIPopup
	{
		[SerializeField]
		private UILabel _descriptionLabel;

		[SerializeField]
		private UISprite _descriptionBackground;

		[SerializeField]
		private UILabel _titleLabel;

		[SerializeField]
		private UILabel _okButtonLabel;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			SetLabels();
			SetColors();
		}

		[Button(null)]
		private void SetLabels()
		{
			_descriptionLabel.text = Strings.Get("CONNECT_GAMECENTER_DESCRIPTION");
			_titleLabel.text = Strings.Get("CONNECT_GAMECENTER_TITLE");
			_okButtonLabel.text = Strings.Get("OK");
		}

		[Button(null)]
		public void SetColors()
		{
			ColorData instance = ColorData.instance;
			_descriptionBackground.applyGradient = true;
			_descriptionBackground.color = Color.white;
			_descriptionBackground.alpha = 0.7f;
			_descriptionBackground.gradientTop = instance.sky;
			_descriptionBackground.gradientBottom = instance.scooter;
			_titleLabel.color = instance.midnight;
			_descriptionLabel.color = instance.peacock;
		}

		[UsedImplicitly]
		public void OkButtonPressed()
		{
			UIScreenController.Instance.ClosePopup();
		}
	}
}
