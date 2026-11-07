using NaughtyAttributes;
using UI;
using UI.RenderTexture;
using UnityEngine;

public class BoomBotBanner : MonoBehaviour
{
	[Header("First purchase bonus setup")]
	[SerializeField]
	private UILabel _boomBotHeader;

	[SerializeField]
	private UILabel _boomBotSubHeader;

	[SerializeField]
	private UISprite _boomBotDividerLine;

	[SerializeField]
	private UISprite _boomBotBackground;

	[SerializeField]
	private FirstPurchaseBonusRenderTextureSetup _boombotRenderTextureSetup;

	private bool _bannerSetup;

	public void Setup()
	{
		if (!_bannerSetup)
		{
			_bannerSetup = true;
			SetupBasic();
			_boombotRenderTextureSetup.Setup();
		}
	}

	[Button(null)]
	private void SetupBasic()
	{
		_boomBotHeader.text = Strings.Get("FIRST_PURCHASE_STORE_TAB_TITLE");
		_boomBotSubHeader.text = Strings.Get("FIRST_PURCHASE_STORE_TAB_SUB_TITLE");
		ColorData instance = ColorData.instance;
		_boomBotHeader.color = instance.crimson;
		_boomBotHeader.effectColor = instance.lightShadow;
		_boomBotSubHeader.color = instance.mahogany;
		_boomBotSubHeader.effectColor = instance.lightShadow;
		_boomBotDividerLine.color = instance.crimson;
		_boomBotBackground.color = instance.sun;
	}
}
