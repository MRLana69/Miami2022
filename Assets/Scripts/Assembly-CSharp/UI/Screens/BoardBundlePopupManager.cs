using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Data.Managers;
using InApp;
using InAppPurchase;
using Info;
using Kiloo;
using Kiloo.Common.Backend.PurchaseValidation;
using Kiloo.Common.Extensions;
using Kiloo.Common.Utility;
using UI.Popups.LimitedBoardPopupInternal;
using UnityEngine;

namespace UI.Screens
{
	public class BoardBundlePopupManager
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onBoardBundleStartedHandler__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onBoardBundleExpiredHandler__BackingField;

		public const string BUNDLE_TIER_01 = "com.kiloo.subwayboard.mumbai.bundletier1";

		public const string BUNDLE_TIER_02 = "com.kiloo.subwayboard.mumbai.bundletier2";

		public readonly string BUNDLE_BOARD;

		public readonly HoverboardThemes.ThemeIdentifier firstCustomSet;

		public readonly HoverboardThemes.ThemeIdentifier secondCustomSet;

		private readonly Boards.BoardId BOARD_TYPE;

		private const bool BOARD_BUNDLE_ACTIVE = false;

		private IEnumerator _startTimerCoroutine;

		private IEnumerator _endTimerCoroutine;

		private Action _onSuccess;

		private static BoardBundlePopupManager _instance;

