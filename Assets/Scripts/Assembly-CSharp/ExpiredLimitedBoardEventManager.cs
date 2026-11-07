using System;
using System.Collections.Generic;
using Data.Managers;
using Events;
using Info;

public class ExpiredLimitedBoardEventManager : IEventManager
{
	private SSEventType _eventType = SSEventType.ExpiredLimitedBoard;

	private long _eventID;

	public static ExpiredLimitedBoardEventManager Create(SSEventType eventType, long eventID)
	{
		ExpiredLimitedBoardEventManager expiredLimitedBoardEventManager = new ExpiredLimitedBoardEventManager();
		expiredLimitedBoardEventManager.Initialize(eventType, eventID);
		return expiredLimitedBoardEventManager;
	}

	private void Initialize(SSEventType eventType, long eventID)
	{
		_eventType = eventType;
		_eventID = eventID;
	}

	public SSEventType GetEventType()
	{
		return _eventType;
	}

	public long GetEventID()
	{
		return _eventID;
	}

	public bool IsEventActive()
	{
		return _eventID > 0 && ShouldEventBeAvailable();
	}

	private bool ShouldEventBeAvailable()
	{
		bool found = false;
		Boards.BoardId eventValue = GetEventValue<Boards.BoardId>(out found);
		if (found)
		{
			return !BoardManager.IsBoardOwned(eventValue);
		}
		return false;
	}

	public T GetEventValue<T>(out bool found)
	{
		T result = default(T);
		found = false;
		string text = ((!Globals.DEBUG_USE_EVENT_TESTER) ? OnlineSettings.instance.GetValue("event_value", string.Empty) : EventTesterManager.Instance.GetEventValue());
		if (!string.IsNullOrEmpty(text))
		{
			try
			{
				Dictionary<string, object> cachedEventValueDict = EventManagerUtils.GetCachedEventValueDict(typeof(T));
				result = (T)cachedEventValueDict[text];
				found = true;
			}
			catch (Exception ex)
			{
				found = false;
				SDebug.LogErrorFormat("The current event does not contain a value '{0}' of the specified type: {1}, stack trace: {2}", text, typeof(T), ex.StackTrace);
			}
		}
		return result;
	}

	public void GetPopupTitle(out string firstLine, out string secondLine)
	{
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			firstLine = Strings.Get("EVENTS_POPUP_WEDNESDAY_EVENT_TITLE_LINE_1");
			secondLine = Strings.Get("EVENTS_POPUP_WEDNESDAY_EVENT_TITLE_LINE_2");
		}
		else
		{
			firstLine = Strings.GetLocalizedOnlineSettings("event_popup_title_first", Strings.Get("EVENTS_POPUP_WEDNESDAY_EVENT_TITLE_LINE_1"));
			secondLine = Strings.GetLocalizedOnlineSettings("event_popup_title_second", Strings.Get("EVENTS_POPUP_WEDNESDAY_EVENT_TITLE_LINE_2"));
		}
	}

	public void GetPopupDescription(out string firstTitle, out string secondTitle)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		bool found = false;
		Boards.BoardId eventValue = GetEventValue<Boards.BoardId>(out found);
		if (found)
		{
			string name = Boards.boardData[eventValue].name;
			text = string.Format(Strings.Get("EVENTS_POPUP_DESCRIPTION_TITLE_COMEBACK"), Strings.Get(name));
			text2 = string.Format(Strings.Get("EVENTS_POPUP_DESCRIPTION_SECOND_TITLE_LIMITED_BOARD"), Strings.Get(name));
		}
		else
		{
			SDebug.LogError("ExpiredLimitedBoardEventManager - GetPopupDescription: The Board type does not exist.");
		}
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			firstTitle = text;
			secondTitle = text2;
		}
		else
		{
			firstTitle = Strings.GetLocalizedOnlineSettings("event_popup_description_first", text);
			secondTitle = Strings.GetLocalizedOnlineSettings("event_popup_description_second", text2);
		}
	}

	public void GetRibbonDescription(out string firstLine, out string secondLine, out string thirdLine)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		if (IsEventActive())
		{
			bool found = false;
			Boards.BoardId eventValue = GetEventValue<Boards.BoardId>(out found);
			if (found)
			{
				string name = Boards.boardData[eventValue].name;
				text = Strings.Get("EVENT_RIBBON_TEXT_WILD");
				text2 = Strings.Get("EVENT_RIBBON_TEXT_WEDNESDAY");
				text3 = string.Format(Strings.Get("EVENT_RIBBON_TEXT_COMEBACK"), Strings.Get(name));
			}
			if (Globals.DEBUG_USE_EVENT_TESTER)
			{
				firstLine = text;
				secondLine = text2;
				thirdLine = text3;
				return;
			}
			string valueString = string.Empty;
			if (OnlineSettings.instance.TryGetValue("event_ribbon_text", out valueString) && !string.IsNullOrEmpty(valueString))
			{
				string[] array = valueString.Split(';');
				text = array[0];
				text2 = array[1];
				text3 = array[2];
			}
		}
		firstLine = text;
		secondLine = text2;
		thirdLine = text3;
	}

	public void SaveProgress()
	{
	}
}
