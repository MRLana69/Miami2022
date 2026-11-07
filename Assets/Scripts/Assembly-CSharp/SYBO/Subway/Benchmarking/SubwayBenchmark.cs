using System.Collections;
using Kiloo.Common.Utility;
using SYBO.Common.Automation;
using SYBO.Common.Benchmarking;
using SYBO.Components.Utilities;
using SYBO.Subway.Benchmarking.Processors;
using SYBO.Subway.Routes;
using UnityEngine;

namespace SYBO.Subway.Benchmarking
{
	public abstract class SubwayBenchmark : Benchmark
	{
		private int _originalScreenTimeout;

		protected int _initialSampleCount;

		protected override int InitialSampleCount
		{
			get
			{
				return _initialSampleCount;
			}
		}

		public SubwayBenchmark(IAutomationStep[] steps, IBenchmarkMetric[] metrics, IMetricSampler[] samplers)
			: base(steps, metrics, samplers)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Physics.IgnoreLayerCollision(Layers.Instance.Character, Layers.Instance.Hit, true);
			Physics.IgnoreLayerCollision(Layers.Instance.Character, Layers.Instance.Pickup, true);
			_originalScreenTimeout = Screen.sleepTimeout;
			Screen.sleepTimeout = -1;
			RouteManager.UseBenchmarkRoute = true;
			SingletonMonoBehaviour<RouteManager>.Instance.Reset();
			UIScreenController.Instance.ClosePopup();
			UIScreenController.Instance.ClosePopup();
		}

		public override void Terminate()
		{
			base.Terminate();
			UIScreenController.Instance.PushScreen("PauseUI");
			RouteManager.UseBenchmarkRoute = false;
			Physics.IgnoreLayerCollision(Layers.Instance.Character, Layers.Instance.Hit, false);
			Physics.IgnoreLayerCollision(Layers.Instance.Character, Layers.Instance.Pickup, false);
			Screen.sleepTimeout = _originalScreenTimeout;
			CoroutineUtil.StartCoroutine(ReturnToHomeScreen());
		}

		private IEnumerator ReturnToHomeScreen()
		{
			yield return null;
			UIScreenController.Instance.GoToMainMenuFromGame(null);
			UIScreenController.Instance.ClosePopup();
		}

		protected override IBenchmarkProcessor CreateProcessor()
		{
			return new GoogleSheetsProcessor(SubwayBenchmarkContainer.SelectedBencharmProcessorPostfix);
		}
	}
}
