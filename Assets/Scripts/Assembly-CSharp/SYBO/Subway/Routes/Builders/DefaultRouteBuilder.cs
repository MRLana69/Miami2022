using System;
using System.Collections.Generic;
using System.Linq;
using Data.Save;
using SYBO.Common.Utilities;
using SYBO.Subway.Level;
using UnityEngine;

namespace SYBO.Subway.Routes.Builders
{
	[Serializable]
	public class DefaultRouteBuilder : RouteBuilder
	{
		public static readonly int ChunkLengthForcingEnvironmentSwitch = 540;

		[SerializeField]
		private RouteSection _tutorialSection;

		[SerializeField]
		private EnvironmentKindRepeatConfig[] _environmentRepeatSettings;

		private SemiRandomlyOrderedList<EnvironmentKindRepeatConfig> _randomizedEnvironmentRepeatConfigs = new SemiRandomlyOrderedList<EnvironmentKindRepeatConfig>();

		[SerializeField]
		private RouteSection _jetpackEndSection;

		private EnvironmentKindRepeatConfig _currentEnvironmentRepeatConfig;

		private int _desiredEnvironmentChangeDistance;

		public RouteSection TutorialSection
		{
			get
			{
				return _tutorialSection;
			}
		}

		public RouteSection JetpackEndSection
		{
			get
			{
				return _jetpackEndSection;
			}
		}

		public override void Initialize(RouteManager routeManager, Route route, SectionSpawnMode sectionSpawnMode, ChunkBuildMode chunkBuildMode, RouteType previousRouteType, SyboRandom randomGenerator)
		{
			base.Initialize(routeManager, route, sectionSpawnMode, chunkBuildMode, previousRouteType, randomGenerator);
			_randomizedEnvironmentRepeatConfigs.Build(_environmentRepeatSettings, base.RandomGenerator);
			_currentEnvironmentRepeatConfig = _randomizedEnvironmentRepeatConfigs.GetNext(base.RandomGenerator);
			if (_currentEnvironmentRepeatConfig != null)
			{
				_desiredEnvironmentChangeDistance = _currentEnvironmentRepeatConfig.RepeatDistance;
				DesiredEnvironmentKind = _currentEnvironmentRepeatConfig.EnvironmentKind;
			}
		}

		public override void Destruct()
		{
			base.Destruct();
		}

		public override bool WillChangeEnvironment(int addedDistance)
		{
			return base.DistanceBuilt + addedDistance >= _desiredEnvironmentChangeDistance;
		}

