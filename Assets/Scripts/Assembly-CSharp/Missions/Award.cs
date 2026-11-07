using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using AppRefresh;
using Data.Save;
using Info;
using Kiloo;
using Kiloo.Common.Extensions;
using UI.SlideIns;
using UnityEngine;

namespace Missions
{
	public abstract class Award
	{
		protected readonly AwardStaticData _awardData;

		private AwardTier _awardActiveTier;

		private DateTime _previewDateTime = DateTime.MinValue;

		protected AwardTier _awardProgressTier;

		protected readonly PersistentAwardProgress savedProgress;

		public AwardTierType tierAnimationPlayed;

		private const char SEPERATOR_CHAR = ';';

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Action<Award> onStateChangedHandler__BackingField;

		public AwardStaticData awardsData
		{
			get
			{
				return _awardData;
			}
		}

		public AwardGoal[] goals
		{
			get
			{
				if (_awardActiveTier != null)
				{
					return _awardActiveTier.listGoals;
				}
				return null;
			}
		}

		public DateTime lastStateChangeDateTime
		{
			get
			{
				if (isInPreview)
				{
					return _previewDateTime;
				}
				return savedProgress.lastActiveStateChangeDateTime;
			}
		}

		public AwardState awardState
		{
			get
			{
				return savedProgress.currentActiveAwardState;
			}
			set
			{
				savedProgress.currentActiveAwardState = value;
			}
		}

		public bool isNew
		{
			get
			{
				bool value;
				if (PlayerSave.awardIsNew.TryGetValue(_awardData.id, out value))
				{
					return value;
				}
				return true;
			}
			set
			{
				PlayerSave.awardIsNew.AddOrSet(_awardData.id, value);
			}
		}

		public AwardTierType activeTierType
		{
			get
			{
				return _awardActiveTier.tierType;
			}
		}

		public bool isInPreview { get; set; }

		public int reward
		{
			get
			{
				if (_awardActiveTier != null)
				{
					return _awardActiveTier.reward;
				}
				return 0;
			}
		}

