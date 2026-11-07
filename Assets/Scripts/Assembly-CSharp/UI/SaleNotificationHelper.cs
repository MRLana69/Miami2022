using UnityEngine;

namespace UI
{
	public class SaleNotificationHelper : MonoBehaviour
	{
		[SerializeField]
		private UILabel _line1;

		[SerializeField]
		private UILabel _line2;

		private string _offerText;

		public void Init(string saleHeader, string subHeader, string saleText, string offerColorCode)
		{
			_offerText = string.Format(saleText, "[c]", offerColorCode, "[-]", "[/c]");
			_line1.text = saleHeader;
			_line2.text = string.Format(subHeader, _offerText);
		}
	}
}
