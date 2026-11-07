using System.Collections.Generic;
using Data.Save;
using TopRun;
using UI.Screens.RewardManagement;

public static class TopRunRewardHelper
{
	private static int _amountOfFriendsBeaten;

	public static void PrepareAndCallCelebrationScreen(IManager manager)
	{
		CelebrationReward pendingReward = GetPendingReward();
		RewardManager.AddRewardToUnlock(pendingReward);
		TopRunSave.lastTopRunResultAwardedID = TopRunSave.pendingTopRunResultID;
		UIScreenController.Instance.QueuePopup("CelebrationPopup");
	}

	private static void GivePendingRewardSilently()
	{
		CelebrationReward pendingReward = GetPendingReward();
		TrackRewardStats(pendingReward);
		TopRunSave.lastTopRunResultAwardedID = TopRunSave.pendingTopRunResultID;
		TopRunEventsHandler.instance.TriggerOnTopRunChanged(TopRunEventsHandler.instance, new TournamentChangedEventArgs());
	}

	public static bool ShouldShowResults()
	{
		return !string.IsNullOrEmpty(TopRunSave.pendingTopRunResultID) && !string.Equals(TopRunSave.pendingTopRunResultID, TopRunSave.lastTopRunResultAwardedID);
	}

	private static CelebrationReward GetPendingReward()
	{
		CelebrationReward celebrationReward = new CelebrationReward();
		celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.TopRun;
		celebrationReward.rewardType = CelebrationRewardType.topRun;
		celebrationReward.rank = TopRunSave.pendingTopRunResultsRank + 1;
		celebrationReward.score = TopRunSave.pendingTopRunResultsScore;
		celebrationReward.weekNumber = TopRunSave.pendingTopRunResultsWeek;
		return celebrationReward;
	}

	public static void RefreshPendingResults()
	{
		IManager manager = TopRunRoot.manager;
		if (manager == null || manager.tournament == null || !manager.tournament.hasResult || manager.score <= 0)
		{
			return;
		}
		if (!string.IsNullOrEmpty(TopRunSave.pendingTopRunResultID))
		{
			if (TopRunSave.pendingTopRunResultID != manager.tournament.id)
			{
				CalculateBeatenFriendsAward();
				if (TopRunSave.lastTopRunResultAwardedID != TopRunSave.pendingTopRunResultID)
				{
					GivePendingRewardSilently();
					OverridePendingResults(manager);
				}
				else
				{
					OverridePendingResults(manager);
				}
			}
		}
		else
		{
			OverridePendingResults(manager);
		}
	}

	private static void CalculateBeatenFriendsAward()
	{
		IManager manager = TopRunRoot.manager;
		if (manager == null || manager.tournament == null)
		{
			return;
		}
		List<IPlayer> list = new List<IPlayer>();
		list.AddRange(manager.tournament.GetPlayers());
		List<IPlayer> list2 = new List<IPlayer>();
		foreach (IPlayer item in list)
		{
			if (item.isFriend)
			{
				list2.Add(item);
			}
		}
		foreach (IPlayer item2 in list2)
		{
			if (manager.score > item2.score)
			{
				_amountOfFriendsBeaten++;
			}
		}
	}

	private static void OverridePendingResults(IManager manager)
	{
		if (manager.tournament != null)
		{
			TopRunSave.pendingTopRunResultID = manager.tournament.id;
			TopRunSave.pendingTopRunResultsWeek = manager.tournament.sequenceId;
			TopRunSave.pendingTopRunResultsScore = manager.score;
			TopRunSave.pendingTopRunResultsRank = manager.tournament.GetRankForScore(manager.score);
			TopRunSave.pendingTopRunBeatenFriendsAward = _amountOfFriendsBeaten;
		}
	}

	public static void TrackRewardStats(CelebrationReward reward)
	{
		Stat stat = Stat.NotSet;
		bool flag = false;
		switch (reward.rank)
		{
		case 1:
			flag = true;
			stat = Stat.TopRunRank0;
			break;
		case 2:
			flag = true;
			stat = Stat.TopRunRank1;
			break;
		case 3:
			flag = true;
			stat = Stat.TopRunRank2;
			break;
		case 4:
			flag = true;
			stat = Stat.TopRunRank3;
			break;
		case 5:
			stat = Stat.TopRunRank4;
			break;
		case 6:
			stat = Stat.TopRunRank5;
			break;
		case 7:
			stat = Stat.TopRunRank6;
			break;
		case 8:
			stat = Stat.TopRunRank7;
			break;
		case 9:
			stat = Stat.TopRunRank8;
			break;
		case 10:
			stat = Stat.TopRunRank9;
			break;
		case 11:
			stat = Stat.TopRunRank10;
			break;
		}
		stat.IncrementValue();
		if (flag)
		{
			Stat.TopRunRank5orBetter.IncrementValue();
		}
	}
}
