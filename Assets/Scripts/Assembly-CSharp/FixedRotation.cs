using UnityEngine;

public class FixedRotation : MonoBehaviour
{
	[SerializeField]
	private Vector3 _eulerAngles = Vector3.zero;

	private Transform _transform;

	private void Awake()
	{
		_transform = base.transform;
	}

	private void LateUpdate()
	{
		_transform.eulerAngles = _eulerAngles;
	}
}
