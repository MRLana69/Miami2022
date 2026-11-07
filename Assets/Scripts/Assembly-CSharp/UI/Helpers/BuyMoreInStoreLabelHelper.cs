using UnityEngine;

namespace UI.Helpers
{
	public class BuyMoreInStoreLabelHelper : MonoBehaviour
	{
		[SerializeField]
		private UILabel _buyMoreInStoreLabel;

		public void UpdateBuyMoreInStoreLabel()
		{
			_buyMoreInStoreLabel.text = Strings.Get("BUY_MORE_IN_STORE");
		}
	}
}
