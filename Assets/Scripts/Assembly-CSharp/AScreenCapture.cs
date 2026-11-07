using System.IO;
using UnityEngine;

public abstract class AScreenCapture
{
	private string _captureFolder = string.Empty;

	private int _width;

	private int _height;

	private Camera[] _camera;

	private RenderTexture _renderTexture;

	private Texture2D _screenshotTexture;

	public AScreenCapture(string captureFolder, int width, int height, Camera[] camera, int antiAliasing = 8)
	{
		_captureFolder = captureFolder;
		Directory.CreateDirectory(_captureFolder);
		_width = width;
		_height = height;
		_camera = camera;
		_renderTexture = new RenderTexture(_width, _height, 32);
		_renderTexture.antiAliasing = antiAliasing;
		_screenshotTexture = new Texture2D(_renderTexture.width, _renderTexture.height, TextureFormat.ARGB32, false);
		if (camera.Length <= 1)
		{
			return;
		}
		Camera[] camera2 = _camera;
		foreach (Camera camera3 in camera2)
		{
			string text = camera3.name;
			if (camera3 == Camera.main)
			{
				text = "Main";
			}
			Directory.CreateDirectory(_captureFolder + "/" + text);
		}
	}

	public AScreenCapture(string captureFolder, int width, int height, Camera camera, int antiAliasing = 8)
	{
		_captureFolder = captureFolder;
		Directory.CreateDirectory(_captureFolder);
		_width = width;
		_height = height;
		_camera = new Camera[1] { camera };
		_renderTexture = new RenderTexture(_width, _height, 32);
		_renderTexture.antiAliasing = antiAliasing;
		_screenshotTexture = new Texture2D(_renderTexture.width, _renderTexture.height, TextureFormat.ARGB32, false);
	}

	public virtual void Cleanup()
	{
		Object.Destroy(_renderTexture);
	}

	protected void SaveScreenshot(string name)
	{
		Camera[] camera = _camera;
		foreach (Camera camera2 in camera)
		{
			string text = name;
			Screenshot(camera2);
			if (_camera.Length > 1)
			{
				string text2 = camera2.name;
				if (camera2 == Camera.main)
				{
					text2 = "Main";
				}
				text = text2 + "/" + text;
			}
			byte[] bytes = _screenshotTexture.EncodeToPNG();
			File.WriteAllBytes(Path.Combine(_captureFolder, text + ".png"), bytes);
		}
	}

	private void Screenshot(Camera camera)
	{
		camera.targetTexture = _renderTexture;
		camera.Render();
		camera.targetTexture = null;
		RenderTexture.active = _renderTexture;
		_screenshotTexture.ReadPixels(new Rect(0f, 0f, _renderTexture.width, _renderTexture.height), 0, 0);
		_screenshotTexture.Apply();
		RenderTexture.active = null;
	}
}
