using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Data.Managers;
using Data.Save;
using Info;
using Kiloo.Common.Extensions;
using Missions;
using SYBO.Subway.Info;
using UI.Screens.RewardManagement;
using UI.SlideIns;

public class CharacterNameEventManager : IEventManager
{
	public class LetterCollectedEventArgs : EventArgs
	{
		private char _characterCollected;

		public char CharacterCollected
		{
			get
			{
				return _characterCollected;
			}
		}

		public LetterCollectedEventArgs(char characterCollected)
		{
			_characterCollected = characterCollected;
		}

		public override string ToString()
		{
			return string.Format("[LetterCollectedEventArgs: l={0}", _characterCollected);
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static EventHandler<LetterCollectedEventArgs> OnLetterCollected__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<Characters.CharacterId, int> OnCharacterSkippedWithKeys__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<Characters.CharacterId> OnCharacterSkippedWithVideo__BackingField;

	public const string ONLINE_SETTING_DESCRIPTION_FIRST_UNLOCK_CHARATER_NAME = "event_popup_description_first_unlock_character_name";

	public const string ONLINE_SETTING_DESCRIPTION_SECOND_UNLOCK_CHARACTER_NAME = "event_popup_description_second_unlock_character_name";

	public const string ONLINE_SETTING_DESCRIPTION_FIRST_SELECT_CHARACTER_NAME = "event_popup_description_first_select_character_name";

	public const string ONLINE_SETTING_DESCRIPTION_SECOND_SELECT_CHARACTER_NAME = "event_popup_description_second_select_character_name";

	public const int DEFAULT_VALUE_FOR_COLLECT = -1;

	public static readonly char[] FILTERED_CHARACTERS = new char[1] { ' ' };

	private SSEventType _eventType = SSEventType.CharacterName;

	private long _eventID;

	private CharacterNameEventData _eventProgressData;

	private bool _hasAddedAdSystemInitialisationEventListener;

	private Characters.CharacterId?[][] normalCharacters = new Characters.CharacterId?[12][]
	{
		new Characters.CharacterId?[36]
		{
			Characters.CharacterId.slick,
			null,
			Characters.CharacterId.facebookRewardCharacter01,
			Characters.CharacterId.tasha,
			null,
			null,
			Characters.CharacterId.fresh,
			Characters.CharacterId.frizzy,
			null,
			null,
			Characters.CharacterId.spike,
			Characters.CharacterId.lucy,
			Characters.CharacterId.brody,
			Characters.CharacterId.tricky,
			null,
			null,
			Characters.CharacterId.king,
			Characters.CharacterId.zoe,
			null,
			null,
			Characters.CharacterId.frank,
			Characters.CharacterId.yutani,
			null,
			null,
			Characters.CharacterId.princek,
			Characters.CharacterId.ninja,
			null,
			null,
			Characters.CharacterId.tag,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterId?[36]
		{
			Characters.CharacterId.lucy,
			Characters.CharacterId.facebookRewardCharacter01,
			null,
			Characters.CharacterId.spike,
			Characters.CharacterId.frizzy,
			null,
			Characters.CharacterId.zoe,
			Characters.CharacterId.fresh,
			null,
			Characters.CharacterId.princek,
			Characters.CharacterId.tricky,
			Characters.CharacterId.ninja,
			Characters.CharacterId.tasha,
			null,
			Characters.CharacterId.yutani,
			Characters.CharacterId.tag,
			null,
			Characters.CharacterId.slick,
			null,
			Characters.CharacterId.king,
			null,
			Characters.CharacterId.brody,
			null,
			Characters.CharacterId.frank,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterId?[36]
		{
			Characters.CharacterId.ninja,
			null,
			Characters.CharacterId.facebookRewardCharacter01,
			Characters.CharacterId.tasha,
			null,
			Characters.CharacterId.yutani,
			Characters.CharacterId.brody,
			null,
			Characters.CharacterId.fresh,
			Characters.CharacterId.zoe,
			null,
			null,
			Characters.CharacterId.princek,
			Characters.CharacterId.frizzy,
			null,
			Characters.CharacterId.tricky,
			Characters.CharacterId.frank,
			null,
			Characters.CharacterId.spike,
			Characters.CharacterId.lucy,
			null,
			null,
			Characters.CharacterId.slick,
			Characters.CharacterId.tag,
			null,
			null,
			Characters.CharacterId.king,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterId?[36]
		{
			Characters.CharacterId.tag,
			null,
			null,
			Characters.CharacterId.tricky,
			Characters.CharacterId.facebookRewardCharacter01,
			null,
			null,
			Characters.CharacterId.lucy,
			Characters.CharacterId.spike,
			null,
			Characters.CharacterId.princek,
			Characters.CharacterId.tasha,
			null,
			Characters.CharacterId.zoe,
			Characters.CharacterId.fresh,
			null,
			Characters.CharacterId.slick,
			Characters.CharacterId.frizzy,
			null,
			null,
			Characters.CharacterId.brody,
			Characters.CharacterId.yutani,
			null,
			null,
			Characters.CharacterId.king,
			Characters.CharacterId.frank,
			null,
			null,
			Characters.CharacterId.ninja,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterId?[37]
		{
			Characters.CharacterId.king,
			null,
			Characters.CharacterId.facebookRewardCharacter01,
			Characters.CharacterId.frizzy,
			null,
			null,
			Characters.CharacterId.spike,
			Characters.CharacterId.zoe,
			null,
			null,
			null,
			Characters.CharacterId.yutani,
			Characters.CharacterId.princek,
			null,
			null,
			Characters.CharacterId.lucy,
			Characters.CharacterId.frank,
			null,
			null,
			Characters.CharacterId.tricky,
			Characters.CharacterId.ninja,
			null,
			Characters.CharacterId.fresh,
			Characters.CharacterId.tag,
			null,
			null,
			Characters.CharacterId.brody,
			null,
			null,
			Characters.CharacterId.slick,
			null,
			Characters.CharacterId.tasha,
			null,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterId?[37]
		{
			Characters.CharacterId.tricky,
			Characters.CharacterId.princek,
			null,
			Characters.CharacterId.facebookRewardCharacter01,
			Characters.CharacterId.zoe,
			null,
			null,
			null,
			Characters.CharacterId.yutani,
			Characters.CharacterId.slick,
			null,
			Characters.CharacterId.king,
			Characters.CharacterId.tasha,
			null,
			null,
			null,
			Characters.CharacterId.frizzy,
			Characters.CharacterId.fresh,
			null,
			null,
			null,
			Characters.CharacterId.frank,
			Characters.CharacterId.lucy,
			null,
			null,
			Characters.CharacterId.spike,
			null,
			null,
			Characters.CharacterId.tag,
			null,
			Characters.CharacterId.ninja,
			null,
			Characters.CharacterId.brody,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterId?[37]
		{
			Characters.CharacterId.tasha,
			Characters.CharacterId.facebookRewardCharacter01,
			null,
			null,
			Characters.CharacterId.yutani,
			Characters.CharacterId.tag,
			null,
			null,
			null,
			Characters.CharacterId.frank,
			Characters.CharacterId.tricky,
			null,
			null,
			Characters.CharacterId.brody,
			Characters.CharacterId.frizzy,
			Characters.CharacterId.spike,
			Characters.CharacterId.zoe,
			null,
			null,
			null,
			null,
			Characters.CharacterId.fresh,
			Characters.CharacterId.lucy,
			Characters.CharacterId.king,
			Characters.CharacterId.ninja,
			null,
			null,
			Characters.CharacterId.princek,
			null,
			null,
			Characters.CharacterId.slick,
			null,
			null,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterId?[37]
		{
			Characters.CharacterId.facebookRewardCharacter01,
			null,
			Characters.CharacterId.slick,
			Characters.CharacterId.zoe,
			null,
			null,
			Characters.CharacterId.fresh,
			Characters.CharacterId.ninja,
			null,
			null,
			Characters.CharacterId.spike,
			Characters.CharacterId.tasha,
			null,
			null,
			null,
			null,
			Characters.CharacterId.princek,
			Characters.CharacterId.yutani,
			null,
			null,
			Characters.CharacterId.frank,
			Characters.CharacterId.tag,
			null,
			null,
			Characters.CharacterId.king,
			Characters.CharacterId.tricky,
			null,
			null,
			Characters.CharacterId.brody,
			Characters.CharacterId.frizzy,
			null,
			Characters.CharacterId.lucy,
			null,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterId?[38]
		{
			Characters.CharacterId.zoe,
			Characters.CharacterId.zombiejake,
			Characters.CharacterId.yutani,
			Characters.CharacterId.facebookRewardCharacter01,
			null,
			Characters.CharacterId.ninja,
			Characters.CharacterId.tasha,
			null,
			Characters.CharacterId.tricky,
			Characters.CharacterId.princek,
			null,
			null,
			Characters.CharacterId.lucy,
			Characters.CharacterId.tag,
			null,
			null,
			Characters.CharacterId.frizzy,
			Characters.CharacterId.spike,
			null,
			null,
			null,
			Characters.CharacterId.fresh,
			Characters.CharacterId.frank,
			null,
			null,
			Characters.CharacterId.king,
			null,
			null,
			Characters.CharacterId.brody,
			null,
			null,
			Characters.CharacterId.slick,
			null,
			null,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterId?[38]
		{
			Characters.CharacterId.zombiejake,
			null,
			null,
			Characters.CharacterId.zoe,
			Characters.CharacterId.facebookRewardCharacter01,
			null,
			null,
			Characters.CharacterId.yutani,
			Characters.CharacterId.frank,
			null,
			null,
			Characters.CharacterId.ninja,
			Characters.CharacterId.fresh,
			null,
			null,
			Characters.CharacterId.frizzy,
			Characters.CharacterId.slick,
			null,
			null,
			Characters.CharacterId.tricky,
			Characters.CharacterId.princek,
			null,
			null,
			Characters.CharacterId.lucy,
			Characters.CharacterId.king,
			null,
			null,
			Characters.CharacterId.tasha,
			Characters.CharacterId.brody,
			null,
			null,
			Characters.CharacterId.tag,
			Characters.CharacterId.spike,
			null,
			null,
			null,
			null,
			null
		},
		new Characters.CharacterId?[39]
		{
			Characters.CharacterId.elftricky,
			Characters.CharacterId.facebookRewardCharacter01,
			null,
			null,
			null,
			Characters.CharacterId.londonCharacter02,
			Characters.CharacterId.frizzy,
			null,
			null,
			Characters.CharacterId.spike,
			Characters.CharacterId.lucy,
			null,
			Characters.CharacterId.yutani,
			Characters.CharacterId.slick,
			null,
			null,
			null,
			Characters.CharacterId.princek,
			Characters.CharacterId.tricky,
			null,
			null,
			Characters.CharacterId.brody,
			Characters.CharacterId.zoe,
			null,
			null,
			Characters.CharacterId.fresh,
			Characters.CharacterId.tasha,
			null,
			null,
			Characters.CharacterId.ninja,
			null,
			Characters.CharacterId.frank,
			null,
			null,
			Characters.CharacterId.tag,
			null,
			null,
			Characters.CharacterId.king,
			null
		},
		new Characters.CharacterId?[39]
		{
			Characters.CharacterId.slick,
			Characters.CharacterId.elftricky,
			Characters.CharacterId.londonCharacter02,
			null,
			null,
			Characters.CharacterId.yutani,
			Characters.CharacterId.facebookRewardCharacter01,
			null,
			null,
			Characters.CharacterId.tasha,
			Characters.CharacterId.fresh,
			null,
			null,
			Characters.CharacterId.zoe,
			Characters.CharacterId.spike,
			null,
			null,
			Characters.CharacterId.frizzy,
			Characters.CharacterId.king,
			null,
			null,
			Characters.CharacterId.lucy,
			Characters.CharacterId.princek,
			null,
			null,
			Characters.CharacterId.tricky,
			Characters.CharacterId.frank,
			null,
			null,
			Characters.CharacterId.brody,
			Characters.CharacterId.tag,
			null,
			Characters.CharacterId.ninja,
			null,
			null,
			null,
			null,
			null,
			null
		}
	};

	private Characters.CharacterId[] _limitedCharacters = new Characters.CharacterId[45]
	{
		Characters.CharacterId.arabiaCharacter01,
		Characters.CharacterId.losangelesCharacter01,
		Characters.CharacterId.carmen,
		Characters.CharacterId.roberto,
		Characters.CharacterId.kim,
		Characters.CharacterId.hawaiiCharacter01,
		Characters.CharacterId.vancouverCharacter01,
		Characters.CharacterId.tony,
		Characters.CharacterId.bangkokCharacter01,
		Characters.CharacterId.nick,
		Characters.CharacterId.cairoCharacter01,
		Characters.CharacterId.halloweenCharacter01,
		Characters.CharacterId.seoulCharacter01,
		Characters.CharacterId.saopauloCharacter01,
		Characters.CharacterId.mexicoCharacter01,
		Characters.CharacterId.lasVegasCharacter01,
		Characters.CharacterId.harumi,
		Characters.CharacterId.monkeyking,
		Characters.CharacterId.coco,
		Characters.CharacterId.mumbaiCharacter01,
		Characters.CharacterId.moscowCharacter01,
		Characters.CharacterId.veniceCharacter01,
		Characters.CharacterId.kenyaCharacter01,
		Characters.CharacterId.greeceCharacter01,
		Characters.CharacterId.sanfranciscoCharacter01,
		Characters.CharacterId.transylvaniaCharacter01,
		Characters.CharacterId.madagascarCharacter01,
		Characters.CharacterId.pragueCharacter01,
		Characters.CharacterId.singaporeCharacter01,
		Characters.CharacterId.peruCharacter01,
		Characters.CharacterId.havanaCharacter01,
		Characters.CharacterId.icelandCharacter01,
		Characters.CharacterId.washingtonCharacter01,
		Characters.CharacterId.holidayCharacter2016,
		Characters.CharacterId.amsterdamCharacter01,
		Characters.CharacterId.copenhagenCharacter01,
		Characters.CharacterId.marrakeshCharacter01,
		Characters.CharacterId.shanghaiCharacter01,
		Characters.CharacterId.barcelonaCharacter01,
		Characters.CharacterId.chicagoCharacter01,
		Characters.CharacterId.buenosAiresCharacter01,
		Characters.CharacterId.veniceBeachCharacter01,
		Characters.CharacterId.berlinCharacter01,
		Characters.CharacterId.hongKongCharacter01,
		Characters.CharacterId.atlantaCharacter01
	};

	private Characters.CharacterId?[][] _musicCharacters = new Characters.CharacterId?[1][] { new Characters.CharacterId?[19]
	{
		Characters.CharacterId.lucy,
		Characters.CharacterId.lasVegasCharacter01,
		Characters.CharacterId.tasha,
		Characters.CharacterId.fresh,
		Characters.CharacterId.singaporeCharacter01,
		Characters.CharacterId.pragueCharacter01,
		Characters.CharacterId.zoe,
		Characters.CharacterId.transylvaniaCharacter01,
		Characters.CharacterId.frizzy,
		Characters.CharacterId.spike,
		Characters.CharacterId.havanaCharacter01,
		Characters.CharacterId.tony,
		Characters.CharacterId.kim,
		Characters.CharacterId.tag,
		Characters.CharacterId.sanfranciscoCharacter01,
		Characters.CharacterId.washingtonCharacter01,
		Characters.CharacterId.amsterdamCharacter01,
		Characters.CharacterId.mexicoCharacter01,
		Characters.CharacterId.tricky
	} };

	private Characters.CharacterId?[][] _movieCharacters = new Characters.CharacterId?[1][] { new Characters.CharacterId?[19]
	{
		Characters.CharacterId.ninja,
		Characters.CharacterId.zoe,
		Characters.CharacterId.saopauloCharacter01,
		Characters.CharacterId.seoulCharacter01,
		Characters.CharacterId.losangelesCharacter01,
		Characters.CharacterId.arabiaCharacter01,
		Characters.CharacterId.lasVegasCharacter01,
		Characters.CharacterId.facebookRewardCharacter01,
		Characters.CharacterId.kim,
		Characters.CharacterId.londonCharacter02,
		Characters.CharacterId.mexicoCharacter01,
		Characters.CharacterId.halloweenCharacter01,
		Characters.CharacterId.tag,
		Characters.CharacterId.sanfranciscoCharacter01,
		Characters.CharacterId.peruCharacter01,
		Characters.CharacterId.amsterdamCharacter01,
		Characters.CharacterId.frank,
		Characters.CharacterId.bangkokCharacter01,
		Characters.CharacterId.transylvaniaCharacter01
	} };

	private Characters.CharacterId?[][] _danceCharacters = new Characters.CharacterId?[1][] { new Characters.CharacterId?[19]
	{
		Characters.CharacterId.lasVegasCharacter01,
		Characters.CharacterId.mexicoCharacter01,
		Characters.CharacterId.hawaiiCharacter01,
		Characters.CharacterId.arabiaCharacter01,
		Characters.CharacterId.halloweenCharacter01,
		Characters.CharacterId.lucy,
		Characters.CharacterId.transylvaniaCharacter01,
		Characters.CharacterId.coco,
		Characters.CharacterId.losangelesCharacter01,
		Characters.CharacterId.havanaCharacter01,
		Characters.CharacterId.frank,
		Characters.CharacterId.tricky,
		Characters.CharacterId.tag,
		Characters.CharacterId.seoulCharacter01,
		Characters.CharacterId.tony,
		Characters.CharacterId.amsterdamCharacter01,
		Characters.CharacterId.frizzy,
		Characters.CharacterId.fresh,
		Characters.CharacterId.carmen
	} };

	private Dictionary<Characters.CharacterId, string> _musicWordCollection = new Dictionary<Characters.CharacterId, string>
	{
		{
			Characters.CharacterId.lucy,
			"WEEKEND_EVENT_WORD_MUSIC_PUNK"
		},
		{
			Characters.CharacterId.lasVegasCharacter01,
			"WEEKEND_EVENT_WORD_MUSIC_ROCK"
		},
		{
			Characters.CharacterId.tasha,
			"WEEKEND_EVENT_WORD_MUSIC_POP"
		},
		{
			Characters.CharacterId.fresh,
			"WEEKEND_EVENT_WORD_MUSIC_HIPHOP"
		},
		{
			Characters.CharacterId.singaporeCharacter01,
			"WEEKEND_EVENT_WORD_MUSIC_INDIE"
		},
		{
			Characters.CharacterId.pragueCharacter01,
			"WEEKEND_EVENT_WORD_MUSIC_CLASSIC"
		},
		{
			Characters.CharacterId.zoe,
			"WEEKEND_EVENT_WORD_MUSIC_COUNTRY"
		},
		{
			Characters.CharacterId.transylvaniaCharacter01,
			"WEEKEND_EVENT_WORD_MUSIC_POP"
		},
		{
			Characters.CharacterId.frizzy,
			"WEEKEND_EVENT_WORD_MUSIC_REGGAE"
		},
		{
			Characters.CharacterId.spike,
			"WEEKEND_EVENT_WORD_MUSIC_PUNK"
		},
		{
			Characters.CharacterId.havanaCharacter01,
			"WEEKEND_EVENT_WORD_MUSIC_LATIN"
		},
		{
			Characters.CharacterId.tony,
			"WEEKEND_EVENT_WORD_MUSIC_RAP"
		},
		{
			Characters.CharacterId.kim,
			"WEEKEND_EVENT_WORD_MUSIC_INDIE"
		},
		{
			Characters.CharacterId.tag,
			"WEEKEND_EVENT_WORD_MUSIC_DANCE"
		},
		{
			Characters.CharacterId.sanfranciscoCharacter01,
			"WEEKEND_EVENT_WORD_MUSIC_ROCK"
		},
		{
			Characters.CharacterId.washingtonCharacter01,
			"WEEKEND_EVENT_WORD_MUSIC_RAP"
		},
		{
			Characters.CharacterId.amsterdamCharacter01,
			"WEEKEND_EVENT_WORD_MUSIC_DANCE"
		},
		{
			Characters.CharacterId.mexicoCharacter01,
			"WEEKEND_EVENT_WORD_MUSIC_LATIN"
		},
		{
			Characters.CharacterId.tricky,
			"WEEKEND_EVENT_WORD_MUSIC_HIPHOP"
		}
	};

	private Dictionary<Characters.CharacterId, string> _movieWordCollection = new Dictionary<Characters.CharacterId, string>
	{
		{
			Characters.CharacterId.ninja,
			"WEEKEND_EVENT_WORD_MOVIE_ACTION"
		},
		{
			Characters.CharacterId.zoe,
			"WEEKEND_EVENT_WORD_MOVIE_HORROR"
		},
		{
			Characters.CharacterId.saopauloCharacter01,
			"WEEKEND_EVENT_WORD_MOVIE_SPORTS"
		},
		{
			Characters.CharacterId.seoulCharacter01,
			"WEEKEND_EVENT_WORD_MOVIE_SCIFI"
		},
		{
			Characters.CharacterId.losangelesCharacter01,
			"WEEKEND_EVENT_WORD_MOVIE_ACTION"
		},
		{
			Characters.CharacterId.arabiaCharacter01,
			"WEEKEND_EVENT_WORD_MOVIE_HISTORY"
		},
		{
			Characters.CharacterId.lasVegasCharacter01,
			"WEEKEND_EVENT_WORD_MOVIE_ROMANCE"
		},
		{
			Characters.CharacterId.facebookRewardCharacter01,
			"WEEKEND_EVENT_WORD_MOVIE_MONSTER"
		},
		{
			Characters.CharacterId.kim,
			"WEEKEND_EVENT_WORD_MOVIE_SPORTS"
		},
		{
			Characters.CharacterId.londonCharacter02,
			"WEEKEND_EVENT_WORD_MOVIE_FANTASY"
		},
		{
			Characters.CharacterId.mexicoCharacter01,
			"WEEKEND_EVENT_WORD_MOVIE_ACTION"
		},
		{
			Characters.CharacterId.halloweenCharacter01,
			"WEEKEND_EVENT_WORD_MOVIE_HORROR"
		},
		{
			Characters.CharacterId.tag,
			"WEEKEND_EVENT_WORD_MOVIE_SCIFI"
		},
		{
			Characters.CharacterId.sanfranciscoCharacter01,
			"WEEKEND_EVENT_WORD_MOVIE_HISTORY"
		},
		{
			Characters.CharacterId.peruCharacter01,
			"WEEKEND_EVENT_WORD_MOVIE_SPORTS"
		},
		{
			Characters.CharacterId.amsterdamCharacter01,
			"WEEKEND_EVENT_WORD_MOVIE_ROMANCE"
		},
		{
			Characters.CharacterId.frank,
			"WEEKEND_EVENT_WORD_MOVIE_ACTION"
		},
		{
			Characters.CharacterId.bangkokCharacter01,
			"WEEKEND_EVENT_WORD_MOVIE_SPORTS"
		},
		{
			Characters.CharacterId.transylvaniaCharacter01,
			"WEEKEND_EVENT_WORD_MOVIE_HORROR"
		}
	};

	private Dictionary<Characters.CharacterId, string> _danceWordCollection = new Dictionary<Characters.CharacterId, string>
	{
		{
			Characters.CharacterId.lasVegasCharacter01,
			"WEEKEND_EVENT_WORD_DANCE_SWING"
		},
		{
			Characters.CharacterId.mexicoCharacter01,
			"WEEKEND_EVENT_WORD_DANCE_TANGO"
		},
		{
			Characters.CharacterId.hawaiiCharacter01,
			"WEEKEND_EVENT_WORD_DANCE_HULA"
		},
		{
			Characters.CharacterId.arabiaCharacter01,
			"WEEKEND_EVENT_WORD_DANCE_BELLY"
		},
		{
			Characters.CharacterId.halloweenCharacter01,
			"WEEKEND_EVENT_WORD_DANCE_WALTZ"
		},
		{
			Characters.CharacterId.lucy,
			"WEEKEND_EVENT_WORD_DANCE_MOSH"
		},
		{
			Characters.CharacterId.transylvaniaCharacter01,
			"WEEKEND_EVENT_WORD_DANCE_DISCO"
		},
		{
			Characters.CharacterId.coco,
			"WEEKEND_EVENT_WORD_DANCE_BALLET"
		},
		{
			Characters.CharacterId.losangelesCharacter01,
			"WEEKEND_EVENT_WORD_DANCE_LINE"
		},
		{
			Characters.CharacterId.havanaCharacter01,
			"WEEKEND_EVENT_WORD_DANCE_SALSA"
		},
		{
			Characters.CharacterId.frank,
			"WEEKEND_EVENT_WORD_DANCE_TAP"
		},
		{
			Characters.CharacterId.tricky,
			"WEEKEND_EVENT_WORD_DANCE_BREAK"
		},
		{
			Characters.CharacterId.tag,
			"WEEKEND_EVENT_WORD_DANCE_LOCK"
		},
		{
			Characters.CharacterId.seoulCharacter01,
			"WEEKEND_EVENT_WORD_DANCE_ZUMBA"
		},
		{
			Characters.CharacterId.tony,
			"WEEKEND_EVENT_WORD_DANCE_BREAK"
		},
		{
			Characters.CharacterId.amsterdamCharacter01,
			"WEEKEND_EVENT_WORD_DANCE_WALTZ"
		},
		{
			Characters.CharacterId.frizzy,
			"WEEKEND_EVENT_WORD_DANCE_RAGGAE"
		},
		{
			Characters.CharacterId.fresh,
			"WEEKEND_EVENT_WORD_DANCE_BREAK"
		},
		{
			Characters.CharacterId.carmen,
			"WEEKEND_EVENT_WORD_DANCE_SAMBA"
		}
	};

	private List<SSEventCategory> _supportedCategories = new List<SSEventCategory>
	{
		SSEventCategory.None,
		SSEventCategory.Cat1,
		SSEventCategory.Cat2,
		SSEventCategory.Cat3
	};

	public static event EventHandler<LetterCollectedEventArgs> OnLetterCollected
	{
		add
		{
			EventHandler<LetterCollectedEventArgs> eventHandler = OnLetterCollected__BackingField;
			EventHandler<LetterCollectedEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				eventHandler = Interlocked.CompareExchange(ref OnLetterCollected__BackingField, (EventHandler<LetterCollectedEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
			}
			while ((object)eventHandler != eventHandler2);
		}
		remove
		{
			EventHandler<LetterCollectedEventArgs> eventHandler = OnLetterCollected__BackingField;
			EventHandler<LetterCollectedEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				eventHandler = Interlocked.CompareExchange(ref OnLetterCollected__BackingField, (EventHandler<LetterCollectedEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

	public static event Action<Characters.CharacterId, int> OnCharacterSkippedWithKeys
	{
		add
		{
			Action<Characters.CharacterId, int> action = OnCharacterSkippedWithKeys__BackingField;
			Action<Characters.CharacterId, int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnCharacterSkippedWithKeys__BackingField, (Action<Characters.CharacterId, int>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<Characters.CharacterId, int> action = OnCharacterSkippedWithKeys__BackingField;
			Action<Characters.CharacterId, int> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnCharacterSkippedWithKeys__BackingField, (Action<Characters.CharacterId, int>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<Characters.CharacterId> OnCharacterSkippedWithVideo
	{
		add
		{
			Action<Characters.CharacterId> action = OnCharacterSkippedWithVideo__BackingField;
			Action<Characters.CharacterId> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnCharacterSkippedWithVideo__BackingField, (Action<Characters.CharacterId>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<Characters.CharacterId> action = OnCharacterSkippedWithVideo__BackingField;
			Action<Characters.CharacterId> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnCharacterSkippedWithVideo__BackingField, (Action<Characters.CharacterId>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static CharacterNameEventManager Create(SSEventType eventType, long eventID)
	{
		CharacterNameEventManager characterNameEventManager = new CharacterNameEventManager();
		characterNameEventManager.Initialize(eventType, eventID);
		return characterNameEventManager;
	}

	private void Initialize(SSEventType eventType, long eventID)
	{
		_eventType = eventType;
		_eventID = eventID;
		if (eventID > 0)
		{
			_eventProgressData = EventsSave.characterNameEventData;
			CheckIfDataIsValid();
			if (_eventProgressData.eventID != eventID)
			{
				_eventProgressData = InitCharacterNameEventData();
				AwardsData.ResetWordyWeekendChallengeOneWeekend();
				EventsSave.characterNameEventData = _eventProgressData;
			}
			if (HasCompletedCollectionForSelectedCharacter())
			{
				SkipCurrentCharacter();
			}
		}
	}

	private void CheckIfDataIsValid()
	{
		List<string> categoryWordsList = _eventProgressData.categoryWordsList;
		if (categoryWordsList == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < categoryWordsList.Count; i++)
		{
			string keyString = categoryWordsList[i];
			if (!Strings.IsMapped(keyString))
			{
				categoryWordsList[i] = string.Empty;
				int num = _eventProgressData.charactersList.IndexOf(_eventProgressData.currentCharacter);
				if (i == num)
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			_eventProgressData.collectedLetterIndex = -1;
		}
	}

	private CharacterNameEventData InitCharacterNameEventData()
	{
		List<Characters.CharacterId> list = CreateNewCharacterList();
		List<string> categoryWordsList = CreateNewCategoryWordsDict(list);
		SSEventCategory eventCategoryFromOnlineSetting = GetEventCategoryFromOnlineSetting();
		int index = 0;
		bool wasLastCharacterCompleted = false;
		int completedCharacters = 0;
		int skippedCharacters = 0;
		int collectedLetterIndex = -1;
		return new CharacterNameEventData(list, categoryWordsList, eventCategoryFromOnlineSetting, list[index], collectedLetterIndex, completedCharacters, skippedCharacters, wasLastCharacterCompleted, _eventID);
	}

	public List<Characters.CharacterId> GetCharacterList()
	{
		if (_eventProgressData != null && _eventProgressData.charactersList != null)
		{
			return _eventProgressData.charactersList;
		}
		return null;
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
		return _eventID > 0 && _eventProgressData != null && _eventProgressData.charactersList.Count > 0 && !HasLastCharacterBeenCompleted();
	}

	public T GetEventValue<T>(out bool found)
	{
		T result = default(T);
		found = false;
		string text = ((!Globals.DEBUG_USE_EVENT_TESTER) ? OnlineSettings.instance.GetValue("event_value", string.Empty) : EventTesterManager.Instance.GetEventValue());
		if (!string.IsNullOrEmpty(text) && typeof(T).IsArray)
		{
			try
			{
				string[] array = text.Split(';');
				Type elementType = typeof(T).GetElementType();
				Array array2 = Array.CreateInstance(elementType, array.Length);
				object[] array3 = Utils.ConvertAll(array, (string item) => Convert.ChangeType(item, elementType));
				array3.CopyTo(array2, 0);
				result = (T)(object)array2;
				found = true;
			}
			catch (Exception ex)
			{
				found = false;
				SDebug.LogError(string.Concat("The current event does not contain a value of the specified type: ", typeof(T), ", stack trace: ", ex.StackTrace));
			}
		}
		return result;
	}

	public void GetPopupTitle(out string firstLine, out string secondLine)
	{
		string key;
		switch (GetEventCategory())
		{
		case SSEventCategory.Cat1:
			key = "WEEKEND_EVENT_CATEGORY_MUSIC";
			break;
		case SSEventCategory.Cat2:
			key = "WEEKEND_EVENT_CATEGORY_MOVIE";
			break;
		case SSEventCategory.Cat3:
			key = "WEEKEND_EVENT_CATEGORY_DANCE";
			break;
		default:
			key = "EVENTS_POPUP_WEEKEND_EVENT_TITLE_LINE_1";
			break;
		}
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			firstLine = Strings.Get(key);
			secondLine = Strings.Get("EVENTS_POPUP_WEEKEND_EVENT_TITLE_LINE_2");
		}
		else
		{
			firstLine = Strings.GetLocalizedOnlineSettings("event_popup_title_first", Strings.Get(key));
			secondLine = Strings.GetLocalizedOnlineSettings("event_popup_title_second", Strings.Get("EVENTS_POPUP_WEEKEND_EVENT_TITLE_LINE_2"));
		}
	}

	public void GetPopupDescription(out string firstTitle, out string secondTitle)
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (SelectedCharacterMatchesFocusedCharacter())
		{
			empty = Strings.Get("EVENTS_POPUP_DESCRIPTION_TITLE_SELECT_CHARACTER_NAME");
			string text = string.Empty;
			if (_eventProgressData != null)
			{
				text = GetCharacterDisplayName(_eventProgressData.currentCharacter);
			}
			empty2 = text;
			if (Globals.DEBUG_USE_EVENT_TESTER)
			{
				firstTitle = empty;
				secondTitle = empty2;
			}
			else
			{
				firstTitle = Strings.GetLocalizedOnlineSettings("event_popup_description_first_select_character_name", empty);
				secondTitle = Strings.GetLocalizedOnlineSettings("event_popup_description_second_select_character_name", empty2);
			}
		}
		else
		{
			empty = GetDescriptionTitleForCharacterNotOwned();
			string arg = string.Empty;
			if (_eventProgressData != null)
			{
				arg = Strings.Get(Characters.characterData[_eventProgressData.currentCharacter].name);
			}
			empty2 = string.Format(Strings.Get("EVENTS_POPUP_DESCRIPTION_SECOND_TITLE_UNLOCK_CHARACTER_NAME"), arg);
			if (Globals.DEBUG_USE_EVENT_TESTER)
			{
				firstTitle = empty;
				secondTitle = empty2;
			}
			else
			{
				firstTitle = Strings.GetLocalizedOnlineSettings("event_popup_description_first_unlock_character_name", empty);
				secondTitle = Strings.GetLocalizedOnlineSettings("event_popup_description_second_unlock_character_name", empty2);
			}
		}
	}

	private string GetDescriptionTitleForCharacterNotOwned()
	{
		CharacterNameEventManager characterNameEventManager = SSEventsManagerController.instance.GetActiveEventManager() as CharacterNameEventManager;
		if (characterNameEventManager != null)
		{
			switch (characterNameEventManager.GetEventCategory())
			{
			case SSEventCategory.Cat1:
				return Strings.Get("EVENTS_POPUP_DESCRIPTION_TITLE_UNLOCK_CHARACTER_NAME_CATEGORY1");
			case SSEventCategory.Cat2:
				return Strings.Get("EVENTS_POPUP_DESCRIPTION_TITLE_UNLOCK_CHARACTER_NAME_CATEGORY2");
			case SSEventCategory.Cat3:
				return Strings.Get("EVENTS_POPUP_DESCRIPTION_TITLE_UNLOCK_CHARACTER_NAME_CATEGORY3");
			default:
				return Strings.Get("EVENTS_POPUP_DESCRIPTION_TITLE_UNLOCK_CHARACTER_NAME");
			}
		}
		return Strings.Get("EVENTS_POPUP_DESCRIPTION_TITLE_UNLOCK_CHARACTER_NAME");
	}

	public void GetRibbonDescription(out string firstLine, out string secondLine, out string thirdLine)
	{
		firstLine = string.Empty;
		secondLine = string.Empty;
		thirdLine = string.Empty;
		SDebug.LogError("The Character Name Event does not contain information for a ribbon description");
	}

	public void SaveProgress()
	{
		if (_eventProgressData != null)
		{
			EventsSave.characterNameEventData = _eventProgressData;
		}
	}

	private List<string> CreateNewCategoryWordsDict(List<Characters.CharacterId> charactersList)
	{
		List<string> list = new List<string>();
		Dictionary<Characters.CharacterId, string> dictionary = null;
		switch (GetEventCategoryFromOnlineSetting())
		{
		case SSEventCategory.Cat1:
			dictionary = _musicWordCollection;
			break;
		case SSEventCategory.Cat2:
			dictionary = _movieWordCollection;
			break;
		case SSEventCategory.Cat3:
			dictionary = _danceWordCollection;
			break;
		default:
			return null;
		}
		if (dictionary != null)
		{
			foreach (Characters.CharacterId characters in charactersList)
			{
				string value;
				if (dictionary.TryGetValue(characters, out value))
				{
					list.Add(value);
					continue;
				}
				Characters.Model model = Characters.characterData[characters];
				list.Add(model.name);
			}
		}
		return list;
	}

	private SSEventCategory GetEventCategoryFromOnlineSetting()
	{
		string valueString = null;
		SSEventCategory sSEventCategory = SSEventCategory.None;
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			sSEventCategory = EventTesterManager.Instance.GetEventCategory();
		}
		else if (OnlineSettings.instance.TryGetValue("event_category", out valueString))
		{
			try
			{
				sSEventCategory = (SSEventCategory)Enum.Parse(typeof(SSEventCategory), valueString);
			}
			catch (Exception)
			{
			}
		}
		if (!_supportedCategories.Contains(sSEventCategory))
		{
			sSEventCategory = SSEventCategory.None;
		}
		return sSEventCategory;
	}

	public SSEventCategory GetEventCategory()
	{
		SSEventCategory sSEventCategory = SSEventCategory.None;
		if (Globals.DEBUG_USE_EVENT_TESTER)
		{
			sSEventCategory = EventTesterManager.Instance.GetEventCategory();
		}
		else if (_eventProgressData != null)
		{
			sSEventCategory = _eventProgressData.eventCategory;
		}
		if (!_supportedCategories.Contains(sSEventCategory))
		{
			sSEventCategory = SSEventCategory.None;
		}
		return sSEventCategory;
	}

	private bool TryGetListNumberAndIndexInExtrasList(out int listNumber, out int indexInExtrasList)
	{
		listNumber = 0;
		indexInExtrasList = 0;
		bool found = false;
		int[] eventValue = GetEventValue<int[]>(out found);
		if (found && eventValue.Length >= 1)
		{
			listNumber = eventValue[0];
			Characters.CharacterId?[][] currentCharacterCollection = GetCurrentCharacterCollection();
			if (listNumber >= currentCharacterCollection.Length || listNumber < 0)
			{
				listNumber = 0;
				SDebug.LogError("CharacterEventManager - Specified list was negative or does not exist! Reverting to default.");
			}
			if (eventValue.Length >= 2)
			{
				indexInExtrasList = eventValue[1];
				if (indexInExtrasList >= _limitedCharacters.Length || indexInExtrasList < 0)
				{
					indexInExtrasList = 0;
					SDebug.LogError("CharacterEventManager - LimitedCharacters list index number was negative or does not exist! Reverting to start.");
				}
			}
		}
		if (!found && GetEventCategoryFromOnlineSetting() != SSEventCategory.None)
		{
			found = true;
			listNumber = 0;
			indexInExtrasList = 0;
		}
		return found;
	}

	private Characters.CharacterId?[][] GetCurrentCharacterCollection()
	{
		switch (GetEventCategoryFromOnlineSetting())
		{
		case SSEventCategory.Cat1:
			return _musicCharacters;
		case SSEventCategory.Cat2:
			return _movieCharacters;
		case SSEventCategory.Cat3:
			return _danceCharacters;
		default:
			return normalCharacters;
		}
	}

	private List<Characters.CharacterId> CreateNewCharacterList()
	{
		List<Characters.CharacterId> list = new List<Characters.CharacterId>();
		int listNumber;
		int indexInExtrasList;
		if (TryGetListNumberAndIndexInExtrasList(out listNumber, out indexInExtrasList))
		{
			Characters.CharacterId?[][] currentCharacterCollection = GetCurrentCharacterCollection();
			Characters.CharacterId?[] array = currentCharacterCollection[listNumber];
			for (int i = 0; i < array.Length; i++)
			{
				Characters.CharacterId? characterId = array[i];
				if (characterId.HasValue)
				{
					if (!AgeRestriction.isRestricted || characterId.Value != Characters.CharacterId.facebookRewardCharacter01 || CharacterManager.IsCharacterOwned(characterId.Value))
					{
						list.Add(characterId.Value);
					}
				}
				else if (GetEventCategoryFromOnlineSetting() == SSEventCategory.None)
				{
					if (indexInExtrasList < _limitedCharacters.Length)
					{
						list.Add(_limitedCharacters[indexInExtrasList]);
					}
					else
					{
						indexInExtrasList = 0;
						list.Add(_limitedCharacters[indexInExtrasList]);
					}
					indexInExtrasList++;
				}
			}
		}
		return list;
	}

	public List<Characters.CharacterId> GetCharactersToDisplay()
	{
		List<Characters.CharacterId> result = new List<Characters.CharacterId>();
		if (_eventProgressData != null)
		{
			result = TrimEventCharactersList(_eventProgressData.charactersList);
		}
		return result;
	}

	private List<Characters.CharacterId> TrimEventCharactersList(List<Characters.CharacterId> allEventCharacters)
	{
		List<Characters.CharacterId> list = new List<Characters.CharacterId>();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (_eventProgressData != null)
		{
			num3 = allEventCharacters.IndexOf(_eventProgressData.currentCharacter);
		}
		if (num3 == 0)
		{
			num = num3;
			num2 = num3 + 1;
		}
		else if (num3 == allEventCharacters.Count - 1)
		{
			num = num3 - 1;
			num2 = num3;
		}
		else
		{
			num = num3 - 1;
			num2 = num3 + 1;
		}
		int num4 = 0;
		foreach (Characters.CharacterId allEventCharacter in allEventCharacters)
		{
			if (num <= num4 && num4 <= num2)
			{
				list.Add(allEventCharacter);
			}
			num4++;
		}
		return list;
	}

	public int GetLastSelectedThemeForCharacterType(Characters.CharacterId charId)
	{
		int result = 0;
		if (Outfits.GetOutfitForCharacter(charId, CharacterManager.GetOutfitIndex(charId)) != null)
		{
			result = CharacterManager.GetOutfitIndex(charId);
		}
		return result;
	}

	public bool IsShowingFirstElement()
	{
		return _eventProgressData != null && _eventProgressData.charactersList.IndexOf(_eventProgressData.currentCharacter) == 0;
	}

	public bool IsShowingLastElement()
	{
		return _eventProgressData != null && _eventProgressData.charactersList.IndexOf(_eventProgressData.currentCharacter) == _eventProgressData.charactersList.Count - 1;
	}

	public void SkipCurrentCharacter()
	{
		if (_eventProgressData != null)
		{
			_eventProgressData.wasLastCharacterCompleted = HasCompletedCollectionForSelectedCharacter();
			if (_eventProgressData.wasLastCharacterCompleted)
			{
				_eventProgressData.completedCharacters++;
			}
			else
			{
				_eventProgressData.skippedCharacters++;
			}
			Characters.CharacterId? nextCharacterToDisplay = GetNextCharacterToDisplay();
			if (nextCharacterToDisplay.HasValue)
			{
				_eventProgressData.currentCharacter = nextCharacterToDisplay.Value;
				ResetCollectProgress();
			}
			SaveProgress();
		}
	}

	public Characters.CharacterId? GetNextCharacterToDisplay()
	{
		Characters.CharacterId? result = null;
		if (!IsLastCharacterSelected() && _eventProgressData != null)
		{
			int num = _eventProgressData.charactersList.IndexOf(_eventProgressData.currentCharacter);
			result = _eventProgressData.charactersList[num + 1];
		}
		return result;
	}

	public bool ShouldShowTheCompletedSign()
	{
		return _eventProgressData != null && _eventProgressData.wasLastCharacterCompleted;
	}

	public bool IsAbleToSkip()
	{
		bool flag = false;
		if (_eventProgressData != null)
		{
			Characters.CharacterId currentCharacter = _eventProgressData.currentCharacter;
			flag = ((Characters.characterData[currentCharacter].unlockType != PurchaseData.UnlockType.Facebook) ? CharacterManager.IsCharacterOwned(currentCharacter) : (SocialManager.instance.facebookIsLoggedIn && CharacterManager.IsCharacterOwned(currentCharacter)));
		}
		return (Globals.DEBUG_USE_EVENT_TESTER || !flag) && !IsLastCharacterSelected();
	}

	private bool HasLastCharacterBeenCompleted()
	{
		return IsLastCharacterSelected() && HasCompletedCollectionForSelectedCharacter();
	}

	public bool ShouldSpawnLetters()
	{
		return SelectedCharacterMatchesFocusedCharacter() && !HasCompletedCollectionForSelectedCharacter();
	}

	private void ResetCollectProgress()
	{
		if (_eventProgressData != null)
		{
			_eventProgressData.collectedLetterIndex = -1;
		}
	}

	public char? NextLetterToSpawn()
	{
		char? result = null;
		if (_eventProgressData != null)
		{
			char[] characterNameArray = GetCharacterNameArray(_eventProgressData.currentCharacter);
			if (characterNameArray.Length > 0 && !HasCompletedCollectionForSelectedCharacter())
			{
				result = ((_eventProgressData.collectedLetterIndex >= characterNameArray.Length - 1) ? new char?(characterNameArray[_eventProgressData.collectedLetterIndex]) : new char?(characterNameArray[_eventProgressData.collectedLetterIndex + 1]));
			}
		}
		return result;
	}

	public void NotifyLetterCollected(char collectedLetter)
	{
		if (_eventProgressData != null)
		{
			char[] characterNameArray = GetCharacterNameArray(_eventProgressData.currentCharacter);
			int num = -1;
			if (_eventProgressData.collectedLetterIndex < characterNameArray.Length - 1)
			{
				num = ((_eventProgressData.collectedLetterIndex < 0) ? Array.IndexOf(characterNameArray, collectedLetter) : Array.IndexOf(characterNameArray, collectedLetter, _eventProgressData.collectedLetterIndex + 1));
			}
			int num2 = num - _eventProgressData.collectedLetterIndex;
			if (num > -1 && num2 == 1)
			{
				_eventProgressData.collectedLetterIndex = num;
				UIScreenController.Instance.QueueSlideIn(new SlideIn
				{
					type = SlideInData.SlideInType.CharacterEventWord,
					value = _eventProgressData.collectedLetterIndex,
					stringValue = GetCharacterDisplayName(_eventProgressData.currentCharacter)
				});
				if (HasCompletedCollectionForSelectedCharacter())
				{
					CelebrationReward celebrationReward = MysteryBox.Roll(MysteryBox.Type.Super);
					celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.CharacterEventSuperMysteryBox;
					celebrationReward.characterEventCompleted = _eventProgressData.completedCharacters + _eventProgressData.skippedCharacters + 1;
					celebrationReward.description = new string(characterNameArray);
					celebrationReward.syboAnalyticsSource = ObtainableItemSource.CharacterLetters;
					celebrationReward.syboAnalyticsSourceId = -1L;
					RewardManager.AddRewardToUnlock(celebrationReward);
					UIScreenController.Instance.QueueSlideIn(new SlideIn
					{
						type = SlideInData.SlideInType.CharacterEventWordReward,
						stringValue = GetCharacterDisplayName(_eventProgressData.currentCharacter)
					});
					Stat.WordyWeekendChallengesCompleted.IncrementValue();
					Stat.WordyWeekendChallengesCompletedOneWeekend.IncrementValue();
				}
				SaveProgress();
			}
		}
		if (OnLetterCollected__BackingField != null)
		{
			OnLetterCollected__BackingField(this, new LetterCollectedEventArgs(collectedLetter));
		}
	}

	public int GetAmountOfCollectedLetters()
	{
		int result = 0;
		if (_eventProgressData != null)
		{
			result = _eventProgressData.collectedLetterIndex + 1;
		}
		return result;
	}

	public bool HasCompletedCollectionForSelectedCharacter()
	{
		bool result = false;
		if (_eventProgressData != null)
		{
			char[] characterNameArray = GetCharacterNameArray(_eventProgressData.currentCharacter);
			result = characterNameArray.Length > 0 && _eventProgressData.collectedLetterIndex == characterNameArray.Length - 1;
		}
		return result;
	}

	public bool IsFirstCharacterSelected()
	{
		return _eventProgressData != null && _eventProgressData.charactersList.Contains(_eventProgressData.currentCharacter) && _eventProgressData.charactersList.IndexOf(_eventProgressData.currentCharacter) == 0;
	}

	public bool IsLastCharacterSelected()
	{
		return _eventProgressData != null && _eventProgressData.charactersList.Contains(_eventProgressData.currentCharacter) && _eventProgressData.charactersList.IndexOf(_eventProgressData.currentCharacter) == _eventProgressData.charactersList.Count - 1;
	}

	private string GetWordForCharacter(Characters.CharacterId characterId)
	{
		string key = string.Empty;
		bool flag = false;
		if (_eventProgressData != null && _eventProgressData.categoryWordsList != null)
		{
			int num = _eventProgressData.charactersList.IndexOf(characterId);
			if (num >= 0 && num < _eventProgressData.categoryWordsList.Count)
			{
				string text = _eventProgressData.categoryWordsList[num];
				if (text != string.Empty)
				{
					key = text;
					flag = true;
				}
			}
		}
		if (!flag)
		{
			Characters.Model model = Characters.characterData[characterId];
			key = model.name;
		}
		return Strings.Get(key);
	}

	private char[] GetCharacterNameArray(Characters.CharacterId characterId)
	{
		string wordForCharacter = GetWordForCharacter(characterId);
		string text = FilterCharacterName(wordForCharacter);
		return text.ToUpper().ToCharArray();
	}

	private string GetCharacterDisplayName(Characters.CharacterId characterId)
	{
		string wordForCharacter = GetWordForCharacter(characterId);
		return wordForCharacter.Trim().ToUpper();
	}

	private string FilterCharacterName(string characterName)
	{
		char[] fILTERED_CHARACTERS = FILTERED_CHARACTERS;
		for (int i = 0; i < fILTERED_CHARACTERS.Length; i++)
		{
			char c = fILTERED_CHARACTERS[i];
			characterName = characterName.Replace(c.ToString(), string.Empty);
		}
		return characterName;
	}

	public void UnHookFromGameEvents()
	{
		SaveProgress();
	}

	public int GetNumberOfKeysToSkip()
	{
		int result = 2;
		if (_eventProgressData != null)
		{
			result = _eventProgressData.skippedCharacters + 3;
		}
		return result;
	}

	public bool CanAffordToSkip()
	{
		return InventorySave.keys - GetNumberOfKeysToSkip() >= 0;
	}

	public void UseKeysToSkip()
	{
		int numberOfKeysToSkip = GetNumberOfKeysToSkip();
		if (_eventProgressData != null)
		{
			Flurry.LogEventWithSeveralParameters("Skipped character for Wordy Weekend", "Character;Price;Watched ad", string.Concat(_eventProgressData.currentCharacter, ";", numberOfKeysToSkip, ";no"));
		}
		MissionsManager.Instance.PlayerDidThis(MissionTarget.SpendKeys, numberOfKeysToSkip);
		InventorySave.DecreaseKeys(numberOfKeysToSkip, "WordyWeekendSkip");
		if (_eventProgressData != null)
		{
			OnCharacterSkippedWithKeys__BackingField.SafeInvoke(_eventProgressData.currentCharacter, numberOfKeysToSkip);
		}
	}

	public void LogWatchedAdToSkip()
	{
		int numberOfKeysToSkip = GetNumberOfKeysToSkip();
		if (_eventProgressData != null)
		{
			Flurry.LogEventWithSeveralParameters("Skipped character for Wordy Weekend", "Character;Price;Watched ad", string.Concat(_eventProgressData.currentCharacter, ";", numberOfKeysToSkip, ";yes"));
			OnCharacterSkippedWithVideo__BackingField.SafeInvoke(_eventProgressData.currentCharacter);
		}
	}

	public bool SelectedCharacterMatchesFocusedCharacter()
	{
		return _eventProgressData != null && _eventProgressData.currentCharacter == PlayerSave.currentCharacterId;
	}
}
