public class SuperMysteryBoxEventManager : IEventManager
{
	private SSEventType _eventType = SSEventType.SuperMysteryBox;

	private long _eventID;

	public static SuperMysteryBoxEventManager Create(SSEventType eventType, long eventID)
	{
		SuperMysteryBoxEventManager superMysteryBoxEventManager = new SuperMysteryBoxEventManager();
		superMysteryBoxEventManager.Initialize(eventType, eventID);
		return superMysteryBoxEventManager;
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
		return _eventID > 0;
	}

	public T GetEventValue<T>(out bool found)
	{
		T result = default(T);
		found = false;
		SDebug.LogError("The Super Mystery Box Event does not contain a value of the specified type: " + typeof(T));
		return result;
	}

	public void GetPopupTitle(out string firstLine, out string secondLine)
	{
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			firstLine = Strings.Get("EVENTS_POPUP_FRIDAY_EVENT_TITLE_LINE_1");
			secondLine = Strings.Get("EVENTS_POPUP_FRIDAY_EVENT_TITLE_LINE_2");
		}
		else
		{
			firstLine = Strings.GetLocalizedOnlineSettings("event_popup_title_first", Strings.Get("EVENTS_POPUP_FRIDAY_EVENT_TITLE_LINE_1"));
			secondLine = Strings.GetLocalizedOnlineSettings("event_popup_title_second", Strings.Get("EVENTS_POPUP_FRIDAY_EVENT_TITLE_LINE_2"));
		}
	}

	public void GetPopupDescription(out string firstTitle, out string secondTitle)
	{
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			firstTitle = Strings.Get("EVENTS_POPUP_DESCRIPTION_TITLE_SMB");
			secondTitle = Strings.Get("EVENTS_POPUP_DESCRIPTION_SECOND_TITLE_SMB");
		}
		else
		{
			firstTitle = Strings.GetLocalizedOnlineSettings("event_popup_description_first", Strings.Get("EVENTS_POPUP_DESCRIPTION_TITLE_SMB"));
			secondTitle = Strings.GetLocalizedOnlineSettings("event_popup_description_second", Strings.Get("EVENTS_POPUP_DESCRIPTION_SECOND_TITLE_SMB"));
		}
	}

	public void GetRibbonDescription(out string firstLine, out string secondLine, out string thirdLine)
	{
		firstLine = string.Empty;
		secondLine = string.Empty;
		thirdLine = string.Empty;
		SDebug.LogError("The Super Mystery Box Event does not contain information for a ribbon description");
	}

	public void SaveProgress()
	{
	}
}
