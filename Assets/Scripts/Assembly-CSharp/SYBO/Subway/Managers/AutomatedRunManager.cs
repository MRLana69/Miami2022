using SYBO.Components.Utilities;
using UnityEngine;

namespace SYBO.Subway.Managers
{
	[PrefabPath("Prefabs/Managers/AutomatedRunManager")]
	public class AutomatedRunManager : SingletonMonoBehaviour<AutomatedRunManager>
	{
		public const string AutomatedRunFinishedFileName = "sybo_automated_run_finished.txt";

		[Range(1f, 1000f)]
		[SerializeField]
		private int _gameAutoStartCount;

		[SerializeField]
		private int _gameAutoRestartDelay;

		[SerializeField]
		private int _gameAutoReviveCount;

		[SerializeField]
		private float _hitsEnabledDelayedSeconds;

		[Header("Benchmarking")]
		[SerializeField]
		private bool _enableBenchmarking;

		[SerializeField]
		private int _numberOfFakeEvents;

		[SerializeField]
		private float _flushDelay;

		[Header("Event testing")]
		[SerializeField]
		private bool _editorEnabledEventTesting;
	}
}
