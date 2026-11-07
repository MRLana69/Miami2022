using System;
using System.Collections.Generic;
using InApp;
using Info.VideoRewardCooldowns;
using JetBrains.Annotations;
using Kiloo.Common.Extensions;
using SYBO.Subway;
using UI.ButtonHelpers;
using UI.FreeStuff;
using UI.Generic.Layout;
using UI.Helpers;
using UI.Screens;
using UI.Screens.RewardManagement;
using UI.Screens.StoreScreenInternal;
using UnityEngine;

namespace UI.Popups
{
	public class NotEnoughCurrencyPopup : UIPopup
	{
		private enum PopupClosingAction
		{
			None = 0,
			Buy = 1,
			Cancel = 2,
			Ok = 3,
			VideoCoins = 4
		}

		[SerializeField]
		private UIPanel _popupPanel;

		[SerializeField]
		private UILabel _popupTitle;

		[SerializeField]
		private UILabel _popupDescription;

		[SerializeField]
		private GameObject _uiContentCoins;

		[SerializeField]
		private GameObject _uiContentKeys;

		[SerializeField]
		private VerticalLayout _contentVerticalLayout;

		[SerializeField]
		private Transform _dynamicContentRoot;

		[SerializeField]
		private VerticalLayout _dynamicContentVerticalLayout;

		[Header("Prefabs")]
		[SerializeField]
		private TableLayout _smallContainerLayoutPrefab;

		[SerializeField]
		private StoreContainerSmall _purchaseContainerPrefab;

		[SerializeField]
		private BoomBotBanner _firstPurchaseBonusBannerPrefab;

		[SerializeField]
		private OkButtonHelper _okButtonPrefab;

		[SerializeField]
		private BuyMoreInStoreLabelHelper _buyMoreInStoreLabelPrefab;

		[SerializeField]
		private GameObject _emptySpaceAboveDynamicContent;

		[SerializeField]
		private GameObject _emptySpaceBelowDynamicContent;

		[SerializeField]
		private UIWidget _bgOutline;

		[SerializeField]
		private UIWidget _closeButton;

		private PopupClosingAction _closingAction;

		private static InAppManagerPopupData _popupData;

		private Layout _smallContainerLayoutInstance;

		private GameObject _coinsForAdButton;

		private GameObject _buyCurrencyButton;

		private GameObject _okButton;

		private GameObject _buyMoreInStorelabel;

		private BoomBotBanner _firstPurchaseBanner;

		private int _newCoinPrice;

		private const string STANDARD_VIDEO_REWARD = "-1";

		private const string VIDEO_AVAIL_YES = "Yes";

		private const string VIDEO_AVAIL_FAIL = "No_caching_failed";

		private const string VIDEO_AVAIL_NO_TRY = "No_caching_not_attempted";

		private const string BUTTON_NAME_CLOSE = "Closed";

		private const string BUTTON_NAME_IAP = "IAP";

		private const string BUTTON_NAME_VIDEO = "Video";

		private const string BUTTON_NO_COOLDOWN = "No Cooldown";

		private const string BUTTON_COOLDOWN = "Cooldown_{0}_watched";

		private string _flurryVideoAvailable;

		private string _flurryButtonName;

		private string _flurryVideoReward;

		private string _flurryCooldownStep;

		private bool _panelDepthHasBeenSet;

		private VideoButtonPayload _videoButtonPayload;

		private bool isInRun
		{
			get
			{
				return Game.Instance.IsInGame.Value && UIScreenController.Instance.GetTopScreenName() == "IngameUI";
			}
		}

		protected override float screenOffset
		{
			get
			{
				return (!isInRun) ? (-13f) : 0f;
			}
		}

		public override bool ShouldShowTopBar()
		{
			return !isInRun;
		}

		protected override void Show()
		{
			_popupData = InAppManager.instance.GetPopupData();
			RefreshPopupContent();
		}

		protected override void Hide()
		{
			base.Hide();
			if (_closingAction == PopupClosingAction.None)
			{
				_flurryVideoReward = "-1";
				_flurryButtonName = "Closed";
				_flurryCooldownStep = ((CooldownManager.GetChargesUsed("coins") != 0) ? string.Format("Cooldown_{0}_watched", CooldownManager.GetChargesUsed("coins")) : "No Cooldown");
				Payout(PopupClosingAction.Cancel);
			}
			_closingAction = PopupClosingAction.None;
			_popupData = null;
			_newCoinPrice = 0;
		}

