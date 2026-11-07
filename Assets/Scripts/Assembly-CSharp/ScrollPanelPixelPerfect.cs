using UnityEngine;

public class ScrollPanelPixelPerfect : MonoBehaviour
{
	private Transform _transform;

	private float _pixelFactor = 1f;

	private UIScrollView dragPanel;

	private void Start()
	{
		_transform = base.transform;
		float num = UIRoot.list[0].activeHeight;
		float num2 = Screen.height;
		_pixelFactor = num / num2;
		dragPanel = GetComponent<UIScrollView>();
	}

	private void Update()
	{
		float y = _transform.localPosition.y;
		float num = Mathf.Round(y * _pixelFactor) / _pixelFactor;
		num = y - num;
		dragPanel.MoveRelative(new Vector3(0f, num, 0f));
	}
}
