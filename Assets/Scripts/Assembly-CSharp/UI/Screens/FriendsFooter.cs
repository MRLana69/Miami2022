using Info;
using UnityEngine;

namespace UI.Screens
{
	public class FriendsFooter : MonoBehaviour
	{
		[SerializeField]
		private UILabel _menuLabel;

		[SerializeField]
		private UILabel _friendsLabel;

		[SerializeField]
		private UILabel _leaderboardLabel;

		[SerializeField]
		private UILabel _toprunLabel;

		[SerializeField]
		private GameObject _collectButton;

		private void OnEnable()
		{
			_menuLabel.text = Strings.Get("FOOTER_MENU_BUTTON_TITLE");
			_friendsLabel.text = Strings.Get("COLLECT_TITLE");
			_leaderboardLabel.text = Strings.Get("HIGH_SCORES_TITLE");
			_toprunLabel.text = Strings.Get("TOPRUN_TITLE");
			_collectButton.SetActive(!AgeRestriction.isRestricted);
		}
	}
}
