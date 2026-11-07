using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Extensions;
using Kiloo.Common.Utility;
using SYBO.Subway;
using UnityEngine;

namespace UI
{
	public class CountdownPopupManager
	{
		private static CountdownPopupManager _instance;

		private const int DEFAULT_COUNTDOWN = 3;

		private bool _skippedFirstFrame;

		private Coroutine _countdownCoroutine;

		private Action _customEndLogic;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onCountdownEnded__BackingField;

		public static CountdownPopupManager instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CountdownPopupManager();
				}
				return _instance;
			}
		}

		public float countdownInSeconds { get; private set; }

		public bool isCountdownActive { get; private set; }

		public event Action onCountdownEnded
		{
			add
			{
				Action action = onCountdownEnded__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onCountdownEnded__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onCountdownEnded__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onCountdownEnded__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public void StartCountdown(float countdown = 3f, Action customEndLogic = null)
		{
			if (!isCountdownActive)
			{
				_skippedFirstFrame = false;
				_customEndLogic = customEndLogic;
				countdownInSeconds = countdown;
				isCountdownActive = true;
				_countdownCoroutine = CoroutineUtil.StartCoroutine(Update());
				UIScreenController.Instance.QueuePopup("CountdownPopup");
			}
		}

		public void StopCountdown()
		{
			if (isCountdownActive)
			{
				UIScreenController.Instance.ClosePopup();
				isCountdownActive = false;
				if (_countdownCoroutine != null)
				{
					CoroutineUtil.StopCoroutine(_countdownCoroutine);
					_countdownCoroutine = null;
				}
			}
		}

		private void CountdownEnded()
		{
			isCountdownActive = false;
			countdownInSeconds = 0f;
			onCountdownEnded__BackingField.SafeInvoke();
			if (_customEndLogic != null)
			{
				_customEndLogic.SafeInvoke();
				_customEndLogic = null;
			}
			else
			{
				DefaultEndLogic();
			}
		}

		private void DefaultEndLogic()
		{
			if (Game.IsInstantiated && Game.Instance != null)
			{
				Game.Instance.TriggerPause(false);
			}
		}

		private IEnumerator Update()
		{
			while (isCountdownActive)
			{
				if (!UIScreenController.Instance.IsApplicationFocused)
				{
					yield return null;
				}
				float delta = 0f;
				if (!_skippedFirstFrame)
				{
					_skippedFirstFrame = true;
				}
				else
				{
					delta = Time.unscaledDeltaTime;
				}
				delta *= 1.75f;
				countdownInSeconds -= delta;
				if (countdownInSeconds <= 0f)
				{
					CountdownEnded();
					break;
				}
				yield return null;
			}
		}
	}
}
