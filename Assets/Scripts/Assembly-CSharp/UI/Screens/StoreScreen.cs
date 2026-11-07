using System;
using System.Collections.Generic;
using System.Linq;
using Data.Managers;
using Data.Save;
using InApp;
using Info;
using Kiloo;
using Kiloo.Common.Backend.PurchaseValidation;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using Kiloo.Common.Utility;
using SYBO.Subway.Info;
using SYBO.Subway.Promotion;
using UI.Generic.Layout;
using UI.Helpers.WelcomePack;
using UI.Screens.RewardManagement;
using UI.Screens.StoreScreenInternal;
using UI.ShopInternal;
using UnityEngine;
using Utilities;

namespace UI.Screens
{
	public class StoreScreen : UIScreen
	{
		protected const string ONLINE_SETTINGS_KEY_WEEKEND_SALE_NAME = "discount_deal_name";

		protected const string ONLINE_SETTINGS_KEY_WEEKEND_SALE_END_TIME = "discount_end_time";

		private const int SEGMENT_SPACING_HEIGHT = 13;

		private const int ELEMENT_SPACING_HEIGHT = 8;

		[SerializeField]
		private UIScrollView _scrollView;

		[SerializeField]
		private Layout _contentLayout;

		[SerializeField]
		private UITexture _coloredSpacing;

		[SerializeField]
		private ShopHeader _shopHeaderPrefab;

		private ShopHeader _shopHeaderInstance;

		[SerializeField]
		private SegmentDivider _segmentDividerPrefab;

		private SegmentDivider.SegmentDividerPayload _firstSegmentDivider;

		[SerializeField]
		private VerticalLayout _limitedDealsContainerLayoutPrefab;

		[SerializeField]
		private TableLayout _smallContainerLayoutPrefab;

		[SerializeField]
		private StoreContainerSmall _purchaseContainerPrefab;

		[SerializeField]
		private StoreEventContainerSmall _storeEventContainerSmallPrefab;

		[SerializeField]
		private StoreEventContainerSmall _storeEventContainerBigPrefab;

		[SerializeField]
		private StoreBundleContainer _storeBundleContainerPrefab;

		[SerializeField]
		private StoreEventFillerContainerSmall _smallTokenSaleEventPrefab;

		[SerializeField]
		private WelcomePackBannerHelper _welcomePackCellPrefab;

		[SerializeField]
		private WeekendSaleBanner _weekendSaleBanner;

		private WeekendSaleBanner _weekendSaleBannerInstance;

		[SerializeField]
		private DoubleCoinsCell _doubleCoinsPrefab;

		[SerializeField]
		private StarterPack _starterPackPrefab;

		[SerializeField]
		private GameObject _restorePrefab;

		[SerializeField]
		private string _promotionFolder = "Promotions";

		[SerializeField]
		private string _eventSaleFolder = "EventSales";

		private List<PromotionDefinition> _promotions;

		private List<StoreEventContainerPayload> _eventSales;

		private bool _dailyGiftCurrentlyAvailable = true;

		private static bool _refreshOnNextShow;

		private bool _subscribedToEvents;

		private bool _firstPurchaseBonusBannerShown;

		private bool _safeAreaHandled;

		private bool _tokensAvailablePreviously = true;

		private bool _freeTokenBoxAvailablePreviously = true;

		private bool _doubleCoinsOwnedPreviously;

		private bool _firstPurchaseCharacterOwnedPreviously;

		private TimeSpan _tempTimeLeft;

		private static StoreInterestPoint _interestPoint;

		private Transform _mysteryBoxObject;

		private Transform _saleObject;

		private Transform _limitedDealsObject;

		private SpringPanel sp;

		private static bool _scrollToInterestPoint;

		private float _scrollviewTopPosition;

		private bool refreshScreenPos = true;

		private List<string> _currentScreenContent = new List<string>();

		public static void RefreshOnNextShow()
		{
			_refreshOnNextShow = true;
		}

		public static void ScrollToInterestPointOnNextShow(StoreInterestPoint newPoint)
		{
			_interestPoint = newPoint;
			_scrollToInterestPoint = true;
		}

		public override string GetFlurryName()
		{
			return "CoinsUI_shop";
		}

		protected override void Init()
		{
			_freeTokenBoxAvailablePreviously = !UserSave.hasUserRecievedFreeTokenMysteryBox;
			_tokensAvailablePreviously = MysteryBox.AreTokensAvailable();
			_doubleCoinsOwnedPreviously = UserSave.hasDoubleCoinsUpgrade;
			_firstPurchaseCharacterOwnedPreviously = !FirstPurchaseBonusManager.isFirstPurchaseBonusActive;
			_promotions = (from p in Resources.LoadAll<PromotionDefinition>(_promotionFolder)
				orderby p.CellAppearancePayload.cellPriority
				select p).ToList();
			_eventSales = (from p in Resources.LoadAll<StoreEventContainerPayload>(_eventSaleFolder)
				orderby p.cellAppearancePayload.cellPriority
				select p).ToList();
			FillTable();
			HandleSafeArea();
		}

		protected override void Show()
		{
			SubscribeToEvents();
			if (StoreABManager.StoreNeedsRefresh(StoreABManager.StoreOrigin.Store))
			{
				RefreshOnNextShow();
			}
			if (_footerHandler != null)
			{
				_footerHandler.HandleStickers();
			}
			if (_refreshOnNextShow)
			{
				_refreshOnNextShow = false;
				RefreshContent();
			}
			else if (ScreenContentChanged())
			{
				RefreshContent();
			}
			if (_scrollToInterestPoint)
			{
				Timer.ExecuteNextFrame(ScrollToInterestPoint);
				_scrollToInterestPoint = false;
			}
		}

