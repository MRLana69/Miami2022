using System.Collections;
using SYBO.Common.Managers;
using SYBO.Components.Utilities;
using UnityEngine;

[AddComponentMenu("Utilities/HUDFPS")]
public class HUDFPS : MonoBehaviour
{
	public Rect windowRect = new Rect(10f, 70f, 200f, 100f);

	public bool updateColor = true;

	public bool allowDrag = true;

	public float frequency = 0.5f;

	public int nbDecimal = 1;

	private float accum;

	private int frames;

	private float accumTotal;

	private int framesTotal;

	private Color color = Color.white;

	private Color minColor = Color.white;

	private Color avgColor = Color.white;

	private float minFpsMin = float.MaxValue;

	private float minFpsMax;

	private float fpsMin = float.MaxValue;

	private float fpsMax;

	private Rect windowRectPrev;

	private string sFPS = string.Empty;

	private string sMinFPS = string.Empty;

	private string sAvgFPS = string.Empty;

	private GUIStyle style;

	private float timeMaxFrame;

	private void Start()
	{
		StartFpsCounter();
	}

	private void Update()
	{
		if (!(Time.deltaTime <= 0f))
		{
			float num = SingletonMonoBehaviour<TimeScaleManager>.Instance.TimeScale / Time.deltaTime;
			accum += num;
			accumTotal += num;
			frames++;
			framesTotal++;
			timeMaxFrame = Mathf.Max(timeMaxFrame, Time.deltaTime);
		}
	}

	public void StartFpsCounter()
	{
		Reset();
		StartCoroutine(FPS());
	}

	private void Reset()
	{
		fpsMin = float.MaxValue;
		fpsMax = 0f;
		minFpsMin = float.MaxValue;
		minFpsMax = 0f;
		accum = 0f;
		frames = 0;
		accumTotal = 0f;
		framesTotal = 0;
		windowRectPrev = windowRect;
	}

	private IEnumerator FPS()
	{
		while (true)
		{
			float fps = accum / (float)frames;
			fpsMin = Mathf.Min(fpsMin, fps);
			fpsMax = Mathf.Max(fpsMax, fps);
			color = ((fps >= 30f) ? Color.green : ((!(fps > 10f)) ? Color.yellow : Color.red));
			float minFPS = ((!(0f < timeMaxFrame)) ? 0f : (1f / timeMaxFrame));
			if (0f < minFPS)
			{
				minFpsMin = Mathf.Min(minFpsMin, minFPS);
			}
			minFpsMax = Mathf.Max(minFpsMax, minFPS);
			minColor = ((minFPS >= 30f) ? Color.green : ((!(minFPS > 10f)) ? Color.yellow : Color.red));
			float avgFps = accumTotal / (float)framesTotal;
			avgColor = ((avgFps >= 30f) ? Color.green : ((!(avgFps > 10f)) ? Color.yellow : Color.red));
			sFPS = string.Format("{0:F1} FPS ({1:F1} - {2:F1})", fps, fpsMin, fpsMax);
			sMinFPS = string.Format("{0:F1} MIN ({1:F1} - {2:F1})", minFPS, minFpsMin, minFpsMax);
			sAvgFPS = string.Format("{0:F1} AVG", avgFps);
			accum = 0f;
			frames = 0;
			timeMaxFrame = 0f;
			yield return new WaitForSeconds(frequency);
		}
	}

	private void OnGUI()
	{
		if (style == null)
		{
			style = new GUIStyle(GUI.skin.label);
			style.normal.textColor = Color.white;
			style.alignment = TextAnchor.MiddleCenter;
		}
		windowRect = GUI.Window(0, windowRect, DoMyWindow, string.Empty);
	}

	private void DoMyWindow(int windowID)
	{
		GUI.color = ((!updateColor) ? Color.white : color);
		GUI.Label(new Rect(0f, -20f, windowRect.width, windowRect.height), sFPS, style);
		GUI.color = ((!updateColor) ? Color.white : minColor);
		GUI.Label(new Rect(0f, 0f, windowRect.width, windowRect.height), sMinFPS, style);
		GUI.color = ((!updateColor) ? Color.white : avgColor);
		GUI.Label(new Rect(0f, 20f, windowRect.width, windowRect.height), sAvgFPS, style);
		if (allowDrag)
		{
			GUI.DragWindow(new Rect(0f, 0f, Screen.width, Screen.height));
		}
		if (windowRect != windowRectPrev)
		{
			Reset();
		}
		windowRectPrev = windowRect;
	}
}