		private void ClearDynamicCells()
		{
			_panelDepthHasBeenSet = false;
			foreach (Transform item in _dynamicContentRoot.transform)
			{
				UnityEngine.Object.DestroyImmediate(item.gameObject);
			}
		}

		private void OnPurchaseItemClicked()
		{
			_closingAction = PopupClosingAction.Buy;
			_flurryButtonName = "IAP";
			_flurryVideoReward = "-1";
			_flurryCooldownStep = ((CooldownManager.GetChargesUsed("coins") != 0) ? string.Format("Cooldown_{0}_watched", CooldownManager.GetChargesUsed("coins")) : "No Cooldown");
			Payout(_closingAction);
			ClosePopup();
		}

		private void RefreshPopupContent()
		{
			if (_popupData != null)
			{
				RefreshStaticContent();
				ClearDynamicCells();
				RefreshDynamicContent();
				UpdateAnchors();
				if (!CooldownManager.HasCharge("coins"))
				{
					_flurryVideoAvailable = "No_caching_not_attempted";
				}
				else if (CooldownManager.HasCharge("coins") && !AdSystemManager.IsCached(AdLocations.FREE_COINS_POPUP_VIDEOS))
				{
					_flurryVideoAvailable = "No_caching_failed";
				}
				else
				{
					_flurryVideoAvailable = "Yes";
				}
			}
		}

		private void UpdateAnchors()
		{
			_contentVerticalLayout.Align(true);
			_bgOutline.UpdateAnchors();
			_closeButton.UpdateAnchors();
		}

		private void RefreshStaticContent()
		{
			_popupTitle.text = _popupData.popupTitle;
			_popupTitle.color = ColorData.instance.midnight;
			_popupDescription.text = ((_newCoinPrice != 0) ? string.Format(Strings.Get("NOT_ENOUGH_COINS_POPUP_ADD_ON_COINS"), _newCoinPrice) : _popupData.popupDescription);
			_popupDescription.color = ColorData.instance.peacock;
			if (_popupData.isErrorMessage)
			{
				_uiContentCoins.SetActive(false);
				_uiContentKeys.SetActive(false);
			}
			else if (_popupData.isCoinsPack)
			{
				_uiContentCoins.SetActive(true);
				_uiContentKeys.SetActive(false);
				if (base.parentScreen == "CharacterNameEventPopup")
				{
					Flurry.LogEvent("Attempt to buy coins for Wordy Weekend");
				}
			}
			else
			{
				_uiContentCoins.SetActive(false);
				_uiContentKeys.SetActive(true);
				if (base.parentScreen == "CharacterNameEventPopup")
				{
					Flurry.LogEvent("Attempt to buy keys for the Wordy Weekend");
				}
			}
		}