		private bool ScreenContentChanged()
		{
			if (_tokensAvailablePreviously != MysteryBox.AreTokensAvailable() || _doubleCoinsOwnedPreviously != UserSave.hasDoubleCoinsUpgrade || _freeTokenBoxAvailablePreviously != !UserSave.hasUserRecievedFreeTokenMysteryBox || _firstPurchaseCharacterOwnedPreviously != !FirstPurchaseBonusManager.isFirstPurchaseBonusActive)
			{
				_freeTokenBoxAvailablePreviously = !UserSave.hasUserRecievedFreeTokenMysteryBox;
				_tokensAvailablePreviously = MysteryBox.AreTokensAvailable();
				_doubleCoinsOwnedPreviously = UserSave.hasDoubleCoinsUpgrade;
				_firstPurchaseCharacterOwnedPreviously = !FirstPurchaseBonusManager.isFirstPurchaseBonusActive;
				return true;
			}
			return !_currentScreenContent.SequenceEqual(CreateContentList());
		}

		protected override void Hide()
		{
			UnsubscribeFromEvents();
		}

		private void HandleSafeArea()
		{
			if (!_safeAreaHandled)
			{
				UIPanel panel = _scrollView.panel;
				if (SafeArea.IsRestricted)
				{
					panel.topAnchor.absolute += -SafeArea.OFFSET_TOP;
					panel.bottomAnchor.absolute += SafeArea.OFFSET_BOTTOM;
					panel.ResetAndUpdateAnchors();
				}
				panel.ResetAndUpdateAnchors();
				_safeAreaHandled = true;
			}
		}

		public override void GainFocus()
		{
			if (_refreshOnNextShow)
			{
				_refreshOnNextShow = false;
				RefreshContent();
			}
		}

		private void SubscribeToEvents()
		{
			if (!_subscribedToEvents)
			{
				OnlineSettings.instance.AddOnFinishedDownloadingOnlineSettingsHandler(OnOnlineSettingsFinishedDownloading);
				FirstPurchaseBonusManager.onFirstPurchaseBonusBecameInactive += RefreshContent;
				if (InAppManager.instance != null)
				{
					InAppManager.instance.onProductRequestDone += OnProductRequestDone;
					InAppManager.instance.onPurchaseSuccess += OnPurchaseSuccess;
				}
				_subscribedToEvents = true;
			}
		}

		private void UnsubscribeFromEvents()
		{
			if (_subscribedToEvents)
			{
				OnlineSettings.instance.RemoveOnFinishedDownloadingOnlineSettingsHandler(OnOnlineSettingsFinishedDownloading);
				FirstPurchaseBonusManager.onFirstPurchaseBonusBecameInactive -= RefreshContent;
				if (InAppManager.instance != null)
				{
					InAppManager.instance.onProductRequestDone -= OnProductRequestDone;
					InAppManager.instance.onPurchaseSuccess -= OnPurchaseSuccess;
				}
				_subscribedToEvents = false;
			}
		}

