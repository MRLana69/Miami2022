using UnityEngine;

namespace UI.Popups.AgeRestrictionPopupInternal
{
	public class ListElementHelper : MonoBehaviour
	{
		[SerializeField]
		private UILabel _label;

		[SerializeField]
		private Color _defaultColor;

		[SerializeField]
		private Color _selectedColor;

		private ListContentElement _listContentElement;

		private ListContentElementReferences _references;

		private UIPanel _scrollViewPanel;

		private const int threshold = 50;

		public ListContentElement listContentElement
		{
			get
			{
				return _listContentElement;
			}
		}

		public void InitCell(ListContentElement lce, ListContentElementReferences references)
		{
			_listContentElement = lce;
			_listContentElement.helper = this;
			_references = references;
			_scrollViewPanel = references.scrollView.gameObject.GetComponent<UIPanel>();
			_label.text = lce.content;
		}

		private void Update()
		{
			float y = _scrollViewPanel.clipOffset.y;
			float y2 = base.transform.localPosition.y;
			float num = Mathf.Abs(y2 - y);
			float t = ((!(num < 50f)) ? 0f : (1f - num / 50f));
			Vector3 a = new Vector3(1f, 1f, 1f);
			Vector3 b = new Vector3(1.6f, 1.6f, 1.6f);
			Vector3 localScale = Vector3.Lerp(a, b, t);
			base.transform.localScale = localScale;
			_label.color = Color.Lerp(_defaultColor, _selectedColor, t);
		}

		private void OnClick()
		{
			_references.centerOnChild.CenterOnTransform(base.transform);
		}
	}
}