		private event Action<Award> onStateChangedHandler
		{
			add
			{
				Action<Award> action = onStateChangedHandler__BackingField;
				Action<Award> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onStateChangedHandler__BackingField, (Action<Award>)Delegate.Combine(action2, value), action);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<Award> action = onStateChangedHandler__BackingField;
				Action<Award> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange(ref onStateChangedHandler__BackingField, (Action<Award>)Delegate.Remove(action2, value), action);
				}
				while ((object)action != action2);
			}
		}

		protected Award(AwardStaticData awardData, PersistentAwardProgress savedProgress, Action<Award> handler)
		{
			onStateChangedHandler += handler;
			isInPreview = false;
			_awardData = awardData;
			this.savedProgress = savedProgress;
			AppRefreshHandler.willRefresh += AppWillRefresh;
		}

		private void AppWillRefresh(RefreshType refreshType)
		{
			if (refreshType.HasFlag(RefreshType.Data))
			{
				DeactivateCurrentAwardTier();
				AppRefreshHandler.willRefresh -= AppWillRefresh;
			}
		}

		public void Activate()
		{
			if (savedProgress.currentActiveAwardState != AwardState.Finished)
			{
				AwardTier value;
				if (savedProgress.progressTierType != AwardTierType.None && awardsData.missionTiers.TryGetValue(savedProgress.progressTierType, out value))
				{
					_awardProgressTier = value;
					AwardTier value2;
					if (savedProgress.activeTierType != AwardTierType.None && awardsData.missionTiers.TryGetValue(savedProgress.activeTierType, out value2))
					{
						_awardActiveTier = value2;
					}
					CheckTierValues();
				}
				else
				{
					for (int i = 0; i < Enum.GetNames(typeof(AwardTierType)).Length; i++)
					{
						AwardTier value3;
						if (awardsData.missionTiers.TryGetValue((AwardTierType)i, out value3))
						{
							_awardProgressTier = value3;
							_awardActiveTier = value3;
							savedProgress.currentActiveAwardState = AwardState.InProgress;
							savedProgress.currentProgressAwardState = AwardState.InProgress;
							RecordNewOffsets();
							break;
						}
					}
				}
				if (_awardProgressTier != null && savedProgress.currentProgressAwardState == AwardState.InProgress)
				{
					ActivateCurrentAwardTier();
				}
				return;
			}
			for (int num = Enum.GetNames(typeof(AwardTierType)).Length - 1; num > 0; num--)
			{
				AwardTier value4;
				if (awardsData.missionTiers.TryGetValue((AwardTierType)num, out value4))
				{
					_awardActiveTier = value4;
					_awardProgressTier = value4;
					break;
				}
			}
		}

		private void CheckTierValues()
		{
			if (_awardActiveTier.tierType > _awardProgressTier.tierType)
			{
				_awardActiveTier = _awardData.missionTiers[_awardProgressTier.tierType];
				savedProgress.currentActiveAwardState = savedProgress.currentProgressAwardState;
			}
		}

		public bool BelongsToState(AwardState state)
		{
			if (isInPreview)
			{
				if (state == AwardState.Collectible)
				{
					return true;
				}
			}
			else if (awardState == state)
			{
				return true;
			}
			return false;
		}

		public int GetProgressAsPercentage()
		{
			if (savedProgress.currentActiveAwardState == AwardState.Collectible || savedProgress.currentActiveAwardState == AwardState.Finished)
			{
				return 100;
			}
			int num = 0;
			int num2 = 0;
			AwardGoal[] listGoals = _awardActiveTier.listGoals;
			foreach (AwardGoal awardGoal in listGoals)
			{
				num += GetProgressForGoal(awardGoal);
				num2 += awardGoal.goal;
			}
			float value = (float)num * 100f / (float)num2;
			value = Mathf.Clamp(value, 0f, 100f);
			return (int)value;
		}

		protected void CheckAndUpdateProgress()
		{
			if ((awardsData.useFacebook && AgeRestriction.isRestricted) || savedProgress.currentProgressAwardState == AwardState.Finished)
			{
				return;
			}
			bool flag = true;
			AwardGoal[] listGoals = _awardProgressTier.listGoals;
			foreach (AwardGoal goal in listGoals)
			{
				if (!IsGoalAccomplished(goal))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				GoToState(AwardState.Collectible);
			}
		}

		private void GoToState(AwardState state)
		{
			savedProgress.currentProgressAwardState = state;
			if (savedProgress.currentProgressAwardState == AwardState.InProgress)
			{
				onStateChangedHandler__BackingField.SafeInvoke(this);
				ActivateCurrentAwardTier();
			}
			else if (savedProgress.currentProgressAwardState == AwardState.Collectible)
			{
				// No navegador o save é regravado a cada abertura, e o aviso "Boa aparência concluído" aparecia toda vez
				// que o jogo abria: fora da corrida não mostra o aviso
				bool gjQuiet = Application.platform == RuntimePlatform.WebGLPlayer && (!SYBO.Subway.Game.IsInstantiated || !SYBO.Subway.Game.Instance.IsInGame.Value);
				if (UIScreenController.IsInstanced && !gjQuiet)
				{
					string stringValue = string.Format(Strings.Get("AWARD_SLIDEIN_LINE2"), Strings.Get(awardsData.title));
					UIScreenController.Instance.QueueSlideIn(new SlideIn
					{
						type = SlideInData.SlideInType.Awards,
						awardTierType = _awardProgressTier.tierType,
						stringValue = stringValue
					});
				}
				FlurryLogAwardCollectibleInformation(awardsData.id.ToString(), (int)activeTierType, DateTime.Now);
				if (_awardActiveTier.tierType == _awardProgressTier.tierType && savedProgress.currentActiveAwardState == AwardState.InProgress)
				{
					savedProgress.currentActiveAwardState = AwardState.Collectible;
					_previewDateTime = CDateTime.UtcNow;
					savedProgress.lastActiveStateChangeDateTime = CDateTime.UtcNow;
				}
				DeactivateCurrentAwardTier();
				MoveToNextTierOrFinishIfLast();
			}
			else if (savedProgress.currentProgressAwardState == AwardState.Finished)
			{
				onStateChangedHandler__BackingField.SafeInvoke(this);
				Flurry.LogEventWithAParameter("Award completed", "Award", awardsData.id.ToString());
				if (savedProgress.currentActiveAwardState == AwardState.InProgress)
				{
					savedProgress.currentActiveAwardState = AwardState.Collectible;
					savedProgress.lastActiveStateChangeDateTime = CDateTime.UtcNow;
				}
			}
		}

		private void FlurryLogAwardCollectibleInformation(string throphyDataID, int activeTierTypeID, DateTime dateTime)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder.Append("Award");
			stringBuilder.Append(';');
			stringBuilder.Append("Tier");
			stringBuilder2.Append(throphyDataID);
			stringBuilder2.Append(';');
			stringBuilder2.Append(activeTierTypeID);
			Flurry.LogEventWithSeveralParameters("Award tier completed", stringBuilder.ToString(), stringBuilder2.ToString());
			stringBuilder.Append(';');
			stringBuilder.Append("Time");
			stringBuilder2.Append(';');
			stringBuilder2.Append(dateTime);
			Flurry.LogEventWithSeveralParameters("Award tier completed time", stringBuilder.ToString(), stringBuilder2.ToString());
		}

		public void RewardPayedOut()
		{
			if (savedProgress.currentActiveAwardState == AwardState.Collectible)
			{
				PlayerSave.awardHasPayedOut.AddOrSet(_awardData.id, true);
				CalculateNextActiveTier();
			}
		}

		protected void CalculateNextActiveTier()
		{
			if (_awardActiveTier.tierType < _awardProgressTier.tierType)
			{
				int key = (int)(_awardActiveTier.tierType + 1);
				AwardTier value;
				if (awardsData.missionTiers.TryGetValue((AwardTierType)key, out value))
				{
					_awardActiveTier = value;
					if (_awardActiveTier.tierType == _awardProgressTier.tierType)
					{
						if (savedProgress.currentProgressAwardState == AwardState.Collectible || savedProgress.currentProgressAwardState == AwardState.Finished)
						{
							if (savedProgress.currentActiveAwardState != AwardState.Collectible)
							{
								_previewDateTime = CDateTime.UtcNow;
							}
							savedProgress.currentActiveAwardState = AwardState.Collectible;
						}
						else
						{
							savedProgress.currentActiveAwardState = AwardState.InProgress;
						}
					}
				}
			}
			else if (savedProgress.currentProgressAwardState == AwardState.Finished)
			{
				savedProgress.lastActiveStateChangeDateTime = CDateTime.UtcNow;
				savedProgress.currentActiveAwardState = AwardState.Finished;
			}
			onStateChangedHandler__BackingField.SafeInvoke(this);
		}

		protected void MoveToNextTierOrFinishIfLast()
		{
			bool flag = false;
			for (int i = (int)(_awardProgressTier.tierType + 1); i < Enum.GetNames(typeof(AwardTierType)).Length; i++)
			{
				AwardTier value;
				if (awardsData.missionTiers.TryGetValue((AwardTierType)i, out value))
				{
					_awardProgressTier = value;
					flag = true;
					break;
				}
			}
			if (flag)
			{
				RecordNewOffsets();
				GoToState(AwardState.InProgress);
			}
			else
			{
				GoToState(AwardState.Finished);
			}
		}

		public void CompleteCurrentTierIfDebug()
		{
			if (Globals.DEBUG_POPUP)
			{
				MoveToNextTierOrFinishIfLast();
			}
		}

		public bool HasCompletedTierExact(AwardTierType tier)
		{
			if (_awardProgressTier.tierType == tier)
			{
				if (savedProgress.currentProgressAwardState == AwardState.Collectible || savedProgress.currentProgressAwardState == AwardState.Finished)
				{
					return true;
				}
			}
			else if (_awardProgressTier.tierType == tier + 1 && savedProgress.currentProgressAwardState == AwardState.InProgress)
			{
				return true;
			}
			return false;
		}

		public void SetBackToInProgress()
		{
			if (savedProgress.currentActiveAwardState == AwardState.Finished && savedProgress.activeTierType != (AwardTierType)_awardData.missionTiers.Count)
			{
				savedProgress.activeTierType++;
				savedProgress.currentActiveAwardState = AwardState.InProgress;
				savedProgress.progressTierType++;
				savedProgress.currentProgressAwardState = AwardState.InProgress;
				Activate();
			}
		}

		public PersistentAwardProgress GetPersistentProgress()
		{
			savedProgress.activeTierType = _awardActiveTier.tierType;
			savedProgress.progressTierType = _awardProgressTier.tierType;
			return savedProgress;
		}

		public void SetAwardTierToAlmostComplete()
		{
			if (!Globals.DEBUG)
			{
				return;
			}
			AwardGoal[] listGoals = _awardActiveTier.listGoals;
			foreach (AwardGoal awardGoal in listGoals)
			{
				AwardStatGoal awardStatGoal = awardGoal as AwardStatGoal;
				if (awardStatGoal != null)
				{
					awardStatGoal.stat.SetValue(awardGoal.goal - 1);
				}
			}
		}

		public abstract bool IsGoalAccomplished(AwardGoal goal);

		protected abstract int GetProgressForGoal(AwardGoal goal);

		protected abstract void ActivateCurrentAwardTier();

		protected virtual void DeactivateCurrentAwardTier()
		{
		}

		protected abstract void RecordNewOffsets();
	}
}