		public static BoardBundlePopupManager instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new BoardBundlePopupManager();
				}
				return _instance;
			}
		}

		public event Action onBoardBundleStartedHandler
		{
			add
			{
				Action action = onBoardBundleStartedHandler__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onBoardBundleStartedHandler__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onBoardBundleStartedHandler__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onBoardBundleStartedHandler__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action onBoardBundleExpiredHandler
		{
			add
			{
				Action action = onBoardBundleExpiredHandler__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onBoardBundleExpiredHandler__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onBoardBundleExpiredHandler__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onBoardBundleExpiredHandler__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		private BoardBundlePopupManager()
		{
			UIScreenController.OnApplicationResumed += StartTimer;
			StartTimer();
			InAppItemModel value;
			if (!InAppData.inAppData.TryGetValue("com.kiloo.subwayboard.mumbai.bundletier2", out value))
			{
				return;
			}
			for (int i = 0; i < value.bundleContents.Count; i++)
			{
				if (value.bundleContents[i].type == InAppItemModel.ItemType.Board)
				{
					BOARD_TYPE = value.bundleContents[i].boardId;
				}
				else if (value.bundleContents[i].type == InAppItemModel.ItemType.BoardAbility)
				{
					if (i == 1)
					{
						firstCustomSet = value.bundleContents[i].themeIdentifier;
					}
					else
					{
						secondCustomSet = value.bundleContents[i].themeIdentifier;
					}
				}
			}
			BUNDLE_BOARD = InAppData.ConvertBoardTypeToDynamicPurchaseId(BOARD_TYPE);
		}

		public Boards.BoardId GetBoardType()
		{
			return BOARD_TYPE;
		}

		public void DebugModeChanged()
		{
			if (Globals.DEBUG_ENABLE_BOARD_BUNDLE)
			{
				onBoardBundleStartedHandler__BackingField.SafeInvoke();
			}
			else
			{
				onBoardBundleExpiredHandler__BackingField.SafeInvoke();
			}
		}

		public bool IsBoardBundleSaleActive()
		{
			if (Globals.DEBUG_ENABLE_BOARD_BUNDLE)
			{
				return true;
			}
			if (BoardManager.IsBoardDisabled(BOARD_TYPE))
			{
				return false;
			}
			long boardBundleStartTimestamp = GetBoardBundleStartTimestamp();
			long boardBundleExpireTimestamp = GetBoardBundleExpireTimestamp();
			long currentTimestamp = GetCurrentTimestamp();
			bool flag = currentTimestamp >= boardBundleStartTimestamp && currentTimestamp < boardBundleExpireTimestamp;
			return false;
		}

		public bool ShouldButtonBeVisible()
		{
			return !BoardManager.IsBoardOwned(GetBoardType()) && IsBoardBundleSaleActive();
		}

		public string GetBundleIconName()
		{
			InAppItemModel value;
			if (InAppData.inAppData.TryGetValue("com.kiloo.subwayboard.mumbai.bundletier1", out value))
			{
				return value.GetIconName();
			}
			return null;
		}

		public long GetBoardBundleStartTimestamp()
		{
			return BoardManager.BoardStartAvailabilityDate(BOARD_TYPE).ToUnixTime();
		}

		public long GetBoardBundleExpireTimestamp()
		{
			return BoardManager.BoardExpirationDate(BOARD_TYPE).ToUnixTime();
		}

		public TimeSpan GetTimeLeft()
		{
			long boardBundleExpireTimestamp = GetBoardBundleExpireTimestamp();
			long currentTimestamp = GetCurrentTimestamp();
			return TimeSpan.FromSeconds(boardBundleExpireTimestamp - currentTimestamp);
		}

		public void PurchaseBoardBundle(string productId, Action onSuccess)
		{
			_onSuccess = onSuccess;
			InAppManager.instance.onPurchaseSuccess += OnPurchaseSuccess;
			InAppManager.instance.onPurchaseCanceled += OnPurchaseFailedOrCancelled;
			InAppManager.instance.onPurchaseFailure += OnPurchaseFailedOrCancelled;
			InAppManager.instance.onPurchaseItemNotAvailable += OnPurchaseFailedOrCancelled;
			InAppManager.instance.StartPurchase(productId);
		}

		private void OnPurchaseFailedOrCancelled(string id, PurchaseResult result)
		{
			InAppManager.instance.onPurchaseSuccess -= OnPurchaseSuccess;
			InAppManager.instance.onPurchaseCanceled -= OnPurchaseFailedOrCancelled;
			InAppManager.instance.onPurchaseFailure -= OnPurchaseFailedOrCancelled;
			InAppManager.instance.onPurchaseItemNotAvailable -= OnPurchaseFailedOrCancelled;
		}

		private void OnPurchaseSuccess(string productid, bool wasrestored, IPurchaseValidationResult validationresult)
		{
			InAppManager.instance.onPurchaseSuccess -= OnPurchaseSuccess;
			InAppManager.instance.onPurchaseCanceled -= OnPurchaseFailedOrCancelled;
			InAppManager.instance.onPurchaseFailure -= OnPurchaseFailedOrCancelled;
			InAppManager.instance.onPurchaseItemNotAvailable -= OnPurchaseFailedOrCancelled;
			_onSuccess.SafeInvoke();
			_onSuccess = null;
			Boards.BoardId boardReturned = Boards.BoardId.normal;
			bool flag = false;
			List<HoverboardThemes.ThemeIdentifier> list = new List<HoverboardThemes.ThemeIdentifier>();
			if (InAppData.GetInAppBoardBundleTiers().Contains(productid))
			{
				InAppItemModel inAppItemModel = InAppData.inAppData[productid];
				foreach (InAppItemModel.BundleItem bundleContent in inAppItemModel.bundleContents)
				{
					if (bundleContent.type == InAppItemModel.ItemType.Board)
					{
						BoardManager.ResetDisplayedCustomSets(bundleContent.boardId);
						UIModelController.Instance.SelectCurrentlyShownBoard(bundleContent.boardId);
						boardReturned = bundleContent.boardId;
						flag = true;
					}
					if (bundleContent.type == InAppItemModel.ItemType.BoardAbility)
					{
						list.Add(bundleContent.themeIdentifier);
						BoardManager.SetThemeActive(bundleContent.boardId, bundleContent.themeIdentifier, true);
					}
				}
				if (flag)
				{
					if (list.Contains(firstCustomSet))
					{
						BoardManager.DisplayCustomSet(0, true);
					}
					if (list.Contains(secondCustomSet))
					{
						BoardManager.DisplayCustomSet(1, true);
					}
					UIScreenController.Instance.ShowUnlockAnimationForHoverboardBundle(boardReturned, list);
				}
			}
			else if (InAppData.TryConvertDynamicBoardIdToBoardType(productid, out boardReturned))
			{
				BoardManager.ResetDisplayedCustomSets(boardReturned);
				UIModelController.Instance.SelectCurrentlyShownBoard(boardReturned);
				UIScreenController.Instance.ShowUnlockAnimationForHoverboard(boardReturned);
				flag = true;
			}
			if (flag)
			{
				Boards.Board board = Boards.boardData[boardReturned];
				Flurry.LogEventWithAParameter("Hoverboard bought", "Id", char.ToUpper(Strings.Get(board.name)[0]) + Strings.Get(board.name).Substring(1));
				LimitedBoardPopupManager.instance.OverrideNextShowingWithBoard(BOARD_TYPE);
				UIScreenController.Instance.QueuePopup("LimitedBoardPopup");
			}
		}

		private void StartTimer()
		{
			if (!BoardManager.IsBoardDisabled(BOARD_TYPE))
			{
				long boardBundleStartTimestamp = GetBoardBundleStartTimestamp();
				long boardBundleExpireTimestamp = GetBoardBundleExpireTimestamp();
				long currentTimestamp = GetCurrentTimestamp();
				CoroutineUtil.StopCoroutine(_startTimerCoroutine);
				CoroutineUtil.StopCoroutine(_endTimerCoroutine);
				if (currentTimestamp < boardBundleStartTimestamp)
				{
					_startTimerCoroutine = StartTimer(boardBundleStartTimestamp);
					CoroutineUtil.StartCoroutine(_startTimerCoroutine);
				}
				if (currentTimestamp < boardBundleExpireTimestamp)
				{
					_endTimerCoroutine = ExpireTimer(boardBundleExpireTimestamp);
					CoroutineUtil.StartCoroutine(_endTimerCoroutine);
				}
			}
		}

		private IEnumerator StartTimer(long endTime)
		{
			long currentTime = GetCurrentTimestamp();
			while (currentTime < endTime)
			{
				currentTime = GetCurrentTimestamp();
				yield return null;
			}
			onBoardBundleStartedHandler__BackingField.SafeInvoke();
		}

		private IEnumerator ExpireTimer(long endTime)
		{
			long currentTime = GetCurrentTimestamp();
			while (currentTime < endTime)
			{
				currentTime = GetCurrentTimestamp();
				yield return null;
			}
			onBoardBundleExpiredHandler__BackingField.SafeInvoke();
		}

		private long GetCurrentTimestamp()
		{
			return CDateTime.Now.ToUnixTime();
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogErr(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError("[BoardBundle] " + msg, context);
		}

		[Conditional("ENABLE_BOARDBUNDLE_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}
	}
}
