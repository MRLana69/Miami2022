using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Data.Managers;
using Info;
using Kiloo.Common.Extensions;
using Missions;
using SYBO.Subway.Info;
using UnityEngine;

namespace Data.Save
{
	public class PlayerSave : DataManager<PlayerSave>
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action highScoreChanged__BackingField;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<Stat, int> statChanged__BackingField;

		public static int runsCompletedInCurrentMissionsSet
		{
			get
			{
				return DataManager<PlayerSave>.cloud.runsCompletedInCurrentMissionSet;
			}
			set
			{
				DataManager<PlayerSave>.cloud.runsCompletedInCurrentMissionSet = value;
			}
		}

		public static int currentMissionSet
		{
			get
			{
				return DataManager<PlayerSave>.cloud.currentMissionSet;
			}
			set
			{
				DataManager<PlayerSave>.cloud.currentMissionSet = value;
			}
		}

		public static int currentSkipForVideoMissionIndex
		{
			get
			{
				return DataManager<PlayerSave>.cloud.currentSkipForVideoMissionIndex;
			}
			set
			{
				DataManager<PlayerSave>.cloud.currentSkipForVideoMissionIndex = value;
			}
		}

		public static DateTime currentSkipForVideoIndexSetAt
		{
			get
			{
				return DataManager<PlayerSave>.cloud.currentSkipForVideoIndexSetAt;
			}
			set
			{
				DataManager<PlayerSave>.cloud.currentSkipForVideoIndexSetAt = value;
			}
		}

		public static DateTime lastTimeMissionWasSkippedForVideo
		{
			get
			{
				return DataManager<PlayerSave>.cloud.lastTimeMissionWasSkippedForVideo;
			}
			set
			{
				DataManager<PlayerSave>.cloud.lastTimeMissionWasSkippedForVideo = value;
			}
		}

		public static bool hasSkippedMissionForVideoThisSet
		{
			get
			{
				return DataManager<PlayerSave>.cloud.hasSkippedMissionForVideoThisSet;
			}
			set
			{
				DataManager<PlayerSave>.cloud.hasSkippedMissionForVideoThisSet = value;
			}
		}

		public static int missionSetCompletedCount
		{
			get
			{
				return Mathf.Max(1, DataManager<PlayerSave>.cloud.missionSetCompletedCount);
			}
			set
			{
				DataManager<PlayerSave>.cloud.missionSetCompletedCount = value;
			}
		}

		public static int missionsVersion
		{
			get
			{
				return DataManager<PlayerSave>.local.missionsVersion;
			}
			set
			{
				DataManager<PlayerSave>.local.missionsVersion = value;
			}
		}

		public static int unreportedGames
		{
			get
			{
				return DataManager<PlayerSave>.cloud.unreportedGames;
			}
			set
			{
				DataManager<PlayerSave>.cloud.unreportedGames = value;
			}
		}

		public static DateTime unreportedGamesTimeStamp
		{
			get
			{
				return DataManager<PlayerSave>.cloud.unreportedGamesTimeStamp;
			}
			set
			{
				DataManager<PlayerSave>.cloud.unreportedGamesTimeStamp = value;
			}
		}

		public static int highScore
		{
			get
			{
				return DataManager<PlayerSave>.cloud.highScore;
			}
		}

		public static int bragScoreThreshold { get; set; }

		public static Dictionary<AwardId, PersistentAwardProgress> awardsProgress
		{
			get
			{
				return DataManager<PlayerSave>.cloud.awardsProgress;
			}
			set
			{
				DataManager<PlayerSave>.cloud.awardsProgress = value;
			}
		}

		public static DateTime weeklyGiftUnlockTime
		{
			get
			{
				return DataManager<PlayerSave>.cloud.weeklyGiftUnlockTime;
			}
			set
			{
				DataManager<PlayerSave>.cloud.weeklyGiftUnlockTime = value;
			}
		}

