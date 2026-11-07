using System;
using SYBO.Components.Base.Interfaces;
using UnityEngine;

namespace SYBO.Components.Utilities.Data
{
	[Serializable]
	public class NumericFloatToggle : NumericToggle, IChangeable
	{
		[SerializeField]
		private float _value;

		[HideInInspector]
		[SerializeField]
		private float _valueOriginal;

		public float Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				OnChanged();
			}
		}

		public NumericFloatToggle()
		{
			Initialize(-1f, false, "(Default)");
		}

		public NumericFloatToggle(float value, bool isEnabled = true, string inspectorMessage = "(Default)")
		{
			Initialize(value, isEnabled, inspectorMessage);
		}

		private void Initialize(float value, bool isEnabled, string message)
		{
			_valueOriginal = (_value = value);
			bool isEnabledOriginal = (base.IsEnabled = isEnabled);
			base.IsEnabledOriginal = isEnabledOriginal;
			string inspectorMessageOriginal = (base.InspectorMessage = message);
			base.InspectorMessageOriginal = inspectorMessageOriginal;
		}

		public void Reset()
		{
			Initialize(_valueOriginal, base.IsEnabledOriginal, base.InspectorMessageOriginal);
		}

		public void SoftReset()
		{
			Reset();
		}

		protected override bool IsOriginalValue()
		{
			if (_value == _valueOriginal)
			{
				return true;
			}
			return false;
		}
	}
}
