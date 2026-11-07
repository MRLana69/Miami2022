using UnityEngine;

[ExecuteInEditMode]
public class AnchorWidthResizerHelper : MonoBehaviour
{
	[SerializeField]
	private UIWidget _myWidget;

	[SerializeField]
	private UIWidget[] _allWidgets = new UIWidget[0];

	[SerializeField]
	private int _offset;

	private void Update()
	{
		if (_myWidget == null)
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < _allWidgets.Length; i++)
		{
			if (_allWidgets[i].width > num)
			{
				num = _allWidgets[i].width;
			}
		}
		if (num != -1)
		{
			_myWidget.width = num + _offset;
		}
	}

	private void Reset()
	{
		_myWidget = GetComponent<UIWidget>();
	}
}
