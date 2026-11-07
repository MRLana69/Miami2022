using UnityEngine;

public class SyncPositionHelper : MonoBehaviour
{
	public GameObject fromObject;

	public GameObject toObject;

	private Transform fromTransform;

	private Transform toTransform;

	private void Start()
	{
		if (fromObject == null || toObject == null)
		{
			SDebug.LogError("SyncPositionHelper: Both from and to must be set. Disabling self " + base.gameObject.name, this);
			base.enabled = false;
		}
		else
		{
			fromTransform = fromObject.transform;
			toTransform = toObject.transform;
		}
	}

	private void LateUpdate()
	{
		toTransform.position = new Vector3(fromTransform.position.x, toTransform.position.y, toTransform.position.z);
	}
}
