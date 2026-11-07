using System;
using TopRun;

public class TopRunCell
{
	public TopRunCellType cellType;

	public int rank = -1;

	public int bestScore = -1;

	public IPlayer player;

	public bool useDarkColour;

	public bool isLocalUser;

	public bool noPadding;

	public Action OnUpdateCell;

	public TopRunPlayerCell topRunPlayerCellScript;

	public void UpdateCell()
	{
		if (OnUpdateCell != null)
		{
			OnUpdateCell();
		}
	}
}