		protected override EnvironmentKind EvalNextChunkDesiredEnvironment(RouteChunk spawnedChunk)
		{
			EnvironmentKind environmentKind = ((_currentEnvironmentRepeatConfig == null) ? EnvironmentKind.None : _currentEnvironmentRepeatConfig.EnvironmentKind);
			EnvironmentKind result = environmentKind;
			IConstrainingRouteBuilder builderConstrainingStartAt = GetBuilderConstrainingStartAt(base.BuiltLeadingEdgeGamePositionZ);
			if (builderConstrainingStartAt != null)
			{
				RouteConstraint constraint = builderConstrainingStartAt.GetConstraint(base.Route.Type);
				_randomizedEnvironmentRepeatConfigs.Reset(base.RandomGenerator);
				EnvironmentKindRepeatConfig nextMatching = _randomizedEnvironmentRepeatConfigs.GetNextMatching(base.RandomGenerator, (EnvironmentKindRepeatConfig envRepeatConfig) => constraint.AllowedEnvironmentKinds.Contains(envRepeatConfig.EnvironmentKind));
				if (nextMatching != null && nextMatching != _currentEnvironmentRepeatConfig)
				{
					result = nextMatching.EnvironmentKind;
					_desiredEnvironmentChangeDistance = base.DistanceBuilt + _currentEnvironmentRepeatConfig.RepeatDistance;
					_currentEnvironmentRepeatConfig = nextMatching;
				}
				else if (nextMatching == null)
				{
					int num = base.RandomGenerator.Range(0, constraint.AllowedEnvironmentKinds.Length - 1);
					result = constraint.AllowedEnvironmentKinds[num];
				}
			}
			else
			{
				bool flag = 0 < _randomizedEnvironmentRepeatConfigs.AllEntries.Count;
				bool flag2 = _currentEnvironmentRepeatConfig != null && _currentEnvironmentRepeatConfig.EnvironmentKind != EnvironmentKind.None;
				bool flag3 = _randomizedEnvironmentRepeatConfigs.AllEntries.Any((EnvironmentKindRepeatConfig ec) => spawnedChunk.SupportsEnvironment(ec.EnvironmentKind));
				bool flag4 = _desiredEnvironmentChangeDistance <= base.DistanceBuilt;
				bool flag5 = environmentKind != EnvironmentKind.None && spawnedChunk.SupportsEnvironment(environmentKind);
				bool flag6 = ChunkLengthForcingEnvironmentSwitch <= spawnedChunk.Length && !flag5;
				bool flag7 = flag && flag3 && (flag4 || !flag2 || flag6);
				bool flag8 = !flag7 && flag2 && flag5;
				bool flag9 = 0 < spawnedChunk.LimitedAllowedEnvironmentKinds.Count;
				if (flag7)
				{
					EnvironmentKindRepeatConfig environmentKindRepeatConfig = null;
					if (spawnedChunk.LimitedAllowedEnvironmentKinds.Count == 0)
					{
						environmentKindRepeatConfig = _randomizedEnvironmentRepeatConfigs.GetNext(base.RandomGenerator);
					}
					else
					{
						_randomizedEnvironmentRepeatConfigs.Reset(base.RandomGenerator);
						bool oneEnvironmentAllowed = 1 == spawnedChunk.LimitedAllowedEnvironmentKinds.Count;
						Func<EnvironmentKindRepeatConfig, bool> matchSelector = (EnvironmentKindRepeatConfig envRepeatConfig) =>
						{
							bool flag10 = _currentEnvironmentRepeatConfig == null || _currentEnvironmentRepeatConfig.EnvironmentKind != envRepeatConfig.EnvironmentKind;
							return spawnedChunk.SupportsEnvironment(envRepeatConfig.EnvironmentKind) && (oneEnvironmentAllowed || flag10);
						};
						environmentKindRepeatConfig = _randomizedEnvironmentRepeatConfigs.GetNextMatching(base.RandomGenerator, matchSelector);
					}
					if (environmentKindRepeatConfig != null && environmentKindRepeatConfig != _currentEnvironmentRepeatConfig)
					{
						result = environmentKindRepeatConfig.EnvironmentKind;
						_desiredEnvironmentChangeDistance = base.DistanceBuilt + _currentEnvironmentRepeatConfig.RepeatDistance;
						_currentEnvironmentRepeatConfig = environmentKindRepeatConfig;
					}
					else if (environmentKindRepeatConfig == null)
					{
						result = EnvironmentKind.None;
						SDebug.LogErrorFormat(spawnedChunk, "Couldn't pick environment for chunk {0} at {1}", environmentKind, base.BuiltLeadingEdgeGamePositionZ);
					}
				}
				else if (flag8)
				{
					result = _currentEnvironmentRepeatConfig.EnvironmentKind;
				}
				else if (flag9)
				{
					result = spawnedChunk.GetRandomAllowedEnvironmentKind(base.RandomGenerator);
				}
				else
				{
					int num2 = EnvironmentKind.AllUnconstrained.Length;
					result = EnvironmentKind.AllUnconstrained[base.RandomGenerator.Range(0, num2 - 1)];
				}
			}
			return result;
		}

		public override void ConnectBuilder(RouteBuilder builderToConnect)
		{
			base.ConnectBuilder(builderToConnect);
			ConstrainingRouteBuilder constrainingRouteBuilder = builderToConnect as ConstrainingRouteBuilder;
			if (constrainingRouteBuilder != null)
			{
				constrainingRouteBuilder.OnReachedLengthVisibleFromEntry += ConstrainingRouteBuilder_OnReachedLengthVisibleFromEntry;
				constrainingRouteBuilder.OnCompletedLayingSections += ConstrainingRouteBuilder_OnCompletedLayingSections;
			}
		}

