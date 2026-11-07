using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Data.Managers;
using Data.Save;
using Info;
using Kiloo.Common.Extensions;
using Kiloo.Common.Timer;
using SYBO.Subway.Info;
using UI.FreeStuff;
using UI.Generic.Layout;
using UI.Screens.FreeStuffScreenInternal;
using UI.ShopInternal;
using UnityEngine;
using Utilities;

namespace UI.Screens
{
	public class FreeStuffScreen : UIScreen
	{
		[SerializeField]
		private UIScrollView _scrollView;

		[SerializeField]
		private FreeStuffContainerBig _bigContainerPrefab;

		[SerializeField]
		private FreeStuffContainerSmall _smallContainerPrefab;

		[SerializeField]
		private FreeStuffSocialContainer _socialContainerPrefab;

		[SerializeField]
		private CoinEarnerVideoAds _dailyVideoPrefab;

		[SerializeField]
		private ShopHeader _shopHeaderPrefab;

		[SerializeField]
		private Layout _contentLayout;

		[SerializeField]
		private SegmentDivider _segmentDividerPrefab;

		[SerializeField]
		private Layout _smallContainerLayout;

		private const int SEGMENT_SPACING_HEIGHT = 16;

		private const int ELEMENT_SPACING_HEIGHT = 10;

		private List<Transform> _screenContent;

		private Layout _smallLayoutContainer;

		private bool _socialDeviderCreated;

		private bool _linkSpacerCreated;

		private bool _coinoffersShown;

		private bool _coinsForAdShowing;

		private bool _hoverBoardForAdShowing;

		private bool _headstartForAdShowing;

		private bool _scoreboosterForAdShowing;

		public static bool updateScreen = true;

		public static bool updateTvIcons;

		public static List<FreeStuffContainerSmall> tvIconsSmall;

		public static List<FreeStuffContainerBig> tvIconsBig;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action onGainFocus__BackingField;

		private bool isVideoAdsSupported
		{
			get
			{
				return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Globals.DEBUG_SKIP_VIDEO_ADS;
			}
		}

