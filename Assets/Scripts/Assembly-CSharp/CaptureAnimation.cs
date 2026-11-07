using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class CaptureAnimation : AScreenCapture
{
	private Animation _animation;

	private string _animationName = string.Empty;

	private Action _onDone;

	private GameObject _coroutineObject;

	public CaptureAnimation(Animation animation, string captureFolder, string animationName, int width, int height, int fps, Camera camera, Action onDone)
		: this(animation, captureFolder, animationName, width, height, fps, new Camera[1] { camera }, onDone)
	{
	}

	public CaptureAnimation(Animation animation, string captureFolder, string animationName, int width, int height, int fps, Camera[] camera, Action onDone)
		: base(Path.Combine(captureFolder, animationName), width, height, camera)
	{
		Time.captureFramerate = fps;
		_animation = animation;
		_animationName = animationName;
		_onDone = onDone;
		string[] files = Directory.GetFiles(Path.Combine(captureFolder, animationName));
		for (int i = 0; i < files.Length; i++)
		{
			File.Delete(files[i]);
		}
		_coroutineObject = new GameObject();
		_coroutineObject.hideFlags = HideFlags.HideAndDontSave;
		if (_animation.IsPlaying(_animationName))
		{
			_animation.Stop();
			_animation.Rewind();
		}
		_animation.Play(_animationName, PlayMode.StopAll);
		_coroutineObject.AddComponent<NewMonoBehaviour>().StartCoroutine(CaptureAnimationToEnd());
	}

	private IEnumerator CaptureAnimationToEnd()
	{
		yield return null;
		int frameCount = 0;
		while (true)
		{
			if (frameCount != 0)
			{
				string name = _animationName + "_" + frameCount++;
				SaveScreenshot(name);
				if (_animation[_animationName].time >= _animation[_animationName].length)
				{
					break;
				}
			}
			else
			{
				frameCount++;
			}
			yield return null;
		}
		Cleanup();
	}

	public override void Cleanup()
	{
		base.Cleanup();
		UnityEngine.Object.Destroy(_coroutineObject);
		Time.captureFramerate = 0;
		if (_onDone != null)
		{
			_onDone();
		}
	}
}
