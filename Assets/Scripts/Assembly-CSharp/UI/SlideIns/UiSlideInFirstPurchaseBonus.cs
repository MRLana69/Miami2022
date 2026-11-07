using UI.RenderTexture;
using UnityEngine;

namespace UI.SlideIns
{
	public class UiSlideInFirstPurchaseBonus : UISlideIn
	{
		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UILabel _subTitle;

		[SerializeField]
		private FirstPurchaseBonusRenderTextureSetup _firstPurchaseBonusRenderTextureSetup;

		public override void SetupSlideIn(SlideIn slideIn)
		{
			base.gameObject.SetActive(true);
			_title.text = Strings.Get("FIRST_PURCHASE_SLIDE_IN_TITLE");
			_subTitle.text = Strings.Get("FIRST_PURCHASE_SLIDE_IN_SUB_TITLE");
			_firstPurchaseBonusRenderTextureSetup.Setup();
			SlideIn();
		}
	}
}
