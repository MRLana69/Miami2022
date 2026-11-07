using System;
using System.Collections.Generic;
using System.Diagnostics;
using Data.Save;
using Kiloo.Common.Backend.Cloud;

namespace Cloud.Internal
{
	public class CloudSaveConflictResolver
	{
		public enum Recommendation
		{
			None = 0,
			Mine = 1,
			Theirs = 2
		}

		private ICloudConflict _conflict;

		private UserData _localData;

		private UserData _cloudData;

		public Recommendation recommendation
		{
			get
			{
				if (recommendationEvaluation > 2f)
				{
					return Recommendation.Mine;
				}
				if (recommendationEvaluation < -2f)
				{
					return Recommendation.Theirs;
				}
				return Recommendation.None;
			}
		}

		public float recommendationEvaluation { get; private set; }

		public CloudConflictResolution resolution
		{
			get
			{
				return _conflict.resolveMethod;
			}
		}

		public DateTime lastModifiedMine
		{
			get
			{
				return _conflict.mine.lastModified.GetValueOrDefault();
			}
		}

		public DateTime lastModifiedTheirs
		{
			get
			{
				return _conflict.theirs.lastModified.GetValueOrDefault();
			}
		}

		public string sourceMine
		{
			get
			{
				return _conflict.mine.sourceDescription;
			}
		}

		public string sourceTheirs
		{
			get
			{
				return _conflict.theirs.sourceDescription;
			}
		}

		public CloudSaveConflictResolver(ICloudConflict conflict, bool shouldAttemptAutoResolution = true)
		{
			_conflict = conflict;
			_localData = new UserData(_conflict.mine.userData);
			_cloudData = new UserData(_conflict.theirs.userData);
			DetermineRecommendation();
			if (shouldAttemptAutoResolution)
			{
				AttemptAutoResolution();
			}
		}

		public void RefreshLocalUserData()
		{
			_localData = new UserData(CloudSave.MakeUserData());
			DetermineRecommendation();
		}

		private void AttemptAutoResolution()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(2);
			dictionary.Add("Source", "AutoResolved");
			Dictionary<string, string> dictionary2 = dictionary;
			if (_cloudData.StrictlyBetterThan(_localData))
			{
				ResolveToTheirs();
				dictionary2.Add("AutoResolved", "UsingTheirs");
				Flurry.LogEventWithSeveralParameters("CloudSave Conflict Resolved", dictionary2);
			}
			else if (_localData.StrictlyBetterThan(_cloudData))
			{
				ResolveToMine();
				dictionary2.Add("AutoResolved", "UsingMine");
				Flurry.LogEventWithSeveralParameters("CloudSave Conflict Resolved", dictionary2);
			}
		}

		public void ResolveToMine()
		{
			UserSave.cloudSaveEnabledByPeekRoutine = false;
			SaveManager.Save();
			_conflict.ResolveUsingMine();
		}

		public void ResolveToTheirs()
		{
			UserSave.cloudSaveEnabledByPeekRoutine = false;
			SaveManager.Save();
			_conflict.ResolveUsingTheirs();
		}

		public List<CloudSaveComparisonRow> GetComparisonRows()
		{
			List<CloudSaveComparisonRow> list = new List<CloudSaveComparisonRow>();
			list.Add(new CloudSaveComparisonRow(CloudSaveComparisonElement.Multiplier, _localData.multiplier, _cloudData.multiplier));
			list.Add(new CloudSaveComparisonRow(CloudSaveComparisonElement.Highscore, _localData.highScore, _cloudData.highScore));
			list.Add(new CloudSaveComparisonRow(CloudSaveComparisonElement.Keys, _localData.keys, _cloudData.keys));
			list.Add(new CloudSaveComparisonRow(CloudSaveComparisonElement.Upgrades, _localData.upgradeTiers, _cloudData.upgradeTiers));
			list.Add(new CloudSaveComparisonRow(CloudSaveComparisonElement.Awards, _localData.awardsPercentage, _cloudData.awardsPercentage));
			list.Add(new CloudSaveComparisonRow(CloudSaveComparisonElement.Characters, _localData.characters, _cloudData.characters));
			list.Add(new CloudSaveComparisonRow(CloudSaveComparisonElement.Boards, _localData.boards + 1, _cloudData.boards + 1));
			list.Add(new CloudSaveComparisonRow(CloudSaveComparisonElement.Outfits, _localData.outfits, _cloudData.outfits));
			list.Add(new CloudSaveComparisonRow(CloudSaveComparisonElement.BoardThemes, _localData.boardThemes, _cloudData.boardThemes));
			return list;
		}

		private void DetermineRecommendation()
		{
			float num = 0f;
			num += Evaluate(_localData.multiplier, _cloudData.multiplier, 1f);
			num += Evaluate(_localData.highScore, _cloudData.highScore, 2E-05f);
			num += Evaluate(_localData.keys, _cloudData.keys, 0.05f);
			num += Evaluate(_localData.upgradeTiers, _cloudData.upgradeTiers, 3f);
			num += Evaluate(_localData.awardsPercentage, _cloudData.awardsPercentage, 1f);
			num += Evaluate(_localData.characters, _cloudData.characters, 10f);
			num += Evaluate(_localData.boards, _cloudData.boards, 8f);
			num += Evaluate(_localData.outfits, _cloudData.outfits, 5f);
			num += Evaluate(_localData.boardThemes, _cloudData.boardThemes, 5f);
			recommendationEvaluation = num;
		}

		private static float Evaluate(float localValue, float cloudValue, float factor)
		{
			return localValue * factor - cloudValue * factor;
		}

		[Conditional("CLOUDSAVE_LOGS")]
		private static void Log(string logMessage)
		{
		}
	}
}