		private void CreateLimitedDeals()
		{
			if (!(WelcomePackManager.welcomePackTimeLeft.TotalSeconds > 0.0) && !_promotions.Exists((PromotionDefinition x) => x.IsEnabled && x.TimeLeft.TotalSeconds > 0.0) && !_eventSales.Exists((StoreEventContainerPayload x) => x.enabled && x.timeLeft.TotalSeconds > 0.0 && x.cellAppearancePayload.canBeWide) && _eventSales.Count((StoreEventContainerPayload x) => x.enabled && x.timeLeft.TotalSeconds > 0.0) <= 2)
			{
				return;
			}
			CreateSegmentDivider(new SegmentDivider.SegmentDividerPayload
			{
				headerText = Strings.Get("STORE_HEADER_LIMITED_DEALS"),
				backgroundColor = ColorData.instance.sun,
				textColor = ColorData.instance.mahogany,
				textShadowColor = ColorData.instance.lightShadow
			});
			VerticalLayout verticalLayout = UnityEngine.Object.Instantiate(_limitedDealsContainerLayoutPrefab, _contentLayout.transform);
			bool flag = true;
			bool flag2 = true;
			int num = 3;
			if (WelcomePackManager.welcomePackTimeLeft.TotalSeconds > 0.0)
			{
				UnityEngine.Object.Instantiate(_welcomePackCellPrefab, verticalLayout.transform).Init();
				flag = false;
				UIWidget uIWidget = verticalLayout.gameObject.AddWidget<UIWidget>();
				uIWidget.height = 8;
				uIWidget.width = 100;
			}
			TableLayout tableLayout = null;
			UIWidget uIWidget2 = null;
			if (tableLayout == null && !flag)
			{
				tableLayout = UnityEngine.Object.Instantiate(_smallContainerLayoutPrefab, verticalLayout.transform);
				uIWidget2 = verticalLayout.gameObject.AddWidget<UIWidget>();
				uIWidget2.height = 8;
				uIWidget2.width = 100;
			}
			foreach (PromotionDefinition promotion in _promotions)
			{
				if (!flag && num <= 0 && !flag2)
				{
					break;
				}
				if (promotion.StartTime > CDateTime.Now || promotion.TimeLeft.TotalSeconds <= 0.0 || !promotion.IsEnabled)
				{
					continue;
				}
				if (tableLayout == null && !flag)
				{
					tableLayout = UnityEngine.Object.Instantiate(_smallContainerLayoutPrefab, verticalLayout.transform);
					uIWidget2 = verticalLayout.gameObject.AddWidget<UIWidget>();
					uIWidget2.height = 8;
					uIWidget2.width = 100;
				}
				StoreEventContainerPayload storeEventContainerPayload = ScriptableObject.CreateInstance<StoreEventContainerPayload>();
				storeEventContainerPayload.enabled = promotion.IsEnabled;
				storeEventContainerPayload.cellAppearancePayload = promotion.CellAppearancePayload;
				storeEventContainerPayload.cellAppearancePayload.popupID = promotion.CellAppearancePayload.popupID;
				StoreEventContainerPayload storeEventContainerPayload2 = storeEventContainerPayload;
				storeEventContainerPayload2.onContainerUpdate = (Action<StoreEventContainerSmall>)Delegate.Combine(storeEventContainerPayload2.onContainerUpdate, (Action<StoreEventContainerSmall>)((StoreEventContainerSmall cell) =>
				{
					cell.SetTimeRemaining(promotion.TimeLeft);
				}));
				if (promotion is DuoPromotion)
				{
					if (!(storeEventContainerPayload != null))
					{
						continue;
					}
					if (tableLayout == null && !flag)
					{
						tableLayout = UnityEngine.Object.Instantiate(_smallContainerLayoutPrefab, verticalLayout.transform);
						uIWidget2 = verticalLayout.gameObject.AddWidget<UIWidget>();
						uIWidget2.height = 8;
						uIWidget2.width = 100;
					}
					if (flag && promotion.CellAppearancePayload.canBeWide)
					{
						StoreBundleContainer container = UnityEngine.Object.Instantiate(_storeBundleContainerPrefab, verticalLayout.transform);
						container.Setup(storeEventContainerPayload);
						StoreBundleContainer storeBundleContainer = container;
						storeBundleContainer.onContainerUpdate = (Action<StoreBundleContainer>)Delegate.Combine(storeBundleContainer.onContainerUpdate, (Action<StoreBundleContainer>)((StoreBundleContainer cell) =>
						{
							container.SetTimeRemaining(promotion.TimeLeft);
						}));
						_limitedDealsObject = container.transform;
						flag = false;
						UIWidget uIWidget3 = verticalLayout.gameObject.AddWidget<UIWidget>();
						uIWidget3.height = 8;
						uIWidget3.width = 100;
					}
					else if (flag2 && promotion.CellAppearancePayload.canBeWide)
					{
						StoreBundleContainer container2 = UnityEngine.Object.Instantiate(_storeBundleContainerPrefab, verticalLayout.transform);
						container2.Setup(storeEventContainerPayload);
						StoreBundleContainer storeBundleContainer2 = container2;
						storeBundleContainer2.onContainerUpdate = (Action<StoreBundleContainer>)Delegate.Combine(storeBundleContainer2.onContainerUpdate, (Action<StoreBundleContainer>)((StoreBundleContainer cell) =>
						{
							container2.SetTimeRemaining(promotion.TimeLeft);
						}));
						_limitedDealsObject = container2.transform;
						flag2 = false;
					}
					continue;
				}
				if (promotion is CharacterPromotion)
				{
					CharacterPromotion characterPromotion = (CharacterPromotion)promotion;
					storeEventContainerPayload.useCharacter = true;
					storeEventContainerPayload.character = new CharacterRenderTextureSetup.EventModelCharacter
					{
						animationClip = characterPromotion.CharacterAnimations[0],
						charID = characterPromotion.CharacterId,
						charOutfitIndex = characterPromotion.ThemeIndex,
						position = characterPromotion.CellModelPosition,
						rotation = characterPromotion.CellModelRotation,
						scale = characterPromotion.CellModelScale,
						useWideCellSettings = characterPromotion.useWideCellSettings,
						secondaryPosition = characterPromotion.cellModelSecondaryPosition,
						secondaryRotation = characterPromotion.cellModelSecondaryRotation,
						secondaryScale = characterPromotion.cellModelSecondaryScale
					};
				}
				else if (promotion is BoardPromotion)
				{
					BoardPromotion boardPromotion = (BoardPromotion)promotion;
					storeEventContainerPayload.useCharacter = false;
					storeEventContainerPayload.board = new BoardRenderTextureSetup.EventModelBoard
					{
						animationClip = boardPromotion.CellModelAnimationClip,
						boardID = boardPromotion.BoardId,
						position = boardPromotion.CellModelPosition,
						rotation = boardPromotion.CellModelRotation,
						scale = boardPromotion.CellModelScale,
						useWideCellSettings = boardPromotion.useWideCellSettings,
						secondaryPosition = boardPromotion.cellModelSecondaryPosition,
						secondaryRotation = boardPromotion.cellModelSecondaryRotation,
						secondaryScale = boardPromotion.cellModelSecondaryScale
					};
				}
				else
				{
					if (!(promotion is BoardBundlePromotion))
					{
						if (promotion is MultiBoardPromotion)
						{
							MultiBoardPromotion multiBoardPromotion = (MultiBoardPromotion)promotion;
							foreach (StoreEventContainerPayload shopEntry in multiBoardPromotion.ShopEntries)
							{
								storeEventContainerPayload = shopEntry;
								StoreEventContainerPayload storeEventContainerPayload3 = storeEventContainerPayload;
								storeEventContainerPayload3.onContainerUpdate = (Action<StoreEventContainerSmall>)Delegate.Combine(storeEventContainerPayload3.onContainerUpdate, (Action<StoreEventContainerSmall>)((StoreEventContainerSmall cell) =>
								{
									cell.SetTimeRemaining(promotion.TimeLeft);
								}));
								if (storeEventContainerPayload != null)
								{
									if (tableLayout == null && !flag)
									{
										tableLayout = UnityEngine.Object.Instantiate(_smallContainerLayoutPrefab, verticalLayout.transform);
										uIWidget2 = verticalLayout.gameObject.AddWidget<UIWidget>();
										uIWidget2.height = 8;
										uIWidget2.width = 100;
									}
									if (flag && promotion.CellAppearancePayload.canBeWide)
									{
										UnityEngine.Object.Instantiate(_storeEventContainerBigPrefab, verticalLayout.transform).Setup(storeEventContainerPayload);
										flag = false;
										UIWidget uIWidget4 = verticalLayout.gameObject.AddWidget<UIWidget>();
										uIWidget4.height = 8;
										uIWidget4.width = 100;
									}
									else if (flag2 && promotion.CellAppearancePayload.canBeWide)
									{
										UnityEngine.Object.Instantiate(_storeEventContainerBigPrefab, verticalLayout.transform).Setup(storeEventContainerPayload);
										flag2 = false;
									}
									else
									{
										UnityEngine.Object.Instantiate(_storeEventContainerSmallPrefab, tableLayout.transform).Setup(storeEventContainerPayload);
										num--;
									}
								}
							}
						}
						else
						{
							LogError(string.Concat("Promotion of type: ", promotion.GetType(), " is not supported"));
						}
						continue;
					}
					if (promotion.CellAppearancePayload.buttonAction != StoreEventContainerSmall.EventCellButtonAction.OpenPopup)
					{
						LogError("A Board Bundle Promotion should always use ButtonAction OpenPopup. The promotion will not be shown");
						continue;
					}
					BoardBundlePromotion boardBundlePromotion = (BoardBundlePromotion)promotion;
					storeEventContainerPayload.useCharacter = false;
					storeEventContainerPayload.isBoardBundle = true;
					storeEventContainerPayload.board = new BoardRenderTextureSetup.EventModelBoard
					{
						animationClip = boardBundlePromotion.CellModelAnimationClip,
						boardID = boardBundlePromotion.BoardId,
						position = boardBundlePromotion.CellModelPosition,
						rotation = boardBundlePromotion.CellModelRotation,
						scale = boardBundlePromotion.CellModelScale,
						useWideCellSettings = boardBundlePromotion.useWideCellSettings,
						secondaryPosition = boardBundlePromotion.cellModelSecondaryPosition,
						secondaryRotation = boardBundlePromotion.cellModelSecondaryRotation,
						secondaryScale = boardBundlePromotion.cellModelSecondaryScale
					};
				}
				if (storeEventContainerPayload != null)
				{
					if (flag && promotion.CellAppearancePayload.canBeWide)
					{
						StoreEventContainerSmall storeEventContainerSmall = UnityEngine.Object.Instantiate(_storeEventContainerBigPrefab, verticalLayout.transform);
						storeEventContainerSmall.Setup(storeEventContainerPayload);
						_limitedDealsObject = storeEventContainerSmall.transform;
						flag = false;
						UIWidget uIWidget5 = verticalLayout.gameObject.AddWidget<UIWidget>();
						uIWidget5.height = 8;
						uIWidget5.width = 100;
					}
					else if (flag2 && promotion.CellAppearancePayload.canBeWide)
					{
						StoreEventContainerSmall storeEventContainerSmall2 = UnityEngine.Object.Instantiate(_storeEventContainerBigPrefab, verticalLayout.transform);
						storeEventContainerSmall2.Setup(storeEventContainerPayload);
						_limitedDealsObject = storeEventContainerSmall2.transform;
						flag2 = false;
					}
					else
					{
						UnityEngine.Object.Instantiate(_storeEventContainerSmallPrefab, tableLayout.transform).Setup(storeEventContainerPayload);
						num--;
					}
				}
			}
			foreach (StoreEventContainerPayload shopEvent in _eventSales)
			{
				if (!flag && num <= 0 && !flag2)
				{
					break;
				}
				if (shopEvent.startTime > CDateTime.Now || shopEvent.timeLeft.TotalSeconds <= 0.0 || !shopEvent.enabled)
				{
					continue;
				}
				if (tableLayout == null && !flag)
				{
					tableLayout = UnityEngine.Object.Instantiate(_smallContainerLayoutPrefab, verticalLayout.transform);
					uIWidget2 = verticalLayout.gameObject.AddWidget<UIWidget>();
					uIWidget2.height = 8;
					uIWidget2.width = 100;
				}
				if (shopEvent.useCharacter)
				{
					StoreEventContainerPayload storeEventContainerPayload4 = shopEvent;
					storeEventContainerPayload4.onContainerUpdate = (Action<StoreEventContainerSmall>)Delegate.Combine(storeEventContainerPayload4.onContainerUpdate, (Action<StoreEventContainerSmall>)((StoreEventContainerSmall cell) =>
					{
						cell.SetTimeRemaining(CharacterManager.CharacterExpirationTimespan(shopEvent.character.charID));
					}));
				}
				else
				{
					StoreEventContainerPayload storeEventContainerPayload5 = shopEvent;
					storeEventContainerPayload5.onContainerUpdate = (Action<StoreEventContainerSmall>)Delegate.Combine(storeEventContainerPayload5.onContainerUpdate, (Action<StoreEventContainerSmall>)((StoreEventContainerSmall cell) =>
					{
						cell.SetTimeRemaining(BoardManager.BoardExpirationTimespan(shopEvent.board.boardID));
					}));
				}
				if (!(shopEvent != null))
				{
					continue;
				}
				if (flag && shopEvent.cellAppearancePayload.canBeWide)
				{
					UnityEngine.Object.Instantiate(_storeEventContainerBigPrefab, verticalLayout.transform).Setup(shopEvent);
					flag = false;
					UIWidget uIWidget6 = verticalLayout.gameObject.AddWidget<UIWidget>();
					uIWidget6.height = 8;
					uIWidget6.width = 100;
					continue;
				}
				if (flag2 && shopEvent.cellAppearancePayload.canBeWide)
				{
					UnityEngine.Object.Instantiate(_storeEventContainerBigPrefab, verticalLayout.transform).Setup(shopEvent);
					flag2 = false;
					continue;
				}
				if (tableLayout == null)
				{
					LogError("containerLayout not created. Somthing is not set up correctly");
					break;
				}
				UnityEngine.Object.Instantiate(_storeEventContainerSmallPrefab, tableLayout.transform).Setup(shopEvent);
				num--;
			}
			if (flag && flag2 && num > 0)
			{
				UnityEngine.Object.DestroyImmediate(verticalLayout.gameObject);
			}
			else if (tableLayout != null)
			{
				if (num > 0)
				{
					UnityEngine.Object.Instantiate(_smallTokenSaleEventPrefab, tableLayout.transform);
					num--;
				}
				if (num > 0)
				{
					UnityEngine.Object.DestroyImmediate(tableLayout.gameObject);
					UnityEngine.Object.DestroyImmediate(uIWidget2.gameObject);
				}
				else
				{
					tableLayout.Align();
				}
			}
			CreateSpacer(13);
			verticalLayout.Align();
		}

