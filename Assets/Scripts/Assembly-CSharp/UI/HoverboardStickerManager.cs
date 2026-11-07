using System.Collections.Generic;
using System.Linq;
using Data.Managers;
using Data.Save;
using Info;

namespace UI
{
	public static class HoverboardStickerManager
	{
		public static bool ShouldShowHoverboardStickerFlow()
		{
			return OnboardingSave.ShouldShowStickersForHoverboards();
		}

		public static bool IsAnyBoardOrAbilityFree()
		{
			foreach (Boards.BoardId boardId in Boards.boardOrder)
			{
				Boards.Board board = Boards.boardData[boardId];
				PurchaseData.UnlockType unlockType = board.unlockType;
				if ((unlockType == PurchaseData.UnlockType.Coins || unlockType == PurchaseData.UnlockType.Keys) && board.price == 0 && !BoardManager.IsBoardOwned(boardId) && BoardManager.IsBoardActive(boardId))
				{
					return true;
				}
				if (HoverboardThemes.hoverboardCustomThemes.ContainsKey(boardId) && HoverboardThemes.hoverboardCustomThemes[boardId].Any((HoverboardThemes.HoverboardTheme ht) => ht.price == 0 && (ht.unlockType == PurchaseData.UnlockType.Coins || ht.unlockType == PurchaseData.UnlockType.Keys) && !BoardManager.IsThemeOwned(boardId, ht.identifier)) && BoardManager.IsBoardActive(boardId))
				{
					return true;
				}
			}
			return false;
		}

		public static int GetNumberOfNewHoverboardsAndThemes()
		{
			int num = 0;
			int num2 = 0;
			if (!UserSave.isFreshInstall)
			{
				foreach (Boards.BoardId item in Boards.boardOrder)
				{
					Boards.Board board = Boards.boardData[item];
					if (board.isNewInThisUpdate && BoardManager.IsBoardActive(item) && !BoardManager.HasBoardBeenSeen(item) && !BoardManager.IsBoardOwned(item))
					{
						num++;
					}
				}
				foreach (KeyValuePair<Boards.BoardId, List<HoverboardThemes.HoverboardTheme>> hoverboardCustomTheme in HoverboardThemes.hoverboardCustomThemes)
				{
					if (!BoardManager.IsBoardActive(hoverboardCustomTheme.Key))
					{
						continue;
					}
					foreach (HoverboardThemes.HoverboardTheme item2 in hoverboardCustomTheme.Value)
					{
						if (!BoardManager.HasThemeBeenSeen(hoverboardCustomTheme.Key, item2.identifier) && item2.isNewInThisUpdate)
						{
							num2++;
						}
					}
				}
			}
			return num + num2;
		}
	}
}
