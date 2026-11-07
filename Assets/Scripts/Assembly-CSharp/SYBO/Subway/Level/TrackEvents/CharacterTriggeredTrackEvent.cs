using System;
using UnityEngine;

namespace SYBO.Subway.Level.TrackEvents
{
	[RequireComponent(typeof(TrackObject))]
	public abstract class CharacterTriggeredTrackEvent : TrackEvent
	{
		[SerializeField]
		protected CharacterPassTrigger _trigger;

		public TrackEventTrigger Trigger
		{
			get
			{
				return _trigger;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			_transform = base.transform;
			_trigger.Triggered += CharacterPassTrigger_Triggered;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (_trigger != null)
			{
				_trigger.Triggered -= CharacterPassTrigger_Triggered;
			}
		}

		private void CharacterPassTrigger_Triggered(object sender, EventArgs e)
		{
			if (!_launched)
			{
				EvaluateAndLaunchEvent();
			}
		}
	}
}
