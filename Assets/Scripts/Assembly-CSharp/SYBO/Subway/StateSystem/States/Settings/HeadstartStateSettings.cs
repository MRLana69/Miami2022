using SYBO.Subway.Pickups;
using UnityEngine;

namespace SYBO.Subway.StateSystem.States.Settings
{
	public class HeadstartStateSettings : ScriptableSingleton<HeadstartStateSettings>
	{
		[SerializeField]
		private float _turboHeadstart1FlyHeight = 105f;

		[SerializeField]
		private float _turboHeadstart2FlyHeight = 120f;

		[SerializeField]
		private PickupType[] _pickupTypes;

		[SerializeField]
		private float _megaHeadstartFOVIncrease = 18f;

		[SerializeField]
		private float _fovTransitionTime = 1f;

		public float TurboHeadstart1FlyHeight
		{
			get
			{
				return _turboHeadstart1FlyHeight;
			}
		}

		public float TurboHeadstart2FlyHeight
		{
			get
			{
				return _turboHeadstart2FlyHeight;
			}
		}

		public PickupType[] PickupTypes
		{
			get
			{
				return _pickupTypes;
			}
		}

		public float MegaHeadstartFOVIncrease
		{
			get
			{
				return _megaHeadstartFOVIncrease;
			}
		}

		public float FovTransitionTime
		{
			get
			{
				return _fovTransitionTime;
			}
		}
	}
}
