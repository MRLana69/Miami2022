using Data.Managers;
using Info;

public class BoardScreenManager
{
	private static BoardScreenManager _instance;

	private bool _cameBackFromBundle;

	public static BoardScreenManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new BoardScreenManager();
			}
			return _instance;
		}
	}

	public Boards.BoardId forceSpecificBoard { get; set; }

	public bool forceRefreshOnBoardScreen { get; set; }

	public bool cameBackFromBundle
	{
		get
		{
			return _cameBackFromBundle;
		}
		set
		{
			_cameBackFromBundle = value;
		}
	}

	public bool forceCenterOnSpecificBoard { get; set; }

	public bool willResetBoardAnimation { get; set; }

	public bool shouldResetThemeSelection { get; set; }

	public bool refreshBoardsWhenNotShowingPopup { get; set; }

	public Boards.BoardId currentlyShownBoardId { get; set; }

	public int currentlyShownThemeIndex { get; set; }

	private BoardScreenManager()
	{
	}

	public void ChangedDisplayedHoverboard(Boards.BoardId newBoard)
	{
		currentlyShownBoardId = newBoard;
		BoardManager.ResetDisplayedCustomSets(newBoard);
	}
}
