using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using AppRefresh;
using Data.Managers;
using Data.Save;
using Kiloo.Common.Utility;
using SYBO.Subway.Info;
using UI.SlideIns;

namespace UI.Screens
{
	public static class FirstPurchaseBonusManager
	{
		private static bool _lastIsFirstPurchaseBonusActive;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static FirstPurchaseBonusBecameInactive onFirstPurchaseBonusBecameInactive__BackingField;

		private static IEnumerator _stateCheckCoroutine;

		public static bool running { get; private set; }

		public static bool isFirstPurchaseBonusActive
		{
			get
			{
				return !UserSave.hasPurchasedOrRestored && !CharacterManager.IsCharacterOwned(Characters.CharacterId.firstPurchaseBonusCharacter01);
			}
		}

		public static bool isCollectingFirstPurchaseBonus
		{
			get
			{
				return UserSave.hasPurchasedOrRestored && !CharacterManager.IsCharacterOwned(Characters.CharacterId.firstPurchaseBonusCharacter01);
			}
		}

		public static event FirstPurchaseBonusBecameInactive onFirstPurchaseBonusBecameInactive
		{
			add
			{
				FirstPurchaseBonusBecameInactive firstPurchaseBonusBecameInactive = onFirstPurchaseBonusBecameInactive__BackingField;
				FirstPurchaseBonusBecameInactive firstPurchaseBonusBecameInactive2;
				do
				{
					firstPurchaseBonusBecameInactive2 = firstPurchaseBonusBecameInactive;
					firstPurchaseBonusBecameInactive = Interlocked.CompareExchange(ref onFirstPurchaseBonusBecameInactive__BackingField, (FirstPurchaseBonusBecameInactive)Delegate.Combine(firstPurchaseBonusBecameInactive2, value), firstPurchaseBonusBecameInactive);
				}
				while ((object)firstPurchaseBonusBecameInactive != firstPurchaseBonusBecameInactive2);
			}
			remove
			{
				FirstPurchaseBonusBecameInactive firstPurchaseBonusBecameInactive = onFirstPurchaseBonusBecameInactive__BackingField;
				FirstPurchaseBonusBecameInactive firstPurchaseBonusBecameInactive2;
				do
				{
					firstPurchaseBonusBecameInactive2 = firstPurchaseBonusBecameInactive;
					firstPurchaseBonusBecameInactive = Interlocked.CompareExchange(ref onFirstPurchaseBonusBecameInactive__BackingField, (FirstPurchaseBonusBecameInactive)Delegate.Remove(firstPurchaseBonusBecameInactive2, value), firstPurchaseBonusBecameInactive);
				}
				while ((object)firstPurchaseBonusBecameInactive != firstPurchaseBonusBecameInactive2);
			}
		}

		static FirstPurchaseBonusManager()
		{
			AppRefreshHandler.willRefresh += AppWillRefresh;
			AppRefreshHandler.hasRefreshed += AppHasRefreshed;
		}

		public static void ResetFirstPurchaseBonus()
		{
			CharacterManager.LockCharacter(Characters.CharacterId.firstPurchaseBonusCharacter01);
			UserSave.inAppConsumablePurchaseCount = 0;
			UserSave.inAppNonConsumablePurchaseCount = 0;
			UserSave.inAppRestoredPurchaseCount = 0;
			UserSave.inAppLegacyPurchaseCount = 0;
		}

		public static void ActivateCollectState()
		{
			CharacterManager.LockCharacter(Characters.CharacterId.firstPurchaseBonusCharacter01);
			UserSave.inAppConsumablePurchaseCount = 1;
		}

		public static void StartStateCheckCoroutine()
		{
			if (!running && isFirstPurchaseBonusActive)
			{
				running = true;
				_stateCheckCoroutine = StateCheckCoroutine();
				CoroutineUtil.StartCoroutine(_stateCheckCoroutine);
			}
		}

		private static void AppWillRefresh(RefreshType obj)
		{
			if (obj.HasFlag(RefreshType.Data))
			{
				CoroutineUtil.StopCoroutine(_stateCheckCoroutine);
				_lastIsFirstPurchaseBonusActive = false;
				running = false;
			}
		}

		private static void AppHasRefreshed(RefreshType obj)
		{
			if (obj.HasFlag(RefreshType.Data))
			{
				StartStateCheckCoroutine();
			}
		}

		private static IEnumerator StateCheckCoroutine()
		{
			while (running)
			{
				if (_lastIsFirstPurchaseBonusActive != isFirstPurchaseBonusActive && !isFirstPurchaseBonusActive)
				{
					if (!UIScreenController.IsInstanced)
					{
						yield return null;
						continue;
					}
					UIScreenController.Instance.QueueSlideIn(new SlideIn
					{
						type = SlideInData.SlideInType.FirstPurchaseBonus
					});
					if (onFirstPurchaseBonusBecameInactive__BackingField != null)
					{
						onFirstPurchaseBonusBecameInactive__BackingField();
					}
					if (!Globals.DEBUG_FIRST_PURCHASE_BONUS)
					{
						running = false;
					}
				}
				_lastIsFirstPurchaseBonusActive = isFirstPurchaseBonusActive;
				yield return null;
			}
		}
	}
}
