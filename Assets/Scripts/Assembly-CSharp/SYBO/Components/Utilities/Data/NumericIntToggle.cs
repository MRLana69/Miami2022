using System;
using SYBO.Components.Base.Interfaces;
using UnityEngine;

namespace SYBO.Components.Utilities.Data
{
	[Serializable]
	public class NumericIntToggle : NumericToggle, IChangeable
	{
		[SerializeField]
		private int _value;

		[HideInInspector]
		[SerializeField]
		private int _valueOriginal;

		public int Value
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

		public NumericIntToggle()
		{
			Initialize(-1, false, "(Default)");
		}

		public NumericIntToggle(int value, bool isEnabled = true, string inspectorMessage = "(Default)")
		{
			Initialize(value, isEnabled, inspectorMessage);
		}

		private void Initialize(int value, bool isEnabled, string message)
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
