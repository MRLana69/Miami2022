using System.Collections;
using System.Collections.Generic;
using System.IO;
using SYBO.Common.Managers;
using SYBO.Components.Utilities;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TakeScreenshot : MonoBehaviour
{
	public int scale = 2;

	private bool takeScreenshot;

	private float left;

	private float right;

	private float top;

	private float bottom;

	private float w;

	private float h;

	private float timeScaleReset = 1f;

	private bool _takingScreenshot;

	private int screenshotCount;

	private List<Camera> uiCameras = new List<Camera>();

	private bool uiCamerasEnabled = true;

	private void Start()
	{
		Camera[] allCameras = Camera.allCameras;
		foreach (Camera camera in allCameras)
		{
			int num = (1 << LayerMask.NameToLayer("2DGUI")) | (1 << LayerMask.NameToLayer("2DOVERLAY")) | (1 << LayerMask.NameToLayer("2DOVERLAYCLIP"));
			if ((camera.cullingMask & num) != 0)
			{
				uiCameras.Add(camera);
			}
		}
	}

	private void Update()
	{
		if (!Application.isEditor)
		{
			return;
		}
		if (Input.GetKeyDown("f8"))
		{
			uiCamerasEnabled = !uiCamerasEnabled;
			foreach (Camera uiCamera in uiCameras)
			{
				uiCamera.enabled = uiCamerasEnabled;
			}
		}
		if (Input.GetKeyUp(KeyCode.F9))
		{
			screenshotCount++;
		}
		_Shoot();
		if (takeScreenshot)
		{
			screenshotCount++;
			if (!_takingScreenshot)
			{
				StartCoroutine("TakeLargeScreenshot");
			}
			takeScreenshot = false;
		}
	}

	private void LimitsFromCameraProjectionMatrix()
	{
		Matrix4x4 projectionMatrix = GetComponent<Camera>().projectionMatrix;
		w = 2f * GetComponent<Camera>().nearClipPlane / projectionMatrix.m00;
		h = 2f * GetComponent<Camera>().nearClipPlane / projectionMatrix.m11;
	}

	private void _Shoot()
	{
		string filename = "screenshot" + screenshotCount + ".png";
		ScreenCapture.CaptureScreenshot(filename, scale);
	}

	private IEnumerator TakeLargeScreenshot()
	{
		_takingScreenshot = true;
		LimitsFromCameraProjectionMatrix();
		Camera cam = GetComponent<Camera>();
		timeScaleReset = SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale;
		SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale = 0f;
		int x = 0;
		int y = 0;
		int i = 0;
		int n = scale * scale;
		while (i < n)
		{
			left = (float)x * w / (float)scale - w / 2f;
			right = left + w / (float)scale;
			bottom = (float)y * h / (float)scale - h / 2f;
			top = bottom + h / (float)scale;
			Matrix4x4 m = PerspectiveOffCenter(left, right, bottom, top, cam.nearClipPlane, cam.farClipPlane);
			cam.projectionMatrix = m;
			string screenshotFilename;
			do
			{
				screenshotFilename = "screenshot" + screenshotCount + "_" + x + "_" + y + ".png";
			}
			while (File.Exists(screenshotFilename));
			ScreenCapture.CaptureScreenshot(screenshotFilename, 2);
			x++;
			if (x >= scale)
			{
				x = 0;
				y++;
			}
			i++;
			yield return null;
		}
		cam.ResetProjectionMatrix();
		SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale = timeScaleReset;
		_takingScreenshot = false;
		yield return null;
	}

	private Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
	{
		float value = 2f * near / (right - left);
		float value2 = 2f * near / (top - bottom);
		float value3 = (right + left) / (right - left);
		float value4 = (top + bottom) / (top - bottom);
		float value5 = (0f - (far + near)) / (far - near);
		float value6 = (0f - 2f * far * near) / (far - near);
		float value7 = -1f;
		Matrix4x4 zero = Matrix4x4.zero;
		zero[0, 0] = value;
		zero[0, 1] = 0f;
		zero[0, 2] = value3;
		zero[0, 3] = 0f;
		zero[1, 0] = 0f;
		zero[1, 1] = value2;
		zero[1, 2] = value4;
		zero[1, 3] = 0f;
		zero[2, 0] = 0f;
		zero[2, 1] = 0f;
		zero[2, 2] = value5;
		zero[2, 3] = value6;
		zero[3, 0] = 0f;
		zero[3, 1] = 0f;
		zero[3, 2] = value7;
		zero[3, 3] = 0f;
		return zero;
	}
}
