using System;

namespace SYBO.Common.Events
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
}
