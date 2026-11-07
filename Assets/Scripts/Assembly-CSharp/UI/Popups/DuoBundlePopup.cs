using System;
using System.Collections.Generic;
using Data.Managers;
using Data.Save;
using InApp;
using Info;
using JetBrains.Annotations;
using Kiloo.Common.Backend.PurchaseValidation;
using NaughtyAttributes;
using SYBO.Subway.Info;
using SYBO.Subway.Promotion;
using UI.Generic;
using UI.Generic.Layout;
using UI.RenderTexture;
using UI.Screens;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace UI.Popups
{
	public class DuoBundlePopup : UIPopup
	{
		[Serializable]
		private class TexturePositioningData
		{
			public Vector3 position = Vector3.zero;

			public Vector3 rotation = Vector3.zero;

			public Vector3 scale = Vector3.one;
		}

		[Header("Top")]
		[SerializeField]
		private UILabel _popupTitle;

		[SerializeField]
		private UILabel _popupSubTitle;

		[SerializeField]
		private TimerHelper _timerPrefab;

		[SerializeField]
		private Transform _timerContainer;

		[SerializeField]
		private Layout _timerLayout;

		[SerializeField]
		private UITexture _popupStickerTexture;

		[Header("BoardAndCharacter")]
		[SerializeField]
		private TexturePositioningData _character1PositionData;

		[SerializeField]
		private TexturePositioningData _character2PositionData;

		[SerializeField]
		private TexturePositioningData _board1PositionData;

		[SerializeField]
		private TexturePositioningData _board2PositionData;

		[SerializeField]
		private UILabel _board1Label;

		[SerializeField]
		private UILabel _character1Label;

		[Header("Colors")]
		[SerializeField]
		private Color _fontColorRegular;

		[SerializeField]
		private Color _fontColorPurchased;

		[SerializeField]
		private Color _bundle1BgColorHigh;

		[SerializeField]
		private Color _bundle1BgColorLow;

		[SerializeField]
		private Color _bundle2BgColorHigh;

		[SerializeField]
		private Color _bundle2BgColorLow;

		[SerializeField]
		private Color _bundlePurchasedBgColorHigh;

		[SerializeField]
		private Color _bundlePurchasedBgColorLow;

		[Header("BackgroundsAndSprites")]
		[SerializeField]
		private UISprite _bgCharacter1;

		[SerializeField]
		private UISprite _bgBoard1;

		[SerializeField]
		private UISprite _board1CheckMark;

		[SerializeField]
		private UISprite _character1CheckMark;

		[Header("Renders")]
		[SerializeField]
		private UITexture _character1Texture;

		[SerializeField]
		private UITexture _character2Texture;

		[SerializeField]
		private UITexture _board1Texture;

		[SerializeField]
		private UITexture _board2Texture;

		[SerializeField]
		private UILabel _labelBanner;

		[SerializeField]
		private UILabel _buttonText;

		private DuoPromotion _duoPromotion;

		private TimerHelper _timer;

		private bool _subscribed;

		private string CURRENT_PROMOTION;

		private bool _isCharacter1Owned;

		private bool _isCharacter2Owned;

		private bool _isBoard1Owned;

		private bool _isBoard2Owned;

		private void OnEnable()
		{
			if (!_subscribed && InAppManager.instance != null)
			{
				InAppManager.instance.onPurchaseSuccess += OnPurchaseSuccess;
				InAppManager.instance.onItemRestored += OnItemRestored;
				_subscribed = true;
			}
		}

		private void OnDisable()
		{
			if (_subscribed)
			{
				if (InAppManager.instance != null)
				{
					InAppManager.instance.onPurchaseSuccess -= OnPurchaseSuccess;
					InAppManager.instance.onItemRestored -= OnItemRestored;
				}
				_subscribed = false;
			}
		}

		protected override void Init()
		{
			_duoPromotion = PromotionManager.Instance.TryGetCurrentPromotion() as DuoPromotion;
			if (_duoPromotion == null)
			{
				Debug.LogError("Current Promotion is not a duo promotion.");
				return;
			}
			string[] allProductIds = InAppData.GetAllProductIds();
			foreach (string text in allProductIds)
			{
				if (text == _duoPromotion.InAppDataKey)
				{
					CURRENT_PROMOTION = text;
					break;
				}
			}
			InitializeTimer();
			SetupTop();
			SetupBoardsAndCharacters();
			SetupButton();
		}

		private void InitializeTimer()
		{
			_timer = UnityEngine.Object.Instantiate(_timerPrefab, _timerContainer);
			_timer.SetTimerColors(_duoPromotion.TimerTextColor, _duoPromotion.TimerBgColor);
			_timer.SetAlignment(HorizontalAlignment.Left);
		}

		[Button("UpdateBoardsAndCharacters")]
		private void SetupBoardsAndCharacters()
		{
			SetupBoard(_duoPromotion.Entry1, _board1Texture, _board1PositionData);
			SetupBoard(_duoPromotion.Entry2, _board2Texture, _board2PositionData);
			SetupCharacter(_duoPromotion.Entry1, _character1Texture, _character1PositionData);
			SetupCharacter(_duoPromotion.Entry2, _character2Texture, _character2PositionData);
		}

		private void SetupLabelsAndBackgrounds()
		{
			Boards.BoardId boardId = _duoPromotion.Entry1.BoardId;
			Characters.CharacterId characterId = _duoPromotion.Entry1.CharacterId;
			Characters.Model model = Characters.characterData[characterId];
			Boards.Board board = Boards.boardData[boardId];
			if (model != null)
			{
				_character1Label.text = Strings.Get(model.name);
			}
			if (board != null)
			{
				_board1Label.text = Strings.Get(board.name);
			}
			bool flag = InventorySave.unlockedCharacters.Contains(characterId);
			bool flag2 = InventorySave.unlockedBoards.Contains(boardId);
			_board1CheckMark.gameObject.SetActive(flag2);
			_character1CheckMark.gameObject.SetActive(flag);
			if (flag)
			{
				_bgCharacter1.gradientTop = _bundlePurchasedBgColorHigh;
				_bgCharacter1.gradientBottom = _bundlePurchasedBgColorLow;
				_character1Label.color = _fontColorPurchased;
			}
			else
			{
				_bgCharacter1.gradientTop = _bundle1BgColorHigh;
				_bgCharacter1.gradientBottom = _bundle1BgColorLow;
				_character1Label.color = _fontColorRegular;
			}
			if (flag2)
			{
				_bgBoard1.gradientTop = _bundlePurchasedBgColorHigh;
				_bgBoard1.gradientBottom = _bundlePurchasedBgColorLow;
				_board1Label.color = _fontColorPurchased;
			}
			else
			{
				_bgBoard1.gradientTop = _bundle1BgColorHigh;
				_bgBoard1.gradientBottom = _bundle1BgColorLow;
				_board1Label.color = _fontColorRegular;
			}
		}

		protected override void Show()
		{
			if (_duoPromotion != null)
			{
				SetupLabelsAndBackgrounds();
			}
			base.Show();
		}

		private void SetupTop()
		{
			_popupTitle.text = Strings.Get(_duoPromotion.TitleId);
			_popupTitle.color = _duoPromotion.TitleTextColor;
			_popupSubTitle.text = Strings.Get(_duoPromotion.SubtitleId);
			_popupSubTitle.color = _duoPromotion.SubtitleTextColor;
			_labelBanner.text = Strings.Get(_duoPromotion.BanneTextid);
			_labelBanner.effectColor = ColorData.instance.shadow;
			_popupStickerTexture.mainTexture = _duoPromotion.StickerTexture;
			_timer.StartTimer(_duoPromotion.TimeLeft);
			_timerLayout.Align();
		}

		private void SetupCharacter(DuoPromotionEntry entry, UITexture characterTexture, TexturePositioningData characterPositioningData)
		{
			RenderTextureObject renderTextureObject = new RenderTextureObject();
			renderTextureObject.animationClip = entry.CharacterAnimation;
			renderTextureObject.position = characterPositioningData.position;
			renderTextureObject.rotation = characterPositioningData.rotation;
			renderTextureObject.scale = characterPositioningData.scale;
			renderTextureObject.prefab = UIModelController.Instance.ModelPrefab;
			RenderTextureObject item = renderTextureObject;
			CharacterRenderTextureSetup characterRenderTextureSetup = new CharacterRenderTextureSetup();
			characterRenderTextureSetup.Setup(entry.CharacterId, entry.CharacterThemeIndex, characterTexture, new List<RenderTextureObject> { item }, new CameraSettings
			{
				backgroundColor = new Color(1f, 1f, 1f, 0f)
			});
		}

		private void SetupBoard(DuoPromotionEntry entry, UITexture boardTexture, TexturePositioningData boardPositioningData)
		{
			Boards.Board board = Boards.boardData[entry.BoardId];
			string path = "Prefabs/HoverboardAssets/" + board.boardModelName;
			RenderTextureObject renderTextureObject = new RenderTextureObject();
			renderTextureObject.position = boardPositioningData.position;
			renderTextureObject.rotation = boardPositioningData.rotation;
			renderTextureObject.scale = boardPositioningData.scale;
			renderTextureObject.prefab = Resources.Load(path) as GameObject;
			renderTextureObject.animationDelay = UnityEngine.Random.Range(0f, 1f);
			renderTextureObject.animationClip = entry.BoardAnimation;
			RenderTextureObject item = renderTextureObject;
			BoardRenderTextureSetup boardRenderTextureSetup = new BoardRenderTextureSetup();
			boardRenderTextureSetup.Setup(false, boardTexture, new List<RenderTextureObject> { item }, new CameraSettings
			{
				backgroundColor = new Color(1f, 1f, 1f, 0f)
			});
		}

		private void SetupButton()
		{
			_buttonText.text = ((!InAppData.inAppData.ContainsKey(CURRENT_PROMOTION)) ? Strings.Get("BUY_INAPP_NOPRICE_DEFAULT") : InAppData.inAppData[CURRENT_PROMOTION].localizedPrice);
		}

		[UsedImplicitly]
		public void OnCloseClicked()
		{
			UIScreenController.Instance.CloseActivePopups(screenName);
		}

		[UsedImplicitly]
		public void PurchaseBundle()
		{
			_isCharacter1Owned = CharacterManager.IsCharacterOwned(_duoPromotion.Entry1.CharacterId);
			_isCharacter2Owned = CharacterManager.IsCharacterOwned(_duoPromotion.Entry2.CharacterId);
			_isBoard1Owned = BoardManager.IsBoardOwned(_duoPromotion.Entry1.BoardId);
			_isBoard2Owned = BoardManager.IsBoardOwned(_duoPromotion.Entry2.BoardId);
			InAppManager.instance.StartPurchase(CURRENT_PROMOTION);
		}

		private void OnPurchaseSuccess(string productId, bool wasRestored, IPurchaseValidationResult validationResult)
		{
			if (productId == CURRENT_PROMOTION && !wasRestored)
			{
				if (!_isCharacter1Owned)
				{
					CelebrationReward celebrationReward = new CelebrationReward();
					celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.CharacterUnlock;
					celebrationReward.rewardType = CelebrationRewardType.character;
					celebrationReward.characterId = _duoPromotion.Entry1.CharacterId;
					celebrationReward.characterOutfitIndex = 0;
					RewardManager.AddRewardToUnlock(celebrationReward);
				}
				if (!_isCharacter2Owned)
				{
					CelebrationReward celebrationReward2 = new CelebrationReward();
					celebrationReward2.CelebrationRewardOrigin = CelebrationRewardOrigin.CharacterUnlock;
					celebrationReward2.rewardType = CelebrationRewardType.character;
					celebrationReward2.characterId = _duoPromotion.Entry2.CharacterId;
					celebrationReward2.characterOutfitIndex = 0;
					RewardManager.AddRewardToUnlock(celebrationReward2);
				}
				if (!_isBoard1Owned)
				{
					CelebrationReward celebrationReward3 = new CelebrationReward();
					celebrationReward3.CelebrationRewardOrigin = CelebrationRewardOrigin.BoardUnlock;
					celebrationReward3.rewardType = CelebrationRewardType.specialBoard;
					celebrationReward3.boardId = _duoPromotion.Entry1.BoardId;
					celebrationReward3.boardPower = Hoverboard.BoardAbility.None;
					RewardManager.AddRewardToUnlock(celebrationReward3);
				}
				if (!_isBoard2Owned)
				{
					CelebrationReward celebrationReward4 = new CelebrationReward();
					celebrationReward4.CelebrationRewardOrigin = CelebrationRewardOrigin.BoardUnlock;
					celebrationReward4.rewardType = CelebrationRewardType.specialBoard;
					celebrationReward4.boardId = _duoPromotion.Entry2.BoardId;
					celebrationReward4.boardPower = Hoverboard.BoardAbility.None;
					RewardManager.AddRewardToUnlock(celebrationReward4);
				}
				OnCloseClicked();
				RewardManager.canShowMultipleQueuedCelebrations = true;
				UIScreenController.Instance.PushCelebrationPopup();
			}
		}

		private void OnItemRestored(string result)
		{
			OnCloseClicked();
		}
	}
}