		public static event Action onGainFocus
		{
			add
			{
				Action action = onGainFocus__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onGainFocus__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = onGainFocus__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onGainFocus__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		protected override void Init()
		{
			HandleSafeArea();
			SocialManager.instance.AddFacebookStateChangedHandler(SetUpdateScreen);
			tvIconsSmall = new List<FreeStuffContainerSmall>();
			tvIconsBig = new List<FreeStuffContainerBig>();
			_screenContent = new List<Transform>();
			updateScreen = true;
			updateTvIcons = false;
		}

		private void SetUpdateScreen()
		{
			updateScreen = true;
		}

		private void HandleSafeArea()
		{
			UIPanel panel = _scrollView.panel;
			if (SafeArea.IsRestricted)
			{
				panel.topAnchor.absolute += -SafeArea.OFFSET_TOP;
				panel.bottomAnchor.absolute += SafeArea.OFFSET_BOTTOM;
			}
			panel.ResetAndUpdateAnchors();
		}

		protected override void Show()
		{
			OfferWallManager.instance.onOfferWallReceived += OfferWallRequestSuccess;
			Flurry.LogEventWithAParameter("Earn coins screen opened", "State", AdSystemManager.IsCached(AdLocations.EARN_COINS_VIDEOS).ToString());
			CheckForScreenUpdate();
			RefreshContent();
			onGainFocus__BackingField.SafeInvoke();
		}

		protected override void Hide()
		{
			OfferWallManager.instance.onOfferWallReceived -= OfferWallRequestSuccess;
		}

		public void Update()
		{
			if (updateScreen)
			{
				RefreshContent();
			}
			if (!updateTvIcons)
			{
				return;
			}
			updateTvIcons = false;
			foreach (FreeStuffContainerSmall item in tvIconsSmall)
			{
				item.RefreshVideoLogo();
			}
			foreach (FreeStuffContainerBig item2 in tvIconsBig)
			{
				item2.RefreshVideoLogo();
			}
		}

		private void OnApplicationFocus(bool focus)
		{
			if (focus)
			{
				CheckForScreenUpdate();
			}
		}

		private void CheckForScreenUpdate()
		{
			if (!AgeRestriction.isRestricted && OfferWallManager.instance.IsOfferWallAllowed() && OfferWallManager.instance.IsOfferWallAvailable())
			{
				if (!_coinoffersShown)
				{
					updateScreen = true;
				}
			}
			else if (_coinoffersShown)
			{
				updateScreen = true;
			}
			if (RewardForVideoButtonHelper.coinsForVideo.isAvailable.SafeInvoke() && !_coinsForAdShowing)
			{
				updateScreen = true;
			}
			else if (!RewardForVideoButtonHelper.coinsForVideo.isAvailable.SafeInvoke() && _coinsForAdShowing)
			{
				_coinsForAdShowing = false;
				updateScreen = true;
			}
			if (RewardForVideoButtonHelper.hoverboardForVideo.isAvailable.SafeInvoke() && !_hoverBoardForAdShowing)
			{
				updateScreen = true;
			}
			else if (!RewardForVideoButtonHelper.hoverboardForVideo.isAvailable.SafeInvoke() && _hoverBoardForAdShowing)
			{
				_hoverBoardForAdShowing = false;
				updateScreen = true;
			}
			if (RewardForVideoButtonHelper.headstartForVideo.isAvailable.SafeInvoke() && !_headstartForAdShowing)
			{
				updateScreen = true;
			}
			else if (!RewardForVideoButtonHelper.headstartForVideo.isAvailable.SafeInvoke() && _headstartForAdShowing)
			{
				_headstartForAdShowing = false;
				updateScreen = true;
			}
			if (RewardForVideoButtonHelper.scoreBoosterForVideo.isAvailable.SafeInvoke() && !_scoreboosterForAdShowing)
			{
				updateScreen = true;
			}
			else if (!RewardForVideoButtonHelper.scoreBoosterForVideo.isAvailable.SafeInvoke() && _scoreboosterForAdShowing)
			{
				_scoreboosterForAdShowing = false;
				updateScreen = true;
			}
		}

		public override void GainFocus()
		{
			base.GainFocus();
			CheckForScreenUpdate();
			RefreshContent();
		}

		private void OfferWallRequestSuccess()
		{
			RefreshContent();
		}

		private void RefreshContent()
		{
			if (_footerHandler != null)
			{
				_footerHandler.HandleStickers();
			}
			if (updateScreen)
			{
				updateScreen = false;
				tvIconsBig.Clear();
				tvIconsSmall.Clear();
				DeleteContent();
				CreateHeader();
				CreateContent();
				_contentLayout.Align(true);
				Kiloo.Common.Timer.Timer.ExecuteNextFrame(() =>
				{
					_scrollView.ResetPosition();
				});
			}
		}

		private void DeleteContent()
		{
			foreach (Transform item in _screenContent)
			{
				UnityEngine.Object.DestroyImmediate(item.gameObject);
			}
			_socialDeviderCreated = false;
			_linkSpacerCreated = false;
			_screenContent.Clear();
		}

		private void CreateHeader()
		{
			ShopHeader shopHeader = UnityEngine.Object.Instantiate(_shopHeaderPrefab, _contentLayout.transform);
			shopHeader.Setup(new ShopHeader.ShopHeaderPayload
			{
				headerText = Strings.Get("FREE_STUFF_HEADER"),
				headerCharacter = Characters.CharacterId.king
			});
			_screenContent.Add(shopHeader.transform);
		}

		private void CreateContent()
		{
			CreateDailyVideoContent();
			CreateFacebookContent();
			if (RewardForVideoButtonHelper.coinsForVideo.isAvailable.SafeInvoke())
			{
				_coinsForAdShowing = true;
				CreateBigContainer(FreeStuffContent.coinsForVideo);
			}
			CreateSmallRewardForVideo();
			if (!AgeRestriction.isRestricted && OfferWallManager.instance.IsOfferWallAllowed() && OfferWallManager.instance.IsOfferWallAvailable())
			{
				_coinoffersShown = true;
				CreateBigContainer(FreeStuffContent.coinoffer);
			}
			CreateSocialLinkContent();
			CreateCustomLinkContent();
		}

		private void CreateSmallRewardForVideo()
		{
			if (RewardForVideoButtonHelper.scoreBoosterForVideo.isAvailable.SafeInvoke() || RewardForVideoButtonHelper.hoverboardForVideo.isAvailable.SafeInvoke() || RewardForVideoButtonHelper.headstartForVideo.isAvailable.SafeInvoke())
			{
				Layout layout = CreateSmallLayoutContainer();
				if (RewardForVideoButtonHelper.scoreBoosterForVideo.isAvailable.SafeInvoke())
				{
					_scoreboosterForAdShowing = true;
					CreateSmallContainer(FreeStuffContent.scoreBooster, layout.transform);
				}
				if (RewardForVideoButtonHelper.headstartForVideo.isAvailable.SafeInvoke())
				{
					_headstartForAdShowing = true;
					CreateSmallContainer(FreeStuffContent.headstart, layout.transform);
				}
				if (RewardForVideoButtonHelper.hoverboardForVideo.isAvailable.SafeInvoke())
				{
					_hoverBoardForAdShowing = true;
					CreateSmallContainer(FreeStuffContent.hoverboard, layout.transform);
				}
				layout.Align(true);
				if (layout.transform.childCount > 3)
				{
					CreateSpacer(10);
				}
			}
		}

		private void CreateSegmentDivider(SegmentDivider.SegmentDividerPayload payload)
		{
			SegmentDivider segmentDivider = UnityEngine.Object.Instantiate(_segmentDividerPrefab, _contentLayout.transform);
			segmentDivider.Setup(payload);
			_screenContent.Add(segmentDivider.transform);
		}

		private void CreateSpacer(int height)
		{
			UIWidget uIWidget = _contentLayout.gameObject.AddWidget<UIWidget>();
			uIWidget.height = height;
			uIWidget.width = 100;
			_screenContent.Add(uIWidget.transform);
		}

		private void CreateBigContainer(BigFreeStuffContainerPayload payload)
		{
			FreeStuffContainerBig freeStuffContainerBig = UnityEngine.Object.Instantiate(_bigContainerPrefab, _contentLayout.transform);
			freeStuffContainerBig.Setup(payload);
			_screenContent.Add(freeStuffContainerBig.transform);
			CreateSpacer(10);
		}

		private void CreateSmallContainer(SmallFreeStuffContainerPayload payload, Transform container)
		{
			FreeStuffContainerSmall freeStuffContainerSmall = UnityEngine.Object.Instantiate(_smallContainerPrefab, container);
			freeStuffContainerSmall.Setup(payload);
		}

		private Layout CreateSmallLayoutContainer()
		{
			Layout layout = UnityEngine.Object.Instantiate(_smallContainerLayout, _contentLayout.transform);
			_screenContent.Add(layout.transform);
			return layout;
		}

		private void CreateDailyVideoContent()
		{
			if (isVideoAdsSupported)
			{
				CoinEarnerVideoAds coinEarnerVideoAds = UnityEngine.Object.Instantiate(_dailyVideoPrefab, _contentLayout.transform);
				coinEarnerVideoAds.Init();
				_screenContent.Add(coinEarnerVideoAds.transform);
				CreateSpacer(10);
			}
		}

		private void CreateSocialLinkBar()
		{
			if (!_socialDeviderCreated)
			{
				_socialDeviderCreated = true;
				CreateSegmentDivider(new SegmentDivider.SegmentDividerPayload
				{
					headerText = Strings.Get("FREE_STUFF_SOCIAL_MEDIA"),
					backgroundColor = ColorData.instance.storm,
					textColor = ColorData.instance.white,
					textShadowColor = ColorData.instance.shadow
				});
				CreateSpacer(16);
			}
		}

		private void CreateCustomLinkContent()
		{
			if (AgeRestriction.isRestricted || OnlineSettingsManager.data.socialLinkOverrides == null)
			{
				return;
			}
			List<string> list = OnlineSettingsManager.data.socialLinkOverrides.Keys.Where((string x) => !Enum.GetNames(typeof(EarnCoinsData.Id)).Contains(x)).ToList();
			foreach (string item in list)
			{
				EarnCoinsData.EarnCurrencyProfile earnCurrencyProfile = new EarnCoinsData.EarnCurrencyProfile();
				earnCurrencyProfile.id = EarnCoinsData.Id.CustomLink;
				earnCurrencyProfile.customIdentifier = item;
				EarnCoinsData.EarnCurrencyProfile earnCurrencyProfile2 = earnCurrencyProfile;
				if (!EarnCoinsData.ShouldShowInGUI(earnCurrencyProfile2) || !OnlineSettingsManager.data.socialLinkOverrides.ContainsKey(earnCurrencyProfile2.customIdentifier))
				{
					continue;
				}
				SocialLinkOverrideInfo socialLinkOverrideInfo = OnlineSettingsManager.data.socialLinkOverrides[earnCurrencyProfile2.customIdentifier];
				if (socialLinkOverrideInfo.title == null || socialLinkOverrideInfo.link.IsNullOrEmpty() || socialLinkOverrideInfo.reward == -1)
				{
					continue;
				}
				string linkLocalizedTitle = OnlineSettingsManager.localizedData.getLinkLocalizedTitle(earnCurrencyProfile2.customIdentifier);
				if (linkLocalizedTitle.IsNullOrEmpty())
				{
					continue;
				}
				earnCurrencyProfile2.title = linkLocalizedTitle;
				Uri result;
				if (!Uri.TryCreate(socialLinkOverrideInfo.link, UriKind.Absolute, out result) || !(result.Scheme == Uri.UriSchemeHttps))
				{
					continue;
				}
				earnCurrencyProfile2.url = socialLinkOverrideInfo.link;
				if (socialLinkOverrideInfo.reward <= -1)
				{
					continue;
				}
				earnCurrencyProfile2.defaultAmountOfCoins = socialLinkOverrideInfo.reward;
				if (socialLinkOverrideInfo.buttonString != null && earnCurrencyProfile2.defaultAmountOfCoins <= 0)
				{
					linkLocalizedTitle = OnlineSettingsManager.localizedData.getLinkLocalizedButtonString(earnCurrencyProfile2.customIdentifier);
					if (linkLocalizedTitle.IsNullOrEmpty())
					{
						continue;
					}
					earnCurrencyProfile2.buttonString = linkLocalizedTitle;
				}
				if (_smallLayoutContainer == null)
				{
					CreateSocialLinkBar();
					_smallLayoutContainer = CreateSmallLayoutContainer();
				}
				FreeStuffSocialContainer freeStuffSocialContainer = UnityEngine.Object.Instantiate(_socialContainerPrefab, _smallLayoutContainer.transform);
				freeStuffSocialContainer.Init(earnCurrencyProfile2);
			}
			if (_smallLayoutContainer != null)
			{
				_smallLayoutContainer.Align(true);
				if (!_linkSpacerCreated && _smallLayoutContainer.transform.childCount > 3)
				{
					CreateSpacer(10);
					_linkSpacerCreated = true;
				}
			}
		}

		private void CreateSocialLinkContent()
		{
			EarnCoinsData.EarnCurrencyProfile[] socialProfiles = EarnCoinsData.socialProfiles;
			foreach (EarnCoinsData.EarnCurrencyProfile earnCurrencyProfile in socialProfiles)
			{
				if (!EarnCoinsData.ShouldShowInGUI(earnCurrencyProfile))
				{
					continue;
				}
				if (OnlineSettingsManager.data.socialLinkOverrides != null && OnlineSettingsManager.data.socialLinkOverrides.ContainsKey(earnCurrencyProfile.id.ToString()))
				{
					SocialLinkOverrideInfo socialLinkOverrideInfo = OnlineSettingsManager.data.socialLinkOverrides[earnCurrencyProfile.id.ToString()];
					Uri result;
					if (!socialLinkOverrideInfo.link.IsNullOrEmpty() && Uri.TryCreate(socialLinkOverrideInfo.link, UriKind.Absolute, out result) && result.Scheme == Uri.UriSchemeHttps)
					{
						earnCurrencyProfile.url = socialLinkOverrideInfo.link;
					}
					if (socialLinkOverrideInfo.reward != -1)
					{
						if (socialLinkOverrideInfo.reward < 0)
						{
							continue;
						}
						earnCurrencyProfile.defaultAmountOfCoins = socialLinkOverrideInfo.reward;
					}
					if (socialLinkOverrideInfo.buttonString != null && earnCurrencyProfile.defaultAmountOfCoins <= 0)
					{
						string linkLocalizedButtonString = OnlineSettingsManager.localizedData.getLinkLocalizedButtonString(earnCurrencyProfile.id.ToString());
						if (linkLocalizedButtonString.IsNullOrEmpty())
						{
							continue;
						}
						earnCurrencyProfile.buttonString = linkLocalizedButtonString;
					}
				}
				if (_smallLayoutContainer == null)
				{
					CreateSocialLinkBar();
					_smallLayoutContainer = CreateSmallLayoutContainer();
				}
				FreeStuffSocialContainer freeStuffSocialContainer = UnityEngine.Object.Instantiate(_socialContainerPrefab, _smallLayoutContainer.transform);
				freeStuffSocialContainer.Init(earnCurrencyProfile);
			}
			if (_smallLayoutContainer != null)
			{
				_smallLayoutContainer.Align(true);
				if (!_linkSpacerCreated && _smallLayoutContainer.transform.childCount > 3)
				{
					CreateSpacer(10);
					_linkSpacerCreated = true;
				}
			}
		}

		private void CreateFacebookContent()
		{
			if (!AgeRestriction.isRestricted)
			{
				if (!UserSave.hasPaidOutFacebookReward)
				{
					CreateBigContainer(FreeStuffContent.facebookLogin);
				}
				else if (FacebookWeeklyGiftManager.IsGiftAvailable())
				{
					CreateBigContainer(FreeStuffContent.facebookGift);
				}
			}
		}
	}
}
