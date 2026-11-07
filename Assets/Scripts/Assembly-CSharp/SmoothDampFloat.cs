using System;
using UnityEngine;

[Serializable]
public class SmoothDampFloat
{
	[SerializeField]
	private float _target;

	[SerializeField]
	private float _smoothTime;

	private float _value;

	[HideInInspector]
	public float valueSpeed;

	public float Target
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

	public float Value
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

	public SmoothDampFloat(float value, float smoothTime)
	{
		_smoothTime = smoothTime;
		_value = value;
		_target = value;
	}

	public void Update()
	{
		float num = Mathf.SmoothDamp(_value, _target, ref valueSpeed, _smoothTime);
		if (!float.IsNaN(num))
		{
			_value = num;
		}
	}

	public override string ToString()
	{
		return string.Format("[SmoothDampFloat: t={0}, v={1}, time={2}]", Target, Value, SmoothTime);
	}
}
