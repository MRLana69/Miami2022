using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Data.Managers;
using Data.Save;
using Info;
using UI.Screens.RewardManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class RewardManager
{
	private const int UnusedInt = -1;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<CelebrationReward> OnMysteryBoxAddedForUnlock__BackingField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Action<CelebrationReward> OnRewardPaidOut__BackingField;

	public static bool canShowMultipleQueuedCelebrations = false;

	private static long _nextRewardUid = -1L;

	private static readonly Dictionary<CelebrationRewardOrigin, int> PENDING_REWARD_PRIORITY = new Dictionary<CelebrationRewardOrigin, int>
	{
		{
			CelebrationRewardOrigin.NewHighScore,
			0
		},
		{
			CelebrationRewardOrigin.CharacterUnlock,
			1
		},
		{
			CelebrationRewardOrigin.BoardUnlock,
			2
		},
		{
			CelebrationRewardOrigin.CharacterEventSuperMysteryBox,
			3
		},
		{
			CelebrationRewardOrigin.WeeklyChallengeHiddenPrize,
			4
		},
		{
			CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox,
			5
		},
		{
			CelebrationRewardOrigin.WeeklyChallengeMysteryBox,
			6
		},
		{
			CelebrationRewardOrigin.WeeklyChallengeNormal,
			7
		},
		{
			CelebrationRewardOrigin.SuperMysteryBox,
			8
		},
		{
			CelebrationRewardOrigin.MysteryBox,
			9
		},
		{
			CelebrationRewardOrigin.MysteryBoxMini,
			10
		},
		{
			CelebrationRewardOrigin.TokenMysteryBox,
			11
		},
		{
			CelebrationRewardOrigin.WeeklyChallengeTokenMysteryBox,
			12
		}
	};

	public static int rewardsToUnlockCount
	{
		get
		{
			return InventorySave.pendingRewards.Count;
		}
	}

	public static event Action<CelebrationReward> OnMysteryBoxAddedForUnlock
	{
		add
		{
			Action<CelebrationReward> action = OnMysteryBoxAddedForUnlock__BackingField;
			Action<CelebrationReward> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnMysteryBoxAddedForUnlock__BackingField, (Action<CelebrationReward>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<CelebrationReward> action = OnMysteryBoxAddedForUnlock__BackingField;
			Action<CelebrationReward> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnMysteryBoxAddedForUnlock__BackingField, (Action<CelebrationReward>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static event Action<CelebrationReward> OnRewardPaidOut
	{
		add
		{
			Action<CelebrationReward> action = OnRewardPaidOut__BackingField;
			Action<CelebrationReward> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnRewardPaidOut__BackingField, (Action<CelebrationReward>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<CelebrationReward> action = OnRewardPaidOut__BackingField;
			Action<CelebrationReward> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnRewardPaidOut__BackingField, (Action<CelebrationReward>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}
	}

	public static void AddRewardToUnlock(CelebrationRewardOrigin origin, ObtainableItemSource syboAnalyticsSource, string kilooAnalyticsSourceId = null, string syboAnalyticsSourceMeta = null)
	{
		CelebrationReward celebrationReward = null;
		switch (origin)
		{
		case CelebrationRewardOrigin.MysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
			celebrationReward = MysteryBox.Roll(MysteryBox.Type.Normal);
			break;
		case CelebrationRewardOrigin.SuperMysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
			celebrationReward = MysteryBox.Roll(MysteryBox.Type.Super);
			break;
		case CelebrationRewardOrigin.MysteryBoxMini:
			celebrationReward = MysteryBox.Roll(MysteryBox.Type.Mini);
			break;
		case CelebrationRewardOrigin.TokenMysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeTokenMysteryBox:
			celebrationReward = MysteryBox.Roll(MysteryBox.Type.Token);
			break;
		}
		if (celebrationReward != null)
		{
			celebrationReward.kilooAnalyticsSourceId = kilooAnalyticsSourceId;
			celebrationReward.CelebrationRewardOrigin = origin;
			celebrationReward.syboAnalyticsSource = syboAnalyticsSource;
			celebrationReward.syboAnalyticsSourceId = -1L;
			celebrationReward.syboAnalyticsSourceMetaOrNull = syboAnalyticsSourceMeta;
			AddMysteryBoxToUnlock(celebrationReward);
		}
	}

	public static void AddRewardToUnlock(CelebrationReward reward)
	{
		if (reward.CelebrationRewardOrigin != CelebrationRewardOrigin.Notset && reward.rewardType != CelebrationRewardType._notset)
		{
			AddMysteryBoxToUnlock(reward);
		}
	}

	public static long GenerateRewardUid()
	{
		return SubwayRandom.DateTimeTicksRandomOffset();
	}

	private static void AddMysteryBoxToUnlock(CelebrationReward reward)
	{
		bool flag = InventorySave.pendingRewards.Exists((CelebrationReward mb) => mb.Uid == reward.Uid);
		if (reward.Uid <= 0 || !flag)
		{
			if (_nextRewardUid == -1)
			{
				reward.Uid = GenerateRewardUid();
			}
			else
			{
				reward.Uid = _nextRewardUid;
				_nextRewardUid = -1L;
			}
		}
		if (!flag)
		{
			if (reward.rewardType == CelebrationRewardType.token && reward.amount > CharacterManager.AmountOfUsefulTokens(reward.characterId))
			{
				reward.amount = CharacterManager.AmountOfUsefulTokens(reward.characterId);
			}
			InventorySave.AddPendingReward(reward, GetPendingRewardPriority);
			if (OnMysteryBoxAddedForUnlock__BackingField != null)
			{
				OnMysteryBoxAddedForUnlock__BackingField(reward);
			}
		}
	}

	public static CelebrationReward[] GetRewardsToUnlockForCelebration()
	{
		if (canShowMultipleQueuedCelebrations)
		{
			return InventorySave.pendingRewards.ToArray();
		}
		if (InventorySave.lastAddedReward != null && InventorySave.pendingRewards.Count > 0)
		{
			return new CelebrationReward[1] { InventorySave.lastAddedReward };
		}
		return new CelebrationReward[0];
	}

	public static void RewardPaidOut(CelebrationReward celebrationReward)
	{
		InventorySave.RemovePendingReward(celebrationReward);
		if (OnRewardPaidOut__BackingField != null)
		{
			OnRewardPaidOut__BackingField(celebrationReward);
		}
	}

	public static List<CelebrationReward> GetWeeklyHuntRewardsToUnlock()
	{
		List<CelebrationReward> list = new List<CelebrationReward>();
		for (int i = 0; i < InventorySave.pendingRewards.Count; i++)
		{
			CelebrationReward celebrationReward = InventorySave.pendingRewards[i];
			if (celebrationReward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeNormal)
			{
				list.Add(celebrationReward);
			}
		}
		return list;
	}

	public static void PayoutNonMysteryBoxRewards()
	{
		List<CelebrationReward> source = new List<CelebrationReward>(InventorySave.pendingRewards);
		source = source.Where((CelebrationReward celebrationReward) => !IsRewardMysteryBox(celebrationReward)).ToList();
		foreach (CelebrationReward item in source)
		{
			UIScreenController.Instance.PayoutCelebrationReward(item);
		}
	}

	private static bool IsRewardMysteryBox(CelebrationReward reward)
	{
		return reward.CelebrationRewardOrigin == CelebrationRewardOrigin.MysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.SuperMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.CharacterEventSuperMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.TokenMysteryBox || reward.CelebrationRewardOrigin == CelebrationRewardOrigin.WeeklyChallengeTokenMysteryBox;
	}

	public static void SetNextRewardUid(long rewardUid)
	{
		_nextRewardUid = rewardUid;
	}

	public static MysteryBox.Type ConvertRewardOriginToBoxType(CelebrationRewardOrigin origin)
	{
		switch (origin)
		{
		case CelebrationRewardOrigin.MysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeMysteryBox:
			return MysteryBox.Type.Normal;
		case CelebrationRewardOrigin.MysteryBoxMini:
			return MysteryBox.Type.Mini;
		case CelebrationRewardOrigin.SuperMysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeSuperMysteryBox:
		case CelebrationRewardOrigin.CharacterEventSuperMysteryBox:
			return MysteryBox.Type.Super;
		case CelebrationRewardOrigin.TokenMysteryBox:
		case CelebrationRewardOrigin.WeeklyChallengeTokenMysteryBox:
			return MysteryBox.Type.Token;
		default:
			Debug.LogErrorFormat("Unhandled reward origin {0} received when converting to box type.", origin);
			return MysteryBox.Type.Normal;
		}
	}

	private static int GetPendingRewardPriority(CelebrationRewardOrigin origin)
	{
		if (PENDING_REWARD_PRIORITY.ContainsKey(origin))
		{
			return PENDING_REWARD_PRIORITY[origin];
		}
		return PENDING_REWARD_PRIORITY.Count + 1;
	}
}