		public static int completedRunCount
		{
			get
			{
				return DataManager<PlayerSave>.cloud.completedRunCount;
			}
			set
			{
				DataManager<PlayerSave>.cloud.completedRunCount = value;
			}
		}

		public static int currentOutfitIndex
		{
			get
			{
				return CharacterManager.GetOutfitIndex(currentCharacterId);
			}
			set
			{
				CharacterManager.SetOutfitSelectionIndex(currentCharacterId, value);
			}
		}

		public static Characters.CharacterId currentCharacterId
		{
			get
			{
				return DataManager<PlayerSave>.cloud.currentCharacterId;
			}
			set
			{
				DataManager<PlayerSave>.cloud.currentCharacterId = value;
			}
		}

		public static Boards.BoardId currentBoardId
		{
			get
			{
				return DataManager<PlayerSave>.cloud.currentBoardId;
			}
			set
			{
				DataManager<PlayerSave>.cloud.currentBoardId = value;
			}
		}

		public static Boards.BoardId previousBoardId
		{
			get
			{
				return DataManager<PlayerSave>.cloud.previousBoardId;
			}
			set
			{
				DataManager<PlayerSave>.cloud.previousBoardId = value;
			}
		}

		public static Dictionary<AwardId, bool> awardIsNew
		{
			get
			{
				return DataManager<PlayerSave>.cloud.awardIsNew;
			}
			set
			{
				DataManager<PlayerSave>.cloud.awardIsNew = value;
			}
		}

		public static Dictionary<AwardId, bool> awardHasPayedOut
		{
			get
			{
				return DataManager<PlayerSave>.cloud.awardHasPayedOut;
			}
			set
			{
				DataManager<PlayerSave>.cloud.awardHasPayedOut = value;
			}
		}

		public static bool awardsFirstLoaded
		{
			get
			{
				return DataManager<PlayerSave>.cloud.awardsFirstLoaded;
			}
			set
			{
				DataManager<PlayerSave>.cloud.awardsFirstLoaded = value;
			}
		}

