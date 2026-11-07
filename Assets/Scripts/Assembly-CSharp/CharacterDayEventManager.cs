using System;
using System.Collections.Generic;
using Data.Managers;
using Events;
using Info;
using SYBO.Subway.Info;

public class CharacterDayEventManager : IEventManager
{
	private SSEventType _eventType = SSEventType.CharacterDay;

	private long _eventID;

	public static CharacterDayEventManager Create(SSEventType eventType, long eventID)
	{
		CharacterDayEventManager characterDayEventManager = new CharacterDayEventManager();
		characterDayEventManager.Initialize(eventType, eventID);
		return characterDayEventManager;
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
		bool found = false;
		Characters.CharacterId eventValue = GetEventValue<Characters.CharacterId>(out found);
		if (AgeRestriction.isRestricted && eventValue == Characters.CharacterId.facebookRewardCharacter01 && !CharacterManager.IsCharacterOwned(eventValue))
		{
			return false;
		}
		return _eventID > 0 && found;
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
		firstLine = string.Empty;
		secondLine = string.Empty;
		bool found = false;
		Characters.CharacterId eventValue = GetEventValue<Characters.CharacterId>(out found);
		if (found)
		{
			firstLine = Strings.Get(Characters.characterData[eventValue].name).ToUpper();
			secondLine = Strings.Get("EVENTS_POPUP_THURSDAY_EVENT_TITLE_LINE_2");
			if (Globals.DEBUG_USE_EVENT_TESTER)
			{
				return;
			}
		}
		else
		{
			SDebug.LogError("The CharacterDay Event does not contain a valid character type!");
		}
		firstLine = Strings.GetLocalizedOnlineSettings("event_popup_title_first", firstLine);
		secondLine = Strings.GetLocalizedOnlineSettings("event_popup_title_second", secondLine);
	}

	public virtual void GetPopupDescription(out string firstTitle, out string secondTitle)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		bool found = false;
		Characters.CharacterId eventValue = GetEventValue<Characters.CharacterId>(out found);
		if (found)
		{
			string name = Characters.characterData[eventValue].name;
			text = Strings.Get("EVENTS_POPUP_DESCRIPTION_TITLE_CHARACTER_DAY");
			text2 = string.Format(Strings.Get("EVENTS_POPUP_DESCRIPTION_SECOND_TITLE_CHARACTER_DAY"), Strings.Get(name));
		}
		else
		{
			SDebug.LogError("CharacterDayEventManager - GetPopupDescription: The Character type in the Character Day Event does not exist.");
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
		string empty = string.Empty;
		string empty2 = string.Empty;
		empty = Strings.Get("MULTIPLIER_RIBBON_SCORE");
		empty2 = Strings.Get("MULTIPLIER_RIBBON_MULTIPLIER");
		firstLine = empty;
		secondLine = empty2;
		thirdLine = string.Empty;
	}

	public void SaveProgress()
	{
	}
}
