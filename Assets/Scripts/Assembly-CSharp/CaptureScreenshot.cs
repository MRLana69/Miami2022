using UnityEngine;

public class CaptureScreenshot : AScreenCapture
{
	public CaptureScreenshot(string captureFolder, string fileName, int width, int height, Camera camera)
		: base(captureFolder, width, height, camera)
	{
		SaveScreenshot(fileName);
		Cleanup();
	}

	public CaptureScreenshot(string captureFolder, string fileName, int width, int height, Camera[] camera)
		: base(captureFolder, width, height, camera)
	{
		SaveScreenshot(fileName);
		Cleanup();
	}
}
