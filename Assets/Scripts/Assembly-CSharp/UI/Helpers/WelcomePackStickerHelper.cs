using UnityEngine;

namespace UI.Helpers
{
	public class WelcomePackStickerHelper : MonoBehaviour
	{
		private const int PROCENT_OFF = 70;

		[SerializeField]
		private UISprite _stickerBackground;

		[SerializeField]
		private UILabel _stickerLabel;

		private void OnEnable()
		{
			SetupSticker();
		}

		private void SetupSticker()
		{
			_stickerBackground.color = ColorData.instance.sun;
			_stickerLabel.color = ColorData.instance.mahogany;
			_stickerLabel.text = string.Format(Strings.Get("DISCOUNT_BUTTON_COOL_OFF"), 70);
		}
	}
}
