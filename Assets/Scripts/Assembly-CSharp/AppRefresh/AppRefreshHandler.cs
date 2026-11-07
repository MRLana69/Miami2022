using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;
using Kiloo.Common.Utility;
using UnityEngine;

namespace AppRefresh
{
	public static class AppRefreshHandler
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<RefreshType> willRefresh__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<RefreshType> refresh__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<RefreshType> hasRefreshed__BackingField;

		public static bool refreshInProgress { get; private set; }

		public static bool pendingAppRefresh { get; private set; }

		public static event Action<RefreshType> willRefresh
		{
			add
			{
				Action<RefreshType> action = willRefresh__BackingField;
				Action<RefreshType> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref willRefresh__BackingField, (Action<RefreshType>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<RefreshType> action = willRefresh__BackingField;
				Action<RefreshType> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref willRefresh__BackingField, (Action<RefreshType>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<RefreshType> refresh
		{
			add
			{
				Action<RefreshType> action = refresh__BackingField;
				Action<RefreshType> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref refresh__BackingField, (Action<RefreshType>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<RefreshType> action = refresh__BackingField;
				Action<RefreshType> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref refresh__BackingField, (Action<RefreshType>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<RefreshType> hasRefreshed
		{
			add
			{
				Action<RefreshType> action = hasRefreshed__BackingField;
				Action<RefreshType> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref hasRefreshed__BackingField, (Action<RefreshType>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<RefreshType> action = hasRefreshed__BackingField;
				Action<RefreshType> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref hasRefreshed__BackingField, (Action<RefreshType>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static bool HasFlag(this RefreshType obj, RefreshType flag)
		{
			return (obj & flag) == flag;
		}

		public static Coroutine StartRefresh(RefreshType restartType, Action callback = null)
		{
			return CoroutineUtil.StartCoroutine(PerformRefresh(restartType, callback));
		}

		private static IEnumerator PerformRefresh(RefreshType restartType, Action callback = null)
		{
			refreshInProgress = true;
			pendingAppRefresh = false;
			willRefresh__BackingField.SafeInvoke(restartType);
			yield return null;
			InvokeIfFlagged(refresh__BackingField, restartType, RefreshType.Data);
			InvokeIfFlagged(refresh__BackingField, restartType, RefreshType.UI);
			hasRefreshed__BackingField.SafeInvoke(restartType);
			refreshInProgress = false;
			callback.SafeInvoke();
		}

		private static void InvokeIfFlagged(Action<RefreshType> action, RefreshType requestedType, RefreshType flag)
		{
			if (requestedType.HasFlag(flag))
			{
				action.SafeInvoke(flag);
			}
		}

		public static void QueueAppRefresh()
		{
			pendingAppRefresh = true;
		}

		[Conditional("APP_REFRESH_LOGS")]
		private static void Log(string msg)
		{
		}
	}
}
