using System;
using System.Diagnostics;
using System.Threading;
using Data.Managers;
using Data.Save;
using Kiloo.Common.Plugins.DeviceUtility;
using UI.Screens.RewardManagement;
using UI.VideoAdPrizes;
using UnityEngine;

namespace Info
{
	public static class EarnCoinsData
	{
		public enum Id
		{
			FacebookKiloo = 0,
			FacebookSybo = 1,
			TwitterKiloo = 2,
			TwitterSybo = 3,
			YoutubeKiloo = 4,
			YoutubeSybo = 5,
			Review = 6,
			AdColony = 7,
			VideoAdCycle = 8,
			FacebookLogin = 9,
			FacebookWeeklyGift = 10,
			RewardForVideo = 11,
			CustomLink = 12
		}

		public enum Type
		{
			OpenURL = 0,
			VideoAd = 1,
			ClaimReward = 2
		}

		public enum Repeatability
		{
			Once = 0,
			OncePerVersion = 1,
			Forever = 2
		}

		public class EarnCurrencyProfile
		{
			public Id id;

			public Type type;

			public Repeatability repeatability;

			public int defaultAmountOfCoins;

			public string customIdentifier;

			public string title;

			public string buttonString;

			public string url;

			public int GetAmountOfCoins()
			{
				return defaultAmountOfCoins;
			}
		}

		private const string ONLINESETTINGS_VIDEOADS_DEFAULTREWARD_KEY = "videoads_defaultreward";

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<string, int> OnRewardURLOpened__BackingField;

		public static readonly EarnCurrencyProfile[] socialProfiles = new EarnCurrencyProfile[6]
		{
			new EarnCurrencyProfile
			{
				id = Id.TwitterKiloo,
				type = Type.OpenURL,
				repeatability = Repeatability.Once,
				defaultAmountOfCoins = 300,
				url = "https://mobile.twitter.com/kiloogames"
			},
			new EarnCurrencyProfile
			{
				id = Id.YoutubeKiloo,
				type = Type.OpenURL,
				repeatability = Repeatability.Once,
				defaultAmountOfCoins = 300,
				url = "http://m.youtube.com/kiloomobile"
			},
			new EarnCurrencyProfile
			{
				id = Id.FacebookKiloo,
				type = Type.OpenURL,
				repeatability = Repeatability.Once,
				defaultAmountOfCoins = 300,
				url = "http://m.facebook.com/kiloogames"
			},
			new EarnCurrencyProfile
			{
				id = Id.TwitterSybo,
				type = Type.OpenURL,
				repeatability = Repeatability.Once,
				defaultAmountOfCoins = 300,
				url = "https://mobile.twitter.com/sybogames"
			},
			new EarnCurrencyProfile
			{
				id = Id.YoutubeSybo,
				type = Type.OpenURL,
				repeatability = Repeatability.Once,
				defaultAmountOfCoins = 300,
				url = "http://m.youtube.com/sybogames"
			},
			new EarnCurrencyProfile
			{
				id = Id.FacebookSybo,
				type = Type.OpenURL,
				repeatability = Repeatability.Once,
				defaultAmountOfCoins = 300,
				url = "http://m.facebook.com/SubwaySurfersSybo"
			}
		};

		public static readonly EarnCurrencyProfile facebookGift = new EarnCurrencyProfile
		{
			id = Id.FacebookWeeklyGift,
			type = Type.ClaimReward,
			repeatability = Repeatability.Forever
		};

		public static readonly EarnCurrencyProfile[] profiles = new EarnCurrencyProfile[3]
		{
			new EarnCurrencyProfile
			{
				id = Id.VideoAdCycle,
				type = Type.VideoAd,
				repeatability = Repeatability.Forever
			},
			new EarnCurrencyProfile
			{
				id = Id.FacebookLogin,
				type = Type.OpenURL,
				repeatability = Repeatability.Once,
				defaultAmountOfCoins = 5000,
				url = string.Empty
			},
			new EarnCurrencyProfile
			{
				id = Id.FacebookWeeklyGift,
				type = Type.ClaimReward,
				repeatability = Repeatability.Forever
			}
		};

		private static bool isVideoAddsSupported
		{
			get
			{
				return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor;
			}
		}

