using SYBO.Subway.Cameras;
using UnityEngine;

public class MoveGameObject : MonoBehaviour
{
	private const float MinCameraSpeedDelta = 0.15f;

	public Vector3 vector = Vector3.zero;

	public float MaxMagnitude = 1f;

	public float Speed = 1f;

	[OptionalField]
	public Transform model;

	private CharacterCamera characterCamera;

	private float _speedMultiplier = 1f;

	private float _lastCameraSpeed;

	private Vector3 _startLocal;

	public float SpeedMultiplier
	{
		get
		{
			return _speedMultiplier;
		}
		set
		{
			_speedMultiplier = value;
		}
	}

	public void UpdatePosition(Vector3 localPos)
	{
		_startLocal = localPos;
		base.transform.localPosition = _startLocal;
	}

	public void Update()
	{
		if (characterCamera == null)
		{
			characterCamera = CharacterCamera.Instance;
		}
		if (Mathf.Abs(_lastCameraSpeed - characterCamera.Speed) > 0.15f)
		{
			_lastCameraSpeed = characterCamera.Speed;
			_startLocal = base.transform.localPosition;
		}
		Speed = characterCamera.Speed;
		if (characterCamera.Speed > 0f && !float.IsInfinity(_lastCameraSpeed))
		{
			base.transform.localPosition += _lastCameraSpeed * SpeedMultiplier * Time.deltaTime * vector;
		}
	}
}
