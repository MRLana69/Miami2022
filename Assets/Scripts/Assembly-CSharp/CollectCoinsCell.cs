using System;

public class CollectCoinsCell
{
	public CollectCoinsCellType cellType;

	public Friend friend;

	public CollectCoinsDummy dummyFriend;

	public bool useDarkColour;

	public bool disableScrolling;

	public Action OnUpdateCell;

	public void UpdateCell()
	{
		if (OnUpdateCell != null)
		{
			OnUpdateCell();
		}
	}
}
