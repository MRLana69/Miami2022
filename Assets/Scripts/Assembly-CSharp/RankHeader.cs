using UnityEngine;

public class RankHeader : MonoBehaviour
{
	[SerializeField]
	private UILabel _title;

	[SerializeField]
	private UILabel _best;

	[SerializeField]
	private UILabel _rankMedal;

	[SerializeField]
	private UISprite _background;

	[SerializeField]
	private UISprite _backgroundCorners;

	public void InitHeader(int rank, int bestScore)
	{
		SetTextAndColors(rank, bestScore);
	}

	private void SetTextAndColors(int rank, int score)
	{
		bool flag = false;
		_title.gameObject.SetActive(true);
		switch (rank)
		{
		case 1:
		{
			_title.text = Strings.Get("TOPRUN_RANK_1_TITLE");
			_rankMedal.text = Strings.Get("TOPRUN_RANK_1_TITLE");
			Color titleColor = GlobalColors.TOPRUN_RANK_HEADER_00_TITLE_COLOUR;
			Color shadowColor = GlobalColors.TOPRUN_RANK_HEADER_00_TITLE_SHADOW_COLOUR;
			Color backgroundColor = GlobalColors.TOPRUN_RANK_HEADER_00_BACKGROUND_COLOUR;
			SetColors(titleColor, shadowColor, backgroundColor);
			flag = true;
			break;
		}
		case 2:
		{
			_title.text = Strings.Get("TOPRUN_RANK_2_TITLE");
			_rankMedal.text = Strings.Get("TOPRUN_RANK_2_TITLE");
			Color titleColor = GlobalColors.TOPRUN_RANK_HEADER_10_TITLE_COLOUR;
			Color shadowColor = GlobalColors.TOPRUN_RANK_HEADER_10_TITLE_SHADOW_COLOUR;
			Color backgroundColor = GlobalColors.TOPRUN_RANK_HEADER_10_BACKGROUND_COLOUR;
			SetColors(titleColor, shadowColor, backgroundColor);
			break;
		}
		case 3:
		{
			_title.text = Strings.Get("TOPRUN_RANK_3_TITLE");
			_rankMedal.text = Strings.Get("TOPRUN_RANK_3_TITLE");
			Color titleColor = GlobalColors.TOPRUN_RANK_HEADER_20_TITLE_COLOUR;
			Color shadowColor = GlobalColors.TOPRUN_RANK_HEADER_20_TITLE_SHADOW_COLOUR;
			Color backgroundColor = GlobalColors.TOPRUN_RANK_HEADER_20_BACKGROUND_COLOUR;
			SetColors(titleColor, shadowColor, backgroundColor);
			break;
		}
		case 4:
		{
			_title.text = Strings.Get("TOPRUN_RANK_4_TITLE");
			_rankMedal.text = Strings.Get("TOPRUN_RANK_4_TITLE");
			Color titleColor = GlobalColors.TOPRUN_RANK_HEADER_30_TITLE_COLOUR;
			Color shadowColor = GlobalColors.TOPRUN_RANK_HEADER_30_TITLE_SHADOW_COLOUR;
			Color backgroundColor = GlobalColors.TOPRUN_RANK_HEADER_30_BACKGROUND_COLOUR;
			SetColors(titleColor, shadowColor, backgroundColor);
			break;
		}
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
		{
			_title.gameObject.SetActive(false);
			_rankMedal.text = string.Empty;
			Color titleColor = GlobalColors.TOPRUN_RANK_HEADER_40_TITLE_COLOUR;
			Color shadowColor = GlobalColors.TOPRUN_RANK_HEADER_40_TITLE_SHADOW_COLOUR;
			Color backgroundColor = GlobalColors.TOPRUN_RANK_HEADER_40_BACKGROUND_COLOUR;
			SetColors(titleColor, shadowColor, backgroundColor);
			break;
		}
		default:
			_title.text = string.Empty;
			_rankMedal.text = string.Empty;
			_best.text = string.Empty;
			break;
		}
		if (score < 0 || flag)
		{
			_best.text = string.Empty;
		}
		else
		{
			_best.text = string.Format(Strings.Get("TOPRUN_RANK_BEST"), score);
		}
		_best.gameObject.SetActive(!flag);
		_rankMedal.gameObject.SetActive(false);
	}

	private void SetColors(Color titleColor, Color shadowColor, Color backgroundColor)
	{
		_background.color = backgroundColor;
		_backgroundCorners.color = backgroundColor;
		_title.color = titleColor;
		_best.color = titleColor;
		_title.effectColor = shadowColor;
		_rankMedal.color = titleColor;
	}
}
