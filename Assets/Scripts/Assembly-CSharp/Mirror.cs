using UnityEngine;

public class Mirror : MonoBehaviour
{
	private Transform[] children;

	private TrackObject trackObject;

	private void Awake()
	{
		trackObject = GetComponent<TrackObject>() ?? base.gameObject.AddComponent<TrackObject>();
		trackObject.OnActivate += OnActivate;
		children = new Transform[base.transform.childCount];
		for (int i = 0; i < base.transform.childCount; i++)
		{
			children[i] = base.transform.GetChild(i);
		}
	}

	public void OnActivate()
	{
		int num = Random.Range(0, 2) * 2 - 1;
		for (int i = 0; i < children.Length; i++)
		{
			if (children[i] != null)
			{
				Vector3 localPosition = children[i].localPosition;
				localPosition.x *= num;
				children[i].localPosition = localPosition;
			}
		}
	}
}
