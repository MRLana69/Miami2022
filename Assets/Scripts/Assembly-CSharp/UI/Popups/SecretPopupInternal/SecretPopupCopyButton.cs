using JetBrains.Annotations;
using UnityEngine;

namespace UI.Popups.SecretPopupInternal
{
	public class SecretPopupCopyButton : MonoBehaviour
	{
		[SerializeField]
		private SecretPopup _secretPopUpScreen;

		[UsedImplicitly]
		private void OnClick()
		{
			_secretPopUpScreen.CopyToClipboard();
		}
	}
}
