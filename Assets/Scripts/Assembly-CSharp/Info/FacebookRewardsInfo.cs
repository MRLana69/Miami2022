using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Data.Managers;
using Data.Save;
using Kiloo;
using SYBO.Subway.Info;
using UI.Screens;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace Info
{
	public static class FacebookRewardsInfo
	{
		public enum FacebookRewardType
		{
			Coins = 0
		}

		public const int POPUP_DISPLAY_TIME_LIMIT = 300;

		public const int POPUP_DISPLAY_RUN_LIMIT = 6;

		public const string FACEBOOK_REWARD_RUN_LIMIT_KEY = "facebook_reward_run_limit";

		public const string FACEBOOK_REWARD_TIME_LIMIT_KEY = "facebook_reward_time_limit";

		public const Characters.CharacterId AWARDED_CHARACTER = Characters.CharacterId.facebookRewardCharacter01;

		public const int CHARACTER_THEME = 0;

		public static DateTime connectPopupLastShownOnGameOver = CDateTime.Now.AddDays(-1.0);

		public static DateTime connectPopupLastShownOnFriendsScreen = CDateTime.Now.AddDays(-1.0);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<FacebookRewardType, int> OnFacebookRewardPaidOut__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<Characters.CharacterId, int> OnFacebookCharacterReceived__BackingField;

		public static Dictionary<FacebookRewardType, int> facebookRewardPack = new Dictionary<FacebookRewardType, int> { 
		{
			FacebookRewardType.Coins,
			5000
		} };

		public static event Action<FacebookRewardType, int> OnFacebookRewardPaidOut
		{
			add
			{
				Action<FacebookRewardType, int> action = OnFacebookRewardPaidOut__BackingField;
				Action<FacebookRewardType, int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnFacebookRewardPaidOut__BackingField, (Action<FacebookRewardType, int>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<FacebookRewardType, int> action = OnFacebookRewardPaidOut__BackingField;
				Action<FacebookRewardType, int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnFacebookRewardPaidOut__BackingField, (Action<FacebookRewardType, int>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<Characters.CharacterId, int> OnFacebookCharacterReceived
		{
			add
			{
				Action<Characters.CharacterId, int> action = OnFacebookCharacterReceived__BackingField;
				Action<Characters.CharacterId, int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnFacebookCharacterReceived__BackingField, (Action<Characters.CharacterId, int>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<Characters.CharacterId, int> action = OnFacebookCharacterReceived__BackingField;
				Action<Characters.CharacterId, int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref OnFacebookCharacterReceived__BackingField, (Action<Characters.CharacterId, int>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static bool CanPayoutReward()
		{
			return !UserSave.hasPaidOutFacebookReward && SocialManager.instance.facebookIsLoggedIn;
		}

		public static bool ShouldShowConnectPopupOnGameOver(int showCounter)
		{
			if (AgeRestriction.isRestricted)
			{
				return false;
			}
			TimeSpan timeSpan = CDateTime.Now - connectPopupLastShownOnGameOver;
			TimeSpan timeSpan2 = CDateTime.Now - connectPopupLastShownOnFriendsScreen;
			int value = OnlineSettings.instance.GetValue("facebook_reward_time_limit", 300);
			int value2 = OnlineSettings.instance.GetValue("facebook_reward_run_limit", 6);
			return !UserSave.hasPaidOutFacebookReward && Math.Abs((int)timeSpan.TotalSeconds) >= value && Math.Abs((int)timeSpan2.TotalSeconds) >= value && showCounter >= value2;
		}

		public static bool ShouldShowConnectPopupOnFriendsScreen()
		{
			if (Application.isEditor)
			{
			}
			if (AgeRestriction.isRestricted)
			{
				return false;
			}
			TimeSpan timeSpan = CDateTime.Now - connectPopupLastShownOnFriendsScreen;
			int value = OnlineSettings.instance.GetValue("facebook_reward_time_limit", 300);
			return !UserSave.hasPaidOutFacebookReward && Math.Abs((int)timeSpan.TotalSeconds) >= value;
		}

		public static bool PayoutRewardIfPossible()
		{
			if (!CanPayoutReward())
			{
				return false;
			}
			PayoutReward();
			return true;
		}

		private static void PayoutReward()
		{
			if (!UserSave.hasPaidOutFacebookReward)
			{
				Outfit outfitForCharacter = Outfits.GetOutfitForCharacter(Characters.CharacterId.facebookRewardCharacter01, 0);
				bool flag = outfitForCharacter != null;
				if (OnFacebookCharacterReceived__BackingField != null)
				{
					OnFacebookCharacterReceived__BackingField(Characters.CharacterId.facebookRewardCharacter01, 0);
				}
				if (flag)
				{
					CharacterManager.UnlockOutfit(Characters.CharacterId.facebookRewardCharacter01, 0);
				}
				else
				{
					CharacterManager.UnlockCharacter(Characters.CharacterId.facebookRewardCharacter01);
				}
				CharacterScreenManager.Instance.OnFacebookCharacterUnlocked();
				CelebrationReward celebrationReward = new CelebrationReward();
				celebrationReward.rewardType = CelebrationRewardType.coins;
				celebrationReward.amount = facebookRewardPack[FacebookRewardType.Coins];
				celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.UnlockItem;
				celebrationReward.unlockItemSource = UnlockItemSource.FacebookPromotion;
				CelebrationReward reward = celebrationReward;
				RewardManager.AddRewardToUnlock(reward);
				if (OnFacebookRewardPaidOut__BackingField != null)
				{
					OnFacebookRewardPaidOut__BackingField(FacebookRewardType.Coins, facebookRewardPack[FacebookRewardType.Coins]);
				}
				RewardManager.canShowMultipleQueuedCelebrations = true;
				UserSave.hasPaidOutFacebookReward = true;
				SaveManager.Save();
			}
		}
	}
}
