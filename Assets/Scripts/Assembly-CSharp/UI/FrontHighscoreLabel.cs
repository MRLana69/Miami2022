using AppRefresh;
using Data.Save;
using UnityEngine;

namespace UI
{
	public class FrontHighscoreLabel : MonoBehaviour
	{
		[SerializeField]
		private UILabel _highScoreLabel;

		[SerializeField]
		private UILabel _titleLabel;

		private void OnEnable()
		{
			PlayerSave.highScoreChanged += UpdateHighScore;
			AppRefreshHandler.hasRefreshed += OnAppHasRefreshed;
			UpdateTitleLabel();
			UpdateHighScore();
		}

		private void OnDisable()
		{
			PlayerSave.highScoreChanged -= UpdateHighScore;
			AppRefreshHandler.hasRefreshed -= OnAppHasRefreshed;
		}

		private void UpdateTitleLabel()
		{
			_titleLabel.text = Strings.Get("FRONT_UI_SCORE_TITLE");
		}

		private void OnAppHasRefreshed(RefreshType refreshType)
		{
			UpdateTitleLabel();
			UpdateHighScore();
		}

		private void UpdateHighScore()
		{
			_highScoreLabel.text = string.Format("{0:0000}", PlayerSave.highScore);
		}
	}
}
