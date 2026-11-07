using System;
using System.Diagnostics;
using System.Threading;

namespace SYBO.Common.Data
{
	public class Score
	{
		public class ValueChangedEventArgs : EventArgs
		{
			private float _newValue;

			private float _oldValue;

			private float _deltaValue;

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

			public float DeltaValue
			{
				get
				{
					return _deltaValue;
				}
			}

			public ValueChangedEventArgs(float newValue, float oldValue, float deltaValue)
			{
				_newValue = newValue;
				_oldValue = oldValue;
				_deltaValue = deltaValue;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<ValueChangedEventArgs> OnValueChanged__BackingField;

		private float _value;

		private float _originalValue;

		private ScoreMultiplier _scoreMultiplier;

		public float Value
		{
			get
			{
				return _value;
			}
			private set
			{
				_value = value;
				float deltaValue = _value - value;
				RaiseOnValueChanged(new ValueChangedEventArgs(_value, value, deltaValue));
			}
		}

		public ScoreMultiplier ScoreMultiplier
		{
			get
			{
				return _scoreMultiplier;
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

		public Score(float originalValue)
		{
			_originalValue = originalValue;
			_scoreMultiplier = new ScoreMultiplier(0, new float[1] { 1f });
			Reset();
		}

		public Score(float originalValue, ScoreMultiplier scoreMultiplier)
		{
			_originalValue = originalValue;
			_scoreMultiplier = scoreMultiplier;
			Reset();
		}

		public void Reset()
		{
			_scoreMultiplier.Reset();
			ValueChangeTo(_originalValue);
		}

		public float ValueChangeBy(float baseScoreDelta)
		{
			float value = ScoreMultiplier.Value;
			ScoreMultiplier.NotifyScoreWasChanged(this, baseScoreDelta);
			Value += baseScoreDelta * value;
			return Value;
		}

		private float ValueChangeTo(float newBaseScore)
		{
			float delta = _value - newBaseScore;
			ScoreMultiplier.NotifyScoreWasChanged(this, delta);
			Value = newBaseScore;
			return Value;
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
