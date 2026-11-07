using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Data.Save;
using Info;
using Kiloo;
using Kiloo.Common.Extensions;
using SYBO.Subway.Promotion;
using UI.Popups.LimitedBoardPopupInternal;
using UI.Screens;
using WeeklyHunt;

namespace Data.Managers
{
	public static class BoardManager
	{
		public enum ScreenContext
		{
			BoardScreen = 0,
			LimitedBoardPopup = 1,
			BoardPromotionPopup = 2
		}

		private static readonly CultureInfo ONLINESETTINGS_DATETIME_CULTUREINFO;

		private static bool[] _displayCustomSets;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action boardSelected__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<Hoverboard.BoardAbility> themeActiveStateChanged__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<Boards.BoardId> boardUnlocked__BackingField;

		public const int MAX_THEMES_PER_BOARD = 2;

		public static event Action boardSelected
		{
			add
			{
				Action action = boardSelected__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref boardSelected__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = boardSelected__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref boardSelected__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<Hoverboard.BoardAbility> themeActiveStateChanged
		{
			add
			{
				Action<Hoverboard.BoardAbility> action = themeActiveStateChanged__BackingField;
				Action<Hoverboard.BoardAbility> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref themeActiveStateChanged__BackingField, (Action<Hoverboard.BoardAbility>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<Hoverboard.BoardAbility> action = themeActiveStateChanged__BackingField;
				Action<Hoverboard.BoardAbility> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref themeActiveStateChanged__BackingField, (Action<Hoverboard.BoardAbility>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<Boards.BoardId> boardUnlocked
		{
			add
			{
				Action<Boards.BoardId> action = boardUnlocked__BackingField;
				Action<Boards.BoardId> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref boardUnlocked__BackingField, (Action<Boards.BoardId>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<Boards.BoardId> action = boardUnlocked__BackingField;
				Action<Boards.BoardId> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref boardUnlocked__BackingField, (Action<Boards.BoardId>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		static BoardManager()
		{
			ONLINESETTINGS_DATETIME_CULTUREINFO = new CultureInfo("da-DK");
			_displayCustomSets = new bool[2];
			boardSelected__BackingField = null;
			themeActiveStateChanged__BackingField = null;
			ResetDisplayedCustomSets(PlayerSave.currentBoardId);
		}

		public static Boards.BoardId GetCurrentlyShownBoard(ScreenContext context)
		{
			switch (context)
			{
			case ScreenContext.BoardScreen:
				return BoardScreenManager.Instance.currentlyShownBoardId;
			case ScreenContext.LimitedBoardPopup:
				return LimitedBoardPopupManager.instance.currentBoardId;
			case ScreenContext.BoardPromotionPopup:
			{
				BoardPromotion boardPromotion = PromotionManager.Instance.TryGetCurrentPromotion<BoardPromotion>();
				if (boardPromotion != null)
				{
					return boardPromotion.BoardId;
				}
				break;
			}
			}
			return Boards.BoardId.normal;
		}

		public static int GetCurrentlyShownHoverboardTheme(ScreenContext context)
		{
			switch (context)
			{
			case ScreenContext.BoardScreen:
				return BoardScreenManager.Instance.currentlyShownThemeIndex;
			case ScreenContext.LimitedBoardPopup:
				return LimitedBoardPopupManager.instance.currentlyShownThemeIndex;
			default:
				return 0;
			}
		}

		public static void SelectCurrentlyShownBoard(ScreenContext context)
		{
			UIModelController.Instance.SelectCurrentlyShownBoard(GetCurrentlyShownBoard(context));
			boardSelected__BackingField.SafeInvoke();
		}

		public static void ChangeThemeActiveState(ScreenContext context, Hoverboard.BoardAbility boardAbility = Hoverboard.BoardAbility.None)
		{
			Boards.BoardId currentlyShownBoard = GetCurrentlyShownBoard(context);
			BoardScreenManager.Instance.ChangedDisplayedHoverboard(currentlyShownBoard);
			themeActiveStateChanged__BackingField.SafeInvoke(boardAbility);
		}

		public static void ResetDisplayedCustomSets(Boards.BoardId newBoard)
		{
			if (HoverboardThemes.hoverboardCustomThemes.ContainsKey(newBoard))
			{
				for (int i = 0; i < 2; i++)
				{
					_displayCustomSets[i] = IsThemeActiveForUIIndex(newBoard, i);
				}
			}
			else
			{
				for (int j = 0; j < 2; j++)
				{
					_displayCustomSets[j] = false;
				}
			}
		}

		private static bool IsThemeActiveForUIIndex(Boards.BoardId board, int uiIndex)
		{
			if (HoverboardThemes.hoverboardCustomThemes.ContainsKey(board))
			{
				foreach (HoverboardThemes.HoverboardTheme item in HoverboardThemes.hoverboardCustomThemes[board])
				{
					if (item.uiOrder == (HoverboardThemes.UIOrder)uiIndex)
					{
						return IsCustomSetEquippedForBoard(board, uiIndex);
					}
				}
			}
			return false;
		}

		public static bool IsCustomSetEquippedForBoard(Boards.BoardId boardId, int themeIndex)
		{
			if (HoverboardThemes.hoverboardCustomThemes.ContainsKey(boardId))
			{
				foreach (HoverboardThemes.HoverboardTheme item in HoverboardThemes.hoverboardCustomThemes[boardId])
				{
					if (themeIndex == (int)item.uiOrder)
					{
						return IsThemeActive(boardId, item.identifier);
					}
				}
			}
			return false;
		}

		public static bool ShouldCustomSetBeDisplayed(int themeIndex)
		{
			return _displayCustomSets[themeIndex];
		}

		public static void DisplayCustomSet(int themeIndex, bool display)
		{
			_displayCustomSets[themeIndex] = display;
		}

		public static Boards.BoardId? GetEventBoardType()
		{
			bool found = false;
			Boards.BoardId? boardId = null;
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			if (activeEventManager != null)
			{
				SSEventType eventType = activeEventManager.GetEventType();
				if (eventType == SSEventType.ExpiredLimitedBoard || eventType == SSEventType.SpecialPowerBoard)
				{
					boardId = activeEventManager.GetEventValue<Boards.BoardId>(out found);
				}
			}
			return (!found) ? ((Boards.BoardId?)null) : boardId;
		}

		public static bool IsBoardTrySpecialPowerBoard(Boards.BoardId boardId)
		{
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.SpecialPowerBoard);
			bool result = false;
			if (activeEventManager != null)
			{
				bool found;
				Boards.BoardId eventValue = activeEventManager.GetEventValue<Boards.BoardId>(out found);
				if (found && eventValue == boardId)
				{
					result = true;
				}
			}
			return result;
		}

		public static bool IsBoardEventLimitedExpiredBoard(Boards.BoardId boardId)
		{
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.ExpiredLimitedBoard);
			bool result = false;
			if (activeEventManager != null)
			{
				bool found;
				Boards.BoardId eventValue = activeEventManager.GetEventValue<Boards.BoardId>(out found);
				if (found && eventValue == boardId)
				{
					result = true;
				}
			}
			return result;
		}

		public static bool HasBoardBeenSeen(Boards.BoardId boardId)
		{
			if (!Boards.boardData[boardId].isNewInThisUpdate)
			{
				return true;
			}
			bool value;
			return UserSave.hasBoardBeenSeen.TryGetValue(boardId, out value) && value;
		}

		public static void MarkHoverboardAsSeen(Boards.BoardId boardId)
		{
			UserSave.hasBoardBeenSeen[boardId] = true;
		}

		public static void UnlockBoard(Boards.BoardId boardId)
		{
			InventorySave.unlockedBoards.AddDistinct(boardId);
			boardUnlocked__BackingField.SafeInvoke(boardId);
		}

		public static void LockBoard(Boards.BoardId boardId)
		{
			InventorySave.unlockedBoards.Remove(boardId);
		}

		public static int CountBoardThemesUnlocked()
		{
			int num = 0;
			foreach (KeyValuePair<Boards.BoardId, Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData>> boardThemeDatum in InventorySave.boardThemeData)
			{
				foreach (KeyValuePair<HoverboardThemes.ThemeIdentifier, BoardThemeData> item in boardThemeDatum.Value)
				{
					if (item.Value.isOwned)
					{
						num++;
					}
				}
			}
			return num;
		}

		public static bool IsBoardOwned(Boards.BoardId boardId)
		{
			if (Globals.DEBUG_ALL_BOARDS)
			{
				return true;
			}
			if (Boards.boardData[boardId].unlockType == PurchaseData.UnlockType.AlwaysUnlocked)
			{
				return true;
			}
			return InventorySave.unlockedBoards.Contains(boardId);
		}

		public static bool IsThemeOwned(Boards.BoardId boardId, HoverboardThemes.ThemeIdentifier themeId)
		{
			if (Globals.DEBUG_ALL_BOARDS)
			{
				return true;
			}
			if (themeId != HoverboardThemes.ThemeIdentifier.none && InventorySave.boardThemeData.ContainsKey(boardId))
			{
				Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData> dictionary = InventorySave.boardThemeData[boardId];
				if (dictionary != null && dictionary.ContainsKey(themeId))
				{
					return dictionary[themeId].isOwned;
				}
			}
			return false;
		}

		public static bool IsThemeActive(Boards.BoardId boardId, HoverboardThemes.ThemeIdentifier themeId)
		{
			if (themeId != HoverboardThemes.ThemeIdentifier.none && InventorySave.boardThemeData.ContainsKey(boardId))
			{
				Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData> dictionary = InventorySave.boardThemeData[boardId];
				if (dictionary != null && dictionary.ContainsKey(themeId))
				{
					return dictionary[themeId].isActive;
				}
			}
			return false;
		}

		public static bool IsBoardAbilityActive(Boards.BoardId boardId, Hoverboard.BoardAbility ability)
		{
			HoverboardThemes.HoverboardTheme themeAbility = HoverboardThemes.GetThemeAbility(boardId, ability);
			return PlayerSave.currentBoardId == boardId && themeAbility != null && IsThemeActive(boardId, themeAbility.identifier);
		}

		public static bool HasThemeBeenSeen(Boards.BoardId boardId, HoverboardThemes.ThemeIdentifier themeId)
		{
			if (themeId != HoverboardThemes.ThemeIdentifier.none && InventorySave.boardThemeData.ContainsKey(boardId))
			{
				Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData> dictionary = InventorySave.boardThemeData[boardId];
				if (dictionary != null && dictionary.ContainsKey(themeId))
				{
					return dictionary[themeId].hasBeenSeen;
				}
			}
			return false;
		}

		public static bool HasAllThemesBeenSeen(Boards.BoardId boardId)
		{
			Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData> value;
			if (InventorySave.boardThemeData.TryGetValue(boardId, out value))
			{
				if (value != null)
				{
					return value.All((KeyValuePair<HoverboardThemes.ThemeIdentifier, BoardThemeData> t) => t.Value.hasBeenSeen) && HoverboardThemes.hoverboardCustomThemes[boardId].Count == value.Count;
				}
				return false;
			}
			return false;
		}

		public static void SetThemeAcquired(Boards.BoardId boardId, HoverboardThemes.ThemeIdentifier themeId)
		{
			if (themeId != HoverboardThemes.ThemeIdentifier.none)
			{
				if (InventorySave.boardThemeData.ContainsKey(boardId))
				{
					Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData> dictionary = InventorySave.boardThemeData[boardId];
					if (dictionary != null && dictionary.ContainsKey(themeId))
					{
						BoardThemeData boardThemeData = InventorySave.boardThemeData[boardId][themeId];
						BoardThemeData boardThemeData2 = new BoardThemeData();
						boardThemeData2.isOwned = true;
						boardThemeData2.isActive = boardThemeData.isActive;
						boardThemeData2.hasBeenSeen = boardThemeData.hasBeenSeen;
						BoardThemeData value = boardThemeData2;
						InventorySave.boardThemeData[boardId][themeId] = value;
					}
					else
					{
						BoardThemeData boardThemeData2 = new BoardThemeData();
						boardThemeData2.isOwned = true;
						boardThemeData2.hasBeenSeen = true;
						boardThemeData2.isActive = false;
						BoardThemeData value2 = boardThemeData2;
						InventorySave.boardThemeData[boardId].Add(themeId, value2);
					}
				}
				else
				{
					Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData> dictionary2 = new Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData>();
					BoardThemeData boardThemeData2 = new BoardThemeData();
					boardThemeData2.isOwned = true;
					boardThemeData2.hasBeenSeen = true;
					boardThemeData2.isActive = false;
					BoardThemeData value3 = boardThemeData2;
					dictionary2.Add(themeId, value3);
					InventorySave.boardThemeData.Add(boardId, dictionary2);
				}
			}
			else
			{
				SDebug.LogError("Trying to acquire an invalid theme.");
			}
		}

		public static void SetThemeActive(Boards.BoardId boardId, HoverboardThemes.ThemeIdentifier themeId, bool active)
		{
			if (themeId != HoverboardThemes.ThemeIdentifier.none)
			{
				if (InventorySave.boardThemeData.ContainsKey(boardId))
				{
					Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData> dictionary = InventorySave.boardThemeData[boardId];
					if (dictionary != null && dictionary.ContainsKey(themeId))
					{
						if (InventorySave.boardThemeData[boardId][themeId].isOwned || Globals.DEBUG_ALL_BOARDS)
						{
							BoardThemeData boardThemeData = InventorySave.boardThemeData[boardId][themeId];
							BoardThemeData boardThemeData2 = new BoardThemeData();
							boardThemeData2.isOwned = boardThemeData.isOwned;
							boardThemeData2.isActive = active;
							boardThemeData2.hasBeenSeen = boardThemeData.hasBeenSeen;
							BoardThemeData value = boardThemeData2;
							InventorySave.boardThemeData[boardId][themeId] = value;
						}
						else
						{
							SDebug.LogError("Theme id " + themeId.ToString() + " for board " + boardId.ToString() + " is not owned or data is not available. Activation aborted.");
						}
					}
					else
					{
						SDebug.LogError("Theme id " + themeId.ToString() + " for board " + boardId.ToString() + " is not owned or data is not available. Activation aborted.");
					}
				}
				else
				{
					SDebug.LogError("Theme id " + themeId.ToString() + " for board " + boardId.ToString() + " is not owned or data is not available. Activation aborted.");
				}
			}
			else
			{
				SDebug.LogError("Trying to activate an invalid theme.");
			}
		}

		public static void SetThemeSeen(Boards.BoardId boardId, HoverboardThemes.ThemeIdentifier themeId)
		{
			if (themeId != HoverboardThemes.ThemeIdentifier.none)
			{
				if (InventorySave.boardThemeData.ContainsKey(boardId))
				{
					Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData> dictionary = InventorySave.boardThemeData[boardId];
					if (dictionary != null && dictionary.ContainsKey(themeId))
					{
						BoardThemeData boardThemeData = InventorySave.boardThemeData[boardId][themeId];
						BoardThemeData boardThemeData2 = new BoardThemeData();
						boardThemeData2.isOwned = boardThemeData.isOwned;
						boardThemeData2.isActive = boardThemeData.isActive;
						boardThemeData2.hasBeenSeen = true;
						BoardThemeData value = boardThemeData2;
						InventorySave.boardThemeData[boardId][themeId] = value;
					}
					else
					{
						BoardThemeData boardThemeData2 = new BoardThemeData();
						boardThemeData2.isOwned = false;
						boardThemeData2.hasBeenSeen = true;
						boardThemeData2.isActive = false;
						BoardThemeData value2 = boardThemeData2;
						InventorySave.boardThemeData[boardId].Add(themeId, value2);
					}
				}
				else
				{
					Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData> dictionary2 = new Dictionary<HoverboardThemes.ThemeIdentifier, BoardThemeData>();
					BoardThemeData boardThemeData2 = new BoardThemeData();
					boardThemeData2.isOwned = false;
					boardThemeData2.hasBeenSeen = true;
					boardThemeData2.isActive = false;
					BoardThemeData value3 = boardThemeData2;
					dictionary2.Add(themeId, value3);
					InventorySave.boardThemeData.Add(boardId, dictionary2);
				}
			}
			else
			{
				SDebug.LogError("Trying to set an invalid theme as seen.");
			}
		}

		public static bool DoesBoardHaveThemes(Boards.BoardId boardId)
		{
			return HoverboardThemes.hoverboardCustomThemes.ContainsKey(boardId);
		}

		public static bool AreAllNewThemesSeenForBoard(Boards.BoardId boardId)
		{
			bool flag = true;
			foreach (HoverboardThemes.HoverboardTheme item in HoverboardThemes.hoverboardCustomThemes[boardId])
			{
				if (item.isNewInThisUpdate)
				{
					flag = flag && HasThemeBeenSeen(boardId, item.identifier);
				}
			}
			return flag;
		}

		private static DateTime DefaultExpirationDateForHoverboard(Boards.BoardId bId)
		{
			if (Boards.boardData[bId].defaultExpirationTime != DateTime.MinValue)
			{
				return Boards.boardData[bId].defaultExpirationTime;
			}
			return DateTime.MaxValue;
		}

		public static DateTime BoardExpirationDate(Boards.BoardId boardId)
		{
			string key = "hoverboard_expiretime_" + boardId;
			DateTime result = DateTime.MinValue;
			string valueString;
			if (!OnlineSettings.instance.HasValue(key))
			{
				result = DefaultExpirationDateForHoverboard(boardId);
			}
			else if (OnlineSettings.instance.TryGetValue(key, out valueString) && valueString != "disabled")
			{
				try
				{
					result = DateTime.Parse(valueString, ONLINESETTINGS_DATETIME_CULTUREINFO);
				}
				catch (FormatException)
				{
					result = DefaultExpirationDateForHoverboard(boardId);
				}
			}
			return result;
		}

		public static DateTime BoardStartAvailabilityDate(Boards.BoardId boardId)
		{
			string key = "hoverboard_starttime_" + boardId;
			DateTime result = DateTime.MinValue;
			string valueString;
			if (!OnlineSettings.instance.HasValue(key))
			{
				result = ((!(Boards.boardData[boardId].defaultStartDate != DateTime.MinValue)) ? DateTime.MinValue : Boards.boardData[boardId].defaultStartDate);
			}
			else if (OnlineSettings.instance.TryGetValue(key, out valueString) && valueString != "disabled")
			{
				try
				{
					result = DateTime.Parse(valueString, ONLINESETTINGS_DATETIME_CULTUREINFO);
				}
				catch (FormatException)
				{
				}
			}
			return result;
		}

		public static bool IsBoardActive(Boards.BoardId boardId)
		{
			if (Globals.DEBUG_ALL_BOARDS || Globals.DEBUG_DISPLAY_ALL_BOARDS)
			{
				return true;
			}
			Boards.Board board = Boards.boardData[boardId];
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager(SSEventType.ExpiredLimitedBoard);
			if (activeEventManager != null)
			{
				bool found;
				Boards.BoardId eventValue = activeEventManager.GetEventValue<Boards.BoardId>(out found);
				if (found && eventValue == boardId)
				{
					return true;
				}
			}
			if (board.unlockType == PurchaseData.UnlockType.HiddenUntilUnlocked)
			{
				return false;
			}
			if (board.unlockType == PurchaseData.UnlockType.WeeklyHunt && !WeeklyHuntManager.IsBoardAwardedInCurrentPeriod(boardId))
			{
				return false;
			}
			if (!board.hasOnlineTimeLimit)
			{
				return true;
			}
			if (IsBoardDisabled(boardId))
			{
				return false;
			}
			DateTime t = BoardExpirationDate(boardId);
			DateTime t2 = BoardStartAvailabilityDate(boardId);
			if (DateTime.Compare(t, CDateTime.Now) > 0 && DateTime.Compare(t2, CDateTime.Now) < 0)
			{
				return true;
			}
			return false;
		}

		public static bool IsBoardDisabled(Boards.BoardId boardId)
		{
			string key = "hoverboard_expiretime_" + boardId;
			string valueString;
			if (OnlineSettings.instance.HasValue(key) && OnlineSettings.instance.TryGetValue(key, out valueString) && valueString == "disabled")
			{
				return true;
			}
			return false;
		}

		public static TimeSpan BoardExpirationTimespan(Boards.BoardId boardId)
		{
			string key = "hoverboard_expiretime_" + boardId;
			DateTime dateTime = CDateTime.Now;
			string valueString;
			if (!OnlineSettings.instance.HasValue(key))
			{
				dateTime = ((!(Boards.boardData[boardId].defaultExpirationTime != DateTime.MinValue)) ? DateTime.MaxValue : Boards.boardData[boardId].defaultExpirationTime);
			}
			else if (OnlineSettings.instance.TryGetValue(key, out valueString))
			{
				dateTime = ((!(valueString == "disabled")) ? BoardExpirationDate(boardId) : CDateTime.Now);
			}
			if (dateTime == DateTime.MinValue)
			{
				dateTime = CDateTime.Now;
			}
			return dateTime - CDateTime.Now;
		}
	}
}
