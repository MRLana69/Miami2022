using UI.RenderTexture;
using UnityEngine;

namespace UI
{
	public class DinoLogoCharacterHelper : MonoBehaviour
	{
		public class DinoLogoPayload
		{
			public int panelOffset;
		}

		[SerializeField]
		private FacebookDinoLogoRenderTextureSetup _dinoLogoRenderTextureSetup;

		public void Init(DinoLogoPayload payload)
		{
			UIPanel[] componentsInChildren = GetComponentsInChildren<UIPanel>();
			UIPanel[] array = componentsInChildren;
			foreach (UIPanel uIPanel in array)
			{
				if (uIPanel.depth < 100)
				{
					uIPanel.depth += payload.panelOffset;
				}
			}
		}

		private void OnDisable()
		{
			Object.Destroy(base.gameObject);
		}

		private void OnEnable()
		{
			ActivateDinoLogoAnimation();
		}

		private void ActivateDinoLogoAnimation()
		{
			_dinoLogoRenderTextureSetup.Setup();
		}
	}
}
