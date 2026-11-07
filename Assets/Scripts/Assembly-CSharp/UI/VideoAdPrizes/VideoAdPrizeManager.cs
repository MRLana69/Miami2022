using System;
using System.Collections.Generic;
using System.Diagnostics;
using Data.Save;
using Info;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace UI.VideoAdPrizes
{
	public class VideoAdPrizeManager
	{
		public const string LOGGING_SYMBOL = "ENABLE_VIDEO_AD_PRIZE_MANAGER_LOGS";

		private const string ONLINE_SETTING_KEYS_REWARDED_FOR_VIDEO = "keys_rewarded_for_video_ad";

		public const int DEFAULT_NUMBER_OF_KEYS_TO_REWARD_FOR_VIDEO_AD = 3;

		private List<VideoAdPrize> _introPrizePool;

		private List<VideoAdPrize> _randomPrizePool;

		private VideoAdPrize _currentVideoAdPrize;

		private VideoAdPrizePools _videoAdPrizePools;

		private int _currentVideoAdPrizeIndex;

		private PrizePoolState _prizePoolState;

		private static VideoAdPrizeManager _instance;

		public PrizePoolState prizePoolState
		{
			get
			{
				return _prizePoolState;
			}
		}

		public static VideoAdPrizeManager instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new VideoAdPrizeManager();
				}
				return _instance;
			}
		}

		private VideoAdPrizeManager()
		{
			SetVideoAdPrizeIndexes();
			InitPrizePools();
			UpdatePrizePoolState();
			InitRandomPrizePool();
		}

		public void DebugReload()
		{
			_instance = new VideoAdPrizeManager();
		}

		private void SetVideoAdPrizeIndexes()
		{
			_currentVideoAdPrizeIndex = -1;
			if (Globals.DEBUG_ENABLE_GET_2_KEYS && Globals.DEBUG_INTRO_VIDEO_AD_PRIZE_INDEX > -1)
			{
				AdsSave.currentIntroVideoAdPrizeIndex = Globals.DEBUG_INTRO_VIDEO_AD_PRIZE_INDEX;
			}
			if (Globals.DEBUG_ENABLE_GET_2_KEYS && Globals.DEBUG_RANDOM_VIDEO_AD_PRIZE_INDEX > -1)
			{
				AdsSave.currentRandomVideoAdPrizeIndex = Globals.DEBUG_RANDOM_VIDEO_AD_PRIZE_INDEX;
			}
		}

		private void InitPrizePools()
		{
			_videoAdPrizePools = new VideoAdPrizePools();
			_introPrizePool = _videoAdPrizePools.introPrizePool;
		}

		private void InitRandomPrizePool()
		{
			switch (_prizePoolState)
			{
			case PrizePoolState.Intro:
				ResetRandomPrizePool();
				break;
			case PrizePoolState.Random:
				_randomPrizePool = _videoAdPrizePools.GetShuffledCloneOfRandomPrizePool(AdsSave.currentVideoAdPrizeSeed);
				break;
			}
		}

		public void GiveReward(UnlockItemSource rewardSource)
		{
			VideoAdPrize videoAdPrize = GetVideoAdPrize();
			if (videoAdPrize.prizeType == VideoAdPrizeType.DailyVideoPrize)
			{
				UserSave.lastTimeAVideoForKeysWasSeen = UserSave.lastKnownServerTime;
			}
			else if (videoAdPrize.prizeType != VideoAdPrizeType.DailyVideoPrize && videoAdPrize.prizeType != VideoAdPrizeType.NotSet)
			{
				GoToNextVideoAdPrize();
			}
			AdsSave.videosWatchedSinceDailyKeys++;
			videoAdPrize.GiveReward(rewardSource);
		}

		public void GoToNextVideoAdPrize()
		{
			switch (_prizePoolState)
			{
			case PrizePoolState.Intro:
				AdsSave.currentIntroVideoAdPrizeIndex++;
				break;
			case PrizePoolState.Random:
				AdsSave.currentRandomVideoAdPrizeIndex++;
				ResetRandomPrizePoolIfAllPrizesTaken();
				break;
			}
			UpdatePrizePoolState();
		}

		private void ResetRandomPrizePoolIfAllPrizesTaken()
		{
			if (AdsSave.currentRandomVideoAdPrizeIndex >= _randomPrizePool.Count && _prizePoolState == PrizePoolState.Random)
			{
				ResetRandomPrizePool();
			}
		}

		private void ResetRandomPrizePool()
		{
			AdsSave.currentRandomVideoAdPrizeIndex = 0;
			AdsSave.currentVideoAdPrizeSeed = GetRandomVideoAdPrizeSeed();
			_randomPrizePool = _videoAdPrizePools.GetShuffledCloneOfRandomPrizePool(AdsSave.currentVideoAdPrizeSeed);
		}

		public bool IsVideoForKeysAvailable()
		{
			return AbleToWatchAVideoForKeysToday() && GetAmountOfKeysRewardedForVideo() > 0;
		}

		private bool AbleToWatchAVideoForKeysToday()
		{
			return DateTime.Compare(UserSave.lastKnownServerTime.Date, UserSave.lastTimeAVideoForKeysWasSeen.Date) != 0;
		}

		public DateTime GetLastTimeAVideoForKeysWasSeen()
		{
			return UserSave.lastTimeAVideoForKeysWasSeen;
		}

		public int GetAmountOfKeysRewardedForVideo()
		{
			return OnlineSettings.instance.GetValue("keys_rewarded_for_video_ad", 3);
		}

		public VideoAdPrize GetVideoAdPrize()
		{
			if (Globals.DEBUG_ENABLE_GET_2_KEYS && Globals.DEBUG_VIDEO_AD_PRIZE_TYPE != VideoAdPrizeType.NotSet)
			{
				return new VideoAdPrize(Globals.DEBUG_VIDEO_AD_PRIZE_TYPE);
			}
			if (IsVideoForKeysAvailable())
			{
				_currentVideoAdPrize = new VideoAdPrize(VideoAdPrizeType.DailyVideoPrize, GetAmountOfKeysRewardedForVideo());
			}
			else if (HasVideoAdPrizeIndexChanged() || _currentVideoAdPrize.hasBeenRewarded)
			{
				UpdatePrizePoolState();
				UpdateValueOfCurrentVideoAdPrizeIndex();
				ResetRandomPrizePoolIfAllPrizesTaken();
				SetNewVideoAdPrize();
			}
			return _currentVideoAdPrize;
		}

		private void UpdatePrizePoolState()
		{
			if (_introPrizePool.Count > AdsSave.currentIntroVideoAdPrizeIndex)
			{
				_prizePoolState = PrizePoolState.Intro;
			}
			else if (_introPrizePool.Count <= AdsSave.currentIntroVideoAdPrizeIndex)
			{
				_prizePoolState = PrizePoolState.Random;
			}
		}

		private bool HasVideoAdPrizeIndexChanged()
		{
			switch (_prizePoolState)
			{
			case PrizePoolState.Intro:
				return _currentVideoAdPrizeIndex != AdsSave.currentIntroVideoAdPrizeIndex;
			case PrizePoolState.Random:
				return _currentVideoAdPrizeIndex != AdsSave.currentRandomVideoAdPrizeIndex;
			default:
				return false;
			}
		}

		private void UpdateValueOfCurrentVideoAdPrizeIndex()
		{
			switch (_prizePoolState)
			{
			case PrizePoolState.Intro:
				_currentVideoAdPrizeIndex = AdsSave.currentIntroVideoAdPrizeIndex;
				break;
			case PrizePoolState.Random:
				_currentVideoAdPrizeIndex = AdsSave.currentRandomVideoAdPrizeIndex;
				break;
			}
		}

		private int GetRandomVideoAdPrizeSeed()
		{
			if (Globals.DEBUG_ENABLE_GET_2_KEYS && Globals.DEBUG_RANDOM_VIDEO_AD_PRIZE_SEED > -1)
			{
				return Globals.DEBUG_RANDOM_VIDEO_AD_PRIZE_SEED;
			}
			return Mathf.Abs((int)DateTime.UtcNow.Ticks);
		}

		private void SetNewVideoAdPrize()
		{
			switch (_prizePoolState)
			{
			case PrizePoolState.Intro:
				SetIntroPrize();
				break;
			case PrizePoolState.Random:
				SetRandomPrize();
				break;
			}
		}

		private void SetIntroPrize()
		{
			_currentVideoAdPrize = GetIntroPoolPrize();
		}

		private VideoAdPrize GetIntroPoolPrize()
		{
			return _introPrizePool[_currentVideoAdPrizeIndex];
		}

		private void SetRandomPrize()
		{
			_currentVideoAdPrize = _randomPrizePool[_currentVideoAdPrizeIndex];
		}

		[Conditional("ENABLE_VIDEO_AD_PRIZE_MANAGER_LOGS")]
		public static void Log(object message)
		{
		}
	}
}
