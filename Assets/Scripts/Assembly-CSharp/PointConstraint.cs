using UnityEngine;

public class PointConstraint : MonoBehaviour
{
	private Transform transformCached;

	public Transform master
	{
		set
		{
			transformCached = value;
		}
	}

	private void LateUpdate()
	{
		if (!(transformCached == null))
		{
			base.transform.localPosition = new Vector3(transformCached.localPosition.x, 0f, transformCached.localPosition.z);
		}
	}
}
