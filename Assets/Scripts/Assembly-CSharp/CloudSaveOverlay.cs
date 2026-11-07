using NaughtyAttributes;
using UnityEngine;

public class CloudSaveOverlay : MonoBehaviour
{
	[SerializeField]
	private UILabel _titleLabel;

	[Button(null)]
	public void TestLabel()
	{
		UpdateLabel(Strings.Get("CLOUDSAVE_OVERLAY_TEXT"));
	}

	public void UpdateLabel(string text)
	{
		if (_titleLabel != null)
		{
			_titleLabel.text = text;
		}
	}
}
