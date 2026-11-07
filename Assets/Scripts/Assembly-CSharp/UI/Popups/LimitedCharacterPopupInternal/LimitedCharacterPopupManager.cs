using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Data.Managers;
using Kiloo;
using Kiloo.Common.Utility;
using SYBO.Subway.Info;

namespace UI.Popups.LimitedCharacterPopupInternal
{
	public class LimitedCharacterPopupManager
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action OnLimitedCharacterStartedHandler__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action OnLimitedCharacterExpiredHandler__BackingField;

		private Characters.CharacterId _currentCharacter = Characters.CharacterId.buenosAiresCharacter01;

		private static LimitedCharacterPopupManager _instance;

		private bool _isInitialized;

		private IEnumerator _startTimerCoroutine;

		private IEnumerator _endTimerCoroutine;

		public Characters.CharacterId currentCharacter
		{
			get
			{
				if (Globals.DEBUG_LIMITED_CHARACTER_POPUP)
				{
					return Globals.debugLimitedCharacterPopupId;
				}
				return _currentCharacter;
			}
			set
			{
				_currentCharacter = value;
			}
		}

		public int currentThemeIndex { get; set; }

		public static LimitedCharacterPopupManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new LimitedCharacterPopupManager();
				}
				return _instance;
			}
		}

		public static event Action OnLimitedCharacterStartedHandler
		{
			add
			{
				Action action = OnLimitedCharacterStartedHandler__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnLimitedCharacterStartedHandler__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnLimitedCharacterStartedHandler__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnLimitedCharacterStartedHandler__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action OnLimitedCharacterExpiredHandler
		{
			add
			{
				Action action = OnLimitedCharacterExpiredHandler__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnLimitedCharacterExpiredHandler__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnLimitedCharacterExpiredHandler__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnLimitedCharacterExpiredHandler__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		private LimitedCharacterPopupManager()
		{
			if (!_isInitialized)
			{
				Initialize();
			}
		}

		private void Initialize()
		{
			if (!_isInitialized)
			{
				UIScreenController.OnApplicationResumed += StartTimer;
				StartTimer();
				_isInitialized = true;
			}
		}

		public bool IsCharacterAvailable()
		{
			if (Globals.DEBUG_LIMITED_CHARACTER_POPUP_IGNORE_EXPIRE)
			{
				return true;
			}
			DateTime value = CharacterManager.CharacterStartAvailabilityDate(currentCharacter);
			DateTime value2 = CharacterManager.CharacterExpirationDate(currentCharacter);
			DateTime now = CDateTime.Now;
			return now.CompareTo(value) > 0 && now.CompareTo(value2) < 0;
		}

		public bool ShouldButtonBeVisible()
		{
			return !CharacterManager.IsCharacterOwned(currentCharacter) && IsCharacterAvailable();
		}

		public bool CurrentlyShownCharacterMatchesEventItem()
		{
			bool result = false;
			IEventManager activeEventManager = SSEventsManagerController.instance.GetActiveEventManager();
			if (activeEventManager != null)
			{
				SSEventType eventType = activeEventManager.GetEventType();
				if (eventType == SSEventType.ExpiredLimitedCharacter)
				{
					bool found;
					Characters.CharacterId? characterId = activeEventManager.GetEventValue<Characters.CharacterId>(out found);
					if (found && characterId.GetValueOrDefault() == UIModelController.Instance.currentlyShownModel && characterId.HasValue)
					{
						result = true;
					}
				}
			}
			return result;
		}

		private void StartTimer()
		{
			DateTime dateTime = CharacterManager.CharacterStartAvailabilityDate(currentCharacter);
			DateTime dateTime2 = CharacterManager.CharacterExpirationDate(currentCharacter);
			DateTime now = CDateTime.Now;
			CoroutineUtil.StopCoroutine(_startTimerCoroutine);
			CoroutineUtil.StopCoroutine(_endTimerCoroutine);
			if (now.CompareTo(dateTime) < 0)
			{
				float timeLeft = (float)(dateTime - now).TotalSeconds;
				_startTimerCoroutine = Timer(timeLeft, OnLimitedCharacterStartedHandler__BackingField);
				CoroutineUtil.StartCoroutine(_startTimerCoroutine);
			}
			if (now.CompareTo(dateTime2) < 0)
			{
				float timeLeft2 = (float)(dateTime2 - now).TotalSeconds;
				_endTimerCoroutine = Timer(timeLeft2, OnLimitedCharacterExpiredHandler__BackingField);
				CoroutineUtil.StartCoroutine(_endTimerCoroutine);
			}
		}

		private IEnumerator Timer(float timeLeft, Action handler)
		{
			yield return null;
			yield return null;
			float startPoint = 0f;
			while (startPoint < timeLeft)
			{
				startPoint += RealTimeTracker.deltaTime;
				yield return null;
			}
			if (handler != null)
			{
				handler();
			}
		}
	}
}
