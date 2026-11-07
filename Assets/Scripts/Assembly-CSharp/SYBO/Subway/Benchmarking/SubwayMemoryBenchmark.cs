using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SYBO.Common.Automation;
using SYBO.Common.Automation.Steps;
using SYBO.Common.Benchmarking;
using SYBO.Subway.Automation.Samplers;
using SYBO.Subway.Automation.Steps;
using UI.Screens;
using UI.Screens.RewardManagement;
using UnityEngine;

namespace SYBO.Subway.Benchmarking
{
	public class SubwayMemoryBenchmark : SubwayBenchmark
	{
		private const int CycleCount = 3;

		private const int UIRepeatCount = 10;

		private const float DurationSeconds = 180f;

		private const float SampleInterval = 6f;

		private static string[] _popups;

		private static string[] _screens;

		private bool _forceAllCharacters;

		private bool _forceAllBoards;

		public override bool RequiresConfirmation
		{
			get
			{
				return true;
			}
		}

		public override string ConfirmationMessage
		{
			get
			{
				return "This benchmark will take around 30 minutes to complete. Please ensure that the device is connected to a charger during this test.";
			}
		}

		public SubwayMemoryBenchmark(bool forceAllCharacters, bool forceAllBoards)
			: base(GenerateCompleteMemoryTestSteps(3, 10), new IBenchmarkMetric[1]
			{
				new MemoryMetric()
			}, new IMetricSampler[2]
			{
				new InGameIntervalSampler(6f),
				new ScreenOpenedSampler()
			})
		{
			_forceAllCharacters = forceAllCharacters;
			_forceAllBoards = forceAllBoards;
			int num = Mathf.RoundToInt(30f);
			int num2 = (_popups.Length + _screens.Length) * 10;
			_initialSampleCount = 3 * (num + num2);
		}

		public override void Initialize()
		{
			Globals.DEBUG_DISPLAY_ALL_CHARS = _forceAllCharacters;
			CharacterScreen.forceRefreshOnCharacterScreen = true;
			Globals.DEBUG_DISPLAY_ALL_BOARDS = _forceAllBoards;
			BoardScreenManager.Instance.forceRefreshOnBoardScreen = true;
			if (!Globals.DEBUG_USE_CELEBRATION_TESTER && RewardManager.rewardsToUnlockCount <= 0)
			{
				CelebrationReward celebrationReward = new CelebrationReward();
				celebrationReward.CelebrationRewardOrigin = CelebrationRewardOrigin.MysteryBox;
				celebrationReward.amount = 1;
				celebrationReward.rewardType = CelebrationRewardType.coins;
				RewardManager.AddRewardToUnlock(celebrationReward);
			}
			base.Initialize();
		}

		private static void CachePopupsAndScreens()
		{
			Type typeFromHandle = typeof(UIScreens);
			IEnumerable<FieldInfo> source = from x in typeFromHandle.GetFields(BindingFlags.Static | BindingFlags.Public)
				where (x.Attributes & FieldAttributes.Literal) != 0
				where x.GetCustomAttributes(typeof(MemoryBenchmarkAttribute), false).Length > 0
				select x;
			string[] source2 = source.Select((FieldInfo x) => x.GetValue(null)).Cast<string>().ToArray();
			_popups = source2.Where((string x) => UIScreens.POPUPS.Contains(x)).ToArray();
			_screens = source2.Where((string x) => UIScreens.SCREENS.Contains(x)).ToArray();
		}

		private static IAutomationStep[] GenerateCompleteMemoryTestSteps(int totalRepeatCount, int uiRepeatCount)
		{
			CachePopupsAndScreens();
			List<IAutomationStep> list = new List<IAutomationStep>((uiRepeatCount + 1) * totalRepeatCount);
			for (int i = 0; i < totalRepeatCount; i++)
			{
				list.Add(GenerateUICompoundSteps(uiRepeatCount));
				list.Add(new StartGameAndWaitStep(180f));
			}
			return list.ToArray();
		}

		private static IAutomationStep GenerateUICompoundSteps(int count)
		{
			IAutomationStep[] array = new IAutomationStep[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = GenerateSingleUICompoundStep();
			}
			return new CompoundAutomationStep(array);
		}

		private static CompoundAutomationStep GenerateSingleUICompoundStep()
		{
			IAutomationStep[] array = new IAutomationStep[_popups.Length + _screens.Length];
			for (int i = 0; i < _popups.Length; i++)
			{
				array[i] = new OpenCloseUIElementStep(OpenCloseUIElementStep.UIElementType.Popup, _popups[i]);
			}
			for (int j = 0; j < _screens.Length; j++)
			{
				array[_popups.Length + j] = new OpenCloseUIElementStep(OpenCloseUIElementStep.UIElementType.Screen, _screens[j]);
			}
			return new CompoundAutomationStep(array);
		}
	}
}
