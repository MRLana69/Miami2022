using System.Collections.Generic;
using Info;
using Kiloo.Common.Extensions;
using SYBO.Subway.Promotion;
using UI;
using UI.Generic.Layout;
using UI.RenderTexture;
using UnityEngine;

public class MultiboardPromotionEntryView : MonoBehaviour
{
	[Header("Cost")]
	[SerializeField]
	private HorizontalLayout _currencyContainer;

	[SerializeField]
	private UISprite _spriteCurrency;

	[SerializeField]
	private UILabel _labelCost;

	[Header("Other")]
	[SerializeField]
	private UILabel _labelTitle;

	[SerializeField]
	private RenderTextureObject _starObject;

	[SerializeField]
	private UITexture _boardTexture;

	[Header("Background")]
	[SerializeField]
	private UIWidget _widgetStroke;

	[SerializeField]
	private UIWidget _widgetBackdrop;

	[SerializeField]
	private UISprite _spriteBackground;

	[Header("Flare/Flashes")]
	[SerializeField]
	private UISprite _spriteFlash;

	[SerializeField]
	private UISprite _spriteFlare1;

	[SerializeField]
	private UISprite _spriteFlare2;

	[SerializeField]
	private UISprite _spriteFlare3;

	[SerializeField]
	private UISprite _spriteFlare4;

	private MultiBoardBundlePromotionEntry _entry;

	public void Init(MultiBoardBundlePromotionEntry entry)
	{
		_entry = entry;
		Boards.Board board = Boards.boardData[_entry.BoardId];
		_widgetStroke.color = _entry.StrokeColor;
		_widgetBackdrop.color = _entry.BackdropColor;
		_spriteBackground.gradientTop = _entry.BackgroundColorTop;
		_spriteBackground.gradientBottom = _entry.BackgroundColorBottom;
		_spriteCurrency.width = ((board.unlockType != PurchaseData.UnlockType.Keys) ? 20 : 15);
		_spriteCurrency.spriteName = ((board.unlockType != PurchaseData.UnlockType.Coins) ? "icon_key" : "icon_coin_large");
		_labelTitle.text = Strings.Get(board.name);
		_labelTitle.color = _entry.TitleColor;
		_labelCost.text = board.price.FormatSI();
		_spriteFlash.color = _entry.FlashColor;
		_spriteFlare1.color = _entry.BeamColor;
		_spriteFlare2.color = _entry.BeamColor;
		_spriteFlare3.color = _entry.BeamColor;
		_spriteFlare4.color = _entry.BeamColor;
		_currencyContainer.Align();
		string path = "Prefabs/HoverboardAssets/" + board.boardModelName;
		RenderTextureObject renderTextureObject = new RenderTextureObject();
		renderTextureObject.position = _entry.BoardPosition;
		renderTextureObject.rotation = _entry.BoardRotation;
		renderTextureObject.scale = _entry.BoardScale;
		renderTextureObject.prefab = Resources.Load(path) as GameObject;
		renderTextureObject.animationDelay = Random.Range(0f, 1f);
		renderTextureObject.animationClip = _entry.AnimationClip;
		RenderTextureObject item = renderTextureObject;
		BoardRenderTextureSetup boardRenderTextureSetup = new BoardRenderTextureSetup();
		boardRenderTextureSetup.Setup(false, _boardTexture, new List<RenderTextureObject> { item, _starObject }, new CameraSettings
		{
			backgroundColor = new Color(1f, 1f, 1f, 0f)
		});
	}
}
