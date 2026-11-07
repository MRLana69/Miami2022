#define ENABLE_ERROR_LOGS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AppRefresh;
using Data.Managers;
using Data.Save;
using Info;
using JetBrains.Annotations;
using Kiloo;
using Kiloo.Common.Ads;
using Kiloo.Common.Utility;
using SYBO.Components.Analytics.Integration;
using SYBO.Subway;
using SYBO.Subway.Characters;
using SYBO.Subway.Info;
using UI.Pages.MissionsPageInternal;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace Missions
{
	public class MissionsManager
	{
		public enum VideoButtonTrackingState
		{
			NotSet = 0,
			Fill = 1,
			NoFill = 2,
			Cooldown = 3,
			AlreadyUsedForSet = 4
		}

		public delegate void MissionSetCompleteHandler();

		public delegate void MissionCompleteHandler(string msg, int missionIndex);

		public delegate void MissionProgressHandler(int missionIndex, float progressFactor);

		private int _currentMissionTemplateSetLoaded = -1;

		private MissionTemplate[] _templates;

		private const int MISSION_COUNT_IN_SET = 3;

		private const string OS_SKIP_WITH_VIDEO_COOLDOWN_VALUE_KEY = "mission_skip_with_video_cooldown_seconds";

		private int _defaultSkipWithVideoCooldownSeconds = 28800;

		private int _defaultMissionIndexUpdateInterval = 28800;

		private const string OS_MISSION_INDEX_UPDATE_INTERVAL = "mission_skip_with_video_index_update_interval_seconds";

		public Action<bool, int> OnMissionSkipped;

		private int[] _currentRunProgress;

		private int _currentMissionSetLoaded = -1;

		private Mission[] _combinedArray;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static MissionSetCompleteHandler onMissionSetComplete__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static MissionCompleteHandler onMissionComplete__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MissionProgressHandler onMissionProgressed__BackingField;

		private Mission[][] _missions;

		private static MissionsManager _instance;

		public VideoButtonTrackingState videoButtonState { get; private set; }

		[NotNull]
		private MissionTemplate[] templates
		{
			get
			{
				if (_currentMissionTemplateSetLoaded != PlayerSave.currentMissionSet || _templates == null)
				{
					Dictionary<MissionType, MissionTemplate> missionTemplates = MissionsData.missionTemplates;
					if (_templates == null)
					{
						_templates = new MissionTemplate[44];
					}
					for (int i = 0; i < 3; i++)
					{
						_templates[i] = missionTemplates[missions[PlayerSave.currentMissionSet][i].type];
					}
					if (_currentMissionTemplateSetLoaded == -1)
					{
						for (int j = 0; j < 41; j++)
						{
							_templates[j + 3] = missionTemplates[Achievements.Instance.achievementArray[j].type];
						}
					}
					_currentMissionTemplateSetLoaded = PlayerSave.currentMissionSet;
				}
				return _templates;
			}
		}

		public bool inRun
		{
			get
			{
				return _currentRunProgress != null;
			}
			set
			{
				if (value)
				{
					_currentRunProgress = new int[combinedArray.Length];
					StayInOneLaneMission.instance.Init();
				}
				else if (_currentRunProgress != null)
				{
					CheckAllCompleteAndIncrement();
					SyncAchievements();
					if (GameStats.Instance.CoinsCollected == 50)
					{
						SocialManager.instance.ReportAchievement("CollectExactly50Coins", 100f);
					}
					_currentRunProgress = null;
				}
			}
		}

		private Mission[] combinedArray
		{
			get
			{
				if (_currentMissionSetLoaded != PlayerSave.currentMissionSet || _combinedArray == null)
				{
					if (_combinedArray == null)
					{
						_combinedArray = new Mission[44];
					}
					for (int i = 0; i < 3; i++)
					{
						_combinedArray[i] = missions[PlayerSave.currentMissionSet][i];
					}
					if (_currentMissionSetLoaded == -1)
					{
						for (int j = 0; j < 41; j++)
						{
							_combinedArray[j + 3] = Achievements.Instance.achievementArray[j];
						}
					}
				}
				_currentMissionSetLoaded = currentMissionSet;
				return _combinedArray;
			}
		}

		public int currentMissionSet
		{
			get
			{
				return PlayerSave.currentMissionSet;
			}
			set
			{
				if (value != PlayerSave.currentMissionSet)
				{
					if (value >= missionSetCount)
					{
						int num = PlayerSave.currentMissionSet - MissionsData.repeatableMissions.Length + 1;
						InitCurrentMissionSet(num, missions[num].Length, true);
					}
					else
					{
						InitCurrentMissionSet(value, missions[value].Length, true);
					}
				}
			}
		}

		private Mission[][] missions
		{
			get
			{
				if (_missions == null)
				{
					Mission[][] repeatableMissions = MissionsData.repeatableMissions;
					Mission[][] storylineMissions = MissionsData.storylineMissions;
					_missions = new Mission[GetNumberOfBasicMission() + repeatableMissions.Length][];
					for (int i = 0; i < GetNumberOfBasicMission(); i++)
					{
						_missions[i] = storylineMissions[i];
					}
					for (int j = 0; j < repeatableMissions.Length; j++)
					{
						_missions[GetNumberOfBasicMission() + j] = repeatableMissions[j];
					}
				}
				return _missions;
			}
		}

		public int missionSetCount
		{
			get
			{
				return missions.Length;
			}
		}

		public int missionSetStoryCount
		{
			get
			{
				return missions.Length - MissionsData.repeatableMissions.Length;
			}
		}

		public static MissionsManager Instance
		{
			get
			{
				return _instance ?? (_instance = new MissionsManager());
			}
		}

		public static event MissionSetCompleteHandler onMissionSetComplete
		{
			add
			{
				MissionSetCompleteHandler missionSetCompleteHandler = onMissionSetComplete__BackingField;
				MissionSetCompleteHandler missionSetCompleteHandler2;
				do
				{
					missionSetCompleteHandler2 = missionSetCompleteHandler;
					missionSetCompleteHandler = Interlocked.CompareExchange(ref onMissionSetComplete__BackingField, (MissionSetCompleteHandler)Delegate.Combine(missionSetCompleteHandler2, value), missionSetCompleteHandler);
				}
				while ((object)missionSetCompleteHandler != missionSetCompleteHandler2);
			}
			remove
			{
				MissionSetCompleteHandler missionSetCompleteHandler = onMissionSetComplete__BackingField;
				MissionSetCompleteHandler missionSetCompleteHandler2;
				do
				{
					missionSetCompleteHandler2 = missionSetCompleteHandler;
					missionSetCompleteHandler = Interlocked.CompareExchange(ref onMissionSetComplete__BackingField, (MissionSetCompleteHandler)Delegate.Remove(missionSetCompleteHandler2, value), missionSetCompleteHandler);
				}
				while ((object)missionSetCompleteHandler != missionSetCompleteHandler2);
			}
		}

		public static event MissionCompleteHandler onMissionComplete
		{
			add
			{
				MissionCompleteHandler missionCompleteHandler = onMissionComplete__BackingField;
				MissionCompleteHandler missionCompleteHandler2;
				do
				{
					missionCompleteHandler2 = missionCompleteHandler;
					missionCompleteHandler = Interlocked.CompareExchange(ref onMissionComplete__BackingField, (MissionCompleteHandler)Delegate.Combine(missionCompleteHandler2, value), missionCompleteHandler);
				}
				while ((object)missionCompleteHandler != missionCompleteHandler2);
			}
			remove
			{
				MissionCompleteHandler missionCompleteHandler = onMissionComplete__BackingField;
				MissionCompleteHandler missionCompleteHandler2;
				do
				{
					missionCompleteHandler2 = missionCompleteHandler;
					missionCompleteHandler = Interlocked.CompareExchange(ref onMissionComplete__BackingField, (MissionCompleteHandler)Delegate.Remove(missionCompleteHandler2, value), missionCompleteHandler);
				}
				while ((object)missionCompleteHandler != missionCompleteHandler2);
			}
		}

		public event MissionProgressHandler onMissionProgressed
		{
			add
			{
				MissionProgressHandler missionProgressHandler = onMissionProgressed__BackingField;
				MissionProgressHandler missionProgressHandler2;
				do
				{
					missionProgressHandler2 = missionProgressHandler;
					missionProgressHandler = Interlocked.CompareExchange(ref onMissionProgressed__BackingField, (MissionProgressHandler)Delegate.Combine(missionProgressHandler2, value), missionProgressHandler);
				}
				while ((object)missionProgressHandler != missionProgressHandler2);
			}
			remove
			{
				MissionProgressHandler missionProgressHandler = onMissionProgressed__BackingField;
				MissionProgressHandler missionProgressHandler2;
				do
				{
					missionProgressHandler2 = missionProgressHandler;
					missionProgressHandler = Interlocked.CompareExchange(ref onMissionProgressed__BackingField, (MissionProgressHandler)Delegate.Remove(missionProgressHandler2, value), missionProgressHandler);
				}
				while ((object)missionProgressHandler != missionProgressHandler2);
			}
		}

		private MissionsManager()
		{
			SetupDefaultMissionSetIfNeeded();
			AppRefreshHandler.hasRefreshed += (RefreshType type) =>
			{
				if (type.HasFlag(RefreshType.Data))
				{
					SetupDefaultMissionSetIfNeeded();
				}
			};
			ApplyChangesToProgressIfNecessary();
			if (Application.isEditor)
			{
				ValidateMissions();
			}
		}

		public void InitCurrentMissionSet(int missionSet, int missionCount, bool resetProgress)
		{
			if (missionSet != PlayerSave.currentMissionSet)
			{
				PlayerSave.currentMissionSet = missionSet;
				if (resetProgress)
				{
					PlayerSave.InitializeCurrentMissionProgress(missionCount);
				}
				LogMissionSetRuns();
				DetermineNewSkipForVideoMissionIndex();
				PlayerSave.hasSkippedMissionForVideoThisSet = false;
				PlayerSave.runsCompletedInCurrentMissionsSet = 0;
				PlayerSave.ResetSkipMissions();
				ScoreMultiplierInfo.TriggerOnScoreMultiplierChanged();
			}
		}

		private void DetermineNewSkipForVideoMissionIndex()
		{
			List<int> list = new List<int>();
			int currentSkipForVideoMissionIndex = PlayerSave.currentSkipForVideoMissionIndex;
			for (int i = 0; i < 3; i++)
			{
				if (!GetMissionInfo(i).complete && currentSkipForVideoMissionIndex != i)
				{
					list.Add(i);
				}
			}
			if (list.Count > 0)
			{
				int index = KRandom.Range(0, list.Count);
				PlayerSave.currentSkipForVideoMissionIndex = list[index];
			}
			PlayerSave.currentSkipForVideoIndexSetAt = CDateTime.UtcNow;
		}

		public bool CanThisMissionSkipWithVideo(MissionCell.MissionCellPayload payload)
		{
			videoButtonState = VideoButtonTrackingState.NotSet;
			AdLocationInfo currentLocationInfo = GetCurrentLocationInfo(Game.Instance.IsInGame.Value);
			if (MissionIndexNeedsUpdating())
			{
				DetermineNewSkipForVideoMissionIndex();
			}
			if (payload.missionIndex != PlayerSave.currentSkipForVideoMissionIndex)
			{
				return false;
			}
			if (!AdSystemManager.GetAvailabilityForLocation(currentLocationInfo.id))
			{
				return false;
			}
			if (PlayerSave.hasSkippedMissionForVideoThisSet)
			{
				videoButtonState = VideoButtonTrackingState.AlreadyUsedForSet;
				return false;
			}
			if ((CDateTime.UtcNow - PlayerSave.lastTimeMissionWasSkippedForVideo).TotalSeconds < (double)OnlineSettings.instance.GetValue("mission_skip_with_video_cooldown_seconds", _defaultSkipWithVideoCooldownSeconds))
			{
				videoButtonState = VideoButtonTrackingState.Cooldown;
				return false;
			}
			bool flag = AdSystemManager.IsCached(currentLocationInfo);
			string argValue = ((!flag) ? "NoFill" : "Available");
			string eventName = ((payload.source != MissionCell.PayloadSource.PauseMenu) ? "Mission video fill attempt" : "Mission video game pause fill attempt");
			Flurry.LogEventWithAParameter(eventName, "Video status", argValue);
			if (!flag)
			{
				videoButtonState = VideoButtonTrackingState.NoFill;
				return false;
			}
			videoButtonState = VideoButtonTrackingState.Fill;
			return true;
		}

		private bool MissionIndexNeedsUpdating()
		{
			int value = OnlineSettings.instance.GetValue("mission_skip_with_video_index_update_interval_seconds", _defaultMissionIndexUpdateInterval);
			DateTime dateTime = PlayerSave.currentSkipForVideoIndexSetAt + TimeSpan.FromSeconds(value);
			if (dateTime <= CDateTime.UtcNow)
			{
				return true;
			}
			return false;
		}

		private void LogMissionSetRuns()
		{
			if (PlayerSave.runsCompletedInCurrentMissionsSet != -1)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Runs completed", PlayerSave.runsCompletedInCurrentMissionsSet.ToString());
				string empty = string.Empty;
				empty += ((!PlayerSave.HasSkippedMission(0)) ? "0" : "1");
				empty += ",";
				empty += ((!PlayerSave.HasSkippedMission(1)) ? "0" : "1");
				empty += ",";
				empty += ((!PlayerSave.HasSkippedMission(2)) ? "0" : "1");
				dictionary.Add("Skipped missions", empty);
				Flurry.LogEventWithSeveralParameters("Runs completed for mission", dictionary);
			}
		}

		public void SkipMission(int missionNumber, bool skippedWithVideo)
		{
			if (skippedWithVideo)
			{
				PlayerSave.lastTimeMissionWasSkippedForVideo = CDateTime.UtcNow;
				PlayerSave.hasSkippedMissionForVideoThisSet = true;
			}
			PlayerDidThis(MissionTarget.SkipMission);
			PlayerSave.SetCurrentMissionProgress(missionNumber, missions[PlayerSave.currentMissionSet][missionNumber].goal);
			PlayerSave.SetSkipMission(missionNumber, true);
			if (OnMissionSkipped != null)
			{
				OnMissionSkipped(skippedWithVideo, missionNumber);
			}
			Complete(missionNumber);
			SaveManager.Save();
		}

		private void Complete(int index, float completedFactor = 1f, bool sendEvenIfInRun = false)
		{
			if (index < 3)
			{
				MissionProgressHandler missionProgressHandler = onMissionProgressed__BackingField;
				if (missionProgressHandler != null)
				{
					missionProgressHandler(index, completedFactor);
				}
				if (Math.Abs(completedFactor - 1f) > Mathf.Epsilon)
				{
					return;
				}
				MissionCompleteHandler missionCompleteHandler = onMissionComplete__BackingField;
				if (missionCompleteHandler != null)
				{
					string format = ((combinedArray[index].goal != 1) ? Strings.Get(templates[index].ultraShortDescription) : Strings.Get(templates[index].ultraShortDescriptionSingle));
					missionCompleteHandler(string.Format(format, combinedArray[index].goal), index);
				}
				Stat.MissionCompleted.IncrementValue();
				if (index == PlayerSave.currentSkipForVideoMissionIndex)
				{
					DetermineNewSkipForVideoMissionIndex();
				}
				bool flag = true;
				int num = 0;
				for (int i = 0; i < 3; i++)
				{
					int currentMissionProgress = PlayerSave.GetCurrentMissionProgress(i);
					if (currentMissionProgress >= combinedArray[i].goal)
					{
						num++;
					}
					if (currentMissionProgress < combinedArray[i].goal)
					{
						flag = false;
						break;
					}
				}
				if (num >= 2 && inRun)
				{
					SocialManager.instance.ReportAchievement(Achievements.Instance.achievementIds[36], 100f);
				}
				if (!flag)
				{
					return;
				}
				Flurry.LogEventWithAParameter("Mission Set completed", "Mission Set", (PlayerSave.missionSetCompletedCount + 1).ToString());
				PlayerDidThis(MissionTarget.MissionSet);
				if (_currentRunProgress != null)
				{
					for (int j = 0; j < 3; j++)
					{
						_currentRunProgress[j] = 0;
					}
				}
				if (currentMissionSet + 1 == missionSetStoryCount)
				{
					OnboardingSave.shouldShowEndGameMissionPopup = true;
				}
				if (currentMissionSet + 1 > missionSetStoryCount)
				{
					RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.SuperMysteryBox, ObtainableItemSource.MissionCompleted, "MissionSet", AnalyticsHelper.GetSyboAnalyticsMissionSetString());
					if (!inRun && !UIScreenController.Instance.IsCelebrationPopupBlocked())
					{
						UIScreenController.Instance.PushCelebrationPopup();
					}
				}
				int num2 = ((currentMissionSet < missionSetCount - 1) ? (PlayerSave.currentMissionSet + 1) : (PlayerSave.currentMissionSet - MissionsData.repeatableMissions.Length + 1));
				PlayerSave.missionSetCompletedCount++;
				int missionCount = 0;
				if (num2 < missionSetCount)
				{
					missionCount = missions[num2].Length;
				}
				InitCurrentMissionSet(num2, missionCount, true);
				MissionSetCompleteHandler missionSetCompleteHandler = onMissionSetComplete__BackingField;
				if (missionSetCompleteHandler != null)
				{
					missionSetCompleteHandler();
				}
				PlayerDidThis(MissionTarget.ReachMissionSet);
			}
			else if (!inRun || sendEvenIfInRun)
			{
				int num3 = index - 3;
				if (num3 < 0 || num3 >= 41)
				{
					LogError("Missions.Complete: achievement index is out of bounds");
				}
				SocialManager.instance.ReportAchievement(Achievements.Instance.achievementIds[num3], Mathf.Clamp01(completedFactor) * 100f);
			}
		}

		public void RemoveProgressForThis(MissionTarget myTask)
		{
			for (int i = 0; i < combinedArray.Length; i++)
			{
				if (templates[i].missionTarget == myTask)
				{
					ResetProgressForMissionIndex(i);
				}
			}
		}

		private void CheckAllCompleteAndIncrement()
		{
			for (int i = 3; i < combinedArray.Length; i++)
			{
				int num = _currentRunProgress[i];
				int goal = combinedArray[i].goal;
				int currentMissionProgress = PlayerSave.GetCurrentMissionProgress(i);
				if (templates[i].singleRun && templates[i].completeIfLess)
				{
					if (num < goal)
					{
						PlayerSave.SetCurrentMissionProgress(i, goal);
						Complete(i, 1f, true);
					}
				}
				else if (_currentRunProgress != null)
				{
					if ((float)num / (float)goal > (float)currentMissionProgress / (float)goal)
					{
						Complete(i, (float)num / (float)goal, true);
					}
					else
					{
						Complete(i, (float)currentMissionProgress / (float)goal, true);
					}
				}
				else
				{
					Complete(i, (float)currentMissionProgress / (float)goal, true);
				}
			}
		}

		private void OverWriteAchievementsProgress(MissionTarget myTask, int value)
		{
			for (int i = 3; i < combinedArray.Length; i++)
			{
				if (templates[i].missionTarget == myTask)
				{
					PlayerSave.SetCurrentMissionProgress(i, value);
				}
			}
		}

		private void ResetProgressForMissionIndex(int questIndex)
		{
			if (!GetMissionInfo(questIndex).complete)
			{
				PlayerSave.SetCurrentMissionProgress(questIndex, 0);
				if (_currentRunProgress != null)
				{
					_currentRunProgress[questIndex] = 0;
				}
			}
		}

		private void ApplyChangesForVersion(int versionNumber)
		{
			int numberOfBasicMission = GetNumberOfBasicMission();
			if (currentMissionSet < missionSetCount)
			{
				bool flag = currentMissionSet < numberOfBasicMission;
				List<MissionIndicator> list = ((!flag) ? MissionsData.missionsModificationsForRepeatable : MissionsData.missionsModificationsForStoryline);
				for (int i = 0; i < list.Count; i++)
				{
					MissionIndicator missionIndicator = list[i];
					int num = ((!flag) ? (missionIndicator.missionSet + numberOfBasicMission - 1) : (missionIndicator.missionSet - 1));
					if (missionIndicator.versionNo == versionNumber && currentMissionSet == num)
					{
						int missionNumberInSet = missionIndicator.missionNumberInSet;
						PlayerSave.SetCurrentMissionProgress(missionNumberInSet, 0);
						if (_currentRunProgress != null)
						{
							_currentRunProgress[missionNumberInSet] = 0;
						}
					}
				}
			}
			if (currentMissionSet < numberOfBasicMission || !MissionsData.missionsInsertedForRepeatable.ContainsKey(versionNumber))
			{
				return;
			}
			int[] array = MissionsData.missionsInsertedForRepeatable[versionNumber];
			int num2 = currentMissionSet - numberOfBasicMission;
			int num3 = num2;
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] - 1 <= num3)
				{
					num3++;
				}
			}
			if (Application.isEditor)
			{
				for (int k = 0; k < array.Length - 1; k++)
				{
					if (array[k] >= array[k + 1])
					{
						LogError("The missionsInsertedForRepeatable (in MissionsData) dictionary contains unsorted arrays as values ");
					}
				}
			}
			int num4 = num3 - num2;
			int num5 = Mathf.Clamp(currentMissionSet + num4, 0, missionSetCount);
			if (missions.Length > num5)
			{
				InitCurrentMissionSet(num5, missions[num5].Length, false);
			}
		}

		private void ApplyChangesToProgressIfNecessary()
		{
			int missionsVersion = PlayerSave.missionsVersion;
			if (missionsVersion < 3)
			{
				for (int i = missionsVersion + 1; i <= 3; i++)
				{
					ApplyChangesForVersion(i);
				}
			}
			PlayerSave.missionsVersion = 3;
		}

		public void OnChangeIsCharacterOnGround(Character character)
		{
			if (character.TransformPosition.y < 1f)
			{
				RemoveProgressForThis(MissionTarget.EarnCoinWithoutTouchingGround);
			}
		}

		private void SyncAchievements()
		{
			OverWriteAchievementsProgress(MissionTarget.ReachMissionSet, PlayerSave.currentMissionSet);
			OverWriteAchievementsProgress(MissionTarget.HaveUpgrades, InventorySave.upgrades.Sum((KeyValuePair<UpgradeType, int> x) => x.Value));
			OverWriteAchievementsProgress(MissionTarget.HaveTrophiesFirstAchievements, PlayerSave.GetTrophyAchievementProgressCount());
			OverWriteAchievementsProgress(MissionTarget.FacebookLoggedIn, SocialManager.instance.facebookIsLoggedIn.CompareTo(false));
			int achievementIndexInArray = Achievements.Instance.GetAchievementIndexInArray(MissionType.HaveHeadStartLarge);
			if (achievementIndexInArray > -1 && InventorySave.GetAmount(UpgradeType.headstart2000) >= Achievements.Instance.achievementArray[achievementIndexInArray].goal)
			{
				SocialManager.instance.ReportAchievement(Achievements.Instance.achievementIds[achievementIndexInArray], 100f);
			}
			foreach (KeyValuePair<Characters.CharacterId, Characters.Model> characterDatum in Characters.characterData)
			{
				if (characterDatum.Value.missionTargetKey != MissionTarget.none)
				{
					OverWriteAchievementsProgress(characterDatum.Value.missionTargetKey, CharacterManager.GetCollectedTokens(characterDatum.Key));
				}
			}
		}

		public bool IsMissionTargetActive(MissionTarget target)
		{
			for (int i = 0; i < combinedArray.Length; i++)
			{
				if (templates[i].missionTarget == target)
				{
					int currentMissionProgress = PlayerSave.GetCurrentMissionProgress(i);
					return currentMissionProgress < combinedArray[i].goal;
				}
			}
			return false;
		}

		private static int ApplyCoinDoublers(int value)
		{
			if (UserSave.hasDoubleCoinsUpgrade)
			{
				value <<= 1;
			}
			if (DoubleCoins.HasDoubleCoinsModifier)
			{
				value <<= 1;
			}
			return value;
		}

		private static bool IsOneOf(MissionTarget needle, params MissionTarget[] haystack)
		{
			return haystack.Contains(needle);
		}

		public void PlayerDidThis(MissionTarget myTask, int value = 1, int missionToIgnore = -1)
		{
			if (IsOneOf(myTask, MissionTarget.CoinsWithJetpack, MissionTarget.CoinsWithMagnet, MissionTarget.CollectCoinsCenterLane, MissionTarget.CollectCoinsLeftLane, MissionTarget.CollectCoinsRightLane, MissionTarget.CollectCoinsWithPowerJumper, MissionTarget.EarnCoinWithoutTouchingGround))
			{
				value = ApplyCoinDoublers(value);
			}
			for (int i = 0; i < combinedArray.Length; i++)
			{
				if (i == missionToIgnore)
				{
					continue;
				}
				MissionTemplate missionTemplate = templates[i];
				Mission mission = combinedArray[i];
				if (missionTemplate.missionTarget != myTask || (missionTemplate.singleRun && !inRun))
				{
					continue;
				}
				bool flag = missionTemplate.singleRun && inRun && _currentRunProgress != null;
				int num = PlayerSave.GetCurrentMissionProgress(i);
				if (flag && num < mission.goal)
				{
					num = _currentRunProgress[i];
				}
				int num2 = num + value;
				if (_currentRunProgress != null)
				{
					_currentRunProgress[i] = num2;
				}
				if (missionTemplate.completeIfLess)
				{
					num2 = value;
					if (num2 < mission.goal && num != mission.goal)
					{
						PlayerSave.SetCurrentMissionProgress(i, mission.goal);
						Complete(i);
					}
				}
				else if (missionTemplate.completeIfEqual)
				{
					if (myTask == MissionTarget.GetExactlyAmountOfCoins)
					{
						num2 = value;
					}
					if (num2 == mission.goal)
					{
						PlayerSave.SetCurrentMissionProgress(i, missions[PlayerSave.currentMissionSet][i].goal);
						Complete(i);
					}
				}
				else if (num2 >= mission.goal || !missionTemplate.singleRun)
				{
					int num3 = Mathf.Min(num2, mission.goal);
					PlayerSave.SetCurrentMissionProgress(i, num3);
					if (num < mission.goal)
					{
						Complete(i, (float)num3 / (float)mission.goal);
					}
				}
			}
		}

		public MissionInfo[] GetMissionInfo()
		{
			MissionInfo[] array = new MissionInfo[3];
			for (int i = 0; i < 3; i++)
			{
				GetMissionInfo(i, ref array[i]);
			}
			return array;
		}

		public MissionInfo GetMissionInfo(int missionNumber)
		{
			MissionInfo info = default(MissionInfo);
			GetMissionInfo(missionNumber, ref info);
			return info;
		}

		private void GetMissionInfo(int missionNumber, ref MissionInfo info)
		{
			info.mission = combinedArray[missionNumber];
			info.template = templates[missionNumber];
			info.progress = PlayerSave.GetCurrentMissionProgress(missionNumber);
			info.complete = info.progress >= info.mission.goal;
			if (!info.complete && info.template.singleRun && inRun)
			{
				info.progress = _currentRunProgress[missionNumber];
			}
		}

		public MissionInfo[] GetNextMissionInfos()
		{
			MissionInfo[] array = new MissionInfo[3];
			int num = currentMissionSet + 1;
			if (num >= missions.Length)
			{
				num = missions.Length - MissionsData.repeatableMissions.Length;
			}
			for (int i = 0; i < 3; i++)
			{
				Mission mission = missions[num][i];
				MissionInfo missionInfo = new MissionInfo
				{
					mission = mission,
					template = MissionsData.missionTemplates[mission.type],
					progress = 0,
					complete = false
				};
				array[i] = missionInfo;
			}
			return array;
		}

		[Conditional("ENABLE_MISSIONMANAGER_DEBUG")]
		private void TestGoToMissionSum(int mission)
		{
			int num = ((mission <= MissionsData.storylineMissions.Length) ? mission : ((mission - MissionsData.storylineMissions.Length) % MissionsData.repeatableMissions.Length + MissionsData.storylineMissions.Length));
			int missionCount = 0;
			if (num < missionSetCount)
			{
				missionCount = missions[num].Length;
			}
			InitCurrentMissionSet(num - 1, missionCount, true);
		}

		public int GetNumberOfBasicMission()
		{
			return MissionsData.storylineMissions.Length;
		}

		private void SetupDefaultMissionSetIfNeeded()
		{
			if (PlayerSave.currentMissionSet == -1)
			{
				InitCurrentMissionSet(0, missions[0].Length, true);
			}
		}

		public static AdLocationInfo GetCurrentLocationInfo(bool isInGame)
		{
			if (isInGame)
			{
				return AdLocations.SKIP_MISSION_PAUSE_VIDEOS;
			}
			return AdLocations.SKIP_MISSION_TOUR_VIDEOS;
		}

		private static void ValidateMissions()
		{
			Dictionary<MissionType, MissionTemplate> missionTemplates = MissionsData.missionTemplates;
			string[] names = Enum.GetNames(typeof(MissionType));
			foreach (string text in names)
			{
				MissionType key = (MissionType)Enum.Parse(typeof(MissionType), text);
				if (!missionTemplates.ContainsKey(key))
				{
					LogError("MissionType " + text + " is missing from MissionTemplates array");
				}
			}
			foreach (KeyValuePair<MissionType, MissionTemplate> item in missionTemplates)
			{
				if (item.Value.completeIfLess && !item.Value.singleRun)
				{
					LogError(string.Concat("completeIfLess misisons needs to be singlerun, ", item.Key, " ", item.Value));
				}
				if (item.Value.missionTarget.ToString() != item.Key.ToString() && !item.Value.singleRun)
				{
					LogError("naming convension error or forgot to ass the single run bool:" + item.Key);
				}
			}
			foreach (KeyValuePair<MissionType, MissionTemplate> item2 in missionTemplates)
			{
				if (item2.Key.ToString() != item2.Value.missionTarget.ToString() && item2.Key.ToString().Remove(item2.Key.ToString().Length - 9, 9) != item2.Value.missionTarget.ToString())
				{
					LogError("missions target and mission type confict in missionTemplates Key: " + item2.Key);
				}
			}
			Mission[][] repeatableMissions = MissionsData.repeatableMissions;
			Mission[][] array = repeatableMissions;
			foreach (Mission[] array2 in array)
			{
				Mission[] array3 = array2;
				for (int k = 0; k < array3.Length; k++)
				{
					Mission mission = array3[k];
					if (string.IsNullOrEmpty(Strings.Get(missionTemplates[mission.type].description)) || string.IsNullOrEmpty(Strings.Get(missionTemplates[mission.type].ultraShortDescriptionSingle)) || string.IsNullOrEmpty(Strings.Get(missionTemplates[mission.type].ultraShortDescription)) || string.IsNullOrEmpty(Strings.Get(missionTemplates[mission.type].descriptionSingle)))
					{
						LogError("no description for repeatableMission: " + mission.type);
					}
				}
			}
			Mission[][] storylineMissions = MissionsData.storylineMissions;
			Mission[][] array4 = storylineMissions;
			foreach (Mission[] array5 in array4)
			{
				Mission[] array6 = array5;
				for (int m = 0; m < array6.Length; m++)
				{
					Mission mission2 = array6[m];
					if (string.IsNullOrEmpty(Strings.Get(missionTemplates[mission2.type].description)) || string.IsNullOrEmpty(Strings.Get(missionTemplates[mission2.type].ultraShortDescriptionSingle)) || string.IsNullOrEmpty(Strings.Get(missionTemplates[mission2.type].ultraShortDescription)) || string.IsNullOrEmpty(Strings.Get(missionTemplates[mission2.type].descriptionSingle)))
					{
						LogError("no description for missions: " + mission2.type);
					}
				}
			}
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogError(string msg, UnityEngine.Object context = null)
		{
			SDebug.LogError(msg, context);
		}

		[Conditional("ENABLE_ERROR_LOGS")]
		private static void LogWarning(string msg, UnityEngine.Object context = null)
		{
		}

		[Conditional("ENABLE_MISSIONMANAGER_LOGS")]
		private static void Log(string msg, UnityEngine.Object context = null)
		{
		}
	}
}