		public static event Action highScoreChanged
		{
			add
			{
				Action action = highScoreChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref highScoreChanged__BackingField, (Action)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action action = highScoreChanged__BackingField;
				Action action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref highScoreChanged__BackingField, (Action)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static event Action<Stat, int> statChanged
		{
			add
			{
				Action<Stat, int> action = statChanged__BackingField;
				Action<Stat, int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref statChanged__BackingField, (Action<Stat, int>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<Stat, int> action = statChanged__BackingField;
				Action<Stat, int> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref statChanged__BackingField, (Action<Stat, int>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		public static bool HasSkippedMission(int index)
		{
			bool[] hasSkippedMissions = DataManager<PlayerSave>.cloud.hasSkippedMissions;
			if (hasSkippedMissions.ValidIndex(index))
			{
				return hasSkippedMissions[index];
			}
			return false;
		}

		public static void ResetSkipMissions()
		{
			SetSkipMission(0, false);
			SetSkipMission(1, false);
			SetSkipMission(2, false);
		}

		public static void SetSkipMission(int index, bool skip)
		{
			bool[] hasSkippedMissions = DataManager<PlayerSave>.cloud.hasSkippedMissions;
			if (hasSkippedMissions.ValidIndex(index))
			{
				hasSkippedMissions[index] = skip;
			}
		}

		public static int GetCurrentMissionProgress(int index)
		{
			List<int> achievementProgress = DataManager<PlayerSave>.cloud.achievementProgress;
			if (index >= 3 && achievementProgress.ValidIndex(index))
			{
				return achievementProgress[index - 3];
			}
			int[] currentMissionProgress = DataManager<PlayerSave>.cloud.currentMissionProgress;
			if (currentMissionProgress.ValidIndex(index))
			{
				return currentMissionProgress[index];
			}
			return 0;
		}

		public static int GetAchievementProgress(int index)
		{
			List<int> achievementProgress = DataManager<PlayerSave>.cloud.achievementProgress;
			if (achievementProgress.ValidIndex(index))
			{
				return achievementProgress[index];
			}
			return 0;
		}

		public static void SetCurrentMissionProgress(int index, int value)
		{
			List<int> achievementProgress = DataManager<PlayerSave>.cloud.achievementProgress;
			if (index >= 3 && achievementProgress.ValidIndex(index))
			{
				achievementProgress[index - 3] = value;
			}
			int[] currentMissionProgress = DataManager<PlayerSave>.cloud.currentMissionProgress;
			if (currentMissionProgress.ValidIndex(index))
			{
				currentMissionProgress[index] = value;
			}
		}

		public static void InitializeCurrentMissionProgress(int count)
		{
			DataManager<PlayerSave>.cloud.currentMissionProgress = new int[count];
		}

		public static void LoadCurrentMissionProgress(int[] savedArray)
		{
			DataManager<PlayerSave>.cloud.currentMissionProgress = savedArray;
		}

		public static void LoadAchievementProgress(int[] savedArray)
		{
			if (savedArray != null)
			{
				DataManager<PlayerSave>.cloud.achievementProgress = savedArray.ToList();
			}
		}

		public static void SetHighScore(int value)
		{
			DataManager<PlayerSave>.cloud.highScore = value;
			highScoreChanged__BackingField.SafeInvoke();
		}

		public static int GetTrophyAchievementProgressCount()
		{
			int num = 0;
			for (int i = 0; i < 8; i++)
			{
				if (DataManager<PlayerSave>.cloud.unlockedTrophies[i])
				{
					num++;
				}
			}
			return num;
		}

		public static void UnlockTrophy(Trophies.Trophy trophy)
		{
			DataManager<PlayerSave>.cloud.unlockedTrophies[(int)trophy] = true;
		}

		public static int GetStatValue(Stat stat)
		{
			VerifyStateArrayLength(stat);
			return DataManager<PlayerSave>.cloud.statValues[(int)stat];
		}

		public static void SetStatValue(Stat stat, int value)
		{
			int statValue = GetStatValue(stat);
			DataManager<PlayerSave>.cloud.statValues[(int)stat] = value;
			if (statValue != value)
			{
				statChanged__BackingField.SafeInvoke(stat, value);
			}
		}

		private static void VerifyStateArrayLength(Stat stat)
		{
			int num = Enum.GetNames(typeof(Stat)).Length;
			if (DataManager<PlayerSave>.cloud.statValues.Length != num)
			{
				Array.Resize(ref DataManager<PlayerSave>.cloud.statValues, num);
				DataManager<PlayerSave>.cloud.statValues[(int)stat] = 0;
			}
		}

		public static int GetMysteryBoxesOpened(MysteryBox.Type type)
		{
			ManualMysteryBoxMigrate();
			return DataManager<PlayerSave>.cloud.mysteryBoxesOpened[(int)type];
		}

		public static void IncrementMysteryBoxesOpened(MysteryBox.Type type)
		{
			ManualMysteryBoxMigrate();
			DataManager<PlayerSave>.cloud.mysteryBoxesOpened[(int)type]++;
		}

		public static void MigrateMysteryBoxOpened(MysteryBox.Type type, int value)
		{
			ManualMysteryBoxMigrate();
			DataManager<PlayerSave>.cloud.mysteryBoxesOpened[(int)type] = value;
		}

		private static void ManualMysteryBoxMigrate()
		{
			int num = Enum.GetNames(typeof(MysteryBox.Type)).Length;
			if (DataManager<PlayerSave>.cloud.mysteryBoxesOpened.Length < num)
			{
				int[] mysteryBoxesOpened = DataManager<PlayerSave>.cloud.mysteryBoxesOpened;
				DataManager<PlayerSave>.cloud.mysteryBoxesOpened = new int[Enum.GetNames(typeof(MysteryBox.Type)).Length];
				for (int i = 0; i < mysteryBoxesOpened.Length; i++)
				{
					DataManager<PlayerSave>.cloud.mysteryBoxesOpened[i] = mysteryBoxesOpened[i];
				}
			}
		}
	}
}
