using System;
using System.Diagnostics;
using System.Threading;
using SYBO.Common.Events;
using SYBO.Common.Utilities;
using UnityEngine;

namespace SYBO.Common.Data
{
	public class GatedValue
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<ValueChangedEventArgs> OnValueChanged__BackingField;

		private float _minimumValue;

		private float _maximumValue;

		private float _value;

		private float[] _gateValues;

		public float MinimumValue
		{
			get
			{
				return _minimumValue;
			}
		}

		public float MaximumValue
		{
			get
			{
				return _maximumValue;
			}
		}

		public float Value
		{
			get
			{
				return _value;
			}
			set
			{
				float value2 = _value;
				_value = value;
				_value = Mathf.Min(_value, _maximumValue);
				_value = Mathf.Max(_value, _minimumValue);
				if (value2 != _value)
				{
					float deltaValue = _value - value2;
					RaiseOnValueChanged(new ValueChangedEventArgs(_value, value2, deltaValue));
				}
			}
		}

		public float[] GateValues
		{
			get
			{
				return _gateValues;
			}
			set
			{
				_gateValues = value;
				int num = EnumUtility.ConvertEnumToIntArray<Gate>().Length - 3;
				if (num != _gateValues.Length)
				{
					throw new Exception("GatedValue.GatedValue() gatePercentages.Length of '" + _gateValues.Length + "' must be '" + num + "'");
				}
				Array.Sort(_gateValues);
			}
		}

		public Gate Gate
		{
			get
			{
				if (_value <= _minimumValue)
				{
					return Gate.Empty;
				}
				if (_value >= _maximumValue)
				{
					return Gate.Full;
				}
				for (int i = 0; i < _gateValues.Length; i++)
				{
					if (_value < _gateValues[i])
					{
						return EnumUtility.ConvertIndexToEnumValue<Gate>(i + 1);
					}
				}
				return Gate.High;
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

		public GatedValue(float minimumValue, float maximumValue, float defaultValue, float[] gateValues)
		{
			_minimumValue = minimumValue;
			_maximumValue = maximumValue;
			_value = defaultValue;
			GateValues = gateValues;
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
