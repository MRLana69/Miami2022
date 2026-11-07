using UnityEngine;

[ExecuteInEditMode]
public class SplashScaler : MonoBehaviour
{
	private const float iPhoneXresolution_height = 2436f;

	private const float iPhoneXresolution_width = 1125f;

	private bool passed;

	[SerializeField]
	private Camera cam;

	[SerializeField]
	private bool repositionNow;

	private void Update()
	{
		if (repositionNow)
		{
			passed = false;
			repositionNow = false;
		}
		if (passed)
		{
			return;
		}
		if (cam == null)
		{
			SDebug.LogError("Camera reference was missing on the SplashScaler object.");
			return;
		}
		float num = cam.orthographicSize * 2f * NGUITools.screenSize.x / NGUITools.screenSize.y;
		base.transform.localScale = new Vector3(num, num * 2.1653333f, base.transform.localScale.z);
		if (Application.platform == RuntimePlatform.IPhonePlayer && DeviceInfo.Instance.formFactor == DeviceInfo.FormFactor.iPad)
		{
			base.transform.localPosition = new Vector3(0f, 0.182f, 4f);
		}
		else
		{
			base.transform.localPosition = new Vector3(0f, 0f, 4f);
		}
		passed = true;
	}
}
