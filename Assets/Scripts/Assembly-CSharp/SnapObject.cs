using UnityEngine;

public class SnapObject : MonoBehaviour
{
	public Vector3 spacing = new Vector3(10f, 10f, 10f);

	public Vector3 offset = Vector3.zero;

	protected void Awake()
	{
	}

	protected virtual void Reset()
	{
		spacing = new Vector3(10f, 10f, 10f);
		offset = Vector3.zero;
	}

	public void Snap()
	{
		Vector3 localPosition = base.transform.localPosition;
		localPosition = new Vector3((float)Mathf.RoundToInt((localPosition.x - offset.x) / spacing.x) * spacing.x + offset.x, (float)Mathf.RoundToInt((localPosition.y - offset.y) / spacing.y) * spacing.y + offset.y, (float)Mathf.RoundToInt((localPosition.z - offset.z) / spacing.z) * spacing.z + offset.z);
		if (base.transform.localPosition != localPosition)
		{
		}
		base.transform.localPosition = localPosition;
	}
}
