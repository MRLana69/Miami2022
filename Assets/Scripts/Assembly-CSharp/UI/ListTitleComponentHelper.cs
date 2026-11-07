using UnityEngine;

namespace UI
{
	public class ListTitleComponentHelper : MonoBehaviour
	{
		[SerializeField]
		private UILabel label;

		[SerializeField]
		private UIWidget _frameWidget;

		public void Setup(string text, Color color, int panelWidth = 300, int fontSize = -1)
		{
			label.text = text;
			label.color = color;
			if (fontSize != -1)
			{
				label.fontSize = fontSize;
			}
			_frameWidget.width = panelWidth;
		}
	}
}
