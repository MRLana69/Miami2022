using UnityEngine;

namespace UI.Leaderboard
{
	public class LeaderboardHeaderHelper : MonoBehaviour
	{
		[SerializeField]
		private UILabel _label;

		[SerializeField]
		private UISprite _background;

		public void Setup(string text, Color backgroundColor)
		{
			_label.text = text;
			_background.color = backgroundColor;
		}
	}
}
