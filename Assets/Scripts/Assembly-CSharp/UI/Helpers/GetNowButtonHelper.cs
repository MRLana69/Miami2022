using UnityEngine;

namespace UI.Helpers
{
	public class GetNowButtonHelper : MonoBehaviour
	{
		[SerializeField]
		private UILabel _getNowButtonLabel;

		private void OnEnable()
		{
			_getNowButtonLabel.text = Strings.Get("FIRST_PURCHASE_POPUP_BUTTON");
		}
	}
}
