using UnityEngine;

public class RotateAroundTransform : MonoBehaviour
{
	[SerializeField]
	private Transform _target;

	[SerializeField]
	private Vector3 _offset = Vector3.zero;

	private Vector3 _eulerAngles = Vector3.zero;

	private float _distance = 1f;

	private Vector3 _lastMousePosition = Vector3.zero;

	private void Update()
	{
		if (!(_target != null))
		{
			return;
		}
		_distance -= Input.mouseScrollDelta.y * Time.deltaTime * 10f;
		if (Input.GetMouseButton(0))
		{
			if (_lastMousePosition == Vector3.zero)
			{
				_lastMousePosition = Input.mousePosition;
			}
			Vector3 vector = Input.mousePosition - _lastMousePosition;
			_eulerAngles.y += vector.x;
			_eulerAngles.x -= vector.y;
			_eulerAngles.x = Mathf.Clamp(_eulerAngles.x, -89f, 89f);
			_lastMousePosition = Input.mousePosition;
		}
		else
		{
			_lastMousePosition = Vector3.zero;
		}
		Vector3 vector2 = _target.position + _offset;
		base.transform.position = vector2 - Quaternion.Euler(_eulerAngles) * Vector3.forward * _distance;
		base.transform.LookAt(_target.position + _offset, Vector3.up);
	}
}
