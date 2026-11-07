using UnityEngine;

public class BouncerRotater : MonoBehaviour
{
	[SerializeField]
	private float yRotateSpeed = 0.1f;

	[SerializeField]
	private float yBounceAmp = 0.1f;

	[SerializeField]
	private float yBounceSpeed = 0.1f;

	[SerializeField]
	private float timeOffset;

	private Vector3 initialTransform;

	private void Awake()
	{
		initialTransform = base.transform.localPosition;
	}

	private void Update()
	{
		base.transform.localPosition = initialTransform + Vector3.up * (Mathf.Sin((Time.time + timeOffset) * yBounceSpeed) * yBounceAmp);
		base.transform.Rotate(0f, Time.deltaTime * yRotateSpeed, 0f);
	}
}
