using System;
using System.Diagnostics;
using System.Threading;
using Kiloo;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using UI.Generic.Layout;
using UnityEngine;

namespace UI.Generic
{
	public class TimerHelper : MonoBehaviour
	{
		[SerializeField]
		private UISprite _background;

		[SerializeField]
		private UISprite _animatedClock;

		[SerializeField]
		private UISprite _clockFrame;

		[SerializeField]
		private UILabel _timerTime;

		[SerializeField]
		private HorizontalLayout _timerLayout;

		[SerializeField]
		private TimeSpinner _timerSpinner;

		private DateTime _endTime;

		private int _lastTimerLength;

		private Kiloo.Common.Timer.Timer _refreshTimer;

		private bool _timerIsRunning;

		private string _timerText;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action onTimerEnded__BackingField;

		public event Action onTimerEnded
		{
			add
			{
				Action action = onTimerEnded__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onTimerEnded__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onTimerEnded__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onTimerEnded__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public void StartTimer(TimeSpan timeSpan)
		{
			StartTimer(CDateTime.UtcNow + timeSpan);
		}

		public void StartTimer(DateTime endTime)
		{
			_endTime = endTime;
			StartInternalTimer();
			_timerIsRunning = true;
			RefreshTimerLabels(_endTime - CDateTime.UtcNow);
			AlignBackground();
		}

		public void StopTimer()
		{
			if (_timerIsRunning)
			{
				_timerIsRunning = false;
				StopInternalTimer();
			}
			AnimateClock(false);
		}

		public void SetAlignment(HorizontalAlignment alignment)
		{
			UIWidget component = GetComponent<UIWidget>();
			_timerLayout.alignment = alignment;
			UIWidget.Pivot pivot = component.pivot;
			switch (alignment)
			{
			case HorizontalAlignment.Left:
				pivot = UIWidget.Pivot.Left;
				break;
			case HorizontalAlignment.Center:
				pivot = UIWidget.Pivot.Center;
				break;
			case HorizontalAlignment.Right:
				pivot = UIWidget.Pivot.Right;
				break;
			}
			UIWidget component2 = _timerLayout.GetComponent<UIWidget>();
			component.pivot = pivot;
			component2.pivot = pivot;
			AlignBackground();
		}

		public void SetTimerColors(Color contentColor, Color backgroundColor)
		{
			_background.color = backgroundColor;
			_animatedClock.color = contentColor;
			_clockFrame.color = contentColor;
			_timerTime.color = contentColor;
		}

		public void SetTimerText(string text)
		{
			_timerText = text;
			_timerTime.text = _timerText;
			AlignBackground();
		}

		public void HideIcon()
		{
			_clockFrame.gameObject.SetActive(false);
			AlignBackground();
		}

		public void ShowIcon()
		{
			_clockFrame.gameObject.SetActive(true);
			AlignBackground();
		}

		private void StartInternalTimer()
		{
			if (_refreshTimer == null)
			{
				_refreshTimer = new Kiloo.Common.Timer.Timer(1f);
			}
			if (!_refreshTimer.isRunning)
			{
				_refreshTimer.Start();
			}
			_refreshTimer.intervalElapsed += TimerRefreshed;
		}

		private void StopInternalTimer()
		{
			if (_refreshTimer != null)
			{
				_refreshTimer.Stop();
			}
		}

		private void OnEnable()
		{
			if (_timerIsRunning)
			{
				StartInternalTimer();
			}
		}

		private void OnDisable()
		{
			StopInternalTimer();
		}

		private void TimerRefreshed()
		{
			TimeSpan timeSpan = _endTime - CDateTime.UtcNow;
			RefreshTimerLabels(timeSpan);
			if (timeSpan < TimeSpan.Zero)
			{
				_refreshTimer.intervalElapsed -= TimerRefreshed;
				onTimerEnded__BackingField.SafeInvoke();
			}
		}

		private void RefreshTimerLabels(TimeSpan expireTime)
		{
			if (expireTime < TimeSpan.Zero)
			{
				AnimateClock(false);
				_timerTime.text = expireTime.NicifyTimeString();
				_timerLayout.Align(true);
				AnimateClock(false, true);
				return;
			}
			_timerTime.text = expireTime.NicifyTimeString();
			AnimateClock(true);
			if (_timerTime.text.Length != _lastTimerLength)
			{
				AlignBackground();
			}
		}

		private void AlignBackground()
		{
			_timerLayout.Align(true);
			_lastTimerLength = _timerTime.text.Length;
			switch (_timerLayout.alignment)
			{
			case HorizontalAlignment.Left:
				_timerLayout.transform.localPosition = new Vector3(_timerLayout.autosizeWidgetMargin / 2f, 0f, 0f);
				break;
			case HorizontalAlignment.Center:
				_timerLayout.transform.localPosition = Vector3.zero;
				break;
			case HorizontalAlignment.Right:
				_timerLayout.transform.localPosition = new Vector3((0f - _timerLayout.autosizeWidgetMargin) / 2f, 0f, 0f);
				break;
			}
			UIWidget component = GetComponent<UIWidget>();
			if (component != null)
			{
				component.width = _background.GetComponent<UIWidget>().width;
			}
		}

		private void AnimateClock(bool animate, bool timeExpired = false)
		{
			_timerSpinner.Animate(animate);
			if (!animate && timeExpired)
			{
				_animatedClock.fillAmount = 0.66f;
				_animatedClock.invert = false;
			}
		}
	}
}
