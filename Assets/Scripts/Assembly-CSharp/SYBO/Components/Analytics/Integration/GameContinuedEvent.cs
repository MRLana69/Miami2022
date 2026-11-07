using SYBO.Components.Json;
using UnityEngine;

namespace SYBO.Components.Analytics.Integration
{
	public class GameContinuedEvent : SubwayBaseSessionEvent
	{
		[SerializeField]
		[SerializedAs("game_no")]
		public int GameID;

		[SerializeField]
		[SerializedAs("auto_revive")]
		public int AutoRevive;

		[SerializeField]
		[SerializedAs("revive_with_ads")]
		public int RevivedWithAdds;

		[SerializeField]
		[SerializedAs("ads_was_available")]
		public int AdsAvailable;

		[SerializeField]
		[SerializedAs("ads_failed")]
		public int AdsFailed;

		public override string EventName
		{
			get
			{
				return "game_continued";
			}
		}

		public GameContinuedEvent(int gameIdInSession, bool autoRevived, bool revivedWithAds, bool adsWasAvailable, bool adsFailed)
		{
			GameID = gameIdInSession;
			AutoRevive = (autoRevived ? 1 : 0);
			RevivedWithAdds = (revivedWithAds ? 1 : 0);
			AdsAvailable = (adsWasAvailable ? 1 : 0);
			AdsFailed = (adsFailed ? 1 : 0);
		}
	}
}
