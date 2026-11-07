using NaughtyAttributes;
using UI.Screens;
using UnityEngine;

namespace UI
{
	[RequireComponent(typeof(UIRoot))]
	public class RootScaler : MonoBehaviour
	{
		private UIRoot _myUIRoot;

		[ContextMenu("Scale now")]
		[Button("Scale now")]
		private void Awake()
		{
			_myUIRoot = base.gameObject.GetComponent<UIRoot>();
			if (UIScreen.IsScreenHeightOutOfProportion())
			{
				float num = Screen.height;
				if (num > 480f)
				{
					float num2 = (float)Screen.width / 320f;
					num2 = (float)Screen.height / num2;
					_myUIRoot.manualHeight = (int)num2;
				}
			}
			else
			{
				_myUIRoot.manualHeight = 480;
			}
		}
	}
}
