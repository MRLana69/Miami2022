using System;
using System.Collections.Generic;
using Data.Managers;
using Data.Save;
using Info;
using JetBrains.Annotations;
using Kiloo;
using Kiloo.Common.Extensions;
using SYBO.Subway.Info;
using UI.Generic;
using UI.Generic.Layout;
using UI.RenderTexture;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace UI.Screens.StoreScreenInternal
{
	public class StoreEventFillerContainerSmall : MonoBehaviour
	{
		[Header("Background")]
		[SerializeField]
		private UISprite _fill;

		[Header("Content")]
		[SerializeField]
		private UILabel _titleText;

		[SerializeField]
		private UILabel _subtitleText;

		[SerializeField]
		private UITexture _renderTexture;

		[SerializeField]
		private AnimationClip _tokenAnimation;

		[SerializeField]
		private UISprite _icon;

		[SerializeField]
		private AnimationClip _mysteryBoxAnimation;

		[SerializeField]
		private UISprite _subIconEventIcon;

		[Header("SubContent")]
		[SerializeField]
		private RenderTextureObject _starObject;

		[Header("BuyButton")]
		[SerializeField]
		private UIButtonOverlayOff _buyButton;

		[SerializeField]
		private Layout _buyButtonLayout;

		[SerializeField]
		private UILabel _buyButtonLabel;

		[SerializeField]
		private UISprite _buyButtonPriceIcon;

		[SerializeField]
		private UISprite _buyButtonPurchasedIcon;

		[Header("Timer")]
		[SerializeField]
		private AlignTimer _alignTimer;

		[Header("Items")]
		[SerializeField]
		private RenderTextureObject _jakeTokenPrefab;

		[SerializeField]
		private RenderTextureObject _trickyTokenPrefab;

		[SerializeField]
		private RenderTextureObject _freshTokenPrefab;

		[SerializeField]
		private RenderTextureObject _spikeTokenPrefab;

		[SerializeField]
		private RenderTextureObject _yutaniTokenPrefab;

		private DefaultRenderTextureSetup _tokenModelRenderTexture;

		private TimeSpan _timeLeft;

		private UnscaledAnimation _mystBoxAnimator;

		private bool _initiated;

		private void Start()
		{
			Setup();
		}

		private void Update()
		{
			if (_initiated)
			{
				_timeLeft = UserSave.currentStoreFillerDeal.currentStoreFillerDealStartTime.AddDays(1.0) - CDateTime.Now;
				if (_timeLeft.TotalMilliseconds < 0.0)
				{
					ResetCurrentFillerDeal();
					UpdateContent();
				}
				else if (!UserSave.currentStoreFillerDeal.currentStoreFillerDealPurchased && UserSave.currentStoreFillerDeal.fillerDealType == IngameShopItems.IngameShopItemType.fillercharactertokens && !CharacterManager.IsTokenUseful(UserSave.currentStoreFillerDeal.fillerDealTokenType))
				{
					ResetCurrentFillerDeal();
					UpdateContent();
				}
				_alignTimer.SetTimeRemaining(_timeLeft);
			}
		}

		private void Setup()
		{
			_fill.gradientTop = NGUIMath.HexToColor(12295679u);
			_fill.gradientBottom = NGUIMath.HexToColor(2515913471u);
			_titleText.color = NGUIMath.HexToColor(40329983u);
			_titleText.effectColor = ColorData.instance.lightShadow;
			_subtitleText.color = NGUIMath.HexToColor(40329983u);
			_subtitleText.effectColor = Color.white;
			_timeLeft = UserSave.currentStoreFillerDeal.currentStoreFillerDealStartTime.AddDays(1.0) - CDateTime.Now;
			_alignTimer.SetTimerColor(ColorData.instance.mahogany);
			if (_timeLeft.TotalMilliseconds < 0.0)
			{
				ResetCurrentFillerDeal();
				UpdateContent();
			}
			else
			{
				UpdateContent();
			}
			_initiated = true;
		}

		private void OnEnable()
		{
			if (_initiated)
			{
				UpdateBuyButton();
			}
		}

		private void UpdateContent()
		{
			if (_tokenModelRenderTexture == null)
			{
				_tokenModelRenderTexture = new DefaultRenderTextureSetup();
			}
			_buyButtonPriceIcon.gameObject.SetActive(false);
			_buyButtonPurchasedIcon.gameObject.SetActive(false);
			_icon.gameObject.SetActive(false);
			_subIconEventIcon.gameObject.SetActive(false);
			UpdateBuyButton();
			if (UserSave.currentStoreFillerDeal.fillerDealType == IngameShopItems.IngameShopItemType.fillercharactertokens)
			{
				Characters.Model model = Characters.characterData[UserSave.currentStoreFillerDeal.fillerDealTokenType];
				RenderTextureObject characterTokenModelPrefab = GetCharacterTokenModelPrefab(UserSave.currentStoreFillerDeal.fillerDealTokenType);
				characterTokenModelPrefab.animationClip = _tokenAnimation;
				_tokenModelRenderTexture.Setup(_renderTexture, new List<RenderTextureObject> { characterTokenModelPrefab, _starObject }, new CameraSettings
				{
					backgroundColor = new Color(1f, 1f, 1f, 0f)
				});
				_titleText.text = Strings.Get(model.name);
				_subtitleText.text = Strings.Get("NUMBER_TOKENS").Format(IngameShopItems.instance.GetItem(UserSave.currentStoreFillerDeal.fillerDealType).payoutAmounts[UserSave.currentStoreFillerDeal.fillerDealIndex]);
				return;
			}
			_icon.gameObject.SetActive(true);
			if (_mystBoxAnimator == null && _mysteryBoxAnimation != null)
			{
				_mystBoxAnimator = _icon.AddComponent<UnscaledAnimation>();
				_mystBoxAnimator.loop = true;
				_mystBoxAnimator.playAutomatically = true;
				_mystBoxAnimator.clip = _mysteryBoxAnimation;
				_mystBoxAnimator.StartAnimating();
			}
			_subIconEventIcon.gameObject.SetActive(SSEventsManagerController.instance.GetActiveEventManager() != null && SSEventsManagerController.instance.GetActiveEventManager().GetEventType() == SSEventType.BiggerJackpot);
			_tokenModelRenderTexture.Setup(_renderTexture, new List<RenderTextureObject> { _starObject }, new CameraSettings
			{
				backgroundColor = new Color(1f, 1f, 1f, 0f)
			});
			_titleText.text = Strings.Get("DAILY_CHALLENGE_SUPER_MYSTERY_BOX");
			_subtitleText.text = Strings.Get("NUMBER_BOXES").Format(IngameShopItems.instance.GetItem(UserSave.currentStoreFillerDeal.fillerDealType).payoutAmounts[UserSave.currentStoreFillerDeal.fillerDealIndex]);
		}

		private void UpdateBuyButton()
		{
			if (UserSave.currentStoreFillerDeal.currentStoreFillerDealPurchased || (UserSave.currentStoreFillerDeal.fillerDealType == IngameShopItems.IngameShopItemType.fillercharactertokens && !CharacterManager.IsTokenUseful(UserSave.currentStoreFillerDeal.fillerDealTokenType)))
			{
				_buyButton.GoToNotAvailableState();
				_buyButtonLabel.text = Strings.Get("STORE_CELL_PURCHASED");
				_buyButtonPriceIcon.gameObject.SetActive(false);
				_buyButtonPurchasedIcon.gameObject.SetActive(true);
				_buyButtonLayout.Align(true);
			}
			else
			{
				_buyButton.GoToNormalState();
				_buyButtonLabel.text = IngameShopItems.instance.GetItem(UserSave.currentStoreFillerDeal.fillerDealType).prices[UserSave.currentStoreFillerDeal.fillerDealIndex] + string.Empty;
				_buyButtonPriceIcon.gameObject.SetActive(IngameShopItems.instance.GetItem(UserSave.currentStoreFillerDeal.fillerDealType).priceType == PurchaseData.UnlockType.Keys);
				_buyButtonPurchasedIcon.gameObject.SetActive(false);
				_buyButtonLayout.Align(true);
			}
		}

		private void ResetCurrentFillerDeal()
		{
			UserSave.currentStoreFillerDeal.currentStoreFillerDealStartTime = CDateTime.Today;
			UserSave.currentStoreFillerDeal.currentStoreFillerDealPurchased = false;
			List<Characters.CharacterId> list = new List<Characters.CharacterId>();
			if (MysteryBox.AreTokensAvailable())
			{
				list.AddRange(MysteryBox.TOKEN_REWARD_IDS);
				list.RemoveAll((Characters.CharacterId x) => CharacterManager.AmountOfUsefulTokens(x) < IngameShopItems.instance.GetItem(IngameShopItems.IngameShopItemType.fillercharactertokens).payoutAmounts[0]);
				if (list.Count > 1 && list.Contains(UserSave.currentStoreFillerDeal.fillerDealTokenType))
				{
					list.Remove(UserSave.currentStoreFillerDeal.fillerDealTokenType);
				}
			}
			if (list.Count > 0)
			{
				UserSave.currentStoreFillerDeal.fillerDealTokenType = list.GetRandomElement();
				UserSave.currentStoreFillerDeal.fillerDealType = IngameShopItems.IngameShopItemType.fillercharactertokens;
				UserSave.currentStoreFillerDeal.fillerDealIndex = 0;
			}
			else
			{
				UserSave.currentStoreFillerDeal.fillerDealType = IngameShopItems.IngameShopItemType.fillersupermysterybox;
				UserSave.currentStoreFillerDeal.fillerDealIndex = UnityEngine.Random.Range(0, 2);
			}
		}

		private RenderTextureObject GetCharacterTokenModelPrefab(Characters.CharacterId charID)
		{
			switch (charID)
			{
			case Characters.CharacterId.slick:
				return _jakeTokenPrefab;
			case Characters.CharacterId.tricky:
				return _trickyTokenPrefab;
			case Characters.CharacterId.fresh:
				return _freshTokenPrefab;
			case Characters.CharacterId.spike:
				return _spikeTokenPrefab;
			case Characters.CharacterId.yutani:
				return _yutaniTokenPrefab;
			default:
				SDebug.LogError(string.Concat("Icon for character ", charID, " is not implemented"));
				return null;
			}
		}

		[UsedImplicitly]
		public void OnClick()
		{
			if (!UserSave.currentStoreFillerDeal.currentStoreFillerDealPurchased)
			{
				if (UserSave.currentStoreFillerDeal.fillerDealType == IngameShopItems.IngameShopItemType.fillercharactertokens)
				{
					int amount = IngameShopItems.instance.GetItem(IngameShopItems.IngameShopItemType.fillercharactertokens).payoutAmounts[0];
					PurchaseHandler.Instance.PurchaseShopItem(IngameShopItems.IngameShopItemType.fillercharactertokens, amount, 0, (List<long> instanceIds) =>
					{
						UserSave.currentStoreFillerDeal.currentStoreFillerDealPurchased = true;
						UpdateBuyButton();
						RewardManager.AddRewardToUnlock(new CelebrationReward
						{
							characterId = UserSave.currentStoreFillerDeal.fillerDealTokenType,
							amount = amount,
							CelebrationRewardOrigin = CelebrationRewardOrigin.UnlockItem,
							unlockItemSource = UnlockItemSource.FillerSale,
							syboAnalyticsSource = ObtainableItemSource.IngameCurrencyPurchase,
							rewardType = CelebrationRewardType.token
						});
						UIScreenController.Instance.PushCelebrationPopup();
					}, () =>
					{
					});
					return;
				}
				int amount2 = IngameShopItems.instance.GetItem(IngameShopItems.IngameShopItemType.fillersupermysterybox).payoutAmounts[UserSave.currentStoreFillerDeal.fillerDealIndex];
				PurchaseHandler.Instance.PurchaseShopItem(IngameShopItems.IngameShopItemType.fillersupermysterybox, amount2, UserSave.currentStoreFillerDeal.fillerDealIndex, (List<long> instanceIds) =>
				{
					UserSave.currentStoreFillerDeal.currentStoreFillerDealPurchased = true;
					UpdateContent();
					for (int i = 0; i < amount2; i++)
					{
						CelebrationReward celebrationReward = MysteryBox.Roll(MysteryBox.Type.Super);
						celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.SuperMysteryBox;
						celebrationReward.unlockItemSource = UnlockItemSource.FillerSale;
						celebrationReward.syboAnalyticsSource = ObtainableItemSource.IngameCurrencyPurchase;
						RewardManager.SetNextRewardUid(instanceIds[i]);
						RewardManager.AddRewardToUnlock(celebrationReward);
					}
					RewardManager.canShowMultipleQueuedCelebrations = true;
					UIScreenController.Instance.PushCelebrationPopup();
				}, () =>
				{
				});
			}
			else
			{
				SDebug.LogError("Shouldn't be able to purchase currently");
			}
		}
	}
}
