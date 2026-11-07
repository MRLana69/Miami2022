using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Kiloo;
using Kiloo.Common.Extensions;
using Kiloo.Common.Utility;
using SYBO.Subway.Promotion;
using UnityEngine;

namespace UI.Screens
{
	public class PromotionManager
	{
		private const string PromotionFolder = "Promotions";

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onPromotionStartedHandler__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onPromotionExpiredHandler__BackingField;

		private IEnumerator _startTimerCoroutine;

		private IEnumerator _endTimerCoroutine;

		private bool promotionsLoaded;

		private PromotionDefinition currentPromotion;

		private static PromotionManager instance;

		public static PromotionManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new PromotionManager();
				}
				return instance;
			}
		}

		public bool IsCurrentPromotionActive
		{
			get
			{
				PromotionDefinition promotionDefinition = TryGetCurrentPromotion();
				if (promotionDefinition == null)
				{
					return false;
				}
				return promotionDefinition.IsActive;
			}
		}

		public static event Action onPromotionStartedHandler
		{
			add
			{
				Action action = onPromotionStartedHandler__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onPromotionStartedHandler__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onPromotionStartedHandler__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onPromotionStartedHandler__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action onPromotionExpiredHandler
		{
			add
			{
				Action action = onPromotionExpiredHandler__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onPromotionExpiredHandler__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onPromotionExpiredHandler__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onPromotionExpiredHandler__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		private PromotionManager()
		{
			UIScreenController.OnApplicationResumed += StartTimer;
			UIScreenController.OnPopupClosed += OnPopupClosed;
			StartTimer();
		}

		public PromotionDefinition TryGetCurrentPromotion()
		{
			if (!promotionsLoaded)
			{
				LoadPromotion();
			}
			return currentPromotion;
		}

		public void LoadPromotion(bool forceReload = false)
		{
			if (promotionsLoaded && !forceReload)
			{
				return;
			}
			promotionsLoaded = false;
			currentPromotion = null;
			PromotionDefinition[] array = Resources.LoadAll<PromotionDefinition>("Promotions");
			if (array.Length == 0)
			{
				promotionsLoaded = true;
				return;
			}
			PromotionDefinition[] array2 = array;
			foreach (UnityEngine.Object obj in array2)
			{
				PromotionDefinition promotionDefinition = obj as PromotionDefinition;
				if (promotionDefinition.IsEnabled)
				{
					currentPromotion = promotionDefinition;
					break;
				}
			}
			promotionsLoaded = true;
		}

		public T TryGetCurrentPromotion<T>() where T : PromotionDefinition
		{
			return TryGetCurrentPromotion() as T;
		}

		private void OnPopupClosed(string window)
		{
			PromotionDefinition promotionDefinition = TryGetCurrentPromotion();
			if (!(promotionDefinition == null) && window == promotionDefinition.PopupId)
			{
				if (promotionDefinition.ShouldTrackPopupCloseFlurryEvent())
				{
					Flurry.LogEventWithAParameter("Front Screen Promotion Popup", promotionDefinition.PromotionId, "Closed");
				}
				else
				{
					onPromotionExpiredHandler__BackingField.SafeInvoke();
				}
			}
		}

		public void OnPromotionPurchaseFailed()
		{
			PromotionDefinition promotionDefinition = TryGetCurrentPromotion();
			if (!(promotionDefinition == null))
			{
				string purchaseFailureFlurryEventValue = promotionDefinition.GetPurchaseFailureFlurryEventValue();
				Flurry.LogEventWithAParameter("Front Screen Promotion Popup", promotionDefinition.PromotionId, purchaseFailureFlurryEventValue);
			}
		}

		public void OnPromotionPurchaseSuccess()
		{
			PromotionDefinition promotionDefinition = TryGetCurrentPromotion();
			if (!(promotionDefinition == null))
			{
				string purchaseSuccessFlurryEventValue = promotionDefinition.GetPurchaseSuccessFlurryEventValue();
				Flurry.LogEventWithAParameter("Front Screen Promotion Popup", promotionDefinition.PromotionId, purchaseSuccessFlurryEventValue);
			}
		}

		private void StartTimer()
		{
			PromotionDefinition promotionDefinition = TryGetCurrentPromotion();
			if (!(promotionDefinition == null))
			{
				long startTimeUnix = promotionDefinition.StartTimeUnix;
				long expireTimeUnix = promotionDefinition.ExpireTimeUnix;
				long currentTimestamp = GetCurrentTimestamp();
				CoroutineUtil.StopCoroutine(_startTimerCoroutine);
				CoroutineUtil.StopCoroutine(_endTimerCoroutine);
				if (currentTimestamp < startTimeUnix)
				{
					_startTimerCoroutine = StartTimer(startTimeUnix);
					CoroutineUtil.StartCoroutine(_startTimerCoroutine);
				}
				if (currentTimestamp < expireTimeUnix)
				{
					_endTimerCoroutine = ExpireTimer(expireTimeUnix);
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
			onPromotionStartedHandler__BackingField.SafeInvoke();
		}

		private IEnumerator ExpireTimer(long endTime)
		{
			long currentTime = GetCurrentTimestamp();
			while (currentTime < endTime)
			{
				currentTime = GetCurrentTimestamp();
				yield return null;
			}
			onPromotionExpiredHandler__BackingField.SafeInvoke();
		}

		private long GetCurrentTimestamp()
		{
			return CDateTime.Now.ToUnixTime();
		}

		public void EnableCurrentPromotion(bool enable)
		{
			PromotionDefinition promotionDefinition = TryGetCurrentPromotion();
			if (promotionDefinition != null)
			{
				promotionDefinition.IsEnabled = enable;
			}
		}

		public void DebugModeChanged()
		{
			if (Globals.DEBUG_ENABLE_BOARD_BUNDLE)
			{
				onPromotionStartedHandler__BackingField.SafeInvoke();
			}
			else
			{
				onPromotionExpiredHandler__BackingField.SafeInvoke();
			}
		}
	}
}
