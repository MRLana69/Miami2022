using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

namespace UI.Helpers
{
	public class FooterAnalyticsHelper : MonoBehaviour
	{
		[SerializeField]
		[Dropdown("_buttonNames")]
		private string _buttonName;

		[UsedImplicitly]
		private string[] _buttonNames = new string[9] { "Boosts", "Earn coins", "Store", "Top run", "High scores", "Collect", "Characters", "Boards", "Awards" };

		[UsedImplicitly]
		private void OnClick()
		{
			Flurry.LogEventWithAParameter("FooterButtonClicked", "button_name", _buttonName);
		}
	}
}
