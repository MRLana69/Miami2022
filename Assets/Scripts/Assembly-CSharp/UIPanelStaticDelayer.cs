using System.Collections;
using UnityEngine;

public class UIPanelStaticDelayer : MonoBehaviour
{
	private bool inited;

	[SerializeField]
	private int framesToWait = 2;

	[SerializeField]
	private bool refreshOnEnable;

	private UIPanel _panel;

	private void Awake()
	{
		_panel = GetComponent<UIPanel>();
		if (!(_panel == null) && framesToWait >= 0)
		{
			OrderStaticDelay();
			inited = true;
		}
	}

	private void OrderStaticDelay()
	{
		if (_panel.widgetsAreStatic)
		{
			_panel.widgetsAreStatic = false;
		}
		StartCoroutine(SetStaticDelayed(framesToWait, _panel));
	}

	private void OnEnable()
	{
		if (refreshOnEnable && inited)
		{
			OrderStaticDelay();
		}
	}

	public IEnumerator SetStaticDelayed(int delayFrames, UIPanel panel)
	{
		int num = 0;
		while (num < delayFrames)
		{
			panel.Refresh();
			num++;
			yield return null;
		}
		panel.widgetsAreStatic = true;
	}
}
