using UnityEngine;

namespace UI
{
	public class UICameraScreenClipping : MonoBehaviour
	{
		private const float SCREEN_WIDTH = 326f;

		private const float POPUP_WIDTH = 274f;

		private Rect previousRect = default(Rect);

		[SerializeField]
		private bool _setClippingOnStart = true;

		private void Start()
		{
			if (_setClippingOnStart)
			{
				CalculateClipping(false);
			}
		}

		public void CalculateClipping(bool popupSizedClip)
		{
			Camera component = base.gameObject.GetComponent<Camera>();
			if (component == null)
			{
				SDebug.LogError("The UICameraScreenClipping script is not attached to a Camera");
				return;
			}
			UIRoot uIRoot = UIRoot.list[0];
			if (uIRoot == null)
			{
				SDebug.LogError("UIRoot doesn't exist.");
				return;
			}
			Rect rect = component.rect;
			float num = (float)uIRoot.activeHeight * (float)Screen.width / (float)Screen.height;
			float num2 = 1f / num * ((!popupSizedClip) ? 326f : 274f);
			float x = (1f - num2) / 2f;
			rect.x = x;
			rect.width = num2;
			if (previousRect != component.rect)
			{
				previousRect = component.rect;
			}
			component.rect = rect;
		}

		public void ResetClipping()
		{
			Camera component = base.gameObject.GetComponent<Camera>();
			component.rect = previousRect;
			previousRect = default(Rect);
		}
	}
}
