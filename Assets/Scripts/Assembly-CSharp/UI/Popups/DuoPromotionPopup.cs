using System.Collections.Generic;
using Info;
using JetBrains.Annotations;
using SYBO.Subway.Promotion;
using UI.Generic;
using UI.Generic.Layout;
using UI.RenderTexture;
using UI.Screens;
using UnityEngine;

namespace UI.Popups
{
	public class DuoPromotionPopup : UIPopup
	{
		private enum EntryPosition
		{
			First = 0,
			Second = 1
		}

		[Header("Anchors")]
		[SerializeField]
		private GameObject _topAnchor;

		[Header("Top")]
		[SerializeField]
		private UILabel _popupTitle;

		[SerializeField]
		private UILabel _popupSubTitle;

		[SerializeField]
		private UITexture _popupStickerTexture;

		[SerializeField]
		private TimerHelper _timerPrefab;

		[SerializeField]
		private Transform _timerContainer;

		[SerializeField]
		private Layout _timerLayout;

		[SerializeField]
		private UILabel _labelBanner;

		private TimerHelper _timer;

		[Header("Renders")]
		[SerializeField]
		private UITexture _character1Texture;

		[SerializeField]
		private UITexture _character2Texture;

		[SerializeField]
		private UITexture _board1Texture;

		[SerializeField]
		private UITexture _board2Texture;

		[Header("Bottom")]
		[SerializeField]
		private UILabel _labelGotoShop;

		[Header("Background")]
		[SerializeField]
		private UISprite backgroundSprite;

		[SerializeField]
		private UISprite flashSprite;

		private DuoPromotion _duoPromotion;

		protected override float screenOffset
		{
			get
			{
				return -13f;
			}
		}

		protected override void Init()
		{
			base.Init();
			_duoPromotion = PromotionManager.Instance.TryGetCurrentPromotion<DuoPromotion>();
			if (_duoPromotion == null)
			{
				Debug.LogError("Current Promotion is not a duo promotion.");
				return;
			}
			backgroundSprite.gradientTop = _duoPromotion.BackgroundColorTop;
			backgroundSprite.gradientBottom = _duoPromotion.BackgroundColorBottom;
			foreach (Transform item in backgroundSprite.transform)
			{
				UISprite component = item.GetComponent<UISprite>();
				component.color = ((!(component == flashSprite)) ? _duoPromotion.BeamColor : _duoPromotion.FlashColor);
			}
			SetupBoards();
			SetupCharacters();
			InitializeTimer();
			SetupTop();
			SetupBottom();
		}

		private UITexture CharacterTextureForPosition(EntryPosition position)
		{
			return (position != EntryPosition.First) ? _character2Texture : _character1Texture;
		}

		private void SetupBoards()
		{
			SetupBoard(_duoPromotion.Entry1, EntryPosition.First);
			SetupBoard(_duoPromotion.Entry2, EntryPosition.Second);
		}

		private void SetupCharacters()
		{
			SetupCharacter(_duoPromotion.Entry1, EntryPosition.First);
			SetupCharacter(_duoPromotion.Entry2, EntryPosition.Second);
		}

		private void SetupCharacter(DuoPromotionEntry entry, EntryPosition position)
		{
			RenderTextureObject renderTextureObject = new RenderTextureObject();
			renderTextureObject.animationClip = entry.CharacterAnimation;
			renderTextureObject.position = entry.PopupCharacterPosition;
			renderTextureObject.rotation = entry.PopupCharacterRotation;
			renderTextureObject.scale = entry.PopupCharacterScale;
			renderTextureObject.prefab = UIModelController.Instance.ModelPrefab;
			RenderTextureObject item = renderTextureObject;
			CharacterRenderTextureSetup characterRenderTextureSetup = new CharacterRenderTextureSetup();
			UITexture texture = ((position != EntryPosition.First) ? _character2Texture : _character1Texture);
			characterRenderTextureSetup.Setup(entry.CharacterId, entry.CharacterThemeIndex, texture, new List<RenderTextureObject> { item }, new CameraSettings
			{
				backgroundColor = new Color(1f, 1f, 1f, 0f)
			});
		}

		private void SetupBoard(DuoPromotionEntry entry, EntryPosition position)
		{
			Boards.Board board = Boards.boardData[entry.BoardId];
			string path = "Prefabs/HoverboardAssets/" + board.boardModelName;
			RenderTextureObject renderTextureObject = new RenderTextureObject();
			renderTextureObject.position = entry.PopupBoardPosition;
			renderTextureObject.rotation = entry.PopupBoardRotation;
			renderTextureObject.scale = entry.PopupBoardScale;
			renderTextureObject.prefab = Resources.Load(path) as GameObject;
			renderTextureObject.animationDelay = Random.Range(0f, 1f);
			renderTextureObject.animationClip = entry.BoardAnimation;
			RenderTextureObject item = renderTextureObject;
			BoardRenderTextureSetup boardRenderTextureSetup = new BoardRenderTextureSetup();
			UITexture texture = ((position != EntryPosition.First) ? _board2Texture : _board1Texture);
			boardRenderTextureSetup.Setup(false, texture, new List<RenderTextureObject> { item }, new CameraSettings
			{
				backgroundColor = new Color(1f, 1f, 1f, 0f)
			});
		}

		private void InitializeTimer()
		{
			_timer = Object.Instantiate(_timerPrefab, _timerContainer);
			_timer.SetTimerColors(_duoPromotion.TimerTextColor, _duoPromotion.TimerBgColor);
			_timer.SetAlignment(HorizontalAlignment.Left);
		}

		private void SetupTop()
		{
			_popupTitle.text = Strings.Get(_duoPromotion.TitleId);
			_popupTitle.color = _duoPromotion.TitleTextColor;
			_popupSubTitle.text = Strings.Get(_duoPromotion.SubtitleId);
			_popupSubTitle.color = _duoPromotion.SubtitleTextColor;
			_labelBanner.text = Strings.Get(_duoPromotion.BanneTextid);
			_popupStickerTexture.mainTexture = _duoPromotion.StickerTexture;
			_popupStickerTexture.transform.localPosition = new Vector3(_duoPromotion.StickerPosition.x, _duoPromotion.StickerPosition.y, 0f);
			_popupStickerTexture.GetComponent<UIWidget>().width = (int)_duoPromotion.StickerSize.x;
			_popupStickerTexture.GetComponent<UIWidget>().height = (int)_duoPromotion.StickerSize.y;
			_timer.StartTimer(_duoPromotion.TimeLeft);
			_timerLayout.Align();
		}

		private void SetupBottom()
		{
			_labelGotoShop.text = Strings.Get("POPUP_PROMOTION_GOTO_SHOP");
		}

		[UsedImplicitly]
		public void OnCloseClicked()
		{
			UIScreenController.Instance.CloseActivePopups(screenName);
		}

		[UsedImplicitly]
		public void OnGoToShop()
		{
			StoreScreen.ScrollToInterestPointOnNextShow(StoreInterestPoint.LimitedDeals);
			UIScreenController.Instance.PushScreen("StoreScreen");
			ClosePopup();
		}
	}
}
