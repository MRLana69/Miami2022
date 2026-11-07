using UnityEngine;

public class TopRunEmptyCell : MonoBehaviour
{
	[SerializeField]
	private UISprite background;

	public void InitCell(int rank)
	{
		switch (rank)
		{
		case 1:
			background.color = GlobalColors.TOPRUN_PLAYER_BACKGOURD_00_DARK_COLOUR;
			break;
		case 2:
			background.color = GlobalColors.TOPRUN_PLAYER_BACKGOURD_10_DARK_COLOUR;
			break;
		case 3:
			background.color = GlobalColors.TOPRUN_PLAYER_BACKGOURD_20_DARK_COLOUR;
			break;
		case 4:
			background.color = GlobalColors.TOPRUN_PLAYER_BACKGOURD_30_DARK_COLOUR;
			break;
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
			background.color = GlobalColors.TOPRUN_PLAYER_BACKGOURD_40_DARK_COLOUR;
			break;
		default:
			background.color = GlobalColors.TOPRUN_PLAYER_BACKGOURD_40_DARK_COLOUR;
			break;
		}
	}
}
