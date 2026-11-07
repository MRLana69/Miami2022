using Kiloo.Common.Extensions;
using SYBO.Subway.Info;
using UI.RenderTexture;
using UnityEngine;

namespace UI.ShopInternal
{
	public class ShopHeader : MonoBehaviour
	{
		public class ShopHeaderPayload
		{
			public string headerText;

			public SegmentDivider.SegmentDividerPayload segmentDividerPayload;

			public bool showBoomBot;

			public Characters.CharacterId headerCharacter;
		}

		[Header("Header setups")]
		[SerializeField]
		private UILabel _headerLabel;

		[SerializeField]
		private TrickyHeaderRenderTextureSetup _trickyRenderTextureSetup;

		[SerializeField]
		private KingHeaderRenderTextureSetup _kingRenderTextureSetup;

		[SerializeField]
		private YutaniHeaderRenderTextureSetup _yutaniRenderTextureSetup;

		[Header("First purchase bonus setup")]
		[SerializeField]
		private SegmentDivider _segmentDividerPrefab;

		[SerializeField]
		private BoomBotBanner _boomBotBannerSection;

		private SegmentDivider _segmentDivider;

		public void Setup(ShopHeaderPayload payload)
		{
			_headerLabel.text = payload.headerText;
			if (payload.segmentDividerPayload != null)
			{
				SetupSegmentDivider(payload.segmentDividerPayload);
			}
			if (payload.showBoomBot)
			{
				_boomBotBannerSection.gameObject.SetActive(true);
				_boomBotBannerSection.Setup();
			}
			else
			{
				_boomBotBannerSection.gameObject.SetActive(false);
			}
			switch (payload.headerCharacter)
			{
			case Characters.CharacterId.tricky:
				_trickyRenderTextureSetup.Setup();
				break;
			case Characters.CharacterId.king:
				_kingRenderTextureSetup.Setup();
				break;
			case Characters.CharacterId.yutani:
				_yutaniRenderTextureSetup.Setup();
				break;
			}
		}

		private void SetupSegmentDivider(SegmentDivider.SegmentDividerPayload payload)
		{
			if (_segmentDivider == null)
			{
				float posY = 2f - (float)GetComponent<UIWidget>().height / 2f;
				_segmentDivider = Object.Instantiate(_segmentDividerPrefab, base.transform);
				_segmentDivider.transform.Reset();
				_segmentDivider.transform.SetLocalPosY(posY);
			}
			_segmentDivider.Setup(payload);
		}
	}
}
