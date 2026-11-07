using System;
using UnityEngine;

namespace SYBO.Subway.Inputs
{
	public class InputEventArgs : EventArgs
	{
		private InputType _inputType;

		private float _timeStamp;

		private KeyCode _keyCode;

		public InputType InputType
		{
			get
			{
				return _inputType;
			}
		}

		public float TimeStamp
		{
			get
			{
				return _timeStamp;
			}
		}

		public KeyCode KeyCode
		{
			get
			{
				return _keyCode;
			}
		}

		public InputEventArgs(InputType inputType)
		{
			_inputType = inputType;
			_keyCode = KeyCode.None;
			_timeStamp = Time.timeSinceLevelLoad;
		}

		public InputEventArgs(InputType inputType, KeyCode keyCode)
		{
			_inputType = inputType;
			_keyCode = keyCode;
			_timeStamp = Time.timeSinceLevelLoad;
		}

		public override string ToString()
		{
			return string.Format("[InputEventArgs: InputType={0}, TimeStamp={1}, KeyCode={2}]", InputType, TimeStamp, KeyCode);
		}
	}
}
