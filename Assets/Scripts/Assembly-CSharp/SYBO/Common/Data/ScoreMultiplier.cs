using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Kiloo.Common.Utility;
using UnityEngine;

namespace SYBO.Common.Data
{
	public class ScoreMultiplier
	{
		public class ValueChangedEventArgs : EventArgs
		{
			private float _newValue;

			private float _oldValue;

			public float NewValue
			{
				get
				{
					return _newValue;
				}
			}

			public float OldValue
			{
				get
				{
					return _oldValue;
				}
			}

			public ValueChangedEventArgs(float newValue, float oldValue)
			{
				_newValue = newValue;
				_oldValue = oldValue;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<ValueChangedEventArgs> OnValueChanged__BackingField;

		private float[] _multiplierArray;

		private int _multiplierArrayIndex;

		private float _coolDownDurationMS;

		private float _minimumPositiveScoreDeltaToIncreaseMultiplier = 1f;

		private int _originalMultiplierArrayIndex;

		private float _scoreDeltaSinceLastIncreaseToMultiplier;

		private IEnumerator _coolDownCoroutine;

		public float[] MultiplierArray
		{
			get
			{
				return _multiplierArray;
			}
		}

		public int MultiplierArrayIndex
		{
			get
			{
				return _multiplierArrayIndex;
			}
			private set
			{
				float value2 = Value;
				_multiplierArrayIndex = Mathf.Clamp(value, 0, _multiplierArray.Length - 1);
				if (!value2.Equals(Value))
				{
					RaiseOnValueChanged(new ValueChangedEventArgs(Value, value2));
				}
			}
		}

		public float CoolDownDurationMS
		{
			get
			{
				return _coolDownDurationMS;
			}
			set
			{
				if (value >= 0f)
				{
					_coolDownDurationMS = value;
				}
			}
		}

		public float Value
		{
			get
			{
				return _multiplierArray[MultiplierArrayIndex];
			}
		}

		public float MinimumPositiveScoreDeltaToIncreaseMultiplier
		{
			get
			{
				return _minimumPositiveScoreDeltaToIncreaseMultiplier;
			}
			set
			{
				if (value >= 0f)
				{
					_minimumPositiveScoreDeltaToIncreaseMultiplier = value;
				}
			}
		}

		public event EventHandler<ValueChangedEventArgs> OnValueChanged
		{
			add
			{
				EventHandler<ValueChangedEventArgs> eventHandler = OnValueChanged__BackingField;
				EventHandler<ValueChangedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnValueChanged__BackingField, (EventHandler<ValueChangedEventArgs>)Delegate.Combine(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
			remove
			{
				EventHandler<ValueChangedEventArgs> eventHandler = OnValueChanged__BackingField;
				EventHandler<ValueChangedEventArgs> eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					eventHandler = Interlocked.CompareExchange(ref OnValueChanged__BackingField, (EventHandler<ValueChangedEventArgs>)Delegate.Remove(eventHandler2, value), eventHandler);
				}
				while ((object)eventHandler != eventHandler2);
			}
		}

		public ScoreMultiplier(int multiplierArrayIndex, float[] multiplierArray, float minimumPositiveScoreDeltaToIncreaseMultiplier = 1f)
		{
			_multiplierArray = multiplierArray;
			_minimumPositiveScoreDeltaToIncreaseMultiplier = minimumPositiveScoreDeltaToIncreaseMultiplier;
			_coolDownDurationMS = 3000f;
			MultiplierArrayIndex = (_originalMultiplierArrayIndex = multiplierArrayIndex);
			_coolDownCoroutine = CoolDownCoroutine();
			Reset();
		}

		public void Reset()
		{
			MultiplierArrayIndex = _originalMultiplierArrayIndex;
			_scoreDeltaSinceLastIncreaseToMultiplier = 0f;
			RestartCoolDownCoroutine();
		}

		public float NotifyScoreWasChanged(Score score, float delta)
		{
			_scoreDeltaSinceLastIncreaseToMultiplier += delta;
			if (_scoreDeltaSinceLastIncreaseToMultiplier > 0f && _scoreDeltaSinceLastIncreaseToMultiplier >= _minimumPositiveScoreDeltaToIncreaseMultiplier)
			{
				_scoreDeltaSinceLastIncreaseToMultiplier = 0f;
				MultiplierArrayIndex++;
				RestartCoolDownCoroutine();
			}
			return Value;
		}

		private void RestartCoolDownCoroutine()
		{
			StopCoolDownCoroutine();
			_coolDownCoroutine = CoolDownCoroutine();
			CoroutineUtil.StartCoroutine(_coolDownCoroutine);
		}

		private void StopCoolDownCoroutine()
		{
			CoroutineUtil.StopCoroutine(_coolDownCoroutine);
		}

		private IEnumerator CoolDownCoroutine()
		{
			while (true)
			{
				yield return new WaitForSeconds(_coolDownDurationMS / 1000f);
				if (_multiplierArray != null)
				{
					MultiplierArrayIndex--;
				}
			}
		}

		private void RaiseOnValueChanged(ValueChangedEventArgs valueChangedEventArgs)
		{
			if (OnValueChanged__BackingField != null)
			{
				OnValueChanged__BackingField(this, valueChangedEventArgs);
			}
		}
	}
}
