using JetBrains.Annotations;
using UnityEngine;

namespace UI.Screens
{
	public class GameOverTabHelper : MonoBehaviour
	{
		[SerializeField]
		private GameObject _leaderboardTabActive;

		[SerializeField]
		private GameObject _leaderboardTabInactive;

		[SerializeField]
		private UILabel _leaderboardTabLabel;

		[SerializeField]
		private GameObject _toprunTabActive;

		[SerializeField]
		private GameObject _toprunTabInactive;

		[SerializeField]
		private UILabel _toprunTabLabel;

		[SerializeField]
		private UISprite _toprunTabFlag;

		private const int TOPRUN_LABEL_PIXEL_OFFSET_X = 14;

		private const int TOPRUN_LABEL_PIXEL_OFFSET_Y = -2;

		private void Awake()
		{
			LeaderboardTapped();
		}

		private void OnEnable()
		{
			RefreshFlag();
		}

		public void RefreshFlag()
		{
			_leaderboardTabLabel.text = Strings.Get("HIGH_SCORES_TITLE");
			_toprunTabLabel.text = Strings.Get("TOPRUN_TITLE");
			_toprunTabFlag.spriteName = CountryManager.instance.GetCurrentTournamentCountry().flag;
			UIAnchor component = _toprunTabLabel.GetComponent<UIAnchor>();
			if (_toprunTabFlag.spriteName == string.Empty)
			{
				component.pixelOffset = new Vector2(0f, -2f);
			}
			else
			{
				component.pixelOffset = new Vector2(14f, -2f);
			}
		}

		[UsedImplicitly]
		public void LeaderboardTapped()
		{
			RefreshFlag();
			_leaderboardTabActive.SetActive(true);
			_leaderboardTabInactive.SetActive(false);
			SetLabelActive(_leaderboardTabLabel, true);
			_toprunTabActive.SetActive(false);
			_toprunTabInactive.SetActive(true);
			SetLabelActive(_toprunTabLabel, false);
		}

		[UsedImplicitly]
		public void TopRunTapped()
		{
			RefreshFlag();
			_leaderboardTabActive.SetActive(false);
			_leaderboardTabInactive.SetActive(true);
			SetLabelActive(_leaderboardTabLabel, false);
			_toprunTabActive.SetActive(true);
			_toprunTabInactive.SetActive(false);
			SetLabelActive(_toprunTabLabel, true);
		}

		private void SetLabelActive(UILabel label, bool isActive)
		{
			if (isActive)
			{
				label.color = new Color32(28, 103, 156, byte.MaxValue);
				label.effectColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			}
			else
			{
				label.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				label.effectColor = new Color32(0, 0, 0, byte.MaxValue);
			}
		}
	}
}
