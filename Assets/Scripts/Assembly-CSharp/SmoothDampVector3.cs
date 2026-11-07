using System;
using UnityEngine;

[Serializable]
public class SmoothDampVector3
{
	[SerializeField]
	private Vector3 _target;

	[SerializeField]
	private float _smoothTime;

	private Vector3 _value;

	private Vector3 _velocity = Vector3.zero;

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

	public float SmoothTime
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
			return _velocity;
		}
		set
		{
			_value = value;
		}
	}

	public SmoothDampVector3(Vector3 value, float smoothTime)
	{
		_smoothTime = smoothTime;
		_value = value;
		_target = value;
	}

	public void Update()
	{
		float num = Mathf.SmoothDamp(_value.x, _target.x, ref _velocity.x, _smoothTime, float.PositiveInfinity, Time.deltaTime);
		if (!float.IsNaN(num))
		{
			_value.x = num;
		}
		float num2 = Mathf.SmoothDamp(_value.y, _target.y, ref _velocity.y, _smoothTime, float.PositiveInfinity, Time.deltaTime);
		if (!float.IsNaN(num2))
		{
			_value.y = num2;
		}
		float num3 = Mathf.SmoothDamp(_value.z, _target.z, ref _velocity.z, _smoothTime, float.PositiveInfinity, Time.deltaTime);
		if (!float.IsNaN(num3))
		{
			_value.z = num3;
		}
	}

	public override string ToString()
	{
		return string.Format("[SmoothDampVector3: t={0}, v={1}, time={2}, v={3}]", Target, Value, SmoothTime, Velocity);
	}
}
