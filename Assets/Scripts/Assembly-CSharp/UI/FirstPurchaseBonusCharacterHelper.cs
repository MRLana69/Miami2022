using System.Linq;
using UI.RenderTexture;
using UI.Screens;
using UnityEngine;

namespace UI
{
	public class FirstPurchaseBonusCharacterHelper : MonoBehaviour
	{
		public class FirstPurchasePayload
		{
			public int width = 292;

			public int height = 60;
		}

		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _subTitle;

		[SerializeField]
		private GameObject _rootGo;

		[SerializeField]
		private FirstPurchaseBonusRenderTextureSetup _firstPurchaseBonusRenderTextureSetup;

		private void Awake()
		{
			FirstPurchaseBonusManager.onFirstPurchaseBonusBecameInactive += OnFirstPurchaseBonusBecameInactive;
		}

		private void OnDestroy()
		{
			FirstPurchaseBonusManager.onFirstPurchaseBonusBecameInactive -= OnFirstPurchaseBonusBecameInactive;
		}

		private void OnFirstPurchaseBonusBecameInactive()
		{
			Object.Destroy(_rootGo);
		}

		private void OnEnable()
		{
			if (!FirstPurchaseBonusManager.running && Globals.DEBUG)
			{
				FirstPurchaseBonusManager.StartStateCheckCoroutine();
			}
			Show();
		}

		public void Setup(FirstPurchasePayload payload)
		{
			UIWidget component = GetComponent<UIWidget>();
			component.height = payload.height;
			component.width = payload.width;
			component.ResetAndUpdateAnchors();
			GetComponentsInChildren<UIRect>().ToList().ForEach((UIRect r) =>
			{
				r.ResetAndUpdateAnchors();
			});
		}

		public void Show()
		{
			SetupTexts();
			ActivateFirstPurchaseBonusBannerAnimation();
		}

		public void ActivateFirstPurchaseBonusBannerAnimation()
		{
			_firstPurchaseBonusRenderTextureSetup.Setup();
		}

		private void SetupTexts()
		{
			_title.text = Strings.Get("FIRST_PURCHASE_STORE_TAB_TITLE");
			_subTitle.text = Strings.Get("FIRST_PURCHASE_STORE_TAB_SUB_TITLE");
		}
	}
}
