using SYBO.Subway.Level.TrackEvents;
using UnityEngine;

namespace SYBO.Subway.Placeholders.Lifecycles
{
	public class TrackEventPlaceholderLifecycle : PlaceholderLifecycle
	{
		[SerializeField]
		private TrackEvent _trackEvent;

		protected override void Initialize()
		{
			base.Initialize();
			if (_trackEvent != null)
			{
				_trackEvent.Launching += TrackEvent_Launching;
				_trackEvent.Stopped += TrackEvent_Stopped;
			}
		}

		private void OnDestroy()
		{
			if (_trackEvent != null)
			{
				_trackEvent.Launching -= TrackEvent_Launching;
				_trackEvent.Stopped -= TrackEvent_Stopped;
			}
		}

		private void TrackEvent_Launching()
		{
			Spawn();
		}

		private void TrackEvent_Stopped()
		{
			Despawn();
		}
	}
}
