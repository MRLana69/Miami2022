using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Data.Managers;
using Info;
using Kiloo;
using Kiloo.Common.Utility;
using UI.Popups.LimitedCharacterPopupInternal;

namespace UI.Popups.LimitedBoardPopupInternal
{
	public class LimitedBoardPopupManager
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action OnLimitedBoardStartedHandler__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action OnLimitedBoardExpiredHandler__BackingField;

		private const Boards.BoardId CURRENT_BOARD_TYPE = Boards.BoardId.mumbaiBoard01;

		private bool _overwrittenBoardForNextShowing;

		private Boards.BoardId _overwrittenBoard = Boards.BoardId.mumbaiBoard01;

		private static LimitedBoardPopupManager _instance;

		private bool _isInitialized;

		private IEnumerator _startTimerCoroutine;

		private IEnumerator _endTimerCoroutine;

		public Boards.BoardId currentBoardId
		{
			get
			{
				if (_overwrittenBoardForNextShowing)
				{
					return _overwrittenBoard;
				}
				if (Globals.DEBUG_LIMITED_BOARD_POPUP)
				{
					return Globals.debugLimitedBoardPopupId;
				}
				return Boards.BoardId.mumbaiBoard01;
			}
		}

		public int currentlyShownThemeIndex { get; set; }

		public static LimitedBoardPopupManager instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new LimitedBoardPopupManager();
				}
				return _instance;
			}
		}

		public static event Action OnLimitedBoardStartedHandler
		{
			add
			{
				Action action = OnLimitedBoardStartedHandler__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnLimitedBoardStartedHandler__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnLimitedBoardStartedHandler__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnLimitedBoardStartedHandler__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action OnLimitedBoardExpiredHandler
		{
			add
			{
				Action action = OnLimitedBoardExpiredHandler__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnLimitedBoardExpiredHandler__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = OnLimitedBoardExpiredHandler__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnLimitedBoardExpiredHandler__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		private LimitedBoardPopupManager()
		{
			if (!_isInitialized)
			{
				Initialize();
			}
		}

		public void OverrideNextShowingWithBoard(Boards.BoardId board)
		{
			_overwrittenBoard = board;
			_overwrittenBoardForNextShowing = true;
		}

		private void Initialize()
		{
			if (!_isInitialized)
			{
				UIScreenController.OnApplicationResumed += StartTimer;
				UIScreenController.OnPopupClosed += OnPopupClosed;
				StartTimer();
				_isInitialized = true;
			}
		}

		private void OnPopupClosed(string s)
		{
			if (s == "LimitedBoardPopup")
			{
				_overwrittenBoard = Boards.BoardId.mumbaiBoard01;
				_overwrittenBoardForNextShowing = false;
			}
		}

		public bool IsBoardAvailable()
		{
			if (Globals.DEBUG_LIMITED_BOARD_POPUP_IGNORE_EXPIRE)
			{
				return true;
			}
			DateTime value = BoardManager.BoardExpirationDate(currentBoardId);
			DateTime value2 = BoardManager.BoardStartAvailabilityDate(currentBoardId);
			DateTime now = CDateTime.Now;
			return now.CompareTo(value2) > 0 && now.CompareTo(value) < 0;
		}

		public bool ShouldButtonBeVisible()
		{
			bool flag = LimitedCharacterPopupManager.Instance.ShouldButtonBeVisible();
			bool flag2 = UIScreenController.Instance.GetCurrentPopupName() == "LimitedCharacterPopup";
			bool flag3 = BoardManager.IsBoardOwned(currentBoardId);
			return !flag && !flag2 && !flag3 && IsBoardAvailable();
		}

		private void StartTimer()
		{
			DateTime dateTime = BoardManager.BoardStartAvailabilityDate(currentBoardId);
			DateTime dateTime2 = BoardManager.BoardExpirationDate(currentBoardId);
			DateTime now = CDateTime.Now;
			CoroutineUtil.StopCoroutine(_startTimerCoroutine);
			CoroutineUtil.StopCoroutine(_endTimerCoroutine);
			if (now.CompareTo(dateTime) < 0)
			{
				float timeLeft = (float)(dateTime - now).TotalSeconds;
				_startTimerCoroutine = Timer(timeLeft, OnLimitedBoardStartedHandler__BackingField);
				CoroutineUtil.StartCoroutine(_startTimerCoroutine);
			}
			if (now.CompareTo(dateTime2) < 0)
			{
				float timeLeft2 = (float)(dateTime2 - now).TotalSeconds;
				_endTimerCoroutine = Timer(timeLeft2, OnLimitedBoardExpiredHandler__BackingField);
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
