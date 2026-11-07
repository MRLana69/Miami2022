using System.Collections.Generic;
using System.Linq;
using UI;
using UI.Generic.Layout;
using UnityEngine;

public class TutorialMultilineLabelsHelper : MonoBehaviour
{
	[SerializeField]
	private UILabel _label1Title;

	[SerializeField]
	private UILabel _label1Subtitle;

	[SerializeField]
	private UILabel _label2Title;

	[SerializeField]
	private UILabel _label2Subtitle;

	[SerializeField]
	private UILabel _label3Title;

	[SerializeField]
	private UILabel _label3Subtitle;

	[SerializeField]
	private VerticalLayout _verticalLayout;

	private static readonly Vector3 _defaultPosition = new Vector3(0f, -60f, 0f);

	private const int DEFAULT_FONT_SIZE_TITLE = 20;

	private const int DEFAULT_FONT_SIZE_SUBTITLE = 16;

	public static TutorialMultilineLabelsHelper AddToGameObject(GameObject go, GameObject prefab, Vector3? position = null)
	{
		GameObject gameObject = go.AddChild(prefab);
		TutorialMultilineLabelsHelper component = gameObject.GetComponent<TutorialMultilineLabelsHelper>();
		if (component == null)
		{
			SDebug.LogError("Unable to find TutorialMultilineLabelsHelper on prefab, check setup");
		}
		else
		{
			component.SetupPosition((!position.HasValue) ? _defaultPosition : position.Value);
		}
		return component;
	}

	public void SetupPosition(Vector3 position)
	{
		base.transform.localPosition = position;
	}

	public void SetText(string line1, string line2, string line3, string line4, string line5, string line6)
	{
		_label1Title.text = Strings.Get(line1);
		_label1Title.color = ColorData.instance.midnight;
		_label1Subtitle.text = Strings.Get(line2);
		_label1Subtitle.color = ColorData.instance.peacock;
		_label2Title.text = Strings.Get(line3);
		_label2Title.color = ColorData.instance.midnight;
		_label2Subtitle.text = Strings.Get(line4);
		_label2Subtitle.color = ColorData.instance.peacock;
		_label3Title.text = Strings.Get(line5);
		_label3Title.color = ColorData.instance.midnight;
		_label3Subtitle.text = Strings.Get(line6);
		_label3Subtitle.color = ColorData.instance.peacock;
		AdjustFontSizes();
	}

	[ContextMenu("Adjust font sizes")]
	private void AdjustFontSizes()
	{
		UILabel[] array = new UILabel[3] { _label1Title, _label2Title, _label3Title };
		UILabel[] array2 = new UILabel[3] { _label1Subtitle, _label2Subtitle, _label3Subtitle };
		SetFontSizes(array, 20);
		SetFontSizes(array2, 16);
		int num = array.Select((UILabel title) => title.finalFontSize).Min();
		int num2 = array2.Select((UILabel subtitle) => subtitle.finalFontSize).Min();
		float num3 = (float)num2 / (float)num;
		float num4 = num3 - 0.8f;
		if (num4 < -float.Epsilon)
		{
			num = Mathf.RoundToInt((float)num2 / 0.8f);
		}
		else if (num4 > float.Epsilon)
		{
			num2 = Mathf.RoundToInt((float)num * 0.8f);
		}
		SetFontSizes(array, num);
		SetFontSizes(array2, num2);
		_verticalLayout.Align();
	}

	private void SetFontSizes(IEnumerable<UILabel> labels, int fontSize)
	{
		foreach (UILabel label in labels)
		{
			label.fontSize = fontSize;
		}
	}
}