		private void RefreshDynamicContent()
		{
			ClearDynamicCells();
			if (_popupData.canMakePurchase)
			{
				if (_smallContainerLayoutInstance == null)
				{
					_smallContainerLayoutInstance = UnityEngine.Object.Instantiate(_smallContainerLayoutPrefab, _dynamicContentRoot);
				}
				int num = 3;
				if (_popupData.isCoinsPack && CooldownManager.HasCharge("coins") && !Game.Instance.IsInGame.Value && AdSystemManager.IsCached(RewardForVideoButtonHelper.coinsForVideo.adLocation))
				{
					StoreContainerSmall storeContainerSmall = UnityEngine.Object.Instantiate(_purchaseContainerPrefab, _smallContainerLayoutInstance.transform);
					storeContainerSmall.Setup(new StoreContainerSmall.StoreContainerSmallPayload
					{
						frameTopColor = NGUIMath.HexToColor(358124031u),
						frameBottomColor = NGUIMath.HexToColor(459974143u),
						fillTopColor = NGUIMath.HexToColor(42258943u),
						fillBottomColor = NGUIMath.HexToColor(433581311u),
						iconName = RewardForVideoButtonHelper.coinsForVideo.iconName,
						valueText = Strings.Get(RewardForVideoButtonHelper.coinsForVideo.description),
						valueColor = ColorData.instance.midnight,
						valueEffectColor = ColorData.instance.lightShadow,
						priceText = Strings.Get("STORE_VIDEO_WATCH"),
						showAdIcon = true,
						onButtonClicked = () =>
						{
							AdSystemManager.Show(RewardForVideoButtonHelper.coinsForVideo.adLocation, () =>
							{
								int num3 = RewardForVideoButtonHelper.coinsForVideo.getRewardAmount.SafeInvoke();
								RewardForVideoButtonHelper.coinsForVideo.onVideoWatched();
								CelebrationReward reward = new CelebrationReward
								{
									CelebrationRewardOrigin = CelebrationRewardOrigin.UnlockItem,
									rewardType = RewardForVideoButtonHelper.coinsForVideo.rewardType,
									upgradeType = RewardForVideoButtonHelper.coinsForVideo.upgradeType,
									amount = num3,
									syboAnalyticsSourceMetaOrNull = RewardForVideoButtonHelper.coinsForVideo.unlockItemSource.ToString(),
									unlockItemSource = RewardForVideoButtonHelper.coinsForVideo.unlockItemSource
								};
								RewardManager.AddRewardToUnlock(reward);
								UIScreenController.Instance.PushCelebrationPopup();
								_closingAction = PopupClosingAction.VideoCoins;
								_flurryButtonName = "Video";
								_flurryVideoReward = num3.ToString();
								_flurryCooldownStep = ((CooldownManager.GetChargesUsed("coins") != 0) ? string.Format("Cooldown_{0}_watched", CooldownManager.GetChargesUsed("coins")) : "No Cooldown");
								Payout(_closingAction);
								_newCoinPrice = _popupData.currencyMissing - num3;
								if (_newCoinPrice <= 0)
								{
									_newCoinPrice = 0;
									UIScreenController.Instance.ClosePopup("NotEnoughCurrencyPopup");
								}
								else
								{
									_popupData.currencyMissing = _newCoinPrice;
									RefreshPopupContent();
								}
							});
						}
					});
					num--;
				}
				StoreABManager.VerifyCoinList(_popupData.bestOffers);
				foreach (InAppItemModel bestOffer in _popupData.bestOffers)
				{
					if (num > 0)
					{
						StoreContainerSmall storeContainerSmall2 = UnityEngine.Object.Instantiate(_purchaseContainerPrefab, _smallContainerLayoutInstance.transform);
						StoreContainerSmall.StoreContainerSmallPayload cellPayloadForOffer = ShopCellManager.GetCellPayloadForOffer(bestOffer);
						cellPayloadForOffer.onButtonClicked = (Action)Delegate.Combine(cellPayloadForOffer.onButtonClicked, new Action(OnPurchaseItemClicked));
						storeContainerSmall2.Setup(cellPayloadForOffer);
						num--;
					}
				}
				_smallContainerLayoutInstance.Align();
				if (FirstPurchaseBonusManager.isFirstPurchaseBonusActive)
				{
					if (_firstPurchaseBanner == null)
					{
						_firstPurchaseBanner = UnityEngine.Object.Instantiate(_firstPurchaseBonusBannerPrefab, _dynamicContentRoot);
						_firstPurchaseBanner.Setup();
						_emptySpaceBelowDynamicContent.SetActive(false);
					}
					else
					{
						_firstPurchaseBanner.gameObject.SetActive(true);
						_emptySpaceBelowDynamicContent.SetActive(false);
					}
				}
				else if (_firstPurchaseBanner != null)
				{
					_firstPurchaseBanner.gameObject.SetActive(false);
					_emptySpaceBelowDynamicContent.SetActive(true);
				}
				if (!_panelDepthHasBeenSet)
				{
					_panelDepthHasBeenSet = true;
					UIPanel[] componentsInChildren = _dynamicContentRoot.GetComponentsInChildren<UIPanel>();
					for (int num2 = 0; num2 < componentsInChildren.Length; num2++)
					{
						componentsInChildren[num2].depth += _popupPanel.depth;
					}
				}
				if (_coinsForAdButton != null)
				{
					bool flag = AdSystemManager.IsCached(AdLocations.FREE_COINS_POPUP_VIDEOS);
					if (!Game.Instance.IsInGame.Value && flag && _popupData.isCoinsPack && CooldownManager.HasCharge("coins"))
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string>();
						dictionary.Add("Result", "Fill;popup");
						Flurry.LogEventWithSeveralParameters("Attempt to show not enough coin video button", dictionary);
						_coinsForAdButton.SetActive(true);
					}
					else if (!flag && _popupData.isCoinsPack)
					{
						Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
						dictionary2.Add("Result", "No_Fill;popup");
						Flurry.LogEventWithSeveralParameters("Attempt to show not enough coin video button", dictionary2);
						_coinsForAdButton.SetActive(false);
					}
					else
					{
						_coinsForAdButton.SetActive(false);
					}
				}
			}
			else
			{
				_emptySpaceAboveDynamicContent.SetActive(true);
				UnityEngine.Object.Instantiate(_okButtonPrefab, _dynamicContentRoot).OkButtonClicked += OnOkClicked;
				UnityEngine.Object.Instantiate(_buyMoreInStoreLabelPrefab, _dynamicContentRoot).UpdateBuyMoreInStoreLabel();
			}
			_dynamicContentVerticalLayout.Align(true);
		}

