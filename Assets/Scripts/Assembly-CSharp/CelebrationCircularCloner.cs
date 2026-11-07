using System;
using UnityEngine;

public class CelebrationCircularCloner : MonoBehaviour
{
	public enum Axis
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	[SerializeField]
	private GameObject _targetObject;

	[SerializeField]
	private int _copyCount = 4;

	[SerializeField]
	private bool _destroyOriginal = true;

	[SerializeField]
	private Axis _axis = Axis.Z;

	[SerializeField]
	private bool _alignToCloner = true;

	private float _radius = 1.45f;

	private void Awake()
	{
		if ((float)Screen.height / (float)Screen.width > 2f)
		{
			_radius = 1.2f;
		}
		if (!(_targetObject != null))
		{
			return;
		}
		Vector3 localPosition = base.transform.localPosition;
		Quaternion rotation = _targetObject.transform.localRotation;
		for (int i = 0; i < _copyCount; i++)
		{
			Vector3 vector = CalcCenterOffset(i);
			if (_alignToCloner)
			{
				rotation = Quaternion.LookRotation(base.transform.up, localPosition - vector);
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(_targetObject, vector, rotation);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localScale = _targetObject.transform.localScale;
		}
		if (_destroyOriginal)
		{
			UnityEngine.Object.Destroy(_targetObject);
			_targetObject = null;
		}
	}

	private Vector3 CalcCenterOffset(int index)
	{
		Vector3 result = Vector3.zero;
		float f = (float)index * (float)Math.PI * 2f / (float)_copyCount;
		switch (_axis)
		{
		case Axis.X:
			result = new Vector3(0f, Mathf.Cos(f) * _radius, Mathf.Sin(f) * _radius);
			break;
		case Axis.Y:
			result = new Vector3(Mathf.Cos(f) * _radius, 0f, Mathf.Sin(f) * _radius);
			break;
		case Axis.Z:
			result = new Vector3(Mathf.Cos(f) * _radius, Mathf.Sin(f) * _radius, 0f);
			break;
		}
		return result;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Vector3 position = base.transform.position;
		for (int i = 0; i < _copyCount; i++)
		{
			Vector3 vector = position + CalcCenterOffset(i);
			if (_alignToCloner)
			{
				Gizmos.DrawRay(vector, base.transform.up * _radius * 0.5f);
			}
			Gizmos.DrawSphere(vector, _radius * 0.05f);
		}
	}
}
