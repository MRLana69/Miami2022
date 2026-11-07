using System;
using SYBO.Common.Utility;

namespace SYBO.Common.Benchmarking.Utility
{
	public class FirstMonobehaviour : SubscribableMonobehaviour
	{
		public Action OnOnGUI;

		public Action<bool> OnOnApplicationPause;

		private void Awake()
		{
			base.useGUILayout = false;
		}

		private void OnGUI()
		{
			if (OnOnGUI != null)
			{
				OnOnGUI();
			}
		}

		private void OnApplicationPause(bool pause)
		{
			if (OnOnApplicationPause != null)
			{
				OnOnApplicationPause(pause);
			}
		}
	}
}