		public static event Action<string, int> OnRewardURLOpened
		{
			add
			{
				Action<string, int> action = OnRewardURLOpened__BackingField;
				Action<string, int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnRewardURLOpened__BackingField, (Action<string, int>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<string, int> action = OnRewardURLOpened__BackingField;
				Action<string, int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnRewardURLOpened__BackingField, (Action<string, int>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static EarnCurrencyProfile[] GetOnlineSettingsLinks()
		{
			return new EarnCurrencyProfile[0];
		}

		public static bool ShouldShowInGUI(int profileIndex)
		{
			return ShouldShowInGUI(profiles[profileIndex]);
		}

		public static bool ShouldShowInGUI(EarnCurrencyProfile profile)
		{
			switch (profile.id)
			{
			case Id.FacebookLogin:
				return !UserSave.hasPaidOutFacebookReward;
			case Id.FacebookWeeklyGift:
				return FacebookWeeklyGiftManager.IsGiftAvailable();
			case Id.FacebookKiloo:
			case Id.FacebookSybo:
			case Id.TwitterKiloo:
			case Id.TwitterSybo:
			case Id.YoutubeKiloo:
			case Id.YoutubeSybo:
				if (OnlineSettingsManager.data.socialLinkOverrides != null && OnlineSettingsManager.data.socialLinkOverrides.ContainsKey(profile.id.ToString()) && !OnlineSettingsManager.data.socialLinkOverrides[profile.id.ToString()].enabled)
				{
					return false;
				}
				break;
			case Id.CustomLink:
				if (OnlineSettingsManager.data.socialLinkOverrides != null && OnlineSettingsManager.data.socialLinkOverrides.ContainsKey(profile.customIdentifier) && !OnlineSettingsManager.data.socialLinkOverrides[profile.customIdentifier].enabled)
				{
					return false;
				}
				break;
			}
			if (profile.repeatability == Repeatability.Once || profile.repeatability == Repeatability.OncePerVersion)
			{
				string value;
				if ((profile.id == Id.CustomLink) ? UserSave.earnCurrencyCustomSocialLinkData.TryGetValue(profile.customIdentifier, out value) : UserSave.earnCurrencyData.TryGetValue(profile.id, out value))
				{
					if (profile.repeatability == Repeatability.OncePerVersion)
					{
						string bundleVersion = DeviceUtility.GetBundleVersion();
						if (value != bundleVersion)
						{
							return true;
						}
					}
					return false;
				}
				return true;
			}
			if (profile.id == Id.VideoAdCycle)
			{
				return isVideoAddsSupported;
			}
			return true;
		}

		public static int GetNumberOfDisplayedNonOfferEarners()
		{
			int num = 0;
			for (int i = 0; i < profiles.Length; i++)
			{
				if (ShouldShowInGUI(i))
				{
					num++;
				}
			}
			return num;
		}

		public static void Trigger(int profileIndex)
		{
			EarnCurrencyProfile profile = profiles[profileIndex];
			Trigger(profile);
		}

		public static void Trigger(EarnCurrencyProfile profile)
		{
			if (!ShouldShowInGUI(profile))
			{
				SDebug.LogError("An already trigerred coin earner button has been pressed!");
				return;
			}
			Flurry.LogEventWithAParameter("Earn Coins item clicked", "Id", (profile.id != Id.CustomLink) ? profile.id.ToString() : profile.customIdentifier);
			if (profile.id == Id.FacebookLogin && SocialManager.instance.FacebookLogin(null))
			{
				UIScreenController.Instance.FacebookLogIn(true);
			}
			if (profile.type == Type.OpenURL)
			{
				int amountOfCoins = profile.GetAmountOfCoins();
				if (amountOfCoins > 0)
				{
					InventorySave.IncreaseCoins(amountOfCoins, "OpenUrl");
					SaveManager.Save();
					if (OnRewardURLOpened__BackingField != null)
					{
						OnRewardURLOpened__BackingField((profile.id != Id.CustomLink) ? profile.id.ToString() : profile.customIdentifier, amountOfCoins);
					}
				}
			}
			else if (profile.type != Type.VideoAd)
			{
				if (profile.type == Type.ClaimReward)
				{
					if (profile.id == Id.FacebookWeeklyGift)
					{
						RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.SuperMysteryBox, ObtainableItemSource.SocialMedia, "FacebookGift", Id.FacebookWeeklyGift.ToString().ToLowerInvariant());
						FacebookWeeklyGiftManager.GiftWasClaimed();
					}
				}
				else
				{
					SDebug.LogError("Unhandled earner type: " + profile.type);
				}
			}
			if (profile.repeatability == Repeatability.Once || profile.repeatability == Repeatability.OncePerVersion)
			{
				string bundleVersion = DeviceUtility.GetBundleVersion();
				if (profile.id != Id.CustomLink)
				{
					UserSave.earnCurrencyData[profile.id] = bundleVersion;
				}
				else
				{
					UserSave.earnCurrencyCustomSocialLinkData[profile.customIdentifier] = bundleVersion;
				}
			}
			if (profile.type == Type.OpenURL)
			{
				Application.OpenURL(profile.url);
			}
			else if (profile.type == Type.VideoAd)
			{
				if (AdSystemManager.IsCached(AdLocations.EARN_COINS_VIDEOS))
				{
					AdSystemManager.Show(AdLocations.EARN_COINS_VIDEOS, () =>
					{
						VideoAdPrizeManager.instance.GiveReward(UnlockItemSource.VideoAdPrizeEarn);
					});
				}
			}
			else if (profile.type == Type.ClaimReward)
			{
				if (profile.id == Id.FacebookWeeklyGift)
				{
					UIScreenController.Instance.PushCelebrationPopup();
				}
			}
			else
			{
				SDebug.LogError("Unhandled earner type: " + profile.type);
			}
		}
	}
}
