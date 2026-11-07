using System.Collections;
using System.IO;
using SYBO.Subway.Themes;
using UnityEngine;

public class CaptureInGame : AScreenCapture
{
	private GameObject _coroutineObject;

	public CaptureInGame(string captureFolder, int width, int height, Camera camera)
		: base(captureFolder, width, height, camera)
	{
		Time.captureFramerate = 60;
		string[] files = Directory.GetFiles(captureFolder);
		for (int i = 0; i < files.Length; i++)
		{
			File.Delete(files[i]);
		}
		_coroutineObject = new GameObject();
		_coroutineObject.hideFlags = HideFlags.HideAndDontSave;
		_coroutineObject.AddComponent<NewMonoBehaviour>().StartCoroutine(CaptureAllFrames());
	}

	private IEnumerator CaptureAllFrames()
	{
		int frameCount = 0;
		while (true)
		{
			string fileName = "frame_" + frameCount++;
			ThemeManager.Instance.DistortionYFlipped = true;
			SaveScreenshot(fileName);
			ThemeManager.Instance.DistortionYFlipped = false;
			yield return null;
		}
	}

	public override void Cleanup()
	{
		base.Cleanup();
		_coroutineObject.GetComponent<MonoBehaviour>().StopAllCoroutines();
		Object.Destroy(_coroutineObject);
		Time.captureFramerate = 0;
	}
}