		private void CreateSpecialDeals()
		{
			CreateSegmentDivider(new SegmentDivider.SegmentDividerPayload
			{
				headerText = Strings.Get("STORE_HEADER_SPECIAL_DEALS"),
				backgroundColor = ColorData.instance.crimson,
				textColor = ColorData.instance.white,
				textShadowColor = ColorData.instance.shadow
			});
			CreateSpacer(13);
			CreateSpecialDealCells();
			if (UserSave.lastPurchasedBundle != InAppData.BundleTier.BundleTier5)
			{
				UnityEngine.Object.Instantiate(_starterPackPrefab, _contentLayout.transform).Init();
				CreateSpacer(13);
			}
		}

		private void CreateSpecialDealCells()
		{
			TableLayout tableLayout = UnityEngine.Object.Instantiate(_smallContainerLayoutPrefab, _contentLayout.transform);
			UpgradeType[] specialDealBoxTypes = Upgrades.specialDealBoxTypes;
			for (int i = 0; i < specialDealBoxTypes.Length; i++)
			{
				string priceTypeIconName = string.Empty;
				Action onButtonClicked = null;
				Action onInfoButtonClicked = null;
				Action<StoreContainerSmall> onContainerShouldRefresh = null;
				bool clickable = true;
				bool showEventSubIcon = false;
				bool showcheckedSubIcon = false;
				bool showStars = false;
				bool showTimerIcon = false;
				string iconNameMenu = Upgrades.upgrades[specialDealBoxTypes[i]].iconNameMenu;
				string valueText = Strings.Get(Upgrades.upgrades[specialDealBoxTypes[i]].name);
				string priceText = Upgrades.upgrades[specialDealBoxTypes[i]].GetPriceToIncreaseTier(0).ToString();
				Color fillTopColor;
				Color fillBottomColor;
				Color frameTopColor;
				Color frameBottomColor;
				Color valueColor;
				Color valueEffectColor;
				switch (specialDealBoxTypes[i])
				{
				case UpgradeType.dailyGiftMiniMysteryBox:
					fillTopColor = NGUIMath.HexToColor(967118591u);
					fillBottomColor = NGUIMath.HexToColor(2783525631u);
					frameTopColor = NGUIMath.HexToColor(913506559u);
					frameBottomColor = NGUIMath.HexToColor(1621892351u);
					valueColor = NGUIMath.HexToColor(23396863u);
					valueEffectColor = ColorData.instance.white;
					priceText = Strings.Get("STORE_DAILY_GIFT_BUTTON_TEXT");
					if (UserSave.dailyGiftLastGiven.Date < CDateTime.Now.Date)
					{
						valueText = Strings.Get("STORE_DAILY_GIFT_TITLE");
						priceText = Strings.Get("STORE_DAILY_GIFT_BUTTON_TEXT");
						clickable = true;
						showStars = true;
					}
					else
					{
						valueText = Strings.Get("STORE_DAILY_GIFT_UNAVAILABLE");
						priceText = string.Empty;
						clickable = false;
						showStars = false;
						showTimerIcon = true;
					}
					onContainerShouldRefresh = (StoreContainerSmall containerInstance) =>
					{
						if (!_dailyGiftCurrentlyAvailable && UserSave.dailyGiftLastGiven.Date < CDateTime.Now.Date)
						{
							_dailyGiftCurrentlyAvailable = true;
							if (_footerHandler != null)
							{
								_footerHandler.HandleStickers();
							}
							containerInstance.payload.valueText = Strings.Get("STORE_DAILY_GIFT_TITLE");
							containerInstance.payload.priceText = Strings.Get("STORE_DAILY_GIFT_BUTTON_TEXT");
							containerInstance.payload.priceTypeIconName = string.Empty;
							containerInstance.payload.showTimerIcon = false;
							containerInstance.payload.clickable = true;
							containerInstance.payload.showcheckedSubIcon = false;
							containerInstance.payload.showStars = true;
							containerInstance.Setup(containerInstance.payload);
						}
						else if (_dailyGiftCurrentlyAvailable && UserSave.dailyGiftLastGiven.Date >= CDateTime.Now.Date)
						{
							_dailyGiftCurrentlyAvailable = false;
							if (_footerHandler != null)
							{
								_footerHandler.HandleStickers();
							}
							containerInstance.payload.valueText = Strings.Get("STORE_DAILY_GIFT_UNAVAILABLE");
							containerInstance.payload.priceTypeIconName = string.Empty;
							containerInstance.payload.showTimerIcon = true;
							containerInstance.payload.clickable = false;
							containerInstance.payload.showcheckedSubIcon = true;
							containerInstance.payload.showStars = false;
							containerInstance.Setup(containerInstance.payload);
						}
						if (!_dailyGiftCurrentlyAvailable)
						{
							if ((UserSave.dailyGiftLastGiven.Date.AddDays(1.0) - CDateTime.Now).TotalSeconds <= 0.0)
							{
								RefreshOnNextShow();
							}
							containerInstance.SetButtonTimeLeft(UserSave.dailyGiftLastGiven.Date.AddDays(1.0) - CDateTime.Now);
						}
					};
					onButtonClicked = () =>
					{
						if (UserSave.dailyGiftLastGiven.Date < CDateTime.Now.Date)
						{
							UserSave.dailyGiftLastGiven = CDateTime.UtcNow + CDateTime.Now.GetCurrentTimeZoneUTCOffset();
							RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.MysteryBoxMini, ObtainableItemSource.FreeUnlock);
							UIScreenController.Instance.PushCelebrationPopup();
							Flurry.LogEvent("Daily Gift Collected");
						}
					};
					onInfoButtonClicked = () =>
					{
						UIScreenController.Instance.PushPopup("PrizeMMBPopup");
					};
					break;
				case UpgradeType.mysterybox:
					fillTopColor = NGUIMath.HexToColor(795667711u);
					fillBottomColor = NGUIMath.HexToColor(2093809663u);
					frameTopColor = NGUIMath.HexToColor(154958847u);
					frameBottomColor = NGUIMath.HexToColor(964539647u);
					valueColor = NGUIMath.HexToColor(5143295u);
					valueEffectColor = ColorData.instance.white;
					priceTypeIconName = "icon_coin_large";
					showEventSubIcon = SSEventsManagerController.instance.GetActiveEventManager() != null && SSEventsManagerController.instance.GetActiveEventManager().GetEventType() == SSEventType.BiggerJackpot;
					onButtonClicked = () =>
					{
						PurchaseHandler.Instance.PurchaseUpgrade(UpgradeType.mysterybox, () =>
						{
							RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.MysteryBox, ObtainableItemSource.IngameCurrencyPurchase);
							UIScreenController.Instance.PushCelebrationPopup();
							Flurry.LogEvent("Shop MysteryBox purchased");
						}, () =>
						{
						});
					};
					onInfoButtonClicked = () =>
					{
						UIScreenController.Instance.PushPopup("PrizeMBPopup");
					};
					break;
				case UpgradeType.tokenmysterybox:
					fillTopColor = NGUIMath.HexToColor(12295679u);
					fillBottomColor = NGUIMath.HexToColor(2515913471u);
					frameTopColor = NGUIMath.HexToColor(92634623u);
					frameBottomColor = NGUIMath.HexToColor(297382911u);
					valueColor = NGUIMath.HexToColor(40329983u);
					valueEffectColor = ColorData.instance.white;
					priceTypeIconName = "icon_key";
					if (!MysteryBox.AreTokensAvailable())
					{
						clickable = false;
						priceText = Strings.Get("TOKEN_BOX_FINISHED");
						priceTypeIconName = string.Empty;
						showcheckedSubIcon = true;
					}
					else if (!UserSave.hasUserRecievedFreeTokenMysteryBox)
					{
						priceText = Strings.Get("FREE");
						priceTypeIconName = string.Empty;
					}
					onButtonClicked = () =>
					{
						if (!UserSave.hasUserRecievedFreeTokenMysteryBox)
						{
							_freeTokenBoxAvailablePreviously = false;
							refreshScreenPos = false;
							Timer.ExecuteNextFrame(RefreshContent);
						}
						PurchaseHandler.Instance.PurchaseUpgrade(UpgradeType.tokenmysterybox, () =>
						{
							RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.TokenMysteryBox, ObtainableItemSource.IngameCurrencyPurchase);
							UIScreenController.Instance.PushCelebrationPopup();
							Flurry.LogEventWithAParameter("Character Token Box Opened", "ScreenName", screenName);
							if (!MysteryBox.AreTokensAvailable())
							{
								RefreshOnNextShow();
							}
						}, () =>
						{
						});
					};
					onInfoButtonClicked = () =>
					{
						UIScreenController.Instance.PushPopup("TokenMysteryBoxPrizesPopup");
					};
					break;
				default:
					fillTopColor = NGUIMath.HexToColor(795667711u);
					fillBottomColor = NGUIMath.HexToColor(2093809663u);
					frameTopColor = NGUIMath.HexToColor(154958847u);
					frameBottomColor = NGUIMath.HexToColor(964539647u);
					valueColor = NGUIMath.HexToColor(5143295u);
					valueEffectColor = ColorData.instance.lightShadow;
					break;
				}
				StoreContainerSmall storeContainerSmall = UnityEngine.Object.Instantiate(_purchaseContainerPrefab, tableLayout.transform);
				if (specialDealBoxTypes[i] == UpgradeType.dailyGiftMiniMysteryBox)
				{
					_mysteryBoxObject = storeContainerSmall.transform;
				}
				storeContainerSmall.Setup(new StoreContainerSmall.StoreContainerSmallPayload
				{
					frameTopColor = frameTopColor,
					frameBottomColor = frameBottomColor,
					fillTopColor = fillTopColor,
					fillBottomColor = fillBottomColor,
					iconName = iconNameMenu,
					valueText = valueText,
					valueColor = valueColor,
					valueEffectColor = valueEffectColor,
					priceText = priceText,
					priceTypeIconName = priceTypeIconName,
					showEventSubIcon = showEventSubIcon,
					showcheckedSubIcon = showcheckedSubIcon,
					clickable = clickable,
					showStars = showStars,
					showTimerIcon = showTimerIcon,
					onButtonClicked = onButtonClicked,
					onInfoButtonClicked = onInfoButtonClicked,
					onContainerShouldRefresh = onContainerShouldRefresh
				});
			}
			CreateSpacer(8);
			tableLayout.Align(true);
		}

		private void CreatePurchases()
		{
			List<InAppItemModel> list = InAppManager.GetActiveInAppOffers().Values.ToList();
			CreateSegmentDivider(new SegmentDivider.SegmentDividerPayload
			{
				headerText = Strings.Get("STORE_HEADER_COINS"),
				backgroundColor = ColorData.instance.storm,
				textColor = ColorData.instance.white,
				textShadowColor = ColorData.instance.shadow
			});
			CreateSpacer(13);
			UnityEngine.Object.Instantiate(_doubleCoinsPrefab, _contentLayout.transform).Init();
			CreateSpacer(8);
			bool setSaleObject = false;
			if (InAppManager.GetActiveInAppOffers().ToList().Exists((KeyValuePair<string, InAppItemModel> x) => x.Value.itemOfferType == InAppItemModel.OfferType.Promotion && x.Value.itemType == InAppItemModel.ItemType.Coins))
			{
				CreateWeekendSaleBanner(WeekendSaleBanner.WeekendSaleType.COINS);
				setSaleObject = true;
			}
			List<InAppItemModel> list2 = list.FindAll((InAppItemModel x) => x.itemType == InAppItemModel.ItemType.Coins);
			StoreABManager.VerifyCoinList(list2);
			CreatePurchaseEntries(list2, setSaleObject);
			setSaleObject = false;
			CreateSpacer(13);
			CreateSegmentDivider(new SegmentDivider.SegmentDividerPayload
			{
				headerText = Strings.Get("STORE_HEADER_KEYS"),
				backgroundColor = ColorData.instance.storm,
				textColor = ColorData.instance.white,
				textShadowColor = ColorData.instance.shadow
			});
			CreateSpacer(13);
			if (_weekendSaleBannerInstance == null && InAppManager.GetActiveInAppOffers().ToList().Exists((KeyValuePair<string, InAppItemModel> x) => x.Value.itemOfferType == InAppItemModel.OfferType.Promotion && x.Value.itemType == InAppItemModel.ItemType.Keys))
			{
				CreateWeekendSaleBanner(WeekendSaleBanner.WeekendSaleType.KEYS);
				setSaleObject = true;
			}
			List<InAppItemModel> purchases = list.FindAll((InAppItemModel x) => x.itemType == InAppItemModel.ItemType.Keys);
			CreatePurchaseEntries(purchases, setSaleObject);
			CreateSpacer(8);
		}

		private void CreateHeader()
		{
			_shopHeaderInstance = UnityEngine.Object.Instantiate(_shopHeaderPrefab, _contentLayout.transform);
		}

		private void CreatePurchaseEntries(List<InAppItemModel> purchases, bool setSaleObject)
		{
			TableLayout tableLayout = UnityEngine.Object.Instantiate(_smallContainerLayoutPrefab, _contentLayout.transform);
			if (setSaleObject)
			{
				_saleObject = tableLayout.transform;
			}
			foreach (InAppItemModel purchase in purchases)
			{
				StoreContainerSmall storeContainerSmall = UnityEngine.Object.Instantiate(_purchaseContainerPrefab, tableLayout.transform);
				storeContainerSmall.Setup(ShopCellManager.GetCellPayloadForOffer(purchase));
			}
			tableLayout.Align(true);
		}

		private void CreateWeekendSaleBanner(WeekendSaleBanner.WeekendSaleType type)
		{
			if (OnlineSettings.instance.GetValue("discount_deal_name", null) != null)
			{
				double value = OnlineSettings.instance.GetValue("discount_end_time", 0);
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(value);
				if (dateTime > CDateTime.UtcNow)
				{
					_weekendSaleBannerInstance = UnityEngine.Object.Instantiate(_weekendSaleBanner, _contentLayout.transform);
					_weekendSaleBannerInstance.Init(dateTime, type);
					CreateSpacer(8);
				}
			}
		}

		private void CreateSegmentDivider(SegmentDivider.SegmentDividerPayload payload)
		{
			if (_firstSegmentDivider == null)
			{
				_firstSegmentDivider = payload;
				return;
			}
			SegmentDivider segmentDivider = UnityEngine.Object.Instantiate(_segmentDividerPrefab, _contentLayout.transform);
			segmentDivider.Setup(payload);
		}

		private void CreateSpacer(int height)
		{
			UIWidget uIWidget = _contentLayout.gameObject.AddWidget<UIWidget>();
			uIWidget.height = height;
			uIWidget.width = 100;
		}

		private void RefreshContent()
		{
			_refreshOnNextShow = false;
			Vector3 currentTransform = _scrollView.transform.localPosition;
			DestroyContent();
			FillTable();
			_currentScreenContent = CreateContentList();
			if (!refreshScreenPos)
			{
				Timer.ExecuteNextFrame(() =>
				{
					Vector3 localPosition = _scrollView.transform.localPosition;
					_scrollView.transform.localPosition = currentTransform;
					Vector3 vector = currentTransform - localPosition;
					Vector2 clipOffset = _scrollView.panel.clipOffset;
					clipOffset.x -= vector.x;
					clipOffset.y -= vector.y;
					_scrollView.panel.clipOffset = clipOffset;
				});
				refreshScreenPos = true;
			}
		}

		private void DestroyContent()
		{
			foreach (Transform item in _contentLayout.transform)
			{
				item.gameObject.SetActive(false);
				UnityEngine.Object.Destroy(item.gameObject);
			}
			_shopHeaderInstance = null;
			_firstSegmentDivider = null;
			_weekendSaleBannerInstance = null;
			_contentLayout.Align(true);
			_scrollView.ResetPosition();
		}

		private void FillTable()
		{
			CreateHeader();
			CreateLimitedDeals();
			CreateSpecialDeals();
			CreatePurchases();
			UnityEngine.Object.Instantiate(_restorePrefab, _contentLayout.transform);
			CreateSpacer(13);
			_firstPurchaseBonusBannerShown = FirstPurchaseBonusManager.isFirstPurchaseBonusActive;
			_firstPurchaseCharacterOwnedPreviously = !_firstPurchaseBonusBannerShown;
			_shopHeaderInstance.Setup(new ShopHeader.ShopHeaderPayload
			{
				headerText = Strings.Get("STORE_TITLE"),
				headerCharacter = Characters.CharacterId.tricky,
				segmentDividerPayload = _firstSegmentDivider,
				showBoomBot = FirstPurchaseBonusManager.isFirstPurchaseBonusActive
			});
			_contentLayout.Align(true);
			Timer.ExecuteNextFrame(() =>
			{
				_scrollView.ResetPosition();
				_scrollviewTopPosition = _scrollView.transform.localPosition.y;
			});
		}

		private void ScrollToInterestPoint()
		{
			switch (_interestPoint)
			{
			case StoreInterestPoint.MysteryBox:
				if (_mysteryBoxObject != null)
				{
					CenterOnTransform(_mysteryBoxObject);
				}
				break;
			case StoreInterestPoint.Sale:
				if (_saleObject != null)
				{
					CenterOnTransform(_saleObject);
				}
				break;
			case StoreInterestPoint.LimitedDeals:
				if (_limitedDealsObject != null)
				{
					CenterOnTransform(_limitedDealsObject);
				}
				break;
			}
			if (sp != null)
			{
				Timer.ExecuteNextFrame(() =>
				{
					sp.enabled = true;
				});
			}
		}

		private List<string> CreateContentList()
		{
			List<string> list = new List<string>();
			list.AddRange(from x in _promotions.FindAll((PromotionDefinition promotion) => promotion.StartTime > CDateTime.Now || promotion.TimeLeft.TotalSeconds <= 0.0 || !promotion.IsEnabled)
				select x.name);
			list.AddRange(from x in _eventSales.FindAll((StoreEventContainerPayload promotion) => promotion.startTime > CDateTime.Now || promotion.timeLeft.TotalSeconds <= 0.0 || !promotion.enabled)
				select x.name);
			list.Add(UserSave.lastPurchasedBundle.ToString());
			List<InAppItemModel> list2 = InAppManager.GetActiveInAppOffers().Values.ToList();
			list.AddRange(from x in list2.FindAll((InAppItemModel x) => x.itemType == InAppItemModel.ItemType.Coins)
				select x.itemId);
			list.AddRange(from x in list2.FindAll((InAppItemModel x) => x.itemType == InAppItemModel.ItemType.Keys)
				select x.itemId);
			return list;
		}

		private void CenterOnTransform(Transform target)
		{
			Vector4 finalClipRegion = _scrollView.panel.finalClipRegion;
			Transform cachedTransform = _scrollView.panel.cachedTransform;
			Vector3 localPosition = cachedTransform.localPosition;
			localPosition.x += finalClipRegion.x;
			localPosition.y += finalClipRegion.y;
			localPosition = cachedTransform.parent.TransformPoint(localPosition);
			_scrollView.currentMomentum = Vector3.zero;
			Vector3 vector = cachedTransform.InverseTransformPoint(target.position);
			Vector3 vector2 = cachedTransform.InverseTransformPoint(localPosition);
			Vector3 vector3 = vector - vector2;
			if (!_scrollView.canMoveHorizontally)
			{
				vector3.x = 0f;
			}
			if (!_scrollView.canMoveVertically)
			{
				vector3.y = 0f;
			}
			vector3.z = 0f;
			Vector3 pos = cachedTransform.localPosition - vector3;
			if (pos.y < _scrollviewTopPosition)
			{
				pos.y = _scrollviewTopPosition;
			}
			sp = SpringPanel.Begin(_scrollView.gameObject, pos, 6f);
		}

		private void OnProductRequestDone()
		{
			refreshScreenPos = false;
			RefreshContent();
		}

		private void OnPurchaseSuccess(string productId, bool wasRestored, IPurchaseValidationResult validationResult)
		{
			if (productId.IsOneOf("com.kiloo.subwaysurfers.welcomepacktier01", "com.kiloo.subwaysurfers.welcomepacktier02"))
			{
				_refreshOnNextShow = true;
			}
			else if (_firstPurchaseBonusBannerShown)
			{
				refreshScreenPos = false;
				RefreshContent();
			}
		}

		private void OnOnlineSettingsFinishedDownloading()
		{
			RefreshOnNextShow();
		}

		private void LogError(string message)
		{
			SDebug.LogError("[Shop Screen] " + message);
		}
	}
}
