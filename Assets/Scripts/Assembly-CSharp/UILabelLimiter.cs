using UnityEngine;

public class UILabelLimiter : MonoBehaviour
{
	private UILabel _label;

	private void Awake()
	{
		_label = GetComponent<UILabel>();
		if (_label == null)
		{
			SDebug.LogError("This script is not attached to a game object with an UILabel!");
		}
	}

	private void Start()
	{
		LimitLabel();
	}

	public void LimitLabel()
	{
		if (!(_label == null))
		{
			string processedText = _label.processedText;
			if (processedText.CompareTo(_label.text) != 0 && processedText.Length > 3)
			{
				processedText = processedText.Substring(0, processedText.Length - 3) + "...";
				_label.text = processedText;
			}
		}
	}
}
