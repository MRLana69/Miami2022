using UI.Generic.Layout;
using UnityEngine;

namespace UI
{
	public class RibbonHelper : MonoBehaviour
	{
		public class RibbonPayload
		{
			public bool shouldShow;

			public string line1;

			public string line2;

			public string line3;
		}

		[SerializeField]
		private UILabel _line1;

		[SerializeField]
		private UILabel _line2;

		[SerializeField]
		private UILabel _line3;

		[SerializeField]
		private Layout _layout;

		public void Setup(RibbonPayload payload)
		{
			base.gameObject.SetActive(payload.shouldShow);
			if (payload.shouldShow)
			{
				if (_line1 != null)
				{
					_line1.text = payload.line1;
				}
				if (_line2 != null)
				{
					_line2.text = payload.line2;
				}
				if (_line3 != null)
				{
					_line3.text = payload.line3;
				}
				if (_layout != null)
				{
					_layout.Align();
				}
			}
		}
	}
}
