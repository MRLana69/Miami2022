using System;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using NaughtyAttributes;
using UnityEngine;

namespace Kiloo.Common.Utility
{
	public class StaticUpdater : MonoBehaviour
	{
		private static StaticUpdater _instance;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<float> onUpdate__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<float> onLateUpdate__BackingField;

		public static StaticUpdater instance
		{
			get
			{
				if (_instance == null)
				{
					GameObject gameObject = new GameObject("StaticUpdater");
					_instance = gameObject.GetOrAdd<StaticUpdater>();
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
				}
				return _instance;
			}
		}

		public event Action<float> onUpdate
		{
			add
			{
				Action<float> action = onUpdate__BackingField;
				Action<float> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onUpdate__BackingField, (Action<float>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<float> action = onUpdate__BackingField;
				Action<float> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onUpdate__BackingField, (Action<float>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public event Action<float> onLateUpdate
		{
			add
			{
				Action<float> action = onLateUpdate__BackingField;
				Action<float> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onLateUpdate__BackingField, (Action<float>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<float> action = onLateUpdate__BackingField;
				Action<float> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onLateUpdate__BackingField, (Action<float>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		[UsedImplicitly]
		private void Update()
		{
			onUpdate__BackingField.SafeInvoke(Time.unscaledDeltaTime);
		}

		private void LateUpdate()
		{
			onLateUpdate__BackingField.SafeInvoke(Time.unscaledDeltaTime);
		}

		[Button(null)]
		private void Debug()
		{
			if (onUpdate__BackingField != null)
			{
				Delegate[] invocationList = onUpdate__BackingField.GetInvocationList();
				foreach (Delegate obj in invocationList)
				{
				}
			}
		}
	}
}