		public override void DisconnectBuilder(RouteBuilder builderToDisconnect)
		{
			base.DisconnectBuilder(builderToDisconnect);
			ConstrainingRouteBuilder constrainingRouteBuilder = builderToDisconnect as ConstrainingRouteBuilder;
			if (constrainingRouteBuilder != null)
			{
				constrainingRouteBuilder.OnReachedLengthVisibleFromEntry -= ConstrainingRouteBuilder_OnReachedLengthVisibleFromEntry;
				constrainingRouteBuilder.OnCompletedLayingSections -= ConstrainingRouteBuilder_OnCompletedLayingSections;
			}
		}

		public override void Validate(Route route)
		{
			base.Validate(route);
			EnvironmentKindRepeatConfig[] environmentRepeatSettings = _environmentRepeatSettings;
			foreach (EnvironmentKindRepeatConfig environmentKindRepeatConfig in environmentRepeatSettings)
			{
				environmentKindRepeatConfig.Validate();
			}
		}

		public void EnqueueJetpackEndSection(int gamePositionZ)
		{
			EnqueueBuildDirective(gamePositionZ, SectionSpawnMode.All, ChunkBuildMode.Normal, true, _jetpackEndSection);
		}

		protected override bool CanSpawnSection(RouteSection section)
		{
			bool flag = base.CanSpawnSection(section);
			if (!flag)
			{
				return flag;
			}
			int spawnedLeadingEdgeGamePositionZ = base.SpawnedLeadingEdgeGamePositionZ;
			if (!IsStartPositionConstrained(spawnedLeadingEdgeGamePositionZ))
			{
				return true;
			}
			IConstrainingRouteBuilder builderConstrainingStartAt = GetBuilderConstrainingStartAt(spawnedLeadingEdgeGamePositionZ);
			if (section.LengthMin != section.LengthMax)
			{
				SDebug.LogErrorFormat(base.Route, "{0} CanSpawnSection() - default route sections should not have varying min/max lengths {1]/{2}. Make sure to not have random number of Mid chunks.", base.Route.name, section.LengthMin, section.LengthMax);
			}
			int num = builderConstrainingStartAt.ConstraintEndZ - spawnedLeadingEdgeGamePositionZ;
			bool flag2 = section.LengthMin <= num;
			RouteConstraint constraint = builderConstrainingStartAt.GetConstraint(base.Route.Type);
			bool flag3 = constraint.AllowedEnvironmentKinds.Any((EnvironmentKind ae) => section.SupportsEnvironment(ae));
			bool flag4 = section.ForcedEnvironmentKinds.All((EnvironmentKind le) => constraint.AllowedEnvironmentKinds.Contains(le));
			return flag2 && flag3 && flag4;
		}

		protected override RouteSection GetTransitionSectionPrefab(List<TransitionSectionInfo> transitionSections, RouteType transitionRouteType)
		{
			if (!OnboardingSave.isTutorialCompleted && _tutorialSection != null)
			{
				return _tutorialSection;
			}
			return base.GetTransitionSectionPrefab(transitionSections, transitionRouteType);
		}

		private void ConstrainingRouteBuilder_OnReachedLengthVisibleFromEntry(object sender, EventArgs args)
		{
			ConstrainingRouteBuilder constrainingRouteBuilder = sender as ConstrainingRouteBuilder;
			constrainingRouteBuilder.OnReachedLengthVisibleFromEntry -= ConstrainingRouteBuilder_OnReachedLengthVisibleFromEntry;
			constrainingRouteBuilder.OnCompletedLayingSections -= ConstrainingRouteBuilder_OnCompletedLayingSections;
			EnqueueBuildDirective(constrainingRouteBuilder.LastPositionVisibleFromEntry, SectionSpawnMode.None, ChunkBuildMode.Normal);
		}

		private void ConstrainingRouteBuilder_OnCompletedLayingSections(object sender, EventArgs args)
		{
			ConstrainingRouteBuilder constrainingRouteBuilder = sender as ConstrainingRouteBuilder;
			constrainingRouteBuilder.OnReachedLengthVisibleFromEntry -= ConstrainingRouteBuilder_OnReachedLengthVisibleFromEntry;
			constrainingRouteBuilder.OnCompletedLayingSections -= ConstrainingRouteBuilder_OnCompletedLayingSections;
			EnqueueBuildDirective(constrainingRouteBuilder.SpawnedLeadingEdgeGamePositionZ, SectionSpawnMode.None, ChunkBuildMode.Normal);
		}
	}
}