		public override string GetFlurryName()
		{
			return "NotEnoughCurencyPopup";
		}

		[UsedImplicitly]
		public void OnOkClicked()
		{
			_flurryVideoReward = "-1";
			_flurryButtonName = "Closed";
			_flurryCooldownStep = ((CooldownManager.GetChargesUsed("coins") != 0) ? string.Format("Cooldown_{0}_watched", CooldownManager.GetChargesUsed("coins")) : "No Cooldown");
			_closingAction = PopupClosingAction.Ok;
			Payout(_closingAction);
			if (UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.CloseActivePopups(screenName);
			}
		}

		public override void BackButtonPressed()
		{
			base.BackButtonPressed();
			if (Game.Instance.IsInGame.Value)
			{
				Revive.Instance.SendSkipRevive();
			}
		}

		[UsedImplicitly]
		public void OnCancelClicked()
		{
			_flurryVideoReward = "-1";
			_flurryButtonName = "Closed";
			_flurryCooldownStep = ((CooldownManager.GetChargesUsed("coins") != 0) ? string.Format("Cooldown_{0}_watched", CooldownManager.GetChargesUsed("coins")) : "No Cooldown");
			_closingAction = PopupClosingAction.Cancel;
			Payout(_closingAction);
			if (UIScreenController.IsInstanced)
			{
				UIScreenController.Instance.CloseActivePopups(screenName);
			}
		}

		private void Payout(PopupClosingAction action)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Video_Available", _flurryVideoAvailable);
			dictionary.Add("Button_Name", _flurryButtonName);
			dictionary.Add("Video_Reward", _flurryVideoReward);
			dictionary.Add("Cooldown_step", _flurryCooldownStep);
			Flurry.LogEventWithSeveralParameters("NotEnoughCurrencyPopup", dictionary);
			if (_popupData == null)
			{
				return;
			}
			switch (action)
			{
			case PopupClosingAction.Buy:
				if (base.parentScreen == "CharacterNameEventPopup" && !_popupData.isErrorMessage)
				{
					if (_popupData.isCoinsPack)
					{
						Flurry.LogEvent("Bought coins for Wordy Weekend");
					}
					else
					{
						Flurry.LogEvent("Bought keys for Wordy Weekend");
					}
				}
				break;
			case PopupClosingAction.Ok:
				InAppManager.instance.NativePurchaseFail();
				if (base.parentScreen == "CharacterNameEventPopup" && !_popupData.isErrorMessage)
				{
					if (_popupData.isCoinsPack)
					{
						Flurry.LogEvent("Unable to buy coins for Wordy Weekend");
					}
					else
					{
						Flurry.LogEvent("Unable to buy keys for Wordy Weekend");
					}
				}
				break;
			case PopupClosingAction.Cancel:
				InAppManager.instance.NativePurchaseCancelled();
				if (base.parentScreen == "CharacterNameEventPopup" && !_popupData.isErrorMessage)
				{
					if (_popupData.isCoinsPack)
					{
						Flurry.LogEvent("Canceled buying coins for Wordy Weekend");
					}
					else
					{
						Flurry.LogEvent("Canceled buying keys for Wordy Weekend");
					}
				}
				break;
			case PopupClosingAction.VideoCoins:
				break;
			}
		}
	}
}
