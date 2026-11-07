using System;
using UnityEngine;

namespace SYBO.Common.Data
{
	[Serializable]
	public class SmoothDampVector3B
	{
		[SerializeField]
		private Vector3 _target;

		[SerializeField]
		private Vector3 _smoothTime;

		private Vector3 _value;

		private Vector3 _velocity;

		public Vector3 Target
		{
			get
			{
				return _target;
			}
			set
			{
				_target = value;
			}
		}

		public float TargetX
		{
			get
			{
				return _target.x;
			}
			set
			{
				_target = new Vector3(value, _target.y, _target.z);
			}
		}

		public float TargetY
		{
			get
			{
				return _target.y;
			}
			set
			{
				_target = new Vector3(_target.x, value, _target.z);
			}
		}

		public float TargetZ
		{
			get
			{
				return _target.z;
			}
			set
			{
				_target = new Vector3(_target.x, _target.y, value);
			}
		}

		public Vector3 SmoothTime
		{
			get
			{
				return _smoothTime;
			}
			set
			{
				_smoothTime = value;
			}
		}

		public Vector3 Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		public Vector3 Velocity
		{
			get
			{
				return _value;
			}
		}

		public SmoothDampVector3B(Vector3 value, Vector3 smoothTime)
		{
			_smoothTime = smoothTime;
			_value = value;
			_target = value;
			_velocity = Vector3.zero;
		}

		public void Update()
		{
			if (_smoothTime.x != 0f)
			{
				float num = Mathf.SmoothDamp(_value.x, _target.x, ref _velocity.x, _smoothTime.x, float.PositiveInfinity, Time.deltaTime);
				if (!float.IsNaN(num))
				{
					_value.x = num;
				}
			}
			else
			{
				_value.x = _target.x;
			}
			if (_smoothTime.y != 0f)
			{
				float num2 = Mathf.SmoothDamp(_value.y, _target.y, ref _velocity.y, _smoothTime.y, float.PositiveInfinity, Time.deltaTime);
				if (!float.IsNaN(num2))
				{
					_value.y = num2;
				}
			}
			else
			{
				_value.y = _target.y;
			}
			if (_smoothTime.z != 0f)
			{
				float num3 = Mathf.SmoothDamp(_value.z, _target.z, ref _velocity.z, _smoothTime.z, float.PositiveInfinity, Time.deltaTime);
				if (!float.IsNaN(num3))
				{
					_value.z = num3;
				}
			}
			else
			{
				_value.z = _target.z;
			}
		}

		public override string ToString()
		{
			return string.Format("[SmoothDamp: t={0}, v={1}, time={2}, v={3}]", Target, Value, SmoothTime, Velocity);
		}
	}
}
