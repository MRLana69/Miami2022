using UnityEngine;

namespace SYBO.Common.Components
{
	public class SetActiveComponent : MonoBehaviour
	{
		[SerializeField]
		private bool _toggleActiveValue;

		[SerializeField]
		private Timing _toggleActiveTiming = Timing.Start;

		protected void Awake()
		{
			CheckTiming(Timing.Awake);
		}

		protected void Start()
		{
			CheckTiming(Timing.Start);
		}

		protected void CheckTiming(Timing currentUnityTiming)
		{
			if (currentUnityTiming == _toggleActiveTiming)
			{
				base.gameObject.SetActive(_toggleActiveValue);
			}
		}
	}
}
