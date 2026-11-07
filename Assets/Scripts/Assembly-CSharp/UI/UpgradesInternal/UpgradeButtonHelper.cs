using UI.Generic.Layout;
using UnityEngine;

namespace UI.UpgradesInternal
{
	public class UpgradeButtonHelper : MonoBehaviour
	{
		public class UpgradeButtonPayload
		{
			public string buttonText;

			public bool isComplete;
		}

		[SerializeField]
		private UIButtonOverlayOff _buttonHandler;

		[SerializeField]
		private UILabel _buttonLabel;

		[SerializeField]
		private UISprite _currencyIcon;

		[SerializeField]
		private UISprite _checkMarkIcon;

		[SerializeField]
		private Layout _buttonLayout;

		public void Setup(UpgradeButtonPayload payload)
		{
			_checkMarkIcon.gameObject.SetActive(payload.isComplete);
			_currencyIcon.gameObject.SetActive(!payload.isComplete);
			if (payload.isComplete)
			{
				_buttonHandler.GoToNotAvailableState();
			}
			else
			{
				_buttonHandler.GoToNormalState();
			}
			_buttonLabel.color = Color.white;
			_buttonLabel.effectColor = ColorData.instance.shadow;
			_buttonLabel.text = payload.buttonText;
			_buttonLayout.Align();
		}
	}
}
